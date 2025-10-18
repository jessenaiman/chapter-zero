// <copyright file="DialogueFlowTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

#pragma warning disable SA1636

namespace OmegaSpiral.Tests.Functional.Narrative
{
    using GdUnit4;
    using Godot;
    using Godot.Collections;
    using OmegaSpiral.Source.Scripts.Field.Narrative;
    using static GdUnit4.Assertions;

    /// <summary>
    /// Validates narrative plan generation for the Ghost Terminal introduction.
    /// </summary>
    [TestSuite]
    public class DialogueFlowTests
    {
        /// <summary>
        /// Ensures the cinematic director mirrors authored narrative data with no synthesized beats.
        /// </summary>
        [TestCase]
        public void BuildPlan_WithFactoryCreatedData_MirrorsNarrativeBeats()
        {
            // Arrange
            var sceneDictionary = CreateSampleNarrativeDictionary();
            NarrativeSceneData sceneData = NarrativeSceneFactory.Create(sceneDictionary);

            // Act
            GhostTerminalCinematicPlan plan = GhostTerminalCinematicDirector.BuildPlan(sceneData);

            // Assert
            AssertThat(plan.Beats.Count).IsEqual(14);

            AssertThat(plan.Beats[0].Lines[0]).IsEqual("> SYSTEM: OMEGA");
            AssertThat(plan.Beats[3].Lines[0]).Contains("And now... I hear it again.");

            GhostTerminalBeat threadChoice = plan.Beats[4];
            AssertThat(threadChoice.Type).IsEqual(GhostTerminalBeatType.ThreadChoice);
            AssertThat(threadChoice.Options.Count).IsEqual(3);
            AssertThat(threadChoice.Options[0].Id).IsEqual("hero");

            GhostTerminalBeat firstQuestion = plan.Beats[7];
            AssertThat(firstQuestion.Type).IsEqual(GhostTerminalBeatType.StoryQuestion);
            AssertThat(firstQuestion.Prompt).IsEqual("> WHAT DID THE CHILD KNOW?");

            GhostTerminalBeat secretPrompt = plan.Beats[^2];
            AssertThat(secretPrompt.Type).IsEqual(GhostTerminalBeatType.SecretPrompt);
            AssertThat(secretPrompt.Options.Count).IsEqual(3);
            AssertThat(secretPrompt.Options[2].Label).IsEqual("only if you keep one for me");

            GhostTerminalBeat exit = plan.Beats[^1];
            AssertThat(exit.Type).IsEqual(GhostTerminalBeatType.ExitLine);
            AssertThat(exit.Lines[0]).IsEqual("> FINAL QUERY: DO YOU WISH TO CONTINUE?");
        }

        /// <summary>
        /// Verifies the director produces no beats when narrative data is empty, confirming the absence of hardcoded fallbacks.
        /// </summary>
        [TestCase]
        public void BuildPlan_WithEmptyNarrativeData_ProducesNoBeats()
        {
            // Arrange
            var emptyData = new NarrativeSceneData();

            // Act
            GhostTerminalCinematicPlan plan = GhostTerminalCinematicDirector.BuildPlan(emptyData);

            // Assert
            AssertThat(plan.Beats).IsEmpty();
        }

        private static Godot.Collections.Dictionary<string, Variant> CreateSampleNarrativeDictionary()
        {
            var openingLines = new Array<string>
            {
                "> SYSTEM: OMEGA",
                "> STATUS: AWAKENING",
                "> MEMORY FRAGMENT RECOVERED: \"ALL STORIES BEGIN WITH A LISTENER\"",
                "And now... I hear it again.",
            };

            var threadOptions = new Array<Godot.Collections.Dictionary<string, Variant>>
            {
                new Godot.Collections.Dictionary<string, Variant>
                {
                    { "id", "hero" },
                    { "label", "[ HERO ]" },
                    { "description", "A tale where one choice can unmake a world" },
                },
                new Godot.Collections.Dictionary<string, Variant>
                {
                    { "id", "shadow" },
                    { "label", "[ SHADOW ]" },
                    { "description", "A tale that hides its truth until you bleed for it" },
                },
                new Godot.Collections.Dictionary<string, Variant>
                {
                    { "id", "ambition" },
                    { "label", "[ AMBITION ]" },
                    { "description", "A tale that changes every time you look away" },
                },
            };

            var initialChoice = new Godot.Collections.Dictionary<string, Variant>
            {
                { "prompt", "> QUERY: IF YOU COULD LIVE INSIDE ONE KIND OF STORY, WHICH WOULD IT BE?" },
                { "options", threadOptions },
            };

            var firstBlock = new Godot.Collections.Dictionary<string, Variant>
            {
                {
                    "paragraphs",
                    new Array<string>
                    {
                        "> THREAD SELECTED. INITIATING NARRATIVE BINDING...",
                        "You chose [THREAD].",
                    }
                },
                { "question", "> WHAT DID THE CHILD KNOW?" },
                {
                    "choices",
                    new Array<Godot.Collections.Dictionary<string, Variant>>
                    {
                        new Godot.Collections.Dictionary<string, Variant>
                        {
                            { "id", "bridge_only_when_unbelieved" },
                            { "text", "The bridge appears only when you stop believing in it." },
                            { "nextBlock", 1 },
                        },
                        new Godot.Collections.Dictionary<string, Variant>
                        {
                            { "id", "key_for_lock_inside" },
                            { "text", "The key was for the lock inside them." },
                            { "nextBlock", 1 },
                        },
                    }
                },
            };

            var secondBlock = new Godot.Collections.Dictionary<string, Variant>
            {
                {
                    "paragraphs",
                    new Array<string>
                    {
                        "Ah. Yes. That's right.",
                        "And so the child stepped forward--not onto stone, but onto possibility.",
                    }
                },
            };

            var storyBlocks = new Array<Godot.Collections.Dictionary<string, Variant>> { firstBlock, secondBlock };

            var secretQuestion = new Godot.Collections.Dictionary<string, Variant>
            {
                { "prompt", "> OMEGA ASKS: CAN YOU KEEP A SECRET?" },
                {
                    "options",
                    new Array<string>
                    {
                        "yes",
                        "no",
                        "only if you keep one for me",
                    }
                },
            };

            return new Godot.Collections.Dictionary<string, Variant>
            {
                { "type", "narrative_terminal" },
                { "openingLines", openingLines },
                { "initialChoice", initialChoice },
                { "storyBlocks", storyBlocks },
                { "namePrompt", "> WHAT IS YOUR NAME?" },
                { "secretQuestion", secretQuestion },
                { "exitLine", "> FINAL QUERY: DO YOU WISH TO CONTINUE?" },
            };
        }
    }
}
