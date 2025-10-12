// <copyright file="GamepieceRegistry.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// Keeps track of Gamepieces and their positions on the Gameboard.
/// The GamepieceRegistry maintains a mapping of Gamepieces to their current cell positions
/// on the gameboard. This allows for quick lookup of which Gamepiece occupies a given cell
/// and which cell a given Gamepiece occupies.
/// </summary>
public partial class GamepieceRegistry : Node
{
    /// <summary>
    /// Emitted whenever a Gamepiece is added to the registry.
    /// </summary>
    [Signal]
    public delegate void GamepieceAddedEventHandler(Gamepiece gamepiece);

    /// <summary>
    /// Emitted whenever a Gamepiece is removed from the registry.
    /// </summary>
    [Signal]
    public delegate void GamepieceRemovedEventHandler(Gamepiece gamepiece);

    /// <summary>
    /// Emitted whenever a Gamepiece moves to a new cell.
    /// </summary>
    [Signal]
    public delegate void GamepieceMovedEventHandler(Gamepiece gamepiece, Vector2I oldCell, Vector2I newCell);

    /// <summary>
    /// Gets singleton instance of the GamepieceRegistry.
    /// </summary>
    public static GamepieceRegistry Instance { get; private set; }

    /// <summary>
    /// Dictionary mapping Gamepieces to their current cell positions
    /// Key = Gamepiece reference, Value = Cell position.
    /// </summary>
    private Dictionary<Gamepiece, Vector2I> gamepieceToCell = new Dictionary<Gamepiece, Vector2I>();

    /// <summary>
    /// Dictionary mapping cell positions to Gamepieces occupying them
    /// Key = Cell position, Value = Gamepiece reference.
    /// </summary>
    private Dictionary<Vector2I, Gamepiece> cellToGamepiece = new Dictionary<Vector2I, Gamepiece>();

    /// <inheritdoc/>
    public override void _Ready()
    {
        Instance = this;
    }

    /// <summary>
    /// Register a Gamepiece at a specific cell position.
    /// </summary>
    /// <param name="gamepiece">The Gamepiece to register.</param>
    /// <param name="cell">The cell position to register the Gamepiece at.</param>
    public void RegisterGamepiece(Gamepiece gamepiece, Vector2I cell)
    {
        if (gamepiece == null)
        {
            return;
        }

        // If the gamepiece is already registered, unregister it first
        if (this.gamepieceToCell.ContainsKey(gamepiece))
        {
            this.UnregisterGamepiece(gamepiece);
        }

        // Register the gamepiece at the new cell
        this.gamepieceToCell[gamepiece] = cell;
        this.cellToGamepiece[cell] = gamepiece;

        // Connect to the gamepiece's movement signals
        gamepiece.WaypointChanged += (newWaypoint) => this.OnGamepieceWaypointChanged(gamepiece, newWaypoint);

        this.EmitSignal(SignalName.GamepieceAdded, gamepiece);
    }

    /// <summary>
    /// Unregister a Gamepiece from the registry.
    /// </summary>
    /// <param name="gamepiece">The Gamepiece to unregister.</param>
    public void UnregisterGamepiece(Gamepiece gamepiece)
    {
        if (gamepiece == null || !this.gamepieceToCell.ContainsKey(gamepiece))
        {
            return;
        }

        var oldCell = this.gamepieceToCell[gamepiece];

        // Remove the gamepiece from both dictionaries
        this.gamepieceToCell.Remove(gamepiece);
        if (this.cellToGamepiece.ContainsKey(oldCell))
        {
            this.cellToGamepiece.Remove(oldCell);
        }

        this.EmitSignal(SignalName.GamepieceRemoved, gamepiece);
    }

    /// <summary>
    /// Get the cell position of a Gamepiece.
    /// </summary>
    /// <param name="gamepiece">The Gamepiece to get the cell position for.</param>
    /// <returns>The cell position of the Gamepiece, or Gameboard.InvalidCell if not found.</returns>
    public Vector2I GetCell(Gamepiece gamepiece)
    {
        if (gamepiece == null)
        {
            return Gameboard.InvalidCell;
        }

        return this.gamepieceToCell.GetValueOrDefault(gamepiece, Gameboard.InvalidCell);
    }

    /// <summary>
    /// Get the Gamepiece occupying a specific cell.
    /// </summary>
    /// <param name="cell">The cell position to check.</param>
    /// <returns>The Gamepiece occupying the cell, or null if none.</returns>
    public Gamepiece GetGamepiece(Vector2I cell)
    {
        return this.cellToGamepiece.GetValueOrDefault(cell, null);
    }

    /// <summary>
    /// Check if a cell is occupied by a Gamepiece.
    /// </summary>
    /// <param name="cell">The cell position to check.</param>
    /// <returns>True if the cell is occupied, false otherwise.</returns>
    public bool IsCellOccupied(Vector2I cell)
    {
        return this.cellToGamepiece.ContainsKey(cell);
    }

