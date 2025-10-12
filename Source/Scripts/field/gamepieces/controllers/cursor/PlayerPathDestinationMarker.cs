using Godot;

/// <summary>
/// Marks the destination cell where the player's gamepiece is moving to.
/// Shows when a path is set and hides when the gamepiece arrives.
/// </summary>
[GlobalClass]
public partial class PlayerPathDestinationMarker : Sprite2D
{
    private Gamepiece _trackedGamepiece;

    public override void _Ready()
    {
        base._Ready();

        // Get the Player singleton and connect to its signal
        var player = GetNode("/root/Player");
        if (player != null)
        {
            player.Connect("player_path_set", Callable.From((Gamepiece gamepiece, Vector2I destinationCell) =>
            {
                OnPlayerPathSet(gamepiece, destinationCell);
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
        if (_trackedGamepiece != null && _trackedGamepiece.IsConnected("arrived", Callable.From(OnGamepieceArrived)))
        {
            _trackedGamepiece.Disconnect("arrived", Callable.From(OnGamepieceArrived));
        }

        _trackedGamepiece = gamepiece;

        // Connect to the arrived signal with one-shot flag
        if (!gamepiece.IsConnected("arrived", Callable.From(OnGamepieceArrived)))
        {
            gamepiece.Connect("arrived", Callable.From(OnGamepieceArrived), (uint)ConnectFlags.OneShot);
        }

        // Get the Gameboard singleton to convert cell to pixel
        var gameboard = GetNode("/root/Gameboard");
        if (gameboard != null)
        {
            Position = (Vector2)gameboard.Call("cell_to_pixel", destinationCell);
        }

        Show();
    }

    /// <summary>
    /// Called when the tracked gamepiece arrives at its destination.
    /// </summary>
    private void OnGamepieceArrived()
    {
        Hide();
        _trackedGamepiece = null;
    }
}
