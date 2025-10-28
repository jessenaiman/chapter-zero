using System.Collections.Generic;
using System.Linq;
using Godot;
using OmegaSpiral.Source.Scripts.Infrastructure;
using OmegaSpiral.Source.Ui.Menus;
using OmegaSpiral.Source.Ui.Omega;

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
    /// Initializes a new instance of the <see cref="MainMenu"/> class.
    /// Sets up correct node paths for OmegaFrame structure.
    /// </summary>
    public MainMenu()
    {
        // Override default MenuUi paths to match OmegaFrame structure
        MenuTitlePath = "OmegaFrame/CrtFrame/ContentContainer/MenuTitle";
        MenuButtonContainerPath = "OmegaFrame/CrtFrame/ContentContainer/MenuButtonContainer";
        MenuActionBarPath = "OmegaFrame/CrtFrame/ContentContainer/MenuActionBar";
    }

    // --- PRIVATE FIELDS ---

    private readonly ManifestLoader _ManifestLoader = new();

#pragma warning disable CA2213 // StageManager is an autoload singleton managed by Godot's scene tree
    private StageManager? _StageManager;
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

            _StageManager = GetNodeOrNull<StageManager>("/root/StageManager");
            SetMenuTitle("Ωmega Spiral");

            // Set title with bold single color and subtle shadow effect
            var titleLabel = GetNodeOrNull<Label>("OmegaFrame/CrtFrame/ContentContainer/MenuTitle");
            if (titleLabel != null)
            {
                // Apply Orbitron Black font for maximum impact
                var orbitronFont = GD.Load<Font>("res://source/assets/gui/font/orbitron_title.tres");
                if (orbitronFont != null)
                {
                    titleLabel.AddThemeFontOverride("font", orbitronFont);
                    titleLabel.AddThemeFontSizeOverride("font_size", 48); // Much larger
                }

                // Apply shadow shader effect (no animation, just depth)
                var shadowShader = GD.Load<Shader>("res://source/shaders/title_gradient.gdshader");
                if (shadowShader != null)
                {
                    var shaderMaterial = new ShaderMaterial { Shader = shadowShader };

                    // Set primary color to Ambition (Crimson Red) - bold single color
                    shaderMaterial.SetShaderParameter("color_primary", OmegaSpiralColors.AmbitionThread);

                    // Shadow color for depth effect
                    shaderMaterial.SetShaderParameter("color_shadow", Colors.Black);

                    // Subtle shadow offset for 3D depth
                    shaderMaterial.SetShaderParameter("shadow_offset", 0.02f);

                    titleLabel.Material = shaderMaterial;
                }
            }
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
        /// Uses OmegaUiButton for unified visual consistency across all buttons.
        /// </summary>
        private void PopulateStageList()
        {
            var stages = _ManifestLoader.LoadManifest(StageManifestPath);

            if (stages.Count == 0)
            {
                GD.PrintErr("[MainMenu] Stage manifest is empty.");
                return;
            }

            var buttonContainer = GetNode<VBoxContainer>("OmegaFrame/CrtFrame/ContentContainer/MenuButtonContainer");

            // Create "Start Here" button for first stage
            var firstStage = stages.OrderBy(s => s.Id).FirstOrDefault();
            if (firstStage != null)
            {
                var startButton = new OmegaUiButton
                {
                    Name = "StartButton",
                    Text = $"Start Here - {firstStage.DisplayName}",
                    FocusMode = Control.FocusModeEnum.All,
                    SizeFlagsHorizontal = Control.SizeFlags.Fill,
                    SizeFlagsVertical = Control.SizeFlags.ShrinkCenter
                };
                startButton.Pressed += () => OnStageSelected(firstStage.Id);
                buttonContainer.AddChild(startButton);
                _StartButton = startButton;
            }

            // Create stage selection buttons (stages 2-6)
            int createdCount = 0;
            foreach (var stage in stages.OrderBy(s => s.Id).Skip(1))
            {
                var stageButton = new OmegaUiButton
                {
                    Name = $"Stage{stage.Id}Button",
                    Text = $"Stage {stage.Id} - {stage.DisplayName}",
                    FocusMode = Control.FocusModeEnum.All,
                    SizeFlagsHorizontal = Control.SizeFlags.Fill,
                    SizeFlagsVertical = Control.SizeFlags.ShrinkCenter
                };
                stageButton.Pressed += () => OnStageSelected(stage.Id);
                buttonContainer.AddChild(stageButton);
                createdCount++;
            }

            GD.Print($"[MainMenu] Loaded {createdCount} stage buttons from manifest.");
        }

        /// <summary>
        /// Adds main menu actions by creating option and quit buttons.
        /// </summary>
        private void AddMainMenuActions()
        {
            // Get the menu button container (stage buttons + options/quit all go here)
            var buttonContainer = GetNodeOrNull<VBoxContainer>("ContentContainer/MenuButtonContainer");

            if (buttonContainer != null)
            {
                _OptionsButton = new OmegaUiButton
                {
                    Name = "OptionsButton",
                    Text = "Options",
                    FocusMode = Control.FocusModeEnum.All,
                    SizeFlagsHorizontal = Control.SizeFlags.Fill,
                    SizeFlagsVertical = Control.SizeFlags.ShrinkCenter
                };
                _OptionsButton.Pressed += OnOptionsPressed;
                buttonContainer.AddChild(_OptionsButton);

                _QuitButton = new OmegaUiButton
                {
                    Name = "QuitButton",
                    Text = "Quit Game",
                    FocusMode = Control.FocusModeEnum.All,
                    SizeFlagsHorizontal = Control.SizeFlags.Fill,
                    SizeFlagsVertical = Control.SizeFlags.ShrinkCenter
                };
                _QuitButton.Pressed += OnQuitPressed;
                buttonContainer.AddChild(_QuitButton);
            }
            else
            {
                GD.PrintErr("[MainMenu] MenuButtonContainer not found - cannot add Options/Quit buttons");
            }

            FocusFirstButton(); // Enable keyboard/gamepad navigation
        }        /// <summary>
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

            // Use StageManager for scene transitions if available
            if (_StageManager != null)
            {
                _StageManager.TransitionToScene(entryScenePath, showLoadingScreen: false);
                return;
            }

            // Fallback to direct scene change
            GD.PrintErr("[MainMenu] StageManager not found! Using fallback scene change.");
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
