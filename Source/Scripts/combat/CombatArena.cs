// <copyright file="CombatArena.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using Godot;

/// <summary>
/// An arena is the background for a battle. It is a Control node that contains the battlers and the turn queue.
/// It also contains the music that plays during the battle.
/// </summary>
public partial class CombatArena : Control
{
    /// <summary>
    /// Gets or sets the music that will be automatically played during this combat instance.
    /// </summary>
    [Export]
    public AudioStream Music { get; set; }

    // Keep a reference to the turn queue, which handles combat logic including combat start and end.
    private ActiveTurnQueue turnQueue;

    // UI elements
    private AnimationPlayer uiAnimation;
    private UITurnBar uiTurnBar;
    private UIEffectLabelBuilder uiEffectLabelBuilder;
    private UICombatMenus uiPlayerMenus;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.turnQueue = this.GetNode<ActiveTurnQueue>("Battlers");

        // Initialize UI elements
        this.uiAnimation = this.GetNode<AnimationPlayer>("UI/AnimationPlayer");
        this.uiTurnBar = this.GetNode<UITurnBar>("UI/TurnBar");
        this.uiEffectLabelBuilder = this.GetNode<UIEffectLabelBuilder>("UI/EffectLabelBuilder");
        this.uiPlayerMenus = this.GetNode<UICombatMenus>("UI/PlayerMenus");

        // Setup the different combat UI elements, beginning with the player battler list.
        BattlerList combatParticipantData = this.turnQueue.Battlers;
        this.uiEffectLabelBuilder.Setup(combatParticipantData);
        this.uiPlayerMenus.Setup(combatParticipantData);
        this.uiTurnBar.Setup(combatParticipantData);

        // The UI elements will automatically fade out once one of the battler teams has lost.
        combatParticipantData.BattlersDowned += () =>
        {
            this.uiPlayerMenus.Visible = false;
            this.uiTurnBar.FadeOut();
        };
    }

    /// <summary>
    /// Begin combat, setting up the UI before running combat logic.
    /// </summary>
    public async void Start()
    {
        // Smoothly fade in the UI elements.
        this.uiAnimation.Play("fade_in");
        await this.ToSignal(this.uiAnimation, AnimationPlayer.SignalName.AnimationFinished);

        // Begin the combat logic.
        this.turnQueue.IsActive = true;
    }
}
