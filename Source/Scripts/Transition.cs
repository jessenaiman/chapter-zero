using Godot;
using System;
using System.Threading.Tasks;

/// <summary>
/// Screen transition effects manager.
/// The Transition class provides a centralized way to manage screen transition effects
/// such as fading in/out, sliding, or other visual transitions between game states.
/// It uses a ColorRect with a ShaderMaterial to create smooth transition effects.
/// </summary>
public partial class Transition : ColorRect
{
    /// <summary>
    /// Singleton instance of Transition
    /// </summary>
    public static Transition Instance { get; private set; }

    /// <summary>
    /// Emitted when a transition is finished.
    /// </summary>
    [Signal]
    public delegate void FinishedEventHandler();

    /// <summary>
    /// The duration of transitions in seconds.
    /// </summary>
    [Export]
    public float TransitionDuration { get; set; } = 0.5f;

    /// <summary>
    /// The tween used for animations.
    /// </summary>
    private Tween tween;

    /// <summary>
    /// Whether a transition is currently in progress.
    /// </summary>
    public bool IsTransitioning { get; private set; } = false;

    public override void _Ready()
    {
        Instance = this;

        // Hide the transition by default
        Visible = false;

        // Make sure the rectangle covers the entire screen
        AnchorRight = 1.0f;
        AnchorBottom = 1.0f;

        // Set the initial color to black
        Color = Colors.Black;
    }

    /// <summary>
    /// Cover the screen with a transition effect.
    /// This method fades the screen to black (or the specified color).
    /// </summary>
    public async Task Cover(float duration = -1.0f)
    {
        if (duration < 0)
        {
            duration = TransitionDuration;
        }

        // If already covered, do nothing
        if (Visible && Color.A >= 1.0f)
        {
            return;
        }

        IsTransitioning = true;
        Visible = true;

        // Stop any existing tweens
        if (tween != null && tween.IsRunning())
        {
            tween.Kill();
        }

        // Create a new tween
        tween = CreateTween();
        tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);

        // Animate the alpha from 0 to 1
        tween.TweenProperty(this, "color:a", 1.0f, duration);

        // Wait for the tween to finish
        await ToSignal(tween, Tween.SignalName.Finished);

        IsTransitioning = false;
        EmitSignal(SignalName.Finished);
    }

    /// <summary>
    /// Clear the screen transition effect.
    /// This method fades the screen from black (or the specified color) back to transparent.
    /// </summary>
    public async Task Clear(float duration = -1.0f)
    {
        if (duration < 0)
        {
            duration = TransitionDuration;
        }

        // If already clear, do nothing
        if (Visible && Color.A <= 0.0f)
        {
            return;
        }

        IsTransitioning = true;
        Visible = true;

        // Stop any existing tweens
        if (tween != null && tween.IsRunning())
        {
            tween.Kill();
        }

        // Create a new tween
        tween = CreateTween();
        tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);

        // Animate the alpha from 1 to 0
        tween.TweenProperty(this, "color:a", 0.0f, duration);

        // Wait for the tween to finish
        await ToSignal(tween, Tween.SignalName.Finished);

        // Hide the transition when finished
        Visible = false;
        IsTransitioning = false;
        EmitSignal(SignalName.Finished);
    }

    /// <summary>
    /// Flash the screen with a transition effect.
    /// This method briefly covers the screen and then clears it.
    /// </summary>
    public async Task Flash(float duration = -1.0f)
    {
        if (duration < 0)
        {
            duration = TransitionDuration;
        }

        // Cover the screen
        await Cover(duration / 2.0f);

        // Clear the screen
        await Clear(duration / 2.0f);
    }

    /// <summary>
    /// Slide the screen transition effect.
    /// This method slides a colored bar across the screen.
    /// </summary>
    public async Task Slide(Direction direction = Direction.Left, float duration = -1.0f)
    {
        if (duration < 0)
        {
            duration = TransitionDuration;
        }

        IsTransitioning = true;
        Visible = true;

        // Reset the rectangle to cover the entire screen
        AnchorLeft = 0.0f;
        AnchorTop = 0.0f;
        AnchorRight = 1.0f;
        AnchorBottom = 1.0f;

        // Set initial position based on direction
        switch (direction)
        {
            case Direction.Left:
                Position = new Vector2(-Size.X, 0);
                break;
            case Direction.Right:
                Position = new Vector2(Size.X, 0);
                break;
            case Direction.Up:
                Position = new Vector2(0, -Size.Y);
                break;
            case Direction.Down:
                Position = new Vector2(0, Size.Y);
                break;
        }

        // Stop any existing tweens
        if (tween != null && tween.IsRunning())
        {
            tween.Kill();
        }

        // Create a new tween
        tween = CreateTween();
        tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);

        // Animate to the final position
        switch (direction)
        {
            case Direction.Left:
                tween.TweenProperty(this, "position:x", 0.0f, duration);
                break;
            case Direction.Right:
                tween.TweenProperty(this, "position:x", 0.0f, duration);
                break;
            case Direction.Up:
                tween.TweenProperty(this, "position:y", 0.0f, duration);
                break;
            case Direction.Down:
                tween.TweenProperty(this, "position:y", 0.0f, duration);
                break;
        }

        // Wait for the tween to finish
        await ToSignal(tween, Tween.SignalName.Finished);

        IsTransitioning = false;
        EmitSignal(SignalName.Finished);
    }

    /// <summary>
    /// Directions for slide transitions.
    /// </summary>
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }
}
