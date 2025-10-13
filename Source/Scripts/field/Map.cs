// <copyright file="Map.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// The map defines the properties of the playable grid, which will be applied on _ready to the
/// <see cref="Gameboard"/>. These properties usually correspond to one or multiple tilesets.
/// </summary>
[Tool]
public partial class Map : Node2D
{
    private GameboardProperties? gameboardProperties;

    /// <summary>
    /// Gets or sets the map defines the properties of the playable grid, which will be applied on _ready to the
    /// <see cref="Gameboard"/>. These properties usually correspond to one or multiple tilesets.
    /// </summary>
    [Export]
    public GameboardProperties GameboardProperties
    {
        get => this.gameboardProperties;
        set
        {
            this.gameboardProperties = value;

            if (!this.IsInsideTree())
            {
                // Wait for the node to be ready before accessing children
                this.CallDeferred("_SetGameboardPropertiesDeferred", value);
                return;
            }

            _debugBoundaries.GameboardProperties = this.gameboardProperties;
        }
    }

    private static void SetGameboardPropertiesDeferred(GameboardProperties value)
    {
        // _debugBoundaries.GameboardProperties = value;
    }

    // private DebugGameboardBoundaries _debugBoundaries;
    /// <inheritdoc/>
    public override void _Ready()
    {
        // _debugBoundaries = GetNode<DebugGameboardBoundaries>("Overlay/DebugBoundaries");
        if (!Engine.IsEditorHint())
        {
            Camera.GameboardProperties = this.GameboardProperties;
            Gameboard.Properties = this.GameboardProperties;

            // Gamepieces need to be registered according to which cells they currently occupy.
            // Gamepieces may not overlap, and only the first gamepiece registered to a given cell will
            // be kept.
            // foreach (Gamepiece gamepiece in FindChildren("*", "Gamepiece"))
            // {
            //    Vector2I cell = Gameboard.GetCellUnderNode(gamepiece);
            //    gamepiece.Position = Gameboard.CellToPixel(cell);
            //
            //    if (GamepieceRegistry.Register(gamepiece, cell) == false)
            //    {
            //        gamepiece.QueueFree();
            //    }
            // }
        }
    }
}
