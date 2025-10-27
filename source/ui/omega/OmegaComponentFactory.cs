// <copyright file="OmegaComponentFactory.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// Factory for creating Omega-themed UI components.
/// Ensures consistent styling and configuration across the game.
/// Single Responsibility: Component instantiation with Omega theme applied.
/// </summary>
public static class OmegaComponentFactory
{
    /// <summary>
    /// Creates an animated spiral border frame with Omega design system colors.
    /// Border is self-contained and applies theme automatically.
    /// </summary>
    /// <returns>A configured OmegaBorderFrame ready to add to scene tree.</returns>
    public static OmegaBorderFrame CreateBorderFrame()
    {
        return new OmegaBorderFrame(); // Already applies OmegaSpiralColors internally
    }

    /// <summary>
    /// Creates a text renderer for Omega UI with themed styling.
    /// </summary>
    /// <param name="textDisplay">The RichTextLabel to render text on.</param>
    /// <returns>A configured OmegaTextRenderer with Omega theme colors.</returns>
    public static OmegaTextRenderer CreateTextRenderer(RichTextLabel textDisplay)
    {
        var renderer = new OmegaTextRenderer(textDisplay);
        renderer.SetTextColor(OmegaSpiralColors.WarmAmber);
        return renderer;
    }

    /// <summary>
    /// Creates a shader controller for visual effects.
    /// </summary>
    /// <param name="display">The ColorRect to apply shaders to.</param>
    /// <returns>A configured OmegaShaderController.</returns>
    public static OmegaShaderController CreateShaderController(ColorRect display)
    {
        return new OmegaShaderController(display);
    }

    /// <summary>
    /// Creates a choice presenter for player decision UI.
    /// </summary>
    /// <param name="choiceContainer">The VBoxContainer to hold choice buttons.</param>
    /// <returns>A configured OmegaChoicePresenter.</returns>
    public static OmegaChoicePresenter CreateChoicePresenter(VBoxContainer choiceContainer)
    {
        return new OmegaChoicePresenter(choiceContainer);
    }

    /// <summary>
    /// Creates a button with Omega theme styling.
    /// </summary>
    /// <param name="text">Button text.</param>
    /// <returns>A themed Button with Omega colors.</returns>
    public static Button CreateStyledButton(string text)
    {
        var button = new Button
        {
            Text = text
        };

        // Apply Omega theme colors
        button.AddThemeColorOverride("font_color", OmegaSpiralColors.WarmAmber);
        button.AddThemeColorOverride("font_hover_color", OmegaSpiralColors.PureWhite);
        button.AddThemeColorOverride("font_pressed_color", OmegaSpiralColors.PureWhite);
        button.AddThemeColorOverride("font_focus_color", OmegaSpiralColors.PureWhite);

        return button;
    }
}
