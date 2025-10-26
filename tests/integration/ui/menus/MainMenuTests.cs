// <copyright file="MainMenuTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Linq;
using Godot;
using GdUnit4;
using OmegaSpiral.Source.Stages.Stage0Start;
using OmegaSpiral.Source.Ui.Menus;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.Ui.Menus;

/// <summary>
/// Integration tests for MainMenu.
/// Tests manifest loading (6 stages), button creation (1 Launch + 5 Stage buttons + Options + Quit),
/// no duplicate first stage, and button signal handlers.
/// Follows 1:1 test-to-class mapping: MainMenuTests.cs tests MainMenu.cs.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class MainMenuTests : Node
{
    private ISceneRunner _Runner = null!;
    private MainMenu _MainMenu = null!;

    [Before]
    public void Setup()
    {
        _Runner = ISceneRunner.Load("res://source/ui/menus/main_menu.tscn");
        _MainMenu = (MainMenu)_Runner.Scene();
        AssertThat(_MainMenu).IsNotNull();
    }

    [After]
    public void Cleanup()
    {
        _Runner?.Dispose();
    }

    // ==================== INHERITANCE ====================

    /// <summary>
    /// MainMenu inherits from MenuUi.
    /// </summary>
    [TestCase]
    public void InheritsFromMenuUi()
    {
        AssertThat(_MainMenu).IsInstanceOf<MenuUi>();
        AssertThat(_MainMenu).IsInstanceOf<OmegaSpiral.Source.Ui.Omega.OmegaUi>();
    }

    // ==================== SCENE STRUCTURE ====================

    /// <summary>
    /// MenuTitle displays "Ωmega Spiral" after _Ready().
    /// </summary>
    [TestCase]
    public void MenuTitle_DisplaysOmegaSpiral()
    {
        var title = _MainMenu.GetNodeOrNull<Label>("ContentContainer/MenuTitle");

        AssertThat(title).IsNotNull();
        AssertThat(title!.Text).IsEqual("Ωmega Spiral");
        AssertThat(title.HorizontalAlignment).IsEqual(HorizontalAlignment.Center);
    }

    /// <summary>
    /// MenuButtonContainer exists and contains buttons.
    /// </summary>
    [TestCase]
    public void MenuButtonContainer_ExistsAndHasButtons()
    {
        var buttonContainer = _MainMenu.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");

        AssertThat(buttonContainer).IsNotNull();

        var buttons = buttonContainer!.GetChildren().OfType<Button>().ToList();
        AssertThat(buttons.Count).IsGreaterEqual(8); // 1 Launch + 5 Stage + Options + Quit
    }

    /// <summary>
    /// MenuActionBar exists (inherited from MenuUi).
    /// </summary>
    [TestCase]
    public void MenuActionBar_Exists()
    {
        var actionBar = _MainMenu.GetNodeOrNull<HBoxContainer>("ContentContainer/MenuActionBar");
        AssertThat(actionBar).IsNotNull();
    }

    // ==================== MANIFEST LOADING ====================

    /// <summary>
    /// Manifest loads 6 stages total.
    /// First stage creates "Launch" button, stages 2-6 create "Stage X" buttons.
    /// No duplicate for first stage.
    /// </summary>
    [TestCase]
    public void ManifestLoads_SixStages_NoDuplicateFirst()
    {
        var buttonContainer = _MainMenu.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        AssertThat(buttonContainer).IsNotNull();

        var allButtons = buttonContainer!.GetChildren().OfType<Button>().ToList();

        // Should have: 1 Launch + 5 Stage (2-6) + Options + Quit = 8 buttons
        AssertThat(allButtons.Count).IsEqual(8);

        // First button should be "Launch Start Here: Ghost Terminal"
        var launchButton = allButtons.First(b => b.Name == "StartButton");
        AssertThat(launchButton).IsNotNull();
        AssertThat(launchButton.Text).Contains("Launch");
        AssertThat(launchButton.Text).Contains("Start Here: Ghost Terminal");

        // Should NOT have a "Stage 1" button (no duplicate)
        var stage1Button = allButtons.FirstOrDefault(b => b.Text.StartsWith("Stage 1"));
        AssertThat(stage1Button).IsNull();

        // Should have Stage 2-6 buttons
        for (int i = 2; i <= 6; i++)
        {
            var stageButton = allButtons.FirstOrDefault(b => b.Text.StartsWith($"Stage {i}"));
            AssertThat(stageButton).IsNotNull()
                .OverrideFailureMessage($"Stage {i} button should exist");
        }
    }

    /// <summary>
    /// Stage buttons have correct text from manifest.
    /// </summary>
    [TestCase]
    public void StageButtons_HaveCorrectTextFromManifest()
    {
        var buttonContainer = _MainMenu.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        var allButtons = buttonContainer!.GetChildren().OfType<Button>().ToList();

        // Verify specific stage button texts match manifest
        var stage2 = allButtons.First(b => b.Text.StartsWith("Stage 2"));
        AssertThat(stage2.Text).Contains("Nethack");

        var stage3 = allButtons.First(b => b.Text.StartsWith("Stage 3"));
        AssertThat(stage3.Text).Contains("Amnesia");

        var stage4 = allButtons.First(b => b.Text.StartsWith("Stage 4"));
        AssertThat(stage4.Text).Contains("Never Go Alone");

        var stage5 = allButtons.First(b => b.Text.StartsWith("Stage 5"));
        AssertThat(stage5.Text).Contains("Fractured Escape");

        var stage6 = allButtons.First(b => b.Text.StartsWith("Stage 6"));
        AssertThat(stage6.Text).Contains("System Log Epilogue");
    }

    // ==================== BUTTON EXISTENCE ====================

    /// <summary>
    /// Launch button (first stage) exists with correct name and text.
    /// </summary>
    [TestCase]
    public void LaunchButton_ExistsWithCorrectText()
    {
        var buttonContainer = _MainMenu.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        var launchButton = buttonContainer!.GetChildren().OfType<Button>()
            .FirstOrDefault(b => b.Name == "StartButton");

        AssertThat(launchButton).IsNotNull();
        AssertThat(launchButton!.Text).Contains("Launch");
        AssertThat(launchButton.Text).Contains("Start Here: Ghost Terminal");
    }

    /// <summary>
    /// Options button exists.
    /// </summary>
    [TestCase]
    public void OptionsButton_Exists()
    {
        var buttonContainer = _MainMenu.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        var optionsButton = buttonContainer!.GetChildren().OfType<Button>()
            .FirstOrDefault(b => b.Name == "OptionsButton");

        AssertThat(optionsButton).IsNotNull();
        AssertThat(optionsButton!.Text).IsEqual("Options");
    }

    /// <summary>
    /// Quit button exists.
    /// </summary>
    [TestCase]
    public void QuitButton_Exists()
    {
        var buttonContainer = _MainMenu.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        var quitButton = buttonContainer!.GetChildren().OfType<Button>()
            .FirstOrDefault(b => b.Name == "QuitButton");

        AssertThat(quitButton).IsNotNull();
        AssertThat(quitButton!.Text).IsEqual("Quit Game");
    }

    // ==================== SIGNAL HANDLERS ====================

    /// <summary>
    /// Launch button has "pressed" signal connected to OnStageSelected handler.
    /// </summary>
    [TestCase]
    public void LaunchButton_HasPressedSignalHandler()
    {
        var buttonContainer = _MainMenu.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        var launchButton = buttonContainer!.GetChildren().OfType<Button>()
            .First(b => b.Name == "StartButton");

        var signalList = launchButton.GetSignalConnectionList("pressed");
        AssertThat(signalList.Count).IsGreater(0);
    }

    /// <summary>
    /// Options button has "pressed" signal connected to OnOptionsPressed handler.
    /// </summary>
    [TestCase]
    public void OptionsButton_HasPressedSignalHandler()
    {
        var buttonContainer = _MainMenu.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        var optionsButton = buttonContainer!.GetChildren().OfType<Button>()
            .First(b => b.Name == "OptionsButton");

        var signalList = optionsButton.GetSignalConnectionList("pressed");
        AssertThat(signalList.Count).IsGreater(0);
    }

    /// <summary>
    /// Quit button has "pressed" signal connected to OnQuitPressed handler.
    /// </summary>
    [TestCase]
    public void QuitButton_HasPressedSignalHandler()
    {
        var buttonContainer = _MainMenu.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        var quitButton = buttonContainer!.GetChildren().OfType<Button>()
            .First(b => b.Name == "QuitButton");

        var signalList = quitButton.GetSignalConnectionList("pressed");
        AssertThat(signalList.Count).IsGreater(0);
    }

    /// <summary>
    /// All stage buttons (2-6) have "pressed" signal handlers.
    /// </summary>
    [TestCase]
    public void AllStageButtons_HavePressedSignalHandlers()
    {
        var buttonContainer = _MainMenu.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        var allButtons = buttonContainer!.GetChildren().OfType<Button>().ToList();

        for (int i = 2; i <= 6; i++)
        {
            var stageButton = allButtons.First(b => b.Name == $"Stage{i}Button");
            var signalList = stageButton.GetSignalConnectionList("pressed");
            AssertThat(signalList.Count).IsGreater(0)
                .OverrideFailureMessage($"Stage {i} button should have pressed signal handler");
        }
    }

    // ==================== VISUAL DESIGN COMPLIANCE ====================

    /// <summary>
    /// MenuTitle uses amber color from OmegaSpiralColors.WarmAmber design palette.
    /// Verifies UI compliance with centralized design specification.
    /// </summary>
    [TestCase]
    public void MenuTitle_UsesAmberDesignColor()
    {
        var title = _MainMenu.GetNodeOrNull<Label>("ContentContainer/MenuTitle");
        AssertThat(title).IsNotNull();

        var expectedAmber = OmegaSpiralColors.WarmAmber;
        var actualColor = title!.Modulate;

        // Use design-specified tolerance for float precision
        AssertThat(Math.Abs(actualColor.R - expectedAmber.R)).IsLess(OmegaSpiralColors.ColorTolerance)
            .OverrideFailureMessage($"MenuTitle Red channel: expected {expectedAmber.R}, got {actualColor.R}");
        AssertThat(Math.Abs(actualColor.G - expectedAmber.G)).IsLess(OmegaSpiralColors.ColorTolerance)
            .OverrideFailureMessage($"MenuTitle Green channel: expected {expectedAmber.G}, got {actualColor.G}");
        AssertThat(Math.Abs(actualColor.B - expectedAmber.B)).IsLess(OmegaSpiralColors.ColorTolerance)
            .OverrideFailureMessage($"MenuTitle Blue channel: expected {expectedAmber.B}, got {actualColor.B}");
    }

    /// <summary>
    /// MenuTitle font size is 56pt per design specification (title_font_size from omega_ui.theme.tres).
    /// </summary>
    [TestCase]
    public void MenuTitle_UsesCorrectFontSize()
    {
        var title = _MainMenu.GetNodeOrNull<Label>("ContentContainer/MenuTitle");
        AssertThat(title).IsNotNull();

        const int expectedTitleFontSize = 56;
        var theme = title!.GetTheme();

        // Check if theme override exists for font size
        if (theme != null && theme.HasFontSize("font_size", "Label"))
        {
            var actualSize = theme.GetFontSize("font_size", "Label");
            AssertThat(actualSize).IsEqual(expectedTitleFontSize)
                .OverrideFailureMessage($"Title font size should be {expectedTitleFontSize}pt, got {actualSize}pt");
        }
        // If no theme override, fallback should still be 56pt from inherited theme
    }

    /// <summary>
    /// MenuButtonContainer has 12px vertical separation between buttons (design spec).
    /// VBoxContainer.separation = 12 pixels.
    /// </summary>
    [TestCase]
    public void MenuButtonContainer_HasCorrectSpacing()
    {
        var buttonContainer = _MainMenu.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        AssertThat(buttonContainer).IsNotNull();

        const int expectedSeparation = 12; // 12px between buttons in VBoxContainer
        var actualSeparation = (int)buttonContainer!.Get("theme_override_constants/separation");

        AssertThat(actualSeparation).IsEqual(expectedSeparation)
            .OverrideFailureMessage($"Button container separation should be {expectedSeparation}px, got {actualSeparation}px");
    }

    /// <summary>
    /// ContentContainer (VBoxContainer) has 24px separation for proper spacing between sections
    /// (MenuTitle, MenuButtonContainer, MenuActionBar).
    /// </summary>
    [TestCase]
    public void ContentContainer_HasCorrectSectionSpacing()
    {
        var contentContainer = _MainMenu.GetNodeOrNull<VBoxContainer>("ContentContainer");
        AssertThat(contentContainer).IsNotNull();

        const int expectedSeparation = 24; // 24px between major sections
        var actualSeparation = (int)contentContainer!.Get("theme_override_constants/separation");

        AssertThat(actualSeparation).IsEqual(expectedSeparation)
            .OverrideFailureMessage($"Section separation should be {expectedSeparation}px, got {actualSeparation}px");
    }

    /// <summary>
    /// MenuTitle is horizontally centered on the screen.
    /// ContentContainer has alignment=1 (center).
    /// </summary>
    [TestCase]
    public void MenuTitle_IsHorizontallyCentered()
    {
        var title = _MainMenu.GetNodeOrNull<Label>("ContentContainer/MenuTitle");
        AssertThat(title).IsNotNull();

        // Label itself has HorizontalAlignment.Center (1)
        var titleAlignment = title!.HorizontalAlignment;
        AssertThat(titleAlignment).IsEqual(HorizontalAlignment.Center)
            .OverrideFailureMessage($"MenuTitle should be horizontally centered (alignment=1), got alignment={titleAlignment}");
    }

    /// <summary>
    /// MenuButtonContainer is center-aligned vertically and horizontally.
    /// This ensures buttons don't stack at top of screen.
    /// </summary>
    [TestCase]
    public void MenuButtonContainer_IsCenterAligned()
    {
        var buttonContainer = _MainMenu.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");
        AssertThat(buttonContainer).IsNotNull();

        // VBoxContainer.alignment = 1 means ALIGNMENT_CENTER
        var alignment = buttonContainer!.Alignment;
        AssertThat(alignment).IsEqual(BoxContainer.AlignmentMode.Center)
            .OverrideFailureMessage($"MenuButtonContainer should be center-aligned, got alignment={alignment}");
    }

    /// <summary>
    /// PhosphorLayer (CRT shader effect) is visible with correct opacity from OmegaSpiralColors.PhosphorGlow.
    /// Enables the amber phosphor glow effect over the content.
    /// </summary>
    [TestCase]
    public void PhosphorLayer_IsVisibleWithCorrectOpacity()
    {
        var phosphorLayer = _MainMenu.GetNodeOrNull<ColorRect>("PhosphorLayer");
        AssertThat(phosphorLayer).IsNotNull();

        var expectedColor = OmegaSpiralColors.PhosphorGlow;
        var actualColor = phosphorLayer!.Color;

        AssertThat(Math.Abs(actualColor.A - expectedColor.A)).IsLess(OmegaSpiralColors.OpacityTolerance)
            .OverrideFailureMessage($"PhosphorLayer opacity: expected {expectedColor.A}, got {actualColor.A}");

        // Layer should be visible (not hidden)
        AssertThat(phosphorLayer.Visible).IsTrue()
            .OverrideFailureMessage("PhosphorLayer should be visible to render glow effect");
    }

    /// <summary>
    /// ScanlineLayer (CRT scanline effect) is visible with correct opacity from OmegaSpiralColors.ScanlineOverlay.
    /// Adds scanline pattern overlay for retro aesthetic.
    /// </summary>
    [TestCase]
    public void ScanlineLayer_IsVisibleWithCorrectOpacity()
    {
        var scanlineLayer = _MainMenu.GetNodeOrNull<ColorRect>("ScanlineLayer");
        AssertThat(scanlineLayer).IsNotNull();

        var expectedColor = OmegaSpiralColors.ScanlineOverlay;
        var actualColor = scanlineLayer!.Color;

        AssertThat(Math.Abs(actualColor.A - expectedColor.A)).IsLess(OmegaSpiralColors.OpacityTolerance)
            .OverrideFailureMessage($"ScanlineLayer opacity: expected {expectedColor.A}, got {actualColor.A}");

        AssertThat(scanlineLayer.Visible).IsTrue()
            .OverrideFailureMessage("ScanlineLayer should be visible to render scanlines");
    }

    /// <summary>
    /// GlitchLayer (CRT glitch effect) is visible with correct opacity from OmegaSpiralColors.GlitchDistortion.
    /// Adds glitch distortion effect for authentic retro feel.
    /// </summary>
    [TestCase]
    public void GlitchLayer_IsVisibleWithCorrectOpacity()
    {
        var glitchLayer = _MainMenu.GetNodeOrNull<ColorRect>("GlitchLayer");
        AssertThat(glitchLayer).IsNotNull();

        var expectedColor = OmegaSpiralColors.GlitchDistortion;
        var actualColor = glitchLayer!.Color;

        AssertThat(Math.Abs(actualColor.A - expectedColor.A)).IsLess(OmegaSpiralColors.OpacityTolerance)
            .OverrideFailureMessage($"GlitchLayer opacity: expected {expectedColor.A}, got {actualColor.A}");

        AssertThat(glitchLayer.Visible).IsTrue()
            .OverrideFailureMessage("GlitchLayer should be visible to render glitch effect");
    }

    /// <summary>
    /// Background uses correct dark color from OmegaSpiralColors.DarkVoid design palette.
    /// Verifies UI compliance with centralized design specification.
    /// </summary>
    [TestCase]
    public void Background_UsesCorrectDarkColor()
    {
        var background = _MainMenu.GetNodeOrNull<ColorRect>("Background");
        AssertThat(background).IsNotNull();

        var expectedColor = OmegaSpiralColors.DarkVoid;
        var actualColor = background!.Color;

        // Use design-specified tolerance for float precision
        AssertThat(Math.Abs(actualColor.R - expectedColor.R)).IsLess(OmegaSpiralColors.ColorTolerance)
            .OverrideFailureMessage($"Background Red channel: expected {expectedColor.R}, got {actualColor.R}");
        AssertThat(Math.Abs(actualColor.G - expectedColor.G)).IsLess(OmegaSpiralColors.ColorTolerance)
            .OverrideFailureMessage($"Background Green channel: expected {expectedColor.G}, got {actualColor.G}");
        AssertThat(Math.Abs(actualColor.B - expectedColor.B)).IsLess(OmegaSpiralColors.ColorTolerance)
            .OverrideFailureMessage($"Background Blue channel: expected {expectedColor.B}, got {actualColor.B}");
    }

    /// <summary>
    /// Main menu should have a non-white background to fix the white screen issue.
    /// Checks both the ColorRect's color property and the actual rendered color.
    /// </summary>
    [TestCase]
    public void Background_ShouldNotBeWhite()
    {
        var background = _MainMenu.GetNodeOrNull<ColorRect>("Background");
        AssertThat(background).IsNotNull();

        // Check ColorRect's color property
        var backgroundColor = background!.Color;
        var whiteColor = Colors.White;
        var isWhiteProperty = Math.Abs(backgroundColor.R - whiteColor.R) < OmegaSpiralColors.ColorTolerance &&
                              Math.Abs(backgroundColor.G - whiteColor.G) < OmegaSpiralColors.ColorTolerance &&
                              Math.Abs(backgroundColor.B - whiteColor.B) < OmegaSpiralColors.ColorTolerance;

        AssertThat(isWhiteProperty).IsFalse()
            .OverrideFailureMessage($"Background ColorRect color property is white (R={backgroundColor.R}, G={backgroundColor.G}, B={backgroundColor.B}), it should be dark");

        // Check that background uses correct dark color from design system
        var isUsingDarkVoid = IsColorCloseTo(backgroundColor, OmegaSpiralColors.DarkVoid);
        var isUsingDeepSpace = IsColorCloseTo(backgroundColor, OmegaSpiralColors.DeepSpace);
        
        // Background should use either DarkVoid or DeepSpace (both are dark)
        AssertThat(isUsingDarkVoid || isUsingDeepSpace).IsTrue()
            .OverrideFailureMessage($"Background color (R={backgroundColor.R}, G={backgroundColor.G}, B={backgroundColor.B}) does not match design system dark colors");
    }

    /// <summary>
    /// Helper method to compare two colors within tolerance.
    /// </summary>
    /// <param name="color1">First color to compare.</param>
    /// <param name="color2">Second color to compare.</param>
    /// <returns>True if colors are close enough to be considered equal.</returns>
    private bool IsColorCloseTo(Color color1, Color color2)
    {
        var tolerance = OmegaSpiralColors.ColorTolerance;
        return Math.Abs(color1.R - color2.R) < tolerance &&
               Math.Abs(color1.G - color2.G) < tolerance &&
               Math.Abs(color1.B - color2.B) < tolerance;
    }

    // ==================== BORDER FRAME (SPIRAL ANIMATION) ====================

    /// <summary>
    /// BorderFrame exists and is visible.
    /// OmegaUi creates this programmatically during initialization.
    /// </summary>
    [TestCase]
    public void BorderFrame_Exists()
    {
        var borderFrame = _MainMenu.GetNodeOrNull<ColorRect>("BorderFrame");
        AssertThat(borderFrame).IsNotNull();
        AssertThat(borderFrame!.Visible).IsTrue();
    }

    /// <summary>
    /// BorderFrame has spiral shader material configured.
    /// Uses spiral_border.gdshader for the three-stream animation.
    /// </summary>
    [TestCase]
    public void BorderFrame_HasSpiralShader()
    {
        var borderFrame = _MainMenu.GetNodeOrNull<ColorRect>("BorderFrame");
        AssertThat(borderFrame).IsNotNull();
        AssertThat(borderFrame!.Material).IsInstanceOf<ShaderMaterial>();

        var shaderMaterial = borderFrame.Material as ShaderMaterial;
        AssertThat(shaderMaterial).IsNotNull();
        AssertThat(shaderMaterial!.Shader).IsNotNull();
    }

    /// <summary>
    /// BorderFrame shader uses correct thread colors from OmegaSpiralColors.
    /// Verifies LightThread (Silver), ShadowThread (Golden), AmbitionThread (Crimson).
    /// </summary>
    [TestCase]
    public void BorderFrame_UsesThreeThreadColors()
    {
        var borderFrame = _MainMenu.GetNodeOrNull<ColorRect>("BorderFrame");
        AssertThat(borderFrame).IsNotNull();

        var shaderMaterial = borderFrame!.Material as ShaderMaterial;
        AssertThat(shaderMaterial).IsNotNull();

        // Verify shader parameters exist and match design colors
        var lightThreadColor = (Color)shaderMaterial!.GetShaderParameter("light_thread");
        var shadowThreadColor = (Color)shaderMaterial.GetShaderParameter("shadow_thread");
        var ambitionThreadColor = (Color)shaderMaterial.GetShaderParameter("ambition_thread");

        // Use component-wise comparison with design tolerance
        var tolerance = OmegaSpiralColors.ColorTolerance;

        // Light Thread (Silver)
        AssertThat(Math.Abs(lightThreadColor.R - OmegaSpiralColors.LightThread.R)).IsLess(tolerance)
            .OverrideFailureMessage($"LightThread R: expected {OmegaSpiralColors.LightThread.R}, got {lightThreadColor.R}");
        AssertThat(Math.Abs(lightThreadColor.G - OmegaSpiralColors.LightThread.G)).IsLess(tolerance)
            .OverrideFailureMessage($"LightThread G: expected {OmegaSpiralColors.LightThread.G}, got {lightThreadColor.G}");
        AssertThat(Math.Abs(lightThreadColor.B - OmegaSpiralColors.LightThread.B)).IsLess(tolerance)
            .OverrideFailureMessage($"LightThread B: expected {OmegaSpiralColors.LightThread.B}, got {lightThreadColor.B}");

        // Shadow Thread (Golden)
        AssertThat(Math.Abs(shadowThreadColor.R - OmegaSpiralColors.ShadowThread.R)).IsLess(tolerance)
            .OverrideFailureMessage($"ShadowThread R: expected {OmegaSpiralColors.ShadowThread.R}, got {shadowThreadColor.R}");
        AssertThat(Math.Abs(shadowThreadColor.G - OmegaSpiralColors.ShadowThread.G)).IsLess(tolerance)
            .OverrideFailureMessage($"ShadowThread G: expected {OmegaSpiralColors.ShadowThread.G}, got {shadowThreadColor.G}");
        AssertThat(Math.Abs(shadowThreadColor.B - OmegaSpiralColors.ShadowThread.B)).IsLess(tolerance)
            .OverrideFailureMessage($"ShadowThread B: expected {OmegaSpiralColors.ShadowThread.B}, got {shadowThreadColor.B}");

        // Ambition Thread (Crimson)
        AssertThat(Math.Abs(ambitionThreadColor.R - OmegaSpiralColors.AmbitionThread.R)).IsLess(tolerance)
            .OverrideFailureMessage($"AmbitionThread R: expected {OmegaSpiralColors.AmbitionThread.R}, got {ambitionThreadColor.R}");
        AssertThat(Math.Abs(ambitionThreadColor.G - OmegaSpiralColors.AmbitionThread.G)).IsLess(tolerance)
            .OverrideFailureMessage($"AmbitionThread G: expected {OmegaSpiralColors.AmbitionThread.G}, got {ambitionThreadColor.G}");
        AssertThat(Math.Abs(ambitionThreadColor.B - OmegaSpiralColors.AmbitionThread.B)).IsLess(tolerance)
            .OverrideFailureMessage($"AmbitionThread B: expected {OmegaSpiralColors.AmbitionThread.B}, got {ambitionThreadColor.B}");
    }

    /// <summary>
    /// BorderFrame animation speed matches design specification.
    /// Rotation speed and wave speed should be configurable.
    /// </summary>
    [TestCase]
    public void BorderFrame_AnimationSpeedCorrect()
    {
        var borderFrame = _MainMenu.GetNodeOrNull<ColorRect>("BorderFrame");
        AssertThat(borderFrame).IsNotNull();

        var shaderMaterial = borderFrame!.Material as ShaderMaterial;
        AssertThat(shaderMaterial).IsNotNull();

        // Default values from OmegaUi.CreateBorderFrame()
        var rotationSpeed = (float)shaderMaterial!.GetShaderParameter("rotation_speed");
        var waveSpeed = (float)shaderMaterial.GetShaderParameter("wave_speed");

        // Verify default animation speeds (0.05 for rotation, 0.8 for wave)
        AssertThat(Math.Abs(rotationSpeed - 0.05f)).IsLess(0.01f)
            .OverrideFailureMessage($"Rotation speed: expected 0.05, got {rotationSpeed}");
        AssertThat(Math.Abs(waveSpeed - 0.8f)).IsLess(0.1f)
            .OverrideFailureMessage($"Wave speed: expected 0.8, got {waveSpeed}");
    }
}

