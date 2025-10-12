// <copyright file="PlayerPathDestinationMarker.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Godot;

/// <summary>
/// Marks the destination cell where the player's gamepiece is moving to.
/// Shows when a path is set and hides when the gamepiece arrives.
/// </summary>
[GlobalClass]
public partial class PlayerPathDestinationMarker : Sprite2D
{
    private Gamepiece trackedGamepiece;

    /// <inheritdoc/>
    public override void _Ready()
    {
        GD.Print("PlayerPathDestinationMarker: _Ready() called");
        base._Ready();

        // Get the Player singleton and connect to its signal
        GD.Print($"PlayerPathDestinationMarker: Looking for Player singleton at /root/Player");
        var player = this.GetNode("/root/Player");
        GD.Print($"PlayerPathDestinationMarker: Player node found: {player != null}");
        if (player != null)
        {
            player.Connect("player_path_set", Callable.From((Gamepiece gamepiece, Vector2I destinationCell) =>
            {
                this.OnPlayerPathSet(gamepiece, destinationCell);
            }));
        }
    }

    /// <summary>
    /// Called when the player sets a path for their gamepiece.
    /// </summary>
    /// <param name="gamepiece">The gamepiece that is moving.</param>
    /// <param name="destinationCell">The destination cell coordinates.</param>
    private void OnPlayerPathSet(Gamepiece gamepiece, Vector2I destinationCell)
    {
        // Disconnect from previous gamepiece if any
        if (this.trackedGamepiece != null && this.trackedGamepiece.IsConnected("arrived", Callable.From(this.OnGamepieceArrived)))
        {
            this.trackedGamepiece.Disconnect("arrived", Callable.From(this.OnGamepieceArrived));
        }

        this.trackedGamepiece = gamepiece;

        // Connect to the arrived signal with one-shot flag
        if (!gamepiece.IsConnected("arrived", Callable.From(this.OnGamepieceArrived)))
        {
            gamepiece.Connect("arrived", Callable.From(this.OnGamepieceArrived), (uint)ConnectFlags.OneShot);
        }

        // Get the Gameboard singleton to convert cell to pixel
        var gameboard = this.GetNode("/root/Gameboard");
        if (gameboard != null)
        {
            this.Position = (Vector2)gameboard.Call("cell_to_pixel", destinationCell);
        }

        this.Show();
    }

    /// <summary>
    /// Called when the tracked gamepiece arrives at its destination.
    /// </summary>
    private void OnGamepieceArrived()
    {
        this.Hide();
        this.trackedGamepiece = null;
    }
}
