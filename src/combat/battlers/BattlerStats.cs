
// <copyright file="BattlerStats.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts.Combat.Battlers;
/// <summary>
/// Numerically represents the characteristics of a specific <see cref="Battler"/>.
/// </summary>
[GlobalClass]
public partial class BattlerStats : Resource
{
    /// <summary>
    /// A list of all properties that can receive bonuses.
    /// </summary>
    private static readonly string[] ModifiableStats =
    {
        "max_health", "max_energy", "attack", "defense", "speed", "hit_chance", "evasion",
    };

    private int baseAttack = 10;
    private int baseDefense = 10;
    private int baseSpeed = 70;
    private int baseHitChance = 100;
    private int baseEvasion;
    private int health;
    private int energy;
    private Dictionary<string, Dictionary<int, int>> modifiers = new Dictionary<string, Dictionary<int, int>>();
    private Dictionary<string, Dictionary<int, float>> multipliers = new Dictionary<string, Dictionary<int, float>>();

    /// <summary>
    /// Initializes a new instance of the <see cref="BattlerStats"/> class.
    /// Sets up the initial calculated values and initializes modifier/multiplier dictionaries.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the battler's elemental affinity. Determines which attacks are more or less effective against
    /// this battler.
    /// </summary>
    [ExportCategory("Elements")]
    [Export]
    public Element Affinity { get; set; } = Element.None;

    /// <summary>
    /// Gets or sets the base maximum health value.
    /// </summary>
    [ExportCategory("Stats")]
    [Export]
    public int BaseMaxHealth { get; set; } = 100;

    /// <summary>
    /// Gets or sets the base maximum energy value.
    /// </summary>
    [Export]
    public int BaseMaxEnergy { get; set; } = 6;

    /// <summary>
    /// Gets or sets the base attack value.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the base defense value.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the base speed value.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the base hit chance value.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the base evasion value.
    /// </summary>
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

    // The properties below store a list of modifiers for each property listed in MODIFIABLE_STATS.
    // Dictionary keys are the name of the property (String).
    // Dictionary values are another dictionary, with uid/modifier pairs.

    /// <summary>
    /// Gets the calculated maximum health value, including all modifiers and multipliers.
    /// </summary>
    public int MaxHealth { get; private set; }

    /// <summary>
    /// Gets the calculated maximum energy value, including all modifiers and multipliers.
    /// </summary>
    public int MaxEnergy { get; private set; }

    /// <summary>
    /// Gets the calculated attack value, including all modifiers and multipliers.
    /// </summary>
    public int Attack { get; private set; }

    /// <summary>
    /// Gets the calculated defense value, including all modifiers and multipliers.
    /// </summary>
    public int Defense { get; private set; }

    /// <summary>
    /// Gets the calculated speed value, including all modifiers and multipliers.
    /// </summary>
    public int Speed { get; private set; }

    /// <summary>
    /// Gets the calculated hit chance value, including all modifiers and multipliers.
    /// </summary>
    public int HitChance { get; private set; }

    /// <summary>
    /// Gets the calculated evasion value, including all modifiers and multipliers.
    /// </summary>
    public int Evasion { get; private set; }

    /// <summary>
    /// Gets or sets the current health value. Setting this value clamps it between 0 and <see cref="MaxHealth"/>,
    /// emits the <see cref="HealthChanged"/> signal, and emits the <see cref="HealthDepleted"/> signal if health reaches 0.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the current energy value. Setting this value clamps it between 0 and <see cref="MaxEnergy"/>
    /// and emits the <see cref="EnergyChanged"/> signal.
    /// </summary>
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

    /// <summary>
    /// Initializes the battler's current health to the maximum health value.
    /// This should be called after the battler stats are fully set up.
    /// </summary>
    public void Initialize()
    {
        this.Health = this.MaxHealth;
    }

    /// <summary>
    /// Adds a modifier that affects the stat with the given `statName` and returns its unique id.
    /// </summary>
    /// <param name="statName">The name of the stat to modify.</param>
    /// <param name="value">The modifier value to add.</param>
    /// <returns>The unique id of the added modifier.</returns>
    public int AddModifier(string statName, int value)
    {
        System.Diagnostics.Debug.Assert(ModifiableStats.Contains(statName), "Trying to add a modifier to a nonexistent stat.");

        int id = this.GenerateUniqueId(statName);
        this.modifiers[statName][id] = value;
        this.RecalculateAndUpdate(statName);

        // Returning the id allows the caller to bind it to a signal. For instance
        // with equipment, to call `RemoveModifier()` upon removing the equipment.
        return id;
    }

    /// <summary>
    /// Adds a multiplier that affects the stat with the given `statName` and returns its unique id.
    /// </summary>
    /// <param name="statName">The name of the stat to multiply.</param>
    /// <param name="value">The multiplier value to add.</param>
    /// <returns>The unique id of the added multiplier.</returns>
    public int AddMultiplier(string statName, float value)
    {
        System.Diagnostics.Debug.Assert(ModifiableStats.Contains(statName), "Trying to add a modifier to a nonexistent stat.");

        int id = this.GenerateUniqueId(statName, false);
        this.multipliers[statName][id] = value;
        this.RecalculateAndUpdate(statName);

        return id;
    }

