// <copyright file="IDialogueParser.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts.Common.Dialogue;

/// <summary>
/// Interface for dialogue parsers that convert JSON data to dialogue structures.
/// Implementations handle parsing and validation of dialogue data from JSON.
/// </summary>
public interface IDialogueParser
{
    /// <summary>
    /// Parses JSON data into a dialogue data structure.
    /// </summary>
    /// <param name="jsonData">The JSON data dictionary to parse.</param>
    /// <returns>An IDialogueData instance.</returns>
    IDialogueData ParseDialogueData(Godot.Collections.Dictionary<string, Variant> jsonData);

    /// <summary>
    /// Validates the parsed dialogue data against the expected schema.
    /// </summary>
    /// <param name="dialogueData">The dialogue data to validate.</param>
    /// <returns><see langword="true"/> if valid, <see langword="false"/> otherwise.</returns>
    bool ValidateDialogueData(IDialogueData dialogueData);
}
