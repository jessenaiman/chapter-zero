// <copyright file="StorybookEngine.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend.Narrative;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Narrative;

/// <summary>
/// Generic narrative engine that drives a <see cref="StoryScriptRoot"/> using a <see cref="NarrativeUi"/>.
/// It is stage-agnostic – any UI derived from <see cref="NarrativeUi"/> can participate via signals.
/// </summary>
public sealed class NarrativeEngine
{
    /// <summary>
    /// Plays the entire script in order, delegating all UI work to the supplied handler.
    /// </summary>
    /// <param name="script">The fully parsed narrative script.</param>
    /// <param name="handler">UI node derived from <see cref="NarrativeUi"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task PlayAsync(StoryScriptRoot script, NarrativeUi handler)
    {
        if (script == null) throw new ArgumentNullException(nameof(script));
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        // Optional boot / intro sequence (UI can decide what to do)
        await handler.PlayBootSequenceAsync();

        var scenes = script.Scenes ?? new List<StoryScriptElement>();
        foreach (var scene in scenes)
        {
            await PlaySceneAsync(scene, handler);
        }

        // Notify UI that the whole script has finished
        await handler.NotifySequenceCompleteAsync();
    }

    private async Task PlaySceneAsync(StoryScriptElement scene, NarrativeUi handler)
    {
        // 1️⃣ Display normal lines (skip command lines)
        var displayLines = new List<string>();
        if (scene.Lines != null)
        {
            foreach (var line in scene.Lines)
            {
                // Command lines are bracketed – let the handler decide
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    var handled = await handler.HandleCommandLineAsync(line);
                    if (!handled)
                    {
                        displayLines.Add(line);
                    }
                }
                else
                {
                    displayLines.Add(line);
                }
            }
        }

        if (displayLines.Count > 0)
        {
            await handler.DisplayLinesAsync(displayLines);
        }

        // 2️⃣ Apply any scene‑specific visual/timing effects
        await handler.ApplySceneEffectsAsync(scene);

        // 3️⃣ If the scene has a question, present choices
        if (!string.IsNullOrWhiteSpace(scene.Question) && scene.Choice?.Count > 0)
        {
            var selected = await handler.PresentChoiceAsync(
                scene.Question,
                scene.Owner ?? "system",
                scene.Choice);

            // 4️⃣ Let the handler update game state based on the choice
            await handler.ProcessChoiceAsync(selected);
        }

        // 5️⃣ Optional pause after the scene
        if (scene.Pause.HasValue && scene.Pause.Value > 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(scene.Pause.Value));
        }
    }
}
