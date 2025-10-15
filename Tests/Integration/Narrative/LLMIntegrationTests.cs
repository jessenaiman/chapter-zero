using GdUnit4;
using Godot;
using static GdUnit4.Assertions;
using OmegaSpiral.Tests; // Import test fixtures

namespace OmegaSpiral.Tests.Integration.Narrative;

/// <summary>
/// Integration test suite for LLM-based narrative compilation and fallback mechanisms.
/// Tests the interaction between the NobodyWho LLM service, narrative compiler, and fallback datasets.
/// </summary>
[TestSuite]
public class LLMIntegrationTests
{
    /// <summary>
    /// Tests that local LLM successfully generates fresh narrative content when enabled.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void LocalLLM_ProvidesNewText()
    {
        // Test that when NobodyWho is enabled, local LLM generates fresh script content for scenes
        var llmService = new NobodyWhoClient();
        llmService.IsEnabled = true;

        var compiler = new NarrativeCompiler(llmService);
        var scene = compiler.Compile("Scene1");

        AssertThat(scene).IsNotNull();
        AssertThat(scene.Source).IsEqual(ScriptSource.LLMGenerated);
        AssertThat(scene.Content).IsNotNull();
        AssertThat(scene.Content).IsNotEmpty();
        AssertThat(scene.Timestamp).IsGreaterThanOrEqualTo(DateTime.Now.AddMinutes(-5)); // Recently generated
    }

    /// <summary>
    /// Tests that system falls back to real gameplay datasets when LLM is unavailable.
    /// </summary>
    [TestCase]
    public void Game_DefaultsToRealGameplayDatasets()
    {
        // Test that when LLM unavailable, game uses datasets from real previous gameplay sessions
        var llmService = new NobodyWhoClient();
        llmService.SimulateFailure(); // Make LLM unavailable

        var compiler = new NarrativeCompiler(llmService);
        var scene = compiler.Compile("Scene1");

        AssertThat(scene).IsNotNull();
        AssertThat(scene.Source).IsEqual(ScriptSource.FallbackDataset);
        AssertThat(scene.Origin).IsEqual(DataOrigin.RealGameplaySession);
        AssertThat(scene.FallbackReason).IsEqual("LLM unavailable");
    }

    /// <summary>
    /// Tests that entire script is compiled in single LLM call before scene starts, not line-by-line.
    /// </summary>
    [TestCase]
    public void Script_ProcessesInOnePass()
    {
        // Test that instead of line-by-line LLM calls, entire script is processed in single pass before scene starts
        var llmService = new NobodyWhoClient();
        var callCounter = new CallCounter();
        llmService.OnCall += () => callCounter.Increment();

        var compiler = new NarrativeCompiler(llmService);
        var scene = compiler.Compile("Scene1");

        // Verify only one call was made for the entire scene (not line by line)
        AssertThat(callCounter.Count).IsEqual(1);

        // Verify the response contained complete scene content, not partial responses
        AssertThat(scene.Lines.Count >= 5).IsTrue(); // Should have substantial content
        AssertThat(scene.IsComplete).IsTrue();
    }
}
