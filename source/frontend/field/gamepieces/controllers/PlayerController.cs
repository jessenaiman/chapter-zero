
// <copyright file="PlayerController.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Field.cutscenes;
using System.Collections.ObjectModel;

namespace OmegaSpiral.Source.Scripts.Field.gamepieces.Controllers;
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
    private Interaction? targetInteraction;

    /// <summary>
    /// Keep track of any Triggers that the player has stepped on.
    /// </summary>
    private Trigger? activeTrigger;

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
    private Area2D? interactionSearcher;

    /// <summary>
    /// The collision shape for the interaction searcher.
    /// </summary>
    private CollisionShape2D? interactionShape;

    /// <summary>
    /// The player collision area activates Triggers whenever the player moves onto their collision
    /// shape.
    /// </summary>
    private Area2D? playerCollision;

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
                if (newWaypoint == gameboard.Gameboard.InvalidCell)
                {
                    if (this.playerCollision != null)
                    {
                        this.playerCollision.Position = Vector2.Zero;
                    }
                }
                else
                {
                    var gameboard = this.GetNodeOrNull<global::OmegaSpiral.Source.Scripts.Field.gameboard.Gameboard>("/root/Gameboard");
                    if (this.playerCollision != null && gameboard != null)
                    {
                        this.playerCollision.Position = gameboard.CellToPixel(newWaypoint) - this.Gamepiece.Position;
                    }
                }
            };

            // The player collision picks up any triggers that it moves over. Keep track of them until
            // player movement to the current cells has completed.
            this.playerCollision.AreaEntered += this.OnCollisionTriggered;

            this.Gamepiece.DirectionChanged += this.OnGamepieceDirectionChanged;

            // Connect to field events
            var fieldEvents = this.GetNodeOrNull<FieldEvents>("/root/FieldEvents");
            if (fieldEvents != null)
            {
                fieldEvents.CellSelected += this.OnCellSelected;
                fieldEvents.InteractionSelected += this.OnInteractionSelected;
            }
        }
    }

    /// <inheritdoc/>
    public override void _UnhandledInput(InputEvent @event)
    {
        ArgumentNullException.ThrowIfNull(@event);

        // Handle select action release
        if (@event.IsActionReleased("select"))
        {
            this.StopMoving();
            return;
        }

        // Only process key events
        if (@event is not InputEventKey keyEvent)
        {
            return;
        }

        // Handle movement keys
        this.ProcessMovementKey(keyEvent);
    }

    /// <summary>
    /// Processes movement key input.
    /// </summary>
    /// <param name="keyEvent">The key event to process.</param>
    private void ProcessMovementKey(InputEventKey keyEvent)
    {
        Vector2 direction = this.GetDirectionFromKey(keyEvent);
        if (direction == Vector2.Zero)
        {
            return;
        }

        this.lastInputDirection = direction;
        if (this.Gamepiece.IsMoving)
        {
            this.StopMoving();
        }
        else
        {
            this.MoveToPressedKey(direction);
        }
    }

    /// <summary>
    /// Gets the movement direction from a key event.
    /// </summary>
    /// <param name="keyEvent">The key event.</param>
    /// <returns>The movement direction, or Vector2.Zero if not a movement key.</returns>
    private Vector2 GetDirectionFromKey(InputEventKey keyEvent)
    {
        if (keyEvent.IsActionPressed("ui_up")) return Vector2.Up;
        if (keyEvent.IsActionPressed("ui_down")) return Vector2.Down;
        if (keyEvent.IsActionPressed("ui_left")) return Vector2.Left;
        if (keyEvent.IsActionPressed("ui_right")) return Vector2.Right;
        return Vector2.Zero;
    }

    /// <summary>
    /// Move the gamepiece along a path.
    /// </summary>
    /// <param name="path">The path to follow.</param>
    public override void MoveAlongPath(System.Collections.ObjectModel.ReadOnlyCollection<Vector2I> path)
    {
        base.MoveAlongPath(path);

        if (this.interactionShape != null)
        {
            this.interactionShape.SetDeferred("disabled", true);
        }

        var player = this.GetNodeOrNull<Player>("/root/Player");
        if (player != null && path.Count > 0)
        {
            player.EmitSignal(Player.SignalName.PlayerPathSet, this.Gamepiece, path[^1]);
        }
    }

    /// <summary>
    /// Move the gamepiece towards a pressed key direction.
    /// </summary>
    /// <param name="direction">The direction to move.</param>
    public override void MoveToPressedKey(Vector2 direction)
    {
        if (this.IsActive)
        {
            var gamepieceRegistry = this.GetNodeOrNull<GamepieceRegistry>("/root/GamepieceRegistry");
            var sourceCell = gamepieceRegistry?.GetCell(this.Gamepiece) ?? Vector2I.Zero;
            var targetCell = Vector2I.Zero;

            // Unless using 8-direction movement, one movement axis must be preferred.
            // Default to the x-axis.
            targetCell = sourceCell + new Vector2I((int) direction.X, (int) direction.Y);

            // Try to get a path to destination (will fail if cell is occupied)
            var gameboard = this.GetNodeOrNull<global::OmegaSpiral.Source.Scripts.Field.gameboard.Gameboard>("/root/Gameboard");
            var newMovePath = gameboard?.PathFinder?.GetPathToCell(sourceCell, targetCell) ?? new List<Vector2I>();

            // Path is invalid. Bump animation?
            if (newMovePath.Count < 1)
            {
                this.Gamepiece.Direction = Directions.AngleToDirection(direction.Angle());
            }
            else
            {
                var currentPath = new List<Vector2I>(newMovePath);
                this.MoveAlongPath(new ReadOnlyCollection<Vector2I>(currentPath));
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

        if (this.playerCollision != null)
        {
            this.playerCollision.Position = Vector2.Zero;
        }
        if (this.interactionShape != null)
        {
            this.interactionShape.SetDeferred("disabled", false);
        }

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
    private void OnGamepieceDirectionChanged(Directions.Point newDirection)
    {
        var offset = Directions.Mappings[newDirection] * 16;
        if (this.interactionSearcher != null)
        {
            this.interactionSearcher.Position = offset;
        }
    }

    /// <summary>
    /// The player has clicked on an empty gameboard cell. We'll try to move Gamepiece to the cell.
    /// </summary>
    /// <param name="cell">The cell that was selected.</param>
    private void OnCellSelected(Vector2I cell)
    {
        // Validate conditions for movement
        if (!this.CanMoveToCell(cell, out var path))
        {
            return;
        }

        // Execute the movement
        var pathList = new List<Vector2I>(path);
        this.MoveAlongPath(new ReadOnlyCollection<Vector2I>(pathList));
    }

    /// <summary>
    /// Determines if the gamepiece can move to the specified cell.
    /// </summary>
    /// <param name="cell">The target cell.</param>
    /// <param name="path">The calculated path if movement is possible.</param>
    /// <returns>True if movement is possible, false otherwise.</returns>
    private bool CanMoveToCell(Vector2I cell, out IReadOnlyCollection<Vector2I> path)
    {
        path = new List<Vector2I>();

        // Check basic movement conditions
        if (!this.IsActive || this.Gamepiece.IsMoving)
        {
            return false;
        }

        var gameboard = this.GetNodeOrNull<global::OmegaSpiral.Source.Scripts.Field.gameboard.Gameboard>("/root/Gameboard");
        if (gameboard == null)
        {
            return false;
        }

        var sourceCell = gameboard.PixelToCell(this.Gamepiece.Position);

        // Don't move to the same cell
        if (cell == sourceCell)
        {
            return false;
        }

        // Calculate path
        var calculatedPath = gameboard.PathFinder?.GetPathToCell(sourceCell, cell);
        if (calculatedPath == null || calculatedPath.Count == 0)
        {
            return false;
        }

        path = calculatedPath;
        return true;
    }

    /// <summary>
    /// The player has clicked on something interactable. We'll try to move next to the interaction and
    /// then run the interaction.
    /// </summary>
    /// <param name="interaction">The interaction that was selected.</param>
    private void OnInteractionSelected(Interaction interaction)
    {
        if (!this.CanInteract(interaction))
        {
            return;
        }

        var gameboard = this.GetNodeOrNull<global::OmegaSpiral.Source.Scripts.Field.gameboard.Gameboard>("/root/Gameboard");
        if (gameboard == null)
        {
            return;
        }

        var sourceCell = gameboard.PixelToCell(this.Gamepiece.Position);
        var targetCell = gameboard.PixelToCell(interaction.Position);

        if (sourceCell == targetCell)
        {
            return;
        }

        this.ExecuteOrMoveToInteraction(interaction, gameboard, sourceCell, targetCell);
    }

    /// <summary>
    /// Checks if the player can currently interact.
    /// </summary>
    private bool CanInteract(Interaction interaction)
    {
        if (!this.IsActive || this.Gamepiece.IsMoving)
        {
            this.StopMoving();
            return false;
        }
        return true;
    }

    /// <summary>
    /// Executes the interaction if adjacent, otherwise moves towards it.
    /// </summary>
    private void ExecuteOrMoveToInteraction(Interaction interaction, global::OmegaSpiral.Source.Scripts.Field.gameboard.Gameboard gameboard, Vector2I sourceCell, Vector2I targetCell)
    {
        var adjacentCells = gameboard.GetAdjacentCells(sourceCell);
        if (adjacentCells.Contains(targetCell))
        {
            this.Gamepiece.Direction = Directions.VectorToDirection(interaction.Position - this.Gamepiece.Position);
            interaction.Run();
        }
        else
        {
            var path = gameboard.PathFinder?.GetPathCellsToAdjacentCell(sourceCell, targetCell) ?? new List<Vector2I>();
            if (path.Count > 0)
            {
                this.targetInteraction = interaction;
                this.MoveAlongPath(new ReadOnlyCollection<Vector2I>(new List<Vector2I>(path)));
            }
        }
    }

    /// <summary>
    /// Processes the interaction logic.
    /// </summary>
    /// <param name="interaction">The interaction to process.</param>
    private void ProcessInteraction(Interaction interaction)
    {
        var gameboard = this.GetNodeOrNull<global::OmegaSpiral.Source.Scripts.Field.gameboard.Gameboard>("/root/Gameboard");
        if (gameboard == null)
        {
            return;
        }

        var sourceCell = gameboard.PixelToCell(this.Gamepiece.Position);
        var targetCell = gameboard.PixelToCell(interaction.Position);

        // Don't interact with self
        if (targetCell == sourceCell)
        {
            return;
        }

        // Try immediate interaction if adjacent
        var adjacentCells = gameboard.GetAdjacentCells(sourceCell);
        if (adjacentCells.Contains(targetCell))
        {
            this.ExecuteInteraction(interaction);
            return;
        }

        // Try to move towards interaction
        this.MoveTowardsInteraction(gameboard, sourceCell, targetCell, interaction);
    }

    /// <summary>
    /// Executes an interaction immediately.
    /// </summary>
    /// <param name="interaction">The interaction to execute.</param>
    private void ExecuteInteraction(Interaction interaction)
    {
        this.Gamepiece.Direction = Directions.VectorToDirection(interaction.Position - this.Gamepiece.Position);
        interaction.Run();
    }

    /// <summary>
    /// Attempts to move towards an interaction.
    /// </summary>
    /// <param name="gameboard">The gameboard instance.</param>
    /// <param name="sourceCell">The source cell.</param>
    /// <param name="targetCell">The target cell.</param>
    /// <param name="interaction">The interaction to move towards.</param>
    private void MoveTowardsInteraction(
        global::OmegaSpiral.Source.Scripts.Field.gameboard.Gameboard gameboard,
        Vector2I sourceCell,
        Vector2I targetCell,
        Interaction interaction)
    {
        var path = gameboard.PathFinder?.GetPathCellsToAdjacentCell(sourceCell, targetCell) ?? new List<Vector2I>();
        if (path.Count > 0)
        {
            this.targetInteraction = interaction;
            var pathList = new List<Vector2I>(path);
            this.MoveAlongPath(new ReadOnlyCollection<Vector2I>(pathList));
        }
    }
}
