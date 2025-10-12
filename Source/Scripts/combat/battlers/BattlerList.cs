using Godot;
using System;
using System.Linq;

/// <summary>
/// Keeps reference to the various combat participants, including all <see cref="Battler"/>s and their teams.
/// </summary>
public partial class BattlerList : RefCounted
{
    /// <summary>
    /// Emitted immediately once the player has won or lost the battle. Note that all animations (such
    /// as the player or AI battlers disappearing) are not yet completed.
    /// This is the point at which most UI elements will disappear.
    /// </summary>
    [Signal]
    public delegate void BattlersDownedEventHandler();

    private Battler[] _players = new Battler[0];

    /// <summary>
    /// Array of player battlers.
    /// </summary>
    public Battler[] Players
    {
        get => _players;
        set
        {
            _players = value;
            foreach (Battler battler in _players)
            {
                // If a party member falls in battle, check to see if the player has lost.
                battler.HealthDepleted += () =>
                {
                    foreach (Battler player in _players)
                    {
                        if (player.Stats.Health > 0)
                        {
                            return;
                        }
                    }

                    // All player battlers have zero health. The player lost the battle!
                    HasPlayerWon = false;
                    EmitSignal(SignalName.BattlersDowned);
                };
            }
        }
    }

    private Battler[] _enemies = new Battler[0];

    /// <summary>
    /// Array of enemy battlers.
    /// </summary>
    public Battler[] Enemies
    {
        get => _enemies;
        set
        {
            _enemies = value;
            foreach (Battler battler in _enemies)
            {
                // If an enemy falls in battle, check to see if the player has won.
                battler.HealthDepleted += () =>
                {
                    foreach (Battler enemy in _enemies)
                    {
                        if (enemy.Stats.Health > 0)
                        {
                            return;
                        }
                    }

                    // All enemy battlers have zero health. The player won!
                    HasPlayerWon = true;
                    EmitSignal(SignalName.BattlersDowned);
                };
            }
        }
    }

    /// <summary>
    /// Tracks whether or not the player has won the combat.
    /// </summary>
    public bool HasPlayerWon { get; set; } = false;

    public BattlerList(Battler[] playerBattlers, Battler[] enemyBattlers)
    {
        Players = playerBattlers;
        Enemies = enemyBattlers;
    }

    /// <summary>
    /// Returns an array containing all battlers (both players and enemies).
    /// </summary>
    public Battler[] GetAllBattlers()
    {
        Battler[] allBattlers = new Battler[_players.Length + _enemies.Length];
        Array.Copy(_players, allBattlers, _players.Length);
        Array.Copy(_enemies, 0, allBattlers, _players.Length, _enemies.Length);
        return allBattlers;
    }

    /// <summary>
    /// Returns an array containing only the live battlers from the provided array.
    /// </summary>
    public Battler[] GetLiveBattlers(Battler[] battlers)
    {
        return battlers.Where(battler => battler.Stats.Health > 0).ToArray();
    }
}