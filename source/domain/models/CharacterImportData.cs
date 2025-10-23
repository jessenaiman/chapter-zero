// <copyright file="CharacterImportData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

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
        /// Validates whether this import data contains all required information with detailed error reporting.
        /// </summary>
        /// <param name="errorMessages">List of validation error messages.</param>
        /// <returns><see langword="true"/> if the data is valid for import, <see langword="false"/> otherwise.</returns>
        public bool IsValid(out ICollection<string> errorMessages)
        {
            var errors = new List<string>();
            errorMessages = errors;

            if (string.IsNullOrEmpty(this.Id))
            {
                errors.Add("Id is required");
            }

            if (string.IsNullOrEmpty(this.Name))
            {
                errors.Add("Name is required");
            }

            if (this.ClassData.Count == 0)
            {
                errors.Add("ClassData is required");
            }

            if (this.AppearanceData.Count == 0)
            {
                errors.Add("AppearanceData is required");
            }

            if (this.StatsData.Count == 0)
            {
                errors.Add("StatsData is required");
            }

            return errors.Count == 0;
        }

        /// <summary>
        /// Creates a character instance from this import data.
        /// </summary>
        /// <returns>A new character instance populated from the import data.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the import data contains invalid values.</exception>
        public Character CreateCharacter()
        {
            try
            {
                // Create CharacterIdentity
                var identity = new CharacterIdentity(
                    id: this.Id ?? Guid.NewGuid().ToString(),
                    name: this.Name ?? "Unknown Character",
                    description: this.Description ?? "");

                // Create components from data
                var characterClass = this.GetOrCreateCharacterClass();
                var appearance = this.GetOrCreateCharacterAppearance();
                var stats = this.GetOrCreateCharacterStats();
                var state = this.GetOrCreateCharacterState();
                var progression = this.GetOrCreateCharacterProgression();

                // Create character with all available data
                var character = new Character(
                    identity: identity,
                    characterClass: characterClass,
                    appearance: appearance,
                    stats: stats,
                    state: state,
                    progression: progression);

                return character;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error creating character from import data: {ex.Message}");
                throw new InvalidOperationException($"Error creating character from import data: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets or creates a CharacterClass from stored data or returns a new default instance.
        /// </summary>
        private CharacterClass GetOrCreateCharacterClass()
        {
            if (this.ClassData.Count > 0)
            {
                var createdClass = this.CreateCharacterClassFromData();
                if (createdClass != null)
                {
                    return createdClass;
                }
            }

            return new CharacterClass();
        }

        /// <summary>
        /// Gets or creates a CharacterAppearance from stored data or returns a new default instance.
        /// </summary>
        private CharacterAppearance GetOrCreateCharacterAppearance()
        {
            if (this.AppearanceData.Count > 0)
            {
                var createdAppearance = this.CreateCharacterAppearanceFromData();
                if (createdAppearance != null)
                {
                    return createdAppearance;
                }
            }

            return new CharacterAppearance();
        }

        /// <summary>
        /// Gets or creates a CharacterStats from stored data or returns a new default instance.
        /// </summary>
        private CharacterStats GetOrCreateCharacterStats()
        {
            if (this.StatsData.Count > 0)
            {
                var createdStats = this.CreateCharacterStatsFromData();
                if (createdStats != null)
                {
                    return createdStats;
                }
            }

            return new CharacterStats();
        }

        /// <summary>
        /// Gets or creates a CharacterState from stored data, or returns null if no data available.
        /// </summary>
        private CharacterState? GetOrCreateCharacterState()
        {
            if (this.StateData.Count > 0)
            {
                return this.CreateCharacterStateFromData();
            }

            return null;
        }

        /// <summary>
        /// Gets or creates a CharacterProgression from stored data, or returns null if no data available.
        /// </summary>
        private CharacterProgression? GetOrCreateCharacterProgression()
        {
            if (this.ProgressionData.Count > 0)
            {
                return this.CreateCharacterProgressionFromData();
            }

            return null;
        }

        /// <summary>
        /// Merges this import data with an existing character, updating only specified fields.
        /// </summary>
        /// <param name="existingCharacter">The existing character to update.</param>
        /// <param name="updateClass">Whether to update the character class data.</param>
        /// <param name="updateAppearance">Whether to update the character appearance data.</param>
        /// <param name="updateStats">Whether to update the character stats data.</param>
        /// <param name="updateState">Whether to update the character state data.</param>
        /// <param name="updateProgression">Whether to update the character progression data.</param>
        public void MergeInto(Character? existingCharacter, bool updateClass = true, bool updateAppearance = true, bool updateStats = true, bool updateState = true, bool updateProgression = true)
        {
            if (existingCharacter is null)
            {
                return;
            }

            this.MergeIdentityData(existingCharacter);
            this.MergeClassData(existingCharacter, updateClass);
            this.MergeAppearanceData(existingCharacter, updateAppearance);
            this.MergeStatsData(existingCharacter, updateStats);
            this.MergeStateData(existingCharacter, updateState);
            this.MergeProgressionData(existingCharacter, updateProgression);
        }

        /// <summary>
        /// Merges identity data (Name, Description) into an existing character.
        /// </summary>
        private void MergeIdentityData(Character existingCharacter)
        {
            existingCharacter.Name = string.IsNullOrEmpty(this.Name) ? existingCharacter.Name : this.Name;
            existingCharacter.Description = string.IsNullOrEmpty(this.Description) ? existingCharacter.Description : this.Description;
        }

        /// <summary>
        /// Merges class data into an existing character if update is enabled.
        /// </summary>
        private void MergeClassData(Character existingCharacter, bool updateClass)
        {
            if (updateClass && this.ClassData.Count > 0)
            {
                var newClass = this.CreateCharacterClassFromData();
                if (newClass != null)
                {
                    existingCharacter.Class = newClass;
                }
            }
        }

        /// <summary>
        /// Merges appearance data into an existing character if update is enabled.
        /// </summary>
        private void MergeAppearanceData(Character existingCharacter, bool updateAppearance)
        {
            if (updateAppearance && this.AppearanceData.Count > 0)
            {
                var newAppearance = this.CreateCharacterAppearanceFromData();
                if (newAppearance != null)
                {
                    existingCharacter.Appearance = newAppearance;
                }
            }
        }

        /// <summary>
        /// Merges stats data into an existing character if update is enabled.
        /// </summary>
        private void MergeStatsData(Character existingCharacter, bool updateStats)
        {
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
        /// Merges state data into an existing character if update is enabled.
        /// </summary>
        private void MergeStateData(Character existingCharacter, bool updateState)
        {
            if (updateState && this.StateData.Count > 0)
            {
                var newState = this.CreateCharacterStateFromData();
                if (newState != null)
                {
                    existingCharacter.State = newState;
                }
            }
        }

        /// <summary>
        /// Merges progression data into an existing character if update is enabled.
        /// </summary>
        private void MergeProgressionData(Character existingCharacter, bool updateProgression)
        {
            if (updateProgression && this.ProgressionData.Count > 0)
            {
                var newProgression = this.CreateCharacterProgressionFromData();
                if (newProgression != null)
                {
                    existingCharacter.Progression = newProgression;
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
                BaseHealth = this.GetClassInt("BaseHealth", 100),
                BaseMana = this.GetClassInt("BaseMana", 50),
                BaseAttack = this.GetClassInt("BaseAttack", 10),
                BaseDefense = this.GetClassInt("BaseDefense", 5),
                BaseMagic = this.GetClassInt("BaseMagic", 5),
                BaseMagicDefense = this.GetClassInt("BaseMagicDefense", 5),
                BaseSpeed = this.GetClassInt("BaseSpeed", 10),
                IconPath = this.GetClassString("IconPath"),
            };
        }

        /// <summary>
        /// Gets an integer value from class data with a default fallback.
        /// </summary>
        private int GetClassInt(string key, int defaultValue)
        {
            return this.ClassData.TryGetValue(key, out object? value) ? Convert.ToInt32(value, CultureInfo.InvariantCulture) : defaultValue;
        }

        /// <summary>
        /// Gets a string value from class data with a default fallback.
        /// </summary>
        private string GetClassString(string key, string defaultValue = "")
        {
            return this.ClassData.TryGetValue(key, out object? value) ? value?.ToString() ?? defaultValue : defaultValue;
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
                HairStyle = this.GetAppearanceString("HairStyle"),
                EyeStyle = this.GetAppearanceString("EyeStyle"),
                ClothingStyle = this.GetAppearanceString("ClothingStyle"),
                Accessory = this.GetAppearanceString("Accessory"),
                SpritePath = this.GetAppearanceString("SpritePath"),
                PortraitPath = this.GetAppearanceString("PortraitPath"),
                HeightScale = this.GetAppearanceFloat("HeightScale", 1.0f),
                WidthScale = this.GetAppearanceFloat("WidthScale", 1.0f),
                BodyType = this.GetAppearanceString("BodyType", "Normal"),
                FacialExpression = this.GetAppearanceString("FacialExpression", "Neutral"),
            };
        }

        /// <summary>
        /// Gets a string value from appearance data with a default fallback.
        /// </summary>
        private string GetAppearanceString(string key, string defaultValue = "")
        {
            return this.AppearanceData.TryGetValue(key, out object? value) ? value?.ToString() ?? defaultValue : defaultValue;
        }

        /// <summary>
        /// Gets a float value from appearance data with a default fallback.
        /// </summary>
        private float GetAppearanceFloat(string key, float defaultValue)
        {
            return this.AppearanceData.TryGetValue(key, out object? value) ? Convert.ToSingle(value, CultureInfo.InvariantCulture) : defaultValue;
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
                Health = this.GetStatsInt("Health", 0),
                MaxHealth = this.GetStatsInt("MaxHealth", 100),
                Mana = this.GetStatsInt("Mana", 0),
                MaxMana = this.GetStatsInt("MaxMana", 50),
                Experience = this.GetStatsInt("Experience", 0),
                Level = this.GetStatsInt("Level", 1),
                Attack = this.GetStatsInt("Attack", 10),
                Defense = this.GetStatsInt("Defense", 5),
                Magic = this.GetStatsInt("Magic", 5),
                MagicDefense = this.GetStatsInt("MagicDefense", 5),
                Speed = this.GetStatsInt("Speed", 10),
                Luck = this.GetStatsInt("Luck", 5),
                Strength = this.GetStatsInt("Strength", 10),
                Dexterity = this.GetStatsInt("Dexterity", 10),
                Constitution = this.GetStatsInt("Constitution", 10),
                Intelligence = this.GetStatsInt("Intelligence", 10),
                Wisdom = this.GetStatsInt("Wisdom", 10),
                Charisma = this.GetStatsInt("Charisma", 10),
                CriticalChance = this.GetStatsFloat("CriticalChance", 5.0f),
                CriticalDamage = this.GetStatsFloat("CriticalDamage", 1.5f),
                Evasion = this.GetStatsFloat("Evasion", 5.0f),
                Accuracy = this.GetStatsFloat("Accuracy", 95.0f),
                PhysicalResistance = this.GetStatsFloat("PhysicalResistance", 0.0f),
                MagicalResistance = this.GetStatsFloat("MagicalResistance", 0.0f),
                MovementSpeed = this.GetStatsFloat("MovementSpeed", 1.0f),
                ExperienceForNextLevel = this.GetStatsInt("ExperienceForNextLevel", 10),
            };
        }

        /// <summary>
        /// Gets an integer value from stats data with a default fallback.
        /// </summary>
        private int GetStatsInt(string key, int defaultValue)
        {
            return this.StatsData.TryGetValue(key, out object? value) ? Convert.ToInt32(value, CultureInfo.InvariantCulture) : defaultValue;
        }

        /// <summary>
        /// Gets a float value from stats data with a default fallback.
        /// </summary>
        private float GetStatsFloat(string key, float defaultValue)
        {
            return this.StatsData.TryGetValue(key, out object? value) ? Convert.ToSingle(value, CultureInfo.InvariantCulture) : defaultValue;
        }

        /// <summary>
        /// Creates a CharacterState instance from the stored state data.
        /// </summary>
        /// <returns>A new CharacterState instance, or null if data is invalid.</returns>
        private CharacterState? CreateCharacterStateFromData()
        {
            var state = new CharacterState();
            this.PopulateStateHealth(state);
            this.PopulateStateMana(state);
            this.PopulateStateLocation(state);
            this.PopulateStateStatus(state);
            this.PopulateStateCombat(state);
            return state;
        }

        /// <summary>
        /// Populates the health value in the character state.
        /// </summary>
        private void PopulateStateHealth(CharacterState state)
        {
            state.CurrentHealth = this.GetStateInt("CurrentHealth", 0);
        }

        /// <summary>
        /// Populates the mana value in the character state.
        /// </summary>
        private void PopulateStateMana(CharacterState state)
        {
            state.CurrentMana = this.GetStateInt("CurrentMana", 0);
        }

        /// <summary>
        /// Populates the location value in the character state.
        /// </summary>
        private void PopulateStateLocation(CharacterState state)
        {
            state.CurrentLocation = this.GetStateString("CurrentLocation");
        }

        /// <summary>
        /// Populates the status value in the character state.
        /// </summary>
        private void PopulateStateStatus(CharacterState state)
        {
            state.Status = this.GetStateString("Status", "Alive");
        }

        /// <summary>
        /// Populates the combat status in the character state.
        /// </summary>
        private void PopulateStateCombat(CharacterState state)
        {
            state.IsInCombat = this.GetStateBoolean("IsInCombat", false);
        }

        /// <summary>
        /// Gets an integer value from state data with a default fallback.
        /// </summary>
        private int GetStateInt(string key, int defaultValue)
        {
            return this.StateData.TryGetValue(key, out object? value) ? Convert.ToInt32(value, CultureInfo.InvariantCulture) : defaultValue;
        }

        /// <summary>
        /// Gets a string value from state data with a default fallback.
        /// </summary>
        private string GetStateString(string key, string defaultValue = "")
        {
            return this.StateData.TryGetValue(key, out object? value) ? value?.ToString() ?? defaultValue : defaultValue;
        }

        /// <summary>
        /// Gets a boolean value from state data with a default fallback.
        /// </summary>
        private bool GetStateBoolean(string key, bool defaultValue)
        {
            return this.StateData.TryGetValue(key, out object? value) ? Convert.ToBoolean(value, CultureInfo.InvariantCulture) : defaultValue;
        }

        /// <summary>
        /// Creates a CharacterProgression instance from the stored progression data.
        /// </summary>
        /// <returns>A new CharacterProgression instance, or null if data is invalid.</returns>
        private CharacterProgression? CreateCharacterProgressionFromData()
        {
            var progression = new CharacterProgression();
            this.PopulateProgressionExperience(progression);
            this.PopulateProgressionGold(progression);
            this.PopulateProgressionAvailability(progression);
            this.PopulateProgressionUnlockCondition(progression);
            return progression;
        }

        /// <summary>
        /// Populates the experience value in character progression.
        /// </summary>
        private void PopulateProgressionExperience(CharacterProgression progression)
        {
            progression.CurrentExperience = this.GetProgressionInt("CurrentExperience", 0);
        }

        /// <summary>
        /// Populates the gold value in character progression.
        /// </summary>
        private void PopulateProgressionGold(CharacterProgression progression)
        {
            progression.Gold = this.GetProgressionInt("Gold", 0);
        }

        /// <summary>
        /// Populates the availability status in character progression.
        /// </summary>
        private void PopulateProgressionAvailability(CharacterProgression progression)
        {
            progression.IsAvailable = this.GetProgressionBoolean("IsAvailable", false);
        }

        /// <summary>
        /// Populates the unlock condition in character progression.
        /// </summary>
        private void PopulateProgressionUnlockCondition(CharacterProgression progression)
        {
            progression.UnlockCondition = this.GetProgressionString("UnlockCondition");
        }

        /// <summary>
        /// Gets an integer value from progression data with a default fallback.
        /// </summary>
        private int GetProgressionInt(string key, int defaultValue)
        {
            return this.ProgressionData.TryGetValue(key, out object? value) ? Convert.ToInt32(value, CultureInfo.InvariantCulture) : defaultValue;
        }

        /// <summary>
        /// Gets a string value from progression data with a default fallback.
        /// </summary>
        private string GetProgressionString(string key, string defaultValue = "")
        {
            return this.ProgressionData.TryGetValue(key, out object? value) ? value?.ToString() ?? defaultValue : defaultValue;
        }

        /// <summary>
        /// Gets a boolean value from progression data with a default fallback.
        /// </summary>
        private bool GetProgressionBoolean(string key, bool defaultValue)
        {
            return this.ProgressionData.TryGetValue(key, out object? value) ? Convert.ToBoolean(value, CultureInfo.InvariantCulture) : defaultValue;
        }
    }
}
