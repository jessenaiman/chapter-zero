// <copyright file="Player.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using Godot;

/// <summary>
/// An autoload that provides easy access to the player's state, including both Combat and Field
/// details.
/// Reference to the player's party, inventory, and currently active character are found here.
/// Additionally, game-wide player based signals are emitted from here.
/// </summary>
public partial class Player : Node
{
    /// <summary>
    /// Emitted whenever the player's gamepiece changes.
    /// </summary>
    [Signal]
    public delegate void GamepieceChangedEventHandler();

    /// <summary>
    /// Emitted when the player sets a movement path for their focused gamepiece.
    /// The destination is the last cell in the path.
    /// </summary>
    [Signal]
    public delegate void PlayerPathSetEventHandler(Gamepiece gamepiece, Vector2I destinationCell);

    private Gamepiece? gamepiece;

    /// <summary>
    /// Gets or sets the gamepiece that the player is currently controlling. This is a read-only property.
    /// </summary>
    public Gamepiece Gamepiece
    {
        get => this.gamepiece;
        set
        {
            if (value != this.gamepiece)
            {
                this.gamepiece = value;
                this.EmitSignal(SignalName.GamepieceChanged);
            }
        }
    }
}
