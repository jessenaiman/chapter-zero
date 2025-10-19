using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Domain.Models
{
    /// <summary>
    /// Represents a character in the game with all associated data and properties.
    /// Follows Single Responsibility Principle through composition of specialized classes.
    /// </summary>
    public class Character
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Character"/> class.
        /// </summary>
        /// <param name="identity">The character's identity information.</param>
        /// <param name="characterClass">The character's class information.</param>
        /// <param name="appearance">The character's appearance information.</param>
        /// <param name="stats">The character's stats information.</param>
        /// <param name="state">The character's current state.</param>
        /// <param name="progression">The character's progression information.</param>
        public Character(CharacterIdentity identity, CharacterClass characterClass, CharacterAppearance appearance, CharacterStats stats, CharacterState? state = null, CharacterProgression? progression = null)
        {
            this.Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            this.Class = characterClass ?? throw new ArgumentNullException(nameof(characterClass));
            this.Appearance = appearance ?? throw new ArgumentNullException(nameof(appearance));
            this.Stats = stats ?? throw new ArgumentNullException(nameof(stats));
            this.State = state ?? new CharacterState();
            this.Progression = progression ?? new CharacterProgression();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Character"/> class with basic parameters.
        /// </summary>
        /// <param name="name">The character's name.</param>
        /// <param name="characterClass">The character's class.</param>
        /// <param name="race">The character's race.</param>
        public Character(string name, CharacterClass characterClass, CharacterRace race)
        {
            this.Identity = new CharacterIdentity(
                id: Guid.NewGuid().ToString(),
                name: name,
                description: $"{name} the {characterClass}");

            this.Class = characterClass;
            this.Appearance = new CharacterAppearance();
            this.Stats = new CharacterStats();
            this.Stats.ApplyRacialModifiers(race);
            this.State = new CharacterState();
            this.Progression = new CharacterProgression();
        }

        /// <summary>
        /// Gets the character's identity information.
        /// </summary>
        public CharacterIdentity Identity { get; }

        /// <summary>
        /// Gets the character's class information.
        /// </summary>
        public CharacterClass Class { get; }

        /// <summary>
        /// Gets the character's appearance information.
        /// </summary>
        public CharacterAppearance Appearance { get; }

        /// <summary>
        /// Gets the character's stats information.
        /// </summary>
        public CharacterStats Stats { get; }

        /// <summary>
        /// Gets the character's current state.
        /// </summary>
        public CharacterState State { get; }

        /// <summary>
        /// Gets the character's progression information.
        /// </summary>
        public CharacterProgression Progression { get; }

        // Convenience properties for backward compatibility
        /// <summary>
        /// Gets the unique identifier for the character.
        /// </summary>
        public string Id => this.Identity.Id;

        /// <summary>
        /// Gets the character's name.
        /// </summary>
        public string Name => this.Identity.Name;

        /// <summary>
        /// Gets the character's description.
        /// </summary>
        public string Description => this.Identity.Description;

        /// <summary>
        /// Gets the category of this character.
        /// </summary>
        public string Category => this.Identity.Category;

        /// <summary>
        /// Gets the icon resource path for this character.
        /// </summary>
        public string IconPath => this.Identity.IconPath;

        /// <summary>
        /// Gets or sets the character's current experience points.
        /// </summary>
        public int CurrentExperience
        {
            get => this.Progression.CurrentExperience;
            set => this.Progression.CurrentExperience = value;
        }

        /// <summary>
        /// Gets or sets the character's gold or currency amount.
        /// </summary>
        public int Gold
        {
            get => this.Progression.Gold;
            set => this.Progression.Gold = value;
        }

        /// <summary>
        /// Gets or sets whether this character is available for selection.
        /// </summary>
        public bool IsAvailable
        {
            get => this.Progression.IsAvailable;
            set => this.Progression.IsAvailable = value;
        }

        /// <summary>
        /// Gets or sets the unlock condition for this character.
        /// </summary>
        public string UnlockCondition
        {
            get => this.Progression.UnlockCondition;
            set => this.Progression.UnlockCondition = value;
        }

        /// <summary>
        /// Gets or sets the character's current location in the game world.
        /// </summary>
        public string CurrentLocation
        {
            get => this.State.CurrentLocation;
            set => this.State.CurrentLocation = value;
        }

        /// <summary>
        /// Gets or sets the character's current status.
        /// </summary>
        public string Status
        {
            get => this.State.Status;
            set => this.State.Status = value;
        }

        /// <summary>
        /// Gets or sets whether the character is currently in combat.
        /// </summary>
        public bool IsInCombat
        {
            get => this.State.IsInCombat;
            set => this.State.IsInCombat = value;
        }

        /// <summary>
        /// Gets or sets the character's current health points.
        /// </summary>
        public int CurrentHealth
        {
            get => this.State.CurrentHealth;
            set => this.State.CurrentHealth = value;
        }

        /// <summary>
        /// Gets or sets the character's current mana points.
        /// </summary>
        public int CurrentMana
        {
            get => this.State.CurrentMana;
            set => this.State.CurrentMana = value;
        }

        /// <summary>
        /// Creates a copy of this character.
        /// </summary>
        /// <returns>A new instance of <see cref="Character"/> with the same values.</returns>
        public Character Clone()
        {
            return new Character(
                identity: this.Identity.Clone(),
                characterClass: this.Class.Clone(),
                appearance: this.Appearance.Clone(),
                stats: this.Stats.Clone(),
                state: this.State.Clone(),
                progression: this.Progression.Clone());
        }

        /// <summary>
        /// Heals the character by the specified amount.
        /// </summary>
        /// <param name="amount">The amount to heal.</param>
        /// <returns>The actual amount healed (capped at max health).</returns>
        public int Heal(int amount)
        {
            int actualHeal = this.Stats.Heal(amount);
            this.State.CurrentHealth = this.Stats.Health;
            return actualHeal;
        }

        /// <summary>
        /// Damages the character by the specified amount.
        /// </summary>
        /// <param name="amount">The amount of damage to take.</param>
        /// <returns>The actual damage taken.</returns>
        public int TakeDamage(int amount)
        {
            int actualDamage = this.Stats.TakeDamage(amount);
            this.State.CurrentHealth = this.Stats.Health;

            if (this.State.CurrentHealth <= 0)
            {
                this.State.Status = "Dead";
            }

            return actualDamage;
        }

        /// <summary>
        /// Adds experience points to the character and handles level progression.
        /// </summary>
        /// <param name="exp">The experience points to add.</param>
        /// <returns><see langword="true"/> if the character leveled up, <see langword="false"/> otherwise.</returns>
        public bool AddExperience(int exp)
        {
            bool leveledUp = this.Stats.AddExperience(exp);
            this.Progression.CurrentExperience = this.Stats.Experience;
            return leveledUp;
        }

        /// <summary>
        /// Checks if the character is alive.
        /// </summary>
        /// <returns><see langword="true"/> if the character is alive, <see langword="false"/> otherwise.</returns>
        public bool IsAlive()
        {
            return this.State.IsAlive();
        }

        /// <summary>
        /// Resets the character to default state while preserving identity and class.
        /// </summary>
        public void ResetToDefault()
        {
            this.State.ResetToDefault();
            this.Progression.ResetToDefault();
            this.Stats.ResetToDefault();
        }

        /// <summary>
        /// Gets the character's level based on their stats.
        /// </summary>
        /// <returns>The character's current level.</returns>
        public int GetLevel()
        {
            return this.Stats.Level;
        }

        /// <summary>
        /// Gets the character's current health percentage.
        /// </summary>
        /// <returns>The health percentage as a value between 0 and 1.</returns>
        public float GetHealthPercentage()
        {
            return this.Stats.GetHealthPercentage(this.State.CurrentHealth);
        }

        /// <summary>
        /// Gets the character's current mana percentage.
        /// </summary>
        /// <returns>The mana percentage as a value between 0 and 1.</returns>
        public float GetManaPercentage()
        {
            return this.Stats.GetManaPercentage(this.State.CurrentMana);
        }

        /// <summary>
        /// Gets the display name with category prefix if applicable.
        /// </summary>
        /// <returns>The formatted display name.</returns>
        public string GetDisplayName()
        {
            return this.Identity.GetDisplayName();
        }
    }
}
