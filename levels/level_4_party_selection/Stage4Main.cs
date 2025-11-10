// <copyright file="Stage4Main.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Stages.Stage4;

/// <summary>
/// Main controller for Stage 4 - Echo Vault (Party Selection).
/// Handles character mirror selection and party setup.
/// </summary>
[GlobalClass]
public partial class Stage4Main : Node2D
{
    /// <summary>
    /// Emitted when a character is selected in the mirror selection.
    /// </summary>
    /// <param name="characterId">The selected character ID.</param>
    [Signal]
    public delegate void CharacterSelectedEventHandler(string characterId);

    /// <inheritdoc/>
    public override void _Ready()
    {
        GD.Print("[Stage4Main] Initialized");
    }
}
