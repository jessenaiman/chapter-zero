// <copyright file="PartySaveData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmegaSpiral.Source.Scripts.persistence
{
    /// <summary>
    /// Represents the player's party data for saving.
    /// </summary>
    [Table("PartyData")]
    public class PartySaveData
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the foreign key to the game save.
        /// </summary>
        public int GameSaveId { get; set; }

        /// <summary>
        /// Gets or sets the game save this party belongs to.
        /// </summary>
        [ForeignKey(nameof(GameSaveId))]
        public GameSave? GameSave { get; set; }

        /// <summary>
        /// Gets or sets the party's gold amount.
        /// </summary>
        public int Gold { get; set; }

        /// <summary>
        /// Gets or sets the party members.
        /// </summary>
        public Collection<CharacterSaveData> Members { get; set; } = new();

        /// <summary>
        /// Gets or sets the party inventory as JSON string.
        /// </summary>
        public string InventoryJson { get; set; } = "{}";
    }
}
