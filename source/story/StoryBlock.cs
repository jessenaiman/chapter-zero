// <copyright file="StoryBlock.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend.Narrative;

using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

/// <summary>
/// Represents the entire story structure loaded from JSON files (e.g., ghost.json, nethack.json).
/// This class serves as the data model for narrative content, deserialized from JSON and used by the story system.
/// Signals are emitted to communicate with the GDScript state managers for scene transitions and choice handling.
/// <para>
/// Related Architecture:
/// <see cref="OmegaSpiralGameState"/> - Handles global game state persistence and reacts to story events.
/// <see cref="LevelAndStateManager.gd"/> - Manages scene loading and transitions based on story signals.
/// </para>
/// </summary>
public partial class StoryBlock : Node
{
    /// <summary>
    /// Gets or sets the story title.
    /// </summary>
    [JsonProperty("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the speaker/narrator for the story.
    /// </summary>
    [JsonProperty("speaker")]
    public string? Speaker { get; set; }

    /// <summary>
    /// Gets or sets the story description.
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the list of story scenes.
    /// Each scene contains narrative lines and optional choices.
    /// </summary>
    [JsonProperty("scenes")]
    public List<Scene>? Scenes { get; set; }

    /// <summary>
    /// Signal emitted when the player makes a choice in a scene.
    /// Connected by GDScript state managers to update game state and trigger scene changes.
    /// </summary>
    [Signal]
    public delegate void ChoiceMadeEventHandler(string choiceId, string sceneId);

    /// <summary>
    /// Signal emitted when transitioning to a new scene.
    /// Used by GDScript to load the appropriate scene and update UI.
    /// </summary>
    [Signal]
    public delegate void SceneChangedEventHandler(string sceneId);

    /// <summary>
    /// Emits the ChoiceMade signal when a player selects an option.
    /// This decouples the narrative logic from state management.
    /// </summary>
    /// <param name="choiceId">The identifier of the selected choice.</param>
    /// <param name="sceneId">The identifier of the current scene.</param>
    public void EmitChoiceMade(string choiceId, string sceneId)
    {
        EmitSignal(SignalName.ChoiceMade, choiceId, sceneId);
    }

    /// <summary>
    /// Emits the SceneChanged signal when moving to a new scene.
    /// Allows GDScript to handle scene loading asynchronously.
    /// </summary>
    /// <param name="sceneId">The identifier of the new scene.</param>
    public void EmitSceneChanged(string sceneId)
    {
        EmitSignal(SignalName.SceneChanged, sceneId);
    }
}

/// <summary>
/// Represents a scene in the story, which can be narrative, combat, or other types.
/// Scenes contain narrative content and may include player choices or combat encounters.
/// </summary>
public class Scene
{
    /// <summary>
    /// Gets or sets the scene type (e.g., "narrative", "combat").
    /// Defaults to "narrative" if not specified.
    /// </summary>
    [JsonProperty("type")]
    public string? Type { get; set; } = "narrative";

    /// <summary>
    /// Gets or sets the unique identifier for this scene.
    /// Used for scene transitions and state tracking.
    /// </summary>
    [JsonProperty("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the owner/speaker for this scene (e.g., a dreamweaver persona).
    /// </summary>
    [JsonProperty("owner")]
    public string? Owner { get; set; }

    /// <summary>
    /// Gets or sets the narrative lines displayed in this scene.
    /// Each string represents a line of dialogue or description.
    /// </summary>
    [JsonProperty("lines")]
    public List<string>? Lines { get; set; }

    /// <summary>
    /// Gets or sets the choice options available in this scene.
    /// If null, the scene is narrative-only.
    /// </summary>
    [JsonProperty("choice")]
    public ChoiceBlock? Choice { get; set; }

    /// <summary>
    /// Gets or sets combat encounter data for combat scenes.
    /// Only used when Type is "combat".
    /// </summary>
    [JsonProperty("combat_data")]
    public StoryCombatEncounter? CombatData { get; set; }
}

/// <summary>
/// Represents a choice block in a story scene, containing a question and multiple options.
/// This matches the JSON structure where "choice" is an object with "question" and "options".
/// </summary>
public class ChoiceBlock
{
    /// <summary>
    /// Gets or sets the question text presented when choices are available.
    /// Can be multiple lines for complex questions.
    /// </summary>
    [JsonProperty("question")]
    public List<string>? Question { get; set; }

    /// <summary>
    /// Gets or sets the choice options available in this scene.
    /// Each option has owner, text, and response properties.
    /// </summary>
    [JsonProperty("options")]
    public List<ChoiceOption>? Options { get; set; }
}

/// <summary>
/// Represents a choice option in a story scene, allowing player interaction.
/// Choices drive the narrative branching and emit signals for state updates.
/// </summary>
public class ChoiceOption
{
    /// <summary>
    /// Gets or sets the dreamweaver owner associated with this choice.
    /// Influences narrative style and AI-driven story generation.
    /// </summary>
    [JsonProperty("owner")]
    public string? Owner { get; set; }

    /// <summary>
    /// Gets or sets the display text for this choice option.
    /// Shown to the player in the UI.
    /// </summary>
    [JsonProperty("text")]
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the response lines shown after this choice is selected.
    /// Each string represents a line of narrative response.
    /// </summary>
    [JsonProperty("response")]
    public List<string>? Response { get; set; }
}
