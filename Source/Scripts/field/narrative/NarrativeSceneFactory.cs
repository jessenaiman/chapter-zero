namespace OmegaSpiral.Source.Scripts.Field.Narrative
{
    using System;
    using System.Collections.Generic;
    using Godot;
    using OmegaSpiral.Source.Scripts.Common;

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
                Type = data.TryGetValue("type", out var typeVar) && typeVar.VariantType == Variant.Type.String
                    ? typeVar.AsString()
                    : "narrative_terminal",
            };

            if (data.TryGetValue("openingLines", out var openingLinesVar) &&
                openingLinesVar.VariantType == Variant.Type.Array)
            {
                result.OpeningLines = ExtractStringList(openingLinesVar.AsGodotArray());
            }

            if (data.TryGetValue("initialChoice", out var initialChoiceVar) &&
                initialChoiceVar.VariantType == Variant.Type.Dictionary)
            {
                result.InitialChoice = MapToInitialChoice(initialChoiceVar.AsGodotDictionary());
            }

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

                result.StoryBlocks = storyBlocks;
            }

            if (data.TryGetValue("namePrompt", out var namePromptVar) &&
                namePromptVar.VariantType == Variant.Type.String)
            {
                result.NamePrompt = namePromptVar.AsString();
            }

            if (data.TryGetValue("secretQuestion", out var secretVar) &&
                secretVar.VariantType == Variant.Type.Dictionary)
            {
                result.SecretQuestion = MapToSecretQuestion(secretVar.AsGodotDictionary());
            }

            if (data.TryGetValue("exitLine", out var exitVar) &&
                exitVar.VariantType == Variant.Type.String)
            {
                result.ExitLine = exitVar.AsString();
            }

            return result;
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
                choice.Text = labelVar.AsString();
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
                option.Text = textVar.AsString();
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
