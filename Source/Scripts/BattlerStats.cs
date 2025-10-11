using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Numerically represents the characteristics of a specific Battler.
/// </summary>
public partial class BattlerStats : Resource
{
    /// <summary>
    /// A list of all properties that can receive bonuses.
    /// </summary>
    private static readonly string[] ModifiableStats = {
        "MaxHealth", "MaxEnergy", "Attack", "Defense", "Speed", "HitChance", "Evasion"
    };

    /// <summary>
    /// Emitted when Health has reached 0.
    /// </summary>
    [Signal]
    public delegate void HealthDepletedEventHandler();

    /// <summary>
    /// Emitted whenever Health changes.
    /// </summary>
    [Signal]
    public delegate void HealthChangedEventHandler();

    /// <summary>
    /// Emitted whenever Energy changes.
    /// </summary>
    [Signal]
    public delegate void EnergyChangedEventHandler();

    /// <summary>
    /// The battler's elemental affinity. Determines which attacks are more or less effective against this battler.
    /// </summary>
    [Export]
    public ElementsType Affinity { get; set; } = ElementsType.None;

    [ExportGroup("Stats")]
    [Export]
    public int BaseMaxHealth { get; set; } = 100;
    [Export]
    public int BaseMaxEnergy { get; set; } = 6;
    [Export]
    public int BaseAttack { get; set; } = 10;
    [Export]
    public int BaseDefense { get; set; } = 10;
    [Export]
    public int BaseSpeed { get; set; } = 70;
    [Export]
    public int BaseHitChance { get; set; } = 100;
    [Export]
    public int BaseEvasion { get; set; } = 0;

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

    // The properties below store a list of modifiers for each property listed in ModifiableStats.
    // Dictionary keys are the name of the property (string).
    // Dictionary values are another dictionary, with uid/modifier pairs.
    private Dictionary<string, Dictionary<int, int>> _modifiers = new Dictionary<string, Dictionary<int, int>>();
    private Dictionary<string, Dictionary<int, float>> _multipliers = new Dictionary<string, Dictionary<int, float>>();

    public BattlerStats()
    {
        foreach (string propName in ModifiableStats)
        {
            _modifiers[propName] = new Dictionary<int, int>();
            _multipliers[propName] = new Dictionary<int, float>();
        }
    }

    /// <summary>
    /// Initializes the battler stats with max health and energy.
    /// </summary>
    public void Initialize()
    {
        _health = MaxHealth;
        _energy = 0;
    }

    /// <summary>
    /// Adds a modifier that affects the stat with the given statName and returns its unique id.
    /// </summary>
    public int AddModifier(string statName, int value)
    {
        if (Array.IndexOf(ModifiableStats, statName) == -1)
        {
            GD.PrintErr($"Trying to add a modifier to a nonexistent stat: {statName}");
            return -1;
        }

        int id = GenerateUniqueId(statName, true);
        _modifiers[statName][id] = value;
        RecalculateAndUpdate(statName);

        // Returning the id allows the caller to bind it to a signal. For instance
        // with equipment, to call RemoveModifier() upon removing the equipment.
        return id;
    }

    /// <summary>
    /// Adds a multiplier that affects the stat with the given statName and returns its unique id.
    /// </summary>
    public int AddMultiplier(string statName, float value)
    {
        if (Array.IndexOf(ModifiableStats, statName) == -1)
        {
            GD.PrintErr($"Trying to add a multiplier to a nonexistent stat: {statName}");
            return -1;
        }

        int id = GenerateUniqueId(statName, false);
        _multipliers[statName][id] = value;
        RecalculateAndUpdate(statName);

        return id;
    }

    /// <summary>
    /// Removes a modifier associated with the given statName.
    /// </summary>
    public void RemoveModifier(string statName, int id)
    {
        if (!_modifiers[statName].ContainsKey(id))
        {
            GD.PrintErr($"Stat {statName} does not have a modifier with ID '{id}'.");
            return;
        }

        _modifiers[statName].Remove(id);
        RecalculateAndUpdate(statName);
    }

    /// <summary>
    /// Removes a multiplier associated with the given statName.
    /// </summary>
    public void RemoveMultiplier(string statName, int id)
    {
        if (!_multipliers[statName].ContainsKey(id))
        {
            GD.PrintErr($"Stat {statName} does not have a multiplier with ID '{id}'.");
            return;
        }

        _multipliers[statName].Remove(id);
        RecalculateAndUpdate(statName);
    }

