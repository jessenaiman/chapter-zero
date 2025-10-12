// <copyright file="GameboardProperties.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using Godot;

/// <summary>
/// Determines the Extents of the Gameboard, among other details.
/// The gameboard properties define the playable area of the game and where everything on it lies.
/// The gameboard is defined, essentially, as a grid of Vector2I cells. Anything may be
/// placed on one of these cells, so the gameboard determines where each cell is located. In this
/// case, we are using a simple orthographic (square) projection.
/// </summary>
public partial class GameboardProperties : Resource
{
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

    public GameboardProperties()
    {
        this.HalfCellSize = new Vector2(this.CellSize.X * 0.5f, this.CellSize.Y * 0.5f);
    }

    /// <summary>
    /// Convert cell coordinates to pixel coordinates.
    /// </summary>
    /// <returns></returns>
    public Vector2 CellToPixel(Vector2I cellCoordinates)
    {
        return new Vector2(
            cellCoordinates.X * this.CellSize.X,
            cellCoordinates.Y * this.CellSize.Y) + this.HalfCellSize;
    }

    /// <summary>
    /// Convert pixel coordinates to cell coordinates.
    /// </summary>
    /// <returns></returns>
    public Vector2I PixelToCell(Vector2 pixelCoordinates)
    {
        return new Vector2I(
            Mathf.FloorToInt(pixelCoordinates.X / this.CellSize.X),
            Mathf.FloorToInt(pixelCoordinates.Y / this.CellSize.Y));
    }
}
