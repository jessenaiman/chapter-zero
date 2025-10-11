using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A list of the combat participants, in BattlerList form.
/// This object is created by the turn queue from children Battlers and then made available
/// to other combat systems. It provides methods for accessing and manipulating groups
/// of battlers during combat.
/// </summary>
public partial class BattlerList : Resource
{
    /// <summary>
    /// Emitted whenever battlers are downed (health depleted), indicating that one side has lost.
    /// </summary>
    [Signal]
    public delegate void BattlersDownedEventHandler();

    /// <summary>
    /// List of player battlers
    /// </summary>
    public List<Battler> Players { get; private set; } = new List<Battler>();

    /// <summary>
    /// List of enemy battlers
    /// </summary>
    public List<Battler> Enemies { get; private set; } = new List<Battler>();

    /// <summary>
    /// Whether the player team has won the combat
    /// </summary>
    public bool HasPlayerWon
    {
        get
        {
            // Player wins if all enemies are defeated
            return Enemies.All(enemy => enemy == null || enemy.Stats == null || enemy.Stats.Health <= 0);
        }
    }

    /// <summary>
    /// Whether the enemy team has won the combat
    /// </summary>
    public bool HasEnemyWon
    {
        get
        {
            // Enemy wins if all players are defeated
            return Players.All(player => player == null || player.Stats == null || player.Stats.Health <= 0);
        }
    }

    /// <summary>
    /// Constructor for BattlerList
    /// </summary>
    /// <param name="players">Array of player battlers</param>
    /// <param name="enemies">Array of enemy battlers</param>
    public BattlerList(Battler[]? players = null, Battler[]? enemies = null)
    {
        if (players != null)
        {
            Players = new List<Battler>(players);
        }

        if (enemies != null)
        {
            Enemies = new List<Battler>(enemies);
        }

        // Connect to health depleted signals for all battlers to detect when a team is defeated
        foreach (var battler in Players.Concat(Enemies))
        {
            if (battler != null && battler.Stats != null)
            {
                battler.Stats.HealthDepleted += OnBattlerHealthDepleted;
            }
        }
    }

    /// <summary>
    /// Gets all battlers (players and enemies combined)
    /// </summary>
    public List<Battler> GetAllBattlers()
    {
        return Players.Concat(Enemies).ToList();
    }

    /// <summary>
    /// Gets only the battlers that are still alive (health > 0)
    /// </summary>
    public List<Battler> GetLiveBattlers()
    {
        return GetAllBattlers().Where(b => b != null && b.Stats != null && b.Stats.Health > 0).ToList();
    }

    /// <summary>
    /// Gets only the battlers that are still alive from a given list
    /// </summary>
    public List<Battler> GetLiveBattlers(List<Battler> battlers)
    {
        return battlers.Where(b => b != null && b.Stats != null && b.Stats.Health > 0).ToList();
    }

    /// <summary>
    /// Gets only the battlers that are still alive from a given array
    /// </summary>
    public Battler[] GetLiveBattlers(Battler[] battlers)
    {
        return battlers.Where(b => b != null && b.Stats != null && b.Stats.Health > 0).ToArray();
    }

    /// <summary>
    /// Add a player battler to the list
    /// </summary>
    public void AddPlayer(Battler player)
    {
        if (player != null && !Players.Contains(player))
        {
            Players.Add(player);

            if (player.Stats != null)
            {
                player.Stats.HealthDepleted += OnBattlerHealthDepleted;
            }
        }
    }

    /// <summary>
    /// Add an enemy battler to the list
    /// </summary>
    public void AddEnemy(Battler enemy)
    {
        if (enemy != null && !Enemies.Contains(enemy))
        {
            Enemies.Add(enemy);

            if (enemy.Stats != null)
            {
                enemy.Stats.HealthDepleted += OnBattlerHealthDepleted;
            }
        }
    }

    /// <summary>
    /// Remove a player battler from the list
    /// </summary>
    public void RemovePlayer(Battler player)
    {
        if (player != null && Players.Contains(player))
        {
            Players.Remove(player);

            if (player.Stats != null)
            {
                player.Stats.HealthDepleted -= OnBattlerHealthDepleted;
            }
        }
    }

    /// <summary>
    /// Remove an enemy battler from the list
    /// </summary>
    public void RemoveEnemy(Battler enemy)
    {
        if (enemy != null && Enemies.Contains(enemy))
        {
            Enemies.Remove(enemy);

            if (enemy.Stats != null)
            {
                enemy.Stats.HealthDepleted -= OnBattlerHealthDepleted;
            }
        }
    }

    /// <summary>
    /// Callback when a battler's health is depleted
    /// </summary>
    private void OnBattlerHealthDepleted()
    {
        // Check if any team has won and emit the appropriate signal
        if (HasPlayerWon || HasEnemyWon)
        {
            EmitSignal(SignalName.BattlersDowned);
        }
    }

    /// <summary>
    /// Sort the battlers by their speed stat (highest first)
    /// </summary>
    public void SortBySpeed()
    {
        Players.Sort((a, b) => b.Stats.Speed.CompareTo(a.Stats.Speed));
        Enemies.Sort((a, b) => b.Stats.Speed.CompareTo(a.Stats.Speed));
    }

    /// <summary>
    /// Get a random live player battler
    /// </summary>
    public Battler GetRandomLivePlayer()
    {
        var livePlayers = GetLiveBattlers(Players);
        if (livePlayers.Count == 0)
        {
            return null;
        }

        return livePlayers[GD.Randi() % livePlayers.Count];
    }

    /// <summary>
    /// Get a random live enemy battler
    /// </summary>
    public Battler GetRandomLiveEnemy()
    {
        var liveEnemies = GetLiveBattlers(Enemies);
        if (liveEnemies.Count == 0)
        {
            return null;
        }

        return liveEnemies[GD.Randi() % liveEnemies.Count];
    }

    /// <summary>
    /// Get all live player battlers
    /// </summary>
    public List<Battler> GetLivePlayers()
    {
        return GetLiveBattlers(Players);
    }

    /// <summary>
    /// Get all live enemy battlers
    /// </summary>
    public List<Battler> GetLiveEnemies()
    {
        return GetLiveBattlers(Enemies);
    }

    /// <summary>
    /// Check if there are any live player battlers
    /// </summary>
    public bool HasLivePlayers()
    {
        return GetLivePlayers().Count > 0;
    }

    /// <summary>
    /// Check if there are any live enemy battlers
    /// </summary>
    public bool HasLiveEnemies()
    {
        return GetLiveEnemies().Count > 0;
    }
}
