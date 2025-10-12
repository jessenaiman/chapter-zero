// <copyright file="MovingInteractionPopup.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using Godot;

/// <summary>
/// An <see cref="InteractionPopup"/> that follows a moving <see cref="Gamepiece"/>.
///
/// This Popup must be a child of a <see cref="Gamepiece"/> to function.
///
/// Note that other popup types will jump to the occupied cell of the ancestor <see cref="Gamepiece"/>, whereas
/// MovingInteractionPopups sync their position to that of the gamepiece's graphical representation.
/// </summary>
[Tool]
public partial class MovingInteractionPopup : InteractionPopup
{
    private Gamepiece? gp;

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        // Do not follow anything in editor or if this object's parent is not of the correct type.
        if (Engine.IsEditorHint() || this.gp == null)
        {
            this.SetProcess(false);
        }
    }

    /// <summary>
    /// Gets configuration warnings for this popup.
    /// </summary>
    /// <returns>An array of warning messages, or an empty array if there are no warnings.</returns>
    public string[] GetConfigurationWarnings()
    {
        if (this.gp == null)
        {
            return new string[] { "This popup must be a child of a Gamepiece node!" };
        }

        return Array.Empty<string>();
    }

    /// <inheritdoc/>
    public override void _Notification(int what)
    {
        base._Notification(what);

        if (what == NotificationParented)
        {
            this.gp = this.GetParent() as Gamepiece;

            // UpdateConfigurationWarnings() - Godot doesn't have a direct equivalent in C#
        }
    }

    // Every process frame the popup sets its position to that of the graphical representation of the
    // gamepiece, appearing to follow the gamepiece around the field while still playing nicely with the
    // physics/interaction system.
    /// <inheritdoc/>
    public override void _Process(double delta)
    {
        if (this.gp != null)
        {
            this.Position = this.gp.Follower.Position;
        }
    }

    /// <inheritdoc/>
    public override void _EnterTree()
    {
        base._EnterTree();
        this.gp = this.GetParent() as Gamepiece;
    }
}
