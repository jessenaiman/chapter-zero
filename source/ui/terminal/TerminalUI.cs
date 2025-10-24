using Godot;
using System;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.UI.Omega;

namespace OmegaSpiral.Source.UI.Terminal;

/// <summary>
/// Terminal UI that inherits from OmegaUI and adds terminal-specific behavior.
/// Provides basic choice presentation, terminal modes, and captions.
/// Used across multiple stages that require terminal-style interfaces.
/// This is a reusable base class - stage-specific logic should be in derived classes.
/// </summary>
[GlobalClass]
public partial class TerminalUI : OmegaUI
{
    /// <summary>
    /// Terminal operation modes.
    /// </summary>
    public enum TerminalMode
    {
        /// <summary>No terminal functionality (for minimal UI stages).</summary>
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
    private ITerminalChoicePresenter? _choicePresenter;

    /// <summary>
    /// Gets the choice presenter instance.
    /// </summary>
    protected ITerminalChoicePresenter? ChoicePresenter => _choicePresenter;

    // Terminal-specific nodes
#pragma warning disable CA1816 // Godot nodes don't need explicit disposal
    private VBoxContainer? _choiceContainer;
    private Label? _captionLabel;
#pragma warning restore CA1816

    // Cached autoload references for performance
    private Node? _sceneManager;
    private GameState? _gameState;

    /// <inheritdoc/>
    public override void _Ready()
    {
        if (Mode == TerminalMode.Disabled)
        {
            GD.Print("[TerminalUI] Terminal mode is Disabled - skipping initialization.");
            return;
        }

        // Call base class initialization (OmegaUI)
        base._Ready();

        // Cache autoload references for performance
        _sceneManager = GetNodeOrNull<Node>("/root/SceneManager");
        _gameState = GetNodeOrNull<GameState>("/root/GameState");

        if (_sceneManager == null) GD.PushWarning("[TerminalUI] SceneManager autoload not found.");
        if (_gameState == null) GD.PushWarning("[TerminalUI] GameState autoload not found.");

        try
        {
            InitializeTerminalComponents();
        }
        catch (InvalidOperationException ex)
        {
            GD.PrintErr($"[TerminalUI] Terminal-specific initialization failed: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[TerminalUI] Unexpected error during terminal initialization: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Caches terminal-specific node references in addition to base OmegaUI nodes.
    /// </summary>
    protected override void CacheRequiredNodes()
    {
        // Call base to cache OmegaUI nodes (PhosphorLayer, TextDisplay)
        base.CacheRequiredNodes();

        // Cache terminal-specific nodes
        _choiceContainer = GetNodeOrNull<VBoxContainer>("ChoiceContainer");
        if (_choiceContainer == null)
            throw new InvalidOperationException("Required node 'ChoiceContainer' not found in TerminalUI scene.");

        // Optional nodes - log warning if missing
        _captionLabel = GetNodeOrNull<Label>("CaptionLabel");
        if (_captionLabel == null)
            GD.PushWarning("[TerminalUI] CaptionLabel not found - captions disabled.");
    }

    /// <summary>
    /// Initializes terminal-specific components after base OmegaUI initialization.
    /// </summary>
    private void InitializeTerminalComponents()
    {
        try
        {
            _choicePresenter = CreateChoicePresenter(_choiceContainer!);
            if (_choicePresenter == null)
                throw new InvalidOperationException("ChoicePresenter creation returned null.");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[TerminalUI] Failed to create ChoicePresenter: {ex.Message}");
            throw new InvalidOperationException($"Failed to create ChoicePresenter. See inner exception.", ex);
        }
    }

    /// <summary>
    /// Initializes terminal-specific UI states.
    /// </summary>
    protected override void InitializeComponentStates()
    {
        // Call base to initialize OmegaUI states
        base.InitializeComponentStates();

        InitializeTerminalStates();
    }

    /// <summary>
    /// Initializes terminal-specific component states.
    /// </summary>
    private void InitializeTerminalStates()
    {
        if (_choiceContainer != null)
        {
            _choiceContainer.Visible = false;
        }

        if (_captionLabel != null)
        {
            _captionLabel.Visible = CaptionsEnabled;
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
    /// Terminal-specific behavior - not part of base OmegaUI.
    /// </summary>
    /// <param name="prompt">The prompt text to display.</param>
    /// <param name="optionTexts">The choice option texts.</param>
    /// <returns>The selected choice text, or empty string if none selected.</returns>
    public async Task<string> PresentChoicesAsync(string prompt, string[] optionTexts)
    {
        if (Mode == TerminalMode.Disabled || TextRenderer == null || _choicePresenter == null)
        {
            GD.PushWarning("[TerminalUI] Cannot present choices - components not initialized.");
            return optionTexts.Length > 0 ? optionTexts[0] : string.Empty;
        }

        // Display prompt using base OmegaUI text rendering
        await AppendTextAsync(prompt);

        // Present choices using terminal-specific choice presenter
        var choices = new List<ChoiceOption>();
        foreach (var text in optionTexts)
        {
            choices.Add(new ChoiceOption { Text = text });
        }

        int selectedIndex = await _choicePresenter.PresentChoicesAsync(choices);
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
        if (_captionLabel != null && CaptionsEnabled)
        {
            _captionLabel.Text = caption;
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
        if (_sceneManager != null && _sceneManager.HasMethod("TransitionToScene"))
        {
            _sceneManager.Call("TransitionToScene", scenePath);
        }
        else
        {
            if (_sceneManager == null) GD.PushWarning("[TerminalUI] SceneManager not found, using fallback.");
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
        if (_gameState == null)
        {
            _gameState = GetNodeOrNull<GameState>("/root/GameState");
            if (_gameState == null)
                throw new InvalidOperationException("GameState singleton not found. Ensure it's autoloaded in project.godot");
        }
        return _gameState;
    }
}
