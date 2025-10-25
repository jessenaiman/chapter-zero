using System.Collections.Generic;
using System.Linq;
using Godot;
using OmegaSpiral.Source.Scripts.Infrastructure;
using OmegaSpiral.Source.Ui.Menus;
using OmegaSpiral.Source.Ui.Components;

namespace OmegaSpiral.Source.Stages.Stage0Start
{
    /// <summary>
    /// Main menu for Omega Spiral game.
    /// Loads stage buttons dynamically from the manifest using MenuUi's button factory.
    /// Extends MenuUi for static menu infrastructure (not sequential narrative).
    /// </summary>
    [GlobalClass]
    public partial class MainMenu : MenuUi
    {
        // --- EXPORTED DEPENDENCIES (Set in the Godot Inspector) ---

    /// <summary>
    /// Path to the stage manifest JSON file.
    /// </summary>
    [ExportGroup("Data Sources")]
    [Export(PropertyHint.File, "*.json")]
    public string StageManifestPath { get; set; } = "res://source/ui/menus/main_menu_manifest.json";

    /// <summary>
    /// Path to the StageButton scene template.
    /// </summary>
    [Export(PropertyHint.File, "*.tscn")]
    public string StageButtonScenePath { get; set; } = "res://source/ui/components/stage_button.tscn";

    // --- PRIVATE FIELDS ---

    private readonly ManifestLoader _ManifestLoader = new();

#pragma warning disable CA2213 // SceneManager is an autoload singleton managed by Godot's scene tree
    private SceneManager? _SceneManager;
#pragma warning restore CA2213

    // Button references created dynamically
    private Button? _StartButton;
        private Button? _OptionsButton;
        private Button? _QuitButton;

        /// <summary>
        /// Called when the node enters the scene tree for the first time.
        /// Initializes the menu and sets the title.
        /// Button population is handled by PopulateMenuButtons().
        /// </summary>
        public override void _Ready()
        {
            base._Ready(); // MenuBase calls PopulateMenuButtons() after initialization

            _SceneManager = GetNodeOrNull<SceneManager>("/root/SceneManager");
            SetMenuTitle("Ωmega Spiral");
        }

        /// <summary>
        /// Populates the main menu with stage buttons and menu actions.
        /// Called by MenuBase after initialization completes.
        /// </summary>
        protected override void PopulateMenuButtons()
        {
            PopulateStageList();
            AddMainMenuActions();
        }

        /// <summary>
        /// Loads the stage manifest and populates the dynamic list of stage buttons.
        /// Uses custom StageButton component with status indicators and OmegaUI styling.
        /// </summary>
        private void PopulateStageList()
        {
            var stages = _ManifestLoader.LoadManifest(StageManifestPath);

            if (stages.Count == 0)
            {
                GD.PrintErr("[MainMenu] Stage manifest is empty.");
                return;
            }

            // Load StageButton scene template
            var stageButtonScene = GD.Load<PackedScene>(StageButtonScenePath);
            if (stageButtonScene == null)
            {
                GD.PrintErr($"[MainMenu] Failed to load StageButton scene: {StageButtonScenePath}");
                return;
            }

            // Create "Launch" button for first stage with special styling
            var firstStage = stages.OrderBy(s => s.Id).FirstOrDefault();
            if (firstStage != null)
            {
                var launchButton = stageButtonScene.Instantiate<StageButton>();
                launchButton.Name = "StartButton";
                launchButton.Configure($"{firstStage.Id}", $"Launch {firstStage.DisplayName}", StageButton.ContentStatus.Ready);
                launchButton.ClickedStage += (stageId) => OnStageSelected(int.Parse(stageId));
                GetNode<VBoxContainer>("ContentContainer/MenuButtonContainer").AddChild(launchButton);
                _StartButton = launchButton;
            }

            // Create stage selection buttons (stages 2-6)
            int createdCount = 0;
            foreach (var stage in stages.OrderBy(s => s.Id).Skip(1))
            {
                var stageButton = stageButtonScene.Instantiate<StageButton>();
                stageButton.Name = $"Stage{stage.Id}Button";
                stageButton.Configure($"{stage.Id}", $"Stage {stage.Id} · {stage.DisplayName}", StageButton.ContentStatus.Ready);
                stageButton.ClickedStage += (stageId) => OnStageSelected(int.Parse(stageId));
                GetNode<VBoxContainer>("ContentContainer/MenuButtonContainer").AddChild(stageButton);
                createdCount++;
            }

            GD.Print($"[MainMenu] Loaded {createdCount} stage buttons from manifest.");
        }

        /// <summary>
        /// Adds main menu actions by creating option and quit buttons.
        /// </summary>
        private void AddMainMenuActions()
        {
            _OptionsButton = CreateMenuButton("OptionsButton", "Options");
            _OptionsButton.Pressed += OnOptionsPressed;

            _QuitButton = CreateMenuButton("QuitButton", "Quit Game");
            _QuitButton.Pressed += OnQuitPressed;

            FocusFirstButton(); // Enable keyboard/gamepad navigation
        }

        /// <summary>
        /// Handles stage selection from any stage button.
        /// Transitions to the selected stage using the scene path from the manifest.
        /// </summary>
        /// <param name="stageId">The numeric stage ID to load.</param>
        private void OnStageSelected(int stageId)
        {
            var stage = _ManifestLoader.GetStage(stageId);
            if (stage == null)
            {
                GD.PrintErr($"[MainMenu] Stage {stageId} not found in manifest.");
                return;
            }

            string entryScenePath = stage.Path;
            GD.Print($"[MainMenu] Starting {stage.DisplayName} - Entry Scene: {entryScenePath}");

            // Validate scene exists
            if (!ResourceLoader.Exists(entryScenePath))
            {
                GD.PrintErr($"[MainMenu] Entry scene does not exist: {entryScenePath}");
                return;
            }

            // Use SceneManager for scene transitions if available
            if (_SceneManager != null)
            {
                _SceneManager.TransitionToScene(entryScenePath, showLoadingScreen: false);
                return;
            }

            // Fallback to direct scene change
            GD.PrintErr("[MainMenu] SceneManager not found! Using fallback scene change.");
            var nextScene = GD.Load<PackedScene>(entryScenePath);
            if (nextScene != null)
            {
                GetTree().ChangeSceneToPacked(nextScene);
            }
        }

        /// <summary>
        /// Handles Options button press.
        /// </summary>
        private void OnOptionsPressed()
        {
            GD.Print("[MainMenu] Options selected - implementation needed");
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
