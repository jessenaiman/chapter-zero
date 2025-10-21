using Godot;

namespace OmegaSpiral.Ui.Menus
{
    /// <summary>
    /// Main menu for Omega Spiral game.
    /// Handles stage selection and game exit.
    /// </summary>
    [GlobalClass]
    public partial class MainMenu : Control
{
    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// Sets up button connections.
    /// </summary>
    public override void _Ready()
    {
        var startButton = GetNode<Button>("CenterContainer/VBoxContainer/StartButton");
        var optionsButton = GetNode<Button>("CenterContainer/VBoxContainer/OptionsButton");
        var quitButton = GetNode<Button>("CenterContainer/VBoxContainer/QuitButton");

        startButton.Pressed += OnStartPressed;
        optionsButton.Pressed += OnOptionsPressed;
        quitButton.Pressed += OnQuitPressed;

        GD.Print("Main Menu loaded and centered.");
    }

    /// <summary>
    /// Handles Start button press.
    /// </summary>
    private void OnStartPressed()
    {
        GD.Print("Start Game selected - implementation needed");
        // TODO: Load game scene
    }

    /// <summary>
    /// Handles Options button press.
    /// </summary>
    private void OnOptionsPressed()
    {
        GD.Print("Options selected - implementation needed");
        // TODO: Load options scene
    }

    /// <summary>
    /// Handles Quit button press.
    /// </summary>
    private void OnQuitPressed()
    {
        GetTree().Quit();
    }
}
}
