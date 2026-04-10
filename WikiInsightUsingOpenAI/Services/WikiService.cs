using System.Net;
using System.Text.Json;
using WikiInsightUsingOpenAI.Models;

namespace WikiInsightUsingOpenAI.Services;

public class WikiService
{
    private readonly HttpClient client;
    private readonly string API_BASE_URL = "https://en.wikipedia.org/w/api.php";
    private readonly string WIKI_PAGEURL = "https://en.wikipedia.org/wiki/";

    private JsonSerializerOptions? jsonOptions = new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true
    };

    public WikiService(IHttpClientFactory factory)
    {
        client = factory.CreateClient("wikiClient");
    }

    string CreateWikipediaUrl(string title, bool full)
    {
        var urlBuilder = new UriBuilder(API_BASE_URL);
        var queryString = new Dictionary<string, string>
        {
            ["action"] = "query",
            ["prop"] = "extracts",
            ["format"] = "json",
            ["formatversion"] = "2",
            ["redirects"] = "1",
            ["explaintext"] = "1",
            ["exsectionformat"] = "wiki",
            ["titles"] = title
        };

        if (!full)
            queryString["exintro"] = "1";

        urlBuilder.Query = string.Join("&", queryString.Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));

        return urlBuilder.ToString();
    }

    async Task<Article> GetWikipediaPage(string url)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<WikiApiResponse>(json, jsonOptions)
                            ?? throw new InvalidOperationException("Failed to deserialize Wikipedia response");

        var firstPage = apiResponse.Query?.Pages?.FirstOrDefault();
        if (firstPage is null || firstPage.Missing is true)
            throw new Exception($"Cou1d not find a Wikipedia page for {url}");

        if (string.IsNullOrWhiteSpace(firstPage.Title) || string.IsNullOrWhiteSpace(firstPage.Extract))
            throw new Exception($"Empty Wikipedia page returned for {url}");

        var title = firstPage.Title!;
        var content = firstPage.Extract!.Trim();
        var id = UtilsService.ToUrlSafeId(title);
        var pageUrl = $"{WIKI_PAGEURL}{Uri.EscapeDataString(title.Replace(' ', '_'))}";

        return new Article(
            Id: id,
            Title: title,
            Content: content,
            PageUrl: pageUrl
        );
    }

    public async Task<Article> GetWikipediaPageForTitle(string title, bool full = false)
    {
        var url = CreateWikipediaUrl(title, full);
        return await GetWikipediaPage(url);
    }
}