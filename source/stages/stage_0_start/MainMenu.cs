using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OmegaSpiral.Source.Scripts.Infrastructure;
using OmegaSpiral.Source.Scripts.Infrastructure.StageManagement;
using OmegaSpiral.UI.Menus;

namespace OmegaSpiral.Ui.Menus
{
    /// <summary>
    /// Main menu for Omega Spiral game.
    /// Loads stage buttons dynamically from the manifest.
    /// Enables players to select which stage to play.
    /// </summary>
    [GlobalClass]
    public partial class MainMenu : Control
    {
        private const string StageManifestPath = "res://src/stages/stage_0_start/stages_manifest.json";
        private const string StageButtonScenePath = "res://src/ui/menus/stage_button.tscn";

        private static readonly IReadOnlyDictionary<int, string> StageNameLookup = new Dictionary<int, string>
        {
            { 1, "Start Here: Ghost Terminal" },
            { 2, "Start Here: Nethack" },
            { 3, "Start Here: Amnesia: Classic RPG" },
            { 4, "Start Here: Never Go Alone: Party Selection" },
            { 5, "Start Here: Fractured Escape" },
            { 6, "Start Here: System Log Epilogue" },
        };

    private readonly ManifestLoader _manifestLoader = new();
        private PackedScene? _stageButtonScene;
        private VBoxContainer? _stageButtonList;
        private Label? _stageListPlaceholder;
        private Label? _stageHeader;
        private Button? _startButton;
        private Button? _optionsButton;
        private Button? _quitButton;

#pragma warning disable CA2213 // SceneManager is an autoload singleton managed by Godot's scene tree
        private SceneManager? _sceneManager;
#pragma warning restore CA2213

        /// <summary>
        /// Called when the node enters the scene tree for the first time.
        /// Sets up button connections dynamically from manifest.
        /// </summary>
        public override void _Ready()
        {
            base._Ready();

            _sceneManager = GetNodeOrNull<SceneManager>("/root/SceneManager");
            _stageButtonScene = ResourceLoader.Load<PackedScene>(StageButtonScenePath);

            _stageButtonList = GetNodeOrNull<VBoxContainer>("MenuContainer/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StageButtonList");
            _stageListPlaceholder = GetNodeOrNull<Label>("MenuContainer/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StageButtonList/StageListPlaceholder");
            _stageHeader = GetNodeOrNull<Label>("MenuContainer/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StageHeader");

            _startButton = GetNodeOrNull<Button>("MenuContainer/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StartButton");
            _optionsButton = GetNodeOrNull<Button>("MenuContainer/MenuContent/MenuButtonsMargin/MenuButtonsContainer/OptionsButton");
            _quitButton = GetNodeOrNull<Button>("MenuContainer/MenuContent/MenuButtonsMargin/MenuButtonsContainer/QuitButton");

            ConnectPrimaryButtons();

            var stages = _manifestLoader.LoadManifest(StageManifestPath);
            if (stages == null || !stages.Any())
            {
                GD.PrintErr("[MainMenu] Failed to load manifest. No stages available.");
                return;
            }

            ConfigureStartButton();
            PopulateStageButtons(stages);
        }

        private void ConnectPrimaryButtons()
        {
            if (_startButton != null)
            {
                _startButton.Pressed += OnStartPressed;
            }

            if (_optionsButton != null)
            {
                _optionsButton.Pressed += OnOptionsPressed;
            }

            if (_quitButton != null)
            {
                _quitButton.Pressed += OnQuitPressed;
            }
        }

        private void ConfigureStartButton()
        {
            if (_startButton == null)
            {
                return;
            }

            _startButton.Text = $"Launch {GetStageTitle(1)}";
        }

        private void PopulateStageButtons(IEnumerable<ManifestStage> stages)
        {
            if (_stageButtonList == null)
            {
                GD.PrintErr("[MainMenu] StageButtonList not found in scene hierarchy.");
                return;
            }

            if (_stageButtonScene == null)
            {
                GD.PrintErr("[MainMenu] Stage button scene failed to load.");
                return;
            }

            if (_stageListPlaceholder != null)
            {
                _stageListPlaceholder.QueueFree();
                _stageListPlaceholder = null;
            }

            int createdCount = 0;

            foreach (var stageButton in stages.OrderBy(s => s.Id).Select(CreateStageButton).Where(sb => sb != null))
            {
                _stageButtonList.AddChild(stageButton);
                createdCount++;
            }

            _stageHeader?.Text = $"Stage Access · {createdCount} module{(createdCount == 1 ? string.Empty : "s")} detected";

            GD.Print($"[MainMenu] Loaded {createdCount} stage buttons.");
        }

        private StageButton? CreateStageButton(ManifestStage stage)
        {
            if (_stageButtonScene?.Instantiate() is not StageButton stageButton)
            {
                GD.PrintErr($"[MainMenu] Failed to instantiate stage button for Stage {stage.Id}.");
                return null;
            }

            stageButton.Name = $"Stage{stage.Id}Button";
            stageButton.StageId = stage.Id.ToString();
            stageButton.Status = ResolveStageStatus(stage.Id);

            string stageTitle = GetStageTitle(stage.Id);
            stage.DisplayName = stageTitle;

            if (stageButton.GetNodeOrNull<Label>("HBox/NameLabel") is { } nameLabel)
            {
                nameLabel.Text = stageTitle;
            }

            // Use direct handler method call via Pressed event
            stageButton.Pressed += () => HandleStageButtonClicked(stage.Id.ToString());

            return stageButton;
        }

        /// <summary>
        /// Handles Start button press - starts Stage 1.
        /// </summary>
        private void OnStartPressed()
        {
            GD.Print("Start Game selected - loading Stage 1");
            OnStageSelected(1);
        }

        private void HandleStageButtonClicked(string stageIdValue)
        {
            if (!int.TryParse(stageIdValue, out int stageId))
            {
                GD.PrintErr($"[MainMenu] Invalid stage id '{stageIdValue}' emitted by stage button.");
                return;
            }

            OnStageSelected(stageId);
        }

        private static StageButton.ContentStatus ResolveStageStatus(int stageId)
        {
            return stageId switch
            {
                1 => StageButton.ContentStatus.Ready,
                2 => StageButton.ContentStatus.LLMGenerated,
                _ => StageButton.ContentStatus.Missing,
            };
        }

        private static string GetStageTitle(int stageId)
        {
            return StageNameLookup.TryGetValue(stageId, out string? name)
                ? $"Stage {stageId} · {name}"
                : $"Stage {stageId}";
        }

        /// <summary>
        /// Handles stage selection from any stage button.
        /// </summary>
        /// <param name="stageId">The numeric stage ID (1-5).</param>
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
