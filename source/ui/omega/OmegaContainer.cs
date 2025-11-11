// <copyright file="OmegaContainer.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Narrative;

namespace OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// Base container for UI with component lifecycle management.
/// Pure composition - no visual theming code, just lifecycle orchestration.
/// Subclasses decide which Omega components to use via composition.
/// </summary>
/// <remarks>
/// Architecture: Template Method Pattern
/// - _Ready() defines the initialization sequence
/// - Subclasses override virtual hooks to customize behavior
/// - No visual styling - that's done via composition with OmegaComponentFactory
/// </remarks>
[GlobalClass]
public partial class OmegaContainer : Control
{
    /// <summary>
    /// Emitted when UI has completed initialization.
    /// Subclasses can connect to this signal for dependency setup.
    /// </summary>
    [Signal]
    public delegate void InitializationCompletedEventHandler();

    /// <summary>
    /// Gets the shader controller component if present in the scene.
    /// </summary>
    public OmegaShaderController? ShaderController { get; protected set; }

    /// <summary>
    /// Gets the text renderer component if present in the scene.
    /// </summary>
    public OmegaTextRenderer? TextRenderer { get; protected set; }

    /// <summary>
    /// Gets or sets the choice presenter component.
    /// This references the narrative-driven choice presenter for managing player choices.
    /// </summary>
    public NarrativeChoicePresenter? ChoicePresenter { get; protected set; }

    /// <summary>
    /// Called by Godot when the node enters the scene tree.
    /// Standard initialization pattern - subclasses override to implement specific behavior.
    /// </summary>
    /// <summary>
    /// Called by Godot when the node enters the scene tree.
    /// Standard initialization pattern - subclasses override to implement specific behavior.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        // Ensure proper anchoring so this control fills its parent
        AnchorLeft = 0f;
        AnchorTop = 0f;
        AnchorRight = 1f;
        AnchorBottom = 1f;
        OffsetLeft = 0;
        OffsetTop = 0;
        OffsetRight = 0;
        OffsetBottom = 0;

        // Create OmegaBorderFrame if enabled via export property
        CreateBorderFrameIfEnabled();

