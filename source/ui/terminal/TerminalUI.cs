using Godot;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Ui.Omega;

namespace OmegaSpiral.Source.Ui.Terminal;

/// <summary>
/// Terminal Ui that inherits from OmegaUi and adds terminal-specific behavior.
/// Provides basic choice presentation, terminal modes, and captions.
/// Used across multiple stages that require terminal-style interfaces.
/// This is a reusable base class - stage-specific logic should be in derived classes.
/// </summary>
[GlobalClass]
public partial class TerminalUi : OmegaUi
{
    /// <summary>
    /// Terminal operation modes.
    /// </summary>
    public enum TerminalMode
    {
        /// <summary>No terminal functionality (for minimal Ui stages).</summary>
        Disabled,
        /// <summary>Basic text display only (for dungeon overlays).</summary>
        Minimal,
        /// <summary>Full terminal with shaders and effects.</summary>
        Full
    }

    /// <summary>
    /// The terminal operation mode.
    /// </summary>
    [Export] public TerminalMode Mode { get; set; } = TerminalMode.Full;

    /// <summary>
    /// Whether captions are enabled.
    /// </summary>
    [Export] public bool CaptionsEnabled { get; set; }

    // Terminal-specific components
    private ITerminalChoicePresenter? _ChoicePresenter;

    /// <summary>
    /// Gets the choice presenter instance.
    /// </summary>
    protected ITerminalChoicePresenter? ChoicePresenter => _ChoicePresenter;

    // Terminal-specific nodes
#pragma warning disable CA1816 // Godot nodes don't need explicit disposal
    private VBoxContainer? _ChoiceContainer;
    private Label? _CaptionLabel;
#pragma warning restore CA1816

    // Cached autoload references for performance
    private Node? _SceneManager;
    private GameState? _GameState;

