using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using WikiInsightUsingOpenAI;
using WikiInsightUsingOpenAI.Services;

var top = 3;
var builder = WebApplication.CreateBuilder(args);
Startup.ConfigureServices(builder);
var app = builder.Build();

// var indexingService = app.Services.GetRequiredService<IndexingService>();
// await indexingService.BuildArticleIndex(SourceData.LandmarkNames);

// var indexingService = app.Services.GetRequiredService<IndexingService>();
// await indexingService.BuildFullArticleIndex(SourceData.LandmarkNames);

app.UseCors("FrontendCors");

// GET / api/search?query=...
app.MapGet("/api/search", async (string query, [FromServices] VectorSearchService vectorSearchService) =>
{
    var results = await vectorSearchService.FindTopKArticles(query, top);
    return Results.Ok(results);
});

app.MapGet("/api/searchfullarticle", async (string query, [FromServices] VectorSearchService vectorSearchService) =>
{
    var results = await vectorSearchService.FindTopKArticleChunks(query, top);
    return Results.Ok(results);
});

app.MapGet("/api/ask", async (string question, [FromServices] RagQuestionAnswerService ragQuestionAnswerService) =>
{
    var result = await ragQuestionAnswerService.AnswerQuestion(question);
    return Results.Ok(result);
});

app.MapGet("/api/askwithHYDE", async (string question, [FromServices] RagQuestionAnswerService ragQuestionAnswerService) =>
{
    var result = await ragQuestionAnswerService.AnswerQuestionUsingHYDE(question);
    return Results.Ok(result);
});

app.MapGet("/api/askaboutyoutube", async (string question, [FromServices] RagQuestionAnswerService ragQuestionAnswerService) =>
{
    var result = await ragQuestionAnswerService.AnswerQuestionAboutYoutube(question);
    return Results.Ok(result);
});


app.MapPost("/chat", async (
    List<ChatMessage> messages,
    IChatClient client,
    ChatOptions chatOptions) =>
{
    var withSystemPrompt = (new[] { new ChatMessage(ChatRole.System, PromptService.Prompts["ChatBotSystemPrompt"]) })
                            .Concat(messages)
                            .ToList();

    var response = await client.GetResponseAsync(withSystemPrompt, chatOptions);
    return Results.Ok(response.Messages);
});

app.Run();