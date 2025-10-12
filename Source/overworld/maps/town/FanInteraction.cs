// <copyright file="FanInteraction.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Godot;

/// <summary>
/// Interaction with the adoring fan NPC.
/// Handles the fan's dialogue and quest progression logic.
/// </summary>
[Tool]
public partial class FanInteraction : ConversationTemplate
{
    /// <summary>
    /// Gets or sets the gamepiece controller for the fan's movement.
    /// </summary>
    [Export]
    public GamepieceController Controller { get; set; }

    private Gamepiece adoringFan;
    private InteractionPopup popup;

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        if (!Engine.IsEditorHint())
        {
            this.adoringFan = this.GetParent<Gamepiece>();
            if (this.adoringFan == null)
            {
                GD.PrintErr("Gamepiece was not found, check the node path!");
            }

            if (this.Controller == null)
            {
                GD.PrintErr("Controller was not found, check the node path!");
            }
        }
    }

    /// <summary>
    /// Execute the fan interaction with quest logic.
    /// </summary>
    public async void ExecuteFanInteraction()
    {
        await this.Execute();

        // The quest's state is tracked by a Dialogic variable.
        // After speaking with the character for the first time, he should run to a new position so that
        // the player can speak with the other NPCs.
        var dialogic = this.GetNode("/root/Dialogic");
        if (dialogic != null)
        {
            var tokenQuestStatus = dialogic.Get("VAR").Call("get_variable", "TokenQuestStatus");
            if (tokenQuestStatus.AsInt32() == 1)
            {
                await OnInitialConversationFinished().ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Handle the initial conversation finishing by moving the fan.
    /// </summary>
    private async Task OnInitialConversationFinished()
    {
        var sourceCell = Gameboard.PixelToCell(this.adoringFan.Position);

        // Everything is paused at the moment, so activate the fan's controller so that he can move on a
        // path during the cutscene.
        if (this.Controller != null)
        {
            this.Controller.IsActive = true;
            var path = Gameboard.Pathfinder.GetPathToCell(sourceCell, new Vector2I(23, 13));
            this.Controller.MovePath = new System.Collections.Generic.List<Vector2I>(path);

            // Wait for the fan to arrive at destination
            await this.ToSignal(this.adoringFan, "arrived");

            this.Controller.IsActive = false;
        }
    }

    /// <summary>
    /// Handle Dialogic signal events for quest rewards.
    /// This conversation only emits a signal once: when the player should receive the quest reward.
    /// </summary>
    /// <param name="argument">The signal argument from Dialogic.</param>
    private void OnDialogicSignalEvent(string argument)
    {
        // The popup should be deactivated after the conversation
        if (this.popup != null)
        {
            this.popup.IsActive = false;
        }

        // Handle quest reward: remove 4 coins and add blue wand
        var inventory = Inventory.Restore();
        if (inventory != null)
        {
            inventory.Remove(Inventory.ItemTypes.Coin, 4);
            inventory.Add(Inventory.ItemTypes.BlueWand);
        }
    }

    /// <summary>
    /// Override the Run method to execute the fan interaction.
    /// </summary>
    public override async void Run()
    {
        await this.ExecuteFanInteraction();
        base.Run();
    }
}
