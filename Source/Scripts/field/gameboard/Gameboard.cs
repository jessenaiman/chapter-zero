// <copyright file="Gameboard.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// Defines the playable area of the game and where everything on it lies.
/// The gameboard is defined, essentially, as a grid of Vector2I cells. Anything may be
/// placed on one of these cells, so the gameboard determines where each cell is located. In this
/// case, we are using a simple orthographic (square) projection.
/// The grid is contained within the playable boundaries and its constituent cells.
/// </summary>
public partial class Gameboard : Node
{
    /// <summary>
    /// Emitted whenever properties is set. This is used in case a Gamepiece is added to the
    /// board before the board properties are ready.
    /// </summary>
    [Signal]
    public delegate void PropertiesSetEventHandler();

    /// <summary>
    /// Emitted whenever the pathfinder state changes.
    /// This signal is emitted automatically in response to changed GameboardLayers.
    ///
    /// Note: This signal is only emitted when the actual movement state of the Gameboard
    /// changes. GameboardLayers may change their cells without actually changing the pathfinder's
    /// state (i.e. a visual update only), in which case this signal is not emitted.
    /// </summary>
    [Signal]
    public delegate void PathfinderChangedEventHandler(Godot.Collections.Array<Vector2I> addedCells, Godot.Collections.Array<Vector2I> removedCells);

    /// <summary>
    /// An invalid cell is not part of the gameboard. Note that this requires positive boundaries.
    /// </summary>
    public static readonly Vector2I InvalidCell = new Vector2I(-1, -1);

    public const int InvalidIndex = -1;

    /// <summary>
    /// Gets or sets determines the extents of the Gameboard, among other details.
    /// </summary>
    public GameboardProperties Properties
    {
        get => this.properties;
        set
        {
            if (value != this.properties)
            {
                this.properties = value;
                this.EmitSignal(SignalName.PropertiesSet);
            }
        }
    }

    private GameboardProperties properties;

    /// <summary>
    /// Gets a reference to the Pathfinder for the current playable area.
    /// </summary>
    public Pathfinder PathFinder { get; private set; } = new Pathfinder();

    /// <inheritdoc/>
    public override void _Ready()
    {
        // Initialize the pathfinder
        this.PathFinder = new Pathfinder();
    }

    /// <summary>
    /// Convert cell coordinates to pixel coordinates.
    /// </summary>
    /// <returns></returns>
    public Vector2 CellToPixel(Vector2I cellCoordinates)
    {
        return new Vector2(cellCoordinates.X * this.Properties.CellSize.X, cellCoordinates.Y * this.Properties.CellSize.Y) + this.Properties.HalfCellSize;
    }

    /// <summary>
    /// Convert pixel coordinates to cell coordinates.
    /// </summary>
    /// <returns></returns>
    public Vector2I PixelToCell(Vector2 pixelCoordinates)
    {
        return new Vector2I(
            Mathf.FloorToInt(pixelCoordinates.X / this.Properties.CellSize.X),
            Mathf.FloorToInt(pixelCoordinates.Y / this.Properties.CellSize.Y));
    }

    /// <summary>
    /// Get the cell under a node.
    /// </summary>
    /// <returns></returns>
    public Vector2I GetCellUnderNode(Node2D node)
    {
        return this.PixelToCell(node.GlobalPosition / node.GlobalScale);
    }

    /// <summary>
    /// Convert cell coordinates to an index unique to those coordinates.
    /// Note: cell coordinates outside the extents will return InvalidIndex.
    /// </summary>
    /// <returns></returns>
    public int CellToIndex(Vector2I cellCoordinates)
    {
        if (this.Properties.Extents.HasPoint(cellCoordinates))
        {
            // Negative coordinates can throw off index generation, so offset the boundary so that it's
            // top left corner is always considered Vector2I.Zero and index 0.
            return (cellCoordinates.X - this.Properties.Extents.Position.X) +
                ((cellCoordinates.Y - this.Properties.Extents.Position.Y) * this.Properties.Extents.Size.X);
        }

        return InvalidIndex;
    }

    /// <summary>
    /// Convert a unique index to cell coordinates.
    /// Note: indices outside the gameboard extents will return InvalidCell.
    /// </summary>
    /// <returns></returns>
    public Vector2I IndexToCell(int index)
    {
        var cell = new Vector2I(
            (index % this.Properties.Extents.Size.X) + this.Properties.Extents.Position.X,
            (index / this.Properties.Extents.Size.X) + this.Properties.Extents.Position.Y);

        if (this.Properties.Extents.HasPoint(cell))
        {
            return cell;
        }

        return InvalidCell;
    }

    /// <summary>
    /// Find a neighbouring cell, if it exists. Otherwise, returns InvalidCell.
    /// </summary>
    /// <returns></returns>
    public Vector2I GetAdjacentCell(Vector2I cell, int direction)
    {
        var neighbour = cell + Directions.Mappings.GetValueOrDefault(direction, Vector2I.Zero);
        if (this.Properties.Extents.HasPoint(neighbour))
        {
            return neighbour;
        }

        return InvalidCell;
    }

    /// <summary>
    /// Find all cells adjacent to a given cell. Only existing cells will be included.
    /// </summary>
    /// <returns></returns>
    public static List<Vector2I> GetAdjacentCells(Vector2I cell)
    {
        var neighbours = new List<Vector2I>();
        foreach (var direction in Directions.Points.Values)
        {
            var neighbour = GetAdjacentCell(cell, direction);
            if (neighbour != InvalidCell && neighbour != cell)
            {
                neighbours.Add(neighbour);
            }
        }

        return neighbours;
    }

