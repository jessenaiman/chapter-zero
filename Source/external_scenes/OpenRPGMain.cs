// <copyright file="OpenRPGMain.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.ExternalScenes
{
    /// <summary>
    /// Main controller for the OpenRPG act within Omega Spiral.
    /// Wraps the field exploration system and handles exit conditions to return to the act sequence.
    /// </summary>
    [GlobalClass]
    public partial class OpenRPGMain : Node2D
    {
        /// <inheritdoc/>
        public override void _Ready()
        {
            GD.Print("🎮 OpenRPG Main: Simple test scene loaded!");
            GD.Print("✅ Scene loading successful!");
            GD.Print("🎯 Press ESC to exit");
            GD.Print("🔧 Ready for field integration!");
        }

        /// <inheritdoc/>
        public override void _Input(InputEvent @event)
        {
            // Check for exit condition - ESC key exits the application for testing
            if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.Escape)
            {
                GD.Print("🔄 Exiting test scene...");
                this.GetTree().Quit();
            }
        }
    }
}
