// <copyright file="NethackShadow.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Stages.Stage2.Beats;

/// <summary>
/// Beat: Chamber Exploration (Shadow variant).
/// Displays the shadow chamber where the Dreamweaver of Shadow configures set pieces.
/// </summary>
[GlobalClass]
public partial class NethackShadow : NethackExploration
{
    /// <inheritdoc/>
    public override void _Ready()
    {
        SetChamberOwner("shadow");
        base._Ready();
    }
}
