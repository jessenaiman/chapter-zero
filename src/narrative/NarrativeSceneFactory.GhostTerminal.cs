using System.Collections.Generic;
using Godot;

namespace OmegaSpiral.Source.Narrative
{
    internal static partial class NarrativeSceneFactory
    {
        private static GhostTerminalCinematicData MapToCinematic(Godot.Collections.Dictionary<string, Variant> data)
        {
            var cinematic = new GhostTerminalCinematicData();

            AssignDictionary(data, "metadata", dict => cinematic.Metadata = MapToMetadata(dict));
            AssignDictionary(data, "bootSequence", dict => cinematic.BootSequence = MapToBootSequence(dict));
            AssignDictionary(data, "openingMonologue", dict => cinematic.OpeningMonologue = MapToMonologue(dict));
            AssignDictionary(data, "firstChoice", dict => cinematic.FirstChoice = MapToChoiceBlock(dict));
            AssignDictionary(data, "storyFragment", dict => cinematic.StoryFragment = MapToStoryFragment(dict));
            AssignDictionary(data, "secretQuestion", dict => cinematic.SecretQuestion = MapToSecretQuestionBlock(dict));
            AssignDictionary(data, "nameQuestion", dict => cinematic.NameQuestion = MapToNameQuestion(dict));
            AssignDictionary(data, "exit", dict => cinematic.Exit = MapToExit(dict));

            return cinematic;
        }

        private static GhostTerminalMetadata MapToMetadata(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var metadata = new GhostTerminalMetadata();

            AssignString(dict, "iteration", value => metadata.Iteration = value);
            AssignInt(dict, "iterationFallback", value => metadata.IterationFallback = value);
            AssignString(dict, "previousAttempt", value => metadata.PreviousAttempt = value);
            AssignString(dict, "interface", value => metadata.Interface = value);
            AssignString(dict, "status", value => metadata.Status = value);
            AssignString(dict, "note", value => metadata.Note = value);

            return metadata;
        }

        private static GhostTerminalBootSequence MapToBootSequence(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var boot = new GhostTerminalBootSequence();

            AssignArray(dict, "glitchLines", array =>
            {
                ((List<string>)boot.GlitchLines).AddRange(ExtractStringList(array));
            });
            AssignBool(dict, "fadeToStable", value => boot.FadeToStable = value);

            return boot;
        }

        private static GhostTerminalMonologue MapToMonologue(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var monologue = new GhostTerminalMonologue();

            AssignArray(dict, "lines", array =>
            {
                ((List<string>)monologue.Lines).AddRange(ExtractStringList(array));
            });
            AssignString(dict, "cinematicTiming", value => monologue.CinematicTiming = value);

            return monologue;
        }

        private static GhostTerminalChoiceBlock MapToChoiceBlock(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var block = new GhostTerminalChoiceBlock();

            AssignArray(dict, "setup", array =>
            {
                ((List<string>)block.Setup).AddRange(ExtractStringList(array));
            });
            AssignDictionary(dict, "question", questionDict => block.Question = MapToGhostQuestion(questionDict));

            return block;
        }

        private static GhostTerminalStoryFragment MapToStoryFragment(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var fragment = new GhostTerminalStoryFragment();

            AssignArray(dict, "intro", array =>
            {
                ((List<string>)fragment.Intro).AddRange(ExtractStringList(array));
            });
            AssignDictionary(dict, "question", questionDict => fragment.Question = MapToGhostQuestion(questionDict));
            AssignArray(dict, "continuation", array =>
            {
                ((List<string>)fragment.Continuation).AddRange(ExtractStringList(array));
            });

            return fragment;
        }

        private static GhostTerminalSecretQuestion MapToSecretQuestionBlock(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var question = new GhostTerminalSecretQuestion();

            AssignArray(dict, "setup", array =>
            {
                ((List<string>)question.Setup).AddRange(ExtractStringList(array));
            });
            AssignString(dict, "prompt", value => question.Prompt = value);
            AssignArray(dict, "options", array =>
            {
                foreach (var optionVar in array)
                {
                    if (optionVar.VariantType == Variant.Type.Dictionary)
                    {
                        question.Options.Add(MapToGhostOption(optionVar.AsGodotDictionary<string, Variant>()));
                    }
                }
            });
            AssignDictionary(dict, "secretReveal", revealDict => question.SecretReveal = MapToSecretReveal(revealDict));

            return question;
        }

        private static GhostTerminalNameQuestion MapToNameQuestion(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var question = new GhostTerminalNameQuestion();

            AssignArray(dict, "setup", array =>
            {
                ((List<string>)question.Setup).AddRange(ExtractStringList(array));
            });
            AssignString(dict, "prompt", value => question.Prompt = value);
            AssignArray(dict, "options", array =>
            {
                foreach (var optionVar in array)
                {
                    if (optionVar.VariantType == Variant.Type.Dictionary)
                    {
                        question.Options.Add(MapToGhostOption(optionVar.AsGodotDictionary<string, Variant>()));
                    }
                }
            });

            return question;
        }

        private static GhostTerminalExit MapToExit(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var exit = new GhostTerminalExit();

            AssignString(dict, "selectedThread", value => exit.SelectedThread = value);
            AssignArray(dict, "finalLines", array =>
            {
                ((List<string>)exit.FinalLines).AddRange(ExtractStringList(array));
            });

            return exit;
        }

        private static GhostTerminalQuestion MapToGhostQuestion(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var question = new GhostTerminalQuestion();

            AssignString(dict, "prompt", value => question.Prompt = value);
            AssignString(dict, "context", value => question.Context = value);
            AssignArray(dict, "options", array =>
            {
                foreach (var optionVar in array)
                {
                    if (optionVar.VariantType == Variant.Type.Dictionary)
                    {
                        question.Options.Add(MapToGhostOption(optionVar.AsGodotDictionary<string, Variant>()));
                    }
                }
            });

            return question;
        }

        private static GhostTerminalOption MapToGhostOption(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var option = new GhostTerminalOption();

            AssignString(dict, "id", value => option.Id = value);
            AssignString(dict, "text", value => option.Text = value);
            AssignString(dict, "dreamweaver", value => option.Dreamweaver = value);
            AssignString(dict, "philosophical", value => option.Philosophical = value);
            AssignString(dict, "response", value => option.Response = value);
            AssignDictionary(dict, "scores", scoresDict => option.Scores = MapToScores(scoresDict));

            return option;
        }

        private static GhostTerminalSecretReveal MapToSecretReveal(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var reveal = new GhostTerminalSecretReveal();

            AssignString(dict, "visual", value => reveal.Visual = value);
            AssignArray(dict, "text", array =>
            {
                ((List<string>)reveal.Text).AddRange(ExtractStringList(array));
            });
            AssignBool(dict, "persistent", value => reveal.Persistent = value);
            AssignString(dict, "journalEntry", value => reveal.JournalEntry = value);

            return reveal;
        }

        private static GhostTerminalScoreDistribution MapToScores(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var scores = new GhostTerminalScoreDistribution();

            AssignInt(dict, "light", value => scores.Light = value);
            AssignInt(dict, "shadow", value => scores.Shadow = value);
            AssignInt(dict, "ambition", value => scores.Ambition = value);

            return scores;
        }
    }
}
