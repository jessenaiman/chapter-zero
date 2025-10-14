// <copyright file="CombatAI.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Scripts.Combat.Actions;

/// <summary>
/// Base class for combat artificial intelligence.
/// CombatAI determines the behavior of non-player Battlers in combat. It chooses actions
/// and targets based on the Battler's stats, the situation, and the AI's personality.
/// </summary>
public partial class CombatAI : Node
{
    /// <summary>
    /// Gets the Battler that this AI controls.
    /// </summary>
    public Battler? ControlledBattler { get; private set; }

    /// <summary>
    /// Gets the list of all battlers in combat, used for target selection.
    /// </summary>
    public BattlerList? Battlers { get; private set; }

    /// <summary>
    /// Gets or sets the delay in seconds before the AI takes its turn.
    /// This can be used to make the AI feel more natural and give the player time to react.
    /// </summary>
    [Export]
    public float TurnDelay { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets a value indicating whether whether this AI is currently active and making decisions.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Setup the AI with its controlled battler and the list of all battlers.
    /// This method is called by the ActiveTurnQueue when the combat begins.
    /// </summary>
    /// <param name="controlledBattler">The <see cref="Battler"/> that this AI will control.</param>
    /// <param name="battlers">The list of all <see cref="Battler"/>s in the current combat.</param>
    public virtual void Setup(Battler controlledBattler, BattlerList battlers)
    {
        this.ControlledBattler = controlledBattler;
        this.Battlers = battlers;
    }

    /// <summary>
    /// Choose an action for the controlled battler to take.
    /// This method is called when the battler is ready to act.
    /// Override this method to implement custom AI behavior.
    /// </summary>
    /// <returns>A tuple containing the chosen <see cref="BattlerAction"/> and a list of target <see cref="Battler"/>s.</returns>
    public virtual async Task<(BattlerAction? action, List<Battler> targets)> ChooseAction()
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

        // Choose a random action for now (simple AI)
        var chosenAction = availableActions[(int) (GD.Randi() % availableActions.Count)];

        // Choose targets for the action
        using BattlerList battlerList = this.Battlers ?? new BattlerList(Array.Empty<Battler>(), Array.Empty<Battler>());
        var possibleTargets = chosenAction.GetPossibleTargets(this.ControlledBattler, battlerList);
        var validTargets = possibleTargets.Where(target => chosenAction.IsTargetValid(target)).ToList();

        if (validTargets.Count == 0)
        {
            return (chosenAction, new List<Battler>());
        }

        // For single-target actions, choose one target
        if (chosenAction.TargetScope == ActionTargetScope.One)
        {
            var target = validTargets[(int) (GD.Randi() % validTargets.Count)];
            return (chosenAction, new List<Battler> { target });
        }

        // For all-target actions, choose all valid targets
        if (chosenAction.TargetScope == ActionTargetScope.All)
        {
            return (chosenAction, validTargets);
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
    /// Evaluate the current combat situation and return a score.
    /// Higher scores indicate better situations for the AI's team.
    /// This can be used to make more strategic decisions.
    /// </summary>
    /// <returns>A score representing the favorability of the current combat situation for the AI's team.</returns>
    public virtual float EvaluateSituation()
    {
        if (this.ControlledBattler == null || this.Battlers == null)
        {
            return 0.0f;
        }

        // Simple evaluation: ratio of AI team's health to player team's health
        var aiTeamHealth = 0.0f;
        var playerTeamHealth = 0.0f;

        foreach (var battler in this.Battlers.Enemies)
        {
            if (battler != null && battler.Stats != null)
            {
                aiTeamHealth += battler.Stats.Health;
            }
        }

        foreach (var battler in this.Battlers.Players)
        {
            if (battler != null && battler.Stats != null)
            {
                playerTeamHealth += battler.Stats.Health;
            }
        }

        // Avoid division by zero
        if (playerTeamHealth <= 0)
        {
            return float.PositiveInfinity;
        }

        return aiTeamHealth / playerTeamHealth;
    }

    /// <summary>
    /// Get the priority of a potential action.
    /// Higher priority actions are more likely to be chosen.
    /// Override this method to implement custom priority logic.
    /// </summary>
    /// <param name="action">The <see cref="BattlerAction"/> to evaluate.</param>
    /// <param name="targets">The list of target <see cref="Battler"/>s for the action.</param>
    /// <returns>A priority score for the action, where higher values indicate higher priority.</returns>
    public virtual float GetActionPriority(BattlerAction action, List<Battler> targets)
    {
        if (action == null || targets == null)
        {
            return 0.0f;
        }

        // Simple priority system: prefer actions that cost less energy
        var priority = 10.0f - action.EnergyCost;

        // Prefer actions that target enemies
        if (action.TargetsEnemies)
        {
            priority += 2.0f;
        }

        // Prefer actions that target friendlies if they heal
        if (action.TargetsFriendlies)
        {
            priority += 1.0f;
        }

        return priority;
    }

    /// <summary>
    /// Called when the AI's battler is defeated.
    /// Override this method to implement custom behavior when the AI loses.
    /// </summary>
    public virtual void OnDefeat()
    {
        this.IsActive = false;
    }

    /// <summary>
    /// Called when the AI's team wins the combat.
    /// Override this method to implement custom behavior when the AI wins.
    /// </summary>
    public virtual void OnVictory()
    {
        this.IsActive = false;
    }
}
