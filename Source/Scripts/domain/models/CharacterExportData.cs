// <copyright file="CharacterExportData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Globalization;

using Godot;

namespace OmegaSpiral.Domain.Models
{
    /// <summary>
    /// Represents the data structure for exporting character data.
    /// </summary>
    public class CharacterExportData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterExportData"/> class.
        /// </summary>
        public CharacterExportData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterExportData"/> class from a character instance.
        /// </summary>
        /// <param name="character">The character to export data from.</param>
        public CharacterExportData(Character character)
        {
            if (character != null)
            {
                this.Id = character.Id;
                this.Name = character.Name;
                this.Description = character.Description;

                // Export class data
                if (character.Class != null)
                {
                    this.ClassData = new Dictionary<string, object>
                    {
                        ["Id"] = character.Class.Id,
                        ["Name"] = character.Class.Name,
                        ["Description"] = character.Class.Description,
                        ["BaseHealth"] = character.Class.BaseHealth,
                        ["BaseMana"] = character.Class.BaseMana,
                        ["BaseAttack"] = character.Class.BaseAttack,
                        ["BaseDefense"] = character.Class.BaseDefense,
                        ["BaseMagic"] = character.Class.BaseMagic,
                        ["BaseMagicDefense"] = character.Class.BaseMagicDefense,
                        ["BaseSpeed"] = character.Class.BaseSpeed,
                        ["IconPath"] = character.Class.IconPath,
                    };
                }

                // Export appearance data
                if (character.Appearance != null)
                {
                    this.AppearanceData = new Dictionary<string, object>
                    {
                        ["SkinColor"] = character.Appearance.SkinColor.ToHtml(),
                        ["HairColor"] = character.Appearance.HairColor.ToHtml(),
                        ["EyeColor"] = character.Appearance.EyeColor.ToHtml(),
                        ["HairStyle"] = character.Appearance.HairStyle,
                        ["EyeStyle"] = character.Appearance.EyeStyle,
                        ["ClothingStyle"] = character.Appearance.ClothingStyle,
                        ["Accessory"] = character.Appearance.Accessory,
                        ["SpritePath"] = character.Appearance.SpritePath,
                        ["PortraitPath"] = character.Appearance.PortraitPath,
                        ["HeightScale"] = character.Appearance.HeightScale,
                        ["WidthScale"] = character.Appearance.WidthScale,
                        ["BodyType"] = character.Appearance.BodyType,
                        ["FacialExpression"] = character.Appearance.FacialExpression,
                    };
                }

                // Export stats data
                if (character.Stats != null)
                {
                    this.StatsData = new Dictionary<string, object>
                    {
                        ["Health"] = character.Stats.Health,
                        ["MaxHealth"] = character.Stats.MaxHealth,
                        ["Mana"] = character.Stats.Mana,
                        ["MaxMana"] = character.Stats.MaxMana,
                        ["Experience"] = character.Stats.Experience,
                        ["Level"] = character.Stats.Level,
                        ["Attack"] = character.Stats.Attack,
                        ["Defense"] = character.Stats.Defense,
                        ["Magic"] = character.Stats.Magic,
                        ["MagicDefense"] = character.Stats.MagicDefense,
                        ["Speed"] = character.Stats.Speed,
                        ["Luck"] = character.Stats.Luck,
                        ["Strength"] = character.Stats.Strength,
                        ["Dexterity"] = character.Stats.Dexterity,
                        ["Constitution"] = character.Stats.Constitution,
                        ["Intelligence"] = character.Stats.Intelligence,
                        ["Wisdom"] = character.Stats.Wisdom,
                        ["Charisma"] = character.Stats.Charisma,
                        ["CriticalChance"] = character.Stats.CriticalChance,
                        ["CriticalDamage"] = character.Stats.CriticalDamage,
                        ["Evasion"] = character.Stats.Evasion,
                        ["Accuracy"] = character.Stats.Accuracy,
                        ["PhysicalResistance"] = character.Stats.PhysicalResistance,
                        ["MagicalResistance"] = character.Stats.MagicalResistance,
                        ["MovementSpeed"] = character.Stats.MovementSpeed,
                        ["ExperienceForNextLevel"] = character.Stats.ExperienceForNextLevel,
                    };
                }
            }
        }

