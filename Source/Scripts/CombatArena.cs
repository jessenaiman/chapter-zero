using Godot;
using System;

/// <summary>
/// An arena is the background for a battle. It is a Control node that contains the battlers and the turn queue.
/// It also contains the music that plays during the battle.
/// </summary>
public partial class CombatArena : Control
{
    /// <summary>
    /// The music that will be automatically played during this combat instance.
    /// </summary>
    [Export]
    public AudioStream Music { get; set; }

    // Keep a reference to the turn queue, which handles combat logic including combat start and end.
    private ActiveTurnQueue turnQueue;
    public ActiveTurnQueue TurnQueue
    {
        get
        {
            if (turnQueue == null)
            {
                turnQueue = GetNode<ActiveTurnQueue>("Battlers");
            }
            return turnQueue;
        }
    }

    // UI elements
    private AnimationPlayer uiAnimation;
    private UITurnBar uiTurnBar;
    private UIEffectLabelBuilder uiEffectLabelBuilder;
    private UICombatMenus uiPlayerMenus;

    public override void _Ready()
    {
        // Setup the different combat UI elements, beginning with the player battler list.
        var combatParticipantData = TurnQueue.Battlers;
        SetupUIElements(combatParticipantData);

        // The UI elements will automatically fade out once one of the battler teams has lost.
        combatParticipantData.BattlersDowned += OnBattlersDowned;
    }

    /// <summary>
    /// Sets up the UI elements with the given combat participant data
    /// </summary>
    private void SetupUIElements(BattlerList combatParticipantData)
    {
        // Get UI elements
        if (HasNode("UI/AnimationPlayer"))
        {
            uiAnimation = GetNode<AnimationPlayer>("UI/AnimationPlayer");
        }
        if (HasNode("UI/TurnBar"))
        {
            uiTurnBar = GetNode<UITurnBar>("UI/TurnBar");
        }
        if (HasNode("UI/EffectLabelBuilder"))
        {
            uiEffectLabelBuilder = GetNode<UIEffectLabelBuilder>("UI/EffectLabelBuilder");
        }
        if (HasNode("UI/PlayerMenus"))
        {
            uiPlayerMenus = GetNode<UICombatMenus>("UI/PlayerMenus");
        }

        // Set up UI elements if they exist
        if (uiEffectLabelBuilder != null)
        {
            uiEffectLabelBuilder.Setup(combatParticipantData);
        }
        if (uiPlayerMenus != null)
        {
            uiPlayerMenus.Setup(combatParticipantData);
        }
        if (uiTurnBar != null)
        {
            uiTurnBar.Setup(combatParticipantData);
        }
    }

    /// <summary>
    /// Callback when battlers are downed
    /// </summary>
    private void OnBattlersDowned()
    {
        if (uiPlayerMenus != null)
        {
            uiPlayerMenus.Visible = false;
        }
        if (uiTurnBar != null)
        {
            uiTurnBar.FadeOut();
        }
    }

    /// <summary>
    /// Begin combat, setting up the UI before running combat logic.
    /// </summary>
    public async void Start()
    {
        // Smoothly fade in the UI elements.
        if (uiAnimation != null)
        {
            uiAnimation.Play("fade_in");
            // Wait for the animation to finish
            await ToSignal(uiAnimation, AnimationPlayer.SignalName.AnimationFinished);
        }

        // Begin the combat logic.
        TurnQueue.IsActive = true;
    }
}
