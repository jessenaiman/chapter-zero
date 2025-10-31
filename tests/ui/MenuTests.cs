using GdUnit4;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.UI;

/// <summary>
/// Integration tests for the main menu scene.
/// Tests that the menu UI loads and the "New Game" button triggers game start.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class MenuTests
{
    private ISceneRunner _Runner = null!;

    /// <summary>
    /// Setup: Load the animated main menu scene before each test.
    /// </summary>
    [Before]
    public void Setup()
    {
        _Runner = ISceneRunner.Load("res://source/frontend/ui/menu/scenes/menus/main_menu/main_menu_with_animations.tscn");
    }

    /// <summary>
    /// Teardown: Dispose the scene runner after each test.
    /// </summary>
    [After]
    public void Teardown()
    {
        if (_Runner != null)
        {
            _Runner.Dispose();
        }
    }

    /// <summary>
    /// Test that the menu scene loads and displays the UI.
    /// Verifies that the NewGameButton is visible AND actually rendered (alpha > 0).
    /// </summary>
    [TestCase]
    public async Task MenuLoadsAndDisplaysUI()
    {
        // Simulate a few frames to allow the scene to initialize
        await _Runner.SimulateFrames(2);

        // Find the NewGameButton node in the scene
        var newGameButton = _Runner.FindChild("NewGameButton", recursive: true) as Godot.Button;

        // Verify the button exists
        AssertThat(newGameButton).IsNotNull();

        // Verify the button is visible (not hidden)
        AssertThat(newGameButton!.Visible).IsTrue();

        // Verify the button is actually rendered (alpha channel > 0)
        // This catches the case where animations haven't played and the button is transparent
        AssertThat(newGameButton.Modulate.A).IsGreater(0.0f);
    }

    /// <summary>
    /// Test that pressing the "New Game" button emits the game_started signal.
    /// This verifies that the menu triggers game loading when the button is clicked.
    /// </summary>
    [TestCase]
    public async Task PressNewGameButtonStartsGame()
    {
        // Simulate a few frames to allow the scene to initialize
        await _Runner.SimulateFrames(2);

        // Get the MainMenu scene root
        var mainMenu = _Runner.Scene() as Godot.Node;
        AssertThat(mainMenu).IsNotNull();

        // Start monitoring signals on the main menu
        AssertSignal(mainMenu!).StartMonitoring();

        // Find and verify the NewGameButton exists
        var newGameButton = _Runner.FindChild("NewGameButton", recursive: true) as Godot.Button;
        AssertThat(newGameButton).IsNotNull();

        // Simulate pressing the button via ui_accept action when it has focus
        newGameButton!.GrabFocus();
        _Runner.SimulateActionPressed("ui_accept");
        await _Runner.AwaitInputProcessed();

        // Verify the game_started signal was emitted
        await AssertSignal(mainMenu).IsEmitted("game_started").WithTimeout(500);
    }
}
