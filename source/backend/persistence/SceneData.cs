// <copyright file="SceneData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmegaSpiral.Source.Backend.Persistence;
    /// <summary>
    /// Represents scene-specific data stored as key-value pairs.
    /// </summary>
    [Table("SceneData")]
    public class SceneData
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
        /// Gets or sets the game save this data belongs to.
        /// </summary>
        [ForeignKey(nameof(GameSaveId))]
        public GameSave? GameSave { get; set; }

        /// <summary>
        /// Gets or sets the data key.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the data value as JSON string.
        /// </summary>
        [Required]
        public string Value { get; set; } = string.Empty;
    }
