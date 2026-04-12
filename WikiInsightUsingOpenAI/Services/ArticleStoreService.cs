using Microsoft.Data.Sqlite;

namespace WikiInsightUsingOpenAI.Services;

public class ArticleStoreService
{
    private const string DbFile = "WikiInsight_ContentStore.db";
    static ArticleStoreService()
    {
        using var conn = new SqliteConnection($"Data Source={DbFile}");
        conn.Open();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS TBL_Articles(
                Id TEXT PRIMARY KEY,
                Title TEXT,
                Content TEXT,
                PageUrl TEXT
            }";
    }
}
