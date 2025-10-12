using Godot;
using System;

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
    /// Determines the Extents of the Gameboard, among other details.
    /// </summary>
    [Export]
    public Rect2I Extents { get; set; } = new Rect2I(0, 0, 100, 100);

    /// <summary>
    /// Size of each cell in pixels
    /// </summary>
    [Export]
    public Vector2I CellSize { get; set; } = new Vector2I(16, 16);

    /// <summary>
    /// Half of the cell size, precalculated for performance
    /// </summary>
    public Vector2 HalfCellSize { get; private set; }

    public GameboardProperties()
    {
        HalfCellSize = new Vector2(CellSize.X * 0.5f, CellSize.Y * 0.5f);
    }

    /// <summary>
    /// Convert cell coordinates to pixel coordinates.
    /// </summary>
    public Vector2 CellToPixel(Vector2I cellCoordinates)
    {
        return new Vector2(
            cellCoordinates.X * CellSize.X,
            cellCoordinates.Y * CellSize.Y
        ) + HalfCellSize;
    }

    /// <summary>
    /// Convert pixel coordinates to cell coordinates.
    /// </summary>
    public Vector2I PixelToCell(Vector2 pixelCoordinates)
    {
        return new Vector2I(
            Mathf.FloorToInt(pixelCoordinates.X / CellSize.X),
            Mathf.FloorToInt(pixelCoordinates.Y / CellSize.Y)
        );
    }
}
