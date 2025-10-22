using Godot;
using OmegaSpiral.Source.Scripts.Infrastructure;

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
        private ManifestLoader _manifestLoader = new();
#pragma warning disable CA2213 // SceneManager is an autoload singleton, not owned by this node
        private SceneManager? _sceneManager;
#pragma warning restore CA2213

        /// <summary>
        /// Called when the node enters the scene tree for the first time.
        /// Sets up button connections dynamically from manifest.
        /// </summary>
        public override void _Ready()
        {
            _sceneManager = GetNodeOrNull<SceneManager>("/root/SceneManager");

            // Load manifest
            var stages = _manifestLoader.LoadManifest("res://source/data/manifest.json");

            if (stages.Count == 0)
            {
                GD.PrintErr("[MainMenu] Failed to load manifest. No stages available.");
                return;
            }

            // Get the main button container
            var menuButtonsContainer = GetNodeOrNull<Container>(
                "TerminalWindow/Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/TerminalBody/ScreenPadding/ScreenLayout/MenuContent/MenuButtonsMargin/MenuButtonsContainer/MenuButtonsBoxContainer");

            if (menuButtonsContainer == null)
            {
                GD.PrintErr("[MainMenu] MenuButtonsBoxContainer not found in scene hierarchy.");
                return;
            }

            // Connect existing buttons
            var startButton = GetNodeOrNull<Button>(
                "TerminalWindow/Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/TerminalBody/ScreenPadding/ScreenLayout/MenuContent/MenuButtonsMargin/MenuButtonsContainer/MenuButtonsBoxContainer/StartButton");
            if (startButton != null)
            {
                startButton.Pressed += OnStartPressed;
            }

            // Connect stage buttons
            foreach (var stage in stages)
            {
                ConnectStageButton(stage);
            }

            // Connect utility buttons
            var optionsButton = GetNodeOrNull<Button>(
                "TerminalWindow/Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/TerminalBody/ScreenPadding/ScreenLayout/MenuContent/MenuButtonsMargin/MenuButtonsContainer/MenuButtonsBoxContainer/OptionsButton");
            if (optionsButton != null)
            {
                optionsButton.Pressed += OnOptionsPressed;
            }

            var quitButton = GetNodeOrNull<Button>(
                "TerminalWindow/Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/TerminalBody/ScreenPadding/ScreenLayout/MenuContent/MenuButtonsMargin/MenuButtonsContainer/MenuButtonsBoxContainer/QuitButton");
            if (quitButton != null)
            {
                quitButton.Pressed += OnQuitPressed;
            }

            GD.Print("Main Menu loaded with dynamic stage buttons.");
        }

        /// <summary>
        /// Connects a stage button to its handler.
        /// Buttons are expected to be named Stage{Id}Button (e.g., Stage1Button, Stage2Button).
        /// </summary>
        private void ConnectStageButton(ManifestStage stage)
        {
            var buttonName = $"Stage{stage.Id}Button";
            var stageButton = GetNodeOrNull<Button>(
                $"TerminalWindow/Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/TerminalBody/ScreenPadding/ScreenLayout/MenuContent/MenuButtonsMargin/MenuButtonsContainer/MenuButtonsBoxContainer/{buttonName}");

            if (stageButton != null)
            {
                // Capture stage ID in closure
                int stageId = stage.Id;
                stageButton.Pressed += () => OnStageSelected(stageId);
                GD.Print($"[MainMenu] Connected {buttonName} to Stage {stageId}");
            }
            else
            {
                GD.PrintErr($"[MainMenu] Button not found: {buttonName}");
            }
        }

        /// <summary>
        /// Handles Start button press - starts Stage 1.
        /// </summary>
        private void OnStartPressed()
        {
            GD.Print("Start Game selected - loading Stage 1");
            OnStageSelected(1);
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

            var sceneFlowPath = _manifestLoader.GetSceneFlowPath(stageId);
            if (string.IsNullOrEmpty(sceneFlowPath))
            {
                GD.PrintErr($"[MainMenu] No scene flow configured for Stage {stageId}.");
                return;
            }

            GD.Print($"[MainMenu] Starting {stage.DisplayName} - Scene flow: {sceneFlowPath}");

            // For now, transition to the first scene of the stage
            // TODO: Initialize SceneNavigator with the stage's scene flow
            TransitionToStage(stageId);
        }

        /// <summary>
        /// Transitions to a stage using the first scene of that stage.
        /// For Stage 1, loads the manifest to get the first beat.
        /// </summary>
        private void TransitionToStage(int stageId)
        {
            string? scenePath = null;

            // For Stage 1, load the stage_manifest.json to get the first beat
            if (stageId == 1)
            {
                var stageManifestLoader = new StageManifestLoader();
                var stageManifest = stageManifestLoader.LoadManifest("res://source/stages/stage_1/stage_manifest.json");

                if (stageManifest != null && stageManifest.Scenes.Count > 0)
                {
                    var firstBeat = stageManifest.GetFirstScene();
                    if (firstBeat != null)
                    {
                        scenePath = firstBeat.SceneFile;
                        GD.Print($"[MainMenu] Stage 1 first beat: {scenePath}");
                    }
                }
                else
                {
                    GD.PrintErr("[MainMenu] Failed to load Stage 1 manifest. Falling back to default.");
                }
            }

            // Fallback to hardcoded paths for other stages
            if (string.IsNullOrEmpty(scenePath))
            {
                var firstSceneMap = new System.Collections.Generic.Dictionary<int, string>
                {
                    { 1, "res://source/stages/stage_1/beats/boot_sequence.tscn" },
                    { 2, "res://source/stages/stage_2/echo_hub.tscn" },
                    { 3, "res://source/stages/stage_3/echo_vault_hub.tscn" },
                    { 4, "res://source/stages/stage_4/stage_4_main.tscn" },
                    { 5, "res://source/stages/stage_5/stage5.tscn" },
                };

                if (!firstSceneMap.TryGetValue(stageId, out scenePath))
                {
                    GD.PrintErr($"[MainMenu] No entry scene configured for Stage {stageId}.");
                    return;
                }
            }

            GD.Print($"[MainMenu] Transitioning to Stage {stageId}: {scenePath}");

            if (_sceneManager != null)
            {
                _sceneManager.TransitionToScene(scenePath);
            }
            else
            {
                GD.PrintErr("[MainMenu] SceneManager not found! Using fallback scene change.");
                var nextScene = GD.Load<PackedScene>(scenePath);
                if (nextScene != null)
                {
                    GetTree().ChangeSceneToPacked(nextScene);
                }
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
