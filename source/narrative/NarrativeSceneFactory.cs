using Variant = Godot.Variant;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Narrative
{
    /// <summary>
    /// Factory helpers for constructing <see cref="NarrativeSceneData"/> instances from Godot dictionaries.
    /// Centralizes JSON to domain mapping so both runtime code and tests share the same logic.
    /// </summary>
    internal static partial class NarrativeSceneFactory
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
                // TODO: Implement MapToCinematic if Cinematic type is used
                // result.Cinematic = MapToCinematic(data);
            }
            else if (string.Equals(sceneType, "echo_chamber_stage", StringComparison.OrdinalIgnoreCase))
            {
                // TODO: Implement MapToEchoChamber if EchoChamber type is used
                // result.EchoChamber = MapToEchoChamber(data);
            }
            else if (string.Equals(sceneType, "echo_vault_stage", StringComparison.OrdinalIgnoreCase))
            {
                // TODO: Implement MapToEchoVault if EchoVault type is used
                // result.EchoVault = MapToEchoVault(data);
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
                return MapToInitialChoice(initialChoiceVar.AsGodotDictionary<string, Variant>());
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
                        storyBlocks.Add(MapToStoryBlock(blockVar.AsGodotDictionary<string, Variant>()));
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
                return MapToSecretQuestion(secretVar.AsGodotDictionary<string, Variant>());
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

        private static NarrativeChoice MapToInitialChoice(Godot.Collections.Dictionary<string, Variant> dict)
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
                var options = new List<ChoiceOption>();
                foreach (var optionVar in optionsVar.AsGodotArray())
                {
                    if (optionVar.VariantType == Variant.Type.Dictionary)
                    {
                        options.Add(MapToChoiceOption(optionVar.AsGodotDictionary<string, Variant>()));
                    }
                }

                choice.Options = options;
            }

            return choice;
        }

        private static ChoiceOption MapToChoiceOption(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var choice = new ChoiceOption();

            AssignString(dict, "id", id =>
            {
                choice.Id = id;
                if (!string.IsNullOrWhiteSpace(id) && Enum.TryParse(id, true, out DreamweaverThread parsedThread))
                {
                    choice.Thread = parsedThread;
                }
            });

            AssignString(dict, "label", label =>
            {
                choice.Text = label;
                choice.Label = label;
            });

            AssignString(dict, "description", description => choice.Description = description);

            return choice;
        }

        private static StoryBlock MapToStoryBlock(Godot.Collections.Dictionary<string, Variant> dict)
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
                        choices.Add(MapToChoiceOption(choiceVar.AsGodotDictionary<string, Variant>()));
                    }
                }

                block.Choices = choices;
            }

            return block;
        }

        private static ChoiceOption MapToChoiceOption(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var option = new ChoiceOption();

            AssignString(dict, "text", text =>
            {
                option.Text = text;
                option.Label = text;
            });

            AssignInt(dict, "nextBlock", value => option.NextBlock = value);
            AssignString(dict, "description", description => option.Description = description);
            AssignString(dict, "id", id => option.Id = id);

            return option;
        }

        private static SecretQuestion MapToSecretQuestion(Godot.Collections.Dictionary<string, Variant> dict)
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
                foreach (var option in ExtractStringList(optionsVar.AsGodotArray()))
                {
                    question.Options.Add(option);
                }
            }

            return question;
        }

        private static EchoVaultPresentationTier MapToPresentationTier(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var tier = new EchoVaultPresentationTier();

            if (dict.TryGetValue("tier", out var tierVar) &&
                tierVar.VariantType == Variant.Type.Int)
            {
                tier.Tier = tierVar.AsInt32();
            }

            if (dict.TryGetValue("theme", out var themeVar) &&
                themeVar.VariantType == Variant.Type.String)
            {
                tier.Theme = themeVar.AsString();
            }

            if (dict.TryGetValue("notes", out var notesVar) &&
                notesVar.VariantType == Variant.Type.String)
            {
                tier.Notes = notesVar.AsString();
            }

            return tier;
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
