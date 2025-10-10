using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// A centralized place to store and access directional mappings.
/// This class provides a consistent way to handle directions throughout the game,
/// mapping between different representations (points, angles, vectors) and providing
/// utility functions for direction calculations.
/// </summary>
public static class Directions
{
    /// <summary>
    /// Direction points enumeration
    /// </summary>
    public enum Points
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

    /// <summary>
    /// Direction angles enumeration
    /// </summary>
    public enum Angles
    {
        Up = 90,
        Right = 0,
        Down = 270,
        Left = 180
    }

    /// <summary>
    /// Mapping from Points to Vector2I directions
    /// </summary>
    public static readonly Dictionary<Points, Vector2I> Mappings = new Dictionary<Points, Vector2I>
    {
        { Points.Up, Vector2I.Up },
        { Points.Right, Vector2I.Right },
        { Points.Down, Vector2I.Down },
        { Points.Left, Vector2I.Left }
    };

    /// <summary>
    /// Mapping from Points to their integer values
    /// </summary>
    public static readonly Dictionary<Points, int> Values = new Dictionary<Points, int>
    {
        { Points.Up, 0 },
        { Points.Right, 1 },
        { Points.Down, 2 },
        { Points.Left, 3 }
    };

    /// <summary>
    /// Half cell size vector for positioning calculations
    /// </summary>
    public static readonly Vector2 HalfCellSize = new Vector2(8, 8);

    /// <summary>
    /// Convert an angle to a direction point
    /// </summary>
    public static Points AngleToDirection(float angle)
    {
        // Normalize the angle to 0-360 range
        angle = ((int)angle % 360 + 360) % 360;

        // Map to closest direction
        if (angle >= 45 && angle < 135)
            return Points.Up;
        if (angle >= 135 && angle < 225)
            return Points.Left;
        if (angle >= 225 && angle < 315)
            return Points.Down;
        return Points.Right; // angle >= 315 || angle < 45
    }

    /// <summary>
    /// Convert a vector to a direction point
    /// </summary>
    public static Points VectorToDirection(Vector2 vector)
    {
        if (vector.IsEqualApprox(Vector2.Zero))
        {
            return Points.Down; // Default direction
        }

        var angle = Mathf.RadToDeg(vector.Angle());
        return AngleToDirection(angle);
    }

    /// <summary>
    /// Get adjacent cells to a given cell
    /// </summary>
    public static List<Vector2I> GetAdjacentCells(Vector2I cell)
    {
        var neighbors = new List<Vector2I>();
        foreach (var direction in Mappings.Values)
        {
            neighbors.Add(cell + direction);
        }
        return neighbors;
    }
}
