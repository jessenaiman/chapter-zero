// Archived: previous Stage4AffinityTracker implementation (Stage 4 specific).
/*
public partial class Stage4AffinityTracker : IAffinityTracker
{
    private readonly Dictionary<string, int> scores = new()
    {
        { "light", 0 },
        { "shadow", 0 },
        { "ambition", 0 }
    };

    private string? claimedDreamweaver;

    public void RecordScore(string dreamweaverId, int points)
    {
        if (scores.ContainsKey(dreamweaverId))
        {
            scores[dreamweaverId] += points;
        }
    }

    public IReadOnlyDictionary<string, int> GetAllScores()
    {
        return new Dictionary<string, int>(scores);
    }

    public string DetermineClaim()
    {
        if (claimedDreamweaver != null)
        {
            return claimedDreamweaver;
        }

        claimedDreamweaver = scores
            .OrderByDescending(kvp => kvp.Value)
            .ThenBy(kvp => kvp.Key)
            .First()
            .Key;

        return claimedDreamweaver;
    }
}
*/
