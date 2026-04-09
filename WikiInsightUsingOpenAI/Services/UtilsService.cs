using System.Text.RegularExpressions;

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

    public static string ToUrlSafeId(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return string.Empty;

        var text = title!.Trim();
        text = Regex.Replace(text, @"[^\w\-]+", "_");
        text = Regex.Replace(text, "_{2,}", "_");
        text = text.Trim('_');

        if (string.IsNullOrEmpty(text))
            return Uri.EscapeDataString(title);
        return text;
    }
}