        /// <summary>
        /// Gets the character's unique identifier.
        /// </summary>
        public string Id { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the character's name.
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the character's description.
        /// </summary>
        public string Description { get; private set; } = string.Empty;

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
        /// Gets the timestamp of when this data was exported.
        /// </summary>
        public string ExportTimestamp { get; private set; } = System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        /// <summary>
        /// Gets the version of the export format.
        /// </summary>
        public string Version { get; private set; } = "1.0";

        /// <summary>
        /// Gets the source game or application.
        /// </summary>
        public string Source { get; private set; } = "OmegaSpiral";

        /// <summary>
        /// Gets additional metadata for the export.
        /// </summary>
        public Dictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Creates export data from a JSON string representation.
        /// </summary>
        /// <param name="json">The JSON string to parse.</param>
        /// <returns>A new instance of <see cref="CharacterExportData"/> parsed from the JSON.</returns>
        public static CharacterExportData FromJson(string json)
        {
            var exportData = new CharacterExportData();

            if (string.IsNullOrEmpty(json))
            {
                return exportData;
            }

            var variant = Json.ParseString(json);
            if (variant.VariantType != Variant.Type.Dictionary)
            {
                return exportData;
            }

            var jsonData = variant.AsGodotDictionary();

            if (jsonData.ContainsKey("Id"))
            {
                exportData.Id = jsonData["Id"].ToString() ?? string.Empty;
            }

            if (jsonData.ContainsKey("Name"))
            {
                exportData.Name = jsonData["Name"].ToString() ?? string.Empty;
            }

            if (jsonData.ContainsKey("Description"))
            {
                exportData.Description = jsonData["Description"].ToString() ?? string.Empty;
            }

            if (jsonData.ContainsKey("ClassData"))
            {
                var classDataVariant = jsonData["ClassData"];
                if (classDataVariant.VariantType == Variant.Type.Dictionary)
                {
                    var godotDict = classDataVariant.AsGodotDictionary();
                    exportData.ClassData.Clear();
                    foreach (var key in godotDict.Keys)
                    {
                        exportData.ClassData[key.ToString()] = godotDict[key];
                    }
                }
            }

            if (jsonData.ContainsKey("AppearanceData"))
            {
                var appearanceDataVariant = jsonData["AppearanceData"];
                if (appearanceDataVariant.VariantType == Variant.Type.Dictionary)
                {
                    var godotDict = appearanceDataVariant.AsGodotDictionary();
                    exportData.AppearanceData.Clear();
                    foreach (var key in godotDict.Keys)
                    {
                        exportData.AppearanceData[key.ToString()] = godotDict[key];
                    }
                }
            }

            if (jsonData.ContainsKey("StatsData"))
            {
                var statsDataVariant = jsonData["StatsData"];
                if (statsDataVariant.VariantType == Variant.Type.Dictionary)
                {
                    var godotDict = statsDataVariant.AsGodotDictionary();
                    exportData.StatsData.Clear();
                    foreach (var key in godotDict.Keys)
                    {
                        exportData.StatsData[key.ToString()] = godotDict[key];
                    }
                }
            }

            if (jsonData.ContainsKey("InventoryData"))
            {
                var inventoryDataVariant = jsonData["InventoryData"];
                if (inventoryDataVariant.VariantType == Variant.Type.Dictionary)
                {
                    var godotDict = inventoryDataVariant.AsGodotDictionary();
                    exportData.InventoryData.Clear();
                    foreach (var key in godotDict.Keys)
                    {
                        exportData.InventoryData[key.ToString()] = godotDict[key];
                    }
                }
            }

            if (jsonData.ContainsKey("EquipmentData"))
            {
                var equipmentDataVariant = jsonData["EquipmentData"];
                if (equipmentDataVariant.VariantType == Variant.Type.Dictionary)
                {
                    var godotDict = equipmentDataVariant.AsGodotDictionary();
                    exportData.EquipmentData.Clear();
                    foreach (var key in godotDict.Keys)
                    {
                        exportData.EquipmentData[key.ToString()] = godotDict[key];
                    }
                }
            }

            if (jsonData.ContainsKey("LocationData"))
            {
                var locationDataVariant = jsonData["LocationData"];
                if (locationDataVariant.VariantType == Variant.Type.Dictionary)
                {
                    var godotDict = locationDataVariant.AsGodotDictionary();
                    exportData.LocationData.Clear();
                    foreach (var key in godotDict.Keys)
                    {
                        exportData.LocationData[key.ToString()] = godotDict[key];
                    }
                }
            }

            if (jsonData.ContainsKey("QuestProgressData"))
            {
                var questProgressDataVariant = jsonData["QuestProgressData"];
                if (questProgressDataVariant.VariantType == Variant.Type.Dictionary)
                {
                    var godotDict = questProgressDataVariant.AsGodotDictionary();
                    exportData.QuestProgressData.Clear();
                    foreach (var key in godotDict.Keys)
                    {
                        exportData.QuestProgressData[key.ToString()] = godotDict[key];
                    }
                }
            }

            if (jsonData.ContainsKey("SkillsData"))
            {
                var skillsDataVariant = jsonData["SkillsData"];
                if (skillsDataVariant.VariantType == Variant.Type.Dictionary)
                {
                    var godotDict = skillsDataVariant.AsGodotDictionary();
                    exportData.SkillsData.Clear();
                    foreach (var key in godotDict.Keys)
                    {
                        exportData.SkillsData[key.ToString()] = godotDict[key];
                    }
                }
            }

            if (jsonData.ContainsKey("RelationshipsData"))
            {
                var relationshipsDataVariant = jsonData["RelationshipsData"];
                if (relationshipsDataVariant.VariantType == Variant.Type.Dictionary)
                {
                    var godotDict = relationshipsDataVariant.AsGodotDictionary();
                    exportData.RelationshipsData.Clear();
                    foreach (var key in godotDict.Keys)
                    {
                        exportData.RelationshipsData[key.ToString()] = godotDict[key];
                    }
                }
            }

            if (jsonData.ContainsKey("ExportTimestamp"))
            {
                exportData.ExportTimestamp = jsonData["ExportTimestamp"].ToString() ?? System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            }

            if (jsonData.ContainsKey("Version"))
            {
                exportData.Version = jsonData["Version"].ToString() ?? "1.0";
            }

            if (jsonData.ContainsKey("Source"))
            {
                exportData.Source = jsonData["Source"].ToString() ?? "OmegaSpiral";
            }

            if (jsonData.ContainsKey("Metadata"))
            {
                var metadataVariant = jsonData["Metadata"];
                if (metadataVariant.VariantType == Variant.Type.Dictionary)
                {
                    var godotDict = metadataVariant.AsGodotDictionary();
                    exportData.Metadata.Clear();
                    foreach (var key in godotDict.Keys)
                    {
                        exportData.Metadata[key.ToString()] = godotDict[key];
                    }
                }
            }

            return exportData;
        }

        /// <summary>
        /// Validates whether this export data contains all required information.
        /// </summary>
        /// <returns><see langword="true"/> if the data is valid for export, <see langword="false"/> otherwise.</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(this.Id) &&
                   !string.IsNullOrEmpty(this.Name) &&
                   this.ClassData.Count > 0 &&
                   this.AppearanceData.Count > 0 &&
                   this.StatsData.Count > 0;
        }

