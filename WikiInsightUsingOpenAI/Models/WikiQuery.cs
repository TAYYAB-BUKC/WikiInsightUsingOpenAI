using System;
using System.Text.Json.Serialization;

namespace WikiInsightUsingOpenAI.Models;

public sealed class WikiQuery
{
    [JsonPropertyName("pages")]
    public List<WikiPage> Pages { get; set; } = [];
}