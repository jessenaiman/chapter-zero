using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Encapsulates Gamepiece animation as an optional component.
/// <br/><br/>Allows playing animations that automatically adapt to the parent
/// Gamepiece's direction by calling Play. Transitions between
/// animations are handled automatically, including changes to direction.
/// <br/><br/><b>Note:</b> This is usually not added to the scene tree directly by
/// the designer.
/// <br/><br/>Rather, it is typically added to a Gamepiece through the Gamepiece.AnimationScene
/// property.
/// </summary>
[Tool]
[Icon("res://assets/editor/icons/GamepieceAnimation.svg")]
[GlobalClass]
public partial class GamepieceAnimation : Marker2D
{
    /// <summary>
    /// Name of the animation sequence used to reset animation properties to default.
    /// Note that this animation is only played for a single frame during animation
    /// transitions.
    /// </summary>
    private const string ResetSequenceKey = "RESET";

    /// <summary>
    /// Mapping that pairs cardinal Directions.Points with a string suffix.
    /// </summary>
    private static readonly Dictionary<Directions.Points, string> DirectionSuffixes = new Dictionary<Directions.Points, string>
    {
        { Directions.Points.North, "_n" },
        { Directions.Points.East, "_e" },
        { Directions.Points.South, "_s" },
        { Directions.Points.West, "_w" }
    };

    /// <summary>
    /// The animation currently being played.
    /// </summary>
    private string currentSequenceId = "";

    /// <summary>
    /// The direction faced by the gamepiece.
    /// <br/><br/>Animations may optionally be direction-based. Setting the direction will use
    /// directional animations if they are available; otherwise non-directional
    /// animations will be used.
    /// </summary>
    private Directions.Points direction = Directions.Points.South;

    /// <summary>
    /// Animation player for handling animations.
    /// </summary>
    private AnimationPlayer _anim;

    /// <summary>
    /// The animation currently being played.
    /// </summary>
    public string CurrentSequenceId
    {
        get => currentSequenceId;
        set => Play(value);
    }

    /// <summary>
    /// The direction faced by the gamepiece.
    /// </summary>
    public Directions.Points Direction
    {
        get => direction;
        set => SetDirection(value);
    }

    public override void _Ready()
    {
        _anim = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    /// <summary>
    /// Change the currently playing animation to a new value, if it exists.
    /// <br/><br/>Animations may be added with or without a directional suffix (i.e. _n for
    /// north/up). Directional animations will be preferred with direction-less
    /// animations as a fallback.
    /// </summary>
    /// <param name="value">The animation sequence to play</param>
    public async void Play(string value)
    {
        if (value == currentSequenceId)
        {
            return;
        }

        if (!IsInsideTree())
        {
            await ToSignal(this, Node.SignalName.Ready);
        }

        // We need to check to see if the animation is valid. First of all, look for
        // a directional equivalent - e.g. idle_n. If that fails, look for the new
        // sequence id itself.
        var sequenceSuffix = DirectionSuffixes.ContainsKey(direction) ? DirectionSuffixes[direction] : "";

        if (_anim.HasAnimation(value + sequenceSuffix))
        {
            currentSequenceId = value;
            _SwapAnimation(value + sequenceSuffix, false);
        }
        else if (_anim.HasAnimation(value))
        {
            currentSequenceId = value;
            _SwapAnimation(value, false);
        }
    }

    /// <summary>
    /// Change the animation's direction.
    /// <br/><br/>If the currently running animation has a directional variant matching the new
    /// direction it will be played. Otherwise the direction-less animation will
    /// play.
    /// </summary>
    /// <param name="value">The new direction</param>
    public async void SetDirection(Directions.Points value)
    {
        if (value == direction)
        {
            return;
        }

        direction = value;

        if (!IsInsideTree())
        {
            await ToSignal(this, Node.SignalName.Ready);
        }

        var sequenceSuffix = DirectionSuffixes.ContainsKey(direction) ? DirectionSuffixes[direction] : "";

        if (_anim.HasAnimation(currentSequenceId + sequenceSuffix))
        {
            _SwapAnimation(currentSequenceId + sequenceSuffix, true);
        }
        else if (_anim.HasAnimation(currentSequenceId))
        {
            _SwapAnimation(currentSequenceId, true);
        }
    }

    /// <summary>
    /// Transition to the next animation sequence, accounting for the RESET track and
    /// current animation elapsed time.
    /// </summary>
    /// <param name="nextSequence">The next animation sequence to play</param>
    /// <param name="keepPosition">Whether to keep the current animation position</param>
    private void _SwapAnimation(string nextSequence, bool keepPosition)
    {
        var nextAnim = _anim.GetAnimation(nextSequence);

        if (nextAnim != null)
        {
            // If keeping the current position, we want to do so as a ratio of the
            // position / animation length to account for animations of different length.
            var currentPositionRatio = 0f;
            if (keepPosition)
            {
                currentPositionRatio = _anim.CurrentAnimationPosition / _anim.CurrentAnimationLength;
            }

            // RESET the animation immediately to its default reset state before the next sequence.
            // Take advantage of the default RESET animation to clear uncommon changes (i.e. flip_h).
            if (_anim.HasAnimation(ResetSequenceKey))
            {
                _anim.Play(ResetSequenceKey);
                _anim.Advance(0);
            }

            _anim.Play(nextSequence);
            _anim.Advance(currentPositionRatio * nextAnim.GetLength());
        }
    }
}
