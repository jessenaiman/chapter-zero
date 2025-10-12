/// <summary>
/// Represents a choice in a dialogue.
/// </summary>
public class DialogueChoice
{
    /// <summary>
    /// The text displayed for this choice.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// The response or consequence of selecting this choice.
    /// </summary>
    public string Response { get; set; }

    /// <summary>
    /// Whether this choice is available.
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// The next dialogue node to go to when this choice is selected.
    /// </summary>
    public string NextNodeId { get; set; }

    /// <summary>
    /// Creates a new dialogue choice.
    /// </summary>
    /// <param name="text">The choice text</param>
    /// <param name="response">The response text</param>
    /// <param name="nextNodeId">The next node ID</param>
    public DialogueChoice(string text, string response, string nextNodeId)
    {
        Text = text;
        Response = response;
        NextNodeId = nextNodeId;
    }
}
