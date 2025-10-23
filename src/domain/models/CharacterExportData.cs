// <copyright file="CharacterExportData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;
using Godot;

namespace OmegaSpiral.Domain.Models
{
    /// <summary>
    /// Represents the data structure for exporting character data.
    /// </summary>
    public class CharacterExportData
    {
        /// <summary>
        /// Gets or sets the unique identifier for the character.
        /// </summary>
        [JsonPropertyName("Id")]
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the character's display name.
        /// </summary>
        [JsonPropertyName("Name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the character's description.
        /// </summary>
        [JsonPropertyName("Description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the character's class data.
        /// </summary>
        [JsonPropertyName("ClassData")]
        public CharacterClass? ClassData { get; set; }

        /// <summary>
        /// Gets or sets the character's appearance data.
        /// </summary>
        [JsonPropertyName("AppearanceData")]
        public CharacterAppearance? AppearanceData { get; set; }

        /// <summary>
        /// Gets or sets the character's stats data.
        /// </summary>
        [JsonPropertyName("StatsData")]
        public CharacterStats? StatsData { get; set; }

        /// <summary>
        /// Gets or sets the character's state data.
        /// </summary>
        [JsonPropertyName("StateData")]
        public CharacterState? StateData { get; set; }

        /// <summary>
        /// Gets or sets the character's progression data.
        /// </summary>
        [JsonPropertyName("ProgressionData")]
        public CharacterProgression? ProgressionData { get; set; }

        /// <summary>
        /// Gets or sets the character's inventory data.
        /// </summary>
        [JsonPropertyName("InventoryData")]
        public Dictionary<string, object>? InventoryData { get; set; }

        /// <summary>
        /// Gets or sets the character's equipment data.
        /// </summary>
        [JsonPropertyName("EquipmentData")]
        public Dictionary<string, object>? EquipmentData { get; set; }

        /// <summary>
        /// Gets or sets the character's location data.
        /// </summary>
        [JsonPropertyName("LocationData")]
        public Dictionary<string, object>? LocationData { get; set; }

        /// <summary>
        /// Gets or sets the character's quest progress data.
        /// </summary>
        [JsonPropertyName("QuestProgressData")]
        public Dictionary<string, object>? QuestProgressData { get; set; }

        /// <summary>
        /// Gets or sets the character's skills data.
        /// </summary>
        [JsonPropertyName("SkillsData")]
        public Dictionary<string, object>? SkillsData { get; set; }

        /// <summary>
        /// Gets or sets the character's relationships data.
        /// </summary>
        [JsonPropertyName("RelationshipsData")]
        public Dictionary<string, object>? RelationshipsData { get; set; }

        /// <summary>
        /// Gets or sets the export timestamp.
        /// </summary>
        [JsonPropertyName("ExportTimestamp")]
        public string? ExportTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the version of the export format.
        /// </summary>
        [JsonPropertyName("Version")]
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// Gets or sets the source game or application.
        /// </summary>
        [JsonPropertyName("Source")]
        public string Source { get; set; } = "OmegaSpiral";

        /// <summary>
        /// Gets or sets additional metadata for the export.
        /// </summary>
        [JsonPropertyName("Metadata")]
        public Dictionary<string, object>? Metadata { get; set; }

        /// <summary>
        /// Validates whether this export data contains all required information.
        /// </summary>
        /// <returns><see langword="true"/> if the data is valid for export, <see langword="false"/> otherwise.</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(this.Id)
                && !string.IsNullOrEmpty(this.Name)
                && this.ClassData != null
                && this.AppearanceData != null
                && this.StatsData != null;
        }

        /// <summary>
        /// Creates a character instance from this export data.
        /// </summary>
        /// <returns>A new character instance populated from the export data.</returns>
        public Character CreateCharacter()
        {
            var identity = new CharacterIdentity(
                this.Id ?? string.Empty,
                this.Name ?? string.Empty,
                this.Description ?? string.Empty);
            var characterClass = this.ClassData ?? new CharacterClass();
            var appearance = this.AppearanceData ?? new CharacterAppearance();
            var stats = this.StatsData ?? new CharacterStats();
            var state = this.StateData ?? new CharacterState();
            var progression = this.ProgressionData ?? new CharacterProgression();
            return new Character(identity, characterClass, appearance, stats, state, progression);
        }
    }
}

