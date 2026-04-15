namespace WikiInsightUsingOpenAI.Services;

public class ArticleSplitterService(int MaxTokensPerChunk = 300, int OverlapTokens = 60)
{
    /// <summary>
    /// Quick-and-dirty token estimator (~4 chars = 1 Token)
    /// < / summary>
    public static int EstimateTokens(string text) => (int)Math.Ceiling(text.Length / 4.0);
}