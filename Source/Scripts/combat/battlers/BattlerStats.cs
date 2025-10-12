// <copyright file="BattlerStats.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// Numerically represents the characteristics of a specific <see cref="Battler"/>.
/// </summary>
public partial class BattlerStats : Resource
{
    /// <summary>
    /// A list of all properties that can receive bonuses.
    /// </summary>
    private static readonly string[] ModifiableStats =
    {
        "max_health", "max_energy", "attack", "defense", "speed", "hit_chance", "evasion",
    };

    /// <summary>
    /// Emitted when <see cref="Health"/> has reached 0.
    /// </summary>
    [Signal]
    public delegate void HealthDepletedEventHandler();

    /// <summary>
    /// Emitted whenever <see cref="Health"/> changes.
    /// </summary>
    [Signal]
    public delegate void HealthChangedEventHandler();

    /// <summary>
    /// Emitted whenever <see cref="Energy"/> changes.
    /// </summary>
    [Signal]
    public delegate void EnergyChangedEventHandler();

    [ExportCategory("Elements")]
    /// <summary>
    /// The battler's elemental affinity. Determines which attacks are more or less effective against
    /// this battler.
    /// </summary>
    [Export]
    public Elements.Types Affinity { get; set; } = Elements.Types.None;

    [ExportCategory("Stats")]
    [Export]
    public int BaseMaxHealth { get; set; } = 100;

    [Export]
    public int BaseMaxEnergy { get; set; } = 6;

    private int baseAttack = 10;

    [Export]
    public int BaseAttack
    {
        get => this.baseAttack;
        set
        {
            this.baseAttack = value;
            this.RecalculateAndUpdate("attack");
        }
    }

    private int baseDefense = 10;

    [Export]
    public int BaseDefense
    {
        get => this.baseDefense;
        set
        {
            this.baseDefense = value;
            this.RecalculateAndUpdate("defense");
        }
    }

    private int baseSpeed = 70;

    [Export]
    public int BaseSpeed
    {
        get => this.baseSpeed;
        set
        {
            this.baseSpeed = value;
            this.RecalculateAndUpdate("speed");
        }
    }

    private int baseHitChance = 100;

    [Export]
    public int BaseHitChance
    {
        get => this.baseHitChance;
        set
        {
            this.baseHitChance = value;
            this.RecalculateAndUpdate("hit_chance");
        }
    }

    private int baseEvasion;

    [Export]
    public int BaseEvasion
    {
        get => this.baseEvasion;
        set
        {
            this.baseEvasion = value;
            this.RecalculateAndUpdate("evasion");
        }
    }

    public int MaxHealth { get; private set; }

    public int MaxEnergy { get; private set; }

    public int Attack { get; private set; }

    public int Defense { get; private set; }

    public int Speed { get; private set; }

    public int HitChance { get; private set; }

    public int Evasion { get; private set; }

    private int health;

    public int Health
    {
        get => this.health;
        set
        {
            if (value != this.health)
            {
                this.health = Mathf.Clamp(value, 0, this.MaxHealth);

                this.EmitSignal(SignalName.HealthChanged);
                if (this.health == 0)
                {
                    this.EmitSignal(SignalName.HealthDepleted);
                }
            }
        }
    }

    private int energy;

    public int Energy
    {
        get => this.energy;
        set
        {
            if (value != this.energy)
            {
                this.energy = Mathf.Clamp(value, 0, this.MaxEnergy);
                this.EmitSignal(SignalName.EnergyChanged);
            }
        }
    }

    // The properties below store a list of modifiers for each property listed in MODIFIABLE_STATS.
    // Dictionary keys are the name of the property (String).
    // Dictionary values are another dictionary, with uid/modifier pairs.
    private Dictionary<string, Dictionary<int, int>> modifiers = new Dictionary<string, Dictionary<int, int>>();
    private Dictionary<string, Dictionary<int, float>> multipliers = new Dictionary<string, Dictionary<int, float>>();

    public BattlerStats()
    {
        this.MaxHealth = this.BaseMaxHealth;
        this.MaxEnergy = this.BaseMaxEnergy;
        this.Attack = this.BaseAttack;
        this.Defense = this.BaseDefense;
        this.Speed = this.BaseSpeed;
        this.HitChance = this.BaseHitChance;
        this.Evasion = this.BaseEvasion;

        foreach (string propName in ModifiableStats)
        {
            this.modifiers[propName] = new Dictionary<int, int>();
            this.multipliers[propName] = new Dictionary<int, float>();
        }
    }

    public void Initialize()
    {
        this.Health = this.MaxHealth;
    }