        // Emit signal to indicate initialization is complete
        EmitSignal(SignalName.InitializationCompleted);
    }

    /// <summary>
    /// Creates the OmegaBorderFrame if enable_omega_border is true.
    /// Checks for existing BorderFrame and creates one if missing.
    /// </summary>
    /// <summary>
    /// Creates the OmegaBorderFrame if enable_omega_border is true.
    /// Checks for existing BorderFrame and creates one if missing.
    /// </summary>
    /// <summary>
    /// Creates the OmegaBorderFrame if enable_omega_border is true.
    /// Checks for existing BorderFrame and creates one if missing.
    /// </summary>
    /// <summary>
    /// Creates the OmegaBorderFrame if enable_omega_border is true.
    /// Checks for existing BorderFrame and creates one if missing.
    /// </summary>
    private void CreateBorderFrameIfEnabled()
    {
        // Check if enable_omega_border property is set (default: true)
        bool enableBorder = true;

        // Look for enable_omega_border property in this node or scene
        if (HasMeta("enable_omega_border"))
        {
            enableBorder = (bool) GetMeta("enable_omega_border");
        }

        if (!enableBorder)
        {
            return; // Border frame disabled
        }

        // Check if BorderFrame already exists in the scene tree
        var existingBorder = GetNodeOrNull<ColorRect>("BorderFrame");
        if (existingBorder != null)
        {
            return; // BorderFrame already exists
        }

        // Create new OmegaBorderFrame and add it
        var borderFrame = new OmegaBorderFrame();
        AddChild(borderFrame);

        // Ensure BorderFrame fills parent properly - set anchors manually
        borderFrame.AnchorLeft = 0.0f;
        borderFrame.AnchorTop = 0.0f;
        borderFrame.AnchorRight = 1.0f;
        borderFrame.AnchorBottom = 1.0f;
        borderFrame.OffsetLeft = 0;
        borderFrame.OffsetTop = 0;
        borderFrame.OffsetRight = 0;
        borderFrame.OffsetBottom = 0;
    }

    /// <summary>
    /// Called when node exits the scene tree.
    /// Cleanup is handled by Godot's reference counting and AutoFree in tests.
    /// </summary>
    public override void _ExitTree()
    {
        base._ExitTree();
        // Reset shader effects in the Node owner (this), not in the service
        ShaderController?.ResetShaderEffects();
    }

    /// <summary>
    /// Appends text asynchronously using the text renderer.
    /// </summary>
    /// <param name="text">The text to append.</param>
    /// <param name="typingSpeed">The typing speed in characters per second.</param>
    /// <param name="delayBeforeStart">Delay before starting the animation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AppendTextAsync(string text, float typingSpeed = 30f, float delayBeforeStart = 0f)
    {
        if (TextRenderer != null)
        {
            await TextRenderer.AppendTextAsync(text, typingSpeed, delayBeforeStart);
        }
    }

    /// <summary>
    /// Clears the text using the text renderer.
    /// </summary>
    public void ClearText()
    {
        TextRenderer?.ClearText();
    }

    /// <summary>
    /// Displays text with typing animation.
    /// </summary>
    /// <param name="text">The text to display.</param>
    /// <param name="typingSpeed">Characters per second.</param>
    public async Task DisplayTextAsync(string text, float typingSpeed = 30f)
    {
        await AppendTextAsync(text, typingSpeed);
    }

    /// <summary>
    /// Plays a boot sequence with shader preset and typed lines.
    /// </summary>
    /// <param name="lines">Lines of text to display.</param>
    /// <param name="typingSpeed">Typing speed for each line.</param>
    /// <param name="delayBetweenLines">Delay between lines.</param>
    public async Task PlayBootSequenceAsync(string[] lines, float typingSpeed = 50f, float delayBetweenLines = 0.5f)
    {
        if (ShaderController != null)
        {
            try
            {
                await ShaderController.ApplyVisualPresetAsync("boot_sequence");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Boot sequence shader failed: {ex.Message}");
            }
        }

        foreach (var line in lines)
        {
            await DisplayTextAsync(line, typingSpeed);
            if (delayBetweenLines > 0)
            {
                await ToSignal(GetTree().CreateTimer(delayBetweenLines), SceneTreeTimer.SignalName.Timeout);
            }
        }
    }

    /// <summary>
    /// Plays narrative beats sequentially.
    /// </summary>
    /// <param name="beats">The beats to play.</param>
    public async Task PlayNarrativeBeatsAsync(NarrativeBeat[] beats)
    {
        if (beats == null || beats.Length == 0) return;

        foreach (var beat in beats)
        {
            await ProcessNarrativeBeatAsync(beat);
        }
    }

    /// <summary>
    /// Processes a single narrative beat.
    /// </summary>
    /// <param name="beat">The beat to process.</param>
    private async Task ProcessNarrativeBeatAsync(NarrativeBeat beat)
    {
        await ApplyVisualPresetIfNeededAsync(beat);
        await ApplyDelayIfNeededAsync(beat);
        await DisplayTextIfNeededAsync(beat);
    }

    /// <summary>
    /// Applies visual preset if specified in the beat.
    /// </summary>
    /// <param name="beat">The narrative beat.</param>
    private async Task ApplyVisualPresetIfNeededAsync(NarrativeBeat beat)
    {
        if (!string.IsNullOrEmpty(beat.VisualPreset) && ShaderController != null)
        {
            try
            {
                await ShaderController.ApplyVisualPresetAsync(beat.VisualPreset);
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Visual preset failed: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Applies delay if specified in the beat.
    /// </summary>
    /// <param name="beat">The narrative beat.</param>
    private async Task ApplyDelayIfNeededAsync(NarrativeBeat beat)
    {
        if (beat.DelaySeconds > 0)
        {
            await ToSignal(GetTree().CreateTimer(beat.DelaySeconds), SceneTreeTimer.SignalName.Timeout);
        }
    }

    /// <summary>
    /// Displays text if specified in the beat.
    /// </summary>
    /// <param name="beat">The narrative beat.</param>
    private async Task DisplayTextIfNeededAsync(NarrativeBeat beat)
    {
        if (!string.IsNullOrEmpty(beat.Text))
        {
            float speed = beat.TypingSpeed > 0 ? beat.TypingSpeed : 30f;
            string text = beat.Text.EndsWith("\n") ? beat.Text : beat.Text + "\n";
            await DisplayTextAsync(text, speed);
        }
    }
}
