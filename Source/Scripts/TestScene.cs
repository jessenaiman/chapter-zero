// <copyright file="TestScene.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

/// <summary>
/// Simple test scene to verify the game loads properly.
/// </summary>
public partial class TestScene : Node2D
{
    /// <inheritdoc/>
    public override void _Ready()
    {
        GD.Print("TestScene loaded successfully!");
        GD.Print("Godot version: ", Engine.GetVersionInfo()["string"]);
        GD.Print("C# scripting enabled");
    }

    /// <inheritdoc/>
    public override void _Process(double delta)
    {
        // Optional: Add any per-frame updates here
    }
}
