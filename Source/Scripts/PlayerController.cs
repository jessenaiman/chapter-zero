using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Applied to any gamepiece to allow player control.
/// The controller responds to player input to handle movement and interaction.
/// </summary>
[Tool]
public partial class PlayerController : GamepieceController
{
    public const string Group = "_PLAYER_CONTROLLER_GROUP";

    // Keep track of a targeted interaction. Used to face & interact with the object at a path's end.
    // It is reset on cancelling the move path or continuing movement via arrows/gamepad directions.
    private Interaction targetInteraction = null;

    // Keep track of any Triggers that the player has stepped on.
    private Trigger activeTrigger = null;

    // Also keep track of the most recently pressed move key (e.g. WASD keys). This makes keyboard input
    // feel more intuitive, since the gamepiece will move towards the most recently pressed key rather
    // than preferring an arbitrary axis.
    private Vector2 lastInputDirection = Vector2.Zero;

    // The "interaction searcher" area basically activates any Interactions, which means that they'll
    // respond to key/button input.
    private Area2D interactionSearcher;
    private CollisionShape2D interactionShape;

    // The player collision area activates Triggers whenever the player moves onto their collision
    // shape.
    private Area2D playerCollision;

    public override void _Ready()
    {
        base._Ready();

        if (!Engine.IsEditorHint())
        {
            AddToGroup(Group);

            // Get references to the child nodes
            interactionSearcher = GetNode<Area2D>("InteractionSearcher");
            interactionShape = GetNode<CollisionShape2D>("InteractionSearcher/CollisionShape2D");
            playerCollision = GetNode<Area2D>("PlayerCollision");

            // Refer the various player collision shapes to their gamepiece (parent of the controller).
            // This will allow other objects/systems to quickly find which gamepiece they are working on
            // via the collision "owners".
            if (interactionSearcher != null)
            {
                interactionSearcher.Owner = Gamepiece;
            }
            if (playerCollision != null)
            {
                playerCollision.Owner = Gamepiece;
            }

            // Update the position of the player's collision shape to match the cell that it is currently
            // moving towards.
            WaypointChanged += OnWaypointChanged;

            // The player collision picks up any triggers that it moves over. Keep track of them until
            // player movement to the current cells has completed.
            if (playerCollision != null)
            {
                playerCollision.AreaEntered += OnCollisionTriggered;
            }

            if (Gamepiece != null)
            {
                Gamepiece.DirectionChanged += OnGamepieceDirectionChanged;
            }

            // Connect to field events
            // FieldEvents.CellSelected += OnCellSelected;
            // FieldEvents.InteractionSelected += OnInteractionSelected;
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionReleased("select"))
        {
            StopMoving();
        }
        else if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.IsActionPressed("ui_up"))
            {
                lastInputDirection = Vector2.Up;
                if (Gamepiece.IsMoving())
                {
                    StopMoving();
                }
                else
                {
                    MoveToPressedKey(Vector2.Up);
                }
            }
            else if (keyEvent.IsActionPressed("ui_down"))
            {
                lastInputDirection = Vector2.Down;
                if (Gamepiece.IsMoving())
                {
                    StopMoving();
                }
                else
                {
                    MoveToPressedKey(Vector2.Down);
                }
            }
            else if (keyEvent.IsActionPressed("ui_left"))
            {
                lastInputDirection = Vector2.Left;
                if (Gamepiece.IsMoving())
                {
                    StopMoving();
                }
                else
                {
                    MoveToPressedKey(Vector2.Left);
                }
            }
            else if (keyEvent.IsActionPressed("ui_right"))
            {
                lastInputDirection = Vector2.Right;
                if (Gamepiece.IsMoving())
                {
                    StopMoving();
                }
                else
                {
                    MoveToPressedKey(Vector2.Right);
                }
            }
        }
    }

    public override void MoveAlongPath(List<Vector2I> value)
    {
        base.MoveAlongPath(new List<Vector2I>(value));

        if (interactionShape != null)
        {
            interactionShape.Disabled = true;
        }
        // Player.PlayerPathSet.Emit(Gamepiece, value[value.Count - 1]);
    }

    public void MoveToPressedKey(Vector2 inputDirection)
    {
        if (IsActive)
        {
            var sourceCell = GamepieceRegistry.GetCell(Gamepiece);
            var targetCell = Vector2I.Zero;

            // Unless using 8-direction movement, one movement axis must be preferred.
            // Default to the x-axis.
            targetCell = sourceCell + new Vector2I((int)inputDirection.X, (int)inputDirection.Y);

            // Try to get a path to destination (will fail if cell is occupied)
            var newPath = Gameboard.Pathfinder.GetPathToCell(sourceCell, targetCell);

            // Path is invalid. Bump animation?
            if (newPath.Count < 1)
            {
                Gamepiece.Direction = Directions.AngleToDirection(inputDirection.Angle());
            }
            else
            {
                MovePath = new List<Vector2I>(newPath);
            }
        }
    }

    public override void StopMoving()
    {
        base.StopMoving();
        targetInteraction = null;
    }

    // The player has clicked on an empty gameboard cell. We'll try to move _gamepiece to the cell.
    private void OnCellSelected(Vector2I cell)
    {
        if (IsActive && !Gamepiece.IsMoving())
        {
            var sourceCell = Gameboard.PixelToCell(Gamepiece.Position);

            // Don't move to the cell the focus is standing on.
            if (cell == sourceCell)
            {
                return;
            }

            // Take a look at what's underneath the cursor. If there's an interaction, move towards it
            // and try to interact with it.

            // Otherwise it's just the empty gameboard, so we'll try to move the player towards the
            // selected cell.
            var newPath = Gameboard.Pathfinder.GetPathToCell(sourceCell, cell);
            if (newPath.Count > 0)
            {
                MovePath = new List<Vector2I>(newPath);
            }
        }
    }

    // The player has clicked on something interactable. We'll try to move next to the interaction and
    // then run the interaction.
    private void OnInteractionSelected(Interaction interaction)
    {
        if (IsActive && !Gamepiece.IsMoving())
        {
            var sourceCell = Gameboard.PixelToCell(Gamepiece.Position);
            var targetCell = Gameboard.PixelToCell(interaction.Position);

            if (targetCell == sourceCell)
            {
                return;
            }

            // First of all, check to see if the target is adjacent to the source.
            if (Gameboard.GetAdjacentCells(sourceCell).Contains(targetCell))
            {
                Gamepiece.Direction = Directions.VectorToDirection(interaction.Position - Gamepiece.Position);
                interaction.Run();
            }
            else
            {
                // Only cache the interaction and move towards it if there is a valid move path.
                var newPath = Gameboard.Pathfinder.GetPathCellsToAdjacentCell(sourceCell, targetCell);
                if (newPath.Count > 0)
                {
                    targetInteraction = interaction;
                    MovePath = new List<Vector2I>(newPath);
                }
            }
        }
        // If the player is already moving, cancel that movement.
        else
        {
            StopMoving();
        }
    }

    private void OnWaypointChanged(Vector2I newWaypoint)
    {
        if (newWaypoint == Gameboard.InvalidCell)
        {
            if (playerCollision != null)
            {
                playerCollision.Position = Vector2.Zero;
            }
        }
        else
        {
            if (playerCollision != null)
            {
                playerCollision.Position = Gameboard.CellToPixel(newWaypoint) - Gamepiece.Position;
            }
        }
    }

    private void OnCollisionTriggered(Area2D area)
    {
        if (area.Owner is Trigger)
        {
            activeTrigger = area.Owner as Trigger;
        }
    }

    private void OnGamepieceDirectionChanged(Directions.Points newDirection)
    {
        var offset = Directions.Mappings[newDirection] * 16;
        if (interactionSearcher != null)
        {
            interactionSearcher.Position = offset;
        }
    }

    protected override void OnGamepieceArriving(float excessDistance)
    {
        // If the gamepiece moved onto a trigger, stop the gamepiece in its tracks.
        if (activeTrigger != null)
        {
            StopMoving();
        }
        // Otherwise, carry on with movement.
        else
        {
            base.OnGamepieceArriving(excessDistance);

            // It may be that the player is holding the keys down. In that case, continue moving the
            // gamepiece towards the pressed direction.
            var inputDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            if (!inputDirection.IsEqualApprox(Vector2.Zero))
            {
                MoveToPressedKey(lastInputDirection);
            }
        }
    }

    protected override void OnGamepieceArrived()
    {
        base.OnGamepieceArrived();

        if (playerCollision != null)
        {
            playerCollision.Position = Vector2.Zero;
        }
        if (interactionShape != null)
        {
            interactionShape.Disabled = false;
        }

        // If there's a trigger at this cell, do nothing but reset the trigger reference.
        if (activeTrigger != null)
        {
            activeTrigger = null;
        }
        // Otherwise, if there's an interaction queued, run the interaction.
        else if (targetInteraction != null)
        {
            // Face the selected interaction...
            var directionToTarget = targetInteraction.Position - Gamepiece.Position;
            Gamepiece.Direction = Directions.VectorToDirection(directionToTarget);

            // ...and then execute the interaction.
            targetInteraction.Run();
            targetInteraction = null;
        }
        // No target, but check to see if the player is holding a key down and face in the direction of
        // the last pressed key.
        else
        {
            var inputDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            if (!inputDirection.IsEqualApprox(Vector2.Zero))
            {
                Gamepiece.Direction = Directions.VectorToDirection(lastInputDirection);
            }
        }
    }
}
