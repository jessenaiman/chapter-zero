// <copyright file="IOmegaAudioDirector.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Audio;

/// <summary>
/// Defines orchestration callbacks for the Omega audio system.
/// Implementations bridge high-level game events to the layered audio architecture.
/// </summary>
public interface IOmegaAudioDirector
{
    /// <summary>
    /// Called when the boot sequence begins. Should trigger the cinematic boot timeline.
    /// </summary>
    void OnBootSequenceStarted();

    /// <summary>
    /// Called when the terminal reaches its stable interactive state.
    /// Should enable ambient beds and hybrid flux balancing.
    /// </summary>
    void OnStageStabilized();

    /// <summary>
    /// Called when the secret reveal cinematic begins.
    /// </summary>
    /// <param name="profile">Accessibility preferences that influence timing and mix.</param>
    void OnSecretRevealStarted(OmegaAudioAccessibilityProfile profile);

    /// <summary>
    /// Called when the secret reveal cinematic completes or is acknowledged.
    /// </summary>
    void OnSecretRevealCompleted();

    /// <summary>
    /// Called when a menu becomes the active focus layer.
    /// </summary>
    void OnMenuOpened();

    /// <summary>
    /// Called when a menu loses focus or is hidden.
    /// </summary>
    void OnMenuClosed();

    /// <summary>
    /// Called when the user navigates across selectable UI (buttons, menu entries).
    /// </summary>
    /// <param name="thread">Optional dreamweaver thread accent associated with the selection.</param>
    void OnUiHover(DreamweaverType? thread);

    /// <summary>
    /// Called when the user confirms a UI selection.
    /// </summary>
    /// <param name="thread">Optional dreamweaver thread accent associated with the selection.</param>
    void OnUiConfirm(DreamweaverType? thread);
}
