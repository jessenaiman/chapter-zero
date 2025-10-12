// <copyright file="InteractionPopup.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// A <see cref="UIPopup"/> used specifically to mark <see cref="Interaction"/>s and other points of interest for the player.
///
/// InteractionPopups may be added as children to a variety of objects. They respond to the player's
/// physics layer and show up as an emote bubble when the player is nearby.
/// </summary>
[Tool]
public partial class InteractionPopup : UIPopup
{
    /// <summary>
    /// The emote textures that may appear over a point of interest.
    /// </summary>
    private static readonly Dictionary<EmoteType, Texture2D> Emotes = new Dictionary<EmoteType, Texture2D>
    {
        { EmoteType.Combat, GD.Load<Texture2D>("res://assets/gui/emotes/emote_combat.png") },
        { EmoteType.Empty, GD.Load<Texture2D>("res://assets/gui/emotes/emote__.png") },
        { EmoteType.Exclamation, GD.Load<Texture2D>("res://assets/gui/emotes/emote_exclamations.png") },
        { EmoteType.Question, GD.Load<Texture2D>("res://assets/gui/emotes/emote_question.png") },
    };

    private EmoteType emote = EmoteType.Empty;
    private int radius = 32;
    private bool isActive = true;
    private Area2D? area;
    private CollisionShape2D? collisionShape;

    /// <summary>
    /// The different emote types that may be selected.
    /// </summary>
    public enum EmoteType
    {
        /// <summary>
        /// The combat emote type.
        /// </summary>
        Combat,

        /// <summary>
        /// The empty emote type.
        /// </summary>
        Empty,

        /// <summary>
        /// The exclamation emote type.
        /// </summary>
        Exclamation,

        /// <summary>
        /// The question emote type.
        /// </summary>
        Question,
    }

    /// <summary>
    /// Gets or sets the emote bubble that will be displayed when the character is nearby.
    /// </summary>
    [Export]
    public EmoteType Emote
    {
        get => this.emote;
        set
        {
            this.emote = value;

            if (!this.IsInsideTree())
            {
                // We'll set the value and wait for the node to be ready
                this.emote = value;
                return;
            }

            this.sprite.Texture = Emotes.ContainsKey(this.emote) ? Emotes[this.emote] : Emotes[EmoteType.Empty];
        }
    }

    /// <summary>
    /// Gets or sets how close the player must be to the emote before it will display.
    /// </summary>
    [Export]
    public int Radius
    {
        get => this.radius;
        set
        {
            this.radius = value;

            if (!this.IsInsideTree())
            {
                // We'll set the value and wait for the node to be ready
                this.radius = value;
                return;
            }

            if (this.collisionShape != null && this.collisionShape.Shape is CircleShape2D circleShape)
            {
                circleShape.Radius = this.radius;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether is true if the InteractionPopup should respond to the player's presence. Otherwise, the popup
    /// will not be triggered.
    /// </summary>
    [Export]
    public bool IsActive
    {
        get => this.isActive;
        set
        {
            this.isActive = value;

            if (!Engine.IsEditorHint())
            {
                if (!this.IsInsideTree())
                {
                    // We'll set the value and wait for the node to be ready
                    this.isActive = value;
                    return;
                }

                if (this.area != null)
                {
                    this.area.Monitoring = this.isActive;
                }

                if (this.collisionShape != null)
                {
                    this.collisionShape.Disabled = !this.isActive;
                }
            }
        }
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        if (!Engine.IsEditorHint())
        {
            // FieldEvents.input_paused.connect(_on_input_paused) - we'll need to implement this when FieldEvents is available
        }
    }

    /// <inheritdoc/>
    public override void _EnterTree()
    {
        base._EnterTree();

        this.area = this.GetNode<Area2D>("Area2D");
        this.collisionShape = this.GetNode<CollisionShape2D>("Area2D/CollisionShape2D");

        this.area.AreaEntered += this.OnAreaEntered;
        this.area.AreaExited += this.OnAreaExited;
    }

    private void OnAreaEntered(Area2D enteredArea)
    {
        this.Is_shown = true;
    }

    private void OnAreaExited(Area2D exitedArea)
    {
        this.Is_shown = false;
    }

    // Be sure to hide input when the player is not able to do anything (e.g. cutscenes).
    private void OnInputPaused(bool paused)
    {
        if (this.area != null)
        {
            this.area.Monitoring = !paused;
        }
    }
}
