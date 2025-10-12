using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The player battler UI displays information for each player-owned <see cref="Battler"/> in a combat.
/// These entries may be selected in order to queue actions for the battlers to perform.
/// </summary>
public partial class UIPlayerBattlerList : UIListMenu
{
    private List<Battler> _battlers = new List<Battler>();

    /// <summary>
    /// The battler list will create individual entries for each <see cref="Battler"/> contained in this array.
    /// The entries are created when the member is assigned and the list is <see cref="Node.Ready"/>.
    /// </summary>
    public List<Battler> Battlers
    {
        get => _battlers;
        set
        {
            _battlers = value;

            if (!IsInsideTree())
            {
                // Wait for the node to be ready before processing
                CallDeferred(nameof(ProcessBattlers));
                return;
            }

            ProcessBattlers();
        }
    }

    private void ProcessBattlers()
    {
        // Free any old entries, if they exist.
        foreach (var child in GetChildren())
        {
            if (child is UIBattlerEntry)
            {
                child.QueueFree();
            }
        }

        // Create a UI entry for each battler in the party.
        foreach (var battler in _battlers)
        {
            var newEntry = _CreateEntry() as UIBattlerEntry;
            newEntry.Battler = battler;
        }

        _LoopFirstAndLastEntries();

        FadeIn();
    }

    public override void _Ready()
    {
        base._Ready();

        // If the player has selected a battler, prevent input from reaching the battler list.
        // This is relevant with mouse/touchscreen input.
        // If the player has finished navigating the menu, restore input to the battler list.
        CombatEvents.PlayerBattlerSelected += (battler) =>
        {
            IsDisabled = battler != null;

            // Don't re-enable entries that have dead Battlers.
            if (!IsDisabled)
            {
                foreach (var entry in _entries.OfType<UIBattlerEntry>())
                {
                    if (entry.Battler.Stats.Health <= 0)
                    {
                        entry.Disabled = true;
                    }
                }
            }
        };
    }

    /// <summary>
    /// Create all menu entries needed to track player battlers throughout the combat.
    /// </summary>
    /// <param name="battlerData">The battler list data</param>
    public void Setup(BattlerList battlerData)
    {
        Battlers = battlerData.Players.ToList();
    }

    // Let the combat know which battler was selected.
    protected void _OnEntryPressed(BaseButton entry)
    {
        if (!IsDisabled)
        {
            var battlerEntry = entry as UIBattlerEntry;

            // Prevent the player from issuing orders to AI-controlled Battlers.
            if (battlerEntry.Battler.AiScene == null)
            {
                CombatEvents.PlayerBattlerSelected?.Invoke(battlerEntry.Battler);
            }
        }
    }
}
