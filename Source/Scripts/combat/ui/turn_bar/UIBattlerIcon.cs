using Godot;
using System;
using System.Collections.Generic;

[Tool]
/// <summary>
/// UI element representing a battler icon in the turn bar.
/// </summary>
public partial class UIBattlerIcon : TextureRect
{
    /// <summary>
    /// Describe the type of Battler represented by the icon:
    /// <br/> - Allies are friendly battlers that are not controlled by the player.
    /// <br/> - Player battlers are almost always player-controlled (unless asleep/berzerked, for example).
    /// <br/> - Enemy battlers act against the player and must be defeated.
    /// </summary>
    public enum Types
    {
        Ally,
        Player,
        Enemy
    }

    private static readonly Dictionary<Types, Texture2D> PortraitBacks = new Dictionary<Types, Texture2D>
    {
        { Types.Ally, GD.Load<Texture2D>("res://combat/ui/turn_bar/portrait_bg_ally.png") },
        { Types.Player, GD.Load<Texture2D>("res://combat/ui/turn_bar/portrait_bg_player.png") },
        { Types.Enemy, GD.Load<Texture2D>("res://combat/ui/turn_bar/portrait_bg_enemy.png") }
    };

    private Types _battlerType;
    /// <summary>
    /// The type of battler represented by the icon
    /// </summary>
    [Export]
    public Types BattlerType
    {
        get => _battlerType;
        set
        {
            _battlerType = value;
            Texture = PortraitBacks[_battlerType];
        }
    }

    private Texture2D _iconTexture;
    /// <summary>
    /// The icon texture to display
    /// </summary>
    [Export]
    public Texture2D IconTexture
    {
        get => _iconTexture;
        set
        {
            _iconTexture = value;

            if (!IsInsideTree())
            {
                // We'll set the value and wait for the node to be ready
                _iconTexture = value;
                return;
            }
            _icon.Texture = _iconTexture;
        }
    }

    /// <summary>
    /// The upper and lower bounds describing the UIBattlerIcon's movement along the x-axis.
    /// The icon moves along the turn bar, whose size is used to determine where and how far the icon
    /// may move.
    /// </summary>
    [Export]
    public Vector2 PositionRange { get; set; } = Vector2.Zero;

    private float _progress = 0.0f;
    /// <summary>
    /// Determines where on the turn bar the icon is currently located. The value is clamped between 0
    /// and 1.
    /// </summary>
    [Export]
    public float Progress
    {
        get => _progress;
        set
        {
            _progress = Mathf.Clamp(value, 0.0f, 1.0f);
            Position = new Vector2(Mathf.Lerp(PositionRange.X, PositionRange.Y, _progress), Position.Y);
        }
    }

    private AnimationPlayer _anim;
    private TextureRect _icon;

    public override void _Ready()
    {
        _anim = GetNode<AnimationPlayer>("AnimationPlayer");
        _icon = GetNode<TextureRect>("Icon");

        // If IconTexture was set before the node was ready, apply it now
        if (_iconTexture != null)
        {
            _icon.Texture = _iconTexture;
        }
    }

    /// <summary>
    /// Fade out the battler icon
    /// </summary>
    public void FadeOut()
    {
        _anim.Play("fade_out");
    }
}
