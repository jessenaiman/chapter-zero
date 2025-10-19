using System;
using System.Collections.Generic;
using Godot;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.Field.Narrative
{
    /// <summary>
    /// Factory helpers for constructing <see cref="NarrativeSceneData"/> instances from Godot dictionaries.
    /// Centralizes JSON to domain mapping so both runtime code and tests share the same logic.
    /// </summary>
    public static class NarrativeSceneFactory
    {
        /// <summary>
        /// Creates a <see cref="NarrativeSceneData"/> from raw configuration data.
        /// </summary>
        /// <param name="data">The Godot dictionary parsed from JSON.</param>
        /// <returns>The populated <see cref="NarrativeSceneData"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
        public static NarrativeSceneData Create(Godot.Collections.Dictionary<string, Variant> data)
        {
            ArgumentNullException.ThrowIfNull(data);

            string sceneType = GetSceneType(data);

            var result = new NarrativeSceneData
            {
                Type = sceneType,
                OpeningLines = GetOpeningLines(data),
                InitialChoice = GetInitialChoice(data),
                StoryBlocks = GetStoryBlocks(data),
                NamePrompt = GetNamePrompt(data),
                SecretQuestion = GetSecretQuestion(data),
                ExitLine = GetExitLine(data)
            };

            if (string.Equals(sceneType, "narrative_terminal_cinematic", StringComparison.OrdinalIgnoreCase))
            {
                result.Cinematic = MapToCinematic(data);
            }

            return result;
        }

        private static string GetSceneType(Godot.Collections.Dictionary<string, Variant> data)
        {
            return data.TryGetValue("type", out var typeVar) && typeVar.VariantType == Variant.Type.String
                ? typeVar.AsString()
                : "narrative_terminal";
        }

        private static List<string> GetOpeningLines(Godot.Collections.Dictionary<string, Variant> data)
        {
            if (data.TryGetValue("openingLines", out var openingLinesVar) &&
                openingLinesVar.VariantType == Variant.Type.Array)
            {
                return ExtractStringList(openingLinesVar.AsGodotArray());
            }
            return new List<string>();
        }

        private static NarrativeChoice? GetInitialChoice(Godot.Collections.Dictionary<string, Variant> data)
        {
            if (data.TryGetValue("initialChoice", out var initialChoiceVar) &&
                initialChoiceVar.VariantType == Variant.Type.Dictionary)
            {
                return MapToInitialChoice(initialChoiceVar.AsGodotDictionary());
            }
            return null;
        }

        private static List<StoryBlock> GetStoryBlocks(Godot.Collections.Dictionary<string, Variant> data)
        {
            if (data.TryGetValue("storyBlocks", out var storyBlocksVar) &&
                storyBlocksVar.VariantType == Variant.Type.Array)
            {
                var storyBlocks = new List<StoryBlock>();
                foreach (var blockVar in storyBlocksVar.AsGodotArray())
                {
                    if (blockVar.VariantType == Variant.Type.Dictionary)
                    {
                        storyBlocks.Add(MapToStoryBlock(blockVar.AsGodotDictionary()));
                    }
                }
                return storyBlocks;
            }
            return new List<StoryBlock>();
        }

        private static string? GetNamePrompt(Godot.Collections.Dictionary<string, Variant> data)
        {
            if (data.TryGetValue("namePrompt", out var namePromptVar) &&
                namePromptVar.VariantType == Variant.Type.String)
            {
                return namePromptVar.AsString();
            }
            return null;
        }

        private static SecretQuestion? GetSecretQuestion(Godot.Collections.Dictionary<string, Variant> data)
        {
            if (data.TryGetValue("secretQuestion", out var secretVar) &&
                secretVar.VariantType == Variant.Type.Dictionary)
            {
                return MapToSecretQuestion(secretVar.AsGodotDictionary());
            }
            return null;
        }

        private static string? GetExitLine(Godot.Collections.Dictionary<string, Variant> data)
        {
            if (data.TryGetValue("exitLine", out var exitVar) &&
                exitVar.VariantType == Variant.Type.String)
            {
                return exitVar.AsString();
            }
            return null;
        }

        private static NarrativeChoice MapToInitialChoice(Godot.Collections.Dictionary dict)
        {
            var choice = new NarrativeChoice();

            if (dict.TryGetValue("prompt", out var promptVar) &&
                promptVar.VariantType == Variant.Type.String)
            {
                choice.Prompt = promptVar.AsString();
            }

            if (dict.TryGetValue("options", out var optionsVar) &&
                optionsVar.VariantType == Variant.Type.Array)
            {
                var options = new List<DreamweaverChoice>();
                foreach (var optionVar in optionsVar.AsGodotArray())
                {
                    if (optionVar.VariantType == Variant.Type.Dictionary)
                    {
                        options.Add(MapToDreamweaverChoice(optionVar.AsGodotDictionary()));
                    }
                }

                choice.Options = options;
            }

            return choice;
        }

        private static DreamweaverChoice MapToDreamweaverChoice(Godot.Collections.Dictionary dict)
        {
            var choice = new DreamweaverChoice();

            if (dict.TryGetValue("id", out var idVar) && idVar.VariantType == Variant.Type.String)
            {
                choice.Id = idVar.AsString();

                if (!string.IsNullOrWhiteSpace(choice.Id) &&
                    Enum.TryParse(choice.Id, true, out DreamweaverThread parsedThread))
                {
                    choice.Thread = parsedThread;
                }
            }

            if (dict.TryGetValue("label", out var labelVar) && labelVar.VariantType == Variant.Type.String)
            {
                string label = labelVar.AsString();
                choice.Text = label;
                choice.Label = label;
            }

            if (dict.TryGetValue("description", out var descriptionVar) &&
                descriptionVar.VariantType == Variant.Type.String)
            {
                choice.Description = descriptionVar.AsString();
            }

            return choice;
        }

        private static StoryBlock MapToStoryBlock(Godot.Collections.Dictionary dict)
        {
            var block = new StoryBlock();

            if (dict.TryGetValue("paragraphs", out var paragraphsVar) &&
                paragraphsVar.VariantType == Variant.Type.Array)
            {
                block.Paragraphs = ExtractStringList(paragraphsVar.AsGodotArray());
            }

            if (dict.TryGetValue("question", out var questionVar) &&
                questionVar.VariantType == Variant.Type.String)
            {
                block.Question = questionVar.AsString();
            }

            if (dict.TryGetValue("choices", out var choicesVar) &&
                choicesVar.VariantType == Variant.Type.Array)
            {
                var choices = new List<ChoiceOption>();
                foreach (var choiceVar in choicesVar.AsGodotArray())
                {
                    if (choiceVar.VariantType == Variant.Type.Dictionary)
                    {
                        choices.Add(MapToChoiceOption(choiceVar.AsGodotDictionary()));
                    }
                }

                block.Choices = choices;
            }

            return block;
        }

        private static ChoiceOption MapToChoiceOption(Godot.Collections.Dictionary dict)
        {
            var option = new ChoiceOption();

            if (dict.TryGetValue("text", out var textVar) &&
                textVar.VariantType == Variant.Type.String)
            {
                string text = textVar.AsString();
                option.Text = text;
                option.Label = text;
            }

            if (dict.TryGetValue("nextBlock", out var nextVar) &&
                nextVar.VariantType == Variant.Type.Int)
            {
                option.NextBlock = nextVar.AsInt32();
            }

            if (dict.TryGetValue("description", out var descriptionVar) &&
                descriptionVar.VariantType == Variant.Type.String)
            {
                option.Description = descriptionVar.AsString();
            }

            if (dict.TryGetValue("id", out var idVar) && idVar.VariantType == Variant.Type.String)
            {
                option.Id = idVar.AsString();
            }

            return option;
        }

        private static SecretQuestion MapToSecretQuestion(Godot.Collections.Dictionary dict)
        {
            var question = new SecretQuestion();

            if (dict.TryGetValue("prompt", out var promptVar) &&
                promptVar.VariantType == Variant.Type.String)
            {
                question.Prompt = promptVar.AsString();
            }

            if (dict.TryGetValue("options", out var optionsVar) &&
                optionsVar.VariantType == Variant.Type.Array)
            {
                question.Options.AddRange(ExtractStringList(optionsVar.AsGodotArray()));
            }

            return question;
        }

        private static GhostTerminalCinematicData MapToCinematic(Godot.Collections.Dictionary<string, Variant> data)
        {
            var cinematic = new GhostTerminalCinematicData();

            if (data.TryGetValue("metadata", out var metadataVar) &&
                metadataVar.VariantType == Variant.Type.Dictionary)
            {
                cinematic.Metadata = MapToMetadata(metadataVar.AsGodotDictionary());
            }

            if (data.TryGetValue("bootSequence", out var bootVar) &&
                bootVar.VariantType == Variant.Type.Dictionary)
            {
                cinematic.BootSequence = MapToBootSequence(bootVar.AsGodotDictionary());
            }

            if (data.TryGetValue("openingMonologue", out var monologueVar) &&
                monologueVar.VariantType == Variant.Type.Dictionary)
            {
                cinematic.OpeningMonologue = MapToMonologue(monologueVar.AsGodotDictionary());
            }

            if (data.TryGetValue("firstChoice", out var firstChoiceVar) &&
                firstChoiceVar.VariantType == Variant.Type.Dictionary)
            {
                cinematic.FirstChoice = MapToChoiceBlock(firstChoiceVar.AsGodotDictionary());
            }

            if (data.TryGetValue("storyFragment", out var storyFragmentVar) &&
                storyFragmentVar.VariantType == Variant.Type.Dictionary)
            {
                cinematic.StoryFragment = MapToStoryFragment(storyFragmentVar.AsGodotDictionary());
            }

            if (data.TryGetValue("secretQuestion", out var secretVar) &&
                secretVar.VariantType == Variant.Type.Dictionary)
            {
                cinematic.SecretQuestion = MapToSecretQuestionBlock(secretVar.AsGodotDictionary());
            }

            if (data.TryGetValue("nameQuestion", out var nameQuestionVar) &&
                nameQuestionVar.VariantType == Variant.Type.Dictionary)
            {
                cinematic.NameQuestion = MapToNameQuestion(nameQuestionVar.AsGodotDictionary());
            }

            if (data.TryGetValue("exit", out var exitVar) &&
                exitVar.VariantType == Variant.Type.Dictionary)
            {
                cinematic.Exit = MapToExit(exitVar.AsGodotDictionary());
            }

            return cinematic;
        }

        private static GhostTerminalMetadata MapToMetadata(Godot.Collections.Dictionary dict)
        {
            var metadata = new GhostTerminalMetadata();

            if (dict.TryGetValue("iteration", out var iterationVar) &&
                iterationVar.VariantType == Variant.Type.String)
            {
                metadata.Iteration = iterationVar.AsString();
            }

            if (dict.TryGetValue("iterationFallback", out var fallbackVar) &&
                fallbackVar.VariantType == Variant.Type.Int)
            {
                metadata.IterationFallback = fallbackVar.AsInt32();
            }

            if (dict.TryGetValue("previousAttempt", out var previousVar) &&
                previousVar.VariantType == Variant.Type.String)
            {
                metadata.PreviousAttempt = previousVar.AsString();
            }

            if (dict.TryGetValue("interface", out var interfaceVar) &&
                interfaceVar.VariantType == Variant.Type.String)
            {
                metadata.Interface = interfaceVar.AsString();
            }

            if (dict.TryGetValue("status", out var statusVar) &&
                statusVar.VariantType == Variant.Type.String)
            {
                metadata.Status = statusVar.AsString();
            }

            if (dict.TryGetValue("note", out var noteVar) &&
                noteVar.VariantType == Variant.Type.String)
            {
                metadata.Note = noteVar.AsString();
            }

            return metadata;
        }

        private static GhostTerminalBootSequence MapToBootSequence(Godot.Collections.Dictionary dict)
        {
            var bootSequence = new GhostTerminalBootSequence();

            if (dict.TryGetValue("glitchLines", out var glitchLinesVar) &&
                glitchLinesVar.VariantType == Variant.Type.Array)
            {
                bootSequence.GlitchLines.AddRange(ExtractStringList(glitchLinesVar.AsGodotArray()));
            }

            if (dict.TryGetValue("fadeToStable", out var fadeVar) &&
                fadeVar.VariantType == Variant.Type.Bool)
            {
                bootSequence.FadeToStable = fadeVar.AsBool();
            }

            return bootSequence;
        }

        private static GhostTerminalMonologue MapToMonologue(Godot.Collections.Dictionary dict)
        {
            var monologue = new GhostTerminalMonologue();

            if (dict.TryGetValue("lines", out var linesVar) &&
                linesVar.VariantType == Variant.Type.Array)
            {
                monologue.Lines.AddRange(ExtractStringList(linesVar.AsGodotArray()));
            }

            if (dict.TryGetValue("cinematicTiming", out var timingVar) &&
                timingVar.VariantType == Variant.Type.String)
            {
                monologue.CinematicTiming = timingVar.AsString();
            }

            return monologue;
        }

        private static GhostTerminalChoiceBlock MapToChoiceBlock(Godot.Collections.Dictionary dict)
        {
            var block = new GhostTerminalChoiceBlock();

            if (dict.TryGetValue("setup", out var setupVar) &&
                setupVar.VariantType == Variant.Type.Array)
            {
                block.Setup.AddRange(ExtractStringList(setupVar.AsGodotArray()));
            }

            if (dict.TryGetValue("question", out var questionVar) &&
                questionVar.VariantType == Variant.Type.Dictionary)
            {
                block.Question = MapToGhostQuestion(questionVar.AsGodotDictionary());
            }

            return block;
        }

        private static GhostTerminalStoryFragment MapToStoryFragment(Godot.Collections.Dictionary dict)
        {
            var fragment = new GhostTerminalStoryFragment();

            if (dict.TryGetValue("intro", out var introVar) &&
                introVar.VariantType == Variant.Type.Array)
            {
                fragment.Intro.AddRange(ExtractStringList(introVar.AsGodotArray()));
            }

            if (dict.TryGetValue("question", out var questionVar) &&
                questionVar.VariantType == Variant.Type.Dictionary)
            {
                fragment.Question = MapToGhostQuestion(questionVar.AsGodotDictionary());
            }

            if (dict.TryGetValue("continuation", out var continuationVar) &&
                continuationVar.VariantType == Variant.Type.Array)
            {
                fragment.Continuation.AddRange(ExtractStringList(continuationVar.AsGodotArray()));
            }

            return fragment;
        }

        private static GhostTerminalSecretQuestion MapToSecretQuestionBlock(Godot.Collections.Dictionary dict)
        {
            var secret = new GhostTerminalSecretQuestion();

            if (dict.TryGetValue("setup", out var setupVar) &&
                setupVar.VariantType == Variant.Type.Array)
            {
                secret.Setup.AddRange(ExtractStringList(setupVar.AsGodotArray()));
            }

            if (dict.TryGetValue("prompt", out var promptVar) &&
                promptVar.VariantType == Variant.Type.String)
            {
                secret.Prompt = promptVar.AsString();
            }

            if (dict.TryGetValue("options", out var optionsVar) &&
                optionsVar.VariantType == Variant.Type.Array)
            {
                foreach (var optionVar in optionsVar.AsGodotArray())
                {
                    if (optionVar.VariantType == Variant.Type.Dictionary)
                    {
                        secret.Options.Add(MapToGhostOption(optionVar.AsGodotDictionary()));
                    }
                }
            }

            if (dict.TryGetValue("secretReveal", out var revealVar) &&
                revealVar.VariantType == Variant.Type.Dictionary)
            {
                secret.SecretReveal = MapToSecretReveal(revealVar.AsGodotDictionary());
            }

            return secret;
        }

        private static GhostTerminalNameQuestion MapToNameQuestion(Godot.Collections.Dictionary dict)
        {
            var nameQuestion = new GhostTerminalNameQuestion();

            if (dict.TryGetValue("setup", out var setupVar) &&
                setupVar.VariantType == Variant.Type.Array)
            {
                nameQuestion.Setup.AddRange(ExtractStringList(setupVar.AsGodotArray()));
            }

            if (dict.TryGetValue("prompt", out var promptVar) &&
                promptVar.VariantType == Variant.Type.String)
            {
                nameQuestion.Prompt = promptVar.AsString();
            }

            if (dict.TryGetValue("options", out var optionsVar) &&
                optionsVar.VariantType == Variant.Type.Array)
            {
                foreach (var optionVar in optionsVar.AsGodotArray())
                {
                    if (optionVar.VariantType == Variant.Type.Dictionary)
                    {
                        nameQuestion.Options.Add(MapToGhostOption(optionVar.AsGodotDictionary()));
                    }
                }
            }

            return nameQuestion;
        }

        private static GhostTerminalExit MapToExit(Godot.Collections.Dictionary dict)
        {
            var exit = new GhostTerminalExit();

            if (dict.TryGetValue("selectedThread", out var threadVar) &&
                threadVar.VariantType == Variant.Type.String)
            {
                exit.SelectedThread = threadVar.AsString();
            }

            if (dict.TryGetValue("finalLines", out var finalLinesVar) &&
                finalLinesVar.VariantType == Variant.Type.Array)
            {
                exit.FinalLines.AddRange(ExtractStringList(finalLinesVar.AsGodotArray()));
            }

            return exit;
        }

        private static GhostTerminalQuestion MapToGhostQuestion(Godot.Collections.Dictionary dict)
        {
            var question = new GhostTerminalQuestion();

            if (dict.TryGetValue("prompt", out var promptVar) &&
                promptVar.VariantType == Variant.Type.String)
            {
                question.Prompt = promptVar.AsString();
            }

            if (dict.TryGetValue("context", out var contextVar) &&
                contextVar.VariantType == Variant.Type.String)
            {
                question.Context = contextVar.AsString();
            }

            if (dict.TryGetValue("options", out var optionsVar) &&
                optionsVar.VariantType == Variant.Type.Array)
            {
                foreach (var optionVar in optionsVar.AsGodotArray())
                {
                    if (optionVar.VariantType == Variant.Type.Dictionary)
                    {
                        question.Options.Add(MapToGhostOption(optionVar.AsGodotDictionary()));
                    }
                }
            }

            return question;
        }

        private static GhostTerminalOption MapToGhostOption(Godot.Collections.Dictionary dict)
        {
            var option = new GhostTerminalOption();

            if (dict.TryGetValue("id", out var idVar) &&
                idVar.VariantType == Variant.Type.String)
            {
                option.Id = idVar.AsString();
            }

            if (dict.TryGetValue("text", out var textVar) &&
                textVar.VariantType == Variant.Type.String)
            {
                option.Text = textVar.AsString();
            }

            if (dict.TryGetValue("dreamweaver", out var dreamVar) &&
                dreamVar.VariantType == Variant.Type.String)
            {
                option.Dreamweaver = dreamVar.AsString();
            }

            if (dict.TryGetValue("philosophical", out var philosophyVar) &&
                philosophyVar.VariantType == Variant.Type.String)
            {
                option.Philosophical = philosophyVar.AsString();
            }

            if (dict.TryGetValue("response", out var responseVar) &&
                responseVar.VariantType == Variant.Type.String)
            {
                option.Response = responseVar.AsString();
            }

            if (dict.TryGetValue("scores", out var scoresVar) &&
                scoresVar.VariantType == Variant.Type.Dictionary)
            {
                option.Scores = MapToScores(scoresVar.AsGodotDictionary());
            }

            return option;
        }

        private static GhostTerminalSecretReveal MapToSecretReveal(Godot.Collections.Dictionary dict)
        {
            var reveal = new GhostTerminalSecretReveal();

            if (dict.TryGetValue("visual", out var visualVar) &&
                visualVar.VariantType == Variant.Type.String)
            {
                reveal.Visual = visualVar.AsString();
            }

            if (dict.TryGetValue("text", out var textVar) &&
                textVar.VariantType == Variant.Type.Array)
            {
                reveal.Text.AddRange(ExtractStringList(textVar.AsGodotArray()));
            }

            if (dict.TryGetValue("persistent", out var persistentVar) &&
                persistentVar.VariantType == Variant.Type.Bool)
            {
                reveal.Persistent = persistentVar.AsBool();
            }

            if (dict.TryGetValue("journalEntry", out var journalVar) &&
                journalVar.VariantType == Variant.Type.String)
            {
                reveal.JournalEntry = journalVar.AsString();
            }

            return reveal;
        }

        private static GhostTerminalScoreDistribution MapToScores(Godot.Collections.Dictionary dict)
        {
            var scores = new GhostTerminalScoreDistribution();

            if (dict.TryGetValue("light", out var lightVar) &&
                lightVar.VariantType == Variant.Type.Int)
            {
                scores.Light = lightVar.AsInt32();
            }

            if (dict.TryGetValue("shadow", out var shadowVar) &&
                shadowVar.VariantType == Variant.Type.Int)
            {
                scores.Shadow = shadowVar.AsInt32();
            }

            if (dict.TryGetValue("ambition", out var ambitionVar) &&
                ambitionVar.VariantType == Variant.Type.Int)
            {
                scores.Ambition = ambitionVar.AsInt32();
            }

            return scores;
        }

        private static List<string> ExtractStringList(Godot.Collections.Array array)
        {
            var list = new List<string>(array.Count);
            foreach (var item in array)
            {
                if (item.VariantType == Variant.Type.String)
                {
                    list.Add(item.AsString());
                }
            }

            return list;
        }
    }
}
