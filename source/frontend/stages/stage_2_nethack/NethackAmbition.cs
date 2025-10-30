// <copyright file="NethackAmbition.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Stages.Stage2.Beats;

/// <summary>
/// Beat: Chamber Exploration (Ambition variant).
/// Displays the ambition chamber where the Dreamweaver of Ambition configures set pieces.
/// </summary>
[GlobalClass]
public partial class NethackAmbition : NethackExploration
{
    /// <inheritdoc/>
    public override void _Ready()
    {
        SetChamberOwner("ambition");
        base._Ready();
    }
}
