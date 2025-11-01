// <copyright file="OmegaContainer.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Ui.Omega
{
    using Godot;

    /// <summary>
    /// Base container for all UI components with lifecycle management and event signaling.
    ///
    /// Provides one generic, extensible signal mechanism for any interactive component to notify
    /// orchestrators of completion or state changes. Subclasses emit this signal with domain-specific
    /// data - the container stays agnostic about the payload.
    ///
    /// This enables loose coupling: UI components don't know about orchestrators, but can still
    /// signal when they're done, allowing orchestrators (SceneManager, CinematicDirector) to
    /// await and proceed accordingly.
    /// </summary>
    /// <remarks>
    /// Architecture: Observer Pattern with Generic Events
    /// - One signal accommodates all use cases (narrative complete, choice made, sequence done, etc.)
    /// - Subclasses simply emit with whatever context they need
    /// - Orchestrators await the signal without caring about implementation details
    /// - No coupling between UI layer and orchestration layer
    ///
    /// This follows SOLID principles:
    /// - Open/Closed: New UI behaviors can emit without modifying OmegaContainer
    /// - Liskov Substitution: Any OmegaContainer subclass can be used interchangeably
    /// - Dependency Inversion: Orchestrators depend on signal contract, not UI implementation
    /// </remarks>
    [GlobalClass]
    public partial class OmegaContainer : Control
    {
        /// <summary>
        /// Generic signal emitted when any async operation completes.
        /// Payload is domain-specific and determined by the emitter.
        ///
        /// Usage examples:
        /// - NarrativeUi emits with eventType="display_finished"
        /// - InteractiveUi emits with eventType="choice_made", choice data
        /// - Any UI emits with custom eventType and context as needed
        /// </summary>
        [Signal]
        public delegate void InteractionCompleteEventHandler(string eventType, Variant context);

        /// <summary>
        /// Called by Godot when the node enters the scene tree.
        /// Sets up layout anchors to fill parent container.
        /// </summary>
        public override void _Ready()
        {
            base._Ready();
            this.AnchorLeft = 0f;
            this.AnchorTop = 0f;
            this.AnchorRight = 1f;
            this.AnchorBottom = 1f;
            this.OffsetLeft = 0;
            this.OffsetTop = 0;
            this.OffsetRight = 0;
            this.OffsetBottom = 0;
        }

        /// <summary>
        /// Emits the interaction_complete signal with event type and optional context.
        /// This is the only signal mechanism needed - subclasses use it for any completion event.
        /// </summary>
        /// <param name="eventType">The type of event (e.g., "display_finished", "choice_made", "sequence_done").</param>
        /// <param name="context">Optional context data relevant to this event.</param>
        protected void EmitInteractionComplete(string eventType, Variant context = default)
        {
            this.EmitSignal(SignalName.InteractionComplete, eventType, context);
        }
    }
}
