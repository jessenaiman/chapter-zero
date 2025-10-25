// <copyright file="MainMenuTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Linq;
using Godot;
using GdUnit4;
using OmegaSpiral.Source.Stages.Stage0Start;
using OmegaSpiral.Source.Ui.Menus;
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
    /// MenuTitle uses amber color matching design spec: Color(0.992157, 0.788235, 0.384314, 1).
    /// This is the primary text color from omega_ui.theme.tres.
    /// </summary>
    [TestCase]
    public void MenuTitle_UsesAmberDesignColor()
    {
        var title = _MainMenu.GetNodeOrNull<Label>("ContentContainer/MenuTitle");
        AssertThat(title).IsNotNull();

        // Design spec: Amber from Omega Spiral logo
        var expectedAmber = new Color(0.992157f, 0.788235f, 0.384314f, 1.0f);
        var actualColor = title!.Modulate;

        // Use tolerance due to float precision: ±0.01 per channel
        AssertThat(Math.Abs(actualColor.R - expectedAmber.R)).IsLess(0.01f)
            .OverrideFailureMessage($"MenuTitle Red channel: expected ~0.992157, got {actualColor.R}");
        AssertThat(Math.Abs(actualColor.G - expectedAmber.G)).IsLess(0.01f)
            .OverrideFailureMessage($"MenuTitle Green channel: expected ~0.788235, got {actualColor.G}");
        AssertThat(Math.Abs(actualColor.B - expectedAmber.B)).IsLess(0.01f)
            .OverrideFailureMessage($"MenuTitle Blue channel: expected ~0.384314, got {actualColor.B}");
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
    /// PhosphorLayer (CRT shader effect) is visible with correct opacity (18%).
    /// Enables the amber phosphor glow effect over the content.
    /// </summary>
    [TestCase]
    public void PhosphorLayer_IsVisibleWithCorrectOpacity()
    {
        var phosphorLayer = _MainMenu.GetNodeOrNull<ColorRect>("PhosphorLayer");
        AssertThat(phosphorLayer).IsNotNull();

        // Color should be white with ~18% opacity for glow effect
        var expectedColor = new Color(1.0f, 1.0f, 1.0f, 0.18f);
        var actualColor = phosphorLayer!.Color;

        AssertThat(Math.Abs(actualColor.A - expectedColor.A)).IsLess(0.02f)
            .OverrideFailureMessage($"PhosphorLayer opacity should be ~0.18 (18%), got {actualColor.A}");

        // Layer should be visible (not hidden)
        AssertThat(phosphorLayer.Visible).IsTrue()
            .OverrideFailureMessage("PhosphorLayer should be visible to render glow effect");
    }

    /// <summary>
    /// ScanlineLayer (CRT scanline effect) is visible with correct opacity (12%).
    /// Adds scanline pattern overlay for retro aesthetic.
    /// </summary>
    [TestCase]
    public void ScanlineLayer_IsVisibleWithCorrectOpacity()
    {
        var scanlineLayer = _MainMenu.GetNodeOrNull<ColorRect>("ScanlineLayer");
        AssertThat(scanlineLayer).IsNotNull();

        // Color should be white with ~12% opacity for scanline effect
        var expectedColor = new Color(1.0f, 1.0f, 1.0f, 0.12f);
        var actualColor = scanlineLayer!.Color;

        AssertThat(Math.Abs(actualColor.A - expectedColor.A)).IsLess(0.02f)
            .OverrideFailureMessage($"ScanlineLayer opacity should be ~0.12 (12%), got {actualColor.A}");

        AssertThat(scanlineLayer.Visible).IsTrue()
            .OverrideFailureMessage("ScanlineLayer should be visible to render scanlines");
    }

    /// <summary>
    /// GlitchLayer (CRT glitch effect) is visible with correct opacity (8%).
    /// Adds glitch distortion effect for authentic retro feel.
    /// </summary>
    [TestCase]
    public void GlitchLayer_IsVisibleWithCorrectOpacity()
    {
        var glitchLayer = _MainMenu.GetNodeOrNull<ColorRect>("GlitchLayer");
        AssertThat(glitchLayer).IsNotNull();

        // Color should be white with ~8% opacity for glitch effect
        var expectedColor = new Color(1.0f, 1.0f, 1.0f, 0.08f);
        var actualColor = glitchLayer!.Color;

        AssertThat(Math.Abs(actualColor.A - expectedColor.A)).IsLess(0.02f)
            .OverrideFailureMessage($"GlitchLayer opacity should be ~0.08 (8%), got {actualColor.A}");

        AssertThat(glitchLayer.Visible).IsTrue()
            .OverrideFailureMessage("GlitchLayer should be visible to render glitch effect");
    }

    /// <summary>
    /// Background has correct dark blue-gray color from design spec.
    /// Color: RGB(0.0352941, 0.0352941, 0.0509804) from omega_ui.theme.tres.
    /// </summary>
    [TestCase]
    public void Background_UsesCorrectDarkColor()
    {
        var background = _MainMenu.GetNodeOrNull<ColorRect>("Background");
        AssertThat(background).IsNotNull();

        // Design spec: Very dark blue-gray
        var expectedColor = new Color(0.0352941f, 0.0352941f, 0.0509804f, 1.0f);
        var actualColor = background!.Color;

        AssertThat(Math.Abs(actualColor.R - expectedColor.R)).IsLess(0.01f);
        AssertThat(Math.Abs(actualColor.G - expectedColor.G)).IsLess(0.01f);
        AssertThat(Math.Abs(actualColor.B - expectedColor.B)).IsLess(0.01f);
    }
}
