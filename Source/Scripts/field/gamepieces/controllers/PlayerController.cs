// <copyright file="PlayerController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// Applied to any gamepiece to allow player control.
/// <br/><br/>The controller responds to player input to handle movement and interaction.
/// </summary>
[Tool]
[GlobalClass]
public partial class PlayerController : GamepieceController
{
    /// <summary>
    /// The group name for player controllers.
    /// </summary>
    public const string Group = "_PLAYER_CONTROLLER_GROUP";

    /// <summary>
    /// Keep track of a targeted interaction. Used to face and interact with the object at a path's end.
    /// It is reset on cancelling the move path or continuing movement via arrows/gamepad directions.
    /// </summary>
    private Interaction targetInteraction;

    /// <summary>
    /// Keep track of any Triggers that the player has stepped on.
    /// </summary>
    private Trigger activeTrigger;

    /// <summary>
    /// Also keep track of the most recently pressed move key (e.g. WASD keys). This makes keyboard input
    /// feel more intuitive, since the gamepiece will move towards the most recently pressed key rather
    /// than prefering an arbitrary axis.
    /// </summary>
    private Vector2 lastInputDirection = Vector2.Zero;

    /// <summary>
    /// The "interaction searcher" area basically activates any Interactions, which means that they'll
    /// respond to key/button input.
    /// </summary>
    private Area2D interactionSearcher;

    /// <summary>
    /// The collision shape for the interaction searcher.
    /// </summary>
    private CollisionShape2D interactionShape;

