
// Copyright (c) Ωmega Spiral. All rights reserved.

using System.Threading.Tasks;
using Godot;

namespace OmegaSpiral.Source.Scripts.Common.ScreenTransitions;
/// <summary>
/// A transition (usually between gameplay scenes) in which the screen is hidden behind an opaque
/// color and then shown again.
///
/// Screen transitions are often used in cutscenes to cover up changes in the scenery or sudden
/// changes to the loaded area. Many games begin with the screen covered and play some kind of
/// animation before transitioning (see <see cref="Reveal"/>) to gameplay.
///
/// <br/><br/>ScreenTransitions cover or reveal the screen uniformly as a fade animation.
/// </summary>
[GlobalClass]
public partial class ScreenTransition : CanvasLayer
{
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

    private Tween? tween;
    private ColorRect? colorRect;

    /// <summary>
    /// Emitted when the screen has finished the current animation, whether that is to <see cref="Cover"/> the
    /// screen or <see cref="Reveal"/> the screen.
    /// </summary>
    [Signal]
    public delegate void FinishedEventHandler();

    /// <inheritdoc/>
    public override void _Ready()
    {
        // The screen transitions need to run over the gameplay, which is instantiated below all
        // autoloads (including this class). Therefore, we want to move the ScreenTransition object to
        // the very bottom of the SceneTree's child list.
        // We cannot do so during ready, in which this node's parents are not yet ready. Therefore the
        // call to move_child must be deferred a frame.
        this.GetParent().CallDeferred("move_child", this, this.GetParent().GetChildCount() - 1);

        this.colorRect = this.GetNode<ColorRect>("ColorRect");

        // Allow the mouse through the transition GUI elements.
        this.colorRect.MouseFilter = Control.MouseFilterEnum.Ignore;

        // By default, do NOT have the ColorRect covering the screen.
        this.Show();
        _ = this.ClearScreen();
    }

    /// <summary>
    /// Reveal the screen instantly, unless the duration argument is non-zero.
    /// This method is a coroutine that will finish once the screen has been revealed.
    /// </summary>
    /// <param name="duration"></param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task Reveal(float duration = 0.0f)
    {
        await this.ClearScreen(duration).ConfigureAwait(false);
    }

    /// <summary>
    /// Hide the ColorRect instantly, unless the duration argument is non-zero.
    /// This method is a coroutine that will finish once the screen has been cleared.
    /// </summary>
    /// <param name="duration"></param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ClearScreen(float duration = 0.0f)
    {
        if (this.tween != null)
        {
            this.tween.Kill();
            this.tween = null;
            this.EmitSignal(SignalName.Finished);
        }

        if (this.colorRect != null && (Mathf.IsEqualApprox(duration, 0.0f) || this.colorRect.Modulate.IsEqualApprox(Clear)))
        {
            this.colorRect.Modulate = Clear;
            this.CallDeferred("emit_signal", "finished");
        }
        else if (this.colorRect != null)
        {
            this.TweenTransition(duration, Clear);
        }

        await this.ToSignal(this, SignalName.Finished);
    }

    /// <summary>
    /// Cover the screen instantly, unless the duration argument is non-zero.
    /// This method is a coroutine that will finish once the screen has been covered.
    /// </summary>
    /// <param name="duration"></param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task Cover(float duration = 0.0f)
    {
        if (this.tween != null)
        {
            if (this.tween.IsRunning())
            {
                this.EmitSignal(SignalName.Finished);
            }

            this.tween.Kill();
            this.tween = null;
        }

        if (this.colorRect != null && (Mathf.IsEqualApprox(duration, 0.0f) || this.colorRect.Modulate.IsEqualApprox(Covered)))
        {
            this.colorRect.Modulate = Covered;
            this.CallDeferred("emit_signal", "finished");
        }
        else if (this.colorRect != null)
        {
            this.TweenTransition(duration, Covered);
        }

        await this.ToSignal(this, SignalName.Finished);
    }

    /// <summary>
    /// Relegate the tween creation to a method so that derived classes can easily change transition type.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="targetColor"></param>
    private void TweenTransition(float duration, Color targetColor)
    {
        if (this.colorRect != null)
        {
            this.tween = this.CreateTween();
            this.tween.TweenProperty(this.colorRect, "modulate", targetColor, duration);
            this.tween.TweenCallback(Callable.From(() => this.CallDeferred("emit_signal", "finished")));
        }
    }
}
