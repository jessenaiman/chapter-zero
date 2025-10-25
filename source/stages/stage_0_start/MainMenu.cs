using System.Collections.Generic;
using System.Linq;
using Godot;
using OmegaSpiral.Source.Scripts.Infrastructure;
using OmegaSpiral.Source.Ui.Menus;

namespace OmegaSpiral.Source.Stages.Stage0Start
{
    /// <summary>
    /// Main menu for Omega Spiral game.
    /// Loads stage buttons dynamically from the manifest.
    /// Enables players to select which stage to play.
    /// Extends MenuUi for static menu infrastructure (not sequential narrative).
    /// </summary>
    [GlobalClass]
    public partial class MainMenu : MenuUi
    {
        /// <summary>
        /// Mapping of stage IDs to their display names.
        /// </summary>
        private static readonly IReadOnlyDictionary<int, string> _StageNameLookup = new Dictionary<int, string>
        {
            { 1, "Start Here: Ghost Terminal" },
            { 2, "Start Here: Nethack" },
            { 3, "Start Here: Amnesia: Classic RPG" },
            { 4, "Start Here: Never Go Alone: Party Selection" },
            { 5, "Start Here: Fractured Escape" },
            { 6, "Start Here: System Log Epilogue" },
        };

        // --- EXPORTED DEPENDENCIES (Set in the Godot Inspector) ---

        /// <summary>
        /// Path to the stage manifest JSON file.
        /// </summary>
        [ExportGroup("Data Sources")]
        [Export(PropertyHint.File, "*.json")]
        public string StageManifestPath { get; set; } = "res://source/stages/stage_0_start/main_menu_manifest.json";

        // --- PRIVATE FIELDS ---

        private readonly ManifestLoader _ManifestLoader = new();

#pragma warning disable CA2213 // SceneManager is an autoload singleton managed by Godot's scene tree
        private SceneManager? _SceneManager;
#pragma warning restore CA2213

        // Node references cached in CacheRequiredNodes
        private Label? _StageHeader;
        private Button? _StartButton;
        private VBoxContainer? _StageButtonList;

        /// <summary>
        /// Caches all required node references, overriding the base method to add MainMenu-specific nodes.
        /// </summary>
        protected override void CacheRequiredNodes()
        {
            // Let the base MenuUi class cache its nodes first
            base.CacheRequiredNodes();

            // Cache UI labels and buttons from the scene if they exist
            // Note: Stage button list is now managed by OmegaUI.GetOrCreateButtonList()
            _StageHeader = GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StageHeader");
            _StartButton = GetNodeOrNull<Button>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StartButton");

            // Add validation for optional elements
            if (_StageHeader == null)
                GD.PushWarning("[MainMenu] StageHeader label not found in scene.");
            if (_StartButton == null)
                GD.PushWarning("[MainMenu] StartButton not found in scene.");
        }

        /// <summary>
        /// Called when the node enters the scene tree for the first time.
        /// Initializes the menu with all stage buttons and event handlers.
        /// </summary>
        public override void _Ready()
        {
            base._Ready(); // Calls CacheRequiredNodes

            _SceneManager = GetNodeOrNull<SceneManager>("/root/SceneManager");

            PopulateStageList();
            AddMainMenuActions();
        }

        /// <summary>
        /// Loads the stage manifest and populates the dynamic list of stage buttons.
        /// Requests a button list from OmegaUI and adds stage buttons to it.
        /// </summary>
        private void PopulateStageList()
        {
            // Get or create the button list managed by OmegaUI
            _StageButtonList = GetOrCreateButtonList();

            var stages = _ManifestLoader.LoadManifest(StageManifestPath);

            if (stages.Count == 0)
            {
                if (_StageHeader != null)
                    _StageHeader.Text = "No Stages Detected";
                GD.PrintErr("[MainMenu] Stage manifest is empty.");
                return;
            }

            int createdCount = 0;
            foreach (var stage in stages.OrderBy(s => s.Id))
            {
                if (CreateStageButton(stage) is { } stageButton)
                {
                    // CreateStageButton already adds to _StageButtonList, so just count
                    createdCount++;
                }
            }

            if (_StageHeader != null)
            {
                _StageHeader.Text = $"Stage Access · {createdCount} module{(createdCount == 1 ? string.Empty : "s")} detected";
            }

            if (_StartButton != null)
            {
                _StartButton.Text = $"Launch {GetStageTitle(1)}";
            }

            GD.Print($"[MainMenu] Loaded {createdCount} stage buttons.");
        }

        /// <summary>
        /// Creates and configures a stage button for the given stage.
        /// Uses the factory method pattern inherited from MenuUi.CreateMenuButton()
        /// to create buttons with consistent styling and layout.
        /// </summary>
        /// <param name="stage">The stage data to create a button for.</param>
        /// <returns>The configured Button, or null if creation failed.</returns>
        private Button? CreateStageButton(ManifestStage stage)
        {
            string stageTitle = GetStageTitle(stage.Id);
            stage.DisplayName = stageTitle;

            // Create button using base factory method
            var button = CreateButton($"Stage{stage.Id}Button", stageTitle);

            // Add it to the stage button list instead of menu button container
            if (_StageButtonList != null)
            {
                _StageButtonList.AddChild(button);
            }
            else
            {
                GD.PushWarning($"[MainMenu] Cannot add stage button '{stage.Id}' - StageButtonList is not set.");
            }

            // Connect the button's pressed signal to handle stage selection
            button.Pressed += () => OnStageSelected(stage.Id);

            return button;
        }

        /// <summary>
        /// Adds main menu actions using base class helpers.
        /// </summary>
        private void AddMainMenuActions()
        {
            // Use the base class to create the common menu buttons.
            ClearMenuButtons();
            AddMenuButton("Options", OnOptionsPressed);
            AddMenuButton("Quit", OnQuitPressed);

            // Now, just connect the unique "Start" button that is part of this specific scene
            if (_StartButton != null)
            {
                _StartButton.Pressed += OnStartPressed;
            }
        }

        /// <summary>
        /// Handles Start button press - initiates Stage 1.
        /// </summary>
        private void OnStartPressed()
        {
            GD.Print("[MainMenu] Start Game selected - loading Stage 1");
            OnStageSelected(1);
        }

        /// <summary>
        /// Gets the display title for a given stage ID.
        /// </summary>
        /// <param name="stageId">The stage ID.</param>
        /// <returns>The formatted stage title.</returns>
        private static string GetStageTitle(int stageId)
        {
            return _StageNameLookup.TryGetValue(stageId, out string? name)
                ? $"Stage {stageId} · {name}"
                : $"Stage {stageId}";
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
