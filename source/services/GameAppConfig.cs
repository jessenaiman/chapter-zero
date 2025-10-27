// <copyright file="AppConfig.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts.Infrastructure
{
    /// <summary>
    /// Application configuration singleton managing game settings and preferences.
    /// Handles audio, video, input, and accessibility settings with persistent storage.
    /// </summary>
    public partial class GameAppConfig : Node
    {
        private const string _ConfigFilePath = "user://omega_spiral_config.cfg";
        private const string _UiLayoutConfigPath = "res://source/resources/ui_layout_config.json";

        private static string? _CachedGameScenePath;
        private static string? _CachedOpeningScenePath;
        private static string? _CachedMainMenuScenePath;
        private static string? _CachedMainMenuScenePathAddon;
        private static string? _CachedEndingScenePath;

        private static float _CachedBezelMargin;
        private static float _CachedStageNameWidthRatio;
        private static float _CachedStatusLabelWidthRatio;
        private static int _CachedButtonHeight;
        private static int _CachedButtonPaddingH;
        private static int _CachedButtonPaddingV;
        private static int _CachedButtonSpacing;

        private ConfigFile _ConfigFile = null!;

        public override void _Ready()
        {
            this._ConfigFile = new ConfigFile();
            this.LoadConfig();
            this.LoadUiLayoutConfig();
            this.Name = "AppConfig";

            // Load design colors and apply to all shader materials
            DesignConfigService.ApplyDesignColorsToAllShaders(GetTree().Root);
        }

        /// <summary>
        /// Gets the main game scene path for launching gameplay.
        /// </summary>
        public static string GameScenePath => _CachedGameScenePath ?? "res://source/stages/stage_1_ghost/ghost_terminal.tscn";

        /// <summary>
        /// Gets the opening menu scene path (bypassed for direct Ghost Terminal startup).
        /// </summary>
        public static string OpeningScenePath => _CachedOpeningScenePath ?? "res://source/stages/stage_1_ghost/ghost_terminal.tscn";

        /// <summary>
        /// Gets the main menu scene path.
        /// </summary>
        public static string MainMenuScenePath => _CachedMainMenuScenePath ?? "res://source/ui/menus/main_menu.tscn";

        /// <summary>
        /// Gets the main menu scene path (lowercase version for addon compatibility).
        /// </summary>
        public static string MainMenuScenePathAddon => _CachedMainMenuScenePathAddon ?? MainMenuScenePath;

        /// <summary>
        /// Gets the ending scene path. Returns main menu as fallback if no specific ending scene exists.
        /// </summary>
        public static string EndingScenePath => _CachedEndingScenePath ?? MainMenuScenePath;

        /// <summary>
        /// Gets the game scene path (PascalCase for C# compatibility).
        /// </summary>
        public static string GameScenePathCompat => GameScenePath;

        /// <summary>
        /// Gets the main menu scene path (PascalCase for C# compatibility).
        /// </summary>
        public static string MainMenuScenePathCompat => MainMenuScenePath;

        /// <summary>
        /// Gets the ending scene path (PascalCase for C# compatibility).
        /// </summary>
        public static string EndingScenePathCompat => EndingScenePath;

        /// <inheritdoc/>
        public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
        {
            var list = base._GetPropertyList();

            void AddPathProperty(string name)
            {
                list.Add(new Godot.Collections.Dictionary
                {
                    { "name", name },
                    { "type", (int)Variant.Type.String },
                    { "usage", (int)PropertyUsageFlags.Default }
                });
            }

            AddPathProperty("game_scene_path");
            AddPathProperty("opening_scene_path");
            AddPathProperty("main_menu_scene_path");
            AddPathProperty("main_menu_scene_path_addon");
            AddPathProperty("ending_scene_path");

            return list;
        }

        /// <inheritdoc/>
        public override Variant _Get(StringName property)
        {
            return property.ToString() switch
            {
                "game_scene_path" => GameScenePath,
                "opening_scene_path" => OpeningScenePath,
                "main_menu_scene_path" => MainMenuScenePath,
                "main_menu_scene_path_addon" => MainMenuScenePathAddon,
                "ending_scene_path" => EndingScenePath,
                _ => base._Get(property),
            };
        }

        /// <inheritdoc/>
        public override bool _Set(StringName property, Variant value)
        {
            switch (property.ToString())
            {
                case "game_scene_path":
                    _CachedGameScenePath = value.AsString();
                    return true;
                case "opening_scene_path":
                    _CachedOpeningScenePath = value.AsString();
                    return true;
                case "main_menu_scene_path":
                    _CachedMainMenuScenePath = value.AsString();
                    return true;
                case "main_menu_scene_path_addon":
                    _CachedMainMenuScenePathAddon = value.AsString();
                    return true;
                case "ending_scene_path":
                    _CachedEndingScenePath = value.AsString();
                    return true;
                default:
                    return base._Set(property, value);
            }
        }

        /// <summary>
        /// Loads the configuration file from disk.
        /// </summary>
        private void LoadConfig()
        {
            var error = this._ConfigFile.Load(_ConfigFilePath);
            if (error != Error.Ok)
            {
                GD.Print("Creating new config file.");
                this.CreateDefaultConfig();
                this.SaveConfig();
            }
        }

        /// <summary>
        /// Loads UI layout configuration from the JSON file.
        /// </summary>
        private void LoadUiLayoutConfig()
        {
            if (!ResourceLoader.Exists(_UiLayoutConfigPath))
            {
                GD.PrintErr($"Failed to find UI layout config: {_UiLayoutConfigPath}");
                return;
            }

            try
            {
                var jsonString = Godot.FileAccess.GetFileAsString(_UiLayoutConfigPath);
                var json = new Json();
                if (json.Parse(jsonString) != Error.Ok)
                {
                    GD.PrintErr($"Failed to parse UI layout config JSON: {json.GetErrorMessage()}");
                    return;
                }

                var root = (Godot.Collections.Dictionary<string, Variant>)json.Data;
                if (!root.ContainsKey("ui_layout"))
                {
                    GD.PrintErr("UI layout config missing 'ui_layout' section");
                    return;
                }

                var uiLayout = (Godot.Collections.Dictionary<string, Variant>)root["ui_layout"];
                _CachedBezelMargin = (float)uiLayout["bezel_margin"];
                _CachedStageNameWidthRatio = (float)uiLayout["stage_name_width"];
                _CachedStatusLabelWidthRatio = (float)uiLayout["status_label_width"];
                _CachedButtonHeight = (int)uiLayout["button_height"];
                _CachedButtonPaddingH = (int)uiLayout["button_padding_h"];
                _CachedButtonPaddingV = (int)uiLayout["button_padding_v"];
                _CachedButtonSpacing = (int)uiLayout["button_spacing"];

                GD.Print("[GameAppConfig] UI layout config loaded successfully");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[GameAppConfig] Error loading UI layout config: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates default configuration settings.
        /// </summary>
        /// <summary>
        /// Creates default configuration settings.
        /// </summary>
        private void CreateDefaultConfig()
        {
            // Audio settings
            this._ConfigFile.SetValue("audio", "master_volume", 0.8f);
            this._ConfigFile.SetValue("audio", "music_volume", 0.7f);
            this._ConfigFile.SetValue("audio", "sfx_volume", 0.8f);
            this._ConfigFile.SetValue("audio", "ui_volume", 0.6f);

            // Video settings
            this._ConfigFile.SetValue("video", "fullscreen", false);
            this._ConfigFile.SetValue("video", "vsync", true);
            this._ConfigFile.SetValue("video", "resolution", "1280x720");

            // Input settings
            this._ConfigFile.SetValue("input", "mouse_sensitivity", 1.0f);
            this._ConfigFile.SetValue("input", "gamepad_deadzone", 0.2f);

            // Accessibility
            this._ConfigFile.SetValue("accessibility", "screen_reader_enabled", false);
            this._ConfigFile.SetValue("accessibility", "text_size_scale", 1.0f);

            // UI Layout settings are loaded from source/resources/ui_layout_config.json, not hardcoded here
        }

        /// <summary>
        /// Saves the current configuration to disk.
        /// </summary>
        private void SaveConfig()
        {
            var error = this._ConfigFile.Save(_ConfigFilePath);
            if (error != Error.Ok)
            {
                GD.PrintErr($"Failed to save config: {error}");
            }
        }

        /// <summary>
        /// Gets a configuration value.
        /// </summary>
        /// <param name="section">The configuration section.</param>
        /// <param name="key">The configuration key.</param>
        /// <param name="defaultValue">The default value if not found.</param>
        /// <returns>The configuration value or default.</returns>
        public Variant GetSetting(string section, string key, Variant defaultValue)
        {
            return this._ConfigFile.GetValue(section, key, defaultValue);
        }

        /// <summary>
        /// Sets a configuration value.
        /// </summary>
        /// <param name="section">The configuration section.</param>
        /// <param name="key">The configuration key.</param>
        /// <param name="value">The value to set.</param>
        public void SetSetting(string section, string key, Variant value)
        {
            this._ConfigFile.SetValue(section, key, value);
            this.SaveConfig();
        }

        /// <summary>
        /// Gets a UI layout setting value.
        /// </summary>
        /// <param name="key">The layout key (e.g., "bezel_margin", "stage_name_width").</param>
        /// <param name="defaultValue">The default value if not found.</param>
        /// <returns>The UI layout value or default.</returns>
        public Variant GetUiLayoutSetting(string key, Variant defaultValue)
        {
            return defaultValue;  // Values are now loaded from JSON config file
        }

        /// <summary>
        /// Gets the bezel margin as a fraction (0.0 to 1.0).
        /// </summary>
        public float BezelMargin => _CachedBezelMargin;

        /// <summary>
        /// Gets the stage name label width as a fraction of available width (0.0 to 1.0).
        /// </summary>
        public float StageNameWidthRatio => _CachedStageNameWidthRatio;

        /// <summary>
        /// Gets the status label width as a fraction of available width (0.0 to 1.0).
        /// </summary>
        public float StatusLabelWidthRatio => _CachedStatusLabelWidthRatio;

        /// <summary>
        /// Gets the button height in pixels.
        /// </summary>
        public int ButtonHeight => _CachedButtonHeight;

        /// <summary>
        /// Gets the horizontal button padding in pixels.
        /// </summary>
        public int ButtonPaddingH => _CachedButtonPaddingH;

        /// <summary>
        /// Gets the vertical button padding in pixels.
        /// </summary>
        public int ButtonPaddingV => _CachedButtonPaddingV;

        /// <summary>
        /// Gets the button spacing in pixels.
        /// </summary>
        public int ButtonSpacing => _CachedButtonSpacing;
    }
}
