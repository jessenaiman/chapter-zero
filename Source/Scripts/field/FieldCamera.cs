using Godot;
using System;

/// <summary>
/// Specialized camera that is constrained to the <see cref="Gameboard"/>'s boundaries.
///
/// The camera's limits are set dynamically according to the viewport's dimensions. Normally, the
/// camera is limited to the <see cref="GameboardProperties.Boundaries"/>.
/// <br/><br/>In some cases the gameboard is smaller than the viewport, in which case it will be
/// snapped to the gameboard centre along the constrained axis/axes.
/// </summary>
public partial class FieldCamera : Camera2D
{
    private GameboardProperties _gameboardProperties;
    /// <summary>
    /// The gameboard properties that define the boundaries for the camera.
    /// </summary>
    [Export]
    public GameboardProperties GameboardProperties
    {
        get => _gameboardProperties;
        set
        {
            _gameboardProperties = value;
            _OnViewportResized();
        }
    }

    private Gamepiece _gamepiece;
    /// <summary>
    /// The gamepiece that the camera will follow.
    /// </summary>
    [Export]
    public Gamepiece Gamepiece
    {
        get => _gamepiece;
        set
        {
            if (_gamepiece != null)
            {
                _gamepiece.AnimationTransform.RemotePath = "";
            }

            _gamepiece = value;
            if (_gamepiece != null)
            {
                _gamepiece.AnimationTransform.RemotePath =
                    _gamepiece.AnimationTransform.GetPathTo(this);
            }
        }
    }

    public override void _Ready()
    {
        GetViewport().SizeChanged += _OnViewportResized;
        _OnViewportResized();
    }

    /// <summary>
    /// Reset the camera position to follow the gamepiece.
    /// </summary>
    public void ResetPosition()
    {
        if (_gamepiece != null)
        {
            Position = _gamepiece.Position * Scale;
        }

        ResetSmoothing();
    }

    /// <summary>
    /// Called when the viewport is resized to update the camera boundaries.
    /// </summary>
    private void _OnViewportResized()
    {
        if (_gameboardProperties == null)
        {
            return;
        }

        // Calculate tentative camera boundaries based on the gameboard.
        float boundaryLeft = _gameboardProperties.Extents.Position.X * _gameboardProperties.CellSize.X;
        float boundaryTop = _gameboardProperties.Extents.Position.Y * _gameboardProperties.CellSize.Y;
        float boundaryRight = _gameboardProperties.Extents.End.X * _gameboardProperties.CellSize.X;
        float boundaryBottom = _gameboardProperties.Extents.End.Y * _gameboardProperties.CellSize.Y;

        // We'll also want the current viewport boundary sizes.
        Vector2 vpSize = GetViewportRect().Size / GlobalScale;
        float boundaryWidth = boundaryRight - boundaryLeft;
        float boundaryHeight = boundaryBottom - boundaryTop;

        // If the boundary size is less than the viewport size, the camera limits will be smaller than
        // the camera dimensions (which does all kinds of crazy things in-game).
        // Therefore, if this is the case we'll want to centre the camera on the gameboard and set the
        // limits to be that of the viewport, locking the camera to one or both axes.
        // Start by checking the x-axis.
        // Note that the camera limits must be in global coordinates to function correctly, so account
        // using the global scale.
        if (boundaryWidth < vpSize.X)
        {
            // Set the camera position to the centre of the gameboard.
            Position = new Vector2(
                (_gameboardProperties.Extents.Position.X + _gameboardProperties.Extents.Size.X / 2.0f) * _gameboardProperties.CellSize.X,
                Position.Y
            );

            // And add/subtract half the viewport dimension to come up with the limits. This will fix the
            // camera with the gameboard centred.
            LimitLeft = (int)((Position.X - vpSize.X / 2.0f) * GlobalScale.X);
            LimitRight = (int)((Position.X + vpSize.X / 2.0f) * GlobalScale.X);
        }
        // If, however, the viewport is smaller than the gameplay area, the camera can be free to move
        // as needed.
        else
        {
            LimitLeft = (int)(boundaryLeft * GlobalScale.X);
            LimitRight = (int)(boundaryRight * GlobalScale.X);
        }

        // Perform the same checks as above for the y-axis.
        if (boundaryHeight < vpSize.Y)
        {
            Position = new Vector2(
                Position.X,
                (_gameboardProperties.Extents.Position.Y + _gameboardProperties.Extents.Size.Y / 2.0f) * _gameboardProperties.CellSize.Y
            );
            LimitTop = (int)((Position.Y - vpSize.Y / 2.0f) * GlobalScale.Y);
            LimitBottom = (int)((Position.Y + vpSize.Y / 2.0f) * GlobalScale.Y);
        }
        else
        {
            LimitTop = (int)(boundaryTop * GlobalScale.Y);
            LimitBottom = (int)(boundaryBottom * GlobalScale.Y);
        }
    }
}
