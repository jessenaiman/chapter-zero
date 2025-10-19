
// <copyright file="StrangeTreeInteraction.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Scripts.Field.cutscenes.Popups;
using OmegaSpiral.Source.Scripts.Field.cutscenes.Templates.Conversations;

namespace OmegaSpiral.Source.Overworld.Maps.town;
/// <summary>
/// Opens up a secret path once the 'Strange Tree' has been interacted with twice.
/// Handles the tree's dialogue and the animation for revealing the secret path.
/// </summary>
[GlobalClass]
[Tool]
public partial class StrangeTreeInteraction : ConversationTemplate
{
    private AnimationPlayer anim = null!;
    private InteractionPopup popup = null!;

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        this.anim = this.GetNode<AnimationPlayer>("AnimationPlayer");
        this.popup = this.GetNode<InteractionPopup>("InteractionPopup");
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
        await this.Execute();
        await this.ToSignal(this.GetTree(), SceneTree.SignalName.ProcessFrame);
    }

    /// <summary>
    /// Wait for the disappear animation to finish.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task WaitForAnimationFinish()
    {
        if (this.anim != null)
        {
            await this.ToSignal(this.anim, AnimationPlayer.SignalName.AnimationFinished);
        }
    }

    /// <summary>
    /// Override the Run method to execute the tree interaction.
    /// </summary>
    public override void Run()
    {
        this.ExecuteTreeInteraction();
        base.Run();
    }

    /// <summary>
    /// Handle Dialogic signal events for the secret path reveal.
    /// </summary>
    /// <param name="argument">The signal argument from Dialogic.</param>
    private void OnDialogicSignalEvent(string argument)
    {
        // The argument is intentionally unused - we only care that the signal was emitted
        _ = argument;

        // Once the secret path is cleared, we'll want to deactivate the interaction to
        // prevent the user from running it again.
        this.IsActive = false;

        if (this.popup != null)
        {
            this.popup.Hide();
            this.popup.QueueFree();
        }

        // Clearing the secret path is controlled exclusively by the animation player.
        if (this.anim != null)
        {
            this.anim.Play("disappear");
        }
    }
}
