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

            var result = new NarrativeSceneData
            {
                Type = GetSceneType(data),
                OpeningLines = GetOpeningLines(data),
                InitialChoice = GetInitialChoice(data),
                StoryBlocks = GetStoryBlocks(data),
                NamePrompt = GetNamePrompt(data),
                SecretQuestion = GetSecretQuestion(data),
                ExitLine = GetExitLine(data)
            };

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
