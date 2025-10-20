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

                // Export state data
                if (character.State != null)
                {
                    this.StateData = new Dictionary<string, object>
                    {
                        ["CurrentHealth"] = character.State.CurrentHealth,
                        ["CurrentMana"] = character.State.CurrentMana,
                        ["CurrentLocation"] = character.State.CurrentLocation,
                        ["Status"] = character.State.Status,
                        ["IsInCombat"] = character.State.IsInCombat,
                        ["IsAlive"] = character.State.IsAlive(),
                    };
                }

                // Export progression data
                if (character.Progression != null)
                {
                    this.ProgressionData = new Dictionary<string, object>
                    {
                        ["CurrentExperience"] = character.Progression.CurrentExperience,
                        ["Gold"] = character.Progression.Gold,
                        ["IsAvailable"] = character.Progression.IsAvailable,
                        ["UnlockCondition"] = character.Progression.UnlockCondition,
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
        /// Gets the character's state data.
        /// </summary>
        public Dictionary<string, object> StateData { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets the character's progression data.
        /// </summary>
        public Dictionary<string, object> ProgressionData { get; } = new Dictionary<string, object>();

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
        /// <exception cref="ArgumentException">Thrown when the JSON string is invalid.</exception>
        public static CharacterExportData FromJson(string json)
        {
            var exportData = new CharacterExportData();

            if (string.IsNullOrEmpty(json))
            {
                return exportData;
            }

            try
            {
                var variant = Json.ParseString(json);
                if (variant.VariantType != Variant.Type.Dictionary)
                {
                    GD.PrintErr($"Invalid JSON format: Expected dictionary, got {variant.VariantType}");
                    return exportData;
                }

                var jsonData = variant.AsGodotDictionary();

                // Parse simple string fields
                exportData.Id = GetStringValue(jsonData, "Id", string.Empty);
                exportData.Name = GetStringValue(jsonData, "Name", string.Empty);
                exportData.Description = GetStringValue(jsonData, "Description", string.Empty);
                exportData.ExportTimestamp = GetStringValue(jsonData, "ExportTimestamp", System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture));
                exportData.Version = GetStringValue(jsonData, "Version", "1.0");
                exportData.Source = GetStringValue(jsonData, "Source", "OmegaSpiral");

                // Parse dictionary fields
                CopyDictionaryData(jsonData, "ClassData", exportData.ClassData);
                CopyDictionaryData(jsonData, "AppearanceData", exportData.AppearanceData);
                CopyDictionaryData(jsonData, "StatsData", exportData.StatsData);
                CopyDictionaryData(jsonData, "StateData", exportData.StateData);
                CopyDictionaryData(jsonData, "ProgressionData", exportData.ProgressionData);
                CopyDictionaryData(jsonData, "InventoryData", exportData.InventoryData);
                CopyDictionaryData(jsonData, "EquipmentData", exportData.EquipmentData);
                CopyDictionaryData(jsonData, "LocationData", exportData.LocationData);
                CopyDictionaryData(jsonData, "QuestProgressData", exportData.QuestProgressData);
                CopyDictionaryData(jsonData, "SkillsData", exportData.SkillsData);
                CopyDictionaryData(jsonData, "RelationshipsData", exportData.RelationshipsData);
                CopyDictionaryData(jsonData, "Metadata", exportData.Metadata);
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error parsing JSON character export data: {ex.Message}");
                throw new ArgumentException($"Invalid JSON format for CharacterExportData: {ex.Message}", ex);
            }

            return exportData;
        }

        /// <summary>
        /// Gets a string value from the JSON data with a default fallback.
        /// </summary>
        private static string GetStringValue(Godot.Collections.Dictionary jsonData, string key, string defaultValue)
        {
            if (jsonData.ContainsKey(key))
            {
                return jsonData[key].ToString() ?? defaultValue;
            }
            return defaultValue;
        }

        /// <summary>
        /// Copies dictionary data from JSON to the target dictionary.
        /// </summary>
        private static void CopyDictionaryData(Godot.Collections.Dictionary jsonData, string key, Dictionary<string, object> targetDictionary)
        {
            if (jsonData.ContainsKey(key))
            {
                var dataVariant = jsonData[key];
                if (dataVariant.VariantType == Variant.Type.Dictionary)
                {
                    var godotDict = dataVariant.AsGodotDictionary();
                    targetDictionary.Clear();
                    foreach (var dictKey in godotDict.Keys)
                    {
                        targetDictionary[dictKey.ToString()] = godotDict[dictKey];
                    }
                }
            }
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
        /// Validates whether this export data contains all required information with detailed error reporting.
        /// </summary>
        /// <param name="errorMessages">List of validation error messages.</param>
        /// <returns><see langword="true"/> if the data is valid for export, <see langword="false"/> otherwise.</returns>
        public bool IsValid(out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            if (string.IsNullOrEmpty(this.Id))
            {
                errorMessages.Add("Id is required");
            }

            if (string.IsNullOrEmpty(this.Name))
            {
                errorMessages.Add("Name is required");
            }

            if (this.ClassData.Count == 0)
            {
                errorMessages.Add("ClassData is required");
            }

            if (this.AppearanceData.Count == 0)
            {
                errorMessages.Add("AppearanceData is required");
            }

            if (this.StatsData.Count == 0)
            {
                errorMessages.Add("StatsData is required");
            }

            return errorMessages.Count == 0;
        }

        /// <summary>
        /// Creates a character instance from this export data.
        /// </summary>
        /// <returns>A new character instance populated from the export data.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the export data contains invalid values.</exception>
        public Character CreateCharacter()
        {
            // Create CharacterIdentity
            var identity = new CharacterIdentity(
                id: this.Id ?? Guid.NewGuid().ToString(),
                name: this.Name ?? "Unknown Character",
                description: this.Description ?? "");

            // Create CharacterClass
            CharacterClass characterClass;
            if (this.ClassData.TryGetValue("Id", out object? classIdObj) && classIdObj != null)
            {
                characterClass = new CharacterClass
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
            else
            {
                characterClass = new CharacterClass
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
                }; // Default class
            }

            // Create CharacterAppearance
            CharacterAppearance appearance;
            if (this.AppearanceData.TryGetValue("SkinColor", out object? skinObj) && skinObj != null)
            {
                appearance = new CharacterAppearance
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
            else
            {
                appearance = new CharacterAppearance(); // Default appearance
            }

            // Create CharacterStats
            CharacterStats stats;
            if (this.StatsData.TryGetValue("Health", out object? _))
            {
                stats = new CharacterStats
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
            else
            {
                stats = new CharacterStats(); // Default stats
            }

            // Create CharacterState
            CharacterState state;
            if (this.StateData.Count > 0)
            {
                state = new CharacterState
                {
                    CurrentHealth = GetInt(this.StateData, "CurrentHealth", 0),
                    CurrentMana = GetInt(this.StateData, "CurrentMana", 0),
                    CurrentLocation = GetString(this.StateData, "CurrentLocation"),
                    Status = GetString(this.StateData, "Status", "Alive"),
                    IsInCombat = GetBool(this.StateData, "IsInCombat", false),
                };
            }
            else
            {
                state = new CharacterState();
            }

            // Create CharacterProgression
            CharacterProgression progression;
            if (this.ProgressionData.Count > 0)
            {
                progression = new CharacterProgression
                {
                    CurrentExperience = GetInt(this.ProgressionData, "CurrentExperience", 0),
                    Gold = GetInt(this.ProgressionData, "Gold", 0),
                    IsAvailable = GetBool(this.ProgressionData, "IsAvailable", true),
                    UnlockCondition = GetString(this.ProgressionData, "UnlockCondition"),
                };
            }
            else
            {
                progression = new CharacterProgression();
            }

            return new Character(identity, characterClass, appearance, stats, state, progression);
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

            foreach (var kvp in this.StateData)
            {
                clone.StateData[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in this.ProgressionData)
            {
                clone.ProgressionData[kvp.Key] = kvp.Value;
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
        /// <exception cref="InvalidOperationException">Thrown when the export data is invalid.</exception>
        public string ToJson()
        {
            // Validate data before export
            if (!this.IsValid(out var errorMessages))
            {
                var errorString = string.Join(", ", errorMessages);
                GD.PrintErr($"Cannot export character data: {errorString}");
                throw new InvalidOperationException($"Cannot export character data: {errorString}");
            }

            var disposables = new List<System.IDisposable>();
            try
            {
                var classDict = ConvertToGodotDict(this.ClassData);
                disposables.Add(classDict);
                var appearanceDict = ConvertToGodotDict(this.AppearanceData);
                disposables.Add(appearanceDict);
                var statsDict = ConvertToGodotDict(this.StatsData);
                disposables.Add(statsDict);
                var stateDict = ConvertToGodotDict(this.StateData);
                disposables.Add(stateDict);
                var progressionDict = ConvertToGodotDict(this.ProgressionData);
                disposables.Add(progressionDict);
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
                    ["StateData"] = stateDict,
                    ["ProgressionData"] = progressionDict,
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
            catch (Exception ex)
            {
                GD.PrintErr($"Error converting character export data to JSON: {ex.Message}");
                throw new InvalidOperationException($"Error converting character export data to JSON: {ex.Message}", ex);
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

        private static string GetString(Dictionary<string, object> source, string key, string fallback = "")
        {
            if (!source.TryGetValue(key, out object? raw) || raw == null)
            {
                return fallback;
            }

            return raw.ToString() ?? fallback;
        }

        private static int GetInt(Dictionary<string, object> source, string key, int fallback)
        {
            if (!source.TryGetValue(key, out object? raw) || raw == null)
            {
                return fallback;
            }

            return System.Convert.ToInt32(raw, CultureInfo.InvariantCulture);
        }

        private static float GetFloat(Dictionary<string, object> source, string key, float fallback)
        {
            if (!source.TryGetValue(key, out object? raw) || raw == null)
            {
                return fallback;
            }

            return System.Convert.ToSingle(raw, CultureInfo.InvariantCulture);
        }

        private static bool GetBool(Dictionary<string, object> source, string key, bool fallback)
        {
            if (!source.TryGetValue(key, out object? raw) || raw == null)
            {
                return fallback;
            }

            return System.Convert.ToBoolean(raw, CultureInfo.InvariantCulture);
        }
    }
}
