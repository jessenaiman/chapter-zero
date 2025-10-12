using Godot;

/// <summary>
/// Interaction with the adoring fan NPC.
/// Handles the fan's dialogue and quest progression logic.
/// </summary>
[Tool]
public partial class FanInteraction : ConversationTemplate
{
    /// <summary>
    /// The gamepiece controller for the fan's movement.
    /// </summary>
    [Export]
    public GamepieceController Controller { get; set; }

    private Gamepiece adoringFan;
    private InteractionPopup popup;

    public override void _Ready()
    {
        base._Ready();

        if (!Engine.IsEditorHint())
        {
            adoringFan = GetParent<Gamepiece>();
            if (adoringFan == null)
            {
                GD.PrintErr("Gamepiece was not found, check the node path!");
            }

            if (Controller == null)
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
        await Execute();

        // The quest's state is tracked by a Dialogic variable.
        // After speaking with the character for the first time, he should run to a new position so that
        // the player can speak with the other NPCs.
        var dialogic = GetNode("/root/Dialogic");
        if (dialogic != null)
        {
            var tokenQuestStatus = dialogic.Get("VAR").Call("get_variable", "TokenQuestStatus");
            if (tokenQuestStatus.AsInt32() == 1)
            {
                await OnInitialConversationFinished();
            }
        }
    }

    /// <summary>
    /// Handle the initial conversation finishing by moving the fan.
    /// </summary>
    private async Task OnInitialConversationFinished()
    {
        var sourceCell = Gameboard.PixelToCell(adoringFan.Position);

        // Everything is paused at the moment, so activate the fan's controller so that he can move on a
        // path during the cutscene.
        if (Controller != null)
        {
            Controller.IsActive = true;
            var path = Gameboard.Pathfinder.GetPathToCell(sourceCell, new Vector2I(23, 13));
            Controller.MovePath = new System.Collections.Generic.List<Vector2I>(path);

            // Wait for the fan to arrive at destination
            await ToSignal(adoringFan, "arrived");

            Controller.IsActive = false;
        }
    }

    /// <summary>
    /// Handle Dialogic signal events for quest rewards.
    /// This conversation only emits a signal once: when the player should receive the quest reward.
    /// </summary>
    /// <param name="argument">The signal argument from Dialogic</param>
    private void OnDialogicSignalEvent(string argument)
    {
        // The popup should be deactivated after the conversation
        if (popup != null)
        {
            popup.IsActive = false;
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
        await ExecuteFanInteraction();
        base.Run();
    }
}
