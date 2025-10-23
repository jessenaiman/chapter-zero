using Godot;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts;

/// <summary>
/// Manages global game state signals that span across multiple stages.
/// These signals enable communication between different parts of the game.
/// </summary>
[GlobalClass]
public partial class GameStateSignals : Node
{
    /// <summary>
    /// Emitted when Dreamweaver affinity scores are updated.
    /// </summary>
    /// <param name="dwType">The Dreamweaver type whose affinity changed.</param>
    /// <param name="change">The change amount applied.</param>
    /// <param name="newScore">The new score after the change.</param>
    [Signal]
    public delegate void AffinityScoreUpdatedEventHandler(int dwType, int change, int newScore);

    /// <summary>
    /// Emitted when player reaches a major progression milestone.
    /// </summary>
    /// <param name="milestoneId">The identifier of the milestone reached.</param>
    /// <param name="progressData">Additional data about the progress.</param>
    [Signal]
    public delegate void PlayerProgressionChangedEventHandler(string milestoneId, Godot.Collections.Dictionary<string, Variant> progressData);

    /// <summary>
    /// Emitted when items are added to, removed from, or used in the inventory.
    /// </summary>
    /// <param name="itemId">The ID of the item affected.</param>
    /// <param name="action">The action performed (added, removed, used).</param>
    /// <param name="quantity">The quantity of the item affected.</param>
    [Signal]
    public delegate void InventoryUpdatedEventHandler(string itemId, string action, int quantity);

    /// <summary>
    /// Emitted when character party composition changes.
    /// </summary>
    /// <param name="action">The action performed (added, removed, swapped).</param>
    /// <param name="characterId">The ID of the affected character.</param>
    /// <param name="partyData">Data about the current party composition.</param>
    [Signal]
    public delegate void CharacterPartyChangedEventHandler(string action, string characterId, Godot.Collections.Dictionary<string, Variant> partyData);

    /// <summary>
    /// Emitted when dominant Dreamweaver influence changes.
    /// </summary>
    /// <param name="newDominant">The new dominant Dreamweaver type.</param>
    /// <param name="influenceLevel">The level of influence.</param>
    [Signal]
    public delegate void DreamweaverInfluenceChangedEventHandler(int newDominant, int influenceLevel);

    /// <inheritdoc/>
    public override void _Ready()
    {
        GD.Print("GameState Signals initialized");
    }

    /// <summary>
    /// Emits the AffinityScoreUpdated signal.
    /// </summary>
    /// <param name="dwType">The Dreamweaver type whose affinity changed.</param>
    /// <param name="change">The change amount applied.</param>
    /// <param name="newScore">The new score after the change.</param>
    public void EmitAffinityScoreUpdated(DreamweaverType dwType, int change, int newScore)
    {
        this.EmitSignal(SignalName.AffinityScoreUpdated, (int)dwType, change, newScore);
    }

    /// <summary>
    /// Emits the PlayerProgressionChanged signal.
    /// </summary>
    /// <param name="milestoneId">The identifier of the milestone reached.</param>
    /// <param name="progressData">Additional data about the progress.</param>
    public void EmitPlayerProgressionChanged(string milestoneId, Godot.Collections.Dictionary<string, Variant> progressData)
    {
        this.EmitSignal(SignalName.PlayerProgressionChanged, milestoneId, progressData);
    }

    /// <summary>
    /// Emits the InventoryUpdated signal.
    /// </summary>
    /// <param name="itemId">The ID of the item affected.</param>
    /// <param name="action">The action performed (added, removed, used).</param>
    /// <param name="quantity">The quantity of the item affected.</param>
    public void EmitInventoryUpdated(string itemId, string action, int quantity)
    {
        this.EmitSignal(SignalName.InventoryUpdated, itemId, action, quantity);
    }

    /// <summary>
    /// Emits the CharacterPartyChanged signal.
    /// </summary>
    /// <param name="action">The action performed (added, removed, swapped).</param>
    /// <param name="characterId">The ID of the affected character.</param>
    /// <param name="partyData">Data about the current party composition.</param>
    public void EmitCharacterPartyChanged(string action, string characterId, Godot.Collections.Dictionary<string, Variant> partyData)
    {
        this.EmitSignal(SignalName.CharacterPartyChanged, action, characterId, partyData);
    }

    /// <summary>
    /// Emits the DreamweaverInfluenceChanged signal.
    /// </summary>
    /// <param name="newDominant">The new dominant Dreamweaver type.</param>
    /// <param name="influenceLevel">The level of influence.</param>
    public void EmitDreamweaverInfluenceChanged(DreamweaverType newDominant, int influenceLevel)
    {
        this.EmitSignal(SignalName.DreamweaverInfluenceChanged, (int)newDominant, influenceLevel);
    }
}