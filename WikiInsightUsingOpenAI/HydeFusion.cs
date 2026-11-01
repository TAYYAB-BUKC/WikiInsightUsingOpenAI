namespace WikiInsightUsingOpenAI;

public record SearchHit(string Id, double Score);

public static class HydeFusion
{
    public static List<SearchHit> ReciprocalRankFusion(
        IEnumerable<IEnumerable<SearchHit>> rankedLists,
        int topK,
        int k = 60)
    {
        // Accumulate RRF scores by document Id
        var scores = new Dictionary<string, double>();

        foreach (var list in rankedLists)
        {
            // Use the index as the rank (1-based)
            int rank = 1;
            foreach (var hit in list)
            {
                // RRF contribution for this list and rank
                double contrib = 1.0 / (k + rank);

                if (scores.TryGetValue(hit.Id, out var existing))
                    scores[hit.Id] = existing + contrib;
                else
                    scores[hit.Id] = contrib;

                rank++;
            }
        }

        // Sort by total RRF score (desc) and take topK
        return scores
            .OrderByDescending(kvp => kvp.Value)
            .Take(topK)
            .Select(kvp => new SearchHit(kvp.Key, kvp.Value)) // Score holds the fused RRF score
            .ToList();
    }
}