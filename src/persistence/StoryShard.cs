// <copyright file="StoryShard.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmegaSpiral.Source.Scripts.persistence
{
    /// <summary>
    /// Represents a collected story shard.
    /// </summary>
    [Table("StoryShards")]
    public class StoryShard
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
        /// Gets or sets the game save this shard belongs to.
        /// </summary>
        [ForeignKey(nameof(GameSaveId))]
        public GameSave? GameSave { get; set; }

        /// <summary>
        /// Gets or sets the shard content.
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;
    }
}
