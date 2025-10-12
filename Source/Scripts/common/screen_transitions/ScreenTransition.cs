using Godot;
using System;
using System.Threading.Tasks;

/// <summary>
/// A transition (usually between gameplay scenes) in which the screen is hidden behind an opaque
/// color and then shown again.
///
/// Screen transitions are often used in <see cref="Cutscene"/>s to cover up changes in the scenery or sudden
/// changes to the loaded area. Many games begin with the screen covered and play some kind of
/// animation before transitioning (see <see cref="Reveal"/>) to gameplay.
///
/// <br/><br/>ScreenTransitions cover or reveal the screen uniformly as a fade animation.
/// </summary>
public partial class ScreenTransition : CanvasLayer
{
    /// <summary>
    /// Emitted when the screen has finished the current animation, whether that is to <see cref="Cover"/> the
    /// screen or <see cref="Reveal"/> the screen.
    /// </summary>
    [Signal]
    public delegate void FinishedEventHandler();

    /// <summary>
    /// The modulate color of the scene when it is to be invisible. Note that it is just
    /// <see cref="Colors.White"/> with a zero alpha channel.
    /// </summary>
    private static readonly Color Clear = new Color(1, 1, 1, 0);

    /// <summary>
    /// The target modulate value of the scene when the transition covers the screen. Note that it is
    /// just <see cref="Colors.White"/>.
    /// Consequently, the color of the screen transition may be set through the <see cref="Color"/> property.
    /// </summary>
    private static readonly Color Covered = Colors.White;

    private Tween _tween;
    private ColorRect _colorRect;

    public override void _Ready()
    {
        // The screen transitions need to run over the gameplay, which is instantiated below all
        // autoloads (including this class). Therefore, we want to move the ScreenTransition object to
        // the very bottom of the SceneTree's child list.
        // We cannot do so during ready, in which this node's parents are not yet ready. Therefore the
        // call to move_child must be deferred a frame.
        GetParent().CallDeferred("move_child", this, GetParent().GetChildCount() - 1);

        _colorRect = GetNode<ColorRect>("ColorRect");

        // Allow the mouse through the transition GUI elements.
        _colorRect.MouseFilter = Control.MouseFilterEnum.Ignore;

        // By default, do NOT have the ColorRect covering the screen.
        Show();
        ClearScreen();
    }

    /// <summary>
    /// Hide the ColorRect instantly, unless the duration argument is non-zero.
    /// This method is a coroutine that will finish once the screen has been cleared.
    /// </summary>
    public async Task ClearScreen(float duration = 0.0f)
    {
        if (_tween != null)
        {
            _tween.Kill();
            _tween = null;
            EmitSignal(SignalName.Finished);
        }

        if (Mathf.IsEqualApprox(duration, 0.0f) || _colorRect.Modulate.IsEqualApprox(Clear))
        {
            _colorRect.Modulate = Clear;
            CallDeferred("emit_signal", "finished");
        }
        else
        {
            _TweenTransition(duration, Clear);
        }

        await ToSignal(this, SignalName.Finished);
    }

    /// <summary>
    /// Cover the screen instantly, unless the duration argument is non-zero.
    /// This method is a coroutine that will finish once the screen has been covered.
    /// </summary>
    public async Task Cover(float duration = 0.0f)
    {
        if (_tween != null)
        {
            if (_tween.IsRunning())
            {
                EmitSignal(SignalName.Finished);
            }
            _tween.Kill();
            _tween = null;
        }

        if (Mathf.IsEqualApprox(duration, 0.0f) || _colorRect.Modulate.IsEqualApprox(Covered))
        {
            _colorRect.Modulate = Covered;
            CallDeferred("emit_signal", "finished");
        }
        else
        {
            _TweenTransition(duration, Covered);
        }

        await ToSignal(this, SignalName.Finished);
    }

    /// <summary>
    /// Relegate the tween creation to a method so that derived classes can easily change transition type.
    /// </summary>
    private void _TweenTransition(float duration, Color targetColor)
    {
        _tween = CreateTween();
        _tween.TweenProperty(_colorRect, "modulate", targetColor, duration);
        _tween.TweenCallback(Callable.From(() => CallDeferred("emit_signal", "finished")));
    }

    /// <summary>
    /// Reveal the screen instantly, unless the duration argument is non-zero.
    /// This method is a coroutine that will finish once the screen has been revealed.
    /// </summary>
    public async Task Reveal(float duration = 0.0f)
    {
        await ClearScreen(duration);
    }
}
