
// <copyright file="GameboardProperties.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts.Field.Gameboard;
/// <summary>
/// Determines the Extents of the Gameboard, among other details.
/// The gameboard properties define the playable area of the game and where everything on it lies.
/// The gameboard is defined, essentially, as a grid of Vector2I cells. Anything may be
/// placed on one of these cells, so the gameboard determines where each cell is located. In this
/// case, we are using a simple orthographic (square) projection.
/// </summary>
[GlobalClass]
public partial class GameboardProperties : Resource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameboardProperties"/> class.
    /// Precalculates the half cell size for performance.
    /// </summary>
    /// <remarks>
    /// The constructor sets the <see cref="HalfCellSize"/> property based on the initial <see cref="CellSize"/>.
    /// </remarks>
    public GameboardProperties()
    {
        this.HalfCellSize = new Vector2(this.CellSize.X * 0.5f, this.CellSize.Y * 0.5f);
    }

    /// <summary>
    /// Gets or sets determines the Extents of the Gameboard, among other details.
    /// </summary>
    [Export]
    public Rect2I Extents { get; set; } = new Rect2I(0, 0, 100, 100);

    /// <summary>
    /// Gets or sets size of each cell in pixels.
    /// </summary>
    [Export]
    public Vector2I CellSize { get; set; } = new Vector2I(16, 16);

    /// <summary>
    /// Gets half of the cell size, precalculated for performance.
    /// </summary>
    public Vector2 HalfCellSize { get; private set; }

    /// <summary>
    /// Convert cell coordinates to pixel coordinates.
    /// </summary>
    /// <param name="cellCoordinates">The cell coordinates to convert to pixel coordinates.</param>
    /// <returns>
    /// The pixel coordinates corresponding to the specified cell coordinates.
    /// </returns>
    public Vector2 CellToPixel(Vector2I cellCoordinates)
    {
        return new Vector2(
            cellCoordinates.X * this.CellSize.X,
            cellCoordinates.Y * this.CellSize.Y) + this.HalfCellSize;
    }

    /// <summary>
    /// Convert pixel coordinates to cell coordinates.
    /// </summary>
    /// <param name="pixelCoordinates">The pixel coordinates to convert to cell coordinates.</param>
    /// <returns>
    /// The cell coordinates corresponding to the specified pixel coordinates.
    /// </returns>
    public Vector2I PixelToCell(Vector2 pixelCoordinates)
    {
        return new Vector2I(
            Mathf.FloorToInt(pixelCoordinates.X / this.CellSize.X),
            Mathf.FloorToInt(pixelCoordinates.Y / this.CellSize.Y));
    }
}
