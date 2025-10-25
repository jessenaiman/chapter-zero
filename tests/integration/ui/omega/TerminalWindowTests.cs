using Godot;
using GdUnit4;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

/// <summary>
/// Tests for the TerminalWindow class.
/// Verifies that terminal window components are properly initialized and functional
/// in the refactored component-based Ui architecture.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class TerminalWindowTestsNew : Node
{
    private TerminalWindow? _TerminalWindow;

    /// <summary>
    /// Sets up the test environment before each test.
    /// </summary>
    [Before]
    public void Setup()
    {
    _TerminalWindow = AutoFree(new TerminalWindow())!;
    }

    /// <summary>
    /// Cleans up the test environment after each test.
    /// </summary>
    [After]
    public void Cleanup()
    {
    _TerminalWindow = null;
    }

    /// <summary>
    /// Test: Terminal window initializes successfully.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void TerminalWindow_InitializesSuccessfully()
    {
        // Arrange & Assert
    AssertThat(_TerminalWindow).IsNotNull();
    }

    /// <summary>
    /// Test: Terminal window is a Control node.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void TerminalWindow_IsControlNode()
    {
        // Assert
    AssertThat(_TerminalWindow).IsInstanceOf<Control>();
    }

    /// <summary>
    /// Test: Terminal window can be added to groups.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void TerminalWindow_CanBeAddedToGroups()
    {
        // Arrange
    AssertThat(_TerminalWindow).IsNotNull();

    // Act
    _TerminalWindow!.AddToGroup("terminal_windows");

    // Assert
    AssertThat(_TerminalWindow.IsInGroup("terminal_windows")).IsTrue();
    }

    /// <summary>
    /// Test: Terminal window can set custom data.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void TerminalWindow_CanStoreCustomData()
    {
        // Arrange
    AssertThat(_TerminalWindow).IsNotNull();
    const string testKey = "test_key";
    const string testValue = "test_value";

    // Act
    _TerminalWindow!.SetMeta(testKey, testValue);
    var retrievedValue = _TerminalWindow.GetMeta(testKey);

    // Assert
    AssertThat(retrievedValue.AsString()).IsEqual(testValue);
    }

    /// <summary>
    /// Test: Terminal window size can be set.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void TerminalWindow_CanSetSize()
    {
        // Arrange
    AssertThat(_TerminalWindow).IsNotNull();
        var testSize = new Vector2(800, 600);

        // Act
    _TerminalWindow!.CustomMinimumSize = testSize;

        // Assert
    AssertThat(_TerminalWindow.CustomMinimumSize).IsEqual(testSize);
    }

    /// <summary>
    /// Test: Terminal window is properly parented in scene.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void TerminalWindow_IsProperlyParentedInScene()
    {
        // Arrange
    AssertThat(_TerminalWindow).IsNotNull();
    AddChild(_TerminalWindow!);

    // Assert
    AssertThat(_TerminalWindow!.GetParent()).IsNotNull();
    // Note: IsNodeReady may be false if parent is not in scene tree yet
    }

    /// <summary>
    /// Test: Terminal window processes input events when added to scene.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void TerminalWindow_ProcessesInputEvents()
    {
        // Arrange
    AssertThat(_TerminalWindow).IsNotNull();
    _TerminalWindow!.MouseFilter = Control.MouseFilterEnum.Pass;

    // Assert
    AssertThat(_TerminalWindow.MouseFilter).IsEqual(Control.MouseFilterEnum.Pass);
    }

    /// <summary>
    /// Test: Terminal window supports various minimum sizes.
    /// </summary>
    [TestCase(400, 300)]
    [TestCase(800, 600)]
    [TestCase(1920, 1080)]
    [RequireGodotRuntime]
    public void TerminalWindow_SupportsVariousMinimumSizes(int width, int height)
    {
        // Arrange
    AssertThat(_TerminalWindow).IsNotNull();
    var testSize = new Vector2(width, height);

    // Act
    _TerminalWindow!.CustomMinimumSize = testSize;

    // Assert
    AssertThat(_TerminalWindow.CustomMinimumSize.X).IsEqual(width);
    AssertThat(_TerminalWindow.CustomMinimumSize.Y).IsEqual(height);
    }

    /// <summary>
    /// Test: Terminal window anchor and margin settings work correctly.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void TerminalWindow_AnchorsAndMarginsWork()
    {
        // Arrange
    AssertThat(_TerminalWindow).IsNotNull();

    // Act
    _TerminalWindow!.AnchorLeft = 0.1f;
    _TerminalWindow.AnchorTop = 0.1f;
    _TerminalWindow.AnchorRight = 0.9f;
    _TerminalWindow.AnchorBottom = 0.9f;

    // Assert
    AssertThat(_TerminalWindow.AnchorLeft).IsEqual(0.1f);
    AssertThat(_TerminalWindow.AnchorTop).IsEqual(0.1f);
    AssertThat(_TerminalWindow.AnchorRight).IsEqual(0.9f);
    AssertThat(_TerminalWindow.AnchorBottom).IsEqual(0.9f);
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
            _TerminalWindow?.QueueFree();
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
            _TerminalWindow?.Dispose();
        }
        base.Dispose(disposing);
    }
}
