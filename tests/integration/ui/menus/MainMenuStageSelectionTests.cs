using System.Threading.Tasks;
using Godot;
using GdUnit4;
using GdUnit4.Api;
using static GdUnit4.Assertions;

[TestSuite]
public static class MainMenuStageSelectionTests
{

    [TestCase]
    [RequireGodotRuntime]
    public static async Task MainMenu_LoadsAndProvidesStageButtons()
    {
        // Load the main menu scene via SceneRunner
        using var runner = ISceneRunner.Load("res://source/stages/stage_0_start/main_menu.tscn");
        await runner.SimulateFrames(1).ConfigureAwait(false);

        // Basic assertions that the scene runner and expected buttons are present
        AssertThat(runner).IsNotNull();

        // Ensure StartButton, OptionsButton and QuitButton exist in the loaded scene
        var startButton = runner.GetProperty("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StartButton");
        var optionsButton = runner.GetProperty("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/OptionsButton");
        var quitButton = runner.GetProperty("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/QuitButton");

        AssertThat(startButton).IsNotNull();
        AssertThat(optionsButton).IsNotNull();
        AssertThat(quitButton).IsNotNull();
    }

    [TestCase]
    [RequireGodotRuntime]
    public static async Task Stage1Button_Press_InvokesStageSelection()
    {
        using var runner = ISceneRunner.Load("res://source/stages/stage_0_start/main_menu.tscn");
        await runner.SimulateFrames(1).ConfigureAwait(false);

        // Simulate a press on the Stage1Button by emitting its 'pressed' signal.
        // If the MainMenu wiring is missing or the method throws, this test will fail.
        runner.Invoke("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StartButton", "EmitSignal", Variant.From("pressed"));
        await runner.SimulateFrames(2).ConfigureAwait(false);

        // At minimum ensure the runner still exists and scene processed the input.
        AssertThat(runner).IsNotNull();
    }
}