        /// <summary>
        /// Creates a character instance from this export data.
        /// </summary>
        /// <returns>A new character instance populated from the export data.</returns>
        public Character CreateCharacter()
        {
            var character = new Character
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
            };

            // Deserialize class data if available
            if (this.ClassData.TryGetValue("Id", out object? classIdObj) && classIdObj != null)
            {
                character.Class = new CharacterClass
                {
                    Id = classIdObj.ToString() ?? string.Empty,
                    Name = GetString(this.ClassData, "Name"),
                    Description = GetString(this.ClassData, "Description"),
                    BaseHealth = GetInt(this.ClassData, "BaseHealth", 100),
                    BaseMana = GetInt(this.ClassData, "BaseMana", 50),
                    BaseAttack = GetInt(this.ClassData, "BaseAttack", 10),
                    BaseDefense = GetInt(this.ClassData, "BaseDefense", 5),
                    BaseMagic = GetInt(this.ClassData, "BaseMagic", 5),
                    BaseMagicDefense = GetInt(this.ClassData, "BaseMagicDefense", 5),
                    BaseSpeed = GetInt(this.ClassData, "BaseSpeed", 10),
                    IconPath = GetString(this.ClassData, "IconPath"),
                };
            }

            // Deserialize appearance data if available
            if (this.AppearanceData.TryGetValue("SkinColor", out object? skinObj) && skinObj != null)
            {
                character.Appearance = new CharacterAppearance
                {
                    SkinColor = Color.FromHtml(skinObj.ToString() ?? "#FFFFFF"),
                    HairColor = Color.FromHtml(GetString(this.AppearanceData, "HairColor", "#000000")),
                    EyeColor = Color.FromHtml(GetString(this.AppearanceData, "EyeColor", "#000000")),
                    HairStyle = GetString(this.AppearanceData, "HairStyle"),
                    EyeStyle = GetString(this.AppearanceData, "EyeStyle"),
                    ClothingStyle = GetString(this.AppearanceData, "ClothingStyle"),
                    Accessory = GetString(this.AppearanceData, "Accessory"),
                    SpritePath = GetString(this.AppearanceData, "SpritePath"),
                    PortraitPath = GetString(this.AppearanceData, "PortraitPath"),
                    HeightScale = GetFloat(this.AppearanceData, "HeightScale", 1.0f),
                    WidthScale = GetFloat(this.AppearanceData, "WidthScale", 1.0f),
                    BodyType = GetString(this.AppearanceData, "BodyType", "Normal"),
                    FacialExpression = GetString(this.AppearanceData, "FacialExpression", "Neutral"),
                };
            }

