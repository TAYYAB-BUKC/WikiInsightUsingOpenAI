using Pinecone;
using WikiInsightUsingOpenAI.Models;

namespace WikiInsightUsingOpenAI.Services;

public class VectorSearchService(EmbeddingService embeddingService, IndexClient pineconeIndexClient, ArticleStoreService contentStore)
{
    public async Task<List<Article>> FindTopKArticles(string query, int k)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        var embeddings = await embeddingService.GenerateEmbeddings([query]);

        var response = await pineconeIndexClient.QueryAsync(new QueryRequest()
        {
            TopK = 3,
            Vector = embeddings[0].Vector,
            IncludeMetadata = true
        });

        var matches = (response.Matches ?? []).ToList();
        if (matches.Count == 0)
            return [];

        var ids = matches.Select(m => m.Id!).Where(id => !string.IsNullOrEmpty(id));
        var articles = contentStore.GetArticles(ids);

        var scoreById = matches.Where(m => m.Id is not null).ToDictionary(m => m.Id!, m => m.Score);

        var orderedArticles = articles.OrderByDescending(a => scoreById.GetValueOrDefault(a.Id, 0f)).Take(k).ToList();

        return orderedArticles;
    }
}