
// <copyright file="FanInteraction.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Field.gamepieces;
using OmegaSpiral.Source.Scripts.Field.gamepieces.Controllers;
using OmegaSpiral.Source.Scripts.Field.cutscenes.Popups;
using OmegaSpiral.Source.Scripts.Field.cutscenes.Templates.Conversations;
using System.Collections.ObjectModel;
using OmegaSpiral.Source.Scripts.Field.gameboard;

namespace OmegaSpiral.Source.Overworld.Maps.town;
/// <summary>
/// Interaction with the adoring fan NPC.
/// Handles the fan's dialogue and quest progression logic.
/// </summary>
[Tool]
public partial class FanInteraction : ConversationTemplate
{
    private Gamepiece? adoringFan;
    private InteractionPopup? popup;

    /// <summary>
    /// Gets or sets the gamepiece controller for the fan's movement.
    /// </summary>
    [Export]
    public GamepieceController? Controller { get; set; }

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
    public async Task ExecuteFanInteraction()
    {
        // Await Execute to ensure proper async flow
    await this.Execute().ConfigureAwait(false);

        // The quest's state is tracked by a Dialogic variable.
        // After speaking with the character for the first time, he should run to a new position so that
        // the player can speak with the other NPCs.
        var dialogic = this.GetNode("/root/Dialogic");
        if (dialogic != null)
        {
            var varNode = dialogic.GetNode("VAR");
            var tokenQuestStatus = varNode?.Call("get_variable", "TokenQuestStatus");
            if (tokenQuestStatus != null && (int) tokenQuestStatus == 1)
            {
                await this.OnInitialConversationFinished().ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Override the Run method to execute the fan interaction.
    /// </summary>
    public override async void Run()
    {
        await this.ExecuteFanInteraction().ConfigureAwait(false);
        base.Run();
    }

    /// <summary>
    /// Handle the initial conversation finishing by moving the fan.
    /// </summary>
    private async Task OnInitialConversationFinished()
    {
        var gameboard = this.GetNode<Gameboard>("/root/Gameboard");
        if (gameboard == null || this.Controller == null)
        {
            return;
        }

        var sourceCell = this.GetSourceCell(gameboard);
        await this.MoveFanAlongPath(gameboard, sourceCell).ConfigureAwait(false);
    }

    private Vector2I GetSourceCell(Gameboard gameboard)
    {
        return gameboard.PixelToCell(this.adoringFan?.Position ?? Vector2.Zero);
    }

    private async Task MoveFanAlongPath(Gameboard gameboard, Vector2I sourceCell)
    {
        if (this.Controller == null)
        {
            return;
        }

        this.Controller.IsActive = true;

        var path = FanInteraction.GetPathToDestination(gameboard, sourceCell);
        this.SetControllerPath(path);

        await this.WaitForFanArrival().ConfigureAwait(false);
        this.Controller.IsActive = false;
    }

    private static List<Vector2I>? GetPathToDestination(Gameboard gameboard, Vector2I sourceCell)
    {
    var pathfinder = gameboard.GetNode<Pathfinder>("Pathfinder");
    var path = pathfinder?.GetPathToCell(sourceCell, new Vector2I(23, 13));
    return path == null ? null : path.ToList();
    }

    private void SetControllerPath(List<Vector2I>? path)
    {
        if (this.Controller == null)
        {
            return;
        }

        var pathList = path ?? new List<Vector2I>();
        this.Controller.MoveAlongPath(new ReadOnlyCollection<Vector2I>(pathList));
    }

    private async Task WaitForFanArrival()
    {
        if (this.adoringFan != null)
        {
            await this.ToSignal(this.adoringFan, "arrived");
        }
    }

    /// <summary>
    /// Handle Dialogic signal events for quest rewards.
    /// This conversation only emits a signal once: when the player should receive the quest reward.
    /// </summary>
    /// <param name="argument">The signal argument from Dialogic.</param>
    private void OnDialogicSignalEvent(string argument)
    {
        // Parameter is intentionally unused - Dialogic signal structure requires it
        _ = argument;

        // The popup should be deactivated after the conversation
        if (this.popup != null)
        {
            this.popup.IsActive = false;
        }

        // Handle quest reward: remove 4 coins and add blue wand
        var inventory = Inventory.Restore();
        if (inventory != null)
        {
            inventory.Remove(Inventory.ItemType.Coin, 4);
            inventory.Add(Inventory.ItemType.BlueWand);
        }
    }
}
