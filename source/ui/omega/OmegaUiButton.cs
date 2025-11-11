// <copyright file="OmegaUiButton.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// Base button class with Omega visual theme applied.
/// All Omega UI buttons should inherit from this to maintain consistent styling.
/// </summary>
/// <remarks>
/// Automatically applies Omega color scheme:
/// - Normal: Warm Amber
/// - Hover/Focus/Pressed: Pure White
/// Subclasses can override _Ready() to add additional behavior but should call base._Ready().
/// </remarks>
[GlobalClass]
public partial class OmegaUiButton : Button
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OmegaUiButton"/> class.
    /// Applies Omega theme colors automatically.
    /// </summary>
    public OmegaUiButton()
    {
        ApplyOmegaTheme();
    }

    /// <summary>
    /// Called when the node enters the scene tree.
    /// Sets up default button configuration and applies Orbitron font.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        // Default configuration for Omega buttons
        FocusMode = FocusModeEnum.All; // Support keyboard/gamepad navigation
        SizeFlagsHorizontal = SizeFlags.ExpandFill; // Fill horizontal space by default

        // Apply Orbitron Bold font for buttons
        var orbitronFont = GD.Load<Font>("res://source/assets/gui/font/orbitron_buttons.tres");
        if (orbitronFont != null)
        {
            AddThemeFontOverride("font", orbitronFont);
            AddThemeFontSizeOverride("font_size", 20);
        }
    }    /// <summary>
         /// Applies the Omega Spiral color theme to this button.
         /// Called automatically in constructor, but can be called again to reapply if needed.
         /// </summary>
    protected void ApplyOmegaTheme()
    {
        // Font colors
        AddThemeColorOverride("font_color", OmegaSpiralColors.WarmAmber);
        AddThemeColorOverride("font_hover_color", OmegaSpiralColors.PureWhite);
        AddThemeColorOverride("font_pressed_color", OmegaSpiralColors.PureWhite);
        AddThemeColorOverride("font_focus_color", OmegaSpiralColors.PureWhite);

        // Background stays transparent/dark
        AddThemeColorOverride("font_outline_color", OmegaSpiralColors.WarmAmber);

        // Border constants for retro terminal look
        AddThemeConstantOverride("outline_size", 0);

        // Add hover glow effect
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
