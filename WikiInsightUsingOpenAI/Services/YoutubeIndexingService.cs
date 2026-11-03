using Pinecone;

namespace WikiInsightUsingOpenAI.Services;

public class YoutubeIndexingService
{
    private readonly IndexClient indexClient;
    private readonly EmbeddingService embeddingService;
    private readonly YoutubeSplitterService youtubeSplitterService;
    private readonly YoutubeChunkStoreService youtubeChunkStoreService;

    public YoutubeIndexingService(IndexClient indexClient, EmbeddingService embeddingService, YoutubeSplitterService youtubeSplitterService, YoutubeChunkStoreService youtubeChunkStoreService)
    {
        this.indexClient = indexClient;
        this.embeddingService = embeddingService;
        this.youtubeSplitterService = youtubeSplitterService;
        this.youtubeChunkStoreService = youtubeChunkStoreService;
    }

    public async Task BuildYoutubeTranscriptIndex()
    {
        var jsonlPath = Path.Combine(AppContext.BaseDirectory, "transcripts.jsonl");
        var transcripts = JsonlReaderService.ReadJsonl(jsonlPath);

        foreach (var transcript in transcripts)
        {
            if (string.IsNullOrWhiteSpace(transcript.Transcript)) continue;

            var chunks = youtubeSplitterService.Chunk(transcript.VideoTitle, transcript.Transcript);

            var stringsToEmbed = chunks.Select(c => $"{c.Title} (part {c.ChunkIndex})\n\n{c.Content}");

            var vectors = await embeddingService.GenerateEmbeddings(stringsToEmbed);

            var pineconeVector = chunks.Select((chunk, index) => new Vector
            {
                Id = chunk.Id,
                Values = vectors[index].Vector.ToArray(),
                Metadata = new Metadata
                {
                    { "title", chunk.Title },
                    { "chunk_index", chunk.ChunkIndex }
                }
            });

            await indexClient.UpsertAsync(new UpsertRequest
            {
                Vectors = pineconeVector
            });

            foreach (var chunk in chunks)
            {
                youtubeChunkStoreService.SaveYoutubeChunk(chunk);
            }
        }
    }
}