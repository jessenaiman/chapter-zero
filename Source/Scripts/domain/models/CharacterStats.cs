// Copyright (c) Ωmega Spiral. All rights reserved.

namespace OmegaSpiral.Domain.Models
{
    /// <summary>
    /// Represents the statistical attributes of a character in the game.
    /// </summary>
    public class CharacterStats
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterStats"/> class.
        /// </summary>
        public CharacterStats()
        {
            this.ResetToDefault();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterStats"/> class with specified base values.
        /// </summary>
        /// <param name="baseHealth">The base health points.</param>
        /// <param name="baseMana">The base mana points.</param>
        /// <param name="baseAttack">The base attack power.</param>
        /// <param name="baseDefense">The base defense power.</param>
        /// <param name="baseMagic">The base magic power.</param>
        /// <param name="baseMagicDefense">The base magic defense.</param>
        /// <param name="baseSpeed">The base speed.</param>
        /// <param name="baseLevel">The starting level.</param>
        public CharacterStats(
            int baseHealth,
            int baseMana,
            int baseAttack,
            int baseDefense,
            int baseMagic,
            int baseMagicDefense,
            int baseSpeed,
            int baseLevel = 1)
        {
            this.MaxHealth = baseHealth;
            this.Health = this.MaxHealth;
            this.MaxMana = baseMana;
            this.Mana = this.MaxMana;
            this.Attack = baseAttack;
            this.Defense = baseDefense;
            this.Magic = baseMagic;
            this.MagicDefense = baseMagicDefense;
            this.Speed = baseSpeed;
            this.Level = baseLevel;
            this.Experience = 0;
            this.ExperienceForNextLevel = CharacterStats.CalculateExperienceForLevel(baseLevel + 1);

            this.ResetSecondaryStats();
        }

        /// <summary>
        /// Gets or sets the character's current health points.
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        /// Gets or sets the character's maximum health points.
        /// </summary>
        public int MaxHealth { get; set; }

        /// <summary>
        /// Gets or sets the character's current mana points.
        /// </summary>
        public int Mana { get; set; }

        /// <summary>
        /// Gets or sets the character's maximum mana points.
        /// </summary>
        public int MaxMana { get; set; }

        /// <summary>
        /// Gets or sets the character's current experience points.
        /// </summary>
        public int Experience { get; set; }

        /// <summary>
        /// Gets or sets the character's current level.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the character's attack power.
        /// </summary>
        public int Attack { get; set; }

        /// <summary>
        /// Gets or sets the character's defense power.
        /// </summary>
        public int Defense { get; set; }

        /// <summary>
        /// Gets or sets the character's magic power.
        /// </summary>
        public int Magic { get; set; }

        /// <summary>
        /// Gets or sets the character's magic defense.
        /// </summary>
        public int MagicDefense { get; set; }

        /// <summary>
        /// Gets or sets the character's speed.
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// Gets or sets the character's luck.
        /// </summary>
        public int Luck { get; set; }

        /// <summary>
        /// Gets or sets the character's strength attribute.
        /// </summary>
        public int Strength { get; set; }

        /// <summary>
        /// Gets or sets the character's dexterity attribute.
        /// </summary>
        public int Dexterity { get; set; }

        /// <summary>
        /// Gets or sets the character's constitution attribute.
        /// </summary>
        public int Constitution { get; set; }

        /// <summary>
        /// Gets or sets the character's intelligence attribute.
        /// </summary>
        public int Intelligence { get; set; }

        /// <summary>
        /// Gets or sets the character's wisdom attribute.
        /// </summary>
        public int Wisdom { get; set; }

        /// <summary>
        /// Gets or sets the character's charisma attribute.
        /// </summary>
        public int Charisma { get; set; }

        /// <summary>
        /// Gets or sets the character's critical hit chance percentage.
        /// </summary>
        public float CriticalChance { get; set; }

        /// <summary>
        /// Gets or sets the character's critical hit damage multiplier.
        /// </summary>
        public float CriticalDamage { get; set; }

        /// <summary>
        /// Gets or sets the character's evasion chance percentage.
        /// </summary>
        public float Evasion { get; set; }

        /// <summary>
        /// Gets or sets the character's accuracy percentage.
        /// </summary>
        public float Accuracy { get; set; }

        /// <summary>
        /// Gets or sets the character's resistance to physical damage percentage.
        /// </summary>
        public float PhysicalResistance { get; set; }

        /// <summary>
        /// Gets or sets the character's resistance to magical damage percentage.
        /// </summary>
        public float MagicalResistance { get; set; }

        /// <summary>
        /// Gets or sets the character's movement speed multiplier.
        /// </summary>
        public float MovementSpeed { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the character's experience points needed for next level.
        /// </summary>
        public int ExperienceForNextLevel { get; set; }

        /// <summary>
        /// Creates a copy of this character stats.
        /// </summary>
        /// <returns>A new instance of <see cref="CharacterStats"/> with the same values.</returns>
        public CharacterStats Clone()
        {
            return new CharacterStats
            {
                Health = this.Health,
                MaxHealth = this.MaxHealth,
                Mana = this.Mana,
                MaxMana = this.MaxMana,
                Experience = this.Experience,
                Level = this.Level,
                Attack = this.Attack,
                Defense = this.Defense,
                Magic = this.Magic,
                MagicDefense = this.MagicDefense,
                Speed = this.Speed,
                Luck = this.Luck,
                Strength = this.Strength,
                Dexterity = this.Dexterity,
                Constitution = this.Constitution,
                Intelligence = this.Intelligence,
                Wisdom = this.Wisdom,
                Charisma = this.Charisma,
                CriticalChance = this.CriticalChance,
                CriticalDamage = this.CriticalDamage,
                Evasion = this.Evasion,
                Accuracy = this.Accuracy,
                PhysicalResistance = this.PhysicalResistance,
                MagicalResistance = this.MagicalResistance,
                MovementSpeed = this.MovementSpeed,
                ExperienceForNextLevel = this.ExperienceForNextLevel,
            };
        }

        /// <summary>
        /// Resets all stats to default values based on current level and class.
        /// </summary>
        public void ResetToDefault()
        {
            this.Health = 100;
            this.MaxHealth = 100;
            this.Mana = 50;
            this.MaxMana = 50;
            this.Experience = 0;
            this.Level = 1;
            this.Attack = 10;
            this.Defense = 5;
            this.Magic = 5;
            this.MagicDefense = 5;
            this.Speed = 10;
            this.Luck = 5;
            this.Strength = 10;
            this.Dexterity = 10;
            this.Constitution = 10;
            this.Intelligence = 10;
            this.Wisdom = 10;
            this.Charisma = 10;
            this.CriticalChance = 5.0f;
            this.CriticalDamage = 1.5f;
            this.Evasion = 5.0f;
            this.Accuracy = 95.0f;
            this.PhysicalResistance = 0.0f;
            this.MagicalResistance = 0.0f;
            this.MovementSpeed = 1.0f;
            this.ExperienceForNextLevel = CharacterStats.CalculateExperienceForLevel(2);
        }

        /// <summary>
        /// Resets secondary stats that are derived from primary attributes.
        /// </summary>
        public void ResetSecondaryStats()
        {
            // Calculate derived stats from primary attributes
            this.MaxHealth = this.CalculateMaxHealth();
            this.MaxMana = this.CalculateMaxMana();
            this.Attack = this.CalculateAttack();
            this.Defense = this.CalculateDefense();
            this.Magic = this.CalculateMagic();
            this.MagicDefense = this.CalculateMagicDefense();
            this.Speed = this.CalculateSpeed();

            this.Health = this.MaxHealth;
            this.Mana = this.MaxMana;
        }

        /// <summary>
        /// Levels up the character and updates all stats accordingly.
        /// </summary>
        public void LevelUp()
        {
            this.Level++;
            this.Experience -= this.ExperienceForNextLevel;
            this.ExperienceForNextLevel = CharacterStats.CalculateExperienceForLevel(this.Level + 1);

            // Increase primary stats on level up
            this.Strength += 2;
            this.Dexterity += 2;
            this.Constitution += 2;
            this.Intelligence += 2;
            this.Wisdom += 1;
            this.Charisma += 1;
            this.Luck += 1;

            // Recalculate all derived stats
            this.ResetSecondaryStats();
        }

        /// <summary>
        /// Adds experience points to the character and handles level progression.
        /// </summary>
        /// <param name="exp">The experience points to add.</param>
        /// <returns><see langword="true"/> if the character leveled up, <see langword="false"/> otherwise.</returns>
        public bool AddExperience(int exp)
        {
            if (exp <= 0) return false;

            this.Experience += exp;
            bool leveledUp = false;

            while (this.Experience >= this.ExperienceForNextLevel)
            {
                this.LevelUp();
                leveledUp = true;
            }

            return leveledUp;
        }

        /// <summary>
        /// Heals the character by the specified amount.
        /// </summary>
        /// <param name="amount">The amount to heal.</param>
        /// <returns>The actual amount healed (capped at max health).</returns>
        public int Heal(int amount)
        {
            int actualHeal = System.Math.Min(amount, this.MaxHealth - this.Health);
            this.Health += actualHeal;
            return actualHeal;
        }

        /// <summary>
        /// Damages the character by the specified amount.
        /// </summary>
        /// <param name="amount">The amount of damage to take.</param>
        /// <returns>The actual damage taken.</returns>
        public int TakeDamage(int amount)
        {
            int actualDamage = System.Math.Max(0, amount - this.Defense);
            this.Health -= actualDamage;

            if (this.Health < 0)
            {
                this.Health = 0;
            }

            return actualDamage;
        }

        /// <summary>
        /// Calculates the maximum health based on constitution and level.
        /// </summary>
        /// <returns>The calculated maximum health.</returns>
        private int CalculateMaxHealth()
        {
            return 100 + (this.Constitution * 10) + (this.Level * 5);
        }

        /// <summary>
        /// Calculates the maximum mana based on intelligence and level.
        /// </summary>
        /// <returns>The calculated maximum mana.</returns>
        private int CalculateMaxMana()
        {
            return 50 + (this.Intelligence * 8) + (this.Level * 3);
        }

        /// <summary>
        /// Calculates the attack power based on strength and dexterity.
        /// </summary>
        /// <returns>The calculated attack power.</returns>
        private int CalculateAttack()
        {
            return this.Strength + (this.Dexterity / 2);
        }

        /// <summary>
        /// Calculates the defense power based on constitution and dexterity.
        /// </summary>
        /// <returns>The calculated defense power.</returns>
        private int CalculateDefense()
        {
            return this.Constitution + (this.Dexterity / 3);
        }

        /// <summary>
        /// Calculates the magic power based on intelligence and wisdom.
        /// </summary>
        /// <returns>The calculated magic power.</returns>
        private int CalculateMagic()
        {
            return this.Intelligence + (this.Wisdom / 2);
        }

        /// <summary>
        /// Calculates the magic defense based on wisdom and constitution.
        /// </summary>
        /// <returns>The calculated magic defense.</returns>
        private int CalculateMagicDefense()
        {
            return this.Wisdom + (this.Constitution / 3);
        }

        /// <summary>
        /// Calculates the experience points needed for the specified level.
        /// </summary>
        /// <param name="level">The level to calculate experience for.</param>
        /// <returns>The experience points needed for the specified level.</returns>
        private static int CalculateExperienceForLevel(int level)
        {
            // Exponential growth formula: base * (multiplier ^ level)
            return 100 * (int) System.Math.Pow(1.5, level - 1);
        }

        /// <summary>
        /// Calculates the speed based on dexterity and level.
        /// </summary>
        /// <returns>The calculated speed.</returns>
        private int CalculateSpeed()
        {
            return this.Dexterity + this.Level;
        }
    }
}
