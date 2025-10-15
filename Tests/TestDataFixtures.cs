using System;
using System.Collections.Generic;
using System.Linq;
using OmegaSpiral.Source.Scripts.Common; // Import main project types

namespace OmegaSpiral.Tests;

/// <summary>
/// Provides test data fixtures for scenes and story sections used in unit tests.
/// </summary>
public static class TestDataFixtures
{
    /// <summary>
    /// Loads mock scene data for testing purposes based on the provided scene name.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load data for.</param>
    /// <returns>
    /// A mock scene data instance containing test data for the specified scene.
    /// </returns>
    public static MockSceneData LoadSceneData(string sceneName)
    {
        return new MockSceneData
        {
            Name = sceneName,
            HasOmegaAsPrimary = sceneName == "Scene1",
            HasDirectPlayerQuestions = sceneName == "Scene1"
        };
    }

    /// <summary>
    /// Gets a mock story section database for testing.
    /// </summary>
    /// <returns>A mock story section database.</returns>
    public static MockStorySectionDatabase GetStorySectionDatabase()
    {
        return new MockStorySectionDatabase();
    }
}

/// <summary>
/// Represents mock scene data for testing purposes.
/// </summary>
public class MockSceneData
{
    /// <summary>
    /// Gets or sets the name of the scene.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether Omega is the primary character in this scene.
    /// </summary>
    public bool HasOmegaAsPrimary { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this scene contains direct player questions.
    /// </summary>
    public bool HasDirectPlayerQuestions { get; set; }

    /// <summary>
    /// Gets the questions associated with this scene.
    /// </summary>
    /// <returns>A list of mock questions.</returns>
    public List<MockQuestion> GetQuestions()
    {
        return new List<MockQuestion>
        {
            new MockQuestion { Prompt = "Question 1" },
            new MockQuestion { Prompt = "Question 2" },
            new MockQuestion { Prompt = "Question 3" }
        };
    }
}

/// <summary>
/// Represents a mock question for testing.
/// </summary>
public class MockQuestion
{
    /// <summary>
    /// Gets or sets the question prompt text.
    /// </summary>
    public string Prompt { get; set; } = string.Empty;
}

/// <summary>
/// Represents a mock story section database for testing.
/// </summary>
public class MockStorySectionDatabase
{
    /// <summary>
    /// Gets sections for the specified story.
    /// </summary>
    /// <param name="storyName">The name of the story.</param>
    /// <returns>A list of story section identifiers.</returns>
    public List<string> GetSections(string storyName)
    {
        _ = storyName; // Parameter intentionally unused in mock
        return new List<string> { "story1", "story2", "story3", "story4", "story5" };
    }
}

/// <summary>
/// Mock types for testing that don't exist in the main project
/// </summary>
/// <summary>
/// Loads scene script data for testing purposes.
/// </summary>
public class SceneScriptLoader
{
    private readonly bool nobodyWhoEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneScriptLoader"/> class.
    /// </summary>
    /// <param name="nobodyWhoEnabled">Indicates whether the NobodyWho service is enabled.</param>
    public SceneScriptLoader(bool nobodyWhoEnabled)
    {
        this.nobodyWhoEnabled = nobodyWhoEnabled;
    }

    /// <summary>
    /// Loads script data for the specified scene.
    /// </summary>
    /// <param name="_">The name of the scene to load (unused).</param>
    /// <returns>The loaded script data.</returns>
    public ScriptData Load(string _)
    {
        return new ScriptData
        {
            Source = nobodyWhoEnabled ? ScriptSource.Fallback : ScriptSource.LLMGenerated,
            Content = "Test content",
            Timestamp = DateTime.Now
        };
    }
}

/// <summary>
/// Represents script data for testing purposes.
/// </summary>
public class ScriptData
{
    /// <summary>
    /// Gets or sets the source of the script data.
    /// </summary>
    public ScriptSource Source { get; set; }

    /// <summary>
    /// Gets or sets the content of the script.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp of when the script was created.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the lines of the script.
    /// </summary>
    public List<string> Lines { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets a value indicating whether the script is complete.
    /// </summary>
    public bool IsComplete { get; set; } = true;

    /// <summary>
    /// Gets or sets the origin of the script data.
    /// </summary>
    public DataOrigin Origin { get; set; } = DataOrigin.RealGameplaySession;

    /// <summary>
    /// Gets or sets the reason for fallback if applicable.
    /// </summary>
    public string FallbackReason { get; set; } = string.Empty;
}

/// <summary>
/// Defines the source of script data.
/// </summary>
public enum ScriptSource
{
    /// <summary>
    /// Script generated by LLM.
    /// </summary>
    LLMGenerated = 0,

