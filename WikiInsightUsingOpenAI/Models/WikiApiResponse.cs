using System;
using System.Text.Json.Serialization;

namespace WikiInsightUsingOpenAI.Models;

public sealed class WikiApiResponse
{
    [JsonPropertyName("query")]
    public WikiQuery? Query { get; set; }
}
