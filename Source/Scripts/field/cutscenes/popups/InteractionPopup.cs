using Godot;
using System;
using System.Collections.Generic;

[Tool]
/// <summary>
/// A <see cref="UIPopup"/> used specifically to mark <see cref="Interaction"/>s and other points of interest for the player.
///
/// InteractionPopups may be added as children to a variety of objects. They respond to the player's
/// physics layer and show up as an emote bubble when the player is nearby.
/// </summary>
public partial class InteractionPopup : UIPopup
{
    /// <summary>
    /// The different emote types that may be selected.
    /// </summary>
    public enum EmoteTypes
    {
        Combat,
        Empty,
        Exclamation,
        Question
    }

    /// <summary>
    /// The emote textures that may appear over a point of interest.
    /// </summary>
    private static readonly Dictionary<EmoteTypes, Texture2D> Emotes = new Dictionary<EmoteTypes, Texture2D>
    {
        { EmoteTypes.Combat, GD.Load<Texture2D>("res://assets/gui/emotes/emote_combat.png") },
        { EmoteTypes.Empty, GD.Load<Texture2D>("res://assets/gui/emotes/emote__.png") },
        { EmoteTypes.Exclamation, GD.Load<Texture2D>("res://assets/gui/emotes/emote_exclamations.png") },
        { EmoteTypes.Question, GD.Load<Texture2D>("res://assets/gui/emotes/emote_question.png") }
    };

    private EmoteTypes _emote = EmoteTypes.Empty;
    /// <summary>
    /// The emote bubble that will be displayed when the character is nearby.
    /// </summary>
    [Export]
    public EmoteTypes Emote
    {
        get => _emote;
        set
        {
            _emote = value;

            if (!IsInsideTree())
            {
                // We'll set the value and wait for the node to be ready
                _emote = value;
                return;
            }

            _sprite.Texture = Emotes.ContainsKey(_emote) ? Emotes[_emote] : Emotes[EmoteTypes.Empty];
        }
    }

    private int _radius = 32;
    /// <summary>
    /// How close the player must be to the emote before it will display.
    /// </summary>
    [Export]
    public int Radius
    {
        get => _radius;
        set
        {
            _radius = value;

            if (!IsInsideTree())
            {
                // We'll set the value and wait for the node to be ready
                _radius = value;
                return;
            }

            (_collisionShape.Shape as CircleShape2D).Radius = _radius;
        }
    }

    private bool _isActive = true;
    /// <summary>
    /// Is true if the InteractionPopup should respond to the player's presence. Otherwise, the popup
    /// will not be triggered.
    /// </summary>
    [Export]
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;

            if (!Engine.IsEditorHint())
            {
                if (!IsInsideTree())
                {
                    // We'll set the value and wait for the node to be ready
                    _isActive = value;
                    return;
                }

                _area.Monitoring = _isActive;
                _collisionShape.Disabled = !_isActive;
            }
        }
    }

    private Area2D _area;
    private CollisionShape2D _collisionShape;

    public override void _Ready()
    {
        base._Ready();

        if (!Engine.IsEditorHint())
        {
            // FieldEvents.input_paused.connect(_on_input_paused) - we'll need to implement this when FieldEvents is available
        }
    }

    private void OnAreaEntered(Area2D enteredArea)
    {
        _is_shown = true;
    }

    private void OnAreaExited(Area2D exitedArea)
    {
        _is_shown = false;
    }

    // Be sure to hide input when the player is not able to do anything (e.g. cutscenes).
    private void OnInputPaused(bool paused)
    {
        _area.Monitoring = !paused;
    }

    public override void _EnterTree()
    {
        base._EnterTree();

        _area = GetNode<Area2D>("Area2D");
        _collisionShape = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");

        _area.AreaEntered += OnAreaEntered;
        _area.AreaExited += OnAreaExited;
    }
}
