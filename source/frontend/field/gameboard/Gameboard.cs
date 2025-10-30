
// <copyright file="Gameboard.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;

using Godot;
using OmegaSpiral.Source.Scripts.Field.gamepieces;

namespace OmegaSpiral.Source.Scripts.Field.gameboard;

/// <summary>
/// Defines the playable area of the game and where everything on it lies.
/// The gameboard is defined, essentially, as a grid of Vector2I cells. Anything may be
/// placed on one of these cells, so the gameboard determines where each cell is located. In this
/// case, we are using a simple orthographic (square) projection.
/// The grid is contained within the playable boundaries and its constituent cells.
/// </summary>
[GlobalClass]
public partial class Gameboard : Node
{
    /// <summary>
    /// An invalid index is not part of the gameboard.
    /// </summary>
    public const int InvalidIndex = -1;

    /// <summary>
    /// An invalid cell is not part of the gameboard. Note that this requires positive boundaries.
    /// </summary>
    public static readonly Vector2I InvalidCell = new Vector2I(-1, -1);

    private GameboardProperties? properties;

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
    /// <param name="addedCells">The cells that were added to the pathfinder.</param>
    /// <param name="removedCells">The cells that were removed from the pathfinder.</param>
    [Signal]
    public delegate void PathfinderChangedEventHandler(Godot.Collections.Array addedCells, Godot.Collections.Array removedCells);

    /// <summary>
    /// Gets or sets the extents of the Gameboard, among other details.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the properties have not been initialized.
    /// </exception>
    public GameboardProperties Properties
    {
        get => this.properties ?? throw new InvalidOperationException("Properties not initialized");
        set
        {
            if (value != this.properties)
            {
                this.properties = value;
                this.EmitSignal(SignalName.PropertiesSet);
            }
        }
    }

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
    /// <param name="cellCoordinates">The cell coordinates to convert.</param>
    /// <returns>The pixel coordinates corresponding to the cell.</returns>
    public Vector2 CellToPixel(Vector2I cellCoordinates)
    {
        if (this.Properties == null)
        {
            return Vector2.Zero;
        }

        return new Vector2(cellCoordinates.X * this.Properties.CellSize.X, cellCoordinates.Y * this.Properties.CellSize.Y) + this.Properties.HalfCellSize;
    }

    /// <summary>
    /// Converts pixel coordinates to cell coordinates.
    /// </summary>
    /// <param name="pixelCoordinates">The pixel coordinates to convert.</param>
    /// <returns>The cell coordinates corresponding to the pixel.</returns>
    public Vector2I PixelToCell(Vector2 pixelCoordinates)
    {
        if (this.Properties == null)
        {
            return Vector2I.Zero;
        }

        return new Vector2I(
            Mathf.FloorToInt(pixelCoordinates.X / this.Properties.CellSize.X),
            Mathf.FloorToInt(pixelCoordinates.Y / this.Properties.CellSize.Y));
    }

    /// <summary>
    /// Gets the cell under a node.
    /// </summary>
    /// <param name="node">The node to check.</param>
    /// <returns>The cell under the node.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="node"/> is <c>null</c>.</exception>
    public Vector2I GetCellUnderNode(Node2D node)
    {
        return node == null ? throw new ArgumentNullException(nameof(node)) : this.PixelToCell(node.GlobalPosition / node.GlobalScale);
    }

    /// <summary>
    /// Converts cell coordinates to a unique index.
    /// Note: cell coordinates outside the extents will return InvalidIndex.
    /// </summary>
    /// <param name="cellCoordinates">The cell coordinates to convert.</param>
    /// <returns>The unique index for the cell, or <see cref="InvalidIndex"/> if out of bounds.</returns>
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
    /// Converts a unique index to cell coordinates.
    /// Note: indices outside the gameboard extents will return InvalidCell.
    /// </summary>
    /// <param name="index">The index to convert.</param>
    /// <returns>The cell coordinates for the index, or <see cref="InvalidCell"/> if out of bounds.</returns>
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
    /// Finds a neighbouring cell, if it exists. Otherwise, returns <see cref="InvalidCell"/>.
    /// </summary>
    /// <param name="cell">The cell to check.</param>
    /// <param name="direction">The direction to check.</param>
    /// <returns>The adjacent cell, or <see cref="InvalidCell"/> if none exists.</returns>
    public Vector2I GetAdjacentCell(Vector2I cell, Vector2I direction)
    {
        var neighbour = cell + direction;
        if (this.Properties.Extents.HasPoint(neighbour))
        {
            return neighbour;
        }

        return InvalidCell;
    }

