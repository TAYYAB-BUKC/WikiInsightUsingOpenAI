using System;
using Microsoft.Extensions.AI;

namespace WikiInsightUsingOpenAI.Services;

public class RagQuestionAnswerService(VectorSearchService vectorSearchService, IChatClient chatClient, ChatOptions chatOptions, VectorSearchServiceWithHyDE vectorSearchServiceWithHyDE)
{
    public async Task<string> AnswerQuestion(string question)
    {
        var searchedResults = await vectorSearchService.FindTopKArticleChunks(question, 5);

        var systemPrompt = PromptService.Prompts["RAG_SystemPrompt"];

        var userPrompt = $@"User Question: {question}

        Retrieved Article Sections: {String.Join("\n\n", searchedResults.Select(t => @$"Title: {t.Title} 
            Section: {t.Section})
            Part: {t.ChunkIndex + 1}
            Chunk Content: {t.Content}
            URL: {t.SourcePageUrl}"))}";


        var messages = (new[] {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, userPrompt)
        }).ToList();

        var response = await chatClient.GetResponseAsync(messages, chatOptions);

        return response.Text;
    }

    public async Task<string> AnswerQuestionUsingHYDE(string question)
    {
        var searchedResults = await vectorSearchServiceWithHyDE.FindTopKArticleChunks(question, 5);

        var systemPrompt = PromptService.Prompts["RAG_SystemPrompt"];

        var userPrompt = $@"User Question: {question}

        Retrieved Article Sections: {String.Join("\n\n", searchedResults.Select(t => @$"Title: {t.Title} 
            Section: {t.Section})
            Part: {t.ChunkIndex + 1}
            Chunk Content: {t.Content}
            URL: {t.SourcePageUrl}"))}";


        var messages = (new[] {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, userPrompt)
        }).ToList();

        var response = await chatClient.GetResponseAsync(messages, chatOptions);

        return response.Text;
    }
}
