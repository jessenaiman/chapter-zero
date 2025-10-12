using Godot;
using System.Collections.Generic;

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
    /// Cache the search parameters to quickly perform multiple searches.
    /// </summary>
    public PhysicsShapeQueryParameters2D QueryParameters { get; private set; }

    // Cache the space state that will be queried.
    private PhysicsDirectSpaceState2D _spaceState;

    public CollisionFinder(PhysicsDirectSpaceState2D spaceState, float searchRadius, int collisionMask, bool findAreas = true)
    {
        _spaceState = spaceState;

        var queryShape = new CircleShape2D();
        queryShape.Radius = searchRadius;

        QueryParameters = new PhysicsShapeQueryParameters2D();
        QueryParameters.Shape = queryShape;
        QueryParameters.CollisionMask = collisionMask;
        QueryParameters.CollideWithAreas = findAreas;
    }

    /// <summary>
    /// Find all collision shapes intersecting the query shape at position (in global coordinates).
    ///
    /// Please see PhysicsDirectSpaceState2D.IntersectShape for possible return values.
    ///
    /// Note: position must be given in global coordinates.
    /// </summary>
    /// <param name="position">The position to search at (in global coordinates)</param>
    /// <returns>Array of dictionaries containing collision information</returns>
    public Godot.Collections.Array<Godot.Collections.Dictionary> Search(Vector2 position)
    {
        // To find collision shapes we'll query the PhysicsDirectSpaceState2D (usually from the main
        // viewport's current World2D). Any intersecting collision shape matching the provided collision
        // mask will be included in the results.
        QueryParameters.Transform = new Transform2D(0, 0, position.X, position.Y);

        return _spaceState.IntersectShape(QueryParameters);
    }
}
