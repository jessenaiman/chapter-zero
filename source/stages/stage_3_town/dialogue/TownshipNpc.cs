using Godot;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OmegaSpiral.Source.Scripts.Common.Dialogue;
using OmegaSpiral.Source.Scripts.Dialogue;

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
    /// Gets or sets the dialogue data for this NPC
    /// </summary>
    public NpcDialogueData? DialogueData { get; set; }
    
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
        
        // In a real implementation, this would trigger the dialogue system
        if (DialogueData != null)
        {
            StartDialogue();
        }
        else
        {
            GD.Print($"No dialogue data found for NPC: {Name}");
        }
    }
    
    /// <summary>
    /// Start the dialogue sequence with this NPC
    /// </summary>
    private void StartDialogue()
    {
        GD.Print($"Starting dialogue with {Name}");
        
        // In a real implementation, this would:
        // 1. Load the dialogue window/UI
        // 2. Display the opening lines
        // 3. Show choices
        // 4. Handle player responses
        // 5. Apply any game state changes based on choices
        
        DisplayOpeningLines();
    }
    
    /// <summary>
    /// Display the opening lines of dialogue
    /// </summary>
    private void DisplayOpeningLines()
    {
        if (DialogueData?.OpeningLines == null)
            return;
            
        GD.Print($"{Name}: {string.Join("\n" + Name + ": ", DialogueData.OpeningLines)}");
        
        // After opening lines, show choices if available
        if (DialogueData.Choices?.Count > 0)
        {
            DisplayChoices();
        }
    }
    
    /// <summary>
    /// Display the available dialogue choices
    /// </summary>
    private void DisplayChoices()
    {
        GD.Print("Available choices:");
        for (int i = 0; i < DialogueData!.Choices.Count; i++)
        {
            var choice = DialogueData.Choices[i];
            GD.Print($"{i + 1}. {choice.Text} - {choice.Description}");
        }
        
        // In a real implementation, this would wait for player input
        // and then handle the selected choice
        HandleChoiceSelection(0); // Default to first choice for now
    }
    
    /// <summary>
    /// Handle the player's choice selection
    /// </summary>
    /// <param name="choiceIndex">Index of the selected choice</param>
    private void HandleChoiceSelection(int choiceIndex)
    {
        if (choiceIndex < 0 || choiceIndex >= DialogueData!.Choices.Count)
        {
            GD.PrintErr($"Invalid choice index: {choiceIndex}");
            return;
        }
        
        var selectedChoice = DialogueData.Choices[choiceIndex];
        GD.Print($"Player selected: {selectedChoice.Text}");
        
        // Apply any effects based on the choice
        ApplyChoiceEffects(selectedChoice);
        
        // End the dialogue
        EndDialogue();
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
        // - Hide the dialogue UI
        // - Resume player control
        // - Trigger any post-dialogue events
    }
}