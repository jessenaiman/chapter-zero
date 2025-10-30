// <copyright file="GhostSceneManager.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Stages.Stage1;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Backend;
using OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Executes a single Ghost Terminal scene.
/// Displays narrative lines and handles player choices.
/// </summary>
public sealed partial class GhostSceneManager : SceneManager
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GhostSceneManager"/> class.
    /// </summary>
    /// <param name="scene">The scene element to play.</param>
    /// <param name="data">Additional data for the scene.</param>
    public GhostSceneManager(StoryScriptElement scene, object data)
        : base(scene, data)
    {
    }

    /// <summary>
    /// Runs the scene by displaying lines and handling choices.
    /// </summary>
    /// <returns>A task that completes when the scene finishes.</returns>
    public new async Task RunSceneAsync()
    {
        try
        {
            GD.Print($"[GhostSceneManager] Starting scene: {this.SceneData.Id}");

            // Display narrative lines if present
            if (this.SceneData.Lines?.Count > 0)
            {
                await this.DisplayLinesAsync(this.SceneData.Lines);
            }

            // Present choices if available
            if (!string.IsNullOrEmpty(this.SceneData.Question) && this.SceneData.Choice?.Count > 0)
            {
                var selectedChoice = await this.PresentChoiceAsync(
                    this.SceneData.Question,
                    this.SceneData.Owner ?? "omega",
                    this.SceneData.Choice);

                GD.Print($"[GhostSceneManager] Selected choice: {selectedChoice?.Id}");
            }

            // Apply any scene-specific effects (pause, etc.)
            if (this.SceneData.Pause.HasValue && this.SceneData.Pause.Value > 0)
            {
                await Task.Delay(this.SceneData.Pause.Value);
            }

            GD.Print($"[GhostSceneManager] Completed scene: {this.SceneData.Id}");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[GhostSceneManager] Error running scene: {ex.Message}");
            GD.PrintErr(ex.StackTrace);
        }
    }

    /// <summary>
    /// Displays narrative lines to the player.
    /// </summary>
    /// <param name="lines">The lines to display.</param>
    /// <returns>A task that completes when all lines have been displayed.</returns>
    private async Task DisplayLinesAsync(IList<string> lines)
    {
        foreach (var line in lines)
        {
            if (line.StartsWith("["))
            {
                // Handle command lines (e.g., [GLITCH], [PAUSE:500])
                await this.HandleCommandLineAsync(line);
            }
            else
            {
                // Display narrative line
                GD.Print($"[Ghost] {line}");

                // Simple delay for readability
                await Task.Delay(100);
            }
        }
    }

    /// <summary>
    /// Handles special command lines in the narrative.
    /// </summary>
    /// <param name="commandLine">The command line to handle.</param>
    /// <returns>A task that completes when the command has been processed.</returns>
    private async Task HandleCommandLineAsync(string commandLine)
    {
        GD.Print($"[GhostSceneManager] Command: {commandLine}");

        // Parse command (e.g., [PAUSE:500], [GLITCH], [EFFECT:screenShake])
        if (commandLine.Contains("PAUSE:"))
        {
            var parts = commandLine.Split(':');
            if (parts.Length > 1 && int.TryParse(parts[1].TrimEnd(']'), out var pauseMs))
            {
                await Task.Delay(pauseMs);
            }
        }
    }

    /// <summary>
    /// Presents a choice to the player and waits for selection.
    /// </summary>
    /// <param name="question">The question text.</param>
    /// <param name="context">The context/speaker.</param>
    /// <param name="choices">The available choices.</param>
    /// <returns>The selected choice.</returns>
    private async Task<ChoiceOption> PresentChoiceAsync(
        string question,
        string context,
        IList<ChoiceOption> choices)
    {
        GD.Print($"\n[{context}] {question}");

        for (int i = 0; i < choices.Count; i++)
        {
            GD.Print($"  {i + 1}. {choices[i].Text}");
        }

        // For now, return the first choice as a placeholder
        // TODO: Implement actual input handling for choice selection
        GD.Print("[GhostSceneManager] Auto-selecting first choice (placeholder)");
        await Task.Delay(500);

        return choices[0];
    }
}
