
// <copyright file="CombatArena.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Combat;
using OmegaSpiral.Source.Scripts.Combat.Battlers;
using OmegaSpiral.Source.Scripts.Combat.UI;
using OmegaSpiral.Source.Scripts.Combat.UI.EffectLabels;
using OmegaSpiral.Source.Scripts.Combat.UI.TurnBar;

namespace OmegaSpiral.Source.Scripts.Combat;
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
    public AudioStream? Music { get; set; }

    /// <summary>
    /// Gets the turn queue that handles combat logic including combat start and end.
    /// </summary>
    public ActiveTurnQueue? TurnQueue => this.turnQueue;

    /// <summary>
    /// Keep a reference to the turn queue, which handles combat logic including combat start and end.
    /// </summary>
    private ActiveTurnQueue? turnQueue;

    /// <summary>
    /// UI elements used in the combat arena.
    /// </summary>
    private AnimationPlayer? uiAnimation;
    private UITurnBar? uiTurnBar;
    private UIEffectLabelBuilder? uiEffectLabelBuilder;
    private UICombatMenus? uiPlayerMenus;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.turnQueue = this.GetNode<ActiveTurnQueue>("Battlers");

        // Initialize UI elements
        this.uiAnimation = this.GetNode<AnimationPlayer>("UI/AnimationPlayer");
        this.uiTurnBar = this.GetNode<UITurnBar>("UI/TurnBar");
        this.uiEffectLabelBuilder = this.GetNode<UIEffectLabelBuilder>("UI/EffectLabelBuilder");
        this.uiPlayerMenus = this.GetNode<UICombatMenus>("UI/PlayerMenus");

        // Validate that all required nodes were found
        if (this.turnQueue == null || this.uiAnimation == null || this.uiTurnBar == null ||
            this.uiEffectLabelBuilder == null || this.uiPlayerMenus == null)
        {
            GD.PrintErr("CombatArena: Failed to find all required UI nodes");
            return;
        }

        // Setup the different combat UI elements, beginning with the player battler list.
        BattlerList? combatParticipantData = this.turnQueue.Battlers;
        if (combatParticipantData != null)
        {
            this.uiEffectLabelBuilder.Setup(combatParticipantData);
            this.uiPlayerMenus.Setup(combatParticipantData);
            this.uiTurnBar.Setup(combatParticipantData);
        }

        // The UI elements will automatically fade out once one of the battler teams has lost.
        if (combatParticipantData != null)
        {
            combatParticipantData.BattlersDowned += () =>
            {
                this.uiPlayerMenus.Visible = false;
                this.uiTurnBar?.FadeOut();
            };
        }
    }

    /// <summary>
    /// Begin combat, setting up the UI before running combat logic.
    /// </summary>
    public async void Start()
    {
        if (this.uiAnimation == null || this.turnQueue == null)
        {
            GD.PrintErr("CombatArena: Cannot start combat - required components are null");
            return;
        }

        // Smoothly fade in the UI elements.
        this.uiAnimation.Play("fade_in");
        await this.ToSignal(this.uiAnimation, AnimationPlayer.SignalName.AnimationFinished);

        // Begin the combat logic.
        this.turnQueue.IsActive = true;
    }
}
