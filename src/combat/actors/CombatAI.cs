using System.Collections.ObjectModel;

// <copyright file="CombatAI.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Combat.Battlers;
using OmegaSpiral.Source.Combat.Actions;

namespace OmegaSpiral.Source.Scripts.Combat.Actors;
/// <summary>
/// Base class for combat artificial intelligence.
/// CombatAI determines the behavior of non-player Battlers in combat. It chooses actions
/// and targets based on the Battler's stats, the situation, and the AI's personality.
/// </summary>
[GlobalClass]
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
    /// Checks if the AI is in a valid state to choose an action.
    /// </summary>
    private bool IsValidForAction()
    {
        return this.IsActive && this.ControlledBattler != null && this.ControlledBattler.Actions != null;
    }

    /// <summary>
    /// Choose an action for the controlled battler to take.
    /// This method is called when the battler is ready to act.
    /// Override this method to implement custom AI behavior.
    /// </summary>
    /// <returns>A tuple containing the chosen <see cref="BattlerAction"/> and a list of target <see cref="Battler"/>s.</returns>
    public virtual async Task<(BattlerAction? action, List<Battler> targets)> ChooseAction()
    {
        // Early return if not valid for action
        if (!this.IsValidForAction())
        {
            return (null, new List<Battler>());
        }

        // Apply turn delay
        await this.ApplyTurnDelay().ConfigureAwait(false);

        // Select action
        var chosenAction = this.SelectRandomAction();
        if (chosenAction == null)
        {
            return (null, new List<Battler>());
        }

        // Get valid targets
        var validTargets = this.GetValidTargets(chosenAction);
        if (validTargets.Count == 0)
        {
            return (chosenAction, new List<Battler>());
        }

        // Select targets based on scope
        var selectedTargets = this.SelectTargetsForScope(chosenAction, validTargets);
        return (chosenAction, selectedTargets);
    }

    /// <summary>
    /// Applies the turn delay asynchronously.
    /// </summary>
    private async Task ApplyTurnDelay()
    {
        if (this.TurnDelay > 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(this.TurnDelay)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Selects a random action from available actions.
    /// </summary>
    private BattlerAction? SelectRandomAction()
    {
        var availableActions = this.ControlledBattler!.Actions
            .Where(action => action != null && action.CanExecute(this.ControlledBattler, Array.Empty<Battler>()))
            .Cast<BattlerAction?>()
            .ToList();

        if (availableActions.Count == 0)
        {
            return null;
        }

        return availableActions[(int)(GD.Randi() % availableActions.Count)];
    }

    /// <summary>
    /// Gets the list of valid targets for an action.
    /// </summary>
    private List<Battler> GetValidTargets(BattlerAction chosenAction)
    {
        using BattlerList battlerList = this.Battlers ?? new BattlerList(Array.Empty<Battler>(), Array.Empty<Battler>());
        var possibleTargets = chosenAction.GetPossibleTargets(this.ControlledBattler!, battlerList);
        return possibleTargets.Where(target => chosenAction.IsTargetValid(target)).ToList();
    }

    /// <summary>
    /// Selects targets based on the action's target scope.
    /// </summary>
    private List<Battler> SelectTargetsForScope(BattlerAction chosenAction, List<Battler> validTargets)
    {
        if (chosenAction.TargetScope == ActionTargetScope.One)
        {
            return this.SelectSingleTarget(validTargets);
        }

        if (chosenAction.TargetScope == ActionTargetScope.All)
        {
            return validTargets;
        }

        if (chosenAction.TargetScope == ActionTargetScope.Self)
        {
            return new List<Battler> { this.ControlledBattler! };
        }

        return new List<Battler>();
    }

    /// <summary>
    /// Selects a single random target from the valid targets list.
    /// </summary>
    private List<Battler> SelectSingleTarget(List<Battler> validTargets)
    {
        return new List<Battler> { validTargets[(int)(GD.Randi() % validTargets.Count)] };
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
            aiTeamHealth += battler.Stats?.Health ?? 0;
        }

        foreach (var battler in this.Battlers.Players)
        {
            playerTeamHealth += battler.Stats?.Health ?? 0;
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
    public virtual float GetActionPriority(BattlerAction action, ReadOnlyCollection<Battler> targets)
    {
        // targets is not nullable, so no null check needed

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
#pragma warning restore CA1502, CA1505
