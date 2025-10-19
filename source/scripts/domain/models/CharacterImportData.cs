// <copyright file="CharacterImportData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Globalization;

using Godot;

namespace OmegaSpiral.Domain.Models
{
    /// <summary>
    /// Represents the data structure for importing character data.
    /// </summary>
    public class CharacterImportData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterImportData"/> class.
        /// </summary>
        public CharacterImportData()
        {
        }

        /// <summary>
        /// Gets or sets the character's unique identifier.
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
        /// Gets the character class data.
        /// </summary>
        public Dictionary<string, object> ClassData { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets the character appearance data.
        /// </summary>
        public Dictionary<string, object> AppearanceData { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets the character stats data.
        /// </summary>
        public Dictionary<string, object> StatsData { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets the inventory data.
        /// </summary>
        public Dictionary<string, object> InventoryData { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets the equipment data.
        /// </summary>
        public Dictionary<string, object> EquipmentData { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets the character's current location data.
        /// </summary>
        public Dictionary<string, object> LocationData { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets the character's quest progress data.
        /// </summary>
        public Dictionary<string, object> QuestProgressData { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets the character's skill data.
        /// </summary>
        public Dictionary<string, object> SkillsData { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets the character's relationship data.
        /// </summary>
        public Dictionary<string, object> RelationshipsData { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the timestamp of when this data was exported.
        /// </summary>
        public string ExportTimestamp { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the version of the export format.
        /// </summary>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// Gets or sets the source game or application.
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Gets the metadata dictionary.
        /// </summary>
        public Dictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Validates whether this import data contains all required information.
        /// </summary>
        /// <returns><see langword="true"/> if the data is valid for import, <see langword="false"/> otherwise.</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(this.Id) &&
                   !string.IsNullOrEmpty(this.Name) &&
                   this.ClassData.Count > 0 &&
                   this.AppearanceData.Count > 0 &&
                   this.StatsData.Count > 0;
        }

        /// <summary>
        /// Creates a character instance from this import data.
        /// </summary>
        /// <returns>A new character instance populated from the import data.</returns>
        public Character CreateCharacter()
        {
            var character = new Character
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
            };

            // Create class data if available
            if (this.ClassData.Count > 0)
            {
                var newClass = this.CreateCharacterClassFromData();
                if (newClass != null)
                {
                    character.Class = newClass;
                }
            }

            // Create appearance data if available
            if (this.AppearanceData.Count > 0)
            {
                var newAppearance = this.CreateCharacterAppearanceFromData();
                if (newAppearance != null)
                {
                    character.Appearance = newAppearance;
                }
            }

            // Create stats data if available
            if (this.StatsData.Count > 0)
            {
                var newStats = this.CreateCharacterStatsFromData();
                if (newStats != null)
                {
                    character.Stats = newStats;
                }
            }

            return character;
        }

        /// <summary>
        /// Merges this import data with an existing character, updating only specified fields.
        /// </summary>
        /// <param name="existingCharacter">The existing character to update.</param>
        /// <param name="updateClass">Whether to update the character class data.</param>
        /// <param name="updateAppearance">Whether to update the character appearance data.</param>
        /// <param name="updateStats">Whether to update the character stats data.</param>
        public void MergeInto(Character existingCharacter, bool updateClass = true, bool updateAppearance = true, bool updateStats = true)
        {
            if (existingCharacter == null)
            {
                return;
            }

            existingCharacter.Name = string.IsNullOrEmpty(this.Name) ? existingCharacter.Name : this.Name;
            existingCharacter.Description = string.IsNullOrEmpty(this.Description) ? existingCharacter.Description : this.Description;

            if (updateClass && this.ClassData.Count > 0)
            {
                var newClass = this.CreateCharacterClassFromData();
                if (newClass != null)
                {
                    existingCharacter.Class = newClass;
                }
            }

            if (updateAppearance && this.AppearanceData.Count > 0)
            {
                var newAppearance = this.CreateCharacterAppearanceFromData();
                if (newAppearance != null)
                {
                    existingCharacter.Appearance = newAppearance;
                }
            }

            if (updateStats && this.StatsData.Count > 0)
            {
                var newStats = this.CreateCharacterStatsFromData();
                if (newStats != null)
                {
                    existingCharacter.Stats = newStats;
                }
            }
        }

        /// <summary>
        /// Creates a CharacterClass instance from the stored class data.
        /// </summary>
        /// <returns>A new CharacterClass instance, or null if data is invalid.</returns>
        private CharacterClass? CreateCharacterClassFromData()
        {
            if (!this.ClassData.TryGetValue("Id", out object? value))
            {
                return null;
            }

            return new CharacterClass
            {
                Id = value.ToString() ?? string.Empty,
                Name = this.ClassData["Name"].ToString() ?? string.Empty,
                Description = this.ClassData["Description"].ToString() ?? string.Empty,
                BaseHealth = this.ClassData.TryGetValue("BaseHealth", out object? baseHealthValue) ? System.Convert.ToInt32(baseHealthValue, CultureInfo.InvariantCulture) : 100,
                BaseMana = this.ClassData.TryGetValue("BaseMana", out object? baseManaValue) ? System.Convert.ToInt32(baseManaValue, CultureInfo.InvariantCulture) : 50,
                BaseAttack = this.ClassData.TryGetValue("BaseAttack", out object? baseAttackValue) ? System.Convert.ToInt32(baseAttackValue, CultureInfo.InvariantCulture) : 10,
                BaseDefense = this.ClassData.TryGetValue("BaseDefense", out object? baseDefenseValue) ? System.Convert.ToInt32(baseDefenseValue, CultureInfo.InvariantCulture) : 5,
                BaseMagic = this.ClassData.TryGetValue("BaseMagic", out object? baseMagicValue) ? System.Convert.ToInt32(baseMagicValue, CultureInfo.InvariantCulture) : 5,
                BaseMagicDefense = this.ClassData.TryGetValue("BaseMagicDefense", out object? baseMagicDefenseValue) ? System.Convert.ToInt32(baseMagicDefenseValue, CultureInfo.InvariantCulture) : 5,
                BaseSpeed = this.ClassData.TryGetValue("BaseSpeed", out object? baseSpeedValue) ? System.Convert.ToInt32(baseSpeedValue, CultureInfo.InvariantCulture) : 10,
                IconPath = this.ClassData.TryGetValue("IconPath", out object? iconPathValue) ? iconPathValue.ToString() ?? string.Empty : string.Empty,
            };
        }

        /// <summary>
        /// Creates a CharacterAppearance instance from the stored appearance data.
        /// </summary>
        /// <returns>A new CharacterAppearance instance, or null if data is invalid.</returns>
        private CharacterAppearance? CreateCharacterAppearanceFromData()
        {
            if (!this.AppearanceData.TryGetValue("SkinColor", out object? value))
            {
                return null;
            }

            return new CharacterAppearance
            {
                SkinColor = Color.FromHtml(value.ToString() ?? "#FFFFFF"),
                HairColor = Color.FromHtml(this.AppearanceData["HairColor"].ToString() ?? "#000000"),
                EyeColor = Color.FromHtml(this.AppearanceData["EyeColor"].ToString() ?? "#000000"),
                HairStyle = this.AppearanceData.TryGetValue("HairStyle", out object? hairStyleValue) ? hairStyleValue.ToString() ?? string.Empty : string.Empty,
                EyeStyle = this.AppearanceData.TryGetValue("EyeStyle", out object? eyeStyleValue) ? eyeStyleValue.ToString() ?? string.Empty : string.Empty,
                ClothingStyle = this.AppearanceData.TryGetValue("ClothingStyle", out object? clothingStyleValue) ? clothingStyleValue.ToString() ?? string.Empty : string.Empty,
                Accessory = this.AppearanceData.TryGetValue("Accessory", out object? accessoryValue) ? accessoryValue.ToString() ?? string.Empty : string.Empty,
                SpritePath = this.AppearanceData.TryGetValue("SpritePath", out object? spritePathValue) ? spritePathValue.ToString() ?? string.Empty : string.Empty,
                PortraitPath = this.AppearanceData.TryGetValue("PortraitPath", out object? portraitPathValue) ? portraitPathValue.ToString() ?? string.Empty : string.Empty,
                HeightScale = this.AppearanceData.TryGetValue("HeightScale", out object? heightScaleValue) ? System.Convert.ToSingle(heightScaleValue, CultureInfo.InvariantCulture) : 1.0f,
                WidthScale = this.AppearanceData.TryGetValue("WidthScale", out object? widthScaleValue) ? System.Convert.ToSingle(widthScaleValue, CultureInfo.InvariantCulture) : 1.0f,
                BodyType = this.AppearanceData.TryGetValue("BodyType", out object? bodyTypeValue) ? bodyTypeValue.ToString() ?? "Normal" : "Normal",
                FacialExpression = this.AppearanceData.TryGetValue("FacialExpression", out object? facialExpressionValue) ? facialExpressionValue.ToString() ?? "Neutral" : "Neutral",
            };
        }

        /// <summary>
        /// Creates a CharacterStats instance from the stored stats data.
        /// </summary>
        /// <returns>A new CharacterStats instance, or null if data is invalid.</returns>
        private CharacterStats? CreateCharacterStatsFromData()
        {
            if (!this.StatsData.TryGetValue("Health", out object? value))
            {
                return null;
            }

            return new CharacterStats
            {
                Health = System.Convert.ToInt32(value, CultureInfo.InvariantCulture),
                MaxHealth = System.Convert.ToInt32(this.StatsData["MaxHealth"], CultureInfo.InvariantCulture),
                Mana = System.Convert.ToInt32(this.StatsData["Mana"], CultureInfo.InvariantCulture),
                MaxMana = System.Convert.ToInt32(this.StatsData["MaxMana"], CultureInfo.InvariantCulture),
                Experience = System.Convert.ToInt32(this.StatsData["Experience"], CultureInfo.InvariantCulture),
                Level = System.Convert.ToInt32(this.StatsData["Level"], CultureInfo.InvariantCulture),
                Attack = System.Convert.ToInt32(this.StatsData["Attack"], CultureInfo.InvariantCulture),
                Defense = System.Convert.ToInt32(this.StatsData["Defense"], CultureInfo.InvariantCulture),
                Magic = System.Convert.ToInt32(this.StatsData["Magic"], CultureInfo.InvariantCulture),
                MagicDefense = System.Convert.ToInt32(this.StatsData["MagicDefense"], CultureInfo.InvariantCulture),
                Speed = System.Convert.ToInt32(this.StatsData["Speed"], CultureInfo.InvariantCulture),
                Luck = System.Convert.ToInt32(this.StatsData.TryGetValue("Luck", out object? luckValue) ? luckValue : 5, CultureInfo.InvariantCulture),
                Strength = System.Convert.ToInt32(this.StatsData.TryGetValue("Strength", out object? strengthValue) ? strengthValue : 10, CultureInfo.InvariantCulture),
                Dexterity = System.Convert.ToInt32(this.StatsData.TryGetValue("Dexterity", out object? dexterityValue) ? dexterityValue : 10, CultureInfo.InvariantCulture),
                Constitution = System.Convert.ToInt32(this.StatsData.TryGetValue("Constitution", out object? constitutionValue) ? constitutionValue : 10, CultureInfo.InvariantCulture),
                Intelligence = System.Convert.ToInt32(this.StatsData.TryGetValue("Intelligence", out object? intelligenceValue) ? intelligenceValue : 10, CultureInfo.InvariantCulture),
                Wisdom = System.Convert.ToInt32(this.StatsData.TryGetValue("Wisdom", out object? wisdomValue) ? wisdomValue : 10, CultureInfo.InvariantCulture),
                Charisma = System.Convert.ToInt32(this.StatsData.TryGetValue("Charisma", out object? charismaValue) ? charismaValue : 10, CultureInfo.InvariantCulture),
                CriticalChance = System.Convert.ToSingle(this.StatsData.TryGetValue("CriticalChance", out object? critChanceValue) ? critChanceValue : 5.0f, CultureInfo.InvariantCulture),
                CriticalDamage = System.Convert.ToSingle(this.StatsData.TryGetValue("CriticalDamage", out object? critDamageValue) ? critDamageValue : 1.5f, CultureInfo.InvariantCulture),
                Evasion = System.Convert.ToSingle(this.StatsData.TryGetValue("Evasion", out object? evasionValue) ? evasionValue : 5.0f, CultureInfo.InvariantCulture),
                Accuracy = System.Convert.ToSingle(this.StatsData.TryGetValue("Accuracy", out object? accuracyValue) ? accuracyValue : 95.0f, CultureInfo.InvariantCulture),
                PhysicalResistance = System.Convert.ToSingle(this.StatsData.TryGetValue("PhysicalResistance", out object? physResValue) ? physResValue : 0.0f, CultureInfo.InvariantCulture),
                MagicalResistance = System.Convert.ToSingle(this.StatsData.TryGetValue("MagicalResistance", out object? magResValue) ? magResValue : 0.0f, CultureInfo.InvariantCulture),
                MovementSpeed = System.Convert.ToSingle(this.StatsData.TryGetValue("MovementSpeed", out object? moveSpeedValue) ? moveSpeedValue : 1.0f, CultureInfo.InvariantCulture),
                ExperienceForNextLevel = System.Convert.ToInt32(this.StatsData.TryGetValue("ExperienceForNextLevel", out object? expNextLevelValue) ? expNextLevelValue : 10, CultureInfo.InvariantCulture),
            };
        }
    }
}
