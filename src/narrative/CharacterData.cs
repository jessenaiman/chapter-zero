// <copyright file="CharacterData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Narrative
{
    /// <summary>
    /// Represents a character archetype available for selection in Never Go Alone stage.
    /// Maps to Character class for party persistence.
    /// </summary>
    public class CharacterData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterData"/> class.
        /// </summary>
        public CharacterData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterData"/> class.
        /// </summary>
        /// <param name="id">The character identifier.</param>
        /// <param name="name">The character display name.</param>
        /// <param name="description">The character description.</param>
        /// <param name="dwReflection">The associated Dreamweaver reflection.</param>
        public CharacterData(string id, string name, string description, string dwReflection)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.DreamweaverReflection = dwReflection;
        }

        /// <summary>
        /// Gets or sets the character identifier (e.g., "fighter", "wizard").
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the character display name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the character description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the associated Dreamweaver reflection ("light", "mischief", "wrath").
        /// </summary>
        public string DreamweaverReflection { get; set; } = string.Empty;

        /// <summary>
        /// Creates a CharacterData instance from a Godot dictionary.
        /// </summary>
        /// <param name="dict">The dictionary containing character data from JSON.</param>
        /// <returns>A new <see cref="CharacterData"/> instance.</returns>
        public static CharacterData FromDictionary(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var characterData = new CharacterData();

            if (dict.TryGetValue("id", out var idVar))
            {
                characterData.Id = idVar.AsString();
            }

            if (dict.TryGetValue("name", out var nameVar))
            {
                characterData.Name = nameVar.AsString();
            }

            if (dict.TryGetValue("description", out var descVar))
            {
                characterData.Description = descVar.AsString();
            }

            if (dict.TryGetValue("dw_reflection", out var dwVar))
            {
                characterData.DreamweaverReflection = dwVar.AsString();
            }

            return characterData;
        }

        /// <summary>
        /// Converts this CharacterData to a Character instance for party persistence.
        /// </summary>
        /// <returns>A new <see cref="Character"/> instance.</returns>
        public Character ToCharacter()
        {
            var characterClass = this.Id switch
            {
                "fighter" => CharacterClass.Fighter,
                "wizard" => CharacterClass.Mage,
                "thief" => CharacterClass.Thief,
                "scribe" => CharacterClass.Priest,
                _ => CharacterClass.Fighter,
            };

            return new Character(this.Name, characterClass, CharacterRace.Human);
        }

        /// <summary>
        /// Gets a string representation of the character data.
        /// </summary>
        /// <returns>A formatted string with character details.</returns>
        public override string ToString()
        {
            return $"{this.Name} ({this.Id}): {this.Description}";
        }
    }
}
