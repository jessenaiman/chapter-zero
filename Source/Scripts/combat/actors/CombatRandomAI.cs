using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// A simple combat AI that chooses actions randomly.
/// This AI makes decisions by randomly selecting from available actions and targets.
/// It's useful for testing and for enemies that should behave unpredictably.
/// </summary>
public partial class CombatRandomAI : CombatAI
{
    /// <summary>
    /// The probability (0.0 to 1.0) that the AI will choose to attack instead of using a skill.
    /// </summary>
    [Export]
    public float AttackProbability { get; set; } = 0.7f;

    /// <summary>
    /// The probability (0.0 to 1.0) that the AI will target enemies instead of allies.
    /// </summary>
    [Export]
    public float TargetEnemyProbability { get; set; } = 0.8f;

    /// <summary>
    /// Choose an action for the controlled battler to take.
    /// This AI chooses actions randomly, with a preference for attacking.
    /// </summary>
    public override async Task<(BattlerAction action, List<Battler> targets)> ChooseAction()
    {
        if (!IsActive || ControlledBattler == null || ControlledBattler.Actions == null)
        {
            return (null, new List<Battler>());
        }

        // Wait for the turn delay to make the AI feel more natural
        if (TurnDelay > 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(TurnDelay));
        }

        // Get all available actions
        var availableActions = ControlledBattler.Actions.Where(action =>
            action != null && action.CanExecute(ControlledBattler, new List<Battler>())).ToList();

        if (availableActions.Count == 0)
        {
            return (null, new List<Battler>());
        }

        // Filter actions based on the attack probability
        var filteredActions = new List<BattlerAction>();
        foreach (var action in availableActions)
        {
            // If it's an attack action and we're within the attack probability, include it
            if (action is AttackBattlerAction && GD.Randf() <= AttackProbability)
            {
                filteredActions.Add(action);
            }
            // If it's not an attack action and we're outside the attack probability, include it
            else if (!(action is AttackBattlerAction) && GD.Randf() > AttackProbability)
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
        var chosenAction = filteredActions[GD.Randi() % filteredActions.Count];

        // Choose targets for the action
        var possibleTargets = chosenAction.GetPossibleTargets(ControlledBattler, Battlers);
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
            var isEnemy = Battlers.Enemies.Contains(target);

            // If it's an enemy and we're within the target enemy probability, include it
            if (isEnemy && GD.Randf() <= TargetEnemyProbability)
            {
                filteredTargets.Add(target);
            }
            // If it's not an enemy and we're outside the target enemy probability, include it
            else if (!isEnemy && GD.Randf() > TargetEnemyProbability)
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
        if (chosenAction.TargetScope == ActionTargetScope.Single)
        {
            var target = filteredTargets[GD.Randi() % filteredTargets.Count];
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
            return (chosenAction, new List<Battler> { ControlledBattler });
        }

        // Default: return the chosen action with no targets
        return (chosenAction, new List<Battler>());
    }

    /// <summary>
    /// Evaluate the current combat situation.
    /// This random AI doesn't make strategic evaluations, so it returns a neutral score.
    /// </summary>
    public override float EvaluateSituation()
    {
        // Return a neutral score since this AI doesn't make strategic evaluations
        return 1.0f;
    }

    /// <summary>
    /// Get the priority of a potential action.
    /// This random AI assigns equal priority to all actions.
    /// </summary>
    public override float GetActionPriority(BattlerAction action, List<Battler> targets)
    {
        // Return a neutral priority since this AI doesn't prioritize actions
        return 1.0f;
    }
}
