namespace WikiInsightUsingOpenAI.Services;

public class WikiService
{
    private readonly HttpClient client;
    private readonly string API_BASE_URL = "https://en.wikipedia.org/w/api.php");

    public WikiService(IHttpClientFactory factory)
    {
        client = factory.CreateClient("wikiClient");
    }
}