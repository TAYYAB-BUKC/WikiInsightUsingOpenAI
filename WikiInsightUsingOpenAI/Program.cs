using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WikiInsightUsingOpenAI;
using WikiInsightUsingOpenAI.Services;

var builder = WebApplication.CreateBuilder(args);
Startup.ConfigureServices(builder);
var app = builder.Build();

// var indexingService = app.Services.GetRequiredService<IndexingService>();
// await indexingService.BuildArticleIndex(SourceData.LandmarkNames);