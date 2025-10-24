using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OmegaSpiral.Source.Scripts.Infrastructure;
using OmegaSpiral.Source.Scripts.Infrastructure.StageManagement;
using OmegaSpiral.Source.UI.Menus;
using OmegaSpiral.UI.Menus;

namespace OmegaSpiral.UI.Menus
{
    /// <summary>
    /// Main menu for Omega Spiral game.
    /// Loads stage buttons dynamically from the manifest.
    /// Enables players to select which stage to play.
    /// Extends MenuUI for static menu infrastructure (not sequential narrative).
    /// </summary>
    [GlobalClass]
    public partial class MainMenu : MenuUI
    {
        /// <summary>
        /// Mapping of stage IDs to their display names.
        /// </summary>
        private static readonly IReadOnlyDictionary<int, string> StageNameLookup = new Dictionary<int, string>
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

        /// <summary>
        /// Scene file for individual stage buttons.
        /// </summary>
        [ExportGroup("Scene References")]
        [Export]
        public PackedScene? StageButtonScene { get; set; }

        /// <summary>
        /// Node path to the container holding stage buttons.
        /// </summary>
        [ExportGroup("Node References")]
        [Export]
        public NodePath? StageButtonListPath { get; set; } = new NodePath("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StageButtonList");

        /// <summary>
        /// Node path to the stage list header label.
        /// </summary>
        [Export]
        public NodePath? StageHeaderPath { get; set; } = new NodePath("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StageHeader");

        /// <summary>
        /// Node path to the "Start Game" button.
        /// </summary>
        [Export]
        public NodePath? StartButtonPath { get; set; } = new NodePath("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StartButton");

        // --- PRIVATE FIELDS ---

        private readonly ManifestLoader _manifestLoader = new();

#pragma warning disable CA2213 // SceneManager is an autoload singleton managed by Godot's scene tree
        private SceneManager? _sceneManager;
#pragma warning restore CA2213

        // Node references cached in CacheRequiredNodes
        private VBoxContainer? _stageButtonList;
        private Label? _stageHeader;
        private Button? _startButton;

        /// <summary>
        /// Caches all required node references, overriding the base method to add MainMenu-specific nodes.
        /// </summary>
        protected override void CacheRequiredNodes()
        {
            // Let the base MenuUI class cache its nodes first (like the main button container)
            base.CacheRequiredNodes();

            // Cache the nodes specific to this MainMenu
            _stageButtonList = GetNodeOrNull<VBoxContainer>(StageButtonListPath);
            _stageHeader = GetNodeOrNull<Label>(StageHeaderPath);
            _startButton = GetNodeOrNull<Button>(StartButtonPath);

            // Add validation to catch setup errors early
            if (_stageButtonList == null)
                GD.PushError("[MainMenu] StageButtonList node not assigned in the Inspector or path is incorrect.");
            if (_stageHeader == null)
                GD.PushError("[MainMenu] StageHeader node not assigned in the Inspector or path is incorrect.");
            if (_startButton == null)
                GD.PushError("[MainMenu] StartButton node not assigned in the Inspector or path is incorrect.");
            if (StageButtonScene == null)
                GD.PushError("[MainMenu] StageButtonScene is not assigned in the Inspector.");
        }

        /// <summary>
        /// Called when the node enters the scene tree for the first time.
        /// Initializes the menu with all stage buttons and event handlers.
        /// </summary>
        public override void _Ready()
        {
            base._Ready(); // Calls CacheRequiredNodes

            _sceneManager = GetNodeOrNull<SceneManager>("/root/SceneManager");

            PopulateStageList();
            AddMainMenuActions();
        }

