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
    /// Signal emitted when Dreamweaver points are awarded.
    /// </summary>
    [Signal]
    public delegate void DreamweaverPointsAwardedEventHandler(string dreamweaverType, int points);

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

        // Calculate and emit Dreamweaver points
        if (CurrentScene?.Owner != null && TryGetChoiceOwner(choiceText, out var choiceOwner) && choiceOwner != null)
        {
            int points = (CurrentScene.Owner == choiceOwner) ? 2 : 1;
            EmitSignal("DreamweaverPointsAwarded", choiceOwner, points);
        }
    }

    /// <summary>
    /// Gets the narrative lines for the current scene.
    /// </summary>
    /// <returns>List of lines, or empty if no current scene.</returns>
    public IReadOnlyList<string> GetCurrentLines() => CurrentScene?.Lines ?? new List<string>();

    /// <summary>
    /// Gets the question for the current scene, if any.
    /// </summary>
    public string? GetCurrentQuestion() => CurrentScene?.Choice?.Question?.FirstOrDefault();

    /// <summary>
    /// Attempts to get the owner of a choice from the choice text.
    /// </summary>
    /// <param name="choiceText">The text of the choice.</param>
    /// <param name="owner">The owner if found.</param>
    /// <returns>True if the owner was found, false otherwise.</returns>
    private bool TryGetChoiceOwner(string choiceText, out string? owner)
    {
        owner = null;

        if (CurrentScene?.Choice?.Options == null) return false;

        foreach (var choice in CurrentScene.Choice.Options)
        {
            if (choice.Text == choiceText)
            {
                owner = choice.Owner;
                return true;
            }
        }

        return false;
    }
}
