// <copyright file="DreamweaverScore.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

#nullable enable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OmegaSpiral.Source.Backend.Common;

namespace OmegaSpiral.Source.Backend.Persistence;
    /// <summary>
    /// Represents a Dreamweaver affinity score for database persistence.
    /// This is the Entity Framework entity used for saving/loading scores to SQLite.
    /// </summary>
    /// <remarks>
    /// DO NOT confuse with the runtime scoring system in GameState.
    /// This class is solely for database persistence via EF Core.
    /// </remarks>
    [Table("DreamweaverScores")]
    public class DreamweaverScoreEntity
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
