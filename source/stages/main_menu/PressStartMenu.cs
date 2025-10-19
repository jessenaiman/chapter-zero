using Godot;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Stages.main_menu;

/// <summary>
/// Custom press-start menu for the Chapter Zero demo.
/// Presents inviting and ominous start options while delegating settings to the Maaack template overlays.
/// </summary>
[GlobalClass]
public partial class PressStartMenu : Control
{
    private const string DefaultOptionsMenuPath = "res://addons/maaacks_game_template/examples/scenes/overlaid_menus/main_menu_options_overlaid_menu.tscn";
    private const string InvitingStartSfxPath = "res://Source/Assets/sfx/confirmation_002.ogg";
    private const string OminousStartSfxPath = "res://Source/Assets/sfx/doorClose_4.ogg";

    [Export]
    public string OptionsMenuScenePath { get; set; } = DefaultOptionsMenuPath;

    private Button? _invitingButton;
    private Button? _ominousButton;
    private Button? _optionsButton;
    private Label? _titleLabel;
    private Label? _subtitleLabel;
    private AudioStreamPlayer? _sfxPlayer;

    private bool _isStarting;

    /// <inheritdoc/>
    public override void _Ready()
    {
        CacheNodes();
        ConfigureLabels();
        ConnectSignals();
        GetTree().Paused = false;
    }

    private void CacheNodes()
    {
        _invitingButton = GetNodeOrNull<Button>("%InvitingStartButton");
        _ominousButton = GetNodeOrNull<Button>("%OminousStartButton");
        _optionsButton = GetNodeOrNull<Button>("%OptionsButton");
        _titleLabel = GetNodeOrNull<Label>("%TitleLabel");
        _subtitleLabel = GetNodeOrNull<Label>("%SubtitleLabel");
        _sfxPlayer = GetNodeOrNull<AudioStreamPlayer>("%MenuSfxPlayer");
    }

    private void ConfigureLabels()
    {
        string gameTitle = ProjectSettings.GetSetting("application/config/name", "Ωmega Spiral").AsStringName();
        if (_titleLabel != null)
        {
            _titleLabel.Text = gameTitle;
        }

        if (_subtitleLabel != null)
        {
            _subtitleLabel.Text = "Act I · One Shot Demo";
        }
    }

    private void ConnectSignals()
    {
        if (_invitingButton != null)
        {
            _invitingButton.Pressed += () => _ = HandlePressStartAsync(PressStartMood.Inviting);
        }

        if (_ominousButton != null)
        {
            _ominousButton.Pressed += () => _ = HandlePressStartAsync(PressStartMood.Ominous);
        }

        if (_optionsButton != null)
        {
            _optionsButton.Pressed += OpenOptionsMenu;
        }
    }

    private async Task HandlePressStartAsync(PressStartMood mood)
    {
        if (_isStarting)
        {
            return;
        }

        _isStarting = true;
        DisableButtons();

        PlayStartSfx(mood);
        await AnimatePressStartAsync(mood);

        var gameState = GetNode<GameState>("/root/GameState");
        gameState.ResetForNewRun();
        gameState.PressStartMood = mood;

        var sceneManager = GetNodeOrNull<SceneManager>("/root/SceneManager");
        if (sceneManager != null)
        {
            sceneManager.TransitionToScene("Stage1Boot", showLoadingScreen: false);
        }
        else
        {
            GD.PushWarning("[PressStartMenu] SceneManager not found; falling back to direct scene change.");
            GetTree().ChangeSceneToFile("res://Source/Stages/Stage1/boot_sequence.tscn");
        }
    }

    private void DisableButtons()
    {
        if (_invitingButton != null)
        {
            _invitingButton.Disabled = true;
        }

        if (_ominousButton != null)
        {
            _ominousButton.Disabled = true;
        }

        if (_optionsButton != null)
        {
            _optionsButton.Disabled = true;
        }
    }

    private void PlayStartSfx(PressStartMood mood)
    {
        if (_sfxPlayer is null)
        {
            return;
        }

        string path = mood == PressStartMood.Ominous ? OminousStartSfxPath : InvitingStartSfxPath;
        if (ResourceLoader.Load<AudioStream>(path) is { } stream)
        {
            _sfxPlayer.Stream = stream;
            _sfxPlayer.Play();
        }
    }

    private async Task AnimatePressStartAsync(PressStartMood mood)
    {
        var tween = GetTree().CreateTween();
        tween.SetEase(Tween.EaseType.Out);
        tween.SetTrans(Tween.TransitionType.Cubic);

        float duration = mood == PressStartMood.Ominous ? 0.9f : 0.6f;

        tween.TweenProperty(this, "modulate", new Color(1f, 1f, 1f, 0f), duration);
        await ToSignal(tween, Tween.SignalName.Finished);
    }

    private void OpenOptionsMenu()
    {
        if (string.IsNullOrEmpty(OptionsMenuScenePath))
        {
            GD.PrintErr("[PressStartMenu] OptionsMenuScenePath is not configured.");
            return;
        }

        if (ResourceLoader.Load<PackedScene>(OptionsMenuScenePath) is not { } optionsScene)
        {
            GD.PrintErr($"[PressStartMenu] Unable to load options menu scene at '{OptionsMenuScenePath}'.");
            return;
        }

        if (optionsScene.Instantiate() is Control overlay)
        {
            AddChild(overlay);
            overlay.GrabFocus();
        }
    }
}