            // Deserialize stats data if available
            if (this.StatsData.TryGetValue("Health", out object? _))
            {
                character.Stats = new CharacterStats
                {
                    Health = GetInt(this.StatsData, "Health", 0),
                    MaxHealth = GetInt(this.StatsData, "MaxHealth", 0),
                    Mana = GetInt(this.StatsData, "Mana", 0),
                    MaxMana = GetInt(this.StatsData, "MaxMana", 0),
                    Experience = GetInt(this.StatsData, "Experience", 0),
                    Level = GetInt(this.StatsData, "Level", 1),
                    Attack = GetInt(this.StatsData, "Attack", 0),
                    Defense = GetInt(this.StatsData, "Defense", 0),
                    Magic = GetInt(this.StatsData, "Magic", 0),
                    MagicDefense = GetInt(this.StatsData, "MagicDefense", 0),
                    Speed = GetInt(this.StatsData, "Speed", 0),
                    Luck = GetInt(this.StatsData, "Luck", 5),
                    Strength = GetInt(this.StatsData, "Strength", 10),
                    Dexterity = GetInt(this.StatsData, "Dexterity", 10),
                    Constitution = GetInt(this.StatsData, "Constitution", 10),
                    Intelligence = GetInt(this.StatsData, "Intelligence", 10),
                    Wisdom = GetInt(this.StatsData, "Wisdom", 10),
                    Charisma = GetInt(this.StatsData, "Charisma", 10),
                    CriticalChance = GetFloat(this.StatsData, "CriticalChance", 5.0f),
                    CriticalDamage = GetFloat(this.StatsData, "CriticalDamage", 1.5f),
                    Evasion = GetFloat(this.StatsData, "Evasion", 5.0f),
                    Accuracy = GetFloat(this.StatsData, "Accuracy", 95.0f),
                    PhysicalResistance = GetFloat(this.StatsData, "PhysicalResistance", 0.0f),
                    MagicalResistance = GetFloat(this.StatsData, "MagicalResistance", 0.0f),
                    MovementSpeed = GetFloat(this.StatsData, "MovementSpeed", 1.0f),
                    ExperienceForNextLevel = GetInt(this.StatsData, "ExperienceForNextLevel", 10),
                };
            }