    /// <summary>
    /// Calculates the final value of a single stat. That is, its base value with all modifiers applied.
    /// </summary>
    private void RecalculateAndUpdate(string propName)
    {
        if (Array.IndexOf(ModifiableStats, propName) == -1)
        {
            GD.PrintErr($"Cannot update battler stat '{propName}'! Stat name is invalid!");
            return;
        }

        // All our property names follow a pattern: the base stat has the same identifier as the final
        // stat with the "Base" prefix.
        string basePropId = "Base" + propName;
        if (!HasProperty(basePropId))
        {
            GD.PrintErr($"Cannot update battler stat {propName}! Stat does not have base value!");
            return;
        }

        // Get the base value using reflection since we can't use Get() method directly
        float value = GetBaseValue(basePropId);

        // Multipliers apply to the stat multiplicatively.
        // They are first summed, with the sole restriction that they may not go below zero.
        float statMultiplier = 1.0f;
        var multipliers = _multipliers[propName].Values;
        foreach (float multiplier in multipliers)
        {
            statMultiplier += multiplier;
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
        var modifiers = _modifiers[propName].Values;
        foreach (int modifier in modifiers)
        {
            value += modifier;
        }

        // Finally, don't allow values to drop below zero.
        value = Mathf.Round(Mathf.Max(value, 0.0f));

        // Set the calculated value to the appropriate property
        SetStatValue(propName, (int)value);
    }

    /// <summary>
    /// Helper method to get the base value of a stat property using reflection
    /// </summary>
    private float GetBaseValue(string basePropId)
    {
        return basePropId switch
        {
            "BaseMaxHealth" => BaseMaxHealth,
            "BaseMaxEnergy" => BaseMaxEnergy,
            "BaseAttack" => BaseAttack,
            "BaseDefense" => BaseDefense,
            "BaseSpeed" => BaseSpeed,
            "BaseHitChance" => BaseHitChance,
            "BaseEvasion" => BaseEvasion,
            _ => 0f
        };
    }

    /// <summary>
    /// Helper method to set the value of a stat property
    /// </summary>
    private void SetStatValue(string propName, int value)
    {
        switch (propName)
        {
            case "MaxHealth":
                MaxHealth = value;
                break;
            case "MaxEnergy":
                MaxEnergy = value;
                break;
            case "Attack":
                Attack = value;
                break;
            case "Defense":
                Defense = value;
                break;
            case "Speed":
                Speed = value;
                break;
            case "HitChance":
                HitChance = value;
                break;
            case "Evasion":
                Evasion = value;
                break;
        }
    }

    /// <summary>
    /// Helper method to check if a property exists
    /// </summary>
    private bool HasProperty(string propertyName)
    {
        return propertyName switch
        {
            "BaseMaxHealth" or "BaseMaxEnergy" or "BaseAttack" or "BaseDefense" or "BaseSpeed" or "BaseHitChance" or "BaseEvasion" => true,
            _ => false
        };
    }

    /// <summary>
    /// Find the first unused integer in a stat's modifiers keys.
    /// isModifier determines whether the id is determined from the modifier or multiplier dictionary.
    /// </summary>
    private int GenerateUniqueId(string statName, bool isModifier = true)
    {
        // Generate an ID for either modifiers or multipliers.
        Dictionary<int, int> dictionary = _modifiers[statName];
        if (!isModifier)
        {
            dictionary = new Dictionary<int, int>();
            var multiplierDict = _multipliers[statName];
            foreach (var kvp in multiplierDict)
            {
                dictionary[kvp.Key] = (int)(kvp.Value * 1000); // Convert float to int for comparison
            }
        }

        // If there are no keys, we return 0, which is our first valid unique id. Without existing
        // keys, calling methods like Array.Max() will trigger an error.
        var keys = dictionary.Keys;
        if (keys.Count == 0)
        {
            return 0;
        }
        else
        {
            // We always start from the last key, which will always be the highest number, even if we
            // remove modifiers.
            int maxKey = 0;
            foreach (int key in keys)
            {
                if (key > maxKey) maxKey = key;
            }
            return maxKey + 1;
        }
    }
}

/// <summary>
/// Enum for elemental types used in battler stats
/// </summary>
public enum ElementsType
{
    None,
    Fire,
    Water,
    Earth,
    Air,
    Light,
    Dark
}
