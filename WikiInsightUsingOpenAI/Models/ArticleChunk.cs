namespace WikiInsightUsingOpenAI.Models;

public record ArticleChunk(
    string Id,
    string Title,
    string Section,
    int ChunkIndex,
    string Content,
    string SourcePageUrl
);