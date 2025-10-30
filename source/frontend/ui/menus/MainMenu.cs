using Godot;
using OmegaSpiral.Source.Backend;
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

#pragma warning disable CA2213 // SceneManager is an autoload singleton managed by Godot's scene tree
        private SceneManager? _StageManager;
#pragma warning restore CA2213

        private GameManager? _GameManager;

        /// <summary>
        /// Populates the GameManager's StageScenes array from the manifest.
        /// This ensures GameManager has the correct stage scenes for navigation.
        /// </summary>
        private void PopulateGameManagerStages()
        {
            var stages = _ManifestLoader.LoadManifest(StageManifestPath);
            if (stages.Count == 0)
            {
                GD.PrintErr("[MainMenu] Cannot populate GameManager stages: manifest is empty.");
                return;
            }

            // Sort stages by ID and load their PackedScenes
            var stageScenes = new System.Collections.Generic.List<PackedScene>();
            foreach (var stage in stages.OrderBy(s => s.Id))
            {
                if (ResourceLoader.Exists(stage.Path))
                {
                    var scene = GD.Load<PackedScene>(stage.Path);
                    if (scene != null)
                    {
                        stageScenes.Add(scene);
                        GD.Print($"[MainMenu] Loaded stage {stage.Id}: {stage.DisplayName}");
                    }
                    else
                    {
                        GD.PrintErr($"[MainMenu] Failed to load scene: {stage.Path}");
                    }
                }
                else
                {
                    GD.PrintErr($"[MainMenu] Stage scene does not exist: {stage.Path}");
                }
            }

            _GameManager!.StageScenes = stageScenes.ToArray();
            GD.Print($"[MainMenu] Populated GameManager with {stageScenes.Count} stage scenes.");
        }

        // Button references created dynamically
        private Button? _StartButton;
        private Button? _OptionsButton;
        private Button? _QuitButton;

        /// <summary>
        /// Creates the required menu structure nodes in the OmegaFrame.
        /// Must be called before base._Ready() to ensure MenuUi can find the nodes.
        /// </summary>
        private void CreateMenuStructure()
        {
            // Get the ContentContainer from the OmegaFrame
            var contentContainer = GetNodeOrNull<VBoxContainer>("OmegaFrame/CrtFrame/ContentContainer");
            if (contentContainer == null)
            {
                GD.PrintErr("[MainMenu] ContentContainer not found in OmegaFrame!");
                return;
            }

            // Create MenuTitle label
            var menuTitle = new Label
            {
                Name = "MenuTitle",
                Text = "",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                SizeFlagsHorizontal = Control.SizeFlags.Fill,
                SizeFlagsVertical = Control.SizeFlags.ShrinkCenter
            };
            contentContainer.AddChild(menuTitle);

            // Create MenuButtonContainer (use the ContentContainer itself for buttons)
            // The MenuUi expects MenuButtonContainer to be a separate node, so create one
            var menuButtonContainer = new VBoxContainer
            {
                Name = "MenuButtonContainer",
                SizeFlagsHorizontal = Control.SizeFlags.Fill,
                SizeFlagsVertical = Control.SizeFlags.ExpandFill
            };
            contentContainer.AddChild(menuButtonContainer);

            // Create MenuActionBar
            var menuActionBar = new HBoxContainer
            {
                Name = "MenuActionBar",
                Alignment = BoxContainer.AlignmentMode.Center,
                SizeFlagsHorizontal = Control.SizeFlags.Fill,
                SizeFlagsVertical = Control.SizeFlags.ShrinkCenter
            };
            contentContainer.AddChild(menuActionBar);
        }

        /// <summary>
        /// Called when the node enters the scene tree for the first time.
        /// Initializes the menu and sets the title.
        /// Button population is handled by PopulateMenuButtons().
        /// </summary>
        public override void _Ready()
        {
            // Create the required menu structure before base initialization
            CreateMenuStructure();

            base._Ready(); // MenuBase calls PopulateMenuButtons() after initialization

            _StageManager = GetNodeOrNull<SceneManager>("/root/SceneManager");
            _GameManager = GetNodeOrNull<GameManager>("/root/GameManager");

            // Populate GameManager's StageScenes from manifest
            if (_GameManager != null)
            {
                PopulateGameManagerStages();
            }

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

            var buttonContainer = MenuButtonContainer;

            if (buttonContainer == null)
            {
                GD.PrintErr("[MainMenu] MenuButtonContainer not found!");
                return;
            }

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
            var buttonContainer = MenuButtonContainer;

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
                 /// Starts the game at the selected stage using GameManager.
                 /// </summary>
                 /// <param name="stageId">The numeric stage ID to start at (1-based).</param>
        private async void OnStageSelected(int stageId)
        {
            if (_GameManager == null)
            {
                GD.PrintErr("[MainMenu] GameManager not found! Cannot start stage.");
                return;
            }

            var stage = _ManifestLoader.GetStage(stageId);
            if (stage == null)
            {
                GD.PrintErr($"[MainMenu] Stage {stageId} not found in manifest.");
                return;
            }

            GD.Print($"[MainMenu] Starting game at {stage.DisplayName} (Stage {stageId})");

            // Convert 1-based stage ID to 0-based index
            int stageIndex = stageId - 1;
            await _GameManager.StartGameAtStageAsync(stageIndex).ConfigureAwait(false);
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
