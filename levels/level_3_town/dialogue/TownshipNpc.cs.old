using Godot;
using System.Collections.ObjectModel;
using OmegaSpiral.Source.Scripts.Common.Dialogue;


namespace OmegaSpiral.Source.Scripts.Stages.Stage4.Dialogue;

/// <summary>
/// Represents an NPC in the liminal township
/// </summary>
public partial class TownshipNpc : Node2D
{
    /// <summary>
    /// Gets or sets the NPC's unique identifier
    /// </summary>
    public string NpcId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the NPC's display name
    /// </summary>
    public new string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the NPC's character type (shopkeeper, townsfolk, etc.)
    /// </summary>
    public string CharacterType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this NPC has liminal awareness (mentions simulation elements)
    /// </summary>
    public bool HasLiminalAwareness { get; set; }

    /// <summary>
    /// Gets or sets the dialogue timeline path for this NPC (uses Dialogic).
    /// Example: "res://addons/dialogic/Example Assets/Timeline_1.tres"
    /// </summary>
    public string? DialogueTimelinePath { get; set; }

    /// <summary>
    /// Gets or sets references to previous visitors/loops (liminal elements)
    /// </summary>
    public Collection<string> LoopReferences { get; set; } = new();

    /// <summary>
    /// Called when the node enters the scene tree
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        // Initialize the NPC with default properties if not set
        if (string.IsNullOrEmpty(NpcId))
        {
            NpcId = Name.ToLower().Replace(" ", "_");
        }
    }

    /// <summary>
    /// Interact with this NPC to start dialogue
    /// </summary>
    public void Interact()
    {
        GD.Print($"Interacting with NPC: {Name} ({NpcId})");

        // Use Dialogic timeline if available
        if (!string.IsNullOrEmpty(DialogueTimelinePath))
        {
            StartDialogueTimeline();
        }
        else
        {
            GD.Print($"No dialogue timeline found for NPC: {Name}");
        }
    }

    /// <summary>
    /// Start the Dialogic dialogue timeline for this NPC
    /// </summary>
    private void StartDialogueTimeline()
    {
        GD.Print($"Starting Dialogic dialogue with {Name} using timeline: {DialogueTimelinePath}");

        // In a real implementation, this would:
        // 1. Load the Dialogic timeline
        // 2. Set character name and other variables
        // 3. Start the dialogue presentation
        // 4. Handle player responses
        // 5. Apply any game state changes based on choices

        // TODO: Integrate with Dialogic API to start timeline
        // Example: Dialogic.Start(DialogueTimelinePath)
    }

    /// <summary>
    /// Apply effects based on the player's dialogue choice
    /// </summary>
    /// <param name="choice">The selected choice</param>
    private void ApplyChoiceEffects(IDialogueChoice choice)
    {
        // In a real implementation, this would:
        // - Update GameState values
        // - Change NPC relationships
        // - Unlock quest elements
        // - Trigger events

        GD.Print($"Applying effects for choice: {choice.Id}");
    }

    /// <summary>
    /// End the dialogue sequence
    /// </summary>
    private void EndDialogue()
    {
        GD.Print($"Ending dialogue with {Name}");

        // In a real implementation, this would:
        // - Hide the dialogue Ui
        // - Resume player control
        // - Trigger any post-dialogue events
    }
}
