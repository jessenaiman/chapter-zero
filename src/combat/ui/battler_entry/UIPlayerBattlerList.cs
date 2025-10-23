
// <copyright file="UIPlayerBattlerList.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using Godot;
using OmegaSpiral.Source.Scripts.Combat.Battlers;
using OmegaSpiral.Source.Scripts.Combat.UI.ListMenu;

namespace OmegaSpiral.Source.Scripts.Combat.UI.BattlerEntry;
/// <summary>
/// The player battler UI displays information for each player-owned <see cref="Battler"/> in a combat.
/// These entries may be selected in order to queue actions for the battlers to perform.
/// </summary>
[GlobalClass]
#pragma warning disable IDE1006  // Naming: 2-letter acronym UI stays uppercase per C# style guide
public partial class UIPlayerBattlerList : UIListMenu
#pragma warning restore IDE1006
{
    private Collection<Battler> battlers = new Collection<Battler>();

    /// <summary>
    /// Gets the battler list will create individual entries for each <see cref="Battler"/> contained in this array.
    /// The entries are created when the member is assigned and the list is <see cref="Node.Ready"/>.
    /// </summary>
    public Collection<Battler> Battlers
    {
        get => this.battlers;
        private set
        {
            this.battlers = value;

            if (!this.IsInsideTree())
            {
                // Wait for the node to be ready before processing
                this.CallDeferred(nameof(this.ProcessBattlers));
                return;
            }

            this.ProcessBattlers();
        }
    }

    /// <summary>
    /// Create all menu entries needed to track player battlers throughout the combat.
    /// </summary>
    /// <param name="battlerData">The battler list data.</param>
    /// <exception cref="ArgumentNullException"><paramref name="battlerData"/> is <c>null</c>.</exception>
    public void Setup(BattlerList battlerData)
    {
        ArgumentNullException.ThrowIfNull(battlerData);

        this.Battlers = new Collection<Battler>(battlerData.Players);
    }

    /// <summary>
    /// Handles when a battler entry is pressed, emitting the selected battler if it's player-controlled.
    /// </summary>
    /// <param name="entry">The button entry that was pressed.</param>
    /// <exception cref="ArgumentNullException"><paramref name="entry"/> is <c>null</c>.</exception>
    protected override void OnEntryPressed(BaseButton entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        if (!this.IsDisabled)
        {
            var battlerEntry = entry as UIBattlerEntry;

            // Prevent the player from issuing orders to AI-controlled Battlers.
            if (battlerEntry?.Battler?.AiScene == null)
            {
                var combatEvents = this.GetNode("/root/CombatEvents");
                if (combatEvents != null && battlerEntry != null && battlerEntry.Battler != null)
                {
                    combatEvents.EmitSignal("player_battler_selected", battlerEntry.Battler);
                }
            }
        }
    }

    private void ProcessBattlers()
    {
        // Free any old entries, if they exist.
        foreach (var child in this.GetChildren())
        {
            if (child is UIBattlerEntry)
            {
                child.QueueFree();
            }
        }

        // Create a UI entry for each battler in the party.
        foreach (var battler in this.battlers)
        {
            var newEntry = this.CreateEntry() as UIBattlerEntry;
            if (newEntry != null)
            {
                newEntry.Battler = battler;
            }
        }

        this.LoopFirstAndLastEntries();

        this.FadeIn();
    }
}