    /// <summary>
    /// Removes a modifier associated with the given statName.
    /// </summary>
    /// <param name="statName">The name of the stat to remove the modifier from.</param>
    /// <param name="id">The unique id of the modifier to remove.</param>
    public void RemoveModifier(string statName, int id)
    {
        System.Diagnostics.Debug.Assert(this.modifiers.ContainsKey(statName) && this.modifiers[statName].ContainsKey(id), $"Stat {statName} does not have a modifier with ID '{id}'.");

        this.modifiers[statName].Remove(id);
        this.RecalculateAndUpdate(statName);
    }

    /// <summary>
    /// Removes a multiplier associated with the given statName.
    /// </summary>
    /// <param name="statName">The name of the stat to remove the multiplier from.</param>
    /// <param name="id">The unique id of the multiplier to remove.</param>
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
    /// <param name="propName">The name of the stat property to recalculate.</param>
    private void RecalculateAndUpdate(string propName)
    {
        // All our property names follow a pattern: the base stat has the same identifier as the final
        // stat with the "base_" prefix.
        string basePropId = "base_" + propName;

        // Get the base value using reflection
        var baseValue = this.GetType().GetProperty(basePropId)?.GetValue(this) ?? 0;
        float value = Convert.ToSingle(baseValue, System.Globalization.CultureInfo.InvariantCulture);

        // Apply multipliers and modifiers to the stat
        value = this.ApplyStatMultipliers(propName, value);
        value = this.ApplyStatModifiers(propName, value);

        // Finally, don't allow values to drop below zero.
        value = Mathf.Round(Mathf.Max(value, 0.0f));

        // Assign the value to the stat by name
        this.AssignStatValue(propName, (int) value);
    }

    /// <summary>
    /// Applies all multipliers to a stat value.
    /// </summary>
    /// <param name="propName">The name of the stat.</param>
    /// <param name="value">The base stat value.</param>
    /// <returns>The value after multipliers are applied.</returns>
    private float ApplyStatMultipliers(string propName, float value)
    {
        // Multipliers apply to the stat multiplicatively.
        // They are first summed, with the sole restriction that they may not go below zero.
        float statMultiplier = 1.0f;
        if (this.multipliers.TryGetValue(propName, out Dictionary<int, float>? multiplierDict))
        {
            var multiplierValues = multiplierDict.Values.ToList();
            foreach (float multiplier in multiplierValues)
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

        return value;
    }

    /// <summary>
    /// Applies all modifiers to a stat value.
    /// </summary>
    /// <param name="propName">The name of the stat.</param>
    /// <param name="value">The current stat value.</param>
    /// <returns>The value after modifiers are applied.</returns>
    private float ApplyStatModifiers(string propName, float value)
    {
        // Add all modifiers to the stat.
        if (this.modifiers.TryGetValue(propName, out Dictionary<int, int>? modifierDict))
        {
            var modifierValues = modifierDict.Values.ToList();
            foreach (int modifier in modifierValues)
            {
                value += modifier;
            }
        }

        return value;
    }

    /// <summary>
    /// Assigns a calculated stat value to the appropriate property by name.
    /// </summary>
    /// <param name="propName">The name of the stat property.</param>
    /// <param name="value">The calculated value to assign.</param>
    private void AssignStatValue(string propName, int value)
    {
        switch (propName)
        {
            case "max_health":
                this.MaxHealth = value;
                break;
            case "max_energy":
                this.MaxEnergy = value;
                break;
            case "attack":
                this.Attack = value;
                break;
            case "defense":
                this.Defense = value;
                break;
            case "speed":
                this.Speed = value;
                break;
            case "hit_chance":
                this.HitChance = value;
                break;
            case "evasion":
                this.Evasion = value;
                break;
        }
    }

    /// <summary>
    /// Find the first unused integer in a stat's modifiers keys.
    /// isModifier determines whether the id is determined from the modifier or multiplier dictionary.
    /// </summary>
    /// <param name="statName"></param>
    /// <param name="isModifier"></param>
    private int GenerateUniqueId(string statName, bool isModifier = true)
    {
        // Generate an ID for either modifiers or multipliers.
        Dictionary<string, Dictionary<int, object>> dictionary = isModifier
            ? this.modifiers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToDictionary(mkvp => mkvp.Key, mkvp => (object) mkvp.Value))
            : this.multipliers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToDictionary(mkvp => mkvp.Key, mkvp => (object) mkvp.Value));

        // If there are no keys, we return `0`, which is our first valid unique id. Without existing
        // keys, calling methods like `Array.back()` will trigger an error.
        if (!dictionary.TryGetValue(statName, out Dictionary<int, object>? value) || value.Keys.Count == 0)
        {
            return 0;
        }
        else
        {
            // We always start from the last key, which will always be the highest number, even if we
            // remove modifiers.
            var keys = value.Keys.ToList();
            keys.Sort();
            return keys[keys.Count - 1] + 1;
        }
    }
}
