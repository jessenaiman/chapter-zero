using Godot;

namespace OmegaSpiral.Domain.Models
{
    /// <summary>
    /// Represents a character in the game with all associated data and properties.
    /// </summary>
    public class Character
    {
        /// <summary>
        /// Gets or sets the unique identifier for the character.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the character's name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the character's description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the character's class information.
        /// </summary>
        public CharacterClass Class { get; set; } = new CharacterClass();

        /// <summary>
        /// Gets or sets the character's appearance information.
        /// </summary>
        public CharacterAppearance Appearance { get; set; } = new CharacterAppearance();

        /// <summary>
        /// Gets or sets the character's stats information.
        /// </summary>
        public CharacterStats Stats { get; set; } = new CharacterStats();

        /// <summary>
        /// Gets or sets whether this character is available for selection.
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Gets or sets the unlock condition for this character.
        /// </summary>
        public string UnlockCondition { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the icon resource path for this character.
        /// </summary>
        public string IconPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the category of this character (e.g., "Player", "NPC", "Boss").
        /// </summary>
        public string Category { get; set; } = "Player";

        /// <summary>
        /// Gets or sets the character's current experience points.
        /// </summary>
        public int CurrentExperience { get; set; }

        /// <summary>
        /// Gets or sets the character's gold or currency amount.
        /// </summary>
        public int Gold { get; set; }

        /// <summary>
        /// Gets or sets the character's current location in the game world.
        /// </summary>
        public string CurrentLocation { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the character's current status (e.g., "Alive", "Dead", "Stunned").
        /// </summary>
        public string Status { get; set; } = "Alive";

        /// <summary>
        /// Gets or sets whether the character is currently in combat.
        /// </summary>
        public bool IsInCombat { get; set; }

        /// <summary>
        /// Gets or sets the character's current health points.
        /// </summary>
        public int CurrentHealth { get; set; }

        /// <summary>
        /// Gets or sets the character's current mana points.
        /// </summary>
        public int CurrentMana { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Character"/> class.
        /// </summary>
        public Character()
        {
            // Initialize with default values
            this.CurrentExperience = 0;
            this.Gold = 0;
            this.CurrentHealth = 100;
            this.CurrentMana = 50;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Character"/> class with specified parameters.
        /// </summary>
        /// <param name="id">The unique identifier for the character.</param>
        /// <param name="name">The character's name.</param>
        /// <param name="description">The character's description.</param>
        /// <param name="characterClass">The character's class information.</param>
        /// <param name="appearance">The character's appearance information.</param>
        /// <param name="stats">The character's stats information.</param>
        /// <param name="isAvailable">Whether this character is available for selection.</param>
        /// <param name="unlockCondition">The unlock condition for this character.</param>
        /// <param name="iconPath">The icon resource path for this character.</param>
        /// <param name="category">The category of this character.</param>
        /// <param name="currentExperience">The character's current experience points.</param>
        /// <param name="gold">The character's gold or currency amount.</param>
        /// <param name="currentLocation">The character's current location in the game world.</param>
        /// <param name="status">The character's current status.</param>
        /// <param name="isInCombat">Whether the character is currently in combat.</param>
        public Character(string id, string name, string description, CharacterClass characterClass,
                        CharacterAppearance appearance, CharacterStats stats, bool isAvailable = true,
                        string unlockCondition = "", string iconPath = "", string category = "Player",
                        int currentExperience = 0, int gold = 0, string currentLocation = "",
                        string status = "Alive", bool isInCombat = false)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Class = characterClass;
            this.Appearance = appearance;
            this.Stats = stats;
            this.IsAvailable = isAvailable;
            this.UnlockCondition = unlockCondition;
            this.IconPath = iconPath;
            this.Category = category;
            this.CurrentExperience = currentExperience;
            this.Gold = gold;
            this.CurrentLocation = currentLocation;
            this.Status = status;
            this.IsInCombat = isInCombat;
            this.CurrentHealth = this.Stats?.Health ?? 100;
            this.CurrentMana = this.Stats?.Mana ?? 50;
        }

        /// <summary>
        /// Creates a copy of this character.
        /// </summary>
        /// <returns>A new instance of <see cref="Character"/> with the same values.</returns>
        public Character Clone()
        {
            return new Character
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                Class = this.Class?.Clone(),
                Appearance = this.Appearance?.Clone(),
                Stats = this.Stats?.Clone(),
                IsAvailable = this.IsAvailable,
                UnlockCondition = this.UnlockCondition,
                IconPath = this.IconPath,
                Category = this.Category,
                CurrentExperience = this.CurrentExperience,
                Gold = this.Gold,
                CurrentLocation = this.CurrentLocation,
                Status = this.Status,
                IsInCombat = this.IsInCombat,
                CurrentHealth = this.CurrentHealth,
                CurrentMana = this.CurrentMana
            };
        }

        /// <summary>
        /// Heals the character by the specified amount.
        /// </summary>
        /// <param name="amount">The amount to heal.</param>
        /// <returns>The actual amount healed (capped at max health).</returns>
        public int Heal(int amount)
        {
            if (this.Stats != null)
            {
                int actualHeal = this.Stats.Heal(amount);
                this.CurrentHealth = this.Stats.Health;
                return actualHeal;
            }

            int healAmount = System.Math.Min(amount, this.Stats?.MaxHealth - this.CurrentHealth ?? 0);
            this.CurrentHealth = System.Math.Min(this.CurrentHealth + healAmount, this.Stats?.MaxHealth ?? this.CurrentHealth + healAmount);
            return healAmount;
        }

        /// <summary>
        /// Damages the character by the specified amount.
        /// </summary>
        /// <param name="amount">The amount of damage to take.</param>
        /// <returns>The actual damage taken.</returns>
        public int TakeDamage(int amount)
        {
            if (this.Stats != null)
            {
                int actualDamage = this.Stats.TakeDamage(amount);
                this.CurrentHealth = this.Stats.Health;
                return actualDamage;
            }

            int damageTaken = System.Math.Max(0, amount - (this.Stats?.Defense ?? 0));
            this.CurrentHealth = System.Math.Max(0, this.CurrentHealth - damageTaken);

            if (this.CurrentHealth <= 0)
            {
                this.Status = "Dead";
            }

            return damageTaken;
        }

        /// <summary>
        /// Adds experience points to the character and handles level progression.
        /// </summary>
        /// <param name="exp">The experience points to add.</param>
        /// <returns><see langword="true"/> if the character leveled up, <see langword="false"/> otherwise.</returns>
        public bool AddExperience(int exp)
        {
            if (this.Stats != null)
            {
                bool leveledUp = this.Stats.AddExperience(exp);
                this.CurrentExperience = this.Stats.Experience;
                return leveledUp;
            }

            this.CurrentExperience += exp;
            return false; // No level up logic without Stats
        }

        /// <summary>
        /// Checks if the character is alive.
        /// </summary>
        /// <returns><see langword="true"/> if the character is alive, <see langword="false"/> otherwise.</returns>
        public bool IsAlive()
        {
            return this.Status != "Dead" && this.CurrentHealth > 0;
        }

        /// <summary>
        /// Resets the character to default state while preserving identity and class.
        /// </summary>
        public void ResetToDefault()
        {
            if (this.Stats != null)
            {
                this.Stats.ResetToDefault();
                this.CurrentHealth = this.Stats.Health;
                this.CurrentMana = this.Stats.Mana;
            }

            this.Status = "Alive";
            this.IsInCombat = false;
            this.CurrentExperience = 0;
            this.Gold = 0;
        }

        /// <summary>
        /// Gets the character's level based on their stats.
        /// </summary>
        /// <returns>The character's current level.</returns>
        public int GetLevel()
        {
            return this.Stats?.Level ?? 1;
        }

        /// <summary>
        /// Gets the character's current health percentage.
        /// </summary>
        /// <returns>The health percentage as a value between 0 and 1.</returns>
        public float GetHealthPercentage()
        {
            if (this.Stats?.MaxHealth > 0)
            {
                return (float)this.CurrentHealth / this.Stats.MaxHealth;
            }

            return 0f;
        }

        /// <summary>
        /// Gets the character's current mana percentage.
        /// </summary>
        /// <returns>The mana percentage as a value between 0 and 1.</returns>
        public float GetManaPercentage()
        {
            if (this.Stats?.MaxMana > 0)
            {
                return (float)this.CurrentMana / this.Stats.MaxMana;
            }

            return 0f;
        }

        /// <summary>
        /// Gets the display name with category prefix if applicable.
        /// </summary>
        /// <returns>The formatted display name.</returns>
        public string GetDisplayName()
        {
            if (string.IsNullOrEmpty(this.Category) || this.Category == "Player")
            {
                return this.Name;
            }

            return $"{this.Category}: {this.Name}";
        }
    }
}
