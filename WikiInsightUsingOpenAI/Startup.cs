using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Embeddings;
using Pinecone;
using WikiInsightUsingOpenAI.Services;

namespace WikiInsightUsingOpenAI;

public static class Startup
{
    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        var openAIKey = UtilsService.GetEnvironmentVariable("OPENAI_API_KEY");
        var openAIModelName = "text-embedding-3-small";
        var pineconeKey = UtilsService.GetEnvironmentVariable("PINECONE_API_KEY");
        var pineconeIndexName = UtilsService.GetEnvironmentVariable("PINECONE_INDEX_NAME");

        builder.Services.AddSingleton(embeddingGenerator =>
            new EmbeddingClient(
                model: openAIModelName,
                apiKey: openAIKey
            ).AsIEmbeddingGenerator());

        builder.Services.AddSingleton(client =>
            new PineconeClient(pineconeKey).Index(pineconeIndexName));

        builder.Services.AddHttpClient("wikiClient", client =>
        {
            client.DefaultRequestHeaders.UserAgent.Clear();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("WikiInsight", "v1.0"));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("(Contact: write2tayyabarsalan+wikipedia@gmail.com)"));
        });

        builder.Services.AddSingleton<EmbeddingService>();
        builder.Services.AddSingleton<IndexingService>();
        builder.Services.AddSingleton<WikiService>();
        builder.Services.AddSingleton<ArticleStoreService>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("FrontendCors", policy =>
                    policy.WithOrigins(new[] { "http://localhost:3000", "http://127.0.0.1:3000" })
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                );
        });
    }
}