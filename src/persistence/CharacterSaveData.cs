// <copyright file="CharacterSaveData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

#nullable enable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.persistence;

/// <summary>
/// Represents a character in the party for saving.
/// </summary>
[Table("CharacterData")]
public class CharacterSaveData
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the party data.
    /// </summary>
    public int PartySaveDataId { get; set; }

    /// <summary>
    /// Gets or sets the party data this character belongs to.
    /// </summary>
    [ForeignKey(nameof(PartySaveDataId))]
    public PartySaveData? PartySaveData { get; set; }

    /// <summary>
    /// Gets or sets the character's name.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the character's class.
    /// </summary>
    [Required]
    public CharacterClass Class { get; set; }

    /// <summary>
    /// Gets or sets the character's race.
    /// </summary>
    [Required]
    public CharacterRace Race { get; set; }

    /// <summary>
    /// Gets or sets the character's level.
    /// </summary>
    public int Level { get; set; } = 1;

    /// <summary>
    /// Gets or sets the character's experience points.
    /// </summary>
    public int Experience { get; set; }

    /// <summary>
    /// Gets or sets the character's stats as JSON string.
    /// </summary>
    [Required]
    public string StatsJson { get; set; } = "{}";
}
