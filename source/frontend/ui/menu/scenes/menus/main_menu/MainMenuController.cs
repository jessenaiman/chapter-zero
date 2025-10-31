using Godot;
using OmegaSpiral.Source.Backend;

namespace OmegaSpiral.Source.Frontend.Scenes.Menus.MainMenu;

/// <summary>
/// Wires the maaacks_menus_template MainMenu to Omega Spiral's GameManager.
/// Listens for menu "game_started" signal and delegates to GameManager for stage loading.
/// </summary>
public partial class MainMenuController : Node
{
    private Node? _MainMenuNode;
    private GameManager? _GameManagerNode;

    /// <summary>
    /// Called when node enters scene tree. Initializes connections.
    /// </summary>
    public override void _Ready()
    {
        GD.Print("[MainMenuController] Initializing menu-to-game wiring...");

        // Find MainMenu node (parent of this controller)
        _MainMenuNode = GetParent();
        if (_MainMenuNode == null)
        {
            GD.PrintErr("[MainMenuController] CRITICAL: MainMenu parent not found!");
            return;
        }

        GD.Print("[MainMenuController] MainMenu node found");

        // Find or create GameManager
        _GameManagerNode = GetTree().Root.GetNodeOrNull<GameManager>("/root/GameManager");

        if (_GameManagerNode == null)
        {
            GD.PrintErr("[MainMenuController] GameManager autoload not found. Creating instance...");
            _GameManagerNode = new GameManager { Name = "GameManager" };
            GetTree().Root.AddChild(_GameManagerNode);
            GD.Print("[MainMenuController] GameManager instance created and added to scene tree");
        }
        else
        {
            GD.Print("[MainMenuController] GameManager autoload found");
        }

        // Connect menu signal to GameManager via GDScript signal name
        var callable = Callable.From(() => OnGameStarted());
        _MainMenuNode.Connect("game_started", callable);
        GD.Print("[MainMenuController] Connected MainMenu.game_started → GameManager.StartGame()");
    }

    /// <summary>
    /// Called when menu emits game_started signal.
    /// Instructs GameManager to begin loading stages.
    /// </summary>
    private void OnGameStarted()
    {
        if (_GameManagerNode == null)
        {
            GD.PrintErr("[MainMenuController] CRITICAL: Cannot start game - GameManager is null!");
            return;
        }

        GD.Print("[MainMenuController] Menu triggered game start. Delegating to GameManager...");
        _GameManagerNode.StartGame(0); // Start from stage 0
    }
}
