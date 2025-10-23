// Archived: previous Stage1AffinityTracker implementation (Stage 1 specific).
/*
public partial class Stage1AffinityTracker : IAffinityTracker
{
    private readonly Dictionary<string, int> scores = new()
    {
        { "light", 0 },
        { "shadow", 0 },
        { "ambition", 0 }
    };

    private readonly List<ChoiceRecord> choiceHistory = new();
    private string? claimedDreamweaver;

    public void RecordChoice(string questionId, string choiceText, int lightPoints, int shadowPoints, int ambitionPoints)
    {
        scores["light"] += lightPoints;
        scores["shadow"] += shadowPoints;
        scores["ambition"] += ambitionPoints;
        choiceHistory.Add(new ChoiceRecord(questionId, choiceText, lightPoints, shadowPoints, ambitionPoints));
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
