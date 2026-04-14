using Microsoft.Data.Sqlite;

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
}