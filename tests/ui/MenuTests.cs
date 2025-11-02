using Godot;
using GdUnit4;
using static GdUnit4.Assertions;
using System.Threading.Tasks;

namespace OmegaSpiral.Tests.UI;

/// <summary>
/// Test suite for main menu UI functionality, including scene loading and button interactions.
/// Tests both mouse clicks and keyboard input to ensure proper user interaction.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class MenuTests
{
    /// <summary>
    /// Verifies that the main menu scene loads without hanging or errors.
    /// </summary>
    [TestCase(Timeout = 2000)]
    public void MenuSceneLoadsWithoutHanging()
    {
        // Load scene inline - auto-freed by GdUnit API
        ISceneRunner runner = ISceneRunner.Load("res://source/scenes/menus/main_menu/main_menu_with_animations.tscn");

        // Verify scene loaded
        AssertThat(runner.Scene()).IsNotNull();

        // NO Dispose() - runner is auto-freed
    }

    /// <summary>
    /// Tests that clicking the New Game button with mouse loads level one.
    /// This test simulates a real mouse click on the button to verify the complete interaction chain.
    /// </summary>
    [TestCase(Timeout = 5000)]
    public async Task MouseClickNewGameButtonLoadsLevelOne()
    {
        // Load main menu - runner is auto-freed by GdUnit
        ISceneRunner runner = ISceneRunner.Load("res://source/scenes/menus/main_menu/main_menu_with_animations.tscn");
        AssertThat(runner.Scene()).IsNotNull().WithMessage("Main menu scene failed to load");

        // Find the New Game button (this is the "Start" button)
        var newGameButton = runner.FindChild("NewGameButton", recursive: true, owned: false);
        AssertThat(newGameButton)
            .IsNotNull()
            .WithMessage("NewGameButton not found in main menu scene");

        // Focus the button first (as per Maaacks template's CaptureFocus system)
        var button = (Button)newGameButton;
        button.GrabFocus();

        // Simulate mouse click on the button
        // Per mouse.md: "SimulateMouseButtonPressed simulates a mouse button pressed"
        // We need to position mouse over button first, then click
        var buttonRect = button.GetGlobalRect();
        var buttonCenter = buttonRect.Position + buttonRect.Size / 2;

        // Move mouse to button center
        runner.SetMousePos(buttonCenter);

        // Click left mouse button
        runner.SimulateMouseButtonPressed(MouseButton.Left);

        // CRITICAL: Wait for all input events to be processed
        // Per sync_inputs.md: "It's essential for reliable input simulation"
        await runner.AwaitInputProcessed();

        // Wait for scene transition to complete (~3 seconds for level to load)
        await runner.SimulateFrames(180, 16);

        // Verify level_1_ghost loaded by finding its root node
        var levelNode = runner.FindChild("Level1Ghost", recursive: true, owned: false);
        AssertThat(levelNode)
            .IsNotNull()
            .WithMessage("Level1Ghost node not found - level_1_ghost.tscn did not load after mouse click on NewGameButton");
    }

    /// <summary>
    /// Tests that pressing Enter key on the New Game button loads level one.
    /// This test simulates keyboard navigation and Enter key press to verify the complete interaction chain.
    /// </summary>
    [TestCase(Timeout = 5000)]
    public async Task EnterKeyOnNewGameButtonLoadsLevelOne()
    {
        // Load main menu - runner is auto-freed by GdUnit
        ISceneRunner runner = ISceneRunner.Load("res://source/scenes/menus/main_menu/main_menu_with_animations.tscn");
        AssertThat(runner.Scene()).IsNotNull().WithMessage("Main menu scene failed to load");

        // Find the New Game button (this is the "Start" button)
        var newGameButton = runner.FindChild("NewGameButton", recursive: true, owned: false);
        AssertThat(newGameButton)
            .IsNotNull()
            .WithMessage("NewGameButton not found in main menu scene");

        // Focus the button first (as per Maaacks template's CaptureFocus system)
        var button = (Button)newGameButton;
        button.GrabFocus();

        // Simulate keyboard navigation: focus the button first
        // Per keys.md: "SimulateKeyPressed simulates that a key has been pressed"
        runner.SimulateKeyPressed(Key.Enter);

        // CRITICAL: Wait for all input events to be processed
        // Per sync_inputs.md: "It's essential for reliable input simulation"
        await runner.AwaitInputProcessed();

        // Wait for scene transition to complete (~3 seconds for level to load)
        await runner.SimulateFrames(180, 16);

        // Verify level_1_ghost loaded by finding its root node
        var levelNode = runner.FindChild("Level1Ghost", recursive: true, owned: false);
        AssertThat(levelNode)
            .IsNotNull()
            .WithMessage("Level1Ghost node not found - level_1_ghost.tscn did not load after Enter key press on NewGameButton");
    }

    /// <summary>
    /// Tests that pressing Space key on the New Game button loads level one.
    /// This test simulates keyboard navigation and Space key press to verify the complete interaction chain.
    /// </summary>
    [TestCase(Timeout = 5000)]
    public async Task SpaceKeyOnNewGameButtonLoadsLevelOne()
    {
        // Load main menu - runner is auto-freed by GdUnit
        ISceneRunner runner = ISceneRunner.Load("res://source/scenes/menus/main_menu/main_menu_with_animations.tscn");
        AssertThat(runner.Scene()).IsNotNull().WithMessage("Main menu scene failed to load");

        // Find the New Game button (this is the "Start" button)
        var newGameButton = runner.FindChild("NewGameButton", recursive: true, owned: false);
        AssertThat(newGameButton)
            .IsNotNull()
            .WithMessage("NewGameButton not found in main menu scene");

        // Focus the button first (as per Maaacks template's CaptureFocus system)
        var button = (Button)newGameButton;
        button.GrabFocus();

        // Simulate keyboard navigation: focus the button first
        // Per keys.md: "SimulateKeyPressed simulates that a key has been pressed"
        runner.SimulateKeyPressed(Key.Space);

        // CRITICAL: Wait for all input events to be processed
        // Per sync_inputs.md: "It's essential for reliable input simulation"
        await runner.AwaitInputProcessed();

        // Wait for scene transition to complete (~3 seconds for level to load)
        await runner.SimulateFrames(180, 16);

        // Verify level_1_ghost loaded by finding its root node
        var levelNode = runner.FindChild("Level1Ghost", recursive: true, owned: false);
        AssertThat(levelNode)
            .IsNotNull()
            .WithMessage("Level1Ghost node not found - level_1_ghost.tscn did not load after Space key press on NewGameButton");
    }
}
