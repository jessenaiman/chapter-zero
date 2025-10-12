using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Numerically represents the characteristics of a specific <see cref="Battler"/>.
/// </summary>
public partial class BattlerStats : Resource
{
    /// <summary>
    /// A list of all properties that can receive bonuses.
    /// </summary>
    private static readonly string[] ModifiableStats = {
        "max_health", "max_energy", "attack", "defense", "speed", "hit_chance", "evasion"
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

    private int _baseAttack = 10;
    [Export]
    public int BaseAttack
    {
        get => _baseAttack;
        set
        {
            _baseAttack = value;
            _RecalculateAndUpdate("attack");
        }
    }

    private int _baseDefense = 10;
    [Export]
    public int BaseDefense
    {
        get => _baseDefense;
        set
        {
            _baseDefense = value;
            _RecalculateAndUpdate("defense");
        }
    }

    private int _baseSpeed = 70;
    [Export]
    public int BaseSpeed
    {
        get => _baseSpeed;
        set
        {
            _baseSpeed = value;
            _RecalculateAndUpdate("speed");
        }
    }

    private int _baseHitChance = 100;
    [Export]
    public int BaseHitChance
    {
        get => _baseHitChance;
        set
        {
            _baseHitChance = value;
            _RecalculateAndUpdate("hit_chance");
        }
    }

    private int _baseEvasion = 0;
    [Export]
    public int BaseEvasion
    {
        get => _baseEvasion;
        set
        {
            _baseEvasion = value;
            _RecalculateAndUpdate("evasion");
        }
    }

    public int MaxHealth { get; private set; }
    public int MaxEnergy { get; private set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }
    public int Speed { get; private set; }
    public int HitChance { get; private set; }
    public int Evasion { get; private set; }

    private int _health;
    public int Health
    {
        get => _health;
        set
        {
            if (value != _health)
            {
                _health = Mathf.Clamp(value, 0, MaxHealth);

                EmitSignal(SignalName.HealthChanged);
                if (_health == 0)
                {
                    EmitSignal(SignalName.HealthDepleted);
                }
            }
        }
    }

    private int _energy;
    public int Energy
    {
        get => _energy;
        set
        {
            if (value != _energy)
            {
                _energy = Mathf.Clamp(value, 0, MaxEnergy);
                EmitSignal(SignalName.EnergyChanged);
            }
        }
    }

    // The properties below store a list of modifiers for each property listed in MODIFIABLE_STATS.
    // Dictionary keys are the name of the property (String).
    // Dictionary values are another dictionary, with uid/modifier pairs.
    private Dictionary<string, Dictionary<int, int>> _modifiers = new Dictionary<string, Dictionary<int, int>>();
    private Dictionary<string, Dictionary<int, float>> _multipliers = new Dictionary<string, Dictionary<int, float>>();

    public BattlerStats()
    {
        MaxHealth = BaseMaxHealth;
        MaxEnergy = BaseMaxEnergy;
        Attack = BaseAttack;
        Defense = BaseDefense;
        Speed = BaseSpeed;
        HitChance = BaseHitChance;
        Evasion = BaseEvasion;

        foreach (string propName in ModifiableStats)
        {
            _modifiers[propName] = new Dictionary<int, int>();
            _multipliers[propName] = new Dictionary<int, float>();
        }
    }

    public void Initialize()
    {
        Health = MaxHealth;
    }

    /// <summary>
    /// Adds a modifier that affects the stat with the given `statName` and returns its unique id.
    /// </summary>
    public int AddModifier(string statName, int value)
    {
        System.Diagnostics.Debug.Assert(ModifiableStats.Contains(statName), "Trying to add a modifier to a nonexistent stat.");

        int id = _GenerateUniqueId(statName, true);
        _modifiers[statName][id] = value;
        _RecalculateAndUpdate(statName);

        // Returning the id allows the caller to bind it to a signal. For instance
        // with equipment, to call `RemoveModifier()` upon removing the equipment.
        return id;
    }

    /// <summary>
    /// Adds a multiplier that affects the stat with the given `statName` and returns its unique id.
    /// </summary>
    public int AddMultiplier(string statName, float value)
    {
        System.Diagnostics.Debug.Assert(ModifiableStats.Contains(statName), "Trying to add a modifier to a nonexistent stat.");

        int id = _GenerateUniqueId(statName, false);
        _multipliers[statName][id] = value;
        _RecalculateAndUpdate(statName);

        return id;
    }

    /// <summary>
    /// Removes a modifier associated with the given `statName`.
    /// </summary>
    public void RemoveModifier(string statName, int id)
    {
        System.Diagnostics.Debug.Assert(_modifiers.ContainsKey(statName) && _modifiers[statName].ContainsKey(id), $"Stat {statName} does not have a modifier with ID '{id}'.");

        _modifiers[statName].Remove(id);
        _RecalculateAndUpdate(statName);
    }

    public void RemoveMultiplier(string statName, int id)
    {
        System.Diagnostics.Debug.Assert(_multipliers.ContainsKey(statName) && _multipliers[statName].ContainsKey(id), $"Stat {statName} does not have a multiplier with ID '{id}'.");

        _multipliers[statName].Remove(id);
        _RecalculateAndUpdate(statName);
    }

    /// <summary>
    /// Calculates the final value of a single stat. That is, its base value with all modifiers applied.
    /// We reference a stat property name using a string here and update it with reflection.
    /// </summary>
    private void _RecalculateAndUpdate(string propName)
    {
        // All our property names follow a pattern: the base stat has the same identifier as the final
        // stat with the "base_" prefix.
        string basePropId = "base_" + propName;

        // Get the base value using reflection
        var baseValue = GetType().GetProperty(basePropId)?.GetValue(this) ?? 0;
        float value = Convert.ToSingle(baseValue);

        // Multipliers apply to the stat multiplicatively.
        // They are first summed, with the sole restriction that they may not go below zero.
        float statMultiplier = 1.0f;
        if (_multipliers.ContainsKey(propName))
        {
            var multipliers = _multipliers[propName].Values.ToList();
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
        if (_modifiers.ContainsKey(propName))
        {
            var modifiers = _modifiers[propName].Values.ToList();
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
                MaxHealth = (int)value;
                break;
            case "max_energy":
                MaxEnergy = (int)value;
                break;
            case "attack":
                Attack = (int)value;
                break;
            case "defense":
                Defense = (int)value;
                break;
            case "speed":
                Speed = (int)value;
                break;
            case "hit_chance":
                HitChance = (int)value;
                break;
            case "evasion":
                Evasion = (int)value;
                break;
        }
    }

    /// <summary>
    /// Find the first unused integer in a stat's modifiers keys.
    /// isModifier determines whether the id is determined from the modifier or multiplier dictionary.
    /// </summary>
    private int _GenerateUniqueId(string statName, bool isModifier = true)
    {
        // Generate an ID for either modifiers or multipliers.
        var dictionary = isModifier ? _modifiers : _multipliers;

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