    /// <summary>
    /// The Gameboard's state (where Gamepieces may or may not move) is composed from a number of
    /// GameboardLayers. These layers determine which cells are blocked or clear.
    /// The layers register themselves to the Gameboard in _Ready.
    /// </summary>
    public void RegisterGameboardLayer(GameboardLayer boardMap)
    {
        // We want to know whenever the board_map changes the gameboard state. This occurs when the map
        // is added or removed from the scene tree, or when its list of moveable cells changes.
        // Compare the changed cells with those already in the pathfinder. Any changes will cause the
        // Pathfinder to be updated.
        boardMap.CellsChanged += OnGameboardLayerCellsChanged;
    }

    /// <summary>
    /// Callback when gameboard layer cells change.
    /// </summary>
    private void OnGameboardLayerCellsChanged(List<Vector2I> clearedCells, List<Vector2I> blockedCells)
    {
        var addedCells = this.AddCellsToPathfinder(clearedCells);
        var removedCells = this.RemoveCellsFromPathfinder(blockedCells);

        this.ConnectNewPathfinderCells(addedCells);
        if (addedCells.Count > 0 || removedCells.Count > 0)
        {
            // Convert to Godot arrays for the signal
            var addedArray = new Array<Vector2I>(addedCells.Values.ToArray());
            var removedArray = new Array<Vector2I>(removedCells.ToArray());
            EmitSignal(SignalName.PathfinderChanged, addedArray, removedArray);
        }
    }

    /// <summary>
    /// Add cells to the pathfinder, checking that there are no blocking tiles on any GameboardLayers.
    /// Returns a dictionary representing the cells that are actually added to the pathfinder (may differ
    /// from clearedCells). Key = cell id (int, see CellToIndex), value = coordinate (Vector2I).
    /// </summary>
    private Dictionary<int, Vector2I> AddCellsToPathfinder(List<Vector2I> clearedCells)
    {
        var addedCells = new Dictionary<int, Vector2I>();

        // Verify whether or not cleared/blocked cells will change the state of the pathfinder.
        // If there is no change in state, we will not pass along the cell to other systems and
        // the pathfinder won't actually be changed.
        foreach (var cell in clearedCells)
        {
            // Note that cleared cells need to have all layers checked for a blocking tile.
            if (this.Properties.Extents.HasPoint(cell) && !this.PathFinder.HasCell(cell) &&
                this.IsCellClear(cell))
            {
                var uid = this.CellToIndex(cell);
                this.PathFinder.AddPoint(uid, cell);
                addedCells[uid] = cell;

                // Flag the cell as disabled if it is occupied.
                if (GamepieceRegistry.GetGamepiece(cell) != null)
                {
                    this.PathFinder.SetPointDisabled(uid);
                }
            }
        }

        return addedCells;
    }

    /// <summary>
    /// Remove cells from the pathfinder so that Gamepieces can no longer move through them.
    /// Only one Gameboard layer needs to block a cell for it to be considered blocked.
    /// Returns an array of cell coordinates that have been blocked. Cells that were already not in the
    /// pathfinder will be excluded from this array.
    /// </summary>
    private List<Vector2I> RemoveCellsFromPathfinder(List<Vector2I> blockedCells)
    {
        var removedCells = new List<Vector2I>();
        foreach (var cell in blockedCells)
        {
            // Only remove a cell that is already in the pathfinder. Also, we need to check that the cell
            // is not clear, since this method is also called when cells are removed from GameboardLayers
            // and other layers may still have this cell on their map.
            if (this.PathFinder.HasCell(cell) && !this.IsCellClear(cell))
            {
                this.PathFinder.RemovePoint(this.CellToIndex(cell));
                removedCells.Add(cell);
            }
        }

        return removedCells;
    }

    /// <summary>
    /// Go through a list of cells added to the pathfinder (returned from AddCellsToPathfinder) and
    /// connect them to each other and existing pathfinder cells.
    /// </summary>
    private void ConnectNewPathfinderCells(Dictionary<int, Vector2I> addedCells)
    {
        foreach (var uid in addedCells.Keys)
        {
            if (this.PathFinder.HasPoint(uid))
            {
                foreach (var neighbor in GetAdjacentCells(addedCells[uid]))
                {
                    var neighborId = this.CellToIndex(neighbor);
                    if (this.PathFinder.HasPoint(neighborId))
                    {
                        this.PathFinder.ConnectPoints(uid, neighborId);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Checks all TileMapLayers in the GameboardLayer.Group to see if the cell is clear
    /// (returns true) or blocked (returns false).
    ///
    /// A clear cell must fulfill two criteria:
    ///
    /// - Exists in at least one of the GameboardLayers.
    /// - None of the layers block movement at this cell, as defined by the
    /// GameboardLayer.BlockedCellDataLayer custom data layer (see
    /// TileData.GetCustomData).
    /// </summary>
    private bool IsCellClear(Vector2I coord)
    {
        // Check to make sure that cell exists.
        var cellExists = false;

        // In Godot C#, we would typically get nodes by group like this:
        var tilemaps = this.GetTree().GetNodesInGroup(GameboardLayer.Group);
        foreach (var node in tilemaps)
        {
            if (node is GameboardLayer tilemap)
            {
                if (tilemap != null && tilemap.GetUsedCells().Contains(coord))
                {
                    cellExists = true;
                    if (!tilemap.IsCellClear(coord))
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
}
