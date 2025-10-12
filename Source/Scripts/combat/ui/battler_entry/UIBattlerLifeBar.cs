using Godot;
using System;

/// <summary>
/// An element of the <see cref="UIBattlerEntry"/> that visually shows player <see cref="Battler"/> life points.
/// The LifeBar also can show an action icon to demonstrate when the player has queued an action for
/// one of their Battlers.
/// </summary>
[GlobalClass]
public partial class UIBattlerLifeBar : TextureProgressBar
{
    /// <summary>
    /// Rate of the animation relative to <see cref="TextureProgressBar.MaxValue"/>.
    /// A value of 1.0 means the animation fills the entire bar in one second.
    /// </summary>
    [Export]
    public float FillRate { get; set; } = 0.5f;

    /// <summary>
    /// The health percentage below which the danger animation plays.
    /// </summary>
    [Export(PropertyHint.Range, "0,1.0")]
    public float DangerCutoff { get; set; } = 0.2f;

    private float _targetValue = 0.0f;

    /// <summary>
    /// When this value changes, the bar smoothly animates towards it using a tween.
    /// </summary>
    public float TargetValue
    {
        get => _targetValue;
        set
        {
            // If the amount is lower than the current TargetValue, it means the battler lost health.
            if (_targetValue > value)
            {
                _anim?.Play("damage");
            }

            _targetValue = value;

            _tween?.Kill();

            var duration = Math.Abs(_targetValue - Value) / MaxValue * FillRate;
            _tween = CreateTween().SetTrans(Tween.TransitionType.Quad);
            _tween.TweenProperty(this, "value", _targetValue, duration);
            _tween.TweenCallback(Callable.From(() =>
            {
                if (Value < DangerCutoff * MaxValue)
                {
                    _anim?.Play("danger");
                }
            }));
        }
    }

    private Tween _tween;
    private AnimationPlayer _anim;
    private Label _nameLabel;
    private TextureRect _queuedActionIcon;
    private Label _valueLabel;

    public override void _Ready()
    {
        base._Ready();

        _anim = GetNode<AnimationPlayer>("AnimationPlayer");
        _nameLabel = GetNode<Label>("MarginContainer/HBoxContainer/Name");
        _queuedActionIcon = GetNode<TextureRect>("MarginContainer/HBoxContainer/QueuedActionIcon");
        _valueLabel = GetNode<Label>("MarginContainer/HBoxContainer/Value");

        ValueChanged += OnValueChanged;
    }

    /// <summary>
    /// Initialize the life bar with battler information.
    /// </summary>
    /// <param name="battlerName">The name of the battler.</param>
    /// <param name="maxHp">The maximum hit points.</param>
    /// <param name="startHp">The starting hit points.</param>
    public void Setup(string battlerName, int maxHp, int startHp)
    {
        _nameLabel.Text = battlerName;
        MaxValue = maxHp;
        Value = startHp;
    }

    /// <summary>
    /// Set the action icon texture to display the queued action.
    /// </summary>
    /// <param name="texture">The texture to display.</param>
    public void SetActionIcon(Texture2D texture)
    {
        _queuedActionIcon.Texture = texture;
    }

    /// <summary>
    /// Callback when the value changes to update the label.
    /// </summary>
    private void OnValueChanged(double newValue)
    {
        _valueLabel.Text = ((int)newValue).ToString();
    }
}
