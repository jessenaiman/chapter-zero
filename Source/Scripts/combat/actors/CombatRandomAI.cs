// <copyright file="CombatRandomAI.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// A simple combat AI that chooses actions randomly.
/// This AI makes decisions by randomly selecting from available actions and targets.
/// It's useful for testing and for enemies that should behave unpredictably.
/// </summary>
public partial class CombatRandomAI : CombatAI
{
    /// <summary>
    /// Gets or sets the probability (0.0 to 1.0) that the AI will choose to attack instead of using a skill.
    /// </summary>
    [Export]
    public float AttackProbability { get; set; } = 0.7f;

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
        if (!this.IsActive || this.ControlledBattler == null || this.ControlledBattler.Actions == null)
        {
            return (null, new List<Battler>());
        }

        // Wait for the turn delay to make the AI feel more natural
        if (this.TurnDelay > 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(this.TurnDelay)).ConfigureAwait(false);
        }

        // Get all available actions
        var availableActions = this.ControlledBattler.Actions.Where(action =>
            action != null && action.CanExecute(this.ControlledBattler, Array.Empty<Battler>())).ToList();

        if (availableActions.Count == 0)
        {
            return (null, new List<Battler>());
        }

        // Filter actions based on the attack probability
        var filteredActions = new List<BattlerAction>();
        foreach (var action in availableActions)
        {
            // If it's an attack action and we're within the attack probability, include it
            if (action is AttackBattlerAction && GD.Randf() <= this.AttackProbability)
            {
                filteredActions.Add(action);
            }

            // If it's not an attack action and we're outside the attack probability, include it
            else if (!(action is AttackBattlerAction) && GD.Randf() > this.AttackProbability)
            {
                filteredActions.Add(action);
            }
        }

        // If no actions were filtered in, use all available actions
        if (filteredActions.Count == 0)
        {
            filteredActions = new List<BattlerAction>(availableActions);
        }

        // Choose a random action
        var chosenAction = filteredActions[(int)(GD.Randi() % filteredActions.Count)];

        // Choose targets for the action
        using var battlerList = this.Battlers ?? new BattlerList(Array.Empty<Battler>(), Array.Empty<Battler>());
        var possibleTargets = chosenAction.GetPossibleTargets(this.ControlledBattler, battlerList);
        var validTargets = possibleTargets.Where(target => chosenAction.IsTargetValid(target)).ToList();

        if (validTargets.Count == 0)
        {
            return (chosenAction, new List<Battler>());
        }

        // Filter targets based on the target enemy probability
        var filteredTargets = new List<Battler>();
        foreach (var target in validTargets)
        {
            // Check if the target is an enemy
            var isEnemy = this.Battlers?.Enemies.Contains(target) ?? false;

            // If it's an enemy and we're within the target enemy probability, include it
            if (isEnemy && GD.Randf() <= this.TargetEnemyProbability)
            {
                filteredTargets.Add(target);
            }

            // If it's not an enemy and we're outside the target enemy probability, include it
            else if (!isEnemy && GD.Randf() > this.TargetEnemyProbability)
            {
                filteredTargets.Add(target);
            }
        }

        // If no targets were filtered in, use all valid targets
        if (filteredTargets.Count == 0)
        {
            filteredTargets = new List<Battler>(validTargets);
        }

        // For single-target actions, choose one target
        if (chosenAction.TargetScope == ActionTargetScope.One)
        {
            var target = filteredTargets[(int)(GD.Randi() % filteredTargets.Count)];
            return (chosenAction, new List<Battler> { target });
        }

        // For all-target actions, choose all filtered targets
        if (chosenAction.TargetScope == ActionTargetScope.All)
        {
            return (chosenAction, filteredTargets);
        }

        // For self-target actions, target the controlled battler
        if (chosenAction.TargetScope == ActionTargetScope.Self)
        {
            return (chosenAction, new List<Battler> { this.ControlledBattler });
        }

        // Default: return the chosen action with no targets
        return (chosenAction, new List<Battler>());
    }

    /// <summary>
    /// Evaluate the current combat situation.
    /// This random AI doesn't make strategic evaluations, so it returns a neutral score.
    /// </summary>
    /// <returns>A float value representing the evaluation score (1.0 for neutral).</returns>
    public override float EvaluateSituation()
    {
        // Return a neutral score since this AI doesn't make strategic evaluations
        return 1.0f;
    }

    /// <summary>
    /// Get the priority of a potential action.
    /// This random AI assigns equal priority to all actions.
    /// </summary>
    /// <param name="action">The action to evaluate.</param>
    /// <param name="targets">The targets for the action.</param>
    /// <returns>A float value representing the priority (1.0 for equal priority).</returns>
    public override float GetActionPriority(BattlerAction action, List<Battler> targets)
    {
        // Return a neutral priority since this AI doesn't prioritize actions
        return 1.0f;
    }
}
