using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using OmegaSpiral.Combat.Battlers;

/// <summary>
/// The base class responsible for AI-controlled Battlers.
/// For now, this simply selects a random BattlerAction and picks a random target, if one is
/// available.
/// </summary>
public partial class CombatAIRandom : Node
{
    private bool _hasSelectedAction = false;

    /// <summary>
    /// Connect to the signals of a given Battler. The callback randomly chooses an action from the
    /// Battler's actions and then randomly chooses a target.
    /// </summary>
    /// <param name="battler">The battler to set up AI for.</param>
    /// <param name="battlerList">The list of all battlers in combat.</param>
    public void Setup(Battler battler, BattlerList battlerList)
    {
        battler.ReadyToAct += () => OnBattlerReadyToAct(battler, battlerList);
        battler.ActionFinished += () => _hasSelectedAction = false;
    }

    private void OnBattlerReadyToAct(Battler source, BattlerList battlers)
    {
        if (_hasSelectedAction)
            return;

        // Only a Battler with actions is able to act.
        if (source.Actions.Length == 0)
            return;

        // Randomly choose an action.
        var random = new Random();
        int actionIndex = random.Next(source.Actions.Length);
        var action = source.Actions[actionIndex];

        // Randomly choose a target.
        var possibleTargets = action.GetPossibleTargets(source, battlers);
        var targets = new List<Battler>();
        if (action.TargetsAll())
        {
            targets = possibleTargets.ToList();
        }
        else
        {
            int targetIndex = random.Next(possibleTargets.Length);
            targets.Add(possibleTargets[targetIndex]);
        }

        // If there are targets, register the action.
        if (targets.Count > 0)
        {
            _hasSelectedAction = true;
            CombatEvents.Instance?.EmitSignal("action_selected", action, source, targets.ToArray());
        }
    }
}
