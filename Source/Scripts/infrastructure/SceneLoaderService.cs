// <copyright file="SceneLoader.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts.Infrastructure
{
    /// <summary>
    /// Manages scene loading with support for loading screens and transitions.
    /// Provides a consistent pattern for all scene changes throughout the game.
    /// </summary>
    [GlobalClass]
    public partial class SceneLoaderService : Node
    {
        private static SceneLoaderService? instance;
        private bool isLoading;
        private string? currentLoadingScene;

        /// <summary>
        /// Gets the singleton instance of the scene loader.
        /// </summary>
        public static SceneLoaderService Instance => instance ??= new SceneLoaderService();

        /// <summary>
        /// Gets a value indicating whether a scene is currently loading.
        /// </summary>
        public bool IsLoading => this.isLoading;

        /// <summary>
        /// Gets the path of the scene currently being loaded.
        /// </summary>
        public string? CurrentLoadingScene => this.currentLoadingScene;

        /// <summary>
        /// Loads a scene with a loading screen transition.
        /// </summary>
        /// <param name="scenePath">The file path to the scene to load (e.g., "res://Source/Scenes/Scene1Narrative.tscn").</param>
        /// <param name="showLoadingScreen">Whether to display a loading screen during the transition.</param>
        public async void LoadScene(string scenePath, bool showLoadingScreen = true)
        {
            if (this.isLoading)
            {
                GD.PrintErr("A scene is already loading. Please wait.");
                return;
            }

            if (string.IsNullOrEmpty(scenePath))
            {
                GD.PrintErr("Scene path cannot be empty.");
                return;
            }

            this.isLoading = true;
            this.currentLoadingScene = scenePath;

            try
            {
                // Show loading screen if requested
                if (showLoadingScreen)
                {
                    this.ShowLoadingScreen();
                    await this.ToSignal(this.GetTree(), SceneTree.SignalName.ProcessFrame);
                }

                // Change to the new scene
                var error = this.GetTree().ChangeSceneToFile(scenePath);
                if (error != Error.Ok)
                {
                    GD.PrintErr($"Failed to load scene {scenePath}: {error}");
                }
            }
            finally
            {
                this.isLoading = false;
                this.currentLoadingScene = null;
            }
        }

        /// <summary>
        /// Shows a loading screen overlay.
        /// </summary>
        private void ShowLoadingScreen()
        {
            GD.Print($"Loading: {this.currentLoadingScene}");
            // Loading screen logic will be implemented via scene
        }
    }
}
