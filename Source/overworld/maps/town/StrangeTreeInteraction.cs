using Godot;

/// <summary>
/// Opens up a secret path once the 'Strange Tree' has been interacted with twice.
/// Handles the tree's dialogue and the animation for revealing the secret path.
/// </summary>
[Tool]
public partial class StrangeTreeInteraction : ConversationTemplate
{
    private AnimationPlayer anim = null!;
    private InteractionPopup popup = null!;

    public override void _Ready()
    {
        base._Ready();

        anim = GetNode<AnimationPlayer>("AnimationPlayer");
        popup = GetNode<InteractionPopup>("InteractionPopup");
    }

    /// <summary>
    /// Execute the strange tree interaction.
    /// </summary>
    public async void ExecuteTreeInteraction()
    {
        // We want to open the secret path once the player has performed a specific action.
        // The easiest method is to allow Dialogic to determine when this should happen. This could be
        // done via the EmitSignal event, the Call event, or, as we've opted to do here, an in-dialogue
        // signal.
        // Note that this connection only occurs when this particular dialogue occurs. Since this
        // interaction only really happens once, we don't care what signal argument is passed, only that
        // the signal itself is emitted.
        Execute();
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
    }

    /// <summary>
    /// Handle Dialogic signal events for the secret path reveal.
    /// </summary>
    /// <param name="argument">The signal argument from Dialogic</param>
    private void OnDialogicSignalEvent(string argument)
    {
        // Once the secret path is cleared, we'll want to deactivate the interaction to
        // prevent the user from running it again.
        IsActive = false;

        if (popup != null)
        {
            popup.Hide();
            popup.QueueFree();
        }

        // Clearing the secret path is controlled exclusively by the animation player.
        if (anim != null)
        {
            anim.Play("disappear");
        }
    }

    /// <summary>
    /// Wait for the disappear animation to finish.
    /// </summary>
    public async Task WaitForAnimationFinish()
    {
        if (anim != null)
        {
            await ToSignal(anim, AnimationPlayer.SignalName.AnimationFinished);
        }
    }

    /// <summary>
    /// Override the Run method to execute the tree interaction.
    /// </summary>
    public override void Run()
    {
        ExecuteTreeInteraction();
        base.Run();
    }
}
