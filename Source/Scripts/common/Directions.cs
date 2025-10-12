// <copyright file="Directions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Godot;

/// <summary>
/// A utility class converting between Godot angles (+'ve x axis is 0 rads) and cardinal points.
/// </summary>
public partial class Directions : RefCounted
{
    /// <summary>
    /// The cardinal points, in clockwise order starting from North.
    /// </summary>
    public enum Points
    {
        North,
        East,
        South,
        West,
    }

    /// <summary>
    /// The direction corresponding to a cardinal point as a Vector2I value. NORTH is Vector2I(0, -1), etc.
    /// </summary>
    public static readonly System.Collections.Generic.Dictionary<Points, Vector2I> Mappings = new System.Collections.Generic.Dictionary<Points, Vector2I>
    {
        { Points.North, Vector2I.Up },
        { Points.East, Vector2I.Right },
        { Points.South, Vector2I.Down },
        { Points.West, Vector2I.Left },
    };

    /// <summary>
    /// Convert an angle, such as from Vector2.Angle, to a Points value.
    /// </summary>
    /// <param name="angle">The angle to convert.</param>
    /// <returns>The corresponding cardinal direction.</returns>
    public static Points AngleToDirection(float angle)
    {
        if (angle <= -System.Math.PI / 4.0 && angle > -3.0 * System.Math.PI / 4.0)
        {
            return Points.North;
        }
        else if (angle <= System.Math.PI / 4.0 && angle > -System.Math.PI / 4.0)
        {
            return Points.East;
        }
        else if (angle <= 3.0 * System.Math.PI / 4.0 && angle > System.Math.PI / 4.0)
        {
            return Points.South;
        }

        return Points.West;
    }

    /// <summary>
    /// Convert a vector to a direction.
    /// </summary>
    /// <param name="vector">The vector to convert.</param>
    /// <returns>The corresponding cardinal direction.</returns>
    public static Points VectorToDirection(Vector2 vector)
    {
        return AngleToDirection(vector.Angle());
    }
}
