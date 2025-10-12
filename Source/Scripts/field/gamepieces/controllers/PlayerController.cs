using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
	private Interaction _targetInteraction = null;

	/// <summary>
	/// Keep track of any Triggers that the player has stepped on.
	/// </summary>
	private Trigger _activeTrigger = null;

	/// <summary>
	/// Also keep track of the most recently pressed move key (e.g. WASD keys). This makes keyboard input
	/// feel more intuitive, since the gamepiece will move towards the most recently pressed key rather
	/// than prefering an arbitrary axis.
	/// </summary>
	private Vector2 _lastInputDirection = Vector2.Zero;

	/// <summary>
	/// The "interaction searcher" area basically activates any Interactions, which means that they'll
	/// respond to key/button input.
	/// </summary>
	private Area2D _interactionSearcher;

	/// <summary>
	/// The collision shape for the interaction searcher.
	/// </summary>
	private CollisionShape2D _interactionShape;

	/// <summary>
	/// The player collision area activates Triggers whenever the player moves onto their collision
	/// shape.
	/// </summary>
	private Area2D _playerCollision;

	public override void _Ready()
	{
		base._Ready();

		if (!Engine.IsEditorHint())
		{
			AddToGroup(Group);

			// Get references to child nodes
			_interactionSearcher = GetNode<Area2D>("InteractionSearcher");
			_interactionShape = GetNode<CollisionShape2D>("InteractionSearcher/CollisionShape2D");
			_playerCollision = GetNode<Area2D>("PlayerCollision");

			// Refer the various player collision shapes to their gamepiece (parent of the controller).
			// This will allow other objects/systems to quickly find which gamepiece they are working on
			// via the collision "owners".
			_interactionSearcher.Owner = Gamepiece;
			_playerCollision.Owner = Gamepiece;

			// Update the position of the player's collision shape to match the cell that it is currently
			// moving towards.
			Gamepiece.WaypointChanged += (newWaypoint) =>
			{
				if (newWaypoint == Gameboard.InvalidCell)
				{
					_playerCollision.Position = Vector2.Zero;
				}
				else
				{
					_playerCollision.Position = Gameboard.CellToPixel(newWaypoint) - Gamepiece.Position;
				}
			};

			// The player collision picks up any triggers that it moves over. Keep track of them until
			// player movement to the current cells has completed.
			_playerCollision.AreaEntered += (_onCollisionTriggered);

			Gamepiece.DirectionChanged += (_onGamepieceDirectionChanged);

			// Connect to field events
			FieldEvents.Instance.CellSelected += _OnCellSelected;
			FieldEvents.Instance.InteractionSelected += _OnInteractionSelected;
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
				_lastInputDirection = Vector2.Up;
				if (Gamepiece.IsMoving) StopMoving();
				else MoveToPressedKey(Vector2.Up);
			}
			else if (keyEvent.IsActionPressed("ui_down"))
			{
				_lastInputDirection = Vector2.Down;
				if (Gamepiece.IsMoving) StopMoving();
				else MoveToPressedKey(Vector2.Down);
			}
			else if (keyEvent.IsActionPressed("ui_left"))
			{
				_lastInputDirection = Vector2.Left;
				if (Gamepiece.IsMoving) StopMoving();
				else MoveToPressedKey(Vector2.Left);
			}
			else if (keyEvent.IsActionPressed("ui_right"))
			{
				_lastInputDirection = Vector2.Right;
				if (Gamepiece.IsMoving) StopMoving();
				else MoveToPressedKey(Vector2.Right);
			}
		}
	}

	/// <summary>
	/// Move the gamepiece along a path.
	/// </summary>
	/// <param name="path">The path to follow</param>
	public override void MoveAlongPath(List<Vector2I> path)
	{
		base.MoveAlongPath(new List<Vector2I>(path));

		_interactionShape.SetDeferred("disabled", true);
		Player.Instance.EmitSignal(Player.SignalName.PlayerPathSet, Gamepiece, path[^1]);
	}

	/// <summary>
	/// Move the gamepiece towards a pressed key direction.
	/// </summary>
	/// <param name="inputDirection">The direction to move</param>
	public override void MoveToPressedKey(Vector2 inputDirection)
	{
		if (IsActive)
		{
			var sourceCell = GamepieceRegistry.Instance.GetCell(Gamepiece);
			var targetCell = Vector2I.Zero;

			// Unless using 8-direction movement, one movement axis must be preferred.
			// Default to the x-axis.
			targetCell = sourceCell + new Vector2I((int)inputDirection.X, (int)inputDirection.Y);

			// Try to get a path to destination (will fail if cell is occupied)
			var newMovePath = Gameboard.Instance.PathFinder.GetPathToCell(sourceCell, targetCell);

			// Path is invalid. Bump animation?
			if (newMovePath.Count < 1)
			{
				Gamepiece.Direction = Directions.AngleToDirection(inputDirection.Angle());
			}
			else
			{
				MovePath = new List<Vector2I>(newMovePath);
			}
		}
	}

	/// <summary>
	/// Stop the gamepiece's movement.
	/// </summary>
	public override void StopMoving()
	{
		base.StopMoving();
		_targetInteraction = null;
	}

	/// <summary>
	/// Callback when the player's gamepiece arrives at a waypoint.
	/// </summary>
	/// <param name="excessDistance">The excess distance when arriving</param>
	protected override void OnGamepieceArriving(float excessDistance)
	{
		// If the gamepiece moved onto a trigger, stop the gamepiece in its tracks.
		if (_activeTrigger != null)
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
				MoveToPressedKey(_lastInputDirection);
			}
		}
	}

	/// <summary>
	/// Callback when the gamepiece has arrived at a waypoint.
	/// </summary>
	protected override void OnGamepieceArrived()
	{
		base.OnGamepieceArrived();

		_playerCollision.Position = Vector2.Zero;
		_interactionShape.SetDeferred("disabled", false);

		// If there's a trigger at this cell, do nothing but reset the trigger reference.
		if (_activeTrigger != null)
		{
			_activeTrigger = null;
		}
		// Otherwise, if there's an interaction queued, run the interaction.
		else if (_targetInteraction != null)
		{
			// Face the selected interaction...
			var directionToTarget = _targetInteraction.Position - Gamepiece.Position;
			Gamepiece.Direction = Directions.VectorToDirection(directionToTarget);

			// ...and then execute the interaction.
			_targetInteraction.Run();
			_targetInteraction = null;
		}
		// No target, but check to see if the player is holding a key down and face in the direction of
		// the last pressed key.
		else
		{
			var inputDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
			if (!inputDirection.IsEqualApprox(Vector2.Zero))
			{
				Gamepiece.Direction = Directions.VectorToDirection(_lastInputDirection);
			}
		}
	}

	/// <summary>
	/// Callback when a collision is triggered.
	/// </summary>
	/// <param name="area">The area that was triggered</param>
	private void _onCollisionTriggered(Area2D area)
	{
		if (area.Owner is Trigger trigger)
		{
			_activeTrigger = trigger;
		}
	}

	/// <summary>
	/// Callback when the gamepiece's direction changes.
	/// </summary>
	/// <param name="newDirection">The new direction</param>
	private void _onGamepieceDirectionChanged(Directions.Points newDirection)
	{
		var offset = Directions.Mappings[newDirection] * 16;
		_interactionSearcher.Position = offset;
	}

	/// <summary>
	/// The player has clicked on an empty gameboard cell. We'll try to move Gamepiece to the cell.
	/// </summary>
	/// <param name="cell">The cell that was selected</param>
	private void _OnCellSelected(Vector2I cell)
	{
		if (IsActive && !Gamepiece.IsMoving)
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
			var newPath = Gameboard.Instance.PathFinder.GetPathToCell(sourceCell, cell);
			if (newPath.Count > 0)
			{
				MovePath = new List<Vector2I>(newPath);
			}
		}
	}

	/// <summary>
	/// The player has clicked on something interactable. We'll try to move next to the interaction and
	/// then run the interaction.
	/// </summary>
	/// <param name="interaction">The interaction that was selected</param>
	private void _OnInteractionSelected(Interaction interaction)
	{
		if (IsActive && !Gamepiece.IsMoving)
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
				var newPath = Gameboard.Instance.PathFinder.GetPathCellsToAdjacentCell(sourceCell, targetCell);
				if (newPath.Count > 0)
				{
					_targetInteraction = interaction;

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
}
