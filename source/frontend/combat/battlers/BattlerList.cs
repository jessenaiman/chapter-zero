// <copyright file="BattlerList.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts.Combat.Battlers;
/// <summary>
/// Keeps reference to the various combat participants, including all <see cref="Battler"/>s and their teams.
/// </summary>
[GlobalClass]
public partial class BattlerList : RefCounted
{
    /// <summary>
    /// Emitted immediately once the player has won or lost the battle. Note that all animations (such
    /// as the player or AI battlers disappearing) are not yet completed.
    /// This is the point at which most Ui elements will disappear.
    /// </summary>
    [Signal]
    public delegate void BattlersDownedEventHandler();

    private Battler[] players = Array.Empty<Battler>();

    /// <summary>
    /// Gets or sets array of player battlers.
    /// </summary>
    public Battler[] Players
    {
        get => this.players;
        set
        {
            this.players = value;
            foreach (Battler battler in this.players)
            {
                // If a party member falls in battle, check to see if the player has lost.
                battler.HealthDepleted += () =>
                {
                    foreach (Battler player in this.players)
                    {
                        if (player.Stats?.Health > 0)
                        {
                            return;
                        }
                    }

                    // All player battlers have zero health. The player lost the battle!
                    this.HasPlayerWon = false;
                    this.EmitSignal(SignalName.BattlersDowned);
                };
            }
        }
    }

    private Battler[] enemies = Array.Empty<Battler>();

    /// <summary>
    /// Gets or sets array of enemy battlers.
    /// </summary>
    public Battler[] Enemies
    {
        get => this.enemies;
        set
        {
            this.enemies = value;
            foreach (Battler battler in this.enemies)
            {
                // If an enemy falls in battle, check to see if the player has won.
                battler.HealthDepleted += () =>
                {
                    foreach (Battler enemy in this.enemies)
                    {
                        if (enemy.Stats?.Health > 0)
                        {
                            return;
                        }
                    }

                    // All enemy battlers have zero health. The player won!
                    this.HasPlayerWon = true;
                    this.EmitSignal(SignalName.BattlersDowned);
                };
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether tracks whether or not the player has won the combat.
    /// </summary>
    public bool HasPlayerWon { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BattlerList"/> class with the specified player and enemy battlers.
    /// </summary>
    /// <param name="playerBattlers">An array of <see cref="Battler"/> objects representing the player's team.</param>
    /// <param name="enemyBattlers">An array of <see cref="Battler"/> objects representing the enemy team.</param>
    public BattlerList(Battler[] playerBattlers, Battler[] enemyBattlers)
    {
        this.Players = playerBattlers;
        this.Enemies = enemyBattlers;
    }

    /// <summary>
    /// Returns an array containing all battlers (both players and enemies).
    /// </summary>
    /// <returns></returns>
    public Battler[] GetAllBattlers()
    {
        Battler[] allBattlers = new Battler[this.players.Length + this.enemies.Length];
        Array.Copy(this.players, allBattlers, this.players.Length);
        Array.Copy(this.enemies, 0, allBattlers, this.players.Length, this.enemies.Length);
        return allBattlers;
    }

    /// <summary>
    /// Returns an array containing only the live battlers from the provided array.
    /// </summary>
    /// <param name="battlers"></param>
    /// <returns></returns>
    public static Battler[] GetLiveBattlers(Battler[] battlers)
    {
        return battlers.Where(battler => battler.Stats?.Health > 0).ToArray();
    }
}
