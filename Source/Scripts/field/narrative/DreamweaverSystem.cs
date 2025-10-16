// <copyright file="DreamweaverSystem.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Godot;
    using OmegaSpiral.Scripts.Field.Narrative;
    using NarrativeChoiceOption = OmegaSpiral.Source.Scripts.Field.Narrative.ChoiceOption;
    using YamlDotNet.Serialization;
    using OmegaSpiral.Source.Scripts.Common;

    /// <summary>
    /// Main orchestrator for the Dreamweaver LLM system.
    /// Manages three personas (Hero, Shadow, Ambition) and coordinates
    /// dynamic narrative generation using the nobodywho framework.
    /// </summary>
    public partial class DreamweaverSystem : Node
    {
        private readonly TaskCompletionSource<bool> initializationComplete = new();
        private Dictionary<string, DreamweaverPersona> personas = new();
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
            catch (System.Text.Json.JsonException ex)
            {
                GD.PrintErr($"JSON parsing failed for {personaId}: {ex.Message}");
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
                return choices.Select(c => new NarrativeChoiceOption { Id = c.Id, Text = c.Label, Description = c.Description }).ToList();
            }
            catch (InvalidOperationException ex)
            {
                GD.PrintErr($"Failed to generate choices for {personaId}: {ex.Message}");
                return GetFallbackChoices();
            }
            catch (System.Text.Json.JsonException ex)
            {
                GD.PrintErr($"JSON parsing failed for {personaId}: {ex.Message}");
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
        /// Loads persona configuration from YAML file.
        /// </summary>
        /// <param name="personaId">The persona identifier.</param>
        /// <returns>The loaded persona configuration or null if not found.</returns>
        private static PersonaConfig? LoadPersonaConfig(string personaId)
        {
            try
            {
                var configPath = $"res://Source/Data/scenes/scene1_narrative/{personaId}.yaml";
                GD.Print($"Loading persona config from: {configPath}");
                if (!Godot.FileAccess.FileExists(configPath))
                {
                    GD.PrintErr($"Persona config not found: {configPath}");
                    return null;
                }

                Godot.FileAccess file = Godot.FileAccess.Open(configPath, Godot.FileAccess.ModeFlags.Read);
                var yamlText = file.GetAsText();
                file.Close();

                GD.Print($"YAML text length for {personaId}: {yamlText.Length}");

                // Use YamlDotNet to deserialize the YAML directly into C# objects
                var deserializer = new DeserializerBuilder().Build();
                var config = deserializer.Deserialize<PersonaConfig>(yamlText);

                GD.Print($"Deserialized config for {personaId}: OpeningLines count = {config?.OpeningLines.Count ?? 0}");
                return config;
            }
            catch (InvalidOperationException ex)
            {
                GD.PrintErr($"Failed to load persona config for {personaId}: {ex.Message}");
                return null;
            }
            catch (YamlDotNet.Core.YamlException ex)
            {
                GD.PrintErr($"YAML parsing error for {personaId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Fallback methods for when LLM generation fails
        /// </summary>
        /// <param name="personaId"></param>
        /// <returns></returns>
        private static string GetFallbackNarrative(string personaId)
        {
            var fallbacks = new Dictionary<string, string>
            {
                ["hero"] = "The hero's path calls to you, filled with light and shadow.",
                ["shadow"] = "The shadows whisper secrets that only you can hear.",
                ["ambition"] = "Ambition drives you forward, carving new paths through reality.",
            };

            return fallbacks.GetValueOrDefault(personaId, "The narrative continues...");
        }

        private static string GetFallbackOpeningLine(string personaId)
        {
            var fallbacks = new Dictionary<string, string>
            {
                ["hero"] = "A hero emerges from the darkness.",
                ["shadow"] = "The shadows remember what you forget.",
                ["ambition"] = "Ambition flows upward, defying gravity.",
            };

            return fallbacks.GetValueOrDefault(personaId, "Welcome to the spiral.");
        }

        private static List<NarrativeChoiceOption> GetFallbackChoices()
        {
            return new List<NarrativeChoiceOption>
            {
                new NarrativeChoiceOption { Id = "continue", Text = "Continue", Description = "Continue the journey" },
                new NarrativeChoiceOption { Id = "reflect", Text = "Reflect", Description = "Take a moment to reflect" },
                new NarrativeChoiceOption { Id = "question", Text = "Question", Description = "Ask a question" },
            };
        }

        private void InitializePersonas()
        {
            GD.Print("Initializing personas...");
            // Load persona configurations from YAML files
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
