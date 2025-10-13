// <copyright file="CharacterImportData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Domain.Models
{
    using System.Collections.Generic;
    using System.Globalization;

    using Godot;

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

            // Deserialize class data if available
            if (this.ClassData.ContainsKey("Id"))
            {
                character.Class = new CharacterClass
                {
                    Id = this.ClassData["Id"].ToString() ?? string.Empty,
                    Name = this.ClassData["Name"].ToString() ?? string.Empty,
                    Description = this.ClassData["Description"].ToString() ?? string.Empty,
                    BaseHealth = this.ClassData.ContainsKey("BaseHealth") ? System.Convert.ToInt32(this.ClassData["BaseHealth"], CultureInfo.InvariantCulture) : 100,
                    BaseMana = this.ClassData.ContainsKey("BaseMana") ? System.Convert.ToInt32(this.ClassData["BaseMana"], CultureInfo.InvariantCulture) : 50,
                    BaseAttack = this.ClassData.ContainsKey("BaseAttack") ? System.Convert.ToInt32(this.ClassData["BaseAttack"], CultureInfo.InvariantCulture) : 10,
                    BaseDefense = this.ClassData.ContainsKey("BaseDefense") ? System.Convert.ToInt32(this.ClassData["BaseDefense"], CultureInfo.InvariantCulture) : 5,
                    BaseMagic = this.ClassData.ContainsKey("BaseMagic") ? System.Convert.ToInt32(this.ClassData["BaseMagic"], CultureInfo.InvariantCulture) : 5,
                    BaseMagicDefense = this.ClassData.ContainsKey("BaseMagicDefense") ? System.Convert.ToInt32(this.ClassData["BaseMagicDefense"], CultureInfo.InvariantCulture) : 5,
                    BaseSpeed = this.ClassData.ContainsKey("BaseSpeed") ? System.Convert.ToInt32(this.ClassData["BaseSpeed"], CultureInfo.InvariantCulture) : 10,
                    IconPath = this.ClassData.ContainsKey("IconPath") ? this.ClassData["IconPath"].ToString() ?? string.Empty : string.Empty,
                };
            }

            // Deserialize appearance data if available
            if (this.AppearanceData.ContainsKey("SkinColor"))
            {
                character.Appearance = new CharacterAppearance
                {
                    SkinColor = Color.FromHtml(this.AppearanceData["SkinColor"].ToString() ?? "#FFFFFF"),
                    HairColor = Color.FromHtml(this.AppearanceData["HairColor"].ToString() ?? "#000000"),
                    EyeColor = Color.FromHtml(this.AppearanceData["EyeColor"].ToString() ?? "#000000"),
                    HairStyle = this.AppearanceData.ContainsKey("HairStyle") ? this.AppearanceData["HairStyle"].ToString() ?? string.Empty : string.Empty,
                    EyeStyle = this.AppearanceData.ContainsKey("EyeStyle") ? this.AppearanceData["EyeStyle"].ToString() ?? string.Empty : string.Empty,
                    ClothingStyle = this.AppearanceData.ContainsKey("ClothingStyle") ? this.AppearanceData["ClothingStyle"].ToString() ?? string.Empty : string.Empty,
                    Accessory = this.AppearanceData.ContainsKey("Accessory") ? this.AppearanceData["Accessory"].ToString() ?? string.Empty : string.Empty,
                    SpritePath = this.AppearanceData.ContainsKey("SpritePath") ? this.AppearanceData["SpritePath"].ToString() ?? string.Empty : string.Empty,
                    PortraitPath = this.AppearanceData.ContainsKey("PortraitPath") ? this.AppearanceData["PortraitPath"].ToString() ?? string.Empty : string.Empty,
                    HeightScale = this.AppearanceData.ContainsKey("HeightScale") ? System.Convert.ToSingle(this.AppearanceData["HeightScale"], CultureInfo.InvariantCulture) : 1.0f,
                    WidthScale = this.AppearanceData.ContainsKey("WidthScale") ? System.Convert.ToSingle(this.AppearanceData["WidthScale"], CultureInfo.InvariantCulture) : 1.0f,
                    BodyType = this.AppearanceData.ContainsKey("BodyType") ? this.AppearanceData["BodyType"].ToString() ?? "Normal" : "Normal",
                    FacialExpression = this.AppearanceData.ContainsKey("FacialExpression") ? this.AppearanceData["FacialExpression"].ToString() ?? "Neutral" : "Neutral",
                };
            }

            // Deserialize stats data if available
            if (this.StatsData.ContainsKey("Health"))
            {
                character.Stats = new CharacterStats
                {
                    Health = System.Convert.ToInt32(this.StatsData["Health"], CultureInfo.InvariantCulture),
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
                    Luck = System.Convert.ToInt32(this.StatsData.ContainsKey("Luck") ? this.StatsData["Luck"] : 5, CultureInfo.InvariantCulture),
                    Strength = System.Convert.ToInt32(this.StatsData.ContainsKey("Strength") ? this.StatsData["Strength"] : 10, CultureInfo.InvariantCulture),
                    Dexterity = System.Convert.ToInt32(this.StatsData.ContainsKey("Dexterity") ? this.StatsData["Dexterity"] : 10, CultureInfo.InvariantCulture),
                    Constitution = System.Convert.ToInt32(this.StatsData.ContainsKey("Constitution") ? this.StatsData["Constitution"] : 10, CultureInfo.InvariantCulture),
                    Intelligence = System.Convert.ToInt32(this.StatsData.ContainsKey("Intelligence") ? this.StatsData["Intelligence"] : 10, CultureInfo.InvariantCulture),
                    Wisdom = System.Convert.ToInt32(this.StatsData.ContainsKey("Wisdom") ? this.StatsData["Wisdom"] : 10, CultureInfo.InvariantCulture),
                    Charisma = System.Convert.ToInt32(this.StatsData.ContainsKey("Charisma") ? this.StatsData["Charisma"] : 10, CultureInfo.InvariantCulture),
                    CriticalChance = System.Convert.ToSingle(this.StatsData.ContainsKey("CriticalChance") ? this.StatsData["CriticalChance"] : 5.0f, CultureInfo.InvariantCulture),
                    CriticalDamage = System.Convert.ToSingle(this.StatsData.ContainsKey("CriticalDamage") ? this.StatsData["CriticalDamage"] : 1.5f, CultureInfo.InvariantCulture),
                    Evasion = System.Convert.ToSingle(this.StatsData.ContainsKey("Evasion") ? this.StatsData["Evasion"] : 5.0f, CultureInfo.InvariantCulture),
                    Accuracy = System.Convert.ToSingle(this.StatsData.ContainsKey("Accuracy") ? this.StatsData["Accuracy"] : 95.0f, CultureInfo.InvariantCulture),
                    PhysicalResistance = System.Convert.ToSingle(this.StatsData.ContainsKey("PhysicalResistance") ? this.StatsData["PhysicalResistance"] : 0.0f, CultureInfo.InvariantCulture),
                    MagicalResistance = System.Convert.ToSingle(this.StatsData.ContainsKey("MagicalResistance") ? this.StatsData["MagicalResistance"] : 0.0f, CultureInfo.InvariantCulture),
                    MovementSpeed = System.Convert.ToSingle(this.StatsData.ContainsKey("MovementSpeed") ? this.StatsData["MovementSpeed"] : 1.0f, CultureInfo.InvariantCulture),
                    ExperienceForNextLevel = System.Convert.ToInt32(this.StatsData.ContainsKey("ExperienceForNextLevel") ? this.StatsData["ExperienceForNextLevel"] : 100, CultureInfo.InvariantCulture),
                };
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
            if (!this.ClassData.ContainsKey("Id"))
            {
                return null;
            }

            return new CharacterClass
            {
                Id = this.ClassData["Id"].ToString() ?? string.Empty,
                Name = this.ClassData["Name"].ToString() ?? string.Empty,
                Description = this.ClassData["Description"].ToString() ?? string.Empty,
                BaseHealth = this.ClassData.ContainsKey("BaseHealth") ? System.Convert.ToInt32(this.ClassData["BaseHealth"], CultureInfo.InvariantCulture) : 100,
                BaseMana = this.ClassData.ContainsKey("BaseMana") ? System.Convert.ToInt32(this.ClassData["BaseMana"], CultureInfo.InvariantCulture) : 50,
                BaseAttack = this.ClassData.ContainsKey("BaseAttack") ? System.Convert.ToInt32(this.ClassData["BaseAttack"], CultureInfo.InvariantCulture) : 10,
                BaseDefense = this.ClassData.ContainsKey("BaseDefense") ? System.Convert.ToInt32(this.ClassData["BaseDefense"], CultureInfo.InvariantCulture) : 5,
                BaseMagic = this.ClassData.ContainsKey("BaseMagic") ? System.Convert.ToInt32(this.ClassData["BaseMagic"], CultureInfo.InvariantCulture) : 5,
                BaseMagicDefense = this.ClassData.ContainsKey("BaseMagicDefense") ? System.Convert.ToInt32(this.ClassData["BaseMagicDefense"], CultureInfo.InvariantCulture) : 5,
                BaseSpeed = this.ClassData.ContainsKey("BaseSpeed") ? System.Convert.ToInt32(this.ClassData["BaseSpeed"], CultureInfo.InvariantCulture) : 10,
                IconPath = this.ClassData.ContainsKey("IconPath") ? this.ClassData["IconPath"].ToString() ?? string.Empty : string.Empty,
            };
        }

        /// <summary>
        /// Creates a CharacterAppearance instance from the stored appearance data.
        /// </summary>
        /// <returns>A new CharacterAppearance instance, or null if data is invalid.</returns>
        private CharacterAppearance? CreateCharacterAppearanceFromData()
        {
            if (!this.AppearanceData.ContainsKey("SkinColor"))
            {
                return null;
            }

            return new CharacterAppearance
            {
                SkinColor = Color.FromHtml(this.AppearanceData["SkinColor"].ToString() ?? "#FFFFFF"),
                HairColor = Color.FromHtml(this.AppearanceData["HairColor"].ToString() ?? "#000000"),
                EyeColor = Color.FromHtml(this.AppearanceData["EyeColor"].ToString() ?? "#000000"),
                HairStyle = this.AppearanceData.ContainsKey("HairStyle") ? this.AppearanceData["HairStyle"].ToString() ?? string.Empty : string.Empty,
                EyeStyle = this.AppearanceData.ContainsKey("EyeStyle") ? this.AppearanceData["EyeStyle"].ToString() ?? string.Empty : string.Empty,
                ClothingStyle = this.AppearanceData.ContainsKey("ClothingStyle") ? this.AppearanceData["ClothingStyle"].ToString() ?? string.Empty : string.Empty,
                Accessory = this.AppearanceData.ContainsKey("Accessory") ? this.AppearanceData["Accessory"].ToString() ?? string.Empty : string.Empty,
                SpritePath = this.AppearanceData.ContainsKey("SpritePath") ? this.AppearanceData["SpritePath"].ToString() ?? string.Empty : string.Empty,
                PortraitPath = this.AppearanceData.ContainsKey("PortraitPath") ? this.AppearanceData["PortraitPath"].ToString() ?? string.Empty : string.Empty,
                HeightScale = this.AppearanceData.ContainsKey("HeightScale") ? System.Convert.ToSingle(this.AppearanceData["HeightScale"], CultureInfo.InvariantCulture) : 1.0f,
                WidthScale = this.AppearanceData.ContainsKey("WidthScale") ? System.Convert.ToSingle(this.AppearanceData["WidthScale"], CultureInfo.InvariantCulture) : 1.0f,
                BodyType = this.AppearanceData.ContainsKey("BodyType") ? this.AppearanceData["BodyType"].ToString() ?? "Normal" : "Normal",
                FacialExpression = this.AppearanceData.ContainsKey("FacialExpression") ? this.AppearanceData["FacialExpression"].ToString() ?? "Neutral" : "Neutral",
            };
        }

        /// <summary>
        /// Creates a CharacterStats instance from the stored stats data.
        /// </summary>
        /// <returns>A new CharacterStats instance, or null if data is invalid.</returns>
        private CharacterStats? CreateCharacterStatsFromData()
        {
            if (!this.StatsData.ContainsKey("Health"))
            {
                return null;
            }

            return new CharacterStats
            {
                Health = System.Convert.ToInt32(this.StatsData["Health"], CultureInfo.InvariantCulture),
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
                Luck = System.Convert.ToInt32(this.StatsData.ContainsKey("Luck") ? this.StatsData["Luck"] : 5, CultureInfo.InvariantCulture),
                Strength = System.Convert.ToInt32(this.StatsData.ContainsKey("Strength") ? this.StatsData["Strength"] : 10, CultureInfo.InvariantCulture),
                Dexterity = System.Convert.ToInt32(this.StatsData.ContainsKey("Dexterity") ? this.StatsData["Dexterity"] : 10, CultureInfo.InvariantCulture),
                Constitution = System.Convert.ToInt32(this.StatsData.ContainsKey("Constitution") ? this.StatsData["Constitution"] : 10, CultureInfo.InvariantCulture),
                Intelligence = System.Convert.ToInt32(this.StatsData.ContainsKey("Intelligence") ? this.StatsData["Intelligence"] : 10, CultureInfo.InvariantCulture),
                Wisdom = System.Convert.ToInt32(this.StatsData.ContainsKey("Wisdom") ? this.StatsData["Wisdom"] : 10, CultureInfo.InvariantCulture),
                Charisma = System.Convert.ToInt32(this.StatsData.ContainsKey("Charisma") ? this.StatsData["Charisma"] : 10, CultureInfo.InvariantCulture),
                CriticalChance = System.Convert.ToSingle(this.StatsData.ContainsKey("CriticalChance") ? this.StatsData["CriticalChance"] : 5.0f, CultureInfo.InvariantCulture),
                CriticalDamage = System.Convert.ToSingle(this.StatsData.ContainsKey("CriticalDamage") ? this.StatsData["CriticalDamage"] : 1.5f, CultureInfo.InvariantCulture),
                Evasion = System.Convert.ToSingle(this.StatsData.ContainsKey("Evasion") ? this.StatsData["Evasion"] : 5.0f, CultureInfo.InvariantCulture),
                Accuracy = System.Convert.ToSingle(this.StatsData.ContainsKey("Accuracy") ? this.StatsData["Accuracy"] : 95.0f, CultureInfo.InvariantCulture),
                PhysicalResistance = System.Convert.ToSingle(this.StatsData.ContainsKey("PhysicalResistance") ? this.StatsData["PhysicalResistance"] : 0.0f, CultureInfo.InvariantCulture),
                MagicalResistance = System.Convert.ToSingle(this.StatsData.ContainsKey("MagicalResistance") ? this.StatsData["MagicalResistance"] : 0.0f, CultureInfo.InvariantCulture),
                MovementSpeed = System.Convert.ToSingle(this.StatsData.ContainsKey("MovementSpeed") ? this.StatsData["MovementSpeed"] : 1.0f, CultureInfo.InvariantCulture),
                ExperienceForNextLevel = System.Convert.ToInt32(this.StatsData.ContainsKey("ExperienceForNextLevel") ? this.StatsData["ExperienceForNextLevel"] : 10, CultureInfo.InvariantCulture),
            };
        }
    }
}
