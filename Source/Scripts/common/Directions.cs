// <copyright file="Directions.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral
{
    /// <summary>
    /// A utility class converting between Godot angles (+'ve x axis is 0 rads) and cardinal points.
    /// </summary>
    public partial class Directions : RefCounted
    {
        /// <summary>
        /// The direction corresponding to a cardinal point as a Vector2I value. NORTH is Vector2I(0, -1), etc.
        /// </summary>
        public static readonly System.Collections.Generic.Dictionary<Point, Vector2I> Mappings = new System.Collections.Generic.Dictionary<Point, Vector2I>
    {
        { Point.North, Vector2I.Up },
        { Point.East, Vector2I.Right },
        { Point.South, Vector2I.Down },
        { Point.West, Vector2I.Left },
    };

        /// <summary>
        /// The cardinal points, in clockwise order starting from North.
        /// </summary>
        public enum Point
        {
            /// <summary>
            /// The north cardinal direction.
            /// </summary>
            North = 0,

            /// <summary>
            /// The east cardinal direction.
            /// </summary>
            East = 1,

            /// <summary>
            /// The south cardinal direction.
            /// </summary>
            South = 2,

            /// <summary>
            /// The west cardinal direction.
            /// </summary>
            West = 3,
        }

        /// <summary>
        /// Convert an angle, such as from Vector2.Angle, to a Points value.
        /// </summary>
        /// <param name="angle">The angle to convert.</param>
        /// <returns>The corresponding cardinal direction.</returns>
        public static Point AngleToDirection(float angle)
        {
            if (angle <= -System.Math.PI / 4.0 && angle > -3.0 * System.Math.PI / 4.0)
            {
                return Point.North;
            }
            else if (angle <= System.Math.PI / 4.0 && angle > -System.Math.PI / 4.0)
            {
                return Point.East;
            }
            else if (angle <= 3.0 * System.Math.PI / 4.0 && angle > System.Math.PI / 4.0)
            {
                return Point.South;
            }

            return Point.West;
        }

        /// <summary>
        /// Convert a vector to a direction.
        /// </summary>
        /// <param name="vector">The vector to convert.</param>
        /// <returns>The corresponding cardinal direction.</returns>
        public static Point VectorToDirection(Vector2 vector)
        {
            return AngleToDirection(vector.Angle());
        }
    }
}
