// <copyright file="OptionsMenuController.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts.ui.Menus
{
    /// <summary>
    /// Controls the options menu with audio, video, input, and accessibility settings.
    /// </summary>
    [GlobalClass]
    public partial class OptionsMenuController : Control
    {
        private TabContainer? tabContainer;
        private Button? applyButton;
        private Button? backButton;

        private HSlider? masterVolumeSlider;
        private HSlider? musicVolumeSlider;
        private HSlider? sfxVolumeSlider;
        private HSlider? uiVolumeSlider;

        private CheckButton? fullscreenCheckbox;
        private CheckButton? vsyncCheckbox;
        private OptionButton? resolutionDropdown;

        public override void _Ready()
        {
            this.CacheNodeReferences();
            this.ConnectSignals();
            LoadCurrentSettings();
        }

        /// <summary>
        /// Caches references to UI nodes using unique names.
        /// </summary>
        private void CacheNodeReferences()
        {
            this.tabContainer = this.GetNode<TabContainer>("%TabContainer");
            this.applyButton = this.GetNode<Button>("%ApplyButton");
            this.backButton = this.GetNode<Button>("%BackButton");

            // Audio sliders
            this.masterVolumeSlider = this.GetNode<HSlider>("%MasterVolumeSlider");
            this.musicVolumeSlider = this.GetNode<HSlider>("%MusicVolumeSlider");
            this.sfxVolumeSlider = this.GetNode<HSlider>("%SFXVolumeSlider");
            this.uiVolumeSlider = this.GetNode<HSlider>("%UIVolumeSlider");

            // Video controls
            this.fullscreenCheckbox = this.GetNode<CheckButton>("%FullscreenCheckbox");
            this.vsyncCheckbox = this.GetNode<CheckButton>("%VSyncCheckbox");
            this.resolutionDropdown = this.GetNode<OptionButton>("%ResolutionDropdown");
        }

        /// <summary>
        /// Connects button signals to their respective handlers.
        /// </summary>
        private void ConnectSignals()
        {
            if (this.applyButton is not null)
            {
                this.applyButton.Pressed += this.OnApplyPressed;
            }

            if (this.backButton is not null)
            {
                this.backButton.Pressed += this.OnBackPressed;
            }
        }

        /// <summary>
        /// Loads the current settings from configuration.
        /// </summary>
        private static void LoadCurrentSettings()
        {
            // TODO: Load settings from AppConfig
            GD.Print("Loading current settings...");
        }

        /// <summary>
        /// Applies the selected settings.
        /// </summary>
        private void OnApplyPressed()
        {
            this.ApplyAudioSettings();
            this.ApplyVideoSettings();
            ApplyInputSettings();
            GD.Print("Settings applied");
        }

        /// <summary>
        /// Applies audio settings changes.
        /// </summary>
        private void ApplyAudioSettings()
        {
            if (this.masterVolumeSlider is not null)
            {
                // TODO: Apply master volume
                GD.Print($"Master volume: {this.masterVolumeSlider.Value}");
            }
        }

        /// <summary>
        /// Applies video settings changes.
        /// </summary>
        private void ApplyVideoSettings()
        {
            if (this.fullscreenCheckbox is not null)
            {
                var window = this.GetWindow();
                window.Mode = this.fullscreenCheckbox.ButtonPressed
                    ? Window.ModeEnum.Fullscreen
                    : Window.ModeEnum.Windowed;
                GD.Print($"Fullscreen: {this.fullscreenCheckbox.ButtonPressed}");
            }
        }

        /// <summary>
        /// Applies input settings changes.
        /// </summary>
        private static void ApplyInputSettings()
        {
            // TODO: Apply input settings
            GD.Print("Input settings applied");
        }

        /// <summary>
        /// Handles back button press - closes options menu.
        /// </summary>
        private void OnBackPressed()
        {
            GD.Print("Closing options menu...");
            this.QueueFree();
        }
    }
}
