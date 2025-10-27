// <copyright file="MainMenu_IntegrationTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Stages.Stage0Start;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.Ui.Menus;

/// <summary>
/// Integration tests for MainMenu with real scene instantiation.
/// Tests that Omega visual theme (border, CRT effects, centering) works in actual gameplay.
///
/// RESPONSIBILITY: Verify MainMenu loads correctly with all Omega visual components
/// present and functional - no mocking, tests real scene behavior.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class MainMenu_IntegrationTests
{
    private ISceneRunner? _Runner;
    private MainMenu? _MainMenu;

    [Before]
    public void Setup()
    {
        // Load the actual scene file used in the game
        _Runner = ISceneRunner.Load("res://source/ui/menus/main_menu.tscn");
        _MainMenu = (MainMenu)_Runner.Scene();

        AssertThat(_MainMenu).IsNotNull()
            .OverrideFailureMessage("MainMenu scene failed to load - check scene file exists and is valid");
    }

    [After]
    public void Cleanup()
    {
        _Runner?.Dispose();
    }

    // ==================== OMEGA VISUAL THEME ====================

    /// <summary>
    /// Tests that BorderFrame is created and visible.
    /// This is the signature Omega border that makes the UI look like a CRT terminal.
    /// </summary>
    [TestCase]
    public void MainMenu_HasVisibleBorderFrame()
    {
        // The border should be created by OmegaThemedContainer during _Ready()
        var borderFrame = _MainMenu!.GetNodeOrNull<Control>("BorderFrame");

        AssertThat(borderFrame).IsNotNull()
            .OverrideFailureMessage("BorderFrame not found - Omega border missing! Check OmegaThemedContainer.CreateComponents()");

        AssertThat(borderFrame!.Visible).IsTrue()
            .OverrideFailureMessage("BorderFrame exists but is not visible");
    }

    /// <summary>
    /// Tests that PhosphorLayer (CRT glow effect) is present and visible.
    /// </summary>
    [TestCase]
    public void MainMenu_HasVisiblePhosphorLayer()
    {
        var phosphorLayer = _MainMenu!.GetNodeOrNull<ColorRect>("PhosphorLayer");

        AssertThat(phosphorLayer).IsNotNull()
            .OverrideFailureMessage("PhosphorLayer not found - CRT phosphor glow missing!");

        AssertThat(phosphorLayer!.Visible).IsTrue()
            .OverrideFailureMessage("PhosphorLayer exists but is not visible");

        // Verify color has proper opacity for glow effect
        var color = phosphorLayer.Color;
        AssertThat(color.A).IsGreater(0f)
            .OverrideFailureMessage($"PhosphorLayer has zero opacity - should be translucent, got alpha={color.A}");
    }

    /// <summary>
    /// Tests that ScanlineLayer (CRT scanlines) is present and visible.
    /// </summary>
    [TestCase]
    public void MainMenu_HasVisibleScanlineLayer()
    {
        var scanlineLayer = _MainMenu!.GetNodeOrNull<ColorRect>("ScanlineLayer");

        AssertThat(scanlineLayer).IsNotNull()
            .OverrideFailureMessage("ScanlineLayer not found - CRT scanlines missing!");

        AssertThat(scanlineLayer!.Visible).IsTrue()
            .OverrideFailureMessage("ScanlineLayer exists but is not visible");

        var color = scanlineLayer.Color;
        AssertThat(color.A).IsGreater(0f)
            .OverrideFailureMessage($"ScanlineLayer has zero opacity, got alpha={color.A}");
    }

    /// <summary>
    /// Tests that GlitchLayer (CRT glitch effect) is present and visible.
    /// </summary>
    [TestCase]
    public void MainMenu_HasVisibleGlitchLayer()
    {
        var glitchLayer = _MainMenu!.GetNodeOrNull<ColorRect>("GlitchLayer");

        AssertThat(glitchLayer).IsNotNull()
            .OverrideFailureMessage("GlitchLayer not found - CRT glitch effect missing!");

        AssertThat(glitchLayer!.Visible).IsTrue()
            .OverrideFailureMessage("GlitchLayer exists but is not visible");
    }

    // ==================== LAYOUT & CENTERING ====================

    /// <summary>
    /// Tests that MenuTitle is horizontally centered.
    /// </summary>
    [TestCase]
    public void MenuTitle_IsHorizontallyCentered()
    {
        var menuTitle = _MainMenu!.GetNodeOrNull<Label>("ContentContainer/MenuTitle");

        AssertThat(menuTitle).IsNotNull()
            .OverrideFailureMessage("MenuTitle not found in ContentContainer");

        // Check horizontal alignment is center (1)
        var alignment = menuTitle!.HorizontalAlignment;
        AssertThat(alignment).IsEqual(HorizontalAlignment.Center)
            .OverrideFailureMessage($"MenuTitle should be horizontally centered, got alignment={alignment}");
    }

    /// <summary>
    /// Tests that ContentContainer is properly laid out (no overlapping buttons).
    /// </summary>
    [TestCase]
    public void ContentContainer_HasProperVerticalLayout()
    {
        var contentContainer = _MainMenu!.GetNodeOrNull<VBoxContainer>("ContentContainer");

        AssertThat(contentContainer).IsNotNull()
            .OverrideFailureMessage("ContentContainer not found or wrong type - should be VBoxContainer");

        // VBoxContainer should have separation between items to prevent overlap
        var separation = contentContainer!.GetThemeConstant("separation");
        AssertThat(separation).IsGreaterEqual(12)
            .OverrideFailureMessage($"ContentContainer separation too small - buttons may overlap, got {separation}px");
    }

    /// <summary>
    /// Tests that MenuButtonContainer is center-aligned (buttons centered horizontally).
    /// </summary>
    [TestCase]
    public void MenuButtonContainer_IsCenterAligned()
    {
        var buttonContainer = _MainMenu!.GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");

        AssertThat(buttonContainer).IsNotNull()
            .OverrideFailureMessage("MenuButtonContainer not found");

        // Check BoxContainer alignment is center (1)
        var alignment = buttonContainer!.Alignment;
        AssertThat(alignment).IsEqual(BoxContainer.AlignmentMode.Center)
            .OverrideFailureMessage($"MenuButtonContainer should be center-aligned, got alignment={alignment}");
    }

    // ==================== BACKGROUND COLOR ====================

    /// <summary>
    /// Tests that Background uses correct dark color (not white).
    /// </summary>
    [TestCase]
    public void Background_UsesCorrectDarkColor()
    {
        var background = _MainMenu!.GetNodeOrNull<ColorRect>("Background");

        AssertThat(background).IsNotNull()
            .OverrideFailureMessage("Background ColorRect not found");

        var color = background!.Color;

        // Verify it's a dark color (all RGB components < 0.3)
        AssertThat(color.R).IsLess(0.3f)
            .OverrideFailureMessage($"Background R channel too bright (not dark), got R={color.R}");
        AssertThat(color.G).IsLess(0.3f)
            .OverrideFailureMessage($"Background G channel too bright (not dark), got G={color.G}");
        AssertThat(color.B).IsLess(0.3f)
            .OverrideFailureMessage($"Background B channel too bright (not dark), got B={color.B}");
    }

    /// <summary>
    /// Critical test: Background should NOT be white (common mistake when theme breaks).
    /// </summary>
    [TestCase]
    public void Background_IsNotWhite()
    {
        var background = _MainMenu!.GetNodeOrNull<ColorRect>("Background");

        AssertThat(background).IsNotNull();

        var color = background!.Color;
        var isWhite = color.R > 0.9f && color.G > 0.9f && color.B > 0.9f;

        AssertThat(isWhite).IsFalse()
            .OverrideFailureMessage($"Background is WHITE! Theme is broken. Got color={color}");
    }

    // ==================== MENU STRUCTURE ====================

    /// <summary>
    /// Tests that all required menu nodes exist (smoke test for scene integrity).
    /// </summary>
    [TestCase]
    public void MainMenu_HasRequiredNodes()
    {
        // Core structure
        AssertThat(_MainMenu!.GetNodeOrNull("ContentContainer")).IsNotNull()
            .OverrideFailureMessage("ContentContainer missing");
        AssertThat(_MainMenu.GetNodeOrNull("ContentContainer/MenuTitle")).IsNotNull()
            .OverrideFailureMessage("MenuTitle missing");
        AssertThat(_MainMenu.GetNodeOrNull("ContentContainer/MenuButtonContainer")).IsNotNull()
            .OverrideFailureMessage("MenuButtonContainer missing");
        AssertThat(_MainMenu.GetNodeOrNull("ContentContainer/MenuActionBar")).IsNotNull()
            .OverrideFailureMessage("MenuActionBar missing");
    }

    /// <summary>
    /// Tests that MainMenu extends BaseMenuUi which extends OmegaThemedContainer.
    /// </summary>
    [TestCase]
    public void MainMenu_InheritsOmegaTheming()
    {
        AssertThat(_MainMenu).IsInstanceOf<OmegaThemedContainer>()
            .OverrideFailureMessage("MainMenu should inherit from OmegaThemedContainer through BaseMenuUi");
    }
}
