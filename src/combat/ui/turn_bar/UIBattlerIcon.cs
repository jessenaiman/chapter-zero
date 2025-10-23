
// <copyright file="UIBattlerIcon.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts.Combat.UI.TurnBar;
/// <summary>
/// UI element representing a battler icon in the turn bar.
/// </summary>
[GlobalClass]
[Tool]
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
        /// <summary>
        /// Represents a friendly battler that is not controlled by the player.
        /// </summary>
        Ally = 0,
        /// <summary>
        /// Represents a battler that is controlled by the player.
        /// </summary>
        Player = 1,
        /// <summary>
        /// Represents an enemy battler that acts against the player.
        /// </summary>
        Enemy = 2,
    }

    private static readonly Dictionary<Types, Texture2D?> PortraitBacks = new Dictionary<Types, Texture2D?>
    {
        { Types.Ally, GD.Load<Texture2D>("res://source/combat/ui/turn_bar/portrait_bg_ally.png") },
        { Types.Player, GD.Load<Texture2D>("res://source/combat/ui/turn_bar/portrait_bg_player.png") },
        { Types.Enemy, GD.Load<Texture2D>("res://source/combat/ui/turn_bar/portrait_bg_enemy.png") },
    };

    private Types battlerType;

    /// <summary>
    /// Gets or sets the type of battler represented by the icon.
    /// </summary>
    [Export]
    public Types BattlerType
    {
        get => this.battlerType;
        set
        {
            this.battlerType = value;
            if (PortraitBacks.TryGetValue(this.battlerType, out var texture) && texture != null)
            {
                this.Texture = texture;
            }
        }
    }

    private Texture2D? iconTexture;

    /// <summary>
    /// Gets or sets the icon texture to display.
    /// </summary>
    [Export]
    public Texture2D? IconTexture
    {
        get => this.iconTexture;
        set
        {
            this.iconTexture = value;

            if (!this.IsInsideTree())
            {
                // We'll set the value and wait for the node to be ready
                this.iconTexture = value;
                return;
            }

            if (this.icon != null)
            {
                this.icon.Texture = this.iconTexture;
            }
        }
    }

    /// <summary>
    /// Gets or sets the upper and lower bounds describing the UIBattlerIcon's movement along the x-axis.
    /// The icon moves along the turn bar, whose size is used to determine where and how far the icon
    /// may move.
    /// </summary>
    [Export]
    public Vector2 PositionRange { get; set; } = Vector2.Zero;

    private float progress;

    /// <summary>
    /// Gets or sets determines where on the turn bar the icon is currently located. The value is clamped between 0
    /// and 1.
    /// </summary>
    [Export]
    public float Progress
    {
        get => this.progress;
        set
        {
            this.progress = Mathf.Clamp(value, 0.0f, 1.0f);
            this.Position = new Vector2(Mathf.Lerp(this.PositionRange.X, this.PositionRange.Y, this.progress), this.Position.Y);
        }
    }

    private AnimationPlayer? anim;
    private TextureRect? icon;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.anim = this.GetNode<AnimationPlayer>("AnimationPlayer");
        this.icon = this.GetNode<TextureRect>("Icon");

        // If IconTexture was set before the node was ready, apply it now
        if (this.iconTexture != null)
        {
            this.icon.Texture = this.iconTexture;
        }
    }

    /// <summary>
    /// Fade out the battler icon.
    /// </summary>
    public void FadeOut()
    {
        if (this.anim != null)
        {
            this.anim.Play("fade_out");
        }
    }
}