    /// <summary>
    /// Adds a modifier that affects the stat with the given `statName` and returns its unique id.
    /// </summary>
    /// <returns></returns>
    public int AddModifier(string statName, int value)
    {
        System.Diagnostics.Debug.Assert(ModifiableStats.Contains(statName), "Trying to add a modifier to a nonexistent stat.");

        int id = this.GenerateUniqueId(statName, true);
        this.modifiers[statName][id] = value;
        this.RecalculateAndUpdate(statName);

        // Returning the id allows the caller to bind it to a signal. For instance
        // with equipment, to call `RemoveModifier()` upon removing the equipment.
        return id;
    }

    /// <summary>
    /// Adds a multiplier that affects the stat with the given `statName` and returns its unique id.
    /// </summary>
    /// <returns></returns>
    public int AddMultiplier(string statName, float value)
    {
        System.Diagnostics.Debug.Assert(ModifiableStats.Contains(statName), "Trying to add a modifier to a nonexistent stat.");

        int id = this.GenerateUniqueId(statName, false);
        this.multipliers[statName][id] = value;
        this.RecalculateAndUpdate(statName);

        return id;
    }

    /// <summary>
    /// Removes a modifier associated with the given `statName`.
    /// </summary>
    public void RemoveModifier(string statName, int id)
    {
        System.Diagnostics.Debug.Assert(this.modifiers.ContainsKey(statName) && this.modifiers[statName].ContainsKey(id), $"Stat {statName} does not have a modifier with ID '{id}'.");

        this.modifiers[statName].Remove(id);
        this.RecalculateAndUpdate(statName);
    }

    public void RemoveMultiplier(string statName, int id)
    {
        System.Diagnostics.Debug.Assert(this.multipliers.ContainsKey(statName) && this.multipliers[statName].ContainsKey(id), $"Stat {statName} does not have a multiplier with ID '{id}'.");

        this.multipliers[statName].Remove(id);
        this.RecalculateAndUpdate(statName);
    }

    /// <summary>
    /// Calculates the final value of a single stat. That is, its base value with all modifiers applied.
    /// We reference a stat property name using a string here and update it with reflection.
    /// </summary>
    private void RecalculateAndUpdate(string propName)
    {
        // All our property names follow a pattern: the base stat has the same identifier as the final
        // stat with the "base_" prefix.
        string basePropId = "base_" + propName;

        // Get the base value using reflection
        var baseValue = this.GetType().GetProperty(basePropId)?.GetValue(this) ?? 0;
        float value = Convert.ToSingle(baseValue);

        // Multipliers apply to the stat multiplicatively.
        // They are first summed, with the sole restriction that they may not go below zero.
        float statMultiplier = 1.0f;
        if (this.multipliers.ContainsKey(propName))
        {
            var multipliers = this.multipliers[propName].Values.ToList();
            foreach (float multiplier in multipliers)
            {
                statMultiplier += multiplier;
            }
        }

        if (statMultiplier < 0.0f)
        {
            statMultiplier = 0.0f;
        }

        // Apply the cumulative multiplier to the stat.
        if (!Mathf.IsEqualApprox(statMultiplier, 1.0f))
        {
            value *= statMultiplier;
        }

        // Add all modifiers to the stat.
        if (this.modifiers.ContainsKey(propName))
        {
            var modifiers = this.modifiers[propName].Values.ToList();
            foreach (int modifier in modifiers)
            {
                value += modifier;
            }
        }

        // Finally, don't allow values to drop below zero.
        value = Mathf.Round(Mathf.Max(value, 0.0f));

        // Here's where we assign the value to the stat. For instance, if the `propName` argument is
        // "attack", this sets the Attack property to the calculated value.
        switch (propName)
        {
            case "max_health":
                this.MaxHealth = (int)value;
                break;
            case "max_energy":
                this.MaxEnergy = (int)value;
                break;
            case "attack":
                this.Attack = (int)value;
                break;
            case "defense":
                this.Defense = (int)value;
                break;
            case "speed":
                this.Speed = (int)value;
                break;
            case "hit_chance":
                this.HitChance = (int)value;
                break;
            case "evasion":
                this.Evasion = (int)value;
                break;
        }
    }

    /// <summary>
    /// Find the first unused integer in a stat's modifiers keys.
    /// isModifier determines whether the id is determined from the modifier or multiplier dictionary.
    /// </summary>
    private int GenerateUniqueId(string statName, bool isModifier = true)
    {
        // Generate an ID for either modifiers or multipliers.
        var dictionary = isModifier ? this.modifiers : this.multipliers;

        // If there are no keys, we return `0`, which is our first valid unique id. Without existing
        // keys, calling methods like `Array.back()` will trigger an error.
        if (!dictionary.ContainsKey(statName) || dictionary[statName].Keys.Count == 0)
        {
            return 0;
        }
        else
        {
            // We always start from the last key, which will always be the highest number, even if we
            // remove modifiers.
            var keys = dictionary[statName].Keys.ToList();
            keys.Sort();
            return keys[keys.Count - 1] + 1;
        }
    }
}
