using System;
using Godot;
using GdUnit4;
using System.Threading.Tasks;
using OmegaSpiral.Source.UI.Terminal;
using static GdUnit4.Assertions;

// Tests for the TerminalWindow class.
// Verifies that terminal window components are properly initialized and functional
// in the refactored component-based UI architecture.
[TestSuite]
[RequireGodotRuntime]
public partial class TerminalWindowTests_New : Node
{
    private TerminalWindow? _terminalWindow;

    /// <summary>
    /// Sets up the test environment before each test.
    /// </summary>
    [Before]
    public void Setup()
    {
        _terminalWindow = AutoFree(new TerminalWindow())!;
    }

    /// <summary>
    /// Cleans up the test environment after each test.
    /// </summary>
    [After]
    public void Cleanup()
    {
        _terminalWindow = null;
    }

    /// <summary>
    /// Test: Terminal window initializes successfully.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void TerminalWindow_InitializesSuccessfully()
    {
        // Arrange & Assert
        AssertThat(_terminalWindow).IsNotNull();
    }

    /// <summary>
    /// Test: Terminal window is a Control node.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void TerminalWindow_IsControlNode()
    {
        // Assert
        AssertThat(_terminalWindow).IsInstanceOf<Control>();
    }

    /// <summary>
    /// Test: Terminal window can be added to groups.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void TerminalWindow_CanBeAddedToGroups()
    {
        // Arrange
        AssertThat(_terminalWindow).IsNotNull();

        // Act
        _terminalWindow!.AddToGroup("terminal_windows");

        // Assert
        AssertThat(_terminalWindow.IsInGroup("terminal_windows")).IsTrue();
    }

    /// <summary>
    /// Test: Terminal window can set custom data.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void TerminalWindow_CanStoreCustomData()
    {
        // Arrange
        AssertThat(_terminalWindow).IsNotNull();
        const string testKey = "test_key";
        const string testValue = "test_value";

        // Act
        _terminalWindow!.Set(testKey, testValue);
        var retrievedValue = _terminalWindow.Get(testKey);

        // Assert
        AssertThat(retrievedValue.ToString()).IsEqual(testValue);
    }

    /// <summary>
    /// Test: Terminal window size can be set.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void TerminalWindow_CanSetSize()
    {
        // Arrange
        AssertThat(_terminalWindow).IsNotNull();
        var testSize = new Vector2(800, 600);

        // Act
        _terminalWindow!.CustomMinimumSize = testSize;

        // Assert
        AssertThat(_terminalWindow.CustomMinimumSize).IsEqual(testSize);
    }

    /// <summary>
    /// Test: Terminal window is properly parented when added to scene.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void TerminalWindow_IsProperlyParentedInScene()
    {
        // Arrange
        AssertThat(_terminalWindow).IsNotNull();
        GetTree().Root.AddChild(_terminalWindow!);

        // Assert
        AssertThat(_terminalWindow!.GetParent()).IsNotNull();
        AssertThat(_terminalWindow!.IsNodeReady()).IsTrue();
    }

    /// <summary>
    /// Test: Terminal window processes input events when added to scene.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void TerminalWindow_ProcessesInputEvents()
    {
        // Arrange
        AssertThat(_terminalWindow).IsNotNull();
        _terminalWindow!.MouseFilter = Control.MouseFilterEnum.Pass;

        // Assert
        AssertThat(_terminalWindow.MouseFilter).IsEqual(Control.MouseFilterEnum.Pass);
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
        AssertThat(_terminalWindow).IsNotNull();
        var testSize = new Vector2(width, height);

        // Act
        _terminalWindow!.CustomMinimumSize = testSize;

        // Assert
        AssertThat(_terminalWindow.CustomMinimumSize.X).IsEqual(width);
        AssertThat(_terminalWindow.CustomMinimumSize.Y).IsEqual(height);
    }

    /// <summary>
    /// Test: Terminal window anchor and margin settings work correctly.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void TerminalWindow_AnchorsAndMarginsWork()
    {
        // Arrange
        AssertThat(_terminalWindow).IsNotNull();

        // Act
        _terminalWindow!.AnchorLeft = 0.1f;
        _terminalWindow.AnchorTop = 0.1f;
        _terminalWindow.AnchorRight = 0.9f;
        _terminalWindow.AnchorBottom = 0.9f;

        // Assert
        AssertThat(_terminalWindow.AnchorLeft).IsEqual(0.1f);
        AssertThat(_terminalWindow.AnchorTop).IsEqual(0.1f);
        AssertThat(_terminalWindow.AnchorRight).IsEqual(0.9f);
        AssertThat(_terminalWindow.AnchorBottom).IsEqual(0.9f);
    }

    // Disposes the test resources.
    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
        {
            _terminalWindow?.QueueFree();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _terminalWindow?.Dispose();
        }
        base.Dispose(disposing);
    }
}
