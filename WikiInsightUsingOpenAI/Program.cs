using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using WikiInsightUsingOpenAI;
using WikiInsightUsingOpenAI.Services;

var top = 3;
var builder = WebApplication.CreateBuilder(args);
Startup.ConfigureServices(builder);
var app = builder.Build();

// var indexingService = app.Services.GetRequiredService<IndexingService>();
// await indexingService.BuildArticleIndex(SourceData.LandmarkNames);

app.UseCors("FrontendCors");

// GET / api/search?query=...
app.MapGet("/api/search", async (string query, VectorSearchService vectorSearchService) =>
{
    var results = await vectorSearchService.FindTopKArticles(query, top);
    return Results.Ok(results);
});

app.Run();