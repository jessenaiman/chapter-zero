using Godot;
using System.Threading.Tasks;

/// <summary>
/// A trigger that plays the game ending sequence when the player reaches the end of the forest.
/// Moves the player to a specific position, plays a dialogue, and triggers a final animation.
/// </summary>
[Tool]
[GlobalClass]
public partial class GameEndTrigger : Trigger
{
    /// <summary>
    /// The Dialogic timeline to play during the ending sequence.
    /// </summary>
    [Export]
    public Resource Timeline { get; set; } = null!; // DialogicTimeline

    /// <summary>
    /// The animation player for the ghost lunge animation.
    /// </summary>
    [Export]
    public AnimationPlayer GhostAnimationPlayer { get; set; } = null!;

    private Gamepiece? _gamepiece;
    private Godot.Timer? _timer;

    public override void _Ready()
    {
        base._Ready();

        if (!Engine.IsEditorHint())
        {
            _timer = GetNode<Godot.Timer>("Timer");
        }
    }

    /// <summary>
    /// Execute the game ending sequence.
    /// </summary>
    protected async void Execute()
    {
        if (_gamepiece == null)
        {
            return;
        }

        // Get the Gameboard singleton
        var gameboard = GetNode("/root/Gameboard");
        if (gameboard != null)
        {
            var destinationPixel = (Vector2)gameboard.Call("cell_to_pixel", new Vector2I(53, 30));
            _gamepiece.Call("move_to", destinationPixel);
            await ToSignal(_gamepiece, "arrived");
        }

        // Wait for a moment
        if (_timer != null)
        {
            _timer.Start();
            await ToSignal(_timer, Godot.Timer.SignalName.Timeout);
        }

        // Start the Dialogic timeline
        var dialogic = GetNode("/root/Dialogic");
        if (dialogic != null && Timeline != null)
        {
            dialogic.Call("start_timeline", Timeline);
            await ToSignal(dialogic, "timeline_ended");
        }

        // Wait for another moment
        if (_timer != null)
        {
            _timer.Start();
            await ToSignal(_timer, Godot.Timer.SignalName.Timeout);
        }

        // Note that the lunge animation also includes a screen transition and some text
        if (GhostAnimationPlayer != null)
        {
            GhostAnimationPlayer.Play("lunge");
            await ToSignal(GhostAnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
        }

        // Lock input by waiting forever (infinite wait)
        await ToSignal(GetTree(), SceneTree.SignalName.TreeChanged);
    }

    /// <summary>
    /// Called when an area enters the trigger.
    /// </summary>
    protected void OnAreaEntered(Area2D area)
    {
        if (!Engine.IsEditorHint())
        {
            _gamepiece = area.Owner as Gamepiece;
        }

        // Call the base class method to handle the trigger logic
        base._OnAreaEntered(area);
    }
}
