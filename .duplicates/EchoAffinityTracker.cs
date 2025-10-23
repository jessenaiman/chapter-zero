// Archived: previous EchoAffinityTracker implementation (Stage 2 specific).
/*
public partial class EchoAffinityTracker : IAffinityTracker
{
    private readonly Dictionary<string, int> scores = new()
    {
        { "light", 0 },
        { "shadow", 0 },
        { "ambition", 0 }
    };

    private readonly List<EchoChoiceRecord> choiceHistory = new();
    private string? claimedDreamweaver;

    public void RecordInterludeChoice(string interludeOwner, string choiceId, string choiceAlignment)
    {
        int points = choiceAlignment == interludeOwner ? 2 : 1;
        if (scores.ContainsKey(choiceAlignment))
        {
            scores[choiceAlignment] += points;
        }

        choiceHistory.Add(new EchoChoiceRecord(
            "interlude",
            interludeOwner,
            choiceId,
            choiceAlignment,
            points));
    }

    public void RecordChamberChoice(string chamberOwner, string objectSlot, string objectAlignment)
    {
        int points = objectAlignment == chamberOwner ? 2 : 1;
        if (scores.ContainsKey(objectAlignment))
        {
            scores[objectAlignment] += points;
        }

        choiceHistory.Add(new EchoChoiceRecord(
            "chamber",
            chamberOwner,
            objectSlot,
            objectAlignment,
            points));
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

    public IReadOnlyDictionary<string, int> GetAllScores()
    {
        return new Dictionary<string, int>(scores);
    }

    public IReadOnlyList<EchoChoiceRecord> GetChoiceHistory()
    {
        return choiceHistory.AsReadOnly();
    }

    public void Reset()
    {
        foreach (string key in scores.Keys.ToList())
        {
            scores[key] = 0;
        }

        choiceHistory.Clear();
        claimedDreamweaver = null;
    }
}
*/
