namespace WikiInsightUsingOpenAI.Services;

public static class UtilsService
{
    public static string GetEnvironmentVariable(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrWhiteSpace(value))
            throw new Exception($"Missing Environment Variable: {key}");
        return value!;
    }
}