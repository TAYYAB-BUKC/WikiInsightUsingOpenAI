using System.Net;

namespace WikiInsightUsingOpenAI.Services;

public class WikiService
{
    private readonly HttpClient client;
    private readonly string API_BASE_URL = "https://en.wikipedia.org/w/api.php");

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
}