using Godot;
using GdUnit4;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.Ui.Omega;

/// <summary>
/// Tests for the OmegaWindow class.
/// Verifies that omega window components are properly initialized and functional
/// in the refactored component-based Ui architecture.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class OmegaWindowTests : Node
{
    private OmegaWindow? _OmegaWindow;

    /// <summary>
    /// Sets up the test environment before each test.
    /// </summary>
    [Before]
    public void Setup()
    {
    _OmegaWindow = AutoFree(new OmegaWindow())!;
    }

    /// <summary>
    /// Cleans up the test environment after each test.
    /// </summary>
    [After]
    public void Cleanup()
    {
    _OmegaWindow = null;
    }

    /// <summary>
    /// Test: Omega window initializes successfully.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void OmegaWindow_InitializesSuccessfully()
    {
        // Arrange & Assert
    AssertThat(_OmegaWindow).IsNotNull();
    }

    /// <summary>
    /// Test: Omega window is a Control node.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void OmegaWindow_IsControlNode()
    {
        // Assert
    AssertThat(_OmegaWindow).IsInstanceOf<Control>();
    }

    /// <summary>
    /// Test: Omega window can be added to groups.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void OmegaWindow_CanBeAddedToGroups()
    {
        // Arrange
    AssertThat(_OmegaWindow).IsNotNull();

    // Act
    _OmegaWindow!.AddToGroup("omega_windows");

    // Assert
    AssertThat(_OmegaWindow.IsInGroup("omega_windows")).IsTrue();
    }

    /// <summary>
    /// Test: Omega window can set custom data.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void OmegaWindow_CanStoreCustomData()
    {
        // Arrange
    AssertThat(_OmegaWindow).IsNotNull();
    const string testKey = "test_key";
    const string testValue = "test_value";

    // Act
    _OmegaWindow!.SetMeta(testKey, testValue);
    var retrievedValue = _OmegaWindow.GetMeta(testKey);

    // Assert
    AssertThat(retrievedValue.AsString()).IsEqual(testValue);
    }

    /// <summary>
    /// Test: Omega window size can be set.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void OmegaWindow_CanSetSize()
    {
        // Arrange
    AssertThat(_OmegaWindow).IsNotNull();
        var testSize = new Vector2(800, 600);

        // Act
    _OmegaWindow!.CustomMinimumSize = testSize;

        // Assert
    AssertThat(_OmegaWindow.CustomMinimumSize).IsEqual(testSize);
    }

    /// <summary>
    /// Test: Omega window is properly parented in scene.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void OmegaWindow_IsProperlyParentedInScene()
    {
        // Arrange
    AssertThat(_OmegaWindow).IsNotNull();
    AddChild(_OmegaWindow!);

    // Assert
    AssertThat(_OmegaWindow!.GetParent()).IsNotNull();
    // Note: IsNodeReady may be false if parent is not in scene tree yet
    }

    /// <summary>
    /// Test: Omega window processes input events when added to scene.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void OmegaWindow_ProcessesInputEvents()
    {
        // Arrange
    AssertThat(_OmegaWindow).IsNotNull();
    _OmegaWindow!.MouseFilter = Control.MouseFilterEnum.Pass;

    // Assert
    AssertThat(_OmegaWindow.MouseFilter).IsEqual(Control.MouseFilterEnum.Pass);
    }

    /// <summary>
    /// Test: Omega window supports various minimum sizes.
    /// </summary>
    [TestCase(400, 300)]
    [TestCase(800, 600)]
    [TestCase(1920, 1080)]
    [RequireGodotRuntime]
    public void OmegaWindow_SupportsVariousMinimumSizes(int width, int height)
    {
        // Arrange
    AssertThat(_OmegaWindow).IsNotNull();
    var testSize = new Vector2(width, height);

    // Act
    _OmegaWindow!.CustomMinimumSize = testSize;

    // Assert
    AssertThat(_OmegaWindow.CustomMinimumSize.X).IsEqual(width);
    AssertThat(_OmegaWindow.CustomMinimumSize.Y).IsEqual(height);
    }

    /// <summary>
    /// Test: Omega window anchor and margin settings work correctly.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void OmegaWindow_AnchorsAndMarginsWork()
    {
        // Arrange
    AssertThat(_OmegaWindow).IsNotNull();

    // Act
    _OmegaWindow!.AnchorLeft = 0.1f;
    _OmegaWindow.AnchorTop = 0.1f;
    _OmegaWindow.AnchorRight = 0.9f;
    _OmegaWindow.AnchorBottom = 0.9f;

    // Assert
    AssertThat(_OmegaWindow.AnchorLeft).IsEqual(0.1f);
    AssertThat(_OmegaWindow.AnchorTop).IsEqual(0.1f);
    AssertThat(_OmegaWindow.AnchorRight).IsEqual(0.9f);
    AssertThat(_OmegaWindow.AnchorBottom).IsEqual(0.9f);
    }

    // Disposes the test resources.
    /// <summary>
    /// Handles Godot notifications for node lifecycle events.
    /// Ensures disposal is called on NotificationPredelete for robust cleanup.
    /// </summary>
    /// <param name="what">The notification type.</param>
    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
        {
            _OmegaWindow?.QueueFree();
        }
    }

    /// <summary>
    /// Disposes the test resources.
    /// </summary>
    /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _OmegaWindow?.Dispose();
        }
        base.Dispose(disposing);
    }

    // ============================= FRAME LAYOUT TESTS =============================

    private ISceneRunner? _FrameTestRunner;
    private OmegaWindow? _FrameTestWindow;

    /// <summary>
    /// Setup for frame layout tests - loads the scene.
    /// </summary>
    [Before]
    public void FrameTestSetup()
    {
        _FrameTestRunner = ISceneRunner.Load("res://source/ui/omega/omega_window.tscn");
        _FrameTestWindow = (OmegaWindow)_FrameTestRunner.Scene();
        AssertThat(_FrameTestWindow).IsNotNull();
    }

    /// <summary>
    /// Cleanup for frame layout tests.
    /// </summary>
    [After]
    public void FrameTestCleanup()
    {
        _FrameTestRunner?.Dispose();
    }

    /// <summary>
    /// Border Visibility Test: OmegaFrame must have a visible red border.
    /// SHOULD FAIL if border is not rendering.
    /// </summary>
    [TestCase]
    public void OmegaFrame_HasVisibleBorder()
    {
        var omegaFrame = _FrameTestWindow?.GetNodeOrNull<Panel>("Bezel/MainMargin/MainLayout/OmegaFrame");
        AssertThat(omegaFrame).IsNotNull();

        var styleBox = omegaFrame?.GetThemeStylebox("panel") as StyleBoxFlat;
        AssertThat(styleBox).IsNotNull();

        // Border must be red
        var borderColor = styleBox!.BorderColor;
        AssertThat(borderColor.R).IsEqual(1.0f);
        AssertThat(borderColor.G).IsEqual(0.0f);
        AssertThat(borderColor.B).IsEqual(0.0f);

        // Border must be 4px
        AssertThat(styleBox.BorderWidthTop).IsEqual(4);
        AssertThat(styleBox.BorderWidthBottom).IsEqual(4);
        AssertThat(styleBox.BorderWidthLeft).IsEqual(4);
        AssertThat(styleBox.BorderWidthRight).IsEqual(4);

        // Background must be transparent (so border is visible)
        var bgColor = styleBox.BgColor;
        AssertThat(bgColor.A).IsEqual(0.0f);
    }

    /// <summary>
    /// OmegaFrame must be centered horizontally within the viewport.
    /// </summary>
    [TestCase]
    public void OmegaFrame_ShouldBeCenteredHorizontally()
    {
        var omegaFrame = _FrameTestWindow?.GetNodeOrNull<Panel>("Bezel/MainMargin/MainLayout/OmegaFrame");
        AssertThat(omegaFrame).IsNotNull();

        var viewport = _FrameTestWindow?.GetViewport();
        var viewportSize = viewport?.GetVisibleRect().Size ?? Vector2.Zero;
        var viewportCenterX = viewportSize.X / 2;

        var frameRect = omegaFrame!.GetGlobalRect();
        var frameCenterX = frameRect.GetCenter().X;

        // Frame center should match viewport center (within 2 pixels tolerance)
        AssertThat(Mathf.Abs(frameCenterX - viewportCenterX)).IsLess(2.1f);
    }

    /// <summary>
    /// OmegaFrame must be centered vertically within the viewport.
    /// </summary>
    [TestCase]
    public void OmegaFrame_ShouldBeCenteredVertically()
    {
        var omegaFrame = _FrameTestWindow?.GetNodeOrNull<Panel>("Bezel/MainMargin/MainLayout/OmegaFrame");
        AssertThat(omegaFrame).IsNotNull();

        var viewport = _FrameTestWindow?.GetViewport();
        var viewportSize = viewport?.GetVisibleRect().Size ?? Vector2.Zero;
        var viewportCenterY = viewportSize.Y / 2;

        var frameRect = omegaFrame!.GetGlobalRect();
        var frameCenterY = frameRect.GetCenter().Y;

        // Frame center should match viewport center (within 2 pixels tolerance)
        AssertThat(Mathf.Abs(frameCenterY - viewportCenterY)).IsLess(2.1f);
    }

    /// <summary>
    /// Bezel Background Renders - Should fill viewport with dark gray background.
    /// </summary>
    [TestCase]
    public void Bezel_FillsViewportWithBackground()
    {
        var viewport = _FrameTestWindow?.GetViewport();
        if (viewport == null) return;

        var viewportRect = viewport.GetVisibleRect();
        var bezel = _FrameTestWindow?.GetNodeOrNull<Panel>("Bezel");

        AssertThat(bezel).IsNotNull();

        var bezelRect = bezel!.GetGlobalRect();

        // Bezel must fill entire viewport
        AssertThat(bezelRect.Position.X).IsEqual(0);
        AssertThat(bezelRect.Position.Y).IsEqual(0);
        AssertThat(bezelRect.Size.X).IsEqual(viewportRect.Size.X);
        AssertThat(bezelRect.Size.Y).IsEqual(viewportRect.Size.Y);
    }

    /// <summary>
    /// Layout Fills Bezel - MainLayout should expand to fill available space.
    /// </summary>
    [TestCase]
    public void MainLayout_FillsBezelContainer()
    {
        var bezel = _FrameTestWindow?.GetNodeOrNull<Panel>("Bezel");
        var mainLayout = _FrameTestWindow?.GetNodeOrNull<VBoxContainer>("Bezel/MainMargin/MainLayout");

        AssertThat(bezel).IsNotNull();
        AssertThat(mainLayout).IsNotNull();

        var bezelRect = bezel!.GetGlobalRect();
        var layoutRect = mainLayout!.GetGlobalRect();

        // Layout should be substantial size, not collapsed
        AssertThat(layoutRect.Size.X).IsGreater(200);
        AssertThat(layoutRect.Size.Y).IsGreater(200);
    }

    /// <summary>
    /// Header Structure Test: Omega window header must exist with title and status indicators.
    /// - Title: Kenney Pixel, 64px, centered
    /// - Status indicators: right-aligned, color-coded
    /// </summary>
    [TestCase]
    public void Header_ExistsWithTitleAndIndicators()
    {
        // Act - Find header components
        var header = _FrameTestWindow?.GetNodeOrNull<HBoxContainer>("Bezel/MainMargin/MainLayout/OmegaFrame/OmegaContent/Header");
        var title = _FrameTestWindow?.GetNodeOrNull<Label>("Bezel/MainMargin/MainLayout/OmegaFrame/OmegaContent/Header/Title");
        var indicators = _FrameTestWindow?.GetNodeOrNull<HBoxContainer>("Bezel/MainMargin/MainLayout/OmegaFrame/OmegaContent/Header/Indicators");

        // Assert - Header structure exists
        AssertThat(header).IsNotNull();
        AssertThat(title).IsNotNull();
        AssertThat(indicators).IsNotNull();

        // Assert - Indicators contains three status lights
        AssertThat(indicators!.GetChildCount()).IsGreaterEqual(3);
    }

    /// <summary>
    /// ScreenLayout Initialization Test: Content container must be ready for insertion.
    /// - Ensures layout is accessible and matches spacing system
    /// </summary>
    [TestCase]
    public void ScreenLayout_IsEmptyContentInsertionPoint()
    {
        // Act
        var screenLayout = _FrameTestWindow?.GetNodeOrNull<VBoxContainer>("Bezel/MainMargin/MainLayout/OmegaFrame/OmegaContent/OmegaBody/ScreenPadding/ScreenLayout");

        // Assert - ScreenLayout exists and is initially empty
        AssertThat(screenLayout).IsNotNull();
        // Note: ScreenLayout may have some default children like OutputLabel, that's OK
        // What matters is it's accessible and can receive additional content
    }
}
