using Microsoft.SemanticKernel.Text;
using WikiInsight.Models;
using WikiInsightUsingOpenAI.Models;
using WikiInsightUsingOpenAI.Services;

namespace WikiInsight.Service;

public class ArticleSplitterService(int MaxTokensPerChunk = 300, int OverlapTokens = 60)
{
    /// <summary>
    /// Quick-and-dirty token estimator (~4 chars = 1 Token)
    /// < / summary>
    public static int EstimateTokens(string text) => (int)Math.Ceiling(text.Length / 4.0);

    public static List<string> SplitLines(string text, int softWrapChars = 400)
    {
        var raw = text.Replace("\r\n", "\n").Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .Where(t => t.Length > 0);

        List<string> lines = new();
        foreach (var line in raw)
        {
            if (line.Length <= softWrapChars)
            {
                lines.Add(line);
                continue;
            }

            // Soft wrap long lines to ~softWrapChars chunks on word boundaries
            int index = 0;
            while (index < line.Length)
            {
                int remaining = line.Length - index;
                int take = Math.Min(softWrapChars, remaining);

                // Try not to break in the middle of a word
                int end = index + take;
                if (end < line.Length)
                {
                    int lastSpace = line.LastIndexOf(' ', end - 1, take);
                    if (lastSpace > index + softWrapChars / 2) end = lastSpace;
                }

                lines.Add(line.Substring(index, end - index).Trim());
                index = end;
                while (index < line.Length && line[index] == ' ') index++;
            }
        }
        return lines;
    }

    public IEnumerable<ArticleChunk> Chunks(string title, string content, string pageUrl = "", string section = "")
    {
        var lines = SplitLines(content);
        var chunkBodies = TextChunker.SplitPlainTextParagraphs(
            lines: lines,
            maxTokensPerParagraph: MaxTokensPerChunk,
            overlapTokens: OverlapTokens,
            chunkHeader: null,
            tokenCounter: EstimateTokens
        );

        return chunkBodies.Select((chunkContent, index) =>
            new ArticleChunk(
                Id: UtilsService.ToUrlSafeId($"{title}_{section}_{index + 1:D2}"),
                Title: title,
                Section: section,
                ChunkIndex: index + 1,
                Content: chunkContent.Trim(),
                SourcePageUrl: $"{pageUrl}#{UtilsService.ToUrlSafeId(section)}"
            )
        );
    }
}