    /// <inheritdoc/>
    public override void _Ready()
    {
        if (Mode == TerminalMode.Disabled)
        {
            GD.Print("[TerminalUi] Terminal mode is Disabled - skipping initialization.");
            return;
        }

        // Call base class initialization (OmegaUi)
        base._Ready();

        // Cache autoload references for performance
        _SceneManager = GetNodeOrNull<Node>("/root/SceneManager");
        _GameState = GetNodeOrNull<GameState>("/root/GameState");

        if (_SceneManager == null) GD.PushWarning("[TerminalUi] SceneManager autoload not found.");
        if (_GameState == null) GD.PushWarning("[TerminalUi] GameState autoload not found.");

        try
        {
            InitializeTerminalComponents();
        }
        catch (InvalidOperationException ex)
        {
            GD.PrintErr($"[TerminalUi] Terminal-specific initialization failed: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[TerminalUi] Unexpected error during terminal initialization: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Caches terminal-specific node references in addition to base OmegaUi nodes.
    /// </summary>
    protected override void CacheRequiredNodes()
    {
        // Call base to cache OmegaUi nodes (PhosphorLayer, TextDisplay)
        base.CacheRequiredNodes();

        // Cache terminal-specific nodes
        _ChoiceContainer = GetNodeOrNull<VBoxContainer>("ChoiceContainer");
        if (_ChoiceContainer == null)
            throw new InvalidOperationException("Required node 'ChoiceContainer' not found in TerminalUi scene.");

        // Optional nodes - log warning if missing
        _CaptionLabel = GetNodeOrNull<Label>("CaptionLabel");
        if (_CaptionLabel == null)
            GD.PushWarning("[TerminalUi] CaptionLabel not found - captions disabled.");
    }

    /// <summary>
    /// Initializes terminal-specific components after base OmegaUi initialization.
    /// </summary>
    private void InitializeTerminalComponents()
    {
        try
        {
            _ChoicePresenter = CreateChoicePresenter(_ChoiceContainer!);
            if (_ChoicePresenter == null)
                throw new InvalidOperationException("ChoicePresenter creation returned null.");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[TerminalUi] Failed to create ChoicePresenter: {ex.Message}");
            throw new InvalidOperationException($"Failed to create ChoicePresenter. See inner exception.", ex);
        }
    }

    /// <summary>
    /// Initializes terminal-specific Ui states.
    /// </summary>
    protected override void InitializeComponentStates()
    {
        // Call base to initialize OmegaUi states
        base.InitializeComponentStates();

        InitializeTerminalStates();
    }

    /// <summary>
    /// Initializes terminal-specific component states.
    /// </summary>
    private void InitializeTerminalStates()
    {
        if (_ChoiceContainer != null)
        {
            _ChoiceContainer.Visible = false;
        }

        if (_CaptionLabel != null)
        {
            _CaptionLabel.Visible = CaptionsEnabled;
        }
    }

    /// <summary>
    /// Factory method for creating choice presenter (Open/Closed Principle).
    /// Override in derived classes for custom implementations.
    /// </summary>
    /// <param name="choiceContainer">The choice container node.</param>
    /// <returns>An instance of ITerminalChoicePresenter.</returns>
    protected virtual ITerminalChoicePresenter CreateChoicePresenter(VBoxContainer choiceContainer)
    {
        return new TerminalChoicePresenter(choiceContainer);
    }

    /// <summary>
    /// Presents choices to the player and waits for selection.
    /// Terminal-specific behavior - not part of base OmegaUi.
    /// </summary>
    /// <param name="prompt">The prompt text to display.</param>
    /// <param name="optionTexts">The choice option texts.</param>
    /// <returns>The selected choice text, or empty string if none selected.</returns>
    public async Task<string> PresentChoicesAsync(string prompt, string[] optionTexts)
    {
        if (Mode == TerminalMode.Disabled || TextRenderer == null || _ChoicePresenter == null)
        {
            GD.PushWarning("[TerminalUi] Cannot present choices - components not initialized.");
            return optionTexts.Length > 0 ? optionTexts[0] : string.Empty;
        }

        // Display prompt using base OmegaUi text rendering
        await AppendTextAsync(prompt);

        // Present choices using terminal-specific choice presenter
        var choices = new List<TerminalChoiceOption>();
        foreach (var text in optionTexts)
        {
            choices.Add(new TerminalChoiceOption { Text = text });
        }

        int selectedIndex = await _ChoicePresenter.PresentChoicesAsync(choices);
        return selectedIndex >= 0 && selectedIndex < optionTexts.Length
            ? optionTexts[selectedIndex]
            : string.Empty;
    }

    /// <summary>
    /// Updates caption text (terminal-specific feature).
    /// </summary>
    /// <param name="caption">The caption text to display.</param>
    public void UpdateCaption(string caption)
    {
        if (_CaptionLabel != null && CaptionsEnabled)
        {
            _CaptionLabel.Text = caption;
        }
    }

    // ========== Terminal Helper Methods ==========
    // These provide convenient access to common terminal operations

    /// <summary>
    /// Appends text with optional ghost effect (legacy parameter support).
    /// </summary>
    /// <param name="text">The text to append.</param>
    /// <param name="charDelaySeconds">The delay between characters in seconds.</param>
    /// <param name="useGhostEffect">Whether to use ghost effect (currently ignored, for API compatibility).</param>
    public async Task AppendTextAsync(string text, float charDelaySeconds = 0.03f, bool useGhostEffect = false)
    {
        // Ghost effect is handled by shader controller, not text renderer
        // Just call the base AppendTextAsync with delay
        await base.AppendTextAsync(text, charDelaySeconds);
    }



    /// <summary>
    /// Transitions to a different scene.
    /// </summary>
    /// <param name="scenePath">The path to the scene file.</param>
    protected void TransitionToScene(string scenePath)
    {
        // Use cached reference
        if (_SceneManager != null && _SceneManager.HasMethod("TransitionToScene"))
        {
            _SceneManager.Call("TransitionToScene", scenePath);
        }
        else
        {
            if (_SceneManager == null) GD.PushWarning("[TerminalUi] SceneManager not found, using fallback.");
            GetTree().ChangeSceneToFile(scenePath);
        }
    }

    /// <summary>
    /// Gets the game state singleton.
    /// </summary>
    /// <returns>The game state instance.</returns>
    protected GameState GetGameState()
    {
        // Use cached reference
        if (_GameState == null)
        {
            _GameState = GetNodeOrNull<GameState>("/root/GameState");
            if (_GameState == null)
                throw new InvalidOperationException("GameState singleton not found. Ensure it's autoloaded in project.godot");
        }
        return _GameState;
    }
}
