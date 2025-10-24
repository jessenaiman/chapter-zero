// <copyright file="NethackUI.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.UI.Terminal;

namespace OmegaSpiral.Source.Stages.Nethack;

/// <summary>
/// Nethack UI that extends TerminalUI and adds game-driven dungeon mechanics.
/// Handles turn-based combat, inventory, exploration, and player agency.
/// Base class for Stage 2 (Nethack gameplay experience).
/// Differs from NarrativeUI - this supports non-linear player-driven gameplay, not sequential story beats.
/// </summary>
[GlobalClass]
public partial class NethackUI : TerminalUI
{
    /// <summary>
    /// Game state for Nethack dungeon exploration.
    /// </summary>
    public class DungeonState
    {
        /// <summary>Gets or sets the current depth in the dungeon.</summary>
        public int CurrentDepth { get; set; }

        /// <summary>Gets or sets the player's current HP.</summary>
        public int CurrentHP { get; set; }

        /// <summary>Gets or sets the player's maximum HP.</summary>
        public int MaxHP { get; set; }

        /// <summary>Gets or sets items in the player's inventory.</summary>
        public string[] InventoryItems { get; set; } = Array.Empty<string>();
    }

    private DungeonState _dungeonState = new();

    /// <summary>
    /// Gets the current dungeon state.
    /// </summary>
    protected DungeonState CurrentDungeonState => _dungeonState;

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
        if (availableActions == null || availableActions.Length == 0)
        {
            GD.PushWarning("[NethackUI] No dungeon actions provided.");
            return string.Empty;
        }

        return await PresentChoicesAsync("What do you do?", availableActions).ConfigureAwait(false);
    }

    /// <summary>
    /// Displays a combat encounter with health bars and effects.
    /// </summary>
    /// <param name="encounterText">The encounter description.</param>
    /// <param name="enemyName">The enemy name.</param>
    /// <param name="enemyHP">The enemy's current HP.</param>
    /// <param name="enemyMaxHP">The enemy's maximum HP.</param>
    /// <returns>A task representing the async operation.</returns>
    public async Task DisplayCombatEncounterAsync(string encounterText, string enemyName, int enemyHP, int enemyMaxHP)
    {
        if (TextRenderer != null)
        {
            await AppendTextAsync(encounterText).ConfigureAwait(false);
        }

        // Display health bar
        var hpPercentage = (float)enemyHP / enemyMaxHP;
        var hpBar = $"[{enemyName}] HP: [{new string('=', (int)(hpPercentage * 20))}{new string('-', 20 - (int)(hpPercentage * 20))}]";
        DisplayDungeonStatus(hpBar);
    }

    /// <summary>
    /// Updates the player's inventory display.
    /// </summary>
    /// <param name="items">Array of inventory item names.</param>
    public void UpdateInventoryDisplay(string[] items)
    {
        _dungeonState.InventoryItems = items;

        var inventoryText = items.Length > 0 ? string.Join(", ", items) : "Empty";
        DisplayDungeonStatus($"Inventory: {inventoryText}");
    }

    /// <summary>
    /// Updates player HP and displays health status.
    /// </summary>
    /// <param name="currentHP">The current HP value.</param>
    /// <param name="maxHP">The maximum HP value.</param>
    public void UpdatePlayerHealth(int currentHP, int maxHP)
    {
        _dungeonState.CurrentHP = currentHP;
        _dungeonState.MaxHP = maxHP;

        var healthBar = new string('=', Math.Max(0, (int)((float)currentHP / maxHP * 20)));
        var emptyBar = new string('-', 20 - healthBar.Length);
        DisplayDungeonStatus($"Player HP: [{healthBar}{emptyBar}] {currentHP}/{maxHP}");
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
