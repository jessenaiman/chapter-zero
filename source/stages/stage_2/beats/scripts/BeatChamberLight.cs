// <copyright file="BeatChamberLight.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Stages.Stage2.Beats;

/// <summary>
/// Beat: Chamber Exploration (Light variant).
/// Displays the light chamber where the Dreamweaver of Light configures set pieces.
/// </summary>
[GlobalClass]
public partial class BeatChamberLight : BeatChamberExploration
{
    /// <inheritdoc/>
    public override void _Ready()
    {
        SetChamberOwner("light");
        base._Ready();
    }
}
