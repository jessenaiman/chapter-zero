using System;
using System.Collections.Generic;
using System.Linq;
using OmegaSpiral.Source.Scripts.Common; // Import main project types

namespace OmegaSpiral.Tests;

/// <summary>
/// Provides test data fixtures for scenes and story sections used in unit tests.
/// </summary>
public static class NarrativeTestDataFixtures
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
            Source = nobodyWhoEnabled ? TestScriptSource.Fallback : TestScriptSource.LLMGenerated,
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
    public TestScriptSource Source { get; set; }

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
    public TestDataOrigin Origin { get; set; } = TestDataOrigin.RealGameplaySession;

    /// <summary>
    /// Gets or sets the reason for fallback if applicable.
    /// </summary>
    public string FallbackReason { get; set; } = string.Empty;
}

/// <summary>
/// Defines the source of script data.
/// </summary>
public enum TestScriptSource
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

/// <summary>
/// Represents the origin of test data.
/// </summary>
public enum TestDataOrigin
{
    /// <summary>
    /// Data originated from a real gameplay session.
    /// </summary>
    RealGameplaySession = 0,

    /// <summary>
    /// Data was generated by an LLM.
    /// </summary>
    LLMGenerated = 1
}

/// <summary>
/// Mock client for NobodyWho LLM service.
/// </summary>
public class NobodyWhoClient
{
    /// <summary>
    /// Gets or sets a value indicating whether the client is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the action to invoke when the client is called.
    /// </summary>
    public Action? OnCall { get; set; }

    /// <summary>
    /// Simulates a failure by disabling the client.
    /// </summary>
    public void SimulateFailure()
    {
        IsEnabled = false;
    }
}

/// <summary>
/// Compiles narrative content using LLM services.
/// </summary>
public class NarrativeCompiler
{
    private readonly NobodyWhoClient llmService;

    /// <summary>
    /// Initializes a new instance of the <see cref="NarrativeCompiler"/> class.
    /// </summary>
    /// <param name="llmService">The LLM service client to use for compilation.</param>
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
                Source = TestScriptSource.FallbackDataset,
                Content = "Fallback content from previous session",
                Origin = TestDataOrigin.RealGameplaySession,
                FallbackReason = "LLM unavailable"
            };
        }

        llmService.OnCall?.Invoke();

        return new ScriptData
        {
            Source = TestScriptSource.LLMGenerated,
            Content = "Generated content",
            Lines = new List<string> { "Line 1", "Line 2", "Line 3", "Line 4", "Line 5", "Line 6" },
            IsComplete = true
        };
    }
}

/// <summary>
/// Tracks the number of calls made.
/// </summary>
public class CallCounter
{
    /// <summary>
    /// Gets the current count of calls.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Increments the call count by one.
    /// </summary>
    public void Increment()
    {
        Count++;
    }
}

/// <summary>
/// Represents the Omega system state.
/// </summary>
public class TestOmegaSystem
{
    /// <summary>
    /// Gets or sets a value indicating whether the Dreamweaver program is active.
    /// </summary>
    public bool DreamweaverProgramActive { get; set; }

    /// <summary>
    /// Gets or sets the number of Dreamweavers.
    /// </summary>
    public int DreamweaverCount { get; set; }

    /// <summary>
    /// Gets or sets the number of interactions.
    /// </summary>
    public int InteractionCount { get; set; }

    /// <summary>
    /// Initializes the Dreamweaver program.
    /// </summary>
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

/// <summary>
/// Represents an entity in the Omega system.
/// </summary>
public class OmegaEntity
{
    /// <summary>
    /// Gets or sets the type of the entity.
    /// </summary>
    public object Type { get; set; } = new object(); // Placeholder

    /// <summary>
    /// Gets or sets the role of the entity.
    /// </summary>
    public TestEntityRole Role { get; set; } = TestEntityRole.ProgrammingNPC;
}

/// <summary>
/// Defines the possible roles for entities.
/// </summary>
public enum TestEntityRole
{
    /// <summary>
    /// A programming NPC entity.
    /// </summary>
    ProgrammingNPC = 0,

    /// <summary>
    /// A Dreamweaver entity.
    /// </summary>
    Dreamweaver = 1,

    /// <summary>
    /// A player entity.
    /// </summary>
    Player = 2
}

/// <summary>
/// Manages Dreamweaver entities.
/// </summary>
public class TestDreamweaverManager
{
    /// <summary>
    /// Gets the number of Dreamweavers.
    /// </summary>
    public int DreamweaverCount => 3;

    /// <summary>
    /// Gets the list of Dreamweavers.
    /// </summary>
    /// <returns>A list of Dreamweaver entities.</returns>
    public List<TestDreamweaverEntity> GetDreamweavers()
    {
        return new List<TestDreamweaverEntity>
        {
            new TestDreamweaverEntity { Type = (DreamweaverType)DreamweaverThread.Hero },
            new TestDreamweaverEntity { Type = (DreamweaverType)DreamweaverThread.Shadow },
            new TestDreamweaverEntity { Type = (DreamweaverType)DreamweaverThread.Ambition }
        };
    }
}

/// <summary>
/// Represents a Dreamweaver entity.
/// </summary>
public class TestDreamweaverEntity
{
    /// <summary>
    /// Gets or sets the type of the Dreamweaver.
    /// </summary>
    public DreamweaverType Type { get; set; }
}

/// <summary>
/// Manages Dreamweaver affinity scores.
/// </summary>
public class TestDreamweaverAffinity
{
    private readonly Dictionary<TestAffinityArchetype, int> scores = new Dictionary<TestAffinityArchetype, int>();
    private readonly List<TestAffinityResponse> history = new List<TestAffinityResponse>();

    /// <summary>
    /// Gets the history of responses.
    /// </summary>
    public List<TestAffinityResponse> History { get; } = new List<TestAffinityResponse>();

    /// <summary>
    /// Adds a response to the history.
    /// </summary>
    /// <param name="response">The response to add.</param>
    public void AddResponse(TestAffinityResponse response)
    {
        if (!scores.ContainsKey(response.Archetype))
        {
            scores[response.Archetype] = 0;
        }
        scores[response.Archetype] += response.Points;
        history.Add(response);
    }

    /// <summary>
    /// Gets the score for a specific archetype.
    /// </summary>
    /// <param name="archetype">The archetype to get the score for.</param>
    /// <returns>The score for the archetype.</returns>
    public int GetScore(TestAffinityArchetype archetype)
    {
        return scores.TryGetValue(archetype, out var score) ? score : 0;
    }
}

/// <summary>
/// Represents a response with an archetype and points.
/// </summary>
public class TestAffinityResponse
{
    /// <summary>
    /// Gets or sets the archetype of the response.
    /// </summary>
    public TestAffinityArchetype Archetype { get; set; }

    /// <summary>
    /// Gets or sets the points for the response.
    /// </summary>
    public int Points { get; set; } = 1;
}

/// <summary>
/// Defines the archetypes.
/// </summary>
public enum TestAffinityArchetype
{
    /// <summary>
    /// Hero archetype.
    /// </summary>
    Hero = 0,

    /// <summary>
    /// Shadow archetype.
    /// </summary>
    Shadow = 1,

    /// <summary>
    /// Ambition archetype.
    /// </summary>
    Ambition = 2
}
