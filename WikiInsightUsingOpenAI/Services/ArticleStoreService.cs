using Microsoft.Data.Sqlite;
using WikiInsightUsingOpenAI.Models;

namespace WikiInsightUsingOpenAI.Services;

public class ArticleStoreService
{
    private const string DbFile = "WikiInsight_ContentStore.db";
    static ArticleStoreService()
    {
        using var conn = new SqliteConnection($"Data Source={DbFile}");
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS TBL_Articles(
                Id TEXT PRIMARY KEY,
                Title TEXT,
                Content TEXT,
                PageUrl TEXT
            )";
        cmd.ExecuteNonQuery();
        conn.Close();
    }

    public List<Article> GetArticles(IEnumerable<string> ids)
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
            SELECT Id, Title, Content, PageUrl
            FROM TBL_Articles
            WHERE Id IN ({string.Join(", ", paramNames)})
            ORDER BY {orderByCase}";

        var articles = new List<Article>();
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            articles.Add(new Article(
                        Id: reader.GetString(0),
                        Title: reader.GetString(1),
                        Content: reader.GetString(2),
                        PageUrl: reader.GetString(3)
                    ));
        }

        conn.Close();
        return articles;
    }

    public void SaveArticle(Article article)
    {
        using var conn = new SqliteConnection($"Data Source={DbFile}");
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT OR REPLACE INTO TBL_Articles
            (Id, Title, Content, PageUrl)
            VALUES ($id, $title, $content, $pageUrl)";
        cmd.Parameters.AddWithValue("$id", article.Id);
        cmd.Parameters.AddWithValue("$title", article.Title);
        cmd.Parameters.AddWithValue("$content", article.Content);
        cmd.Parameters.AddWithValue("$pageUrl", article.PageUrl);

        cmd.ExecuteNonQuery();
        conn.Close();
    }
}