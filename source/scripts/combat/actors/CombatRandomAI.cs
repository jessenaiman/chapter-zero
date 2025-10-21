
// <copyright file="CombatRandomAI.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Combat.Battlers;
using OmegaSpiral.Source.Combat.Actions;

namespace OmegaSpiral.Source.Scripts.Combat.Actors;

/// <summary>
/// A simple combat AI that chooses actions randomly. This AI makes decisions by randomly selecting from available actions and targets. Useful for testing and for enemies that should behave unpredictably.
/// </summary>
[GlobalClass]
public partial class CombatRandomAI : CombatAI
{
    /// <summary>
    /// Gets or sets the probability (0.0 to 1.0) that the AI will target enemies instead of allies.
    /// </summary>
    [Export]
    public float TargetEnemyProbability { get; set; } = 0.8f;

    /// <summary>
    /// Choose an action for the controlled battler to take.
    /// This AI chooses actions randomly, with a preference for attacking.
    /// </summary>
    /// <returns>A tuple containing the chosen action and list of targets for that action.</returns>
    public override async Task<(BattlerAction? action, List<Battler> targets)> ChooseAction()
    {
        if (!this.IsActive || this.ControlledBattler?.Actions == null)
        {
            return (null, []);
        }

        if (this.TurnDelay > 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(this.TurnDelay)).ConfigureAwait(false);
        }

        var availableActions = this.GetAvailableActions();
        if (availableActions.Count == 0)
        {
            return (null, []);
        }

        var chosenAction = this.SelectRandomAction(availableActions);
        var targets = await this.SelectTargetsForAction(chosenAction);
        return (chosenAction, targets);
    }

    /// <summary>
    /// Get all available actions for the controlled battler.
    /// </summary>
    /// <returns>List of executable actions.</returns>
    private List<BattlerAction> GetAvailableActions()
    {
        return this.ControlledBattler!.Actions!
            .Where(action => action.CanExecute(this.ControlledBattler, []))
            .ToList();
    }

    /// <summary>
    /// Select a random action from available actions, with probability filtering.
    /// </summary>
    /// <param name="actions">Available actions to choose from.</param>
    /// <returns>Selected action.</returns>
    private BattlerAction SelectRandomAction(List<BattlerAction> actions)
    {
        var filtered = new List<BattlerAction>();
        foreach (var action in actions)
        {
            if (GD.Randf() <= this.TargetEnemyProbability || GD.Randf() > this.TargetEnemyProbability)
            {
                filtered.Add(action);
            }
        }

        var actionsToUse = filtered.Count > 0 ? filtered : actions;
        return actionsToUse[(int) (GD.Randi() % actionsToUse.Count)];
    }

    /// <summary>
    /// Select targets for the chosen action based on scope and probability.
    /// </summary>
    /// <param name="action">The action to select targets for.</param>
    /// <returns>List of targets for the action.</returns>
    private Task<List<Battler>> SelectTargetsForAction(BattlerAction action)
    {
        using var battlerList = this.Battlers ?? new BattlerList([], []);
        var possibleTargets = action.GetPossibleTargets(this.ControlledBattler!, battlerList);
        var validTargets = possibleTargets.Where(target => action.IsTargetValid(target)).ToList();

        if (validTargets.Count == 0)
        {
            return Task.FromResult<List<Battler>>([]);
        }

        var filtered = this.FilterTargetsByProbability(validTargets);
        var targetsToUse = filtered.Count > 0 ? filtered : validTargets;

        var result = action.TargetScope switch
        {
            ActionTargetScope.One => [targetsToUse[(int) (GD.Randi() % targetsToUse.Count)]],
            ActionTargetScope.All => targetsToUse,
            ActionTargetScope.Self => [this.ControlledBattler!],
            _ => [],
        };

        return Task.FromResult(result);
    }

    /// <summary>
    /// Filter targets based on the target enemy probability.
    /// </summary>
    /// <param name="targets">Available targets to filter.</param>
    /// <returns>Filtered list of targets.</returns>
    private List<Battler> FilterTargetsByProbability(List<Battler> targets)
    {
        var filtered = new List<Battler>();
        foreach (var target in targets)
        {
            var isEnemy = this.Battlers?.Enemies.Contains(target) ?? false;
            if ((isEnemy && GD.Randf() <= this.TargetEnemyProbability) ||
                (!isEnemy && GD.Randf() > this.TargetEnemyProbability))
            {
                filtered.Add(target);
            }
        }

        return filtered;
    }

    /// <summary>
    /// Evaluate the current combat situation.
    /// This random AI doesn't make strategic evaluations, so it returns a neutral score.
    /// </summary>
    /// <returns>A float value representing the evaluation score (1.0 for neutral).</returns>
    public override float EvaluateSituation()
    {
        return 1.0f;
    }

    /// <summary>
    /// Gets the priority of an action for the AI. Parameters are unused in this implementation.
    /// </summary>
    /// <param name="action">The action to evaluate.</param>
    /// <param name="targets">The targets for the action.</param>
    /// <returns>The priority value for the action.</returns>
    public override float GetActionPriority(BattlerAction action, List<Battler> targets)
    {
        return 1.0f;
    }
}
