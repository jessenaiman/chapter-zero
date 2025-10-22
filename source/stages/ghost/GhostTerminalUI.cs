// <copyright file="GhostTerminalUI.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.UI.Terminal;

namespace OmegaSpiral.Source.Stages.Ghost;

/// <summary>
/// Stage 1 (Ghost Terminal) specific UI that extends TerminalUI with Stage 1 game logic integration.
/// Inherits all terminal aesthetic and presentation features from TerminalUI.
/// Stage 1 question scenes inherit from this to access the UI infrastructure.
/// </summary>
[GlobalClass]
public partial class GhostTerminalUI : TerminalUI
{
    // This class serves as the Stage 1-specific UI base.
    // All Stage 1 question scenes inherit from this.
    // Specific game logic methods are implemented directly in the question scene files,
    // keeping concerns properly separated (UI infrastructure vs. narrative content).
}
