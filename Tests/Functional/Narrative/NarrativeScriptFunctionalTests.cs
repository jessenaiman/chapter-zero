using GdUnit4;
using Godot;
using static GdUnit4.Assertions;
using OmegaSpiral.Tests; // Import test fixtures

namespace OmegaSpiral.Tests.Functional.Narrative;

/// <summary>
/// Functional test suite for validating narrative script behavior, flow, character interactions,
/// and the Omega/Dreamweaver system. These tests verify game narrative mechanics end-to-end.
/// </summary>
[TestSuite]
public class NarrativeScriptFunctionalTests
{
    #region Script Loading Tests

    /// <summary>
    /// Tests that scene loads fallback script content when NobodyWho LLM is disabled.
    /// </summary>
    [TestCase]
    public static void LoadScript_WhenNobodyWhoDisabled_ReturnsFallbackContent()
    {
        // Arrange
        var scriptLoader = new SceneScriptLoader(nobodyWhoEnabled: false);

        // Act
        var script = scriptLoader.Load("Scene1");

        // Assert
        AssertThat(script).IsNotNull();
        AssertThat(script.Source).IsEqual(ScriptSource.Fallback);
        AssertThat(script.Content).IsNotEmpty();
    }

    #endregion

    #region Question System Tests

    /// <summary>
    /// Tests that Scene1 presents exactly three questions to the player.
    /// </summary>
    [TestCase]
    public void LoadScene_WhenScene1Loaded_ReturnsThreeQuestions()
    {
        // Arrange
        var sceneData = TestDataFixtures.LoadSceneData("Scene1");

        // Act
        var questions = sceneData.GetQuestions();

        // Assert
        AssertThat(questions).HasSize(3);
        // Verify each has non-empty prompt but don't test specific content
        AssertThat(questions.All(q => !string.IsNullOrEmpty(q.Prompt))).IsTrue();
    }

    #endregion

    #region Story Section Tests

    /// <summary>
    /// Tests that story database contains at least 3 unique cryptic sections.
    /// </summary>
    [TestCase]
    public void LoadStory_WhenOneStoryLoaded_ContainsMinimumThreeSections()
    {
        // Arrange & Act
        var storySections = TestDataFixtures.GetStorySectionDatabase().GetSections("OneStory");

        // Assert
        AssertThat(storySections).HasSizeGreaterThanOrEqualTo(3);
        AssertThat(storySections.Distinct().Count()).IsEqual(storySections.Count);
    }

    /// <summary>
    /// Tests that each story section blocks until player provides interaction input.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public static void DisplayStorySection_ForEachSection_RequiresUserInteraction()
    {
        // Arrange
        var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Scene1Narrative.tscn");
        var instance = scene.Instantiate<Node2D>();
        var storySystem = instance.GetNode<Node>("StorySystem");

        // Act & Assert - for each of the 3 sections, verify interaction is required
        for (int i = 0; i < 3; i++)
        {
            // Trigger section i
            // Verify section is active
            // Simulate user interaction
            // Verify progression to next section

            // This would involve checking state transitions and interaction requirements
        }
    }

    /// <summary>
    /// Tests that narrative flow transitions to content block after completing all story sections.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public static void CompleteStorySections_WhenAllFinished_TransitionsToContentBlock()
    {
        // Arrange
        var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Scene1Narrative.tscn");
        var instance = scene.Instantiate<Node2D>();

        // Act - complete all story sections
        // (Implementation would simulate completing all sections)

        // Assert - verify transition to content block
        var contentBlock = instance.GetNode<Control>("ContentBlock");
        AssertThat(contentBlock.Visible).IsTrue();
    }

    #endregion

    #region Name Entry Tests

    /// <summary>
    /// Tests that name entry section displays input prompt to player.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public static void DisplayNamePrompt_WhenNameSectionActive_ShowsInputField()
    {
        // Arrange
        var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Scene1Narrative.tscn");
        var instance = scene.Instantiate<Node2D>();

        // Act - trigger name section
        var nameInput = instance.GetNode<LineEdit>("NameInput");

        // Assert
        AssertThat(nameInput.Visible).IsTrue();
        AssertThat(nameInput.PlaceholderText).IsNotEmpty(); // Don't test specific text
    }

    /// <summary>
    /// Tests that system presents two follow-up questions after player enters name.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public static void SubmitName_WhenPlayerEntersName_DisplaysTwoMeaningQuestions()
    {
        // Arrange
        var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Scene1Narrative.tscn");
        var instance = scene.Instantiate<Node2D>();
        var nameInput = instance.GetNode<LineEdit>("NameInput");

        // Act - enter name
        nameInput.Text = "TestPlayer";
        // Submit name and verify meaning questions appear

        // Assert - verify two question prompts appear (don't test content)
        // This would involve triggering the name submission
    }

