using Godot.Collections;

namespace OmegaSpiral.Source.Scripts.Common.Dialogue;

/// <summary>
/// Interface for dialogue data structures that can be loaded from JSON
/// </summary>
public interface IDialogueData
{
    /// <summary>
    /// Gets the dialogue lines for the initial greeting/opening
    /// </summary>
    List<string> OpeningLines { get; }
    
    /// <summary>
    /// Gets the dialogue lines for the main content
    /// </summary>
    List<string> DialogueLines { get; }
    
    /// <summary>
    /// Gets the choices available to the player
    /// </summary>
    List<IDialogueChoice> Choices { get; }
    
    /// <summary>
    /// Gets any special narrative blocks or responses
    /// </summary>
    List<INarrativeBlock> NarrativeBlocks { get; }
}

/// <summary>
/// Interface representing a dialogue choice
/// </summary>
public interface IDialogueChoice
{
    /// <summary>
    /// Gets the unique identifier for this choice
    /// </summary>
    string Id { get; }
    
    /// <summary>
    /// Gets the display text for this choice
    /// </summary>
    string Text { get; }
    
    /// <summary>
    /// Gets the description or additional context for this choice
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Gets the index of the next dialogue block to show
    /// </summary>
    int NextDialogueIndex { get; }
}

/// <summary>
/// Interface representing a narrative block containing paragraphs and choices
/// </summary>
public interface INarrativeBlock
{
    /// <summary>
    /// Gets the paragraphs of text in this narrative block
    /// </summary>
    List<string> Paragraphs { get; }
    
    /// <summary>
    /// Gets the question or prompt following the paragraphs
    /// </summary>
    string Question { get; }
    
    /// <summary>
    /// Gets the choices available after this narrative block
    /// </summary>
    List<IDialogueChoice> Choices { get; }
    
    /// <summary>
    /// Gets the index of the next narrative block
    /// </summary>
    int NextBlock { get; }
}

/// <summary>
/// Interface for dialogue parsers that convert JSON data to dialogue structures
/// </summary>
public interface IDialogueParser
{
    /// <summary>
    /// Parses JSON data into a dialogue data structure
    /// </summary>
    /// <param name="jsonData">The JSON data dictionary to parse</param>
    /// <returns>An IDialogueData instance</returns>
    IDialogueData ParseDialogueData(Dictionary<string, Variant> jsonData);
    
    /// <summary>
    /// Validates the parsed dialogue data against the expected schema
    /// </summary>
    /// <param name="dialogueData">The dialogue data to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    bool ValidateDialogueData(IDialogueData dialogueData);
}

/// <summary>
/// Interface for dialogue managers that handle loading, caching, and serving dialogue
/// </summary>
public interface IDialogueManager
{
    /// <summary>
    /// Loads dialogue data from a JSON resource path
    /// </summary>
    /// <param name="resourcePath">Path to the dialogue JSON file</param>
    /// <returns>A task that resolves to the dialogue data</returns>
    Task<IDialogueData> LoadDialogueAsync(string resourcePath);
    
    /// <summary>
    /// Gets cached dialogue data by ID, or loads it if not cached
    /// </summary>
    /// <param name="dialogueId">The unique identifier for the dialogue</param>
    /// <returns>The dialogue data, or null if not found</returns>
    Task<IDialogueData> GetDialogueAsync(string dialogueId);
    
    /// <summary>
    /// Preloads and caches dialogue data for future use
    /// </summary>
    /// <param name="dialogueId">The identifier for the dialogue to preload</param>
    /// <param name="resourcePath">The path to the dialogue resource</param>
    void PreloadDialogue(string dialogueId, string resourcePath);
    
    /// <summary>
    /// Clears cached dialogue data
    /// </summary>
    /// <param name="dialogueId">The ID of the dialogue to clear, or null for all</param>
    void ClearCache(string? dialogueId = null);
}