    /// <summary>
    /// Finds all cells adjacent to a given cell. Only existing cells will be included.
    /// </summary>
    /// <param name="cell">The cell to check.</param>
    /// <returns>A list of adjacent cells.</returns>
    public ReadOnlyCollection<Vector2I> GetAdjacentCells(Vector2I cell)
    {
        var neighbours = new List<Vector2I>();
        foreach (var direction in Enum.GetValues<Directions.Point>())
        {
            var directionVector = Directions.Mappings[direction];
            var neighbour = this.GetAdjacentCell(cell, directionVector);
            if (neighbour != InvalidCell && neighbour != cell)
            {
                neighbours.Add(neighbour);
            }
        }
        return neighbours.AsReadOnly();
    }

    /// <summary>
    /// Registers a GameboardLayer to the Gameboard.
    /// </summary>
    /// <param name="boardMap">The GameboardLayer to register.</param>
    /// <exception cref="ArgumentNullException"><paramref name="boardMap"/> is <c>null</c>.</exception>
    public void RegisterGameboardLayer(GameboardLayer boardMap)
    {
        ArgumentNullException.ThrowIfNull(boardMap);

        boardMap.CellsChanged += this.OnGameboardLayerCellsChanged;
    }

    /// <summary>
    /// Callback when gameboard layer cells change.
    /// </summary>
    /// <param name="addedCells">Cells added to the pathfinder.</param>
    /// <param name="removedCells">Cells removed from the pathfinder.</param>
    private void OnGameboardLayerCellsChanged(Godot.Collections.Array addedCells, Godot.Collections.Array removedCells)
    {
        // Convert Godot arrays to dictionaries for internal processing
        var addedCellsDict = new Dictionary<int, Vector2I>();
        for (int i = 0; i < addedCells.Count; i++)
        {
            var cell = (Vector2I) addedCells[i];
            addedCellsDict[this.CellToIndex(cell)] = cell;
        }

        this.ConnectNewPathfinderCells(addedCellsDict);
        if (addedCells.Count > 0 || removedCells.Count > 0)
        {
            this.EmitSignal(SignalName.PathfinderChanged, addedCells, removedCells);
        }
    }

    /// <summary>
    /// Add cells to the pathfinder, checking that there are no blocking tiles on any GameboardLayers.
    /// Returns a dictionary representing the cells that are actually added to the pathfinder (may differ
    /// from clearedCells). Key = cell id (int, see CellToIndex), value = coordinate (Vector2I).
    /// </summary>
    /// <param name="clearedCells"></param>
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
                if (GamepieceRegistry.Instance?.GetGamepiece(cell) != null)
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
    /// <param name="blockedCells">A list of cells to be removed from the pathfinder.</param>
    private ReadOnlyCollection<Vector2I> RemoveCellsFromPathfinder(List<Vector2I> blockedCells)
    {
        var removedCells = new List<Vector2I>();
        foreach (var cell in blockedCells)
        {
            if (this.PathFinder.HasCell(cell) && !this.IsCellClear(cell))
            {
                this.PathFinder.RemovePoint(this.CellToIndex(cell));
                removedCells.Add(cell);
            }
        }
        return removedCells.AsReadOnly();
    }

    /// <summary>
    /// Go through a list of cells added to the pathfinder (returned from AddCellsToPathfinder) and
    /// connect them to each other and existing pathfinder cells.
    /// </summary>
    /// <param name="addedCells">A dictionary of cell indices and their coordinates that have been added to the pathfinder.</param>
    private void ConnectNewPathfinderCells(Dictionary<int, Vector2I> addedCells)
    {
        foreach (var uid in addedCells.Keys)
        {
            if (this.PathFinder.HasPoint(uid))
            {
                foreach (var neighbor in this.GetAdjacentCells(addedCells[uid]))
                {
                    var neighborId = this.CellToIndex(neighbor);
                    if (this.PathFinder.HasPoint(neighborId))
                    {
                        this.PathFinder.ConnectPoints(uid, neighborId);
                    }
                }

                // Flag the cell as disabled if it is occupied.
                if (GamepieceRegistry.Instance?.GetGamepiece(addedCells[uid]) != null)
                {
                    this.PathFinder.SetPointDisabled(uid);
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
    /// <param name="coord"></param>
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
