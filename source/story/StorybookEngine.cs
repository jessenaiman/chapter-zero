// <copyright file="StorybookEngine.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend.Narrative;

using System.Collections.Generic;
using Godot;

/// <summary>
/// Minimal narrative engine that processes a <see cref="StoryBlock"/> and exposes story state.
/// Provides methods for CinematicDirector to advance the story, and emits signals for events.
/// Decouples story logic from UI/scene management.
/// <para>
/// Related Architecture:
/// <see cref="CinematicDirector"/> - Uses this engine to drive narrative flow.
/// <see cref="StoryBlock"/> - The data source loaded from JSON.
/// </para>
/// <para>
/// Usage Example:
/// <code>
/// // In CinematicDirector or similar:
/// var engine = new StorybookEngine();
/// var story = StoryLoader.LoadJsonScript("res://data/story.json");
/// engine.LoadStory(story);
///
/// while (engine.HasNextScene)
/// {
///     engine.AdvanceToNextScene();
///     var lines = engine.GetCurrentLines();
///     // Display lines via GDScript UI...
///
///     if (!string.IsNullOrEmpty(engine.GetCurrentQuestion()))
///     {
///         var choices = engine.GetCurrentChoices();
///         // Present choices via GDScript UI...
///         // When choice made: engine.MakeChoice(selectedChoice.Text);
///     }
/// }
/// </code>
/// </para>
/// </summary>
public partial class StorybookEngine : Node
{
    private StoryBlock? _StoryBlock;
    private int _CurrentSceneIndex = -1;

    /// <summary>
    /// Gets the currently active scene, or null if no story is loaded.
    /// </summary>
    public Scene? CurrentScene => _StoryBlock?.Scenes?[_CurrentSceneIndex];

    /// <summary>
    /// Gets a value indicating whether there are more scenes to process.
    /// </summary>
    public bool HasNextScene => _CurrentSceneIndex < (_StoryBlock?.Scenes?.Count ?? 0) - 1;

    /// <summary>
    /// Signal emitted when a choice is made.
    /// </summary>
    [Signal]
    public delegate void ChoiceMadeEventHandler(string choiceText, string sceneId);

    /// <summary>
    /// Signal emitted when Dreamweaver scores are updated.
    /// </summary>
    [Signal]
    public delegate void DreamweaverScoresUpdatedEventHandler(Godot.Collections.Array scores);

    /// <summary>
    /// Loads a story block from the provided data.
    /// </summary>
    /// <param name="storyBlock">The story data to process.</param>
    public void LoadStory(StoryBlock storyBlock)
    {
        _StoryBlock = storyBlock;
        _CurrentSceneIndex = -1;
    }

    /// <summary>
    /// Advances to the next scene and emits SceneChanged.
    /// </summary>
    /// <returns>True if advanced successfully, false if no more scenes.</returns>
    public bool AdvanceToNextScene()
    {
        if (!HasNextScene) return false;

        _CurrentSceneIndex++;
        EmitSignal("SceneChanged", CurrentScene?.Id ?? "");
        return true;
    }

    /// <summary>
    /// Processes a choice selection and emits ChoiceMade.
    /// </summary>
    /// <param name="choiceText">The text of the selected choice.</param>
    public void MakeChoice(string choiceText)
    {
        EmitSignal("ChoiceMade", choiceText, CurrentScene?.Id ?? "");
    }

    /// <summary>
    /// Gets the narrative lines for the current scene.
    /// </summary>
    /// <returns>List of lines, or empty if no current scene.</returns>
    public IReadOnlyList<string> GetCurrentLines() => CurrentScene?.Lines ?? new List<string>();

    /// <summary>
    /// Gets the question for the current scene, if any.
    /// </summary>
    public string? GetCurrentQuestion() => CurrentScene?.Question;

    /// <summary>
    /// Updates Dreamweaver scores based on story completion and emits the update signal.
    /// </summary>
    /// <param name="dreamweaverIndex">Index of the Dreamweaver (0-2) whose score to update.</param>
    /// <param name="scoreValue">The score value to add (typically 1 or 2).</param>
    public void UpdateDreamweaverScore(int dreamweaverIndex, int scoreValue)
    {
        // Get current scores (initialize if needed)
        var currentScores = GetDreamweaverScores();

        // Update the specific Dreamweaver's score
        if (dreamweaverIndex >= 0 && dreamweaverIndex < currentScores.Count)
        {
            int currentScore = (int)currentScores[dreamweaverIndex];
            currentScores[dreamweaverIndex] = currentScore + scoreValue;
        }

        // Emit signal with updated scores
        EmitSignal("DreamweaverScoresUpdated", new Godot.Collections.Array(currentScores));
    }

    /// <summary>
    /// Gets the current Dreamweaver scores.
    /// </summary>
    /// <returns>Array of 3 integers representing Dreamweaver scores.</returns>
    public Godot.Collections.Array GetDreamweaverScores()
    {
        // In a real implementation, this would be stored persistently
        // For now, return default scores
        return new Godot.Collections.Array { 0, 0, 0 };
    }
}
