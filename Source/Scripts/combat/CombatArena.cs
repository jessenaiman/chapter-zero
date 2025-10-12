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
    private ActiveTurnQueue _turnQueue;

    // UI elements
    private AnimationPlayer _uiAnimation;
    private UITurnBar _uiTurnBar;
    private UIEffectLabelBuilder _uiEffectLabelBuilder;
    private UICombatMenus _uiPlayerMenus;

    public override void _Ready()
    {
        _turnQueue = GetNode<ActiveTurnQueue>("Battlers");

        // Initialize UI elements
        _uiAnimation = GetNode<AnimationPlayer>("UI/AnimationPlayer");
        _uiTurnBar = GetNode<UITurnBar>("UI/TurnBar");
        _uiEffectLabelBuilder = GetNode<UIEffectLabelBuilder>("UI/EffectLabelBuilder");
        _uiPlayerMenus = GetNode<UICombatMenus>("UI/PlayerMenus");

        // Setup the different combat UI elements, beginning with the player battler list.
        BattlerList combatParticipantData = _turnQueue.Battlers;
        _uiEffectLabelBuilder.Setup(combatParticipantData);
        _uiPlayerMenus.Setup(combatParticipantData);
        _uiTurnBar.Setup(combatParticipantData);

        // The UI elements will automatically fade out once one of the battler teams has lost.
        combatParticipantData.BattlersDowned += () =>
        {
            _uiPlayerMenus.Visible = false;
            _uiTurnBar.FadeOut();
        };
    }

    /// <summary>
    /// Begin combat, setting up the UI before running combat logic.
    /// </summary>
    public async void Start()
    {
        // Smoothly fade in the UI elements.
        _uiAnimation.Play("fade_in");
        await ToSignal(_uiAnimation, AnimationPlayer.SignalName.AnimationFinished);

        // Begin the combat logic.
        _turnQueue.IsActive = true;
    }
}
