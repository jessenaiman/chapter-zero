// <copyright file="BackgroundComponentTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Ui.Components;

using Godot;
using GdUnit4;
using static GdUnit4.Assertions;

/// <summary>
/// Structural validation tests for the reusable Background component.
/// Confirms the bezel panel scene keeps its anchors, margins, and style configuration.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class BackgroundComponentTests : Node
{
    private const string _BackgroundScenePath = "res://source/ui/components/background.tscn";

    private Panel InstantiateBackground()
    {
        var scene = ResourceLoader.Load<PackedScene>(_BackgroundScenePath);
        AssertThat(scene).IsNotNull();

        var instance = AutoFree(scene!.Instantiate())!;
        AssertThat(instance).IsInstanceOf<Panel>();

        return (Panel)instance;
    }

    /// <summary>
    /// Ensures the background scene loads and anchors stretch to the viewport bounds.
    /// </summary>
    [TestCase]
    public void BackgroundScene_LoadsPanelWithViewportAnchors()
    {
    var panel = InstantiateBackground();
        AssertThat(panel.AnchorLeft).IsEqual(0.0f);
        AssertThat(panel.AnchorTop).IsEqual(0.0f);
        AssertThat(panel.AnchorRight).IsEqual(1.0f);
        AssertThat(panel.AnchorBottom).IsEqual(1.0f);
        AssertThat(panel.GrowHorizontal).IsEqual(Control.GrowDirection.Both);
        AssertThat(panel.GrowVertical).IsEqual(Control.GrowDirection.Both);
    }

    /// <summary>
    /// Validates the bezel style keeps the expected colors and border widths.
    /// </summary>
    [TestCase]
    public void BackgroundScene_UsesExpectedBezelStyle()
    {
    var panel = InstantiateBackground();

    var styleBox = panel.GetThemeStylebox("panel") as StyleBoxFlat;
        AssertThat(styleBox).IsNotNull();

        var bgColor = styleBox!.BgColor;
        AssertThat(bgColor.R).IsEqual(0.2f);
        AssertThat(bgColor.G).IsEqual(0.2f);
        AssertThat(bgColor.B).IsEqual(0.1f);
        AssertThat(bgColor.A).IsEqual(1.0f);

        AssertThat(styleBox.BorderWidthLeft).IsEqual(0);
        AssertThat(styleBox.BorderWidthTop).IsEqual(0);
        AssertThat(styleBox.BorderWidthRight).IsEqual(0);
        AssertThat(styleBox.BorderWidthBottom).IsEqual(0);
    }

    /// <summary>
    /// Verifies the content margins align with the bezel spacing contract.
    /// </summary>
    [TestCase]
    public void BackgroundScene_HasConsistentContentMargins()
    {
    var panel = InstantiateBackground();

    var styleBox = panel.GetThemeStylebox("panel") as StyleBoxFlat;
        AssertThat(styleBox).IsNotNull();

        AssertThat(styleBox!.ContentMarginLeft).IsEqual(16.0f);
        AssertThat(styleBox.ContentMarginRight).IsEqual(16.0f);
        AssertThat(styleBox.ContentMarginTop).IsEqual(8.0f);
        AssertThat(styleBox.ContentMarginBottom).IsEqual(8.0f);
    }
}