    /// <summary>
    /// Fallback script.
    /// </summary>
    Fallback = 1,

    /// <summary>
    /// Fallback dataset script.
    /// </summary>
    FallbackDataset = 2
}

public enum DataOrigin
{
    RealGameplaySession = 0,
    LLMGenerated = 1
}

public class NobodyWhoClient
{
    public bool IsEnabled { get; set; } = true;
    public Action? OnCall { get; set; }

    public void SimulateFailure()
    {
        IsEnabled = false;
    }
}

public class NarrativeCompiler
{
    private readonly NobodyWhoClient llmService;

    public NarrativeCompiler(NobodyWhoClient llmService)
    {
        this.llmService = llmService;
    }

    /// <summary>
    /// Compiles script data for the specified scene.
    /// </summary>
    /// <param name="_">The name of the scene to compile (unused).</param>
    /// <returns>The compiled script data.</returns>
    public ScriptData Compile(string _)
    {
        if (!llmService.IsEnabled)
        {
            return new ScriptData
            {
                Source = ScriptSource.FallbackDataset,
                Content = "Fallback content from previous session",
                Origin = DataOrigin.RealGameplaySession,
                FallbackReason = "LLM unavailable"
            };
        }

        llmService.OnCall?.Invoke();

        return new ScriptData
        {
            Source = ScriptSource.LLMGenerated,
            Content = "Generated content",
            Lines = new List<string> { "Line 1", "Line 2", "Line 3", "Line 4", "Line 5", "Line 6" },
            IsComplete = true
        };
    }
}

public class CallCounter
{
    public int Count { get; private set; }

    public void Increment()
    {
        Count++;
    }
}

public class OmegaSystem
{
    public bool DreamweaverProgramActive { get; set; }
    public int DreamweaverCount { get; set; }
    public int InteractionCount { get; set; }

    public void InitializeDreamweaverProgram()
    {
        DreamweaverProgramActive = true;
        DreamweaverCount = 3;
    }

    /// <summary>
    /// Processes a comment.
    /// </summary>
    /// <param name="_">The comment to process (unused).</param>
    /// <returns>The processed result.</returns>
    public object ProcessComment(string _)
    {
        InteractionCount++;
        return new object(); // Omega doesn't respond to dreamweaver comments
    }
}

public class OmegaEntity
{
    /// <summary>
    /// Gets or sets the type of the entity.
    /// </summary>
    public object Type { get; set; } = new object(); // Placeholder
    public EntityRole Role { get; set; } = EntityRole.ProgrammingNPC;
}

public enum EntityRole
{
    ProgrammingNPC = 0,
    Dreamweaver = 1,
    Player = 2
}

public enum DreamweaverType
{
    Hero = 0,
    Shadow = 1,
    Ambition = 2,
    Light = 3,
    Mischief = 4,
    Wrath = 5
}

public class DreamweaverManager
{
    public int DreamweaverCount => 3;

    public List<DreamweaverEntity> GetDreamweavers()
    {
        return new List<DreamweaverEntity>
        {
            new DreamweaverEntity { Type = DreamweaverType.Hero },
            new DreamweaverEntity { Type = DreamweaverType.Shadow },
            new DreamweaverEntity { Type = DreamweaverType.Ambition }
        };
    }
}

public class DreamweaverEntity
{
    public DreamweaverType Type { get; set; }
}

public class DreamweaverAffinity
{
    private readonly Dictionary<Archetype, int> scores = new Dictionary<Archetype, int>();
    private readonly List<Response> history = new List<Response>();

    public List<Response> History { get; } = new List<Response>();

    public void AddResponse(Response response)
    {
        if (!scores.ContainsKey(response.Archetype))
        {
            scores[response.Archetype] = 0;
        }
        scores[response.Archetype] += response.Points;
        history.Add(response);
    }

    public int GetScore(Archetype archetype)
    {
        return scores.ContainsKey(archetype) ? scores[archetype] : 0;
    }
}

public class Response
{
    public Archetype Archetype { get; set; }
    public int Points { get; set; } = 1;
}

public enum Archetype
{
    Hero = 0,
    Shadow = 1,
    Ambition = 2
}
