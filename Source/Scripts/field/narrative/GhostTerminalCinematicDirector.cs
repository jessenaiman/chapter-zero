using System.Collections.Generic;
using System.Globalization;

namespace OmegaSpiral.Source.Scripts.Field.Narrative
{
    /// <summary>
    /// Translates <see cref="NarrativeSceneData"/> into a cinematic plan that Scene1Narrative can realise with Godot scenes.
    /// </summary>
    public static class GhostTerminalCinematicDirector
    {
        private const string ThreadChoiceScenePath = "res://Source/Scenes/ui/ghost_terminal/ThreadChoice.tscn";

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

            beats.AddRange(BuildOpeningLinesBeats(data.OpeningLines));
            beats.AddRange(BuildInitialChoiceBeats(data.InitialChoice));
            beats.AddRange(BuildStoryBlocksBeats(data.StoryBlocks, data.NamePrompt, data.SecretQuestion, data.ExitLine));

            return new GhostTerminalCinematicPlan(beats);
        }

        private static IEnumerable<GhostTerminalBeat> BuildOpeningLinesBeats(IEnumerable<string> openingLines)
        {
            foreach (string line in openingLines)
            {
                var beat = new GhostTerminalBeat(GhostTerminalBeatType.OpeningLine, string.Empty);
                beat.Lines.Add(line);
                yield return beat;
            }
        }

        private static IEnumerable<GhostTerminalBeat> BuildInitialChoiceBeats(NarrativeChoice? initialChoice)
        {
            if (initialChoice?.Options != null && initialChoice.Options.Count > 0)
            {
                var threadBeat = new GhostTerminalBeat(GhostTerminalBeatType.ThreadChoice, ThreadChoiceScenePath)
                {
                    Prompt = initialChoice.Prompt ?? string.Empty,
                };

                foreach (DreamweaverChoice option in initialChoice.Options)
                {
                    threadBeat.Options.Add(new GhostTerminalOption
                    {
                        Id = option.Id ?? string.Empty,
                        Label = option.Text ?? option.Id ?? string.Empty,
                        Description = option.Description ?? string.Empty,
                    });
                }

                yield return threadBeat;
            }
        }

        private static IEnumerable<GhostTerminalBeat> BuildStoryBlocksBeats(List<StoryBlock> storyBlocks, string? namePrompt, SecretQuestion? secretQuestion, string? exitLine)
        {
            for (int blockIndex = 0; blockIndex < storyBlocks.Count; blockIndex++)
            {
                StoryBlock block = storyBlocks[blockIndex];

                foreach (var beat in BuildParagraphBeats(block.Paragraphs))
                {
                    yield return beat;
                }

                foreach (var beat in BuildQuestionBeats(block, blockIndex))
                {
                    yield return beat;
                }
            }

            if (!string.IsNullOrWhiteSpace(namePrompt))
            {
                var nameBeat = new GhostTerminalBeat(GhostTerminalBeatType.NamePrompt, string.Empty)
                {
                    Prompt = namePrompt!,
                };
                yield return nameBeat;
            }

            foreach (var beat in BuildSecretQuestionBeats(secretQuestion))
            {
                yield return beat;
            }

            if (!string.IsNullOrWhiteSpace(exitLine))
            {
                var exitBeat = new GhostTerminalBeat(GhostTerminalBeatType.ExitLine, string.Empty);
                exitBeat.Lines.Add(exitLine!);
                yield return exitBeat;
            }
        }

        private static IEnumerable<GhostTerminalBeat> BuildParagraphBeats(IEnumerable<string> paragraphs)
        {
            foreach (string paragraph in paragraphs)
            {
                var paragraphBeat = new GhostTerminalBeat(GhostTerminalBeatType.StoryParagraph, string.Empty);
                paragraphBeat.Lines.Add(paragraph);
                yield return paragraphBeat;
            }
        }

        private static IEnumerable<GhostTerminalBeat> BuildQuestionBeats(StoryBlock block, int blockIndex)
        {
            if (!string.IsNullOrWhiteSpace(block.Question))
            {
                var questionBeat = new GhostTerminalBeat(GhostTerminalBeatType.StoryQuestion, string.Empty);
                questionBeat.Prompt = block.Question;
                yield return questionBeat;

                if (block.Choices.Count > 0)
                {
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

                    yield return choiceBeat;
                }
            }
        }

        private static IEnumerable<GhostTerminalBeat> BuildSecretQuestionBeats(SecretQuestion? secretQuestion)
        {
            if (secretQuestion?.Options != null && secretQuestion.Options.Count > 0)
            {
                var secretBeat = new GhostTerminalBeat(GhostTerminalBeatType.SecretPrompt, string.Empty)
                {
                    Prompt = secretQuestion.Prompt ?? string.Empty,
                };

                for (int i = 0; i < secretQuestion.Options.Count; i++)
                {
                    string optionText = secretQuestion.Options[i];
                    secretBeat.Options.Add(new GhostTerminalOption
                    {
                        Id = (i + 1).ToString(CultureInfo.InvariantCulture),
                        Label = optionText,
                        Description = string.Empty,
                    });
                }

                yield return secretBeat;
            }
        }
    }
}
