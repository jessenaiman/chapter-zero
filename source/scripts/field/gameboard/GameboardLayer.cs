
// <copyright file="GameboardLayer.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Godot;

namespace OmegaSpiral.Source.Scripts.Field.gameboard;
/// <summary>
/// A single layer of the gameboard that determines which cells are blocked or clear.
/// The Gameboard's state (where Gamepieces may or may not move) is composed from a number of
/// GameboardLayers. These layers determine which cells are blocked or clear.
/// The layers register themselves to the Gameboard in _Ready.
/// </summary>
[GlobalClass]
[Tool]
public partial class GameboardLayer : TileMapLayer
{
    /// <summary>
    /// The name of the node group that will contain all GameboardLayers.
    /// </summary>
    public const string Group = "_GAMEBOARD_LAYER_GROUP";

    /// <summary>
    /// The custom data layer that will be used to determine if a cell blocks movement.
    /// </summary>
    public const string BlockedCellDataLayer = "blocked";

    /// <summary>
    /// Emitted whenever the layer's cells change the gameboard state.
    /// This occurs when the map is added or removed from the scene tree, or when its list of
    /// moveable cells changes.
    /// Compare the changed cells with those already in the pathfinder. Any changes will cause the
    /// Pathfinder to be updated.
    /// </summary>
    /// <param name="clearedCells">The cells that are now clear for movement.</param>
    /// <param name="blockedCells">The cells that are now blocked from movement.</param>
    [Signal]
    public delegate void CellsChangedEventHandler(Godot.Collections.Array clearedCells, Godot.Collections.Array blockedCells);

    /// <summary>
    /// Gets a list of cells that can be moved to. Only cells that exist in the layer and are not blocked
    /// will be included.
    /// </summary>
    public List<Vector2I> MoveableCells { get; private set; } = new List<Vector2I>();

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        if (!Engine.IsEditorHint())
        {
            this.AddToGroup(Group);

            // When the GameboardLayer enters or exits the tree, update the Gameboard with the cells
            // that have been added or removed.
            this.TreeEntered += this.OnTreeEntered;
            this.TreeExited += this.OnTreeExited;

            // Update the list of moveable cells whenever the layer's tiles change.
            this.Changed += this.OnChanged;
        }
    }

    /// <summary>
    /// Checks all TileMapLayers in the Group to see if the cell is clear
    /// (returns true) or blocked (returns false).
    ///
    /// A clear cell must fulfill two criteria:
    ///
    /// - Exists in at least one of the GameboardLayers.
    /// - None of the layers block movement at this cell, as defined by the
    /// BlockedCellDataLayer custom data layer (see
    /// TileData.GetCustomData).
    /// </summary>
    /// <param name="coord">The coordinate to check.</param>
    /// <returns>True if the cell is clear for movement, false otherwise.</returns>
    public bool IsCellClear(Vector2I coord)
    {
        // Check to make sure that cell exists.
        var cellExists = false;

        var tilemaps = this.GetTree().GetNodesInGroup(Group);
        foreach (var node in tilemaps)
        {
            if (node is GameboardLayer tilemap)
            {
                if (tilemap != null && tilemap.GetUsedCells().Contains(coord))
                {
                    cellExists = true;

                    // Check if this layer blocks movement at this cell
                    if (tilemap.IsCellBlocked(coord))
                    {
                        return false;
                    }
                }
            }
        }

        // There is no terrain blocking cell movement. However we only want to allow movement if the cell
        // actually exists in one of the tilemap layers.
        return cellExists;
    }

    /// <summary>
    /// Check if a cell is blocked by this layer.
    /// </summary>
    /// <param name="coord">The coordinate to check.</param>
    /// <returns>True if the cell is blocked, false otherwise.</returns>
    public bool IsCellBlocked(Vector2I coord)
    {
        // Get the tile data at the specified coordinate
        var tileData = this.GetCellTileData(coord);
        if (tileData != null)
        {
            // Check if the tile has the "blocked" custom data set to true
            if (tileData.GetCustomData(BlockedCellDataLayer).AsBool())
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Update the list of moveable cells.
    /// </summary>
    private void UpdateMoveableCells()
    {
        var oldMoveableCells = new List<Vector2I>(this.MoveableCells);
        this.MoveableCells.Clear();

        // Go through each cell in the layer
        foreach (var cell in this.GetUsedCells())
        {
            // A cell is moveable if it exists and is not blocked
            if (this.IsCellClear(cell))
            {
                this.MoveableCells.Add(cell);
            }
        }

        // Determine which cells have been added or removed
        var addedCells = this.MoveableCells.Except(oldMoveableCells).ToList();
        var removedCells = oldMoveableCells.Except(this.MoveableCells).ToList();

        // If there are changes, emit the signal
        if (addedCells.Count > 0 || removedCells.Count > 0)
        {
            var clearedArray = new Godot.Collections.Array();
            foreach (var cell in addedCells)
            {
                clearedArray.Add(cell);
            }

            var blockedArray = new Godot.Collections.Array();
            foreach (var cell in removedCells)
            {
                blockedArray.Add(cell);
            }

            this.EmitSignal(SignalName.CellsChanged, clearedArray, blockedArray);
        }
    }

    /// <summary>
    /// Callback when the node enters the tree.
    /// </summary>
    private void OnTreeEntered()
    {
        // When entering the tree, all cells become "cleared" (available for movement)
        var clearedCells = this.GetUsedCells().ToList();
        var blockedCells = new List<Vector2I>();

        // Separate cleared and blocked cells
        for (int i = clearedCells.Count - 1; i >= 0; i--)
        {
            var cell = clearedCells[i];
            if (this.IsCellBlocked(cell))
            {
                blockedCells.Add(cell);
                clearedCells.RemoveAt(i);
            }
        }

        var clearedArray = new Godot.Collections.Array();
        foreach (var cell in clearedCells)
        {
            clearedArray.Add(cell);
        }

        var blockedArray = new Godot.Collections.Array();
        foreach (var cell in blockedCells)
        {
            blockedArray.Add(cell);
        }

        this.EmitSignal(SignalName.CellsChanged, clearedArray, blockedArray);
    }

    /// <summary>
    /// Callback when the node exits the tree.
    /// </summary>
    private void OnTreeExited()
    {
        // When exiting the tree, all cells become "blocked" (no longer available for movement)
        var clearedCells = new List<Vector2I>();
        var blockedCells = this.GetUsedCells().ToList();

        var clearedArray = new Godot.Collections.Array();
        foreach (var cell in clearedCells)
        {
            clearedArray.Add(cell);
        }

        var blockedArray = new Godot.Collections.Array();
        foreach (var cell in blockedCells)
        {
            blockedArray.Add(cell);
        }

        this.EmitSignal(SignalName.CellsChanged, clearedArray, blockedArray);
    }

    /// <summary>
    /// Callback when the layer changes.
    /// </summary>
    private void OnChanged()
    {
        this.UpdateMoveableCells();
    }
}
