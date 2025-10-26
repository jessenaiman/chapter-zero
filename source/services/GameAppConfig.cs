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

        private static string? _CachedGameScenePath;
        private static string? _CachedOpeningScenePath;
        private static string? _CachedMainMenuScenePath;
        private static string? _CachedMainMenuScenePathAddon;
        private static string? _CachedEndingScenePath;

        private ConfigFile _ConfigFile = null!;

        public override void _Ready()
        {
            this._ConfigFile = new ConfigFile();
            this.LoadConfig();
            this.Name = "AppConfig";
        }

        /// <summary>
        /// Gets the main game scene path for launching gameplay.
        /// </summary>
        public static string GameScenePath => _CachedGameScenePath ?? "res://source/stages/stage_1/opening.tscn";

        /// <summary>
        /// Gets the opening menu scene path (bypassed for direct Ghost Terminal startup).
        /// </summary>
        public static string OpeningScenePath => _CachedOpeningScenePath ?? "res://source/stages/stage_1/opening.tscn";

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
    }
}
