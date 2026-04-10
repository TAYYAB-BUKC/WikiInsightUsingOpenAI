using Microsoft.Extensions.AI;

namespace WikiInsightUsingOpenAI.Services;

public class EmbeddingService(StringEmbeddingGenerator embeddingGenerator)
{
    public async Task<GeneratedEmbeddings<Embedding<float>>> GenerateEmbeddings(IEnumerable<string>? content)
    {
        EmbeddingGenerationOptions embeddingOptions = new()
        {
            Dimensions = 512,
        };

        var embeddings = await embeddingGenerator.GenerateAsync(content, embeddingOptions);

        return embeddings;
    }
}