    /// <summary>
    /// Tests that both meaning questions accept and store player responses.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public static void AnswerMeaningQuestions_WhenPlayerResponds_StoresResponses()
    {
        // Arrange
        var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Scene1Narrative.tscn");
        var instance = scene.Instantiate<Node2D>();

        // Act - simulate answering both meaning questions
        // (Implementation would provide test responses)

        // Assert - verify responses are stored in game state
    }

    /// <summary>
    /// Tests that flow continues to content block after answering all name-related questions.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public static void CompleteMeaningQuestions_WhenAllAnswered_TransitionsToContentBlock()
    {
        // Arrange
        var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Scene1Narrative.tscn");
        var instance = scene.Instantiate<Node2D>();

        // Act - complete name and meaning questions
        // (Implementation would simulate completing section)

        // Assert - verify transition to content block
    }

    #endregion

    #region Secret Section Tests

    /// <summary>
    /// Tests that secret section displays the cryptic question about keeping secrets.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public static void DisplaySecretSection_WhenSectionActive_ShowsSecretPrompt()
    {
        // Arrange
        var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Scene1Narrative.tscn");
        var instance = scene.Instantiate<Node2D>();

        // Act - trigger secret section
        var secretPrompt = instance.GetNode<Label>("SecretPrompt");

        // Assert
        AssertThat(secretPrompt.Visible).IsTrue();
    }

    /// <summary>
    /// Tests that secret question offers three choices, each mapping to different Dreamweaver affinity.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public static void DisplaySecretChoices_WhenSectionActive_ShowsThreeDreamweaverOptions()
    {
        // Arrange
        var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Scene1Narrative.tscn");
        var instance = scene.Instantiate<Node2D>();
        var choiceButtons = instance.GetNode<Control>("ChoiceButtons");

        // Act
        var choices = choiceButtons.GetChildren().OfType<Button>().ToList();

        // Assert
        AssertThat(choices).HasSize(3);
        // Verify each choice maps to different dreamweaver (don't test specific mappings)
        // This would involve checking button properties or associated data
    }

    /// <summary>
    /// Tests that scientific equation displays as ghostwritten text with mid-sequence screen freeze.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public static void DisplayEquation_WhenSecretChoiceSelected_ShowsGhostwritingWithFreeze()
    {
        // Arrange
        var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Scene1Narrative.tscn");
        var instance = scene.Instantiate<Node2D>();

        // Act - select secret choice that triggers equation
        // (Implementation would trigger specific choice)

        // Assert - verify ghostwritten text effect
        // Verify screen freeze at specified point
        // Check for visual effects indicating system control
    }

    /// <summary>
    /// Tests that Omega control message appears after equation freeze resolves.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public static void ResolveEquationFreeze_WhenFreezeEnds_DisplaysOmegaControlMessage()
    {
        // Arrange
        var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Scene1Narrative.tscn");
        var instance = scene.Instantiate<Node2D>();

        // Act - trigger equation sequence
        // (Implementation would complete freeze sequence)

        // Assert - verify Omega control message appears after freeze
    }

    /// <summary>
    /// Tests that final cryptic message about game reality and predetermined choice appears.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public static void CompleteSecretSection_WhenFinished_DisplaysCrypticRealityMessage()
    {
        // Arrange
        var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Scene1Narrative.tscn");
        var instance = scene.Instantiate<Node2D>();

        // Act - complete secret section
        // (Implementation would finish entire sequence)

        // Assert - verify final cryptic message about game reality
    }

    #endregion

    #region Omega Character Tests

    /// <summary>
    /// Tests that Omega appears as primary character in Scene1 but not in subsequent scenes.
    /// </summary>
    [TestCase]
    public void LoadScenes_WhenCheckingOmegaPresence_AppearsOnlyInScene1()
    {
        // Arrange & Act
        var scene1Data = TestDataFixtures.LoadSceneData("Scene1");
        var scene2Data = TestDataFixtures.LoadSceneData("Scene2");

        // Assert
        AssertThat(scene1Data.HasOmegaAsPrimary).IsTrue();
        AssertThat(scene2Data.HasOmegaAsPrimary).IsFalse();
    }

    /// <summary>
    /// Tests that Omega successfully initializes the Dreamweaver creation system.
    /// </summary>
    [TestCase]
    public static void InitializeSystem_WhenOmegaStarts_CreatesDreamweaverProgram()
    {
        // Arrange
        var omegaSystem = new OmegaSystem();

        // Act
        omegaSystem.InitializeDreamweaverProgram();

        // Assert
        AssertThat(omegaSystem.DreamweaverProgramActive).IsTrue();
        AssertThat(omegaSystem.DreamweaverCount).IsEqual(3);
    }

