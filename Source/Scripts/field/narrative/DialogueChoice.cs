// <copyright file="DialogueChoice.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.Field.Narrative;

/// <summary>
/// Represents a choice in a dialogue.
/// </summary>
public class DialogueChoice
{
    // TODO: duplicate Source/Scripts/field/narrative/NarrativeSceneData.cs - Consider consolidating choice option classes
    /// <summary>
    /// Initializes a new instance of the <see cref="DialogueChoice"/> class.
    /// Creates a new dialogue choice.
    /// </summary>
    /// <param name="text">The choice text.</param>
    /// <param name="response">The response text.</param>
    /// <param name="nextNodeId">The next node ID.</param>
    public DialogueChoice(string text, string response, string nextNodeId)
    {
        this.Text = text;
        this.Response = response;
        this.NextNodeId = nextNodeId;
    }

    /// <summary>
    /// Gets or sets the text displayed for this choice.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the response or consequence of selecting this choice.
    /// </summary>
    public string Response { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether whether this choice is available.
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// Gets or sets the next dialogue node to go to when this choice is selected.
    /// </summary>
    public string NextNodeId { get; set; }
}
