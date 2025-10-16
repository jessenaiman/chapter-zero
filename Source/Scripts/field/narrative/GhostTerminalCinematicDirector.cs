namespace OmegaSpiral.Source.Scripts.Field.Narrative
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Translates <see cref="NarrativeSceneData"/> into a cinematic plan that Scene1Narrative can realise with Godot scenes.
    /// </summary>
    public static class GhostTerminalCinematicDirector
    {
        private const string OpeningScenePath = "res://Source/Scenes/ui/ghost_terminal/OpeningLine.tscn";
        private const string ThreadChoiceScenePath = "res://Source/Scenes/ui/ghost_terminal/ThreadChoice.tscn";
        private const string StoryParagraphScenePath = "res://Source/Scenes/ui/ghost_terminal/StoryParagraph.tscn";
        private const string StoryQuestionScenePath = "res://Source/Scenes/ui/ghost_terminal/StoryQuestion.tscn";
        private const string StoryChoiceScenePath = "res://Source/Scenes/ui/ghost_terminal/StoryChoice.tscn";
        private const string NamePromptScenePath = "res://Source/Scenes/ui/ghost_terminal/NamePrompt.tscn";
        private const string SecretPromptScenePath = "res://Source/Scenes/ui/ghost_terminal/SecretPrompt.tscn";
        private const string ExitScenePath = "res://Source/Scenes/ui/ghost_terminal/ExitLine.tscn";

        /// <summary>
        /// Builds a cinematic plan describing the Ghost Terminal introduction.
        /// </summary>
        /// <param name="data">Narrative data sourced from JSON.</param>
        /// <returns>The assembled cinematic plan.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <see langword="null"/>.</exception>
        public static GhostTerminalCinematicPlan BuildPlan(NarrativeSceneData data)
        {
            ArgumentNullException.ThrowIfNull(data);

            var beats = new List<GhostTerminalBeat>();

            foreach (string line in data.OpeningLines)
            {
                var beat = new GhostTerminalBeat(GhostTerminalBeatType.OpeningLine, OpeningScenePath);
                beat.Lines.Add(line);
                beats.Add(beat);
            }

            if (data.InitialChoice?.Options != null && data.InitialChoice.Options.Count > 0)
            {
                var threadBeat = new GhostTerminalBeat(GhostTerminalBeatType.ThreadChoice, ThreadChoiceScenePath)
                {
                    Prompt = data.InitialChoice.Prompt ?? string.Empty,
                };

                foreach (DreamweaverChoice option in data.InitialChoice.Options)
                {
                    threadBeat.Options.Add(new GhostTerminalOption
                    {
                        Id = option.Id ?? string.Empty,
                        Label = option.Text ?? option.Id ?? string.Empty,
                        Description = option.Description ?? string.Empty,
                    });
                }

                beats.Add(threadBeat);
            }

            for (int blockIndex = 0; blockIndex < data.StoryBlocks.Count; blockIndex++)
            {
                StoryBlock block = data.StoryBlocks[blockIndex];

                foreach (string paragraph in block.Paragraphs)
                {
                    var paragraphBeat = new GhostTerminalBeat(GhostTerminalBeatType.StoryParagraph, StoryParagraphScenePath);
                    paragraphBeat.Lines.Add(paragraph);
                    beats.Add(paragraphBeat);
                }

                if (!string.IsNullOrWhiteSpace(block.Question))
                {
                    var questionBeat = new GhostTerminalBeat(GhostTerminalBeatType.StoryQuestion, StoryQuestionScenePath)
                    {
                        Prompt = block.Question,
                    };
                    beats.Add(questionBeat);

                    if (block.Choices.Count > 0)
                    {
                        var choiceBeat = new GhostTerminalBeat(GhostTerminalBeatType.StoryChoice, StoryChoiceScenePath);
                        for (int i = 0; i < block.Choices.Count; i++)
                        {
                            ChoiceOption choice = block.Choices[i];
                            string label = string.IsNullOrWhiteSpace(choice.Text)
                                ? $"Option {i + 1}"
                                : choice.Text!;

                            choiceBeat.Options.Add(new GhostTerminalOption
                            {
                                Id = string.IsNullOrWhiteSpace(choice.Id)
                                    ? $"block_{blockIndex}_choice_{i}"
                                    : choice.Id!,
                                Label = label,
                                Description = choice.Description ?? string.Empty,
                            });
                        }

                        beats.Add(choiceBeat);
                    }
                }
            }

            var namePrompt = string.IsNullOrWhiteSpace(data.NamePrompt)
                ? "What name should the terminal record?"
                : data.NamePrompt;

            var nameBeat = new GhostTerminalBeat(GhostTerminalBeatType.NamePrompt, NamePromptScenePath)
            {
                Prompt = namePrompt,
            };
            beats.Add(nameBeat);

            if (data.SecretQuestion != null)
            {
                var secretBeat = new GhostTerminalBeat(GhostTerminalBeatType.SecretPrompt, SecretPromptScenePath)
                {
                    Prompt = data.SecretQuestion.Prompt ?? "Can you face what hides in the dark?",
                };

                for (int i = 0; i < data.SecretQuestion.Options.Count; i++)
                {
                    string optionText = data.SecretQuestion.Options[i];
                    secretBeat.Options.Add(new GhostTerminalOption
                    {
                        Id = (i + 1).ToString(CultureInfo.InvariantCulture),
                        Label = optionText,
                        Description = string.Empty,
                    });
                }

                beats.Add(secretBeat);
            }

            var exitLine = string.IsNullOrWhiteSpace(data.ExitLine)
                ? "Moving to the next part of your journey..."
                : data.ExitLine;

            var exitBeat = new GhostTerminalBeat(GhostTerminalBeatType.ExitLine, ExitScenePath);
            exitBeat.Lines.Add(exitLine);
            beats.Add(exitBeat);

            return new GhostTerminalCinematicPlan(beats);
        }
    }
}
