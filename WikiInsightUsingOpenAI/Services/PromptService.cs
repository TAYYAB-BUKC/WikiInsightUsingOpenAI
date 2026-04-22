namespace WikiInsightUsingOpenAI.Services;

public class PromptService
{
    public static readonly Dictionary<string, string> Prompts = [];

    static PromptService()
    {
        var promptsDirectory = Path.Combine(AppContext.BaseDirectory, "Prompts");
        foreach (string file in Directory.EnumerateFiles(promptsDirectory, "*.txt"))
        {
            Prompts[Path.GetFileNameWithoutExtension(file)] = File.ReadAllText(file);
        }
    }
}