namespace WikiInsightUsingOpenAI.Models;

public record Article(
    string Id,
    string Title,
    string Content,
    string PageUrl
);