// <copyright file="CollisionFinder.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Godot;

/// <summary>
/// Find all collision shapes of a given mask within a specified search radius.
///
/// The CollisionFinder is used to search for objects that contain a CollisionObject2D. In the
/// OpenRPG it serves as the cornerstone of the Gameboard/Gamepiece system, since objects
/// (gamepieces, terrain, etc.) are found dynamically by collision shape.
///
/// For example, to see if a given cell is occupied a CollisionFinder would be used to
/// search for collision shapes at that cell's location.
///
/// Note: physics objects update on the physics 'tick' so changes in position often
/// need a single frame before they may be found by search.
/// </summary>
public partial class CollisionFinder : RefCounted
{
    /// <summary>
    /// Cache the space state that will be queried.
    /// </summary>
    private PhysicsDirectSpaceState2D spaceState;

    /// <summary>
    /// Initializes a new instance of the <see cref="CollisionFinder"/> class.
    /// </summary>
    /// <param name="spaceState">The physics space state to query for collisions.</param>
    /// <param name="searchRadius">The radius of the search circle.</param>
    /// <param name="collisionMask">The collision layer mask to search in.</param>
    /// <param name="findAreas">Whether to include Area2D nodes in the search.</param>
    public CollisionFinder(PhysicsDirectSpaceState2D spaceState, float searchRadius, int collisionMask, bool findAreas = true)
    {
        this.spaceState = spaceState;

        var queryShape = new CircleShape2D();
        queryShape.Radius = searchRadius;

        this.QueryParameters = new PhysicsShapeQueryParameters2D();
        this.QueryParameters.Shape = queryShape;
        this.QueryParameters.CollisionMask = (uint) collisionMask;
        this.QueryParameters.CollideWithAreas = findAreas;
    }

    /// <summary>
    /// Gets cache the search parameters to quickly perform multiple searches.
    /// </summary>
    public PhysicsShapeQueryParameters2D QueryParameters { get; private set; }

    /// <summary>
    /// Find all collision shapes intersecting the query shape at position (in global coordinates).
    ///
    /// Please see PhysicsDirectSpaceState2D.IntersectShape for possible return values.
    ///
    /// Note: position must be given in global coordinates.
    /// </summary>
    /// <param name="position">The position to search at (in global coordinates).</param>
    /// <returns>Array of dictionaries containing collision information.</returns>
    public Godot.Collections.Array<Godot.Collections.Dictionary> Search(Vector2 position)
    {
        // To find collision shapes we'll query the PhysicsDirectSpaceState2D (usually from the main
        // viewport's current World2D). Any intersecting collision shape matching the provided collision
        // mask will be included in the results.
        this.QueryParameters.Transform = new Transform2D(0, position);

        return this.spaceState.IntersectShape(this.QueryParameters);
    }
}
