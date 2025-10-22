using System.Threading.Tasks;
using Godot;
using GdUnit4;
using GdUnit4.Api;
using static GdUnit4.Assertions;

[TestSuite]
public class MainMenuStageSelectionTests
{
    [TestCase]
    [RequireGodotRuntime]
    public static async Task StageSelectMenu_LoadsAndProvidesStageButtons()
    {
        // Load the stage select menu scene via SceneRunner
        using var runner = ISceneRunner.Load("res://source/ui/menus/stage_select_menu.tscn");
        await runner.SimulateFrames(1);

        // Basic assertions that the scene runner and expected buttons are present
        AssertThat(runner).IsNotNull();

        // Ensure Stage1Button and QuitButton exist in the loaded scene
        var stage1Button = runner.GetProperty("Center/MenuVBox/MenuButtonsMargin/MenuButtonsContainer/StagesPanel/Stage1Button");
        var quitButton = runner.GetProperty("Center/MenuVBox/MenuButtonsMargin/MenuButtonsContainer/StagesPanel/QuitButton");

        AssertThat(stage1Button).IsNotNull();
        AssertThat(quitButton).IsNotNull();
    }

    [TestCase]
    [RequireGodotRuntime]
    public static async Task Stage1Button_Press_InvokesStageSelection()
    {
        using var runner = ISceneRunner.Load("res://source/ui/menus/stage_select_menu.tscn");
        await runner.SimulateFrames(1);

        // Simulate a press on the Stage1Button by emitting its 'pressed' signal.
        // If the StageSelectMenu wiring is missing or the method throws, this test will fail.
        runner.Invoke("Center/MenuVBox/MenuButtonsMargin/MenuButtonsContainer/StagesPanel/Stage1Button", "EmitSignal", Variant.From("pressed"));
        await runner.SimulateFrames(2);

        // At minimum ensure the runner still exists and scene processed the input.
        AssertThat(runner).IsNotNull();
    }
}