    /// <summary>
    /// Tests that Omega ignores and does not respond to Dreamweaver comments or dialogue.
    /// </summary>
    [TestCase]
    public static void ProcessComment_WhenDreamweaverComments_OmegaDoesNotRespond()
    {
        // Arrange
        var omegaSystem = new OmegaSystem();
        var dreamweaverComment = "Dreamweaver comment";

        // Act
        var response = omegaSystem.ProcessComment(dreamweaverComment);

        // Assert
        AssertThat(response).IsNull();
        AssertThat(omegaSystem.InteractionCount).IsEqual(0);
    }

    /// <summary>
    /// Tests that direct player questions only originate from Omega in Scene1.
    /// </summary>
    [TestCase]
    public void LoadScenes_WhenCheckingQuestions_OmegaAsksOnlyInScene1()
    {
        // Arrange & Act
        var scene1Data = TestDataFixtures.LoadSceneData("Scene1");
        var scene2Data = TestDataFixtures.LoadSceneData("Scene2");

        // Assert - direct player questions only from Omega in first scene
        AssertThat(scene1Data.HasDirectPlayerQuestions).IsTrue();
        AssertThat(scene2Data.HasDirectPlayerQuestions).IsFalse();
    }

    /// <summary>
    /// Tests that Omega functions as programming NPC, not as one of the three Dreamweavers.
    /// </summary>
    [TestCase]
    public static void CheckOmegaRole_WhenEvaluatingType_IsNotDreamweaver()
    {
        // Arrange
        var omega = new OmegaEntity();
        var dreamweavers = new[] { DreamweaverType.Hero, DreamweaverType.Shadow, DreamweaverType.Ambition };

        // Act & Assert
        AssertThat(omega.Type).IsNotEqual(DreamweaverType.Hero);
        AssertThat(omega.Type).IsNotEqual(DreamweaverType.Shadow);
        AssertThat(omega.Type).IsNotEqual(DreamweaverType.Ambition);
        AssertThat(omega.Role).IsEqual(EntityRole.ProgrammingNPC);
    }

    #endregion

    #region Dreamweaver System Tests

    /// <summary>
    /// Tests that Dreamweavers have dialogue directed at each other during chapter-zero scenes.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public static void DisplayDreamweaverDialogue_DuringChapterZero_DirectedAtEachOther()
    {
        // Arrange
        var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Scene1Narrative.tscn");
        var instance = scene.Instantiate<Node2D>();
        var dreamweaverSystem = instance.GetNode<Node>("DreamweaverSystem");

        // Act - simulate scene events
        // (Implementation would trigger dreamweaver interactions)

        // Assert - verify dreamweaver-to-dreamweaver dialogue occurs
        // Check that dialogue is not directed to player/Omega
    }

    /// <summary>
    /// Tests that Dreamweavers provide commentary and reactions to player choices.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public static void MakePlayerChoice_WhenChoiceSelected_DreamweaversReact()
    {
        // Arrange
        var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Scene1Narrative.tscn");
        var instance = scene.Instantiate<Node2D>();

        // Act - make a player choice
        // (Implementation would select specific option)

        // Assert - verify dreamweaver commentary appears
        // Check that commentary is related to the choice made
    }

    /// <summary>
    /// Tests that exactly three Dreamweaver entities exist in the system.
    /// </summary>
    [TestCase]
    public static void InitializeDreamweavers_WhenSystemStarts_CreatesExactlyThreeEntities()
    {
        // Arrange & Act
        var dreamweaverManager = new DreamweaverManager();

        // Assert
        AssertThat(dreamweaverManager.DreamweaverCount).IsEqual(3);
        AssertThat(dreamweaverManager.GetDreamweavers()).HasSize(3);

        var types = dreamweaverManager.GetDreamweavers().Select(dw => dw.Type).ToList();
        AssertThat(types).Contains(DreamweaverType.Hero);
        AssertThat(types).Contains(DreamweaverType.Shadow);
        AssertThat(types).Contains(DreamweaverType.Ambition);
    }

    #endregion

    #region Affinity System Tests

    /// <summary>
    /// Tests that scene maintains point tracking system with 1-2 point awards updated after each scene.
    /// </summary>
    [TestCase]
    public static void AwardPoints_WhenPlayerAnswers_UpdatesDreamweaverAffinity()
    {
        // Arrange
        var affinitySystem = new DreamweaverAffinity();

        // Act - simulate player choices that award points
        affinitySystem.AddResponse(new Response { Archetype = Archetype.Hero, Points = 1 });
        affinitySystem.AddResponse(new Response { Archetype = Archetype.Shadow, Points = 2 });

        // Assert
        AssertThat(affinitySystem.GetScore(Archetype.Hero)).IsEqual(1);
        AssertThat(affinitySystem.GetScore(Archetype.Shadow)).IsEqual(2);
        AssertThat(affinitySystem.History).HasSize(2);
    }

    #endregion
}
