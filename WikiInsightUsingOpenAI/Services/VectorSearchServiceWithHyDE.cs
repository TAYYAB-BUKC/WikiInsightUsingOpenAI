using Microsoft.Extensions.AI;
using Pinecone;
using WikiInsightUsingOpenAI.Models;

namespace WikiInsightUsingOpenAI.Services;

public class VectorSearchServiceWithHyDE(EmbeddingService embeddingService, IndexClient pineconeIndexClient, ArticleStoreService contentStore, ArticleChunkStoreService articleChunkStoreService, IChatClient chatClient, ChatOptions chatOptions)
{
    private async Task<string?> GenerateHypothesisAsync(string question)
    {
        var systemPrompt = "You are a helpful assistant that generates a hypothesis based on the user's question. The hypothesis should be concise, relevant, and provide a potential answer or explanation to the question. Please provide the hypothesis in a 3-6 sentence.";

        var userPrompt = PromptService.Prompts["RAG_SystemPrompt"]?.Replace("{question}", question);

        var messages = (new[] {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, userPrompt)
        }).ToList();

        var response = await chatClient.GetResponseAsync(messages, chatOptions);

        var text = response?.Text?.Trim();

        return string.IsNullOrWhiteSpace(text) ? null : text.Length > 1500 ? text[..1500] : text;
    }

    public async Task<List<ArticleChunk>> FindTopKArticleChunks(string query, int k)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        var hypothesis = await GenerateHypothesisAsync(query);

        var textToEmbed = hypothesis ?? query;

        var embeddings = await embeddingService.GenerateEmbeddings([textToEmbed]);

        if (embeddings.Count == 0)
            return [];

        var response = await pineconeIndexClient.QueryAsync(new QueryRequest()
        {
            TopK = (uint)k,
            Vector = embeddings[0].Vector,
            IncludeMetadata = true
        });

        var matches = (response.Matches ?? []).ToList();

        if (matches.Count == 0)
            return [];

        var ids = matches.Select(m => m.Id!).Where(id => !string.IsNullOrEmpty(id));

        var articles = articleChunkStoreService.GetArticleChunks(ids);

        var scoreById = matches.Where(m => m.Id is not null).ToDictionary(m => m.Id!, m => m.Score);

        var orderedArticles = articles.OrderByDescending(a => scoreById.GetValueOrDefault(a.Id, 0f)).Take(k).ToList();

        return orderedArticles;
    }
}