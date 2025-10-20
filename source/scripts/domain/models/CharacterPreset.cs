// <copyright file="CharacterPreset.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Domain.Models
{
    /// <summary>
    /// Represents a character preset that combines appearance, stats, and class information.
    /// </summary>
    public class CharacterPreset
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterPreset"/> class.
        /// </summary>
        public CharacterPreset()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterPreset"/> class with specified parameters.
        /// </summary>
        /// <param name="id">The unique identifier for the character preset.</param>
        /// <param name="name">The display name of the character preset.</param>
        /// <param name="description">The description of the character preset.</param>
        /// <param name="characterClass">The character class for this preset.</param>
        /// <param name="appearance">The character appearance for this preset.</param>
        /// <param name="stats">The character stats for this preset.</param>
        /// <param name="isAvailable">Whether this preset is available for selection.</param>
        /// <param name="unlockCondition">The unlock condition for this preset.</param>
        /// <param name="iconPath">The icon resource path for this preset.</param>
        /// <param name="category">The category of this preset.</param>
        public CharacterPreset(
            string id,
            string name,
            string description,
            CharacterClass characterClass,
            CharacterAppearance appearance,
            CharacterStats stats,
            bool isAvailable = true,
            string unlockCondition = "",
            string iconPath = "",
            string category = "Default")
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.CharacterClass = characterClass;
            this.Appearance = appearance;
            this.Stats = stats;
            this.IsAvailable = isAvailable;
            this.UnlockCondition = unlockCondition;
            this.IconPath = iconPath;
            this.Category = category;
        }

        /// <summary>
        /// Gets or sets the unique identifier for the character preset.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the character preset.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the character preset.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the character class for this preset.
        /// </summary>
        public CharacterClass CharacterClass { get; set; } = new CharacterClass();

        /// <summary>
        /// Gets or sets the character appearance for this preset.
        /// </summary>
        public CharacterAppearance Appearance { get; set; } = new CharacterAppearance();

        /// <summary>
        /// Gets or sets the character stats for this preset.
        /// </summary>
        public CharacterStats Stats { get; set; } = new CharacterStats();

        /// <summary>
        /// Gets or sets a value indicating whether this preset is available for selection.
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Gets or sets the unlock condition for this preset.
        /// </summary>
        public string UnlockCondition { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the icon resource path for this preset.
        /// </summary>
        public string IconPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the category of this preset (e.g., "Default", "Premium", "Event").
        /// </summary>
        public string Category { get; set; } = "Default";

        /// <summary>
        /// Creates a copy of this character preset.
        /// </summary>
        /// <returns>A new instance of <see cref="CharacterPreset"/> with the same values.</returns>
        public CharacterPreset Clone()
        {
            return new CharacterPreset
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                CharacterClass = this.CharacterClass?.Clone() ?? new CharacterClass(),
                Appearance = this.Appearance?.Clone() ?? new CharacterAppearance(),
                Stats = this.Stats?.Clone() ?? new CharacterStats(),
                IsAvailable = this.IsAvailable,
                UnlockCondition = this.UnlockCondition,
                IconPath = this.IconPath,
                Category = this.Category,
            };
        }

        /// <summary>
        /// Creates a new character instance based on this preset.
        /// </summary>
        /// <returns>A new character instance with the preset's properties.</returns>
        public Character CreateCharacter()
        {
            return new Character
            {
                Id = System.Guid.NewGuid().ToString(),
                Name = this.Name,
                Description = this.Description,
                Class = this.CharacterClass?.Clone() ?? new CharacterClass(),
                Appearance = this.Appearance?.Clone() ?? new CharacterAppearance(),
                Stats = this.Stats?.Clone() ?? new CharacterStats(),
                IsAvailable = this.IsAvailable,
                UnlockCondition = this.UnlockCondition,
                IconPath = this.IconPath,
                Category = this.Category,
            };
        }

        /// <summary>
        /// Validates whether this preset is properly configured.
        /// </summary>
        /// <returns><see langword="true"/> if the preset is valid, <see langword="false"/> otherwise.</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(this.Id) &&
                   !string.IsNullOrEmpty(this.Name) &&
                   this.CharacterClass != null &&
                   this.Appearance != null &&
                   this.Stats != null;
        }

        /// <summary>
        /// Applies this preset's appearance to an existing character.
        /// </summary>
        /// <param name="character">The character to apply the appearance to.</param>
        public void ApplyAppearanceTo(Character character)
        {
            if (character != null && this.Appearance != null)
            {
                character.Appearance = this.Appearance.Clone();
            }
        }

        /// <summary>
        /// Applies this preset's stats to an existing character.
        /// </summary>
        /// <param name="character">The character to apply the stats to.</param>
        public void ApplyStatsTo(Character character)
        {
            if (character != null && this.Stats != null)
            {
                character.Stats = this.Stats.Clone();
            }
        }

        /// <summary>
        /// Applies this preset's class to an existing character.
        /// </summary>
        /// <param name="character">The character to apply the class to.</param>
        public void ApplyClassTo(Character character)
        {
            if (character != null && this.CharacterClass != null)
            {
                character.Class = this.CharacterClass.Clone();
            }
        }

        /// <summary>
        /// Gets the display name with category prefix if applicable.
        /// </summary>
        /// <returns>The formatted display name.</returns>
        public string GetDisplayName()
        {
            if (string.IsNullOrEmpty(this.Category) || this.Category == "Default")
            {
                return this.Name;
            }

            return $"{this.Category}: {this.Name}";
        }
    }
}