    /// <summary>
    /// The player collision area activates Triggers whenever the player moves onto their collision
    /// shape.
    /// </summary>
    private Area2D playerCollision;

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        if (!Engine.IsEditorHint())
        {
            this.AddToGroup(Group);

            // Get references to child nodes
            this.interactionSearcher = this.GetNode<Area2D>("InteractionSearcher");
            this.interactionShape = this.GetNode<CollisionShape2D>("InteractionSearcher/CollisionShape2D");
            this.playerCollision = this.GetNode<Area2D>("PlayerCollision");

            // Refer the various player collision shapes to their gamepiece (parent of the controller).
            // This will allow other objects/systems to quickly find which gamepiece they are working on
            // via the collision "owners".
            this.interactionSearcher.Owner = this.Gamepiece;
            this.playerCollision.Owner = this.Gamepiece;

            // Update the position of the player's collision shape to match the cell that it is currently
            // moving towards.
            this.Gamepiece.WaypointChanged += (newWaypoint) =>
            {
                if (newWaypoint == Gameboard.InvalidCell)
                {
                    this.playerCollision.Position = Vector2.Zero;
                }
                else
                {
                    this.playerCollision.Position = Gameboard.CellToPixel(newWaypoint) - this.Gamepiece.Position;
                }
            };

            // The player collision picks up any triggers that it moves over. Keep track of them until
            // player movement to the current cells has completed.
            this.playerCollision.AreaEntered += this.OnCollisionTriggered;

            this.Gamepiece.DirectionChanged += this.OnGamepieceDirectionChanged;

            // Connect to field events
            FieldEvents.Instance.CellSelected += _OnCellSelected;
            FieldEvents.Instance.InteractionSelected += _OnInteractionSelected;
        }
    }

    /// <inheritdoc/>
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionReleased("select"))
        {
            this.StopMoving();
        }
        else if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.IsActionPressed("ui_up"))
            {
                this.lastInputDirection = Vector2.Up;
                if (this.Gamepiece.IsMoving)
                {
                    this.StopMoving();
                }
                else
                {
                    this.MoveToPressedKey(Vector2.Up);
                }
            }
            else if (keyEvent.IsActionPressed("ui_down"))
            {
                this.lastInputDirection = Vector2.Down;
                if (this.Gamepiece.IsMoving)
                {
                    this.StopMoving();
                }
                else
                {
                    this.MoveToPressedKey(Vector2.Down);
                }
            }
            else if (keyEvent.IsActionPressed("ui_left"))
            {
                this.lastInputDirection = Vector2.Left;
                if (this.Gamepiece.IsMoving)
                {
                    this.StopMoving();
                }
                else
                {
                    this.MoveToPressedKey(Vector2.Left);
                }
            }
            else if (keyEvent.IsActionPressed("ui_right"))
            {
                this.lastInputDirection = Vector2.Right;
                if (this.Gamepiece.IsMoving)
                {
                    this.StopMoving();
                }
                else
                {
                    this.MoveToPressedKey(Vector2.Right);
                }
            }
        }
    }

    /// <summary>
    /// Move the gamepiece along a path.
    /// </summary>
    /// <param name="path">The path to follow.</param>
    public override void MoveAlongPath(List<Vector2I> path)
    {
        base.MoveAlongPath(new List<Vector2I>(path));

        this.interactionShape.SetDeferred("disabled", true);
        Player.Instance.EmitSignal(Player.SignalName.PlayerPathSet, this.Gamepiece, path[^1]);
    }

    /// <summary>
    /// Move the gamepiece towards a pressed key direction.
    /// </summary>
    /// <param name="inputDirection">The direction to move.</param>
    public override void MoveToPressedKey(Vector2 inputDirection)
    {
        if (this.IsActive)
        {
            var sourceCell = GamepieceRegistry.Instance.GetCell(this.Gamepiece);
            var targetCell = Vector2I.Zero;

            // Unless using 8-direction movement, one movement axis must be preferred.
            // Default to the x-axis.
            targetCell = sourceCell + new Vector2I((int)inputDirection.X, (int)inputDirection.Y);

            // Try to get a path to destination (will fail if cell is occupied)
            var newMovePath = Gameboard.Instance.PathFinder.GetPathToCell(sourceCell, targetCell);

            // Path is invalid. Bump animation?
            if (newMovePath.Count < 1)
            {
                this.Gamepiece.Direction = Directions.AngleToDirection(inputDirection.Angle());
            }
            else
            {
                this.MovePath = new List<Vector2I>(newMovePath);
            }
        }
    }

    /// <summary>
    /// Stop the gamepiece's movement.
    /// </summary>
    public override void StopMoving()
    {
        base.StopMoving();
        this.targetInteraction = null;
    }

    /// <summary>
    /// Callback when the player's gamepiece arrives at a waypoint.
    /// </summary>
    /// <param name="excessDistance">The excess distance when arriving.</param>
    protected override void OnGamepieceArriving(float excessDistance)
    {
        // If the gamepiece moved onto a trigger, stop the gamepiece in its tracks.
        if (this.activeTrigger != null)
        {
            this.StopMoving();
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
                this.MoveToPressedKey(this.lastInputDirection);
            }
        }
    }

    /// <summary>
    /// Callback when the gamepiece has arrived at a waypoint.
    /// </summary>
    protected override void OnGamepieceArrived()
    {
        base.OnGamepieceArrived();

        this.playerCollision.Position = Vector2.Zero;
        this.interactionShape.SetDeferred("disabled", false);

        // If there's a trigger at this cell, do nothing but reset the trigger reference.
        if (this.activeTrigger != null)
        {
            this.activeTrigger = null;
        }

        // Otherwise, if there's an interaction queued, run the interaction.
        else if (this.targetInteraction != null)
        {
            // Face the selected interaction...
            var directionToTarget = this.targetInteraction.Position - this.Gamepiece.Position;
            this.Gamepiece.Direction = Directions.VectorToDirection(directionToTarget);

            // ...and then execute the interaction.
            this.targetInteraction.Run();
            this.targetInteraction = null;
        }

        // No target, but check to see if the player is holding a key down and face in the direction of
        // the last pressed key.
        else
        {
            var inputDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            if (!inputDirection.IsEqualApprox(Vector2.Zero))
            {
                this.Gamepiece.Direction = Directions.VectorToDirection(this.lastInputDirection);
            }
        }
    }

    /// <summary>
    /// Callback when a collision is triggered.
    /// </summary>
    /// <param name="area">The area that was triggered.</param>
    private void OnCollisionTriggered(Area2D area)
    {
        if (area.Owner is Trigger trigger)
        {
            this.activeTrigger = trigger;
        }
    }

    /// <summary>
    /// Callback when the gamepiece's direction changes.
    /// </summary>
    /// <param name="newDirection">The new direction.</param>
    private void OnGamepieceDirectionChanged(Directions.Points newDirection)
    {
        var offset = Directions.Mappings[newDirection] * 16;
        this.interactionSearcher.Position = offset;
    }

    /// <summary>
    /// The player has clicked on an empty gameboard cell. We'll try to move Gamepiece to the cell.
    /// </summary>
    /// <param name="cell">The cell that was selected.</param>
    private void OnCellSelected(Vector2I cell)
    {
        if (this.IsActive && !this.Gamepiece.IsMoving)
        {
            var sourceCell = Gameboard.PixelToCell(this.Gamepiece.Position);

            // Don't move to the cell the focus is standing on.
            if (cell == sourceCell)
            {
                return;
            }

            // Take a look at what's underneath the cursor. If there's an interaction, move towards it
            // and try to interact with it.

            // Otherwise it's just the empty gameboard, so we'll try to move the player towards the
            // selected cell.
            var newPath = Gameboard.Instance.PathFinder.GetPathToCell(sourceCell, cell);
            if (newPath.Count > 0)
            {
                this.MovePath = new List<Vector2I>(newPath);
            }
        }
    }

    /// <summary>
    /// The player has clicked on something interactable. We'll try to move next to the interaction and
    /// then run the interaction.
    /// </summary>
    /// <param name="interaction">The interaction that was selected.</param>
    private void OnInteractionSelected(Interaction interaction)
    {
        if (this.IsActive && !this.Gamepiece.IsMoving)
        {
            var sourceCell = Gameboard.PixelToCell(this.Gamepiece.Position);
            var targetCell = Gameboard.PixelToCell(interaction.Position);

            if (targetCell == sourceCell)
            {
                return;
            }

            // First of all, check to see if the target is adjacent to the source.
            if (Gameboard.GetAdjacentCells(sourceCell).Contains(targetCell))
            {
                this.Gamepiece.Direction = Directions.VectorToDirection(interaction.Position - this.Gamepiece.Position);
                interaction.Run();
            }
            else
            {
                // Only cache the interaction and move towards it if there is a valid move path.
                var newPath = Gameboard.Instance.PathFinder.GetPathCellsToAdjacentCell(sourceCell, targetCell);
                if (newPath.Count > 0)
                {
                    this.targetInteraction = interaction;

                    this.MovePath = new List<Vector2I>(newPath);
                }
            }
        }

        // If the player is already moving, cancel that movement.
        else
        {
            this.StopMoving();
        }
    }
}
