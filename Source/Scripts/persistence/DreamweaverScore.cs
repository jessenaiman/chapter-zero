// <copyright file="DreamweaverScore.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.Persistence
{
    /// <summary>
    /// Represents a Dreamweaver affinity score.
    /// </summary>
    [Table("DreamweaverScores")]
    public class DreamweaverScore
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
        /// Gets or sets the game save this score belongs to.
        /// </summary>
        [ForeignKey(nameof(GameSaveId))]
        public GameSave? GameSave { get; set; }

        /// <summary>
        /// Gets or sets the Dreamweaver type.
        /// </summary>
        [Required]
        public DreamweaverType DreamweaverType { get; set; }

        /// <summary>
        /// Gets or sets the affinity score.
        /// </summary>
        [Required]
        public int Score { get; set; }
    }
}