    /// <summary>
    /// Move a Gamepiece to a new cell position.
    /// </summary>
    /// <param name="gamepiece">The Gamepiece to move.</param>
    /// <param name="newCell">The new cell position.</param>
    public void MoveGamepiece(Gamepiece gamepiece, Vector2I newCell)
    {
        if (gamepiece == null || !this.gamepieceToCell.ContainsKey(gamepiece))
        {
            return;
        }

        var oldCell = this.gamepieceToCell[gamepiece];

        // If the new cell is the same as the old cell, do nothing
        if (oldCell == newCell)
        {
            return;
        }

        // Remove the gamepiece from its old cell
        if (this.cellToGamepiece.ContainsKey(oldCell))
        {
            this.cellToGamepiece.Remove(oldCell);
        }

        // Update the gamepiece's cell mapping
        this.gamepieceToCell[gamepiece] = newCell;

        // Add the gamepiece to its new cell
        this.cellToGamepiece[newCell] = gamepiece;

        this.EmitSignal(SignalName.GamepieceMoved, gamepiece, oldCell, newCell);
    }

    /// <summary>
    /// Get all registered Gamepieces.
    /// </summary>
    /// <returns>A list of all registered Gamepieces.</returns>
    public List<Gamepiece> GetAllGamepieces()
    {
        return new List<Gamepiece>(this.gamepieceToCell.Keys);
    }

    /// <summary>
    /// Get all cell positions that are occupied by Gamepieces.
    /// </summary>
    /// <returns>A list of all occupied cell positions.</returns>
    public List<Vector2I> GetAllOccupiedCells()
    {
        return new List<Vector2I>(this.cellToGamepiece.Keys);
    }

    /// <summary>
    /// Callback when a Gamepiece's waypoint changes.
    /// </summary>
    /// <param name="gamepiece">The Gamepiece whose waypoint changed.</param>
    /// <param name="newWaypoint">The new waypoint position.</param>
    private void OnGamepieceWaypointChanged(Gamepiece gamepiece, Vector2I newWaypoint)
    {
        // When a gamepiece's waypoint changes, update its registered cell position
        if (this.gamepieceToCell.ContainsKey(gamepiece))
        {
            this.MoveGamepiece(gamepiece, newWaypoint);
        }
        else
        {
            this.RegisterGamepiece(gamepiece, newWaypoint);
        }
    }

    /// <summary>
    /// Get all Gamepieces within a rectangular area.
    /// </summary>
    /// <param name="area">The rectangular area to check.</param>
    /// <returns>A list of Gamepieces within the area.</returns>
    public List<Gamepiece> GetGamepiecesInArea(Rect2I area)
    {
        var gamepieces = new List<Gamepiece>();

        foreach (var kvp in this.cellToGamepiece)
        {
            if (area.HasPoint(kvp.Key))
            {
                gamepieces.Add(kvp.Value);
            }
        }

        return gamepieces;
    }

    /// <summary>
    /// Get the nearest Gamepiece to a given position.
    /// </summary>
    /// <param name="position">The position to find the nearest Gamepiece to.</param>
    /// <param name="maxDistance">The maximum distance to consider.</param>
    /// <returns>The nearest Gamepiece, or null if none found within maxDistance.</returns>
    public Gamepiece GetNearestGamepiece(Vector2 position, float maxDistance = float.MaxValue)
    {
        Gamepiece nearest = null;
        var minDistanceSquared = maxDistance * maxDistance;

        foreach (var kvp in this.gamepieceToCell)
        {
            var gamepiece = kvp.Key;
            var cell = kvp.Value;

            var gamepiecePosition = Gameboard.CellToPixel(cell);
            var distanceSquared = position.DistanceSquaredTo(gamepiecePosition);

            if (distanceSquared < minDistanceSquared)
            {
                minDistanceSquared = distanceSquared;
                nearest = gamepiece;
            }
        }

        return nearest;
    }

    /// <summary>
    /// Check if a path is blocked by any Gamepieces.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <returns>True if the path is blocked, false otherwise.</returns>
    public bool IsPathBlocked(List<Vector2I> path)
    {
        if (path == null || path.Count == 0)
        {
            return false;
        }

        // Check each cell in the path (except the first, which is the starting position)
        for (int i = 1; i < path.Count; i++)
        {
            if (this.IsCellOccupied(path[i]))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Get all Gamepieces of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of Gamepiece to find.</typeparam>
    /// <returns>A list of all Gamepieces of the specified type.</returns>
    public List<T> GetGamepiecesOfType<T>()
        where T : Gamepiece
    {
        var gamepieces = new List<T>();

        foreach (var gamepiece in this.gamepieceToCell.Keys)
        {
            if (gamepiece is T typedGamepiece)
            {
                gamepieces.Add(typedGamepiece);
            }
        }

        return gamepieces;
    }
}