        /// <summary>
        /// Loads the stage manifest and populates the dynamic list of stage buttons.
        /// </summary>
        private void PopulateStageList()
        {
            if (_stageButtonList == null)
            {
                GD.PrintErr("[MainMenu] StageButtonList is null. Cannot populate stage list.");
                return;
            }

            var stages = _manifestLoader.LoadManifest(StageManifestPath);
            if (stages == null || !stages.Any())
            {
                _stageHeader!.Text = "No Stages Detected";
                GD.PrintErr("[MainMenu] Failed to load or parse stage manifest.");
                return;
            }

            // Clear any placeholder children from the scene
            foreach (var child in _stageButtonList.GetChildren())
            {
                child.QueueFree();
            }

            int createdCount = 0;
            foreach (var stage in stages.OrderBy(s => s.Id))
            {
                if (CreateStageButton(stage) is { } stageButton)
                {
                    _stageButtonList.AddChild(stageButton);
                    createdCount++;
                }
            }

            if (_stageHeader != null)
            {
                _stageHeader.Text = $"Stage Access · {createdCount} module{(createdCount == 1 ? string.Empty : "s")} detected";
            }

            if (_startButton != null)
            {
                _startButton.Text = $"Launch {GetStageTitle(1)}";
            }

            GD.Print($"[MainMenu] Loaded {createdCount} stage buttons.");
        }

        /// <summary>
        /// Creates and configures a stage button for the given stage.
        /// </summary>
        /// <param name="stage">The stage data to create a button for.</param>
        /// <returns>The configured StageButton, or null if instantiation failed.</returns>
        private StageButton? CreateStageButton(ManifestStage stage)
        {
            if (StageButtonScene?.Instantiate() is not StageButton stageButton)
            {
                GD.PrintErr($"[MainMenu] Failed to instantiate StageButtonScene for Stage {stage.Id}. Is the scene assigned in the Inspector?");
                return null;
            }

            stageButton.Name = $"Stage{stage.Id}Button";
            string stageTitle = GetStageTitle(stage.Id);
            stage.DisplayName = stageTitle;
            stageButton.Configure(stage.Id.ToString(), stageTitle, ResolveStageStatus(stage.Id));
            stageButton.Pressed += () => OnStageSelected(stage.Id);
            return stageButton;
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
            if (_startButton != null)
            {
                _startButton.Pressed += OnStartPressed;
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
        /// Resolves the content status for a given stage ID.
        /// </summary>
        /// <param name="stageId">The stage ID to resolve status for.</param>
        /// <returns>The ContentStatus for the stage.</returns>
        private static StageButton.ContentStatus ResolveStageStatus(int stageId)
        {
            return stageId switch
            {
                1 => StageButton.ContentStatus.Ready,
                2 => StageButton.ContentStatus.LLMGenerated,
                _ => StageButton.ContentStatus.Missing,
            };
        }

        /// <summary>
        /// Gets the display title for a given stage ID.
        /// </summary>
        /// <param name="stageId">The stage ID.</param>
        /// <returns>The formatted stage title.</returns>
        private static string GetStageTitle(int stageId)
        {
            return StageNameLookup.TryGetValue(stageId, out string? name)
                ? $"Stage {stageId} · {name}"
                : $"Stage {stageId}";
        }

        /// <summary>
        /// Handles stage selection from any stage button.
        /// Transitions to the selected stage via the stage manager.
        /// </summary>
        /// <param name="stageId">The numeric stage ID to load.</param>
        private void OnStageSelected(int stageId)
        {
            var stage = _manifestLoader.GetStage(stageId);
            if (stage == null)
            {
                GD.PrintErr($"[MainMenu] Stage {stageId} not found in manifest.");
                return;
            }

            var stageManager = StageManagerRegistry.GetStageManager(stageId);
            if (stageManager == null)
            {
                GD.PrintErr($"[MainMenu] No stage manager registered for Stage {stageId}.");
                return;
            }

            string entryScenePath = stageManager.ResolveEntryScenePath();
            GD.Print($"[MainMenu] Starting {stage.DisplayName} - Entry Scene: {entryScenePath}");

            if (_sceneManager != null)
            {
                stageManager.TransitionToStage(_sceneManager);
                return;
            }

            GD.PrintErr("[MainMenu] SceneManager not found! Using fallback scene change.");
            if (!ResourceLoader.Exists(entryScenePath))
            {
                GD.PrintErr($"[MainMenu] Entry scene does not exist: {entryScenePath}");
                return;
            }

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
