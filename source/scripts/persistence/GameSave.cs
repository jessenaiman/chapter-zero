// <copyright file="GameSave.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.persistence
{
    /// <summary>
    /// Represents a game save file stored in the database.
    /// </summary>
    [Table("GameSaves")]
    public class GameSave
    {
        /// <summary>
        /// Gets or sets the unique identifier for the save.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the save slot name.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string SaveSlot { get; set; } = "default";

        /// <summary>
        /// Gets or sets the save version.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string SaveVersion { get; set; } = "1.0.0";

        /// <summary>
        /// Gets or sets the timestamp when the save was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the timestamp when the save was last modified.
        /// </summary>
        public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the current scene index.
        /// </summary>
        public int CurrentScene { get; set; } = 1;

        /// <summary>
        /// Gets or sets the player's chosen Dreamweaver thread.
        /// </summary>
        public DreamweaverThread DreamweaverThread { get; set; } = DreamweaverThread.Hero;

        /// <summary>
        /// Gets or sets the player's name.
        /// </summary>
        [MaxLength(100)]
        public string? PlayerName { get; set; }

        /// <summary>
        /// Gets or sets the player's secret response.
        /// </summary>
        [MaxLength(500)]
        public string? PlayerSecret { get; set; }

        /// <summary>
        /// Gets or sets the selected Dreamweaver type.
        /// </summary>
        public DreamweaverType? SelectedDreamweaver { get; set; }

        /// <summary>
        /// Gets or sets the collection of collected story shards.
        /// </summary>
        public List<StoryShard> Shards { get; set; } = new();

        /// <summary>
        /// Gets or sets the scene-specific data.
        /// </summary>
        public List<SceneData> SceneData { get; set; } = new();

        /// <summary>
        /// Gets or sets the Dreamweaver affinity scores for persistence.
        /// </summary>
        public List<DreamweaverScoreEntity> DreamweaverScores { get; set; } = new();

        /// <summary>
        /// Gets or sets the player's party data.
        /// </summary>
        public PartySaveData? PartyData { get; set; }

        /// <summary>
        /// Gets or sets the narrator message queue.
        /// </summary>
        public List<NarratorMessage> NarratorQueue { get; set; } = new();
    }
}
