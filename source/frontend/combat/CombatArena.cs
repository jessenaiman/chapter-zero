
// <copyright file="CombatArena.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Combat.Battlers;
using OmegaSpiral.Source.Scripts.Combat.Ui;
using OmegaSpiral.Source.Scripts.Combat.Ui.EffectLabels;
using OmegaSpiral.Source.Scripts.Combat.Ui.TurnBar;

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
    /// Ui elements used in the combat arena.
    /// </summary>
    private AnimationPlayer? uiAnimation;
    private UiTurnBar? uiTurnBar;
    private UiEffectLabelBuilder? uiEffectLabelBuilder;
    private UiCombatMenus? uiPlayerMenus;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.turnQueue = this.GetNode<ActiveTurnQueue>("Battlers");

        // Initialize Ui elements
        this.uiAnimation = this.GetNode<AnimationPlayer>("Ui/AnimationPlayer");
        this.uiTurnBar = this.GetNode<UiTurnBar>("Ui/TurnBar");
        this.uiEffectLabelBuilder = this.GetNode<UiEffectLabelBuilder>("Ui/EffectLabelBuilder");
        this.uiPlayerMenus = this.GetNode<UiCombatMenus>("Ui/PlayerMenus");

        // Validate that all required nodes were found
        if (this.turnQueue == null || this.uiAnimation == null || this.uiTurnBar == null ||
            this.uiEffectLabelBuilder == null || this.uiPlayerMenus == null)
        {
            GD.PrintErr("CombatArena: Failed to find all required Ui nodes");
            return;
        }

        SetupCombatUi();
    }

    /// <summary>
    /// Sets up the combat Ui elements by calling Setup on each component.
    /// </summary>
    private void SetupCombatUi()
    {
        BattlerList? combatParticipantData = this.turnQueue?.Battlers;
        if (combatParticipantData == null)
        {
            GD.PrintErr("CombatArena: Combat participant data is null");
            return;
        }

        SetupEffectLabels(combatParticipantData);
        SetupPlayerMenus(combatParticipantData);
        SetupTurnBar(combatParticipantData);
        SubscribeToBattlersDowned(combatParticipantData);
    }

    /// <summary>
    /// Sets up the effect label builder.
    /// </summary>
    private void SetupEffectLabels(BattlerList combatParticipantData)
    {
        if (this.uiEffectLabelBuilder != null)
        {
            this.uiEffectLabelBuilder.Setup(combatParticipantData);
        }
    }

    /// <summary>
    /// Sets up the player menus.
    /// </summary>
    private void SetupPlayerMenus(BattlerList combatParticipantData)
    {
        if (this.uiPlayerMenus != null)
        {
            this.uiPlayerMenus.Setup(combatParticipantData);
        }
    }

    /// <summary>
    /// Sets up the turn bar.
    /// </summary>
    private void SetupTurnBar(BattlerList combatParticipantData)
    {
        if (this.uiTurnBar != null)
        {
            this.uiTurnBar.Setup(combatParticipantData);
        }
    }

    /// <summary>
    /// Subscribes to the battlers downed event.
    /// </summary>
    private void SubscribeToBattlersDowned(BattlerList combatParticipantData)
    {
        combatParticipantData.BattlersDowned += () =>
        {
            if (this.uiPlayerMenus != null)
            {
                this.uiPlayerMenus.Visible = false;
            }

            this.uiTurnBar?.FadeOut();
        };
    }

    /// <summary>
    /// Begin combat, setting up the Ui before running combat logic.
    /// </summary>
    public async void Start()
    {
        if (this.uiAnimation == null || this.turnQueue == null)
        {
            GD.PrintErr("CombatArena: Cannot start combat - required components are null");
            return;
        }

        // Smoothly fade in the Ui elements.
        this.uiAnimation.Play("fade_in");
        await this.ToSignal(this.uiAnimation, AnimationMixer.SignalName.AnimationFinished);

        // Begin the combat logic.
        this.turnQueue.IsActive = true;
    }
}
