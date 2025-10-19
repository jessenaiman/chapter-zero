// <copyright file="CharacterIdentity.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Domain.Models
{
    /// <summary>
    /// Represents the immutable identity aspects of a character.
    /// Follows Single Responsibility Principle by handling only identity concerns.
    /// </summary>
    public class CharacterIdentity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterIdentity"/> class.
        /// </summary>
        /// <param name="id">The unique identifier for the character.</param>
        /// <param name="name">The character's name.</param>
        /// <param name="description">The character's description.</param>
        /// <param name="category">The category of this character.</param>
        /// <param name="iconPath">The icon resource path for this character.</param>
        public CharacterIdentity(string id, string name, string description, string category = "Player", string iconPath = "")
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Category = category;
            this.IconPath = iconPath;
        }

        /// <summary>
        /// Gets the unique identifier for the character.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the character's name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the character's description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the category of this character (e.g., "Player", "NPC", "Boss").
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Gets the icon resource path for this character.
        /// </summary>
        public string IconPath { get; }

        /// <summary>
        /// Gets the display name with category prefix if applicable.
        /// </summary>
        /// <returns>The formatted display name.</returns>
        public string GetDisplayName()
        {
            if (string.IsNullOrEmpty(this.Category) || this.Category == "Player")
            {
                return this.Name;
            }

            return $"{this.Category}: {this.Name}";
        }

        /// <summary>
        /// Creates a copy of this identity.
        /// </summary>
        /// <returns>A new instance of <see cref="CharacterIdentity"/> with the same values.</returns>
        public CharacterIdentity Clone()
        {
            return new CharacterIdentity(this.Id, this.Name, this.Description, this.Category, this.IconPath);
        }
    }
}
