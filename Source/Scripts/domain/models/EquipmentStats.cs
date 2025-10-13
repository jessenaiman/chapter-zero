// Copyright (c) Ωmega Spiral. All rights reserved.

using Godot;

namespace OmegaSpiral.Domain.Models
{
    /// <summary>
    /// Represents the statistical bonuses and properties of equipment items.
    /// </summary>
    public class EquipmentStats
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentStats"/> class.
        /// </summary>
        public EquipmentStats()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentStats"/> class with specified parameters.
        /// </summary>
        /// <param name="attackBonus">The bonus to attack power.</param>
        /// <param name="defenseBonus">The bonus to defense power.</param>
        /// <param name="magicBonus">The bonus to magic power.</param>
        /// <param name="magicDefenseBonus">The bonus to magic defense.</param>
        /// <param name="speedBonus">The bonus to speed.</param>
        /// <param name="healthBonus">The bonus to health points.</param>
        /// <param name="manaBonus">The bonus to mana points.</param>
        /// <param name="luckBonus">The bonus to luck.</param>
        /// <param name="criticalChanceBonus">The bonus to critical hit chance percentage.</param>
        /// <param name="criticalDamageBonus">The bonus to critical hit damage multiplier.</param>
        /// <param name="evasionBonus">The bonus to evasion chance percentage.</param>
        /// <param name="accuracyBonus">The bonus to accuracy percentage.</param>
        /// <param name="physicalResistanceBonus">The bonus to physical resistance percentage.</param>
        /// <param name="magicalResistanceBonus">The bonus to magical resistance percentage.</param>
        /// <param name="movementSpeedBonus">The bonus to movement speed multiplier.</param>
        /// <param name="strengthBonus">The strength bonus.</param>
        /// <param name="dexterityBonus">The dexterity bonus.</param>
        /// <param name="constitutionBonus">The constitution bonus.</param>
        /// <param name="intelligenceBonus">The intelligence bonus.</param>
        /// <param name="wisdomBonus">The wisdom bonus.</param>
        /// <param name="charismaBonus">The charisma bonus.</param>
        /// <param name="durability">The durability of this equipment (0-100%).</param>
        /// <param name="maxDurability">The maximum durability of this equipment.</param>
        /// <param name="equipmentSlot">The equipment slot.</param>
        /// <param name="equipmentType">The equipment type.</param>
        /// <param name="requiredLevel">The required level to equip this item.</param>
        /// <param name="requiredClass">The required character class to equip this item.</param>
        /// <param name="weight">The weight of this equipment.</param>
        /// <param name="value">The value or cost of this equipment.</param>
        public EquipmentStats(
            int attackBonus = 0,
            int defenseBonus = 0,
            int magicBonus = 0,
            int magicDefenseBonus = 0,
            int speedBonus = 0,
            int healthBonus = 0,
            int manaBonus = 0,
            int luckBonus = 0,
            float criticalChanceBonus = 0f,
            float criticalDamageBonus = 0f,
            float evasionBonus = 0f,
            float accuracyBonus = 0f,
            float physicalResistanceBonus = 0f,
            float magicalResistanceBonus = 0f,
            float movementSpeedBonus = 0f,
            int strengthBonus = 0,
            int dexterityBonus = 0,
            int constitutionBonus = 0,
            int intelligenceBonus = 0,
            int wisdomBonus = 0,
            int charismaBonus = 0,
            float durability = 100f,
            float maxDurability = 100f,
            string equipmentSlot = "",
            string equipmentType = "",
            int requiredLevel = 1,
            string requiredClass = "",
            float weight = 1.0f,
            int value = 0)
        {
            this.AttackBonus = attackBonus;
            this.DefenseBonus = defenseBonus;
            this.MagicBonus = magicBonus;
            this.MagicDefenseBonus = magicDefenseBonus;
            this.SpeedBonus = speedBonus;
            this.HealthBonus = healthBonus;
            this.ManaBonus = manaBonus;
            this.LuckBonus = luckBonus;
            this.CriticalChanceBonus = criticalChanceBonus;
            this.CriticalDamageBonus = criticalDamageBonus;
            this.EvasionBonus = evasionBonus;
            this.AccuracyBonus = accuracyBonus;
            this.PhysicalResistanceBonus = physicalResistanceBonus;
            this.MagicalResistanceBonus = magicalResistanceBonus;
            this.MovementSpeedBonus = movementSpeedBonus;
            this.StrengthBonus = strengthBonus;
            this.DexterityBonus = dexterityBonus;
            this.ConstitutionBonus = constitutionBonus;
            this.IntelligenceBonus = intelligenceBonus;
            this.WisdomBonus = wisdomBonus;
            this.CharismaBonus = charismaBonus;
            this.Durability = durability;
            this.MaxDurability = maxDurability;
            this.EquipmentSlot = equipmentSlot;
            this.EquipmentType = equipmentType;
            this.RequiredLevel = requiredLevel;
            this.RequiredClass = requiredClass;
            this.Weight = weight;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the bonus to attack power provided by this equipment.
        /// </summary>
        public int AttackBonus { get; set; }

        /// <summary>
        /// Gets or sets the bonus to defense power provided by this equipment.
        /// </summary>
        public int DefenseBonus { get; set; }

        /// <summary>
        /// Gets or sets the bonus to magic power provided by this equipment.
        /// </summary>
        public int MagicBonus { get; set; }

        /// <summary>
        /// Gets or sets the bonus to magic defense provided by this equipment.
        /// </summary>
        public int MagicDefenseBonus { get; set; }

        /// <summary>
        /// Gets or sets the bonus to speed provided by this equipment.
        /// </summary>
        public int SpeedBonus { get; set; }

        /// <summary>
        /// Gets or sets the bonus to health points provided by this equipment.
        /// </summary>
        public int HealthBonus { get; set; }

        /// <summary>
        /// Gets or sets the bonus to mana points provided by this equipment.
        /// </summary>
        public int ManaBonus { get; set; }

        /// <summary>
        /// Gets or sets the bonus to luck provided by this equipment.
        /// </summary>
        public int LuckBonus { get; set; }

        /// <summary>
        /// Gets or sets the bonus to critical hit chance percentage provided by this equipment.
        /// </summary>
        public float CriticalChanceBonus { get; set; }

        /// <summary>
        /// Gets or sets the bonus to critical hit damage multiplier provided by this equipment.
        /// </summary>
        public float CriticalDamageBonus { get; set; }

        /// <summary>
        /// Gets or sets the bonus to evasion chance percentage provided by this equipment.
        /// </summary>
        public float EvasionBonus { get; set; }

        /// <summary>
        /// Gets or sets the bonus to accuracy percentage provided by this equipment.
        /// </summary>
        public float AccuracyBonus { get; set; }

        /// <summary>
        /// Gets or sets the bonus to physical resistance percentage provided by this equipment.
        /// </summary>
        public float PhysicalResistanceBonus { get; set; }

        /// <summary>
        /// Gets or sets the bonus to magical resistance percentage provided by this equipment.
        /// </summary>
        public float MagicalResistanceBonus { get; set; }

        /// <summary>
        /// Gets or sets the bonus to movement speed multiplier provided by this equipment.
        /// </summary>
        public float MovementSpeedBonus { get; set; }

        /// <summary>
        /// Gets or sets the strength bonus provided by this equipment.
        /// </summary>
        public int StrengthBonus { get; set; }

        /// <summary>
        /// Gets or sets the dexterity bonus provided by this equipment.
        /// </summary>
        public int DexterityBonus { get; set; }

        /// <summary>
        /// Gets or sets the constitution bonus provided by this equipment.
        /// </summary>
        public int ConstitutionBonus { get; set; }

        /// <summary>
        /// Gets or sets the intelligence bonus provided by this equipment.
        /// </summary>
        public int IntelligenceBonus { get; set; }

        /// <summary>
        /// Gets or sets the wisdom bonus provided by this equipment.
        /// </summary>
        public int WisdomBonus { get; set; }

        /// <summary>
        /// Gets or sets the charisma bonus provided by this equipment.
        /// </summary>
        public int CharismaBonus { get; set; }

        /// <summary>
        /// Gets or sets the durability of this equipment (0-100%).
        /// </summary>
        public float Durability { get; set; } = 100.0f;

        /// <summary>
        /// Gets or sets the maximum durability of this equipment.
        /// </summary>
        public float MaxDurability { get; set; } = 100.0f;

        /// <summary>
        /// Gets or sets the equipment slot (e.g., "Weapon", "Armor", "Helmet", "Accessory").
        /// </summary>
        public string EquipmentSlot { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the equipment type (e.g., "Sword", "Shield", "Robe", "Ring").
        /// </summary>
        public string EquipmentType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the required level to equip this item.
        /// </summary>
        public int RequiredLevel { get; set; }

        /// <summary>
        /// Gets or sets the required character class to equip this item.
        /// </summary>
        public string RequiredClass { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the weight of this equipment.
        /// </summary>
        public float Weight { get; set; }

        /// <summary>
        /// Gets or sets the value or cost of this equipment.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Creates a copy of this equipment stats.
        /// </summary>
        /// <returns>A new instance of <see cref="EquipmentStats"/> with the same values.</returns>
        public EquipmentStats Clone()
        {
            return new EquipmentStats
            {
                AttackBonus = this.AttackBonus,
                DefenseBonus = this.DefenseBonus,
                MagicBonus = this.MagicBonus,
                MagicDefenseBonus = this.MagicDefenseBonus,
                SpeedBonus = this.SpeedBonus,
                HealthBonus = this.HealthBonus,
                ManaBonus = this.ManaBonus,
                LuckBonus = this.LuckBonus,
                CriticalChanceBonus = this.CriticalChanceBonus,
                CriticalDamageBonus = this.CriticalDamageBonus,
                EvasionBonus = this.EvasionBonus,
                AccuracyBonus = this.AccuracyBonus,
                PhysicalResistanceBonus = this.PhysicalResistanceBonus,
                MagicalResistanceBonus = this.MagicalResistanceBonus,
                MovementSpeedBonus = this.MovementSpeedBonus,
                StrengthBonus = this.StrengthBonus,
                DexterityBonus = this.DexterityBonus,
                ConstitutionBonus = this.ConstitutionBonus,
                IntelligenceBonus = this.IntelligenceBonus,
                WisdomBonus = this.WisdomBonus,
                CharismaBonus = this.CharismaBonus,
                Durability = this.Durability,
                MaxDurability = this.MaxDurability,
                EquipmentSlot = this.EquipmentSlot,
                EquipmentType = this.EquipmentType,
                RequiredLevel = this.RequiredLevel,
                RequiredClass = this.RequiredClass,
                Weight = this.Weight,
                Value = this.Value,
            };
        }

        /// <summary>
        /// Applies these equipment stats to a character's stats.
        /// </summary>
        /// <param name="characterStats">The character stats to modify.</param>
        public void ApplyTo(CharacterStats characterStats)
        {
            if (characterStats == null)
            {
                return;
            }

            characterStats.Attack += this.AttackBonus;
            characterStats.Defense += this.DefenseBonus;
            characterStats.Magic += this.MagicBonus;
            characterStats.MagicDefense += this.MagicDefenseBonus;
            characterStats.Speed += this.SpeedBonus;
            characterStats.MaxHealth += this.HealthBonus;
            characterStats.MaxMana += this.ManaBonus;
            characterStats.Luck += this.LuckBonus;
            characterStats.CriticalChance += this.CriticalChanceBonus;
            characterStats.CriticalDamage += this.CriticalDamageBonus;
            characterStats.Evasion += this.EvasionBonus;
            characterStats.Accuracy += this.AccuracyBonus;
            characterStats.PhysicalResistance += this.PhysicalResistanceBonus;
            characterStats.MagicalResistance += this.MagicalResistanceBonus;
            characterStats.MovementSpeed += this.MovementSpeedBonus;
            characterStats.Strength += this.StrengthBonus;
            characterStats.Dexterity += this.DexterityBonus;
            characterStats.Constitution += this.ConstitutionBonus;
            characterStats.Intelligence += this.IntelligenceBonus;
            characterStats.Wisdom += this.WisdomBonus;
            characterStats.Charisma += this.CharismaBonus;

            // Adjust current health/mana if max values changed
            if (this.HealthBonus > 0)
            {
                characterStats.Health = System.Math.Min(characterStats.Health + this.HealthBonus, characterStats.MaxHealth);
            }

            if (this.ManaBonus > 0)
            {
                characterStats.Mana = System.Math.Min(characterStats.Mana + this.ManaBonus, characterStats.MaxMana);
            }
        }

        /// <summary>
        /// Removes these equipment stats from a character's stats.
        /// </summary>
        /// <param name="characterStats">The character stats to modify.</param>
        public void RemoveFrom(CharacterStats characterStats)
        {
            if (characterStats == null)
            {
                return;
            }

            characterStats.Attack -= this.AttackBonus;
            characterStats.Defense -= this.DefenseBonus;
            characterStats.Magic -= this.MagicBonus;
            characterStats.MagicDefense -= this.MagicDefenseBonus;
            characterStats.Speed -= this.SpeedBonus;
            characterStats.MaxHealth -= this.HealthBonus;
            characterStats.MaxMana -= this.ManaBonus;
            characterStats.Luck -= this.LuckBonus;
            characterStats.CriticalChance -= this.CriticalChanceBonus;
            characterStats.CriticalDamage -= this.CriticalDamageBonus;
            characterStats.Evasion -= this.EvasionBonus;
            characterStats.Accuracy -= this.AccuracyBonus;
            characterStats.PhysicalResistance -= this.PhysicalResistanceBonus;
            characterStats.MagicalResistance -= this.MagicalResistanceBonus;
            characterStats.MovementSpeed -= this.MovementSpeedBonus;
            characterStats.Strength -= this.StrengthBonus;
            characterStats.Dexterity -= this.DexterityBonus;
            characterStats.Constitution -= this.ConstitutionBonus;
            characterStats.Intelligence -= this.IntelligenceBonus;
            characterStats.Wisdom -= this.WisdomBonus;
            characterStats.Charisma -= this.CharismaBonus;

            // Adjust current health/mana if max values changed
            if (this.HealthBonus > 0)
            {
                characterStats.Health = System.Math.Min(characterStats.Health, characterStats.MaxHealth);
            }

            if (this.ManaBonus > 0)
            {
                characterStats.Mana = System.Math.Min(characterStats.Mana, characterStats.MaxMana);
            }
        }

        /// <summary>
        /// Checks if this equipment can be equipped by the specified character.
        /// </summary>
        /// <param name="character">The character to check.</param>
        /// <returns><see langword="true"/> if the character can equip this item, <see langword="false"/> otherwise.</returns>
        public bool CanBeEquippedBy(Character character)
        {
            if (character == null)
            {
                return false;
            }

            // Check level requirement
            if (character.Stats?.Level < this.RequiredLevel)
            {
                return false;
            }

            // Check class requirement
            if (!string.IsNullOrEmpty(this.RequiredClass) &&
                !string.IsNullOrEmpty(character.Class?.Id) &&
                character.Class.Id != this.RequiredClass)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reduces the durability of this equipment.
        /// </summary>
        /// <param name="amount">The amount to reduce durability by.</param>
        /// <returns>The new durability percentage.</returns>
        public float ReduceDurability(float amount)
        {
            this.Durability = System.Math.Max(0f, this.Durability - amount);
            return this.Durability;
        }

        /// <summary>
        /// Repairs the durability of this equipment.
        /// </summary>
        /// <param name="amount">The amount to repair durability by.</param>
        /// <returns>The new durability percentage.</returns>
        public float RepairDurability(float amount)
        {
            this.Durability = System.Math.Min(this.MaxDurability, this.Durability + amount);
            return this.Durability;
        }

        /// <summary>
        /// Checks if this equipment is broken (durability at 0%).
        /// </summary>
        /// <returns><see langword="true"/> if the equipment is broken, <see langword="false"/> otherwise.</returns>
        public bool IsBroken()
        {
            return this.Durability <= 0f;
        }

        /// <summary>
        /// Gets the durability percentage as a value between 0 and 1.
        /// </summary>
        /// <returns>The durability percentage as a normalized value.</returns>
        public float GetDurabilityPercentage()
        {
            return this.MaxDurability > 0 ? this.Durability / this.MaxDurability : 0f;
        }
    }
}
