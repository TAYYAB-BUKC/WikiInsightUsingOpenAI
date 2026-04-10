using Pinecone;

namespace WikiInsightUsingOpenAI.Services;

public class IndexingService
{
    private readonly IndexClient indexClient;
    private readonly WikiService wikiService;
    private readonly EmbeddingService embeddingService;

    public IndexingService(IndexClient indexClient, WikiService wikiService, EmbeddingService embeddingService)
    {
        this.indexClient = indexClient;
        this.wikiService = wikiService;
        this.embeddingService = embeddingService;
    }

    public async Task BuildArticleIndex(string[] pageTitles)
    {
        foreach (var landmark in pageTitles)
        {
            var wikiPage = await wikiService.GetWikipediaPageForTitle(landmark);
            var vectors = await embeddingService.GenerateEmbeddings([wikiPage.Content]);

            var pineconeVector = new Vector
            {
                Id = wikiPage.Id,
                Values = vectors[0].Vector,
                Metadata = new Metadata
                {
                    { "title", wikiPage.Title }
                },
            };

            await indexClient.UpsertAsync(new UpsertRequest
            {
                Vectors = [pineconeVector]
            });
        }
    }
}