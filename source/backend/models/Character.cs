using OmegaSpiral.Source.Backend.Common;

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
        /// Initializes a new instance of the <see cref="Character"/> class with default values.
        /// Parameterless constructor for serialization/deserialization.
        /// </summary>
        public Character()
        {
            this.Identity = new CharacterIdentity(Guid.NewGuid().ToString(), "Default", "Default character");
            this.Class = new CharacterClass
            {
                Id = "fighter",
                Name = "Fighter",
                Description = "A strong warrior focused on physical combat",
                BaseHealth = 100,
                BaseMana = 50,
                BaseAttack = 10,
                BaseDefense = 5,
                BaseMagic = 5,
                BaseMagicDefense = 5,
                BaseSpeed = 10,
                IconPath = "res://assets/icons/fighter.png"
            };
            this.Appearance = new CharacterAppearance();
            this.Stats = new CharacterStats();
            this.State = new CharacterState();
            this.Progression = new CharacterProgression();
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
        /// Initializes a new instance of the <see cref="Character"/> class with common enum parameters.
        /// </summary>
        /// <param name="name">The character's name.</param>
        /// <param name="characterClass">The character's class from common enums.</param>
        /// <param name="race">The character's race from common enums.</param>
        public Character(string name, OmegaSpiral.Source.Backend.Common.CharacterClass characterClass, OmegaSpiral.Source.Backend.Common.CharacterRace race)
        {
            this.Identity = new CharacterIdentity(
                id: Guid.NewGuid().ToString(),
                name: name,
                description: $"{name} the {characterClass}");

            this.Class = CreateCharacterClassFromCommonEnum(characterClass);
            this.Appearance = new CharacterAppearance();
            this.Stats = new CharacterStats();
            this.Stats.ApplyRacialModifiers(race);
            this.State = new CharacterState();
            this.Progression = new CharacterProgression();
        }

        /// <summary>
        /// Gets or sets the character's identity information.
        /// </summary>
        public CharacterIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets the character's class information.
        /// </summary>
        public CharacterClass Class { get; set; }

        /// <summary>
        /// Gets or sets the character's appearance information.
        /// </summary>
        public CharacterAppearance Appearance { get; set; }

        /// <summary>
        /// Gets or sets the character's stats information.
        /// </summary>
        public CharacterStats Stats { get; set; }

        /// <summary>
        /// Gets or sets the character's current state.
        /// </summary>
        public CharacterState State { get; set; }

        /// <summary>
        /// Gets or sets the character's progression information.
        /// </summary>
        public CharacterProgression Progression { get; set; }

        // Convenience properties for backward compatibility
        /// <summary>
        /// Gets or sets the unique identifier for the character.
        /// </summary>
        public string Id
        {
            get => this.Identity.Id;
            set => this.Identity = new CharacterIdentity(value, this.Identity.Name, this.Identity.Description, this.Identity.Category, this.Identity.IconPath);
        }

        /// <summary>
        /// Gets or sets the character's name.
        /// </summary>
        public string Name
        {
            get => this.Identity.Name;
            set => this.Identity = new CharacterIdentity(this.Identity.Id, value, this.Identity.Description, this.Identity.Category, this.Identity.IconPath);
        }

        /// <summary>
        /// Gets or sets the character's description.
        /// </summary>
        public string Description
        {
            get => this.Identity.Description;
            set => this.Identity = new CharacterIdentity(this.Identity.Id, this.Identity.Name, value, this.Identity.Category, this.Identity.IconPath);
        }

        /// <summary>
        /// Gets or sets the category of this character.
        /// </summary>
        public string Category
        {
            get => this.Identity.Category;
            set => this.Identity = new CharacterIdentity(this.Identity.Id, this.Identity.Name, this.Identity.Description, value, this.Identity.IconPath);
        }

        /// <summary>
        /// Gets or sets the icon resource path for this character.
        /// </summary>
        public string IconPath
        {
            get => this.Identity.IconPath;
            set => this.Identity = new CharacterIdentity(this.Identity.Id, this.Identity.Name, this.Identity.Description, this.Identity.Category, value);
        }

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
            var clone = new Character
            {
                Identity = this.Identity.Clone(),
                Class = this.Class.Clone(),
                Appearance = this.Appearance.Clone(),
                Stats = this.Stats.Clone(),
                State = this.State.Clone(),
                Progression = this.Progression.Clone()
            };
            return clone;
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

        /// <summary>
        /// Creates a domain model CharacterClass from a common enum CharacterClass.
        /// </summary>
        /// <param name="commonClass">The common enum character class.</param>
        /// <returns>A new domain model CharacterClass instance.</returns>
        private static CharacterClass CreateCharacterClassFromCommonEnum(OmegaSpiral.Source.Backend.Common.CharacterClass commonClass)
        {
            return commonClass switch
            {
                OmegaSpiral.Source.Backend.Common.CharacterClass.Fighter => new CharacterClass
                {
                    Id = "fighter",
                    Name = "Fighter",
                    Description = "A strong warrior focused on physical combat",
                    BaseHealth = 10,
                    BaseMana = 50,
                    BaseAttack = 10,
                    BaseDefense = 5,
                    BaseMagic = 5,
                    BaseMagicDefense = 5,
                    BaseSpeed = 10,
                    IconPath = "res://assets/icons/fighter.png"
                },
                OmegaSpiral.Source.Backend.Common.CharacterClass.Mage => new CharacterClass
                {
                    Id = "mage",
                    Name = "Mage",
                    Description = "A spellcaster focused on magical abilities",
                    BaseHealth = 60,
                    BaseMana = 120,
                    BaseAttack = 5,
                    BaseDefense = 3,
                    BaseMagic = 15,
                    BaseMagicDefense = 10,
                    BaseSpeed = 8,
                    IconPath = "res://assets/icons/mage.png"
                },
                OmegaSpiral.Source.Backend.Common.CharacterClass.Priest => new CharacterClass
                {
                    Id = "priest",
                    Name = "Priest",
                    Description = "A healer and support character",
                    BaseHealth = 70,
                    BaseMana = 100,
                    BaseAttack = 6,
                    BaseDefense = 7,
                    BaseMagic = 12,
                    BaseMagicDefense = 12,
                    BaseSpeed = 8,
                    IconPath = "res://assets/icons/priest.png"
                },
                OmegaSpiral.Source.Backend.Common.CharacterClass.Thief => new CharacterClass
                {
                    Id = "thief",
                    Name = "Thief",
                    Description = "A stealthy character focused on agility",
                    BaseHealth = 80,
                    BaseMana = 40,
                    BaseAttack = 12,
                    BaseDefense = 4,
                    BaseMagic = 3,
                    BaseMagicDefense = 4,
                    BaseSpeed = 15,
                    IconPath = "res://assets/icons/thief.png"
                },
                OmegaSpiral.Source.Backend.Common.CharacterClass.Bard => new CharacterClass
                {
                    Id = "bard",
                    Name = "Bard",
                    Description = "A support character with magical songs",
                    BaseHealth = 85,
                    BaseMana = 90,
                    BaseAttack = 8,
                    BaseDefense = 6,
                    BaseMagic = 10,
                    BaseMagicDefense = 8,
                    BaseSpeed = 12,
                    IconPath = "res://assets/icons/bard.png"
                },
                OmegaSpiral.Source.Backend.Common.CharacterClass.Paladin => new CharacterClass
                {
                    Id = "paladin",
                    Name = "Paladin",
                    Description = "A holy warrior combining combat and healing",
                    BaseHealth = 110,
                    BaseMana = 80,
                    BaseAttack = 12,
                    BaseDefense = 10,
                    BaseMagic = 8,
                    BaseMagicDefense = 8,
                    BaseSpeed = 9,
                    IconPath = "res://assets/icons/paladin.png"
                },
                OmegaSpiral.Source.Backend.Common.CharacterClass.Ranger => new CharacterClass
                {
                    Id = "ranger",
                    Name = "Ranger",
                    Description = "A nature-focused warrior with ranged abilities",
                    BaseHealth = 90,
                    BaseMana = 60,
                    BaseAttack = 14,
                    BaseDefense = 6,
                    BaseMagic = 6,
                    BaseMagicDefense = 6,
                    BaseSpeed = 13,
                    IconPath = "res://assets/icons/ranger.png"
                },
                _ => new CharacterClass
                {
                    Id = "fighter",
                    Name = "Fighter",
                    Description = "A strong warrior focused on physical combat",
                    BaseHealth = 10,
                    BaseMana = 50,
                    BaseAttack = 10,
                    BaseDefense = 5,
                    BaseMagic = 5,
                    BaseMagicDefense = 5,
                    BaseSpeed = 10,
                    IconPath = "res://assets/icons/fighter.png"
                }
            };
        }
    }
}
