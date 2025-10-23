using Godot;
using System;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.UI.Terminal;

/// <summary>
/// Base terminal UI orchestrator following SOLID principles.
/// Composes atomic components and manages their lifecycle.
/// Single Responsibility: Scene lifecycle and component composition only.
/// </summary>
[GlobalClass]
public partial class TerminalBase : BaseNarrativeScene
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
        /// <summary>Full terminal with shaders and effects (for Stage 1).</summary>
        Full
    }

    [Export] public TerminalMode Mode { get; set; } = TerminalMode.Full;
    [Export] public bool CaptionsEnabled { get; set; }

    // Component composition (Dependency Inversion Principle)
    private ITerminalShaderController? _shaderController;
    private ITerminalTextRenderer? _textRenderer;
    private ITerminalChoicePresenter? _choicePresenter;

    // Protected accessors for derived classes (Open/Closed Principle)
    protected ITerminalShaderController? ShaderController => _shaderController;
    protected ITerminalTextRenderer? TextRenderer => _textRenderer;
    protected ITerminalChoicePresenter? ChoicePresenter => _choicePresenter;

    // Node references (cached for performance)
    private ColorRect? _primaryShaderLayer;
    private RichTextLabel? _textDisplay;
    private VBoxContainer? _choiceContainer;
    private Label? _captionLabel;

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        if (Mode == TerminalMode.Disabled)
        {
            GD.Print("[TerminalBase] Terminal mode is Disabled - skipping initialization.");
            return;
        }

        try
        {
            InitializeTerminal();
        }
        catch (InvalidOperationException ex)
        {
            GD.PrintErr($"[TerminalBase] Terminal initialization failed: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[TerminalBase] Unexpected error during initialization: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Initializes the terminal by caching node references and creating components.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when required nodes are missing.</exception>
    private void InitializeTerminal()
    {
        CacheRequiredNodes();
        CreateComponents();
        InitializeComponentStates();
    }

    /// <summary>
    /// Caches references to required child nodes.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when required nodes are missing.</exception>
    private void CacheRequiredNodes()
    {
        // Required nodes - throw if missing
        _primaryShaderLayer = GetNode<ColorRect>("PhosphorLayer");
        if (_primaryShaderLayer == null)
            throw new InvalidOperationException("PhosphorLayer node is required but not found.");

        _textDisplay = GetNode<RichTextLabel>("TextDisplay");
        if (_textDisplay == null)
            throw new InvalidOperationException("TextDisplay node is required but not found.");

        _choiceContainer = GetNode<VBoxContainer>("ChoiceContainer");
        if (_choiceContainer == null)
            throw new InvalidOperationException("ChoiceContainer node is required but not found.");

        // Optional nodes - log warning if missing
        _captionLabel = GetNodeOrNull<Label>("CaptionLabel");
        if (_captionLabel == null)
            GD.PushWarning("[TerminalBase] CaptionLabel not found - captions disabled.");
    }

    /// <summary>
    /// Creates component instances using factory methods (Open/Closed Principle).
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when component creation fails.</exception>
    private void CreateComponents()
    {
        try
        {
            _shaderController = CreateShaderController(_primaryShaderLayer!);
            if (_shaderController == null)
                throw new InvalidOperationException("ShaderController creation returned null.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create ShaderController: {ex.Message}", ex);
        }

        try
        {
            _textRenderer = CreateTextRenderer(_textDisplay!);
            if (_textRenderer == null)
                throw new InvalidOperationException("TextRenderer creation returned null.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create TextRenderer: {ex.Message}", ex);
        }

        try
        {
            _choicePresenter = CreateChoicePresenter(_choiceContainer!);
            if (_choicePresenter == null)
                throw new InvalidOperationException("ChoicePresenter creation returned null.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create ChoicePresenter: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Initializes default states for UI components.
    /// </summary>
    private void InitializeComponentStates()
    {
        if (_textDisplay != null)
        {
            _textDisplay.Text = string.Empty;
            _textDisplay.Modulate = Colors.White;
        }

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
    /// Factory method for creating shader controller (Open/Closed Principle).
    /// Override in derived classes for custom implementations.
    /// </summary>
    /// <param name="display">The primary shader display layer.</param>
    /// <returns>An instance of ITerminalShaderController.</returns>
    protected virtual ITerminalShaderController CreateShaderController(ColorRect display)
    {
        return new TerminalShaderController(display);
    }

    /// <summary>
    /// Factory method for creating text renderer (Open/Closed Principle).
    /// Override in derived classes for custom implementations.
    /// </summary>
    /// <param name="textDisplay">The text display node.</param>
    /// <returns>An instance of ITerminalTextRenderer.</returns>
    protected virtual ITerminalTextRenderer CreateTextRenderer(RichTextLabel textDisplay)
    {
        return new TerminalTextRenderer(textDisplay);
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
    /// Appends text to the terminal with optional typing animation.
    /// Delegates to TextRenderer component (Single Responsibility).
    /// </summary>
    /// <param name="text">The text to append.</param>
    /// <param name="typingSpeed">Characters per second for typing animation.</param>
    /// <param name="delayBeforeStart">Delay in seconds before starting.</param>
    /// <returns>A task representing the async operation.</returns>
    public async Task AppendTextAsync(string text, float typingSpeed = 30f, float delayBeforeStart = 0f)
    {
        if (Mode == TerminalMode.Disabled || _textRenderer == null)
        {
            GD.PushWarning("[TerminalBase] Cannot append text - TextRenderer not initialized.");
            return;
        }

        await _textRenderer.AppendTextAsync(text, typingSpeed, delayBeforeStart).ConfigureAwait(false);
    }

    /// <summary>
    /// Presents choices to the player and waits for selection.
    /// Delegates to ChoicePresenter component (Single Responsibility).
    /// </summary>
    /// <param name="prompt">The prompt text to display.</param>
    /// <param name="optionTexts">The choice option texts.</param>
    /// <returns>The selected choice text, or empty string if none selected.</returns>
    public async Task<string> PresentChoicesAsync(string prompt, string[] optionTexts)
    {
        if (Mode == TerminalMode.Disabled || _textRenderer == null || _choicePresenter == null)
        {
            GD.PushWarning("[TerminalBase] Cannot present choices - components not initialized.");
            return optionTexts.Length > 0 ? optionTexts[0] : string.Empty;
        }

        // Display prompt
        await AppendTextAsync(prompt).ConfigureAwait(false);

        // Present choices
        var choices = new System.Collections.Generic.List<ChoiceOption>();
        foreach (var text in optionTexts)
        {
            choices.Add(new ChoiceOption { Text = text });
        }

        int selectedIndex = await _choicePresenter.PresentChoicesAsync(choices).ConfigureAwait(false);
        return selectedIndex >= 0 && selectedIndex < optionTexts.Length
            ? optionTexts[selectedIndex] 
            : string.Empty;
    }

    /// <summary>
    /// Clears all text from the terminal display.
    /// Delegates to TextRenderer component (Single Responsibility).
    /// </summary>
    public void ClearText()
    {
        if (_textRenderer != null)
        {
            _textRenderer.ClearText();
        }
    }

    /// <summary>
    /// Applies a visual preset to the terminal shaders.
    /// Delegates to ShaderController component (Single Responsibility).
    /// </summary>
    /// <param name="presetName">The name of the preset to apply.</param>
    /// <returns>A task representing the async operation.</returns>
    public async Task ApplyVisualPresetAsync(string presetName)
    {
        if (Mode == TerminalMode.Disabled || _shaderController == null)
        {
            GD.PushWarning("[TerminalBase] Cannot apply visual preset - ShaderController not initialized.");
            return;
        }

        await _shaderController.ApplyVisualPresetAsync(presetName).ConfigureAwait(false);
    }

    /// <summary>
    /// Performs a pixel dissolve effect on the terminal.
    /// Delegates to ShaderController component (Single Responsibility).
    /// </summary>
    /// <param name="durationSeconds">The duration of the effect.</param>
    /// <returns>A task representing the async operation.</returns>
    public async Task PixelDissolveAsync(float durationSeconds = 2.5f)
    {
        if (Mode == TerminalMode.Disabled || _shaderController == null)
        {
            GD.PushWarning("[TerminalBase] Cannot perform dissolve - ShaderController not initialized.");
            return;
        }

        await _shaderController.PixelDissolveAsync(durationSeconds).ConfigureAwait(false);
    }
}
