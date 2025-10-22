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
            var startButton = GetNode<Button>("MenuContainer/MenuContent/MenuButtonsMargin/MenuButtonsContainer/MenuButtonsBoxContainer/StartButton");
            var stage1Button = GetNode<Button>("MenuContainer/MenuContent/MenuButtonsMargin/MenuButtonsContainer/MenuButtonsBoxContainer/Stage1Button");
            var stage2Button = GetNode<Button>("MenuContainer/MenuContent/MenuButtonsMargin/MenuButtonsContainer/MenuButtonsBoxContainer/Stage2Button");
            var stage3Button = GetNode<Button>("MenuContainer/MenuContent/MenuButtonsMargin/MenuButtonsContainer/MenuButtonsBoxContainer/Stage3Button");
            var stage4Button = GetNode<Button>("MenuContainer/MenuContent/MenuButtonsMargin/MenuButtonsContainer/MenuButtonsBoxContainer/Stage4Button");
            var stage5Button = GetNode<Button>("MenuContainer/MenuContent/MenuButtonsMargin/MenuButtonsContainer/MenuButtonsBoxContainer/Stage5Button");
            var optionsButton = GetNode<Button>("MenuContainer/MenuContent/MenuButtonsMargin/MenuButtonsContainer/MenuButtonsBoxContainer/OptionsButton");
            var quitButton = GetNode<Button>("MenuContainer/MenuContent/MenuButtonsMargin/MenuButtonsContainer/MenuButtonsBoxContainer/QuitButton");

            startButton.Pressed += OnStartPressed;
            stage1Button.Pressed += OnStage1Pressed;
            stage2Button.Pressed += OnStage2Pressed;
            stage3Button.Pressed += OnStage3Pressed;
            stage4Button.Pressed += OnStage4Pressed;
            stage5Button.Pressed += OnStage5Pressed;
            optionsButton.Pressed += OnOptionsPressed;
            quitButton.Pressed += OnQuitPressed;

            GD.Print("Main Menu loaded and centered.");
        }

        /// <summary>
        /// Handles Start button press.
        /// </summary>
        private void OnStartPressed()
        {
            GD.Print("Start Game selected - loading Stage 1");
            TransitionToStage("res://source/stages/ghost/scenes/boot_sequence.tscn");
        }

        /// <summary>
        /// Public helper to start stage 1 (exposed for tests).
        /// </summary>
        public void StartStage1()
        {
            TransitionToStage("res://source/stages/ghost/scenes/boot_sequence.tscn");
        }

        /// <summary>
        /// Handles Stage 1 button press.
        /// </summary>
        private void OnStage1Pressed()
        {
            GD.Print("Stage 1 selected - loading Opening sequence");
            TransitionToStage("res://source/stages/ghost/scenes/boot_sequence.tscn");
        }

        /// <summary>
        /// Handles Stage 2 button press.
        /// </summary>
        private void OnStage2Pressed()
        {
            GD.Print("Stage 2 selected - loading Echo Chamber");
            TransitionToStage("res://source/stages/stage_2/echo_hub.tscn");
        }

        /// <summary>
        /// Handles Stage 3 button press.
        /// </summary>
        private void OnStage3Pressed()
        {
            GD.Print("Stage 3 selected - loading Never Go Alone");
            TransitionToStage("res://source/stages/stage_3/echo_vault_hub.tscn");
        }

        /// <summary>
        /// Handles Stage 4 button press.
        /// </summary>
        private void OnStage4Pressed()
        {
            GD.Print("Stage 4 selected - loading Liminal Township");
            TransitionToStage("res://source/stages/stage_4/stage_4_main.tscn");
        }

        /// <summary>
        /// Handles Stage 5 button press.
        /// </summary>
        private void OnStage5Pressed()
        {
            GD.Print("Stage 5 selected - loading Final Convergence");
            TransitionToStage("res://source/stages/stage_5/stage5.tscn");
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

        /// <summary>
        /// Transitions to the specified stage scene using SceneManager.
        /// </summary>
        /// <param name="scenePath">The path to the scene to transition to.</param>
        private void TransitionToStage(string scenePath)
        {
            GD.Print($"Transitioning to scene: {scenePath}");
            var sceneManager = GetNodeOrNull<SceneManager>("/root/SceneManager");
            if (sceneManager != null)
            {
                sceneManager.TransitionToScene(scenePath);
            }
            else
            {
                GD.PrintErr("SceneManager not found! Using direct scene change.");
                var nextScene = GD.Load<PackedScene>(scenePath);
                if (nextScene != null)
                {
                    GetTree().ChangeSceneToPacked(nextScene);
                }
            }
        }
    }
}
