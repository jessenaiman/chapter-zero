// <copyright file="OmegaUiButton.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// Base button class with Omega visual theme applied via theme resource.
/// All Omega UI buttons should inherit from this to maintain consistent styling.
/// </summary>
/// <remarks>
/// Styling is now controlled by omega_ui.theme.tres:
/// - Colors (warm amber, pure white on hover/focus) are in the theme
/// - Font (Orbitron) and size (20) are in the theme
/// - Button styles (normal, hover, focus) are in the theme
///
/// This class adds Omega-specific behavior:
/// - Modulate effects for hover/focus (visual feedback beyond color changes)
/// - Keyboard/gamepad navigation support
/// - Default size flags for layout
///
/// Subclasses can override _Ready() to add additional behavior but should call base._Ready().
/// </remarks>
[GlobalClass]
public partial class OmegaUiButton : Button
{
    /// <summary>
    /// Called when the node enters the scene tree.
    /// Sets up behavior unique to Omega buttons.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        // Omega-specific button behavior (not styling!)
        FocusMode = FocusModeEnum.All; // Support keyboard/gamepad navigation
        SizeFlagsHorizontal = SizeFlags.ExpandFill; // Fill horizontal space by default

        // Add subtle visual feedback on interaction (beyond theme colors)
        MouseEntered += OnButtonHover;
        FocusEntered += OnButtonFocus;
        MouseExited += OnButtonUnhover;
        FocusExited += OnButtonUnfocus;
    }

    private void OnButtonHover()
    {
        Modulate = new Color(1.2f, 1.2f, 1.2f, 1.0f); // Slight brightness boost
    }

    private void OnButtonFocus()
    {
        Modulate = new Color(1.3f, 1.3f, 1.0f, 1.0f); // Warm glow on focus
    }

    private void OnButtonUnhover()
    {
        Modulate = Colors.White;
    }

    private void OnButtonUnfocus()
    {
        Modulate = Colors.White;
    }
}
