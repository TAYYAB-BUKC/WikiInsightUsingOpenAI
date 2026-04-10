using System.Text.Json.Serialization;

namespace WikiInsightUsingOpenAI.Models;

public sealed class WikiPage
{
    [JsonPropertyName("pageid")]
    public long? PageId { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("extract")]
    public string? Extract { get; set; }

    [JsonPropertyName("missing")]
    public bool? Missing { get; set; }
}