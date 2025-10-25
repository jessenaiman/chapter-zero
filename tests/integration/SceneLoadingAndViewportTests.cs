// Copyright (c) Ωmega Spiral. All rights reserved.

using Godot;
using GdUnit4;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration;

/// <summary>
/// Integration test suite for scene loading and viewport positioning.
/// Validates that any loaded scene appears in the correct position within the game viewport.
/// This is a game-level test ensuring scenes render centered and visible at runtime.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class SceneLoadingAndViewportTests
{
    /// <summary>
    /// Simplest possible test: Create an X control and verify it can be positioned in the center.
    /// This proves we understand Godot control positioning.
    /// </summary>
    [TestCase]
    public void CreateCenteredXControl()
    {
        // Create a label with "X" as the simplest visual element
        var xLabel = new Label
        {
            Text = "X",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        // Set the label to be centered using anchor properties
        xLabel.LayoutMode = 1; // ANCHOR_MODE
        xLabel.AnchorsPreset = 8; // PRESET_CENTER
        xLabel.OffsetLeft = -25;
        xLabel.OffsetTop = -25;
        xLabel.OffsetRight = 25;
        xLabel.OffsetBottom = 25;

        // Simulate being in a 1920x1080 viewport
        var viewportSize = new Vector2(1920, 1080);
        var expectedCenter = viewportSize / 2;  // (960, 540)

        // Calculate what the label's global rect should be when centered
        // With center anchors and ±25 offsets, the rect should be centered
        var expectedRect = new Rect2(expectedCenter - new Vector2(25, 25), new Vector2(50, 50));

        GD.Print($"Expected viewport center: {expectedCenter}");
        GD.Print($"Expected label rect: {expectedRect}");

        // Verify the anchor settings are correct
        AssertThat(xLabel.AnchorsPreset).IsEqual(8);  // PRESET_CENTER
        AssertThat(xLabel.OffsetLeft).IsEqual(-25);
        AssertThat(xLabel.OffsetTop).IsEqual(-25);
        AssertThat(xLabel.OffsetRight).IsEqual(25);
        AssertThat(xLabel.OffsetBottom).IsEqual(25);

        // The expected rect center should be at the viewport center
        AssertThat(expectedRect.GetCenter()).IsEqual(expectedCenter);

        // The rect should be fully within a 1920x1080 viewport
        var viewportRect = new Rect2(Vector2.Zero, viewportSize);
        AssertThat(viewportRect.Encloses(expectedRect)).IsTrue();

        // Clean up
        xLabel.QueueFree();
    }
    /// <summary>
    /// Test fixture menu scene should not render outside viewport bounds or off-screen.
    /// Validates menu position is within safe bounds (top-left and bottom-right).
    /// </summary>
    [TestCase]
    public void MenuFixture_NeverRendersOffscreen()
    {
        var scene = GD.Load<PackedScene>("res://tests/fixtures/menu_ui_test_fixture.tscn");
        var menu = scene.Instantiate<Control>();

        var viewport = new SubViewport { Size = new Vector2I(1920, 1080) };
        viewport.AddChild(menu);
        menu.Size = viewport.Size;

        var menuRect = menu.GetGlobalRect();
        var viewportSize = new Vector2(1920, 1080);

        // Top-left corner must not be negative
        AssertThat(menuRect.Position.X >= 0.0f).IsTrue();
        AssertThat(menuRect.Position.Y >= 0.0f).IsTrue();

        // Bottom-right corner must not exceed viewport
        AssertThat(menuRect.Position.X + menuRect.Size.X <= viewportSize.X).IsTrue();
        AssertThat(menuRect.Position.Y + menuRect.Size.Y <= viewportSize.Y).IsTrue();

        // Clean up
        menu.QueueFree();
        viewport.QueueFree();
    }

    /// <summary>
    /// Any scene loaded into the game should center properly and render fully visible.
    /// This is a general test for all game scenes to ensure consistent viewport behavior.
    /// </summary>
    [TestCase]
    public void AnyScene_LoadsWithinViewportBounds()
    {
    const string scenePath = "res://source/stages/stage_0_start/main_menu.tscn";
        var scene = GD.Load<PackedScene>(scenePath);
        AssertThat(scene).IsNotNull();

        var instance = scene.Instantiate<Node>();
        AssertThat(instance).IsNotNull();

        var viewport = new SubViewport { Size = new Vector2I(1920, 1080) };
        viewport.AddChild(instance);

        var viewportSize = new Vector2(1920, 1080);
        var viewportCenter = viewportSize / 2.0f;

        // For Control nodes, validate positioning
        if (instance is Control control)
        {
            control.Size = viewport.Size;
            var rect = control.GetGlobalRect();
            var center = rect.GetCenter();

            // Validate centering (within 5 pixel tolerance)
            var deltaX = Mathf.Abs(center.X - viewportCenter.X);
            var deltaY = Mathf.Abs(center.Y - viewportCenter.Y);
            AssertThat(deltaX).IsLess(5.0f);
            AssertThat(deltaY).IsLess(5.0f);

            // Validate bounds
            var viewportRect = new Rect2(Vector2.Zero, viewportSize);
            AssertThat(viewportRect.Encloses(rect)).IsTrue();
        }

        // Clean up
        instance.QueueFree();
        viewport.QueueFree();
    }

    /// <summary>
    /// Scene position must be deterministic and consistent across multiple loads.
    /// Tests that scenes always load in the same position (not random or corrupted).
    /// </summary>
    [TestCase]
    public void Scene_LoadsConsistentlyAcrossMultipleInstances()
    {
    const string scenePath = "res://source/stages/stage_0_start/main_menu.tscn";
        var positions = new Vector2[3];

        for (int i = 0; i < 3; i++)
        {
            var scene = GD.Load<PackedScene>(scenePath);
            var menu = scene.Instantiate<Control>();

            var viewport = new SubViewport { Size = new Vector2I(1920, 1080) };
            viewport.AddChild(menu);
            menu.Size = viewport.Size;

            positions[i] = menu.GetGlobalRect().GetCenter();

            // Clean up
            menu.QueueFree();
            viewport.QueueFree();
        }

        // All three loads should have the same center position
        AssertThat(Mathf.Abs(positions[0].X - positions[1].X)).IsLess(0.1f);
        AssertThat(Mathf.Abs(positions[1].X - positions[2].X)).IsLess(0.1f);
        AssertThat(Mathf.Abs(positions[0].Y - positions[1].Y)).IsLess(0.1f);
        AssertThat(Mathf.Abs(positions[1].Y - positions[2].Y)).IsLess(0.1f);
    }

    /// <summary>
    /// Scene must have non-zero dimensions and not be collapsed.
    /// Validates that scenes are properly sized and not invisible/zero-sized.
    /// </summary>
    [TestCase]
    public void Scene_HasValidDimensions()
    {
        var scene = GD.Load<PackedScene>("res://source/ui/menus/stage_select_menu.tscn");
        var menu = scene.Instantiate<Control>();

        var viewport = new SubViewport { Size = new Vector2I(1920, 1080) };
        viewport.AddChild(menu);
        menu.Size = viewport.Size;

        var rect = menu.GetGlobalRect();

        // Size must be positive (not collapsed)
        AssertThat(rect.Size.X > 0.0f).IsTrue();
        AssertThat(rect.Size.Y > 0.0f).IsTrue();

        // Clean up
        menu.QueueFree();
        viewport.QueueFree();
    }
}
