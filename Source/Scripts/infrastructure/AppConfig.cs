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
    [GlobalClass]
    public partial class AppConfig : Node
    {
        private const string ConfigFile = "user://omega_spiral_config.cfg";
        private ConfigFile configFile = null!;

        public override void _Ready()
        {
            this.configFile = new ConfigFile();
            this.LoadConfig();
            this.Name = "AppConfig";
        }

        /// <summary>
        /// Gets the main game scene path for launching gameplay.
        /// </summary>
        public static string GameScenePath => "res://Source/Scenes/GhostTerminal/Opening.tscn";

        /// <summary>
        /// Gets the opening menu scene path (bypassed for direct Ghost Terminal startup).
        /// </summary>
        public static string OpeningScenePath => "res://Source/Scenes/GhostTerminal/Opening.tscn";

        /// <summary>
        /// Gets the main menu scene path.
        /// </summary>
        public static string MainMenuScenePath => "res://addons/maaacks_game_template/base/scenes/menus/main_menu/MainMenu.tscn";

        /// <summary>
        /// Gets the main menu scene path (lowercase version for addon compatibility).
        /// </summary>
        public static string MainMenuScenePathAddon => MainMenuScenePath;

        /// <summary>
        /// Gets the ending scene path. Returns main menu as fallback if no specific ending scene exists.
        /// </summary>
        public static string EndingScenePath => MainMenuScenePath;

        /// <summary>
        /// Loads the configuration file from disk.
        /// </summary>
        private void LoadConfig()
        {
            var error = this.configFile.Load(ConfigFile);
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
            this.configFile.SetValue("audio", "master_volume", 0.8f);
            this.configFile.SetValue("audio", "music_volume", 0.7f);
            this.configFile.SetValue("audio", "sfx_volume", 0.8f);
            this.configFile.SetValue("audio", "ui_volume", 0.6f);

            // Video settings
            this.configFile.SetValue("video", "fullscreen", false);
            this.configFile.SetValue("video", "vsync", true);
            this.configFile.SetValue("video", "resolution", "1280x720");

            // Input settings
            this.configFile.SetValue("input", "mouse_sensitivity", 1.0f);
            this.configFile.SetValue("input", "gamepad_deadzone", 0.2f);

            // Accessibility
            this.configFile.SetValue("accessibility", "screen_reader_enabled", false);
            this.configFile.SetValue("accessibility", "text_size_scale", 1.0f);
        }

        /// <summary>
        /// Saves the current configuration to disk.
        /// </summary>
        private void SaveConfig()
        {
            var error = this.configFile.Save(ConfigFile);
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
            return this.configFile.GetValue(section, key, defaultValue);
        }

        /// <summary>
        /// Sets a configuration value.
        /// </summary>
        /// <param name="section">The configuration section.</param>
        /// <param name="key">The configuration key.</param>
        /// <param name="value">The value to set.</param>
        public void SetSetting(string section, string key, Variant value)
        {
            this.configFile.SetValue(section, key, value);
            this.SaveConfig();
        }
    }
}
