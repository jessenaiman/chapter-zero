// <copyright file="NethackUi.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Ui.Terminal;

namespace OmegaSpiral.Source.Stages.Nethack;

/// <summary>
/// Nethack Ui that extends TerminalUi and adds game-driven dungeon mechanics.
/// Handles turn-based combat, inventory, exploration, and player agency.
/// Base class for Stage 2 (Nethack gameplay experience).
/// Differs from NarrativeUi - this supports non-linear player-driven gameplay, not sequential story beats.
/// </summary>
[GlobalClass]
public partial class NethackUi : TerminalUi
{
    /// <summary>
    /// Game state for Nethack dungeon exploration.
    /// </summary>
    public class DungeonState
    {
        /// <summary>Gets or sets the current depth in the dungeon.</summary>
        public int CurrentDepth { get; set; }

        /// <summary>Gets or sets the player's current HP.</summary>
        public int CurrentHp { get; set; }

        /// <summary>Gets or sets the player's maximum HP.</summary>
        public int MaxHp { get; set; }

        /// <summary>Gets or sets items in the player's inventory.</summary>
        public string[] InventoryItems { get; set; } = Array.Empty<string>();
    }

    private DungeonState _DungeonState = new();

    /// <summary>
    /// Gets the current dungeon state.
    /// </summary>
    protected DungeonState CurrentDungeonState => _DungeonState;

    /// <summary>
    /// Displays the dungeon status overlay (HP, inventory, current room).
    /// </summary>
    /// <param name="statusText">The status text to display.</param>
    public void DisplayDungeonStatus(string statusText)
    {
        if (!string.IsNullOrEmpty(statusText))
        {
            UpdateCaption(statusText);
        }
    }

    /// <summary>
    /// Presents dungeon actions (move, attack, cast spell, etc.).
    /// Non-linear gameplay - player chooses their action.
    /// </summary>
    /// <param name="availableActions">Array of action descriptions.</param>
    /// <returns>The selected action text.</returns>
    public async Task<string> PresentDungeonActionsAsync(string[] availableActions)
    {
        if (availableActions.Length == 0)
        {
            GD.PushWarning("[NethackUi] No dungeon actions provided.");
            return string.Empty;
        }

        return await PresentChoicesAsync("What do you do?", availableActions).ConfigureAwait(false);
    }

    /// <summary>
    /// Displays a combat encounter with health bars and effects.
    /// </summary>
    /// <param name="encounterText">The encounter description.</param>
    /// <param name="enemyName">The enemy name.</param>
    /// <param name="enemyHp">The enemy's current HP.</param>
    /// <param name="enemyMaxHp">The enemy's maximum HP.</param>
    /// <returns>A task representing the async operation.</returns>
    public async Task DisplayCombatEncounterAsync(string encounterText, string enemyName, int enemyHp, int enemyMaxHp)
    {
        if (TextRenderer != null)
        {
            await AppendTextAsync(encounterText).ConfigureAwait(false);
        }

        // Display health bar
        var hpPercentage = (float)enemyHp / enemyMaxHp;
        var hpBar = $"[{enemyName}] HP: [{new string('=', (int)(hpPercentage * 20))}{new string('-', 20 - (int)(hpPercentage * 20))}]";
        DisplayDungeonStatus(hpBar);
    }

    /// <summary>
    /// Updates the player's inventory display.
    /// </summary>
    /// <param name="items">Array of inventory item names.</param>
    public void UpdateInventoryDisplay(string[] items)
    {
        _DungeonState.InventoryItems = items;

        var inventoryText = items.Length > 0 ? string.Join(", ", items) : "Empty";
        DisplayDungeonStatus($"Inventory: {inventoryText}");
    }

    /// <summary>
    /// Updates player HP and displays health status.
    /// </summary>
    /// <param name="currentHp">The current HP value.</param>
    /// <param name="maxHp">The maximum HP value.</param>
    public void UpdatePlayerHealth(int currentHp, int maxHp)
    {
        _DungeonState.CurrentHp = currentHp;
        _DungeonState.MaxHp = maxHp;

        var healthBar = new string('=', Math.Max(0, (int)((float)currentHp / maxHp * 20)));
        var emptyBar = new string('-', 20 - healthBar.Length);
        DisplayDungeonStatus($"Player HP: [{healthBar}{emptyBar}] {currentHp}/{maxHp}");
    }

    /// <summary>
    /// Applies a game-specific visual effect (e.g., enemy attack flash, spell effect).
    /// </summary>
    /// <param name="effectPreset">The effect preset name.</param>
    /// <param name="durationSeconds">Duration of the effect.</param>
    /// <returns>A task representing the async operation.</returns>
    public async Task ApplyGameEffectAsync(string effectPreset, float durationSeconds = 0.3f)
    {
        if (ShaderController != null)
        {
            await ShaderController.ApplyVisualPresetAsync(effectPreset).ConfigureAwait(false);
            await Task.Delay((int)(durationSeconds * 1000)).ConfigureAwait(false);
        }
    }
}