            return character;
        }

        /// <summary>
        /// Creates a deep copy of this export data.
        /// </summary>
        /// <returns>A new instance of <see cref="CharacterExportData"/> with the same values.</returns>
        public CharacterExportData Clone()
        {
            var clone = new CharacterExportData
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                ExportTimestamp = this.ExportTimestamp,
                Version = this.Version,
                Source = this.Source,
            };

            // Copy dictionary contents
            foreach (var kvp in this.ClassData)
            {
                clone.ClassData[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in this.AppearanceData)
            {
                clone.AppearanceData[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in this.StatsData)
            {
                clone.StatsData[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in this.InventoryData)
            {
                clone.InventoryData[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in this.EquipmentData)
            {
                clone.EquipmentData[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in this.LocationData)
            {
                clone.LocationData[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in this.QuestProgressData)
            {
                clone.QuestProgressData[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in this.SkillsData)
            {
                clone.SkillsData[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in this.RelationshipsData)
            {
                clone.RelationshipsData[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in this.Metadata)
            {
                clone.Metadata[kvp.Key] = kvp.Value;
            }

            return clone;
        }

        /// <summary>
        /// Converts this export data to a JSON string representation.
        /// </summary>
        /// <returns>A JSON string representation of the export data.</returns>
        public string ToJson()
        {
            var disposables = new List<System.IDisposable>();
            try
            {
                var classDict = ConvertToGodotDict(this.ClassData);
                disposables.Add(classDict);
                var appearanceDict = ConvertToGodotDict(this.AppearanceData);
                disposables.Add(appearanceDict);
                var statsDict = ConvertToGodotDict(this.StatsData);
                disposables.Add(statsDict);
                var inventoryDict = ConvertToGodotDict(this.InventoryData);
                disposables.Add(inventoryDict);
                var equipmentDict = ConvertToGodotDict(this.EquipmentData);
                disposables.Add(equipmentDict);
                var locationDict = ConvertToGodotDict(this.LocationData);
                disposables.Add(locationDict);
                var questProgressDict = ConvertToGodotDict(this.QuestProgressData);
                disposables.Add(questProgressDict);
                var skillsDict = ConvertToGodotDict(this.SkillsData);
                disposables.Add(skillsDict);
                var relationshipsDict = ConvertToGodotDict(this.RelationshipsData);
                disposables.Add(relationshipsDict);
                var metadataDict = ConvertToGodotDict(this.Metadata);
                disposables.Add(metadataDict);

                var exportDict = new Godot.Collections.Dictionary
                {
                    ["Id"] = this.Id,
                    ["Name"] = this.Name,
                    ["Description"] = this.Description,
                    ["ClassData"] = classDict,
                    ["AppearanceData"] = appearanceDict,
                    ["StatsData"] = statsDict,
                    ["InventoryData"] = inventoryDict,
                    ["EquipmentData"] = equipmentDict,
                    ["LocationData"] = locationDict,
                    ["QuestProgressData"] = questProgressDict,
                    ["SkillsData"] = skillsDict,
                    ["RelationshipsData"] = relationshipsDict,
                    ["ExportTimestamp"] = this.ExportTimestamp,
                    ["Version"] = this.Version,
                    ["Source"] = this.Source,
                    ["Metadata"] = metadataDict,
                };
                disposables.Add(exportDict);

                return Json.Stringify(exportDict);
            }
            finally
            {
                foreach (var disposable in disposables)
                {
                    disposable.Dispose();
                }
            }
        }

        private static Godot.Collections.Dictionary ConvertToGodotDict(Dictionary<string, object> dict)
        {
            var godotDict = new Godot.Collections.Dictionary();
            foreach (var kvp in dict)
            {
                godotDict[kvp.Key] = (Variant) kvp.Value;
            }

            return godotDict;
        }

        private static string GetString(IDictionary<string, object> source, string key, string fallback = "")
        {
            if (!source.TryGetValue(key, out object? raw) || raw == null)
            {
                return fallback;
            }

            return raw.ToString() ?? fallback;
        }

        private static int GetInt(IDictionary<string, object> source, string key, int fallback)
        {
            if (!source.TryGetValue(key, out object? raw) || raw == null)
            {
                return fallback;
            }

            return System.Convert.ToInt32(raw, CultureInfo.InvariantCulture);
        }

        private static float GetFloat(IDictionary<string, object> source, string key, float fallback)
        {
            if (!source.TryGetValue(key, out object? raw) || raw == null)
            {
                return fallback;
            }

            return System.Convert.ToSingle(raw, CultureInfo.InvariantCulture);
        }
    }
}
