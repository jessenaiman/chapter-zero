// <copyright file="SceneBase.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using Godot;

namespace OmegaSpiral.Source.Scripts.Infrastructure;

/// <summary>
/// Shared base class for beat/scene scripts across stages.
/// Provides lightweight helpers for dynamic UI construction and beat progression hooks.
/// </summary>
[GlobalClass]
public abstract partial class SceneBase : Control
{
    /// <summary>
    /// Gets the identifier of the current beat (should align with manifest entry where applicable).
    /// </summary>
    protected virtual string CurrentBeatId => string.Empty;

    /// <summary>
    /// Gets the stage manifest path used for determining sequencing.
    /// </summary>
    protected virtual string StageManifestPath => string.Empty;

    /// <summary>
    /// Adds a <see cref="RichTextLabel"/> to the specified container.
    /// </summary>
    /// <param name="container">Parent node that will host the label.</param>
    /// <param name="text">Text to display.</param>
    /// <param name="fontSize">Optional font size hint for future theming.</param>
    protected void AddTextLabel(Control container, string text, int fontSize = 14)
    {
        if (container == null)
        {
            throw new ArgumentNullException(nameof(container));
        }

        var label = new RichTextLabel
        {
            Text = text,
            CustomMinimumSize = new Vector2(0, 24)
        };

        // TODO: Wire up project-wide theme support for font size overrides.
        container.AddChild(label);
    }

    /// <summary>
    /// Adds a <see cref="Button"/> and associates a callback with its Pressed event.
    /// </summary>
    /// <param name="container">Parent node that will host the button.</param>
    /// <param name="text">Button text.</param>
    /// <param name="onPressed">Callback invoked when the button is pressed.</param>
    protected void AddButton(Control container, string text, Action onPressed)
    {
        if (container == null)
        {
            throw new ArgumentNullException(nameof(container));
        }

        if (onPressed == null)
        {
            throw new ArgumentNullException(nameof(onPressed));
        }

        var button = new Button
        {
            Text = text
        };

        button.Pressed += onPressed;
        container.AddChild(button);
    }

    /// <summary>
    /// Hook for advancing to the next beat/scene. Stages should override to integrate with their orchestrators.
    /// </summary>
    protected virtual void TransitionToNextBeat()
    {
        GD.Print($"[SceneBase] Request transition from {CurrentBeatId} (manifest: {StageManifestPath})");
    }

    /// <summary>
    /// Retrieves the active <see cref="StageController"/> for the current scene tree, if any.
    /// </summary>
    /// <returns>The active stage controller or <c>null</c> if none registered.</returns>
    protected StageController? GetStageController()
    {
        return StageController.GetActiveController(GetTree());
    }

    /// <summary>
    /// Awards affinity points to the active stage controller, if available.
    /// </summary>
    /// <param name="alignmentId">Alignment identifier (e.g., light, shadow, ambition).</param>
    /// <param name="points">Points to award (can be negative).</param>
    protected void AwardAffinity(string alignmentId, int points)
    {
        GetStageController()?.AwardAffinityScore(alignmentId, points);
    }
}
