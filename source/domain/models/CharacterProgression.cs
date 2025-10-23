// <copyright file="CharacterProgression.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Domain.Models
{
    /// <summary>
    /// Represents the progression aspects of a character.
    /// Follows Single Responsibility Principle by handling only progression concerns.
    /// </summary>
    public class CharacterProgression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterProgression"/> class.
        /// </summary>
        /// <param name="currentExperience">The character's current experience points.</param>
        /// <param name="gold">The character's gold or currency amount.</param>
        /// <param name="isAvailable">Whether this character is available for selection.</param>
        /// <param name="unlockCondition">The unlock condition for this character.</param>
        public CharacterProgression(int currentExperience = 0, int gold = 0, bool isAvailable = true, string unlockCondition = "")
        {
            this.CurrentExperience = currentExperience;
            this.Gold = gold;
            this.IsAvailable = isAvailable;
            this.UnlockCondition = unlockCondition;
        }

        /// <summary>
        /// Gets or sets the character's current experience points.
        /// </summary>
        public int CurrentExperience { get; set; }

        /// <summary>
        /// Gets or sets the character's gold or currency amount.
        /// </summary>
        public int Gold { get; set; }

        /// <summary>
        /// Gets or sets whether this character is available for selection.
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Gets or sets the unlock condition for this character.
        /// </summary>
        public string UnlockCondition { get; set; }

        /// <summary>
        /// Adds experience points to the character.
        /// </summary>
        /// <param name="exp">The experience points to add.</param>
        public void AddExperience(int exp)
        {
            this.CurrentExperience += exp;
        }

        /// <summary>
        /// Resets the progression to default values.
        /// </summary>
        public void ResetToDefault()
        {
            this.CurrentExperience = 0;
            this.Gold = 0;
            // Keep IsAvailable and UnlockCondition as-is
        }

        /// <summary>
        /// Creates a copy of this progression.
        /// </summary>
        /// <returns>A new instance of <see cref="CharacterProgression"/> with the same values.</returns>
        public CharacterProgression Clone()
        {
            return new CharacterProgression(this.CurrentExperience, this.Gold, this.IsAvailable, this.UnlockCondition);
        }
    }
}
