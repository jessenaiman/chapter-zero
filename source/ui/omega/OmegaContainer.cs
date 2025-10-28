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

        // Emit signal to indicate initialization is complete
        EmitSignal(SignalName.InitializationCompleted);
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
}
