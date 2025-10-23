// <copyright file="NarratorMessage.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmegaSpiral.Source.Scripts.persistence
{
    /// <summary>
    /// Represents a narrator message in the queue.
    /// </summary>
    [Table("NarratorMessages")]
    public class NarratorMessage
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
        /// Gets or sets the game save this message belongs to.
        /// </summary>
        [ForeignKey(nameof(GameSaveId))]
        public GameSave? GameSave { get; set; }

        /// <summary>
        /// Gets or sets the message content.
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the order of the message in the queue.
        /// </summary>
        public int Order { get; set; }
    }
}
