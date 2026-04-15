using Microsoft.Data.Sqlite;
using WikiInsightUsingOpenAI.Models;

namespace WikiInsightUsingOpenAI.Services;

public class ArticleChunkStoreService
{
    private static readonly string DbFile = "WikiInsight_ContentStore.db";
    static ArticleChunkStoreService()
    {
        DbFile = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")), "ContentStore", DbFile);

        using var conn = new SqliteConnection($"Data Source={DbFile}");
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS TBL_ArticleChunks(
                Id TEXT PRIMARY KEY,
                Title TEXT,
                Section TEXT,
                ChunkIndex INTEGER,
                Content TEXT,
                SourcePageUrl TEXT
            )";
        cmd.ExecuteNonQuery();
        conn.Close();
    }

    public List<ArticleChunk> GetArticleChunks(IEnumerable<string> ids)
    {
        var idList = ids?.Distinct().ToList() ?? [];
        if (idList.Count == 0) return [];

        using var conn = new SqliteConnection($"Data Source={DbFile}");
        conn.Open();
        using var cmd = conn.CreateCommand();
        var paramNames = new List<string>(idList.Count);
        for (int i = 0; i < idList.Count; i++)
        {
            var p = "$p" + i;
            paramNames.Add(p);
            cmd.Parameters.AddWithValue(p, idList[i]);
        }

        var orderByCase =
            "CASE Id " +
            string.Join(" ", idList.Select((id, i) => $"WHEN $p{i} THEN {i}")) +
            " END";

        cmd.CommandText = $@"
            SELECT Id, Title, Section, ChunkIndex, Content, SourcePageUrl
            FROM TBL_ArticleChunks
            WHERE Id IN ({string.Join(", ", paramNames)})
            ORDER BY {orderByCase}";

        var articleChunks = new List<ArticleChunk>();
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            articleChunks.Add(new ArticleChunk(
                Id: reader.GetString(0),
                Title: reader.GetString(1),
                Section: reader.GetString(2),
                ChunkIndex: reader.GetInt32(3),
                Content: reader.GetString(4),
                SourcePageUrl: reader.GetString(5)
            ));
        }

        conn.Close();
        return articleChunks;
    }

    public void SaveArticleChunk(ArticleChunk articleChunk)
    {
        using var conn = new SqliteConnection($"Data Source={DbFile}");
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT OR REPLACE INTO TBL_ArticleChunks
            (Id, Title, Section, ChunkIndex, Content, SourcePageUrl)
            VALUES ($id, $title, $section, $chunkIndex, $content, $sourcePageUrl)";
        cmd.Parameters.AddWithValue("$id", articleChunk.Id);
        cmd.Parameters.AddWithValue("$title", articleChunk.Title);
        cmd.Parameters.AddWithValue("$section", articleChunk.Section);
        cmd.Parameters.AddWithValue("$chunkIndex", articleChunk.ChunkIndex);
        cmd.Parameters.AddWithValue("$content", articleChunk.Content);
        cmd.Parameters.AddWithValue("$sourcePageUrl", articleChunk.SourcePageUrl);
        cmd.ExecuteNonQuery();
        conn.Close();
    }
}