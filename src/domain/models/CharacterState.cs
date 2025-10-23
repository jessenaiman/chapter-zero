// <copyright file="CharacterState.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Domain.Models
{
    /// <summary>
    /// Represents the mutable state aspects of a character.
    /// Follows Single Responsibility Principle by handling only state management.
    /// </summary>
    public class CharacterState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterState"/> class.
        /// </summary>
        /// <param name="currentLocation">The character's current location.</param>
        /// <param name="status">The character's current status.</param>
        /// <param name="isInCombat">Whether the character is currently in combat.</param>
        /// <param name="currentHealth">The character's current health points.</param>
        /// <param name="currentMana">The character's current mana points.</param>
        public CharacterState(string currentLocation = "", string status = "Alive", bool isInCombat = false, int currentHealth = 100, int currentMana = 50)
        {
            this.CurrentLocation = currentLocation;
            this.Status = status;
            this.IsInCombat = isInCombat;
            this.CurrentHealth = currentHealth;
            this.CurrentMana = currentMana;
        }

        /// <summary>
        /// Gets or sets the character's current location in the game world.
        /// </summary>
        public string CurrentLocation { get; set; }

        /// <summary>
        /// Gets or sets the character's current status (e.g., "Alive", "Dead", "Stunned").
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets whether the character is currently in combat.
        /// </summary>
        public bool IsInCombat { get; set; }

        /// <summary>
        /// Gets or sets the character's current health points.
        /// </summary>
        public int CurrentHealth { get; set; }

        /// <summary>
        /// Gets or sets the character's current mana points.
        /// </summary>
        public int CurrentMana { get; set; }

        /// <summary>
        /// Checks if the character is alive.
        /// </summary>
        /// <returns><see langword="true"/> if the character is alive, <see langword="false"/> otherwise.</returns>
        public bool IsAlive()
        {
            return this.Status != "Dead" && this.CurrentHealth > 0;
        }

        /// <summary>
        /// Resets the character state to default values while preserving location.
        /// </summary>
        public void ResetToDefault()
        {
            this.Status = "Alive";
            this.IsInCombat = false;
            // Keep CurrentLocation as-is
        }

        /// <summary>
        /// Creates a copy of this state.
        /// </summary>
        /// <returns>A new instance of <see cref="CharacterState"/> with the same values.</returns>
        public CharacterState Clone()
        {
            return new CharacterState(this.CurrentLocation, this.Status, this.IsInCombat, this.CurrentHealth, this.CurrentMana);
        }
    }
}
