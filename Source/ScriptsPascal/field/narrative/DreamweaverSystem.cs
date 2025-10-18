// <copyright file="DreamweaverSystem.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Scripts.Field.Narrative;
using NarrativeChoiceOption = OmegaSpiral.Source.Scripts.Field.Narrative.ChoiceOption;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Scripts
{
    /// <summary>
    /// Main orchestrator for the Dreamweaver LLM system.
    /// Manages three personas (Hero, Shadow, Ambition) and coordinates
    /// dynamic narrative generation using the nobodywho framework.
    /// </summary>
    public partial class DreamweaverSystem : Node
    {
        private readonly TaskCompletionSource<bool> initializationComplete = new();
        private readonly Dictionary<string, DreamweaverPersona> personas = new();
        private GameState? gameState;

        /// <summary>
        /// Gets a task that completes when the Dreamweaver system has finished initializing.
        /// </summary>
        public Task<bool> InitializationComplete => this.initializationComplete.Task;

        /// <summary>
        /// Emits the signal when narrative is generated for a persona.
        /// </summary>
        /// <param name="personaId">The identifier of the persona that generated the narrative.</param>
        /// <param name="generatedText">The generated narrative text.</param>
        [Signal]
        public delegate void NarrativeGeneratedEventHandler(string personaId, string generatedText);

        /// <summary>
        /// Emits the signal when a persona is activated.
        /// </summary>
        /// <param name="personaId">The identifier of the activated persona.</param>
        [Signal]
        public delegate void PersonaActivatedEventHandler(string personaId);

        /// <summary>
        /// Emits the signal when an error occurs during narrative generation.
        /// </summary>
        /// <param name="personaId">The identifier of the persona that encountered the error.</param>
        /// <param name="errorMessage">The error message describing what went wrong.</param>
        [Signal]
        public delegate void GenerationErrorEventHandler(string personaId, string errorMessage);

        /// <inheritdoc/>
        public override void _Ready()
        {
            GD.Print($"DreamweaverSystem _Ready called, instance: {this.GetInstanceId()}");
            this.gameState = this.GetNode<GameState>("/root/GameState");

            // Initialize the three Dreamweaver personas
            this.InitializePersonas();

            GD.Print("Dreamweaver System initialized with 3 personas");
            this.initializationComplete.SetResult(true);
        }

        /// <summary>
        /// Generates dynamic narrative for a specific persona using LLM.
        /// Uses the JSON text as a foundation for the prompt.
        /// </summary>
        /// <param name="personaId">The identifier of the persona to use for generation.</param>
        /// <param name="context">Additional context for narrative generation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<string> GenerateNarrativeAsync(string personaId, string context = "")
        {
            if (!this.personas.TryGetValue(personaId, out DreamweaverPersona? persona))
            {
                GD.PrintErr($"Unknown persona: {personaId}");
                this.EmitSignal(SignalName.GenerationError, personaId, "Unknown persona");
                return GetFallbackNarrative(personaId);
            }

            try
            {
                var generatedText = await persona.GenerateNarrativeAsync(context).ConfigureAwait(false);

                this.EmitSignal(SignalName.NarrativeGenerated, personaId, generatedText);
                return generatedText;
            }
            catch (InvalidOperationException ex)
            {
                GD.PrintErr($"Failed to generate narrative for {personaId}: {ex.Message}");
                this.EmitSignal(SignalName.GenerationError, personaId, ex.Message);
                return GetFallbackNarrative(personaId);
            }
        }

        /// <summary>
        /// Gets a random opening line for a persona, enhanced by LLM if available.
        /// </summary>
        /// <param name="personaId">The identifier of the persona to get the opening line for.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<string> GetOpeningLineAsync(string personaId)
        {
            if (!this.personas.TryGetValue(personaId, out DreamweaverPersona? persona))
            {
                return GetFallbackOpeningLine(personaId);
            }

            try
            {
                return await persona.GetOpeningLineAsync().ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                GD.PrintErr($"Failed to get opening line for {personaId}: {ex.Message}");
                return GetFallbackOpeningLine(personaId);
            }
        }

        /// <summary>
        /// Generates dynamic choices for a persona based on current game state.
        /// </summary>
        /// <param name="personaId">The identifier of the persona to generate choices for.</param>
        /// <param name="context">Additional context for choice generation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<List<NarrativeChoiceOption>> GenerateChoicesAsync(string personaId, string context = "")
        {
            if (!this.personas.TryGetValue(personaId, out DreamweaverPersona? persona))
            {
                return GetFallbackChoices();
            }

            try
            {
                var choices = await persona.GenerateChoicesAsync(context).ConfigureAwait(false);
                return choices.Select(c => new NarrativeChoiceOption { Id = c.Id, Label = c.Label, Description = c.Description }).ToList();
            }
            catch (InvalidOperationException ex)
            {
                GD.PrintErr($"Failed to generate choices for {personaId}: {ex.Message}");
                return GetFallbackChoices();
            }
        }

        /// <summary>
        /// Activates a persona, making it the primary narrative voice.
        /// </summary>
        /// <param name="personaId">The identifier of the persona to activate.</param>
        public void ActivatePersona(string personaId)
        {
            if (!this.personas.ContainsKey(personaId))
            {
                GD.PrintErr($"Cannot activate unknown persona: {personaId}");
                return;
            }

            // Deactivate all other personas
            foreach (var kvp in this.personas)
            {
                kvp.Value.IsActive = kvp.Key == personaId;
            }

            this.EmitSignal(SignalName.PersonaActivated, personaId);
            GD.Print($"Activated Dreamweaver persona: {personaId}");
        }

        /// <summary>
        /// Gets the currently active persona.
        /// </summary>
        /// <returns>The currently active persona, or null if none is active.</returns>
        public DreamweaverPersona? GetActivePersona()
        {
            return this.personas.Values.FirstOrDefault(p => p.IsActive);
        }

        /// <summary>
        /// Gets a persona by its identifier.
        /// </summary>
        /// <param name="personaId">The identifier of the persona to retrieve.</param>
        /// <returns>The persona with the given identifier, or null if not found.</returns>
        public DreamweaverPersona? GetPersona(string personaId)
        {
            var persona = this.personas.GetValueOrDefault(personaId);
            GD.Print($"GetPersona called for '{personaId}', found: {persona != null}, total personas: {this.personas.Count}");
            return persona;
        }

        /// <summary>
        /// Loads persona configuration from JSON file using Godot native parsing with schema validation.
        /// </summary>
        /// <param name="personaId">The persona identifier.</param>
        /// <returns>The loaded persona configuration or null if not found.</returns>
        /// <exception cref="InvalidOperationException">Thrown when JSON parsing or schema validation fails.</exception>
        private static PersonaConfig? LoadPersonaConfig(string personaId)
        {
            try
            {
                var configPath = $"res://Source/Data/stages/ghost-terminal/{personaId}.json";

                GD.Print($"Loading persona config from: {configPath}");

                if (!Godot.FileAccess.FileExists(configPath))
                {
                    GD.PrintErr($"Persona config not found: {configPath}");
                    return null;
                }

                // Use ConfigurationService for unified JSON loading with schema validation
                var jsonData = ConfigurationService.LoadConfiguration(configPath);

                if (jsonData == null)
                {
                    GD.PrintErr($"Failed to load or validate config for {personaId}");
                    return null;
                }

                // Validate against narrative terminal schema
                var schemaPath = "res://Source/Data/schemas/narrative_terminal_schema.json";
                if (!ConfigurationService.ValidateConfiguration(jsonData, schemaPath))
                {
                    GD.PrintErr($"Schema validation failed for {personaId} config");
                    return null;
                }

                // Map Godot.Collections.Dictionary to PersonaConfig
                var config = MapToPersonaConfig(jsonData);
                GD.Print($"Loaded config for {personaId}: OpeningLines count = {config?.OpeningLines.Count ?? 0}");
                return config;
            }
            catch (InvalidOperationException ex)
            {
                GD.PrintErr($"Failed to load persona config for {personaId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Maps a Godot.Collections.Dictionary to PersonaConfig domain object.
        /// </summary>
        /// <param name="data">The dictionary data from JSON.</param>
        /// <returns>The mapped PersonaConfig object.</returns>
        private static PersonaConfig MapToPersonaConfig(Godot.Collections.Dictionary<string, Variant> data)
        {
            var config = new PersonaConfig();

            // Map opening lines
            if (data.TryGetValue("openingLines", out var openingLinesValue) && openingLinesValue.VariantType == Variant.Type.Array)
            {
                var openingArray = data["openingLines"].AsGodotArray();
                foreach (var line in openingArray)
                {
                    config.OpeningLines.Add(line.AsString());
                }
            }

            // Map initial choice
            if (data.TryGetValue("initialChoice", out var initialChoiceValue) && initialChoiceValue.VariantType == Variant.Type.Dictionary)
            {
                var initialChoiceDict = data["initialChoice"].AsGodotDictionary();
                config.InitialChoice = new PersonaChoiceBlock
                {
                    Prompt = initialChoiceDict.ContainsKey("prompt")
                        ? initialChoiceDict["prompt"].AsString()
                        : string.Empty,
                    Options = new(),
                };

                if (initialChoiceDict.ContainsKey("options") && initialChoiceDict["options"].VariantType == Variant.Type.Array)
                {
                    var optionsArray = initialChoiceDict["options"].AsGodotArray();
                    foreach (var opt in optionsArray)
                    {
                        if (opt.VariantType == Variant.Type.Dictionary)
                        {
                            var optDict = opt.AsGodotDictionary();
                            config.InitialChoice.Options.Add(new PersonaChoiceOption
                            {
                                Id = optDict.ContainsKey("id") ? optDict["id"].AsString() : string.Empty,
                                Label = optDict.ContainsKey("label") ? optDict["label"].AsString() : string.Empty,
                                Description = optDict.ContainsKey("description") ? optDict["description"].AsString() : string.Empty,
                            });
                        }
                    }
                }
            }

            // Map story blocks
            if (data.TryGetValue("storyBlocks", out var storyBlocksValue) && storyBlocksValue.VariantType == Variant.Type.Array)
            {
                var blocksArray = data["storyBlocks"].AsGodotArray();
                foreach (var block in blocksArray)
                {
                    if (block.VariantType == Variant.Type.Dictionary)
                    {
                        var blockDict = block.AsGodotDictionary();
                        var storyBlock = new PersonaStoryBlock();

                        if (blockDict.ContainsKey("paragraphs") && blockDict["paragraphs"].VariantType == Variant.Type.Array)
                        {
                            var paragraphsArray = blockDict["paragraphs"].AsGodotArray();
                            foreach (var para in paragraphsArray)
                            {
                                storyBlock.Paragraphs.Add(para.AsString());
                            }
                        }

                        if (blockDict.ContainsKey("question"))
                        {
                            storyBlock.Question = blockDict["question"].AsString();
                        }

                        if (blockDict.ContainsKey("choices") && blockDict["choices"].VariantType == Variant.Type.Array)
                        {
                            var choicesArray = blockDict["choices"].AsGodotArray();
                            foreach (var choice in choicesArray)
                            {
                                if (choice.VariantType == Variant.Type.Dictionary)
                                {
                                    var choiceDict = choice.AsGodotDictionary();
                                    storyBlock.Choices.Add(new PersonaNarrativeChoice
                                    {
                                        Text = choiceDict.ContainsKey("text") ? choiceDict["text"].AsString() : string.Empty,
                                        NextBlock = choiceDict.ContainsKey("nextBlock") ? choiceDict["nextBlock"].AsInt32() : 0,
                                    });
                                }
                            }
                        }

                        config.StoryBlocks.Add(storyBlock);
                    }
                }
            }

            // Map name prompt
            if (data.TryGetValue("namePrompt", out var namePromptValue))
            {
                config.NamePrompt = data["namePrompt"].AsString();
            }

            // Map secret question
            if (data.TryGetValue("secretQuestion", out var secretQuestionValue) && secretQuestionValue.VariantType == Variant.Type.Dictionary)
            {
                var secretQuestionDict = data["secretQuestion"].AsGodotDictionary();
                config.SecretQuestion = new PersonaSecretQuestionBlock
                {
                    Prompt = secretQuestionDict.ContainsKey("prompt")
                        ? secretQuestionDict["prompt"].AsString()
                        : string.Empty,
                    Options = new List<string>(),
                };

                if (secretQuestionDict.ContainsKey("options") && secretQuestionDict["options"].VariantType == Variant.Type.Array)
                {
                    var optionsArray = secretQuestionDict["options"].AsGodotArray();
                    foreach (var opt in optionsArray)
                    {
                        if (opt.VariantType == Variant.Type.String)
                        {
                            config.SecretQuestion.Options.Add(opt.AsString());
                        }
                    }
                }
            }

            // Map exit line
            if (data.TryGetValue("exitLine", out var exitLineValue))
            {
                config.ExitLine = data["exitLine"].AsString();
            }

            return config;
        }

        /// <summary>
        /// Fallback methods for when LLM generation fails.
        /// </summary>
        /// <param name="personaId">The persona identifier.</param>
        /// <returns>A fallback narrative string.</returns>
        private static string GetFallbackNarrative(string personaId)
        {
            var fallbacks = new Dictionary<string, string>()
            {
                ["hero"] = "The hero's path calls to you, filled with light and shadow.",
                ["shadow"] = "The shadows whisper secrets that only you can hear.",
                ["ambition"] = "Ambition drives you forward, carving new paths through reality.",
            };

            return fallbacks.GetValueOrDefault(personaId, "The narrative continues...");
        }

        /// <summary>
        /// Gets a fallback opening line for when configuration fails to load.
        /// </summary>
        /// <param name="personaId">The persona identifier.</param>
        /// <returns>A fallback opening line string.</returns>
        private static string GetFallbackOpeningLine(string personaId)
        {
            var fallbacks = new Dictionary<string, string>()
            {
                ["hero"] = "A hero emerges from the darkness.",
                ["shadow"] = "The shadows remember what you forget.",
                ["ambition"] = "Ambition flows upward, defying gravity.",
            };

            return fallbacks.GetValueOrDefault(personaId, "Welcome to the spiral.");
        }

        /// <summary>
        /// Gets fallback choices for when dynamic choice generation fails.
        /// </summary>
        /// <returns>A list of default narrative choices.</returns>
        private static List<NarrativeChoiceOption> GetFallbackChoices()
        {
            return new()
            {
                new NarrativeChoiceOption { Id = "continue", Label = "Continue", Description = "Continue the journey" },
                new NarrativeChoiceOption { Id = "reflect", Label = "Reflect", Description = "Take a moment to reflect" },
                new NarrativeChoiceOption { Id = "question", Label = "Question", Description = "Ask a question" },
            };
        }

        /// <summary>
        /// Initializes the three Dreamweaver personas by loading their configurations.
        /// </summary>
        private void InitializePersonas()
        {
            GD.Print("Initializing personas...");
            // Load persona configurations from JSON files
            var heroConfig = LoadPersonaConfig("hero");
            var shadowConfig = LoadPersonaConfig("shadow");
            var ambitionConfig = LoadPersonaConfig("ambition");

            GD.Print($"Loaded configs - Hero: {heroConfig != null}, Shadow: {shadowConfig != null}, Ambition: {ambitionConfig != null}");

            if (this.gameState == null)
            {
                GD.PrintErr("GameState not found, cannot initialize personas");
                return;
            }

            if (heroConfig != null)
            {
                this.personas["hero"] = new DreamweaverPersona("hero", heroConfig, this.gameState);
                GD.Print($"Hero persona initialized with {heroConfig.OpeningLines.Count} opening lines");
            }

            if (shadowConfig != null)
            {
                this.personas["shadow"] = new DreamweaverPersona("shadow", shadowConfig, this.gameState);
            }

            if (ambitionConfig != null)
            {
                this.personas["ambition"] = new DreamweaverPersona("ambition", ambitionConfig, this.gameState);
            }

            GD.Print($"Total personas initialized: {this.personas.Count}");
            foreach (var key in this.personas.Keys)
            {
                GD.Print($"Persona key: '{key}'");
            }
        }
    }
}
