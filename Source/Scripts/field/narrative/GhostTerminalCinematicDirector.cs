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
        // Updated: These scenes do not exist, so references are commented out or replaced with TODOs.
        // private const string OpeningScenePath = "res://Source/Scenes/ui/ghost_terminal/OpeningLine.tscn";
        // private const string ThreadChoiceScenePath = "res://Source/Scenes/ui/ghost_terminal/ThreadChoice.tscn";
        // private const string StoryParagraphScenePath = "res://Source/Scenes/ui/ghost_terminal/StoryParagraph.tscn";
        // private const string StoryQuestionScenePath = "res://Source/Scenes/ui/ghost_terminal/StoryQuestion.tscn";
        // private const string StoryChoiceScenePath = "res://Source/Scenes/ui/ghost_terminal/StoryChoice.tscn";
        // private const string NamePromptScenePath = "res://Source/Scenes/ui/ghost_terminal/NamePrompt.tscn";
        // private const string SecretPromptScenePath = "res://Source/Scenes/ui/ghost_terminal/SecretPrompt.tscn";
        // private const string ExitScenePath = "res://Source/Scenes/ui/ghost_terminal/ExitLine.tscn";
        // TODO: Implement or provide fallback for missing GhostTerminal UI scenes.

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
                // Fallback: create beat with type and no scene path.
                var beat = new GhostTerminalBeat(GhostTerminalBeatType.OpeningLine, string.Empty);
                beat.Lines.Add(line);
                beats.Add(beat);
            }

            if (data.InitialChoice?.Options != null && data.InitialChoice.Options.Count > 0)
            {
                // Fallback: create thread choice beat with type and no scene path.
                var threadBeat = new GhostTerminalBeat(GhostTerminalBeatType.ThreadChoice, string.Empty)
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
                    // Fallback: create paragraph beat with type and no scene path.
                    var paragraphBeat = new GhostTerminalBeat(GhostTerminalBeatType.StoryParagraph, string.Empty);
                    paragraphBeat.Lines.Add(paragraph);
                    beats.Add(paragraphBeat);
                }

                if (!string.IsNullOrWhiteSpace(block.Question))
                {
                    // Fallback: create question beat with type and no scene path.
                    var questionBeat = new GhostTerminalBeat(GhostTerminalBeatType.StoryQuestion, string.Empty);
                    questionBeat.Prompt = block.Question;
                    beats.Add(questionBeat);

                    if (block.Choices.Count > 0)
                    {
                        // Fallback: create story choice beat with type and no scene path.
                        var choiceBeat = new GhostTerminalBeat(GhostTerminalBeatType.StoryChoice, string.Empty);
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

            // Fallback: create name prompt beat with type and no scene path.
            var nameBeat = new GhostTerminalBeat(GhostTerminalBeatType.NamePrompt, string.Empty)
            {
                Prompt = namePrompt,
            };
            beats.Add(nameBeat);

            if (data.SecretQuestion != null)
            {
                // Fallback: create secret prompt beat with type and no scene path.
                var secretBeat = new GhostTerminalBeat(GhostTerminalBeatType.SecretPrompt, string.Empty)
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

            // Fallback: create exit beat with type and no scene path.
            var exitBeat = new GhostTerminalBeat(GhostTerminalBeatType.ExitLine, string.Empty);
            exitBeat.Lines.Add(exitLine);
            beats.Add(exitBeat);

            return new GhostTerminalCinematicPlan(beats);
        }
    }
}
