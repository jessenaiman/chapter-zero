namespace OmegaSpiral.Source.Scripts.Field;

// <copyright file="FieldCamera.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Field.Gameboard;
using OmegaSpiral.Source.Scripts.Field.Gamepieces;

/// <summary>
/// Specialized camera that is constrained to the <see cref="Gameboard"/>'s boundaries.
///
/// The camera's limits are set dynamically according to the viewport's dimensions. Normally, the
/// camera is limited to the gameboard boundaries.
/// <br/><br/>In some cases the gameboard is smaller than the viewport, in which case it will be
/// snapped to the gameboard centre along the constrained axis/axes.
/// </summary>
[GlobalClass]
public partial class FieldCamera : Camera2D
{
    private GameboardProperties? gameboardProperties;

    /// <summary>
    /// Gets or sets the gameboard properties that define the boundaries for the camera.
    /// </summary>
    [Export]
    public GameboardProperties? GameboardProperties
    {
        get => this.gameboardProperties;
        set
        {
            this.gameboardProperties = value;
            this.OnViewportResized();
        }
    }

    private Gamepiece? gamepiece;

    /// <summary>
    /// Gets or sets the gamepiece that the camera will follow.
    /// </summary>
    [Export]
    public Gamepiece? Gamepiece
    {
        get => this.gamepiece;
        set
        {
            this.gamepiece = value;
            if (this.gamepiece != null)
            {
                this.Position = this.gamepiece.Position;
            }
        }
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.GetViewport().SizeChanged += this.OnViewportResized;
        this.OnViewportResized();
    }

    /// <summary>
    /// Reset the camera position to follow the gamepiece.
    /// </summary>
    public void ResetPosition()
    {
        if (this.gamepiece != null)
        {
            this.Position = this.gamepiece.Position * this.Scale;
        }

        this.ResetSmoothing();
    }

    /// <summary>
    /// Called when the viewport is resized to update the camera boundaries.
    /// </summary>
    private void OnViewportResized()
    {
        if (this.gameboardProperties == null)
        {
            return;
        }

        // Calculate tentative camera boundaries based on the gameboard.
        float boundaryLeft = this.gameboardProperties.Extents.Position.X * this.gameboardProperties.CellSize.X;
        float boundaryTop = this.gameboardProperties.Extents.Position.Y * this.gameboardProperties.CellSize.Y;
        float boundaryRight = this.gameboardProperties.Extents.End.X * this.gameboardProperties.CellSize.X;
        float boundaryBottom = this.gameboardProperties.Extents.End.Y * this.gameboardProperties.CellSize.Y;

        // We'll also want the current viewport boundary sizes.
        Vector2 vpSize = this.GetViewportRect().Size / this.GlobalScale;
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
            this.Position = new Vector2(
                (this.gameboardProperties.Extents.Position.X + (this.gameboardProperties.Extents.Size.X / 2.0f)) * this.gameboardProperties.CellSize.X,
                this.Position.Y);

            // And add/subtract half the viewport dimension to come up with the limits. This will fix the
            // camera with the gameboard centred.
            this.LimitLeft = (int) ((this.Position.X - (vpSize.X / 2.0f)) * this.GlobalScale.X);
            this.LimitRight = (int) ((this.Position.X + (vpSize.X / 2.0f)) * this.GlobalScale.X);
        }

        // If, however, the viewport is smaller than the gameplay area, the camera can be free to move
        // as needed.
        else
        {
            this.LimitLeft = (int) (boundaryLeft * this.GlobalScale.X);
            this.LimitRight = (int) (boundaryRight * this.GlobalScale.X);
        }

        // Perform the same checks as above for the y-axis.
        if (boundaryHeight < vpSize.Y)
        {
            this.Position = new Vector2(
                this.Position.X,
                (this.gameboardProperties.Extents.Position.Y + (this.gameboardProperties.Extents.Size.Y / 2.0f)) * this.gameboardProperties.CellSize.Y);
            this.LimitTop = (int) ((this.Position.Y - (vpSize.Y / 2.0f)) * this.GlobalScale.Y);
            this.LimitBottom = (int) ((this.Position.Y + (vpSize.Y / 2.0f)) * this.GlobalScale.Y);
        }
        else
        {
            this.LimitTop = (int) (boundaryTop * this.GlobalScale.Y);
            this.LimitBottom = (int) (boundaryBottom * this.GlobalScale.Y);
        }
    }
}
