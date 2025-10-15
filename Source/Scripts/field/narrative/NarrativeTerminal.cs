// <copyright file="NarrativeTerminal.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Godot;
    using OmegaSpiral;
    using OmegaSpiral.Source.Scripts;
    using OmegaSpiral.Source.Scripts.Common;
    using OmegaSpiral.Source.Scripts.Field.Narrative;

    /// <summary>
    /// Presents the opening narrative terminal with a flexible prompt/choice system that content teams can extend via JSON.
    /// FUTURE: Will integrate with DreamweaverSystem for LLM-powered dynamic narrative (see ADR-0003).
    /// Integration points marked with // FUTURE: LLM_INTEGRATION comments.
    /// </summary>
    public partial class NarrativeTerminal : Control
    {
        private readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

        /// <summary>
        /// Optional Dialogic integration for enhanced dialogue presentation.
        /// When enabled, uses Dialogic's timeline system for dialogue UI while
        /// maintaining C# control over narrative logic and state.
        /// </summary>
        private DialogicIntegration? dialogicIntegration;

        private RichTextLabel outputLabel = default!;
        private LineEdit inputField = default!;
        private Button submitButton = default!;
        private Label promptLabel = default!;

        private SceneManager sceneManager = default!;
        private GameState gameState = default!;
        private NarratorEngine? narratorEngine;

        private DreamweaverSystem? dreamweaverSystem;

        private NarrativeSceneData sceneData = new();
        private PromptKind currentPrompt = PromptKind.None;
        private IReadOnlyList<DreamweaverChoice> threadChoices = Array.Empty<DreamweaverChoice>();
        private IReadOnlyList<ChoiceOption> activeChoices = Array.Empty<ChoiceOption>();
        private bool awaitingInput;
        private int currentBlockIndex;

        /// <summary>
        /// Dynamic narrative state
        /// </summary>
        private bool useDynamicNarrative;
        private string lastGeneratedNarrative = string.Empty;

        private enum PromptKind
        {
            None = 0,
            InitialChoice = 1,
            StoryChoice = 2,
            Freeform = 3,
            PlayerName = 4,
            Secret = 5,
        }

        /// <summary>
        /// Gets or sets a value indicating whether dynamic narrative generation should be used.
        /// FUTURE: LLM_INTEGRATION - Toggle for dynamic vs static narrative.
        /// When true and _dreamweaverSystem is available, use LLM responses.
        /// When false or _dreamweaverSystem is null, use static JSON (current behavior).
        /// </summary>
        [Export]
        public bool UseDynamicNarrative { get; set; } = false;

        /// <inheritdoc/>
        public override void _Ready()
        {
            this.outputLabel = this.GetNode<RichTextLabel>("%OutputLabel");
            this.inputField = this.GetNode<LineEdit>("%InputField");
            this.submitButton = this.GetNode<Button>("%SubmitButton");
            this.promptLabel = this.GetNode<Label>("%PromptLabel");

            this.submitButton.Pressed += this.OnSubmitPressed;
            this.inputField.TextSubmitted += _ => this.OnSubmitPressed();

            this.sceneManager = this.GetNode<SceneManager>("/root/SceneManager");
            this.gameState = this.GetNode<GameState>("/root/GameState");
            this.narratorEngine = this.GetNodeOrNull<NarratorEngine>("/root/NarratorEngine");

            // FUTURE: LLM_INTEGRATION - Connect to DreamweaverSystem when available
            this.dreamweaverSystem = this.GetNodeOrNull<DreamweaverSystem>("/root/DreamweaverSystem");
            if (this.dreamweaverSystem != null)
            {
                // Connect to signals for dynamic narrative updates
                this.dreamweaverSystem.Connect("NarrativeGenerated", new Callable(this, nameof(this.OnNarrativeGenerated)));
                this.dreamweaverSystem.Connect("GenerationError", new Callable(this, nameof(this.OnGenerationError)));
                GD.Print("NarrativeTerminal: DreamweaverSystem connected for dynamic narrative");
            }

            this.inputField.GrabFocus();
            this.CallDeferred(nameof(this.InitializeNarrativeAsync));
        }

        /// <summary>
        /// Signal handlers for DreamweaverSystem
        /// </summary>
        /// <param name="personaId">The ID of the persona that generated the narrative.</param>
        /// <param name="generatedText">The generated narrative text.</param>
        private void OnNarrativeGenerated(string personaId, string generatedText)
        {
            GD.Print($"Dreamweaver narrative generated for {personaId}: {generatedText}");

            // Store the generated text for use in narrative display
            this.lastGeneratedNarrative = generatedText;
        }

        private void OnGenerationError(string personaId, string errorMessage)
        {
            GD.PrintErr($"Dreamweaver generation error for {personaId}: {errorMessage}");

            // Fall back to static narrative when LLM fails
            this.useDynamicNarrative = false;
        }

        private async void InitializeNarrativeAsync()
        {
            // Initialize Dialogic integration if available
            this.dialogicIntegration = this.GetNodeOrNull<DialogicIntegration>("/root/DialogicIntegration");
            if (this.dialogicIntegration != null)
            {
                GD.Print("Dialogic integration enabled for enhanced dialogue presentation");
            }

            if (!this.TryLoadSceneData())
            {
                this.DisplayImmediate("[color=#ff5959]Unable to load terminal narrative. Please verify content files.[/color]");
                return;
            }

            await this.DisplayOpeningAsync().ConfigureAwait(false);
            this.PresentInitialChoice();
        }

        private bool TryLoadSceneData()
        {
            string basePath = "res://Source/Data/scenes/scene1_narrative";
            string threadKey = this.gameState.DreamweaverThread.ToString().ToUpperInvariant();
            var candidates = new[] { threadKey, "hero", "shadow", "ambition" };

            foreach (string candidate in candidates)
            {
                string path = $"{basePath}/{candidate}.json";
                if (!Godot.FileAccess.FileExists(path))
                {
                    continue;
                }

                try
                {
                    string json = Godot.FileAccess.GetFileAsString(path);
                    NarrativeSceneData? data = JsonSerializer.Deserialize<NarrativeSceneData>(json, this.jsonOptions);
                    if (data == null)
                    {
                        continue;
                    }

                    this.NormalizeNarrativeData(data);
                    this.sceneData = data;
                    return true;
                }
                catch (JsonException ex)
                {
                    GD.PrintErr($"Failed to parse narrative data at {path}: {ex.Message}");
                }
                catch (Exception ex) when (ex is not JsonException)
                {
                    GD.PrintErr($"Unexpected error loading narrative data at {path}: {ex.Message}");
                    throw;
                }
            }

            GD.PrintErr("NarrativeTerminal: no valid data files found. Expected hero/shadow/ambition variants.");
            return false;
        }

        private void NormalizeNarrativeData(NarrativeSceneData data)
        {
            data.OpeningLines ??= new List<string>();
            data.StoryBlocks ??= new List<StoryBlock>();
            data.SecretQuestion ??= new SecretQuestion { Options = new List<string>() };

            if (data.InitialChoice != null)
            {
                data.InitialChoice.Options = new List<DreamweaverChoice>();
                foreach (DreamweaverChoice option in data.InitialChoice.Options)
                {
                    if (!Enum.TryParse(option.Id, true, out DreamweaverThread parsedThread))
                    {
                        parsedThread = DreamweaverThread.Hero;
                    }

                    option.Thread = parsedThread;
                }

                this.threadChoices = data.InitialChoice.Options;
            }

            foreach (StoryBlock block in data.StoryBlocks)
            {
                block.Paragraphs ??= new List<string>();
                if (block.Choices == null)
                {
                    block.Choices = new List<ChoiceOption>();
                }
            }
        }

        private async Task DisplayOpeningAsync()
        {
            if (this.UseDynamicNarrative && this.dreamweaverSystem != null)
            {
                // Use dynamic narrative generation
                var personaId = this.gameState.DreamweaverThread.ToString().ToUpperInvariant();
                var openingLine = await this.dreamweaverSystem.GetOpeningLineAsync(personaId).ConfigureAwait(false);
                await this.DisplayTextWithTypewriterAsync(openingLine).ConfigureAwait(false);
            }
            else
            {
                // Use static JSON narrative (current behavior)
                foreach (string line in this.sceneData.OpeningLines)
                {
                    await this.DisplayTextWithTypewriterAsync(line).ConfigureAwait(false);
                }
            }
        }

        private async void PresentInitialChoice()
        {
            if (this.sceneData.InitialChoice == null || this.threadChoices.Count == 0)
            {
                this.currentBlockIndex = 0;
                this.PresentStoryBlock();
                return;
            }

            if (this.UseDynamicNarrative && this.dreamweaverSystem != null)
            {
                // Use dynamic choice generation
                var personaId = this.gameState.DreamweaverThread.ToString().ToUpperInvariant();
                var dynamicChoices = await this.dreamweaverSystem.GenerateChoicesAsync(personaId, "initial choice").ConfigureAwait(false);

                this.DisplayImmediate($"[b]{this.sceneData.InitialChoice.Prompt}[/b]");

                for (int i = 0; i < dynamicChoices.Count; i++)
                {
                    var choice = dynamicChoices[i];
                    this.DisplayImmediate($"  {i + 1}. {choice.Text} — {choice.Description}");
                }

                this.activeChoices = Array.Empty<ChoiceOption>();
            }
            else
            {
                // Use static JSON choices (current behavior)
                this.DisplayImmediate($"[b]{this.sceneData.InitialChoice.Prompt}[/b]");

                for (int i = 0; i < this.threadChoices.Count; i++)
                {
                    DreamweaverChoice option = this.threadChoices[i];
                    string label = option.Text ?? option.Id!;
                    this.DisplayImmediate($"  {i + 1}. {label} — {option.Description}");
                }

                this.activeChoices = Array.Empty<ChoiceOption>();
            }

            this.ConfigurePrompt(PromptKind.InitialChoice, "Choose your story thread (ex: 1 or hero)");
        }

        private void PresentStoryBlock()
        {
            if (this.currentBlockIndex < 0 || this.currentBlockIndex >= this.sceneData.StoryBlocks.Count)
            {
                this.PromptForName();
                return;
            }

            var block = this.sceneData.StoryBlocks[this.currentBlockIndex];

            _ = this.DisplayStoryBlockAsync(block);
        }

        private async Task DisplayStoryBlockAsync(StoryBlock block)
        {
            foreach (string paragraph in block.Paragraphs)
            {
                await this.DisplayTextWithTypewriterAsync(paragraph).ConfigureAwait(false);
            }

            if (!string.IsNullOrEmpty(block.Question))
            {
                this.DisplayImmediate($"[b]{block.Question}[/b]");

                if (block.Choices.Count > 0)
                {
                    this.activeChoices = (IReadOnlyList<ChoiceOption>) block.Choices;
                    for (int i = 0; i < block.Choices.Count; i++)
                    {
                        this.DisplayImmediate($"  {i + 1}. {block.Choices[i].Text}");
                    }

                    this.ConfigurePrompt(PromptKind.StoryChoice, "Select an option (number or text)");
                }
                else
                {
                    this.activeChoices = Array.Empty<ChoiceOption>();
                    this.ConfigurePrompt(PromptKind.Freeform, "Enter your response");
                }
            }
            else
            {
                this.currentBlockIndex++;
                this.PresentStoryBlock();
            }
        }

        private void PromptForName()
        {
            string prompt = string.IsNullOrWhiteSpace(this.sceneData.NamePrompt)
                ? "What name should the terminal record?"
                : this.sceneData.NamePrompt;

            this.DisplayImmediate($"[b]{prompt}[/b]");
            this.ConfigurePrompt(PromptKind.PlayerName, "Enter your name");
        }

        private void PromptForSecret()
        {
            if (this.sceneData.SecretQuestion == null)
            {
                this.CompleteNarrativeSceneAsync();
                return;
            }

            this.DisplayImmediate($"[b]{this.sceneData.SecretQuestion.Prompt}[/b]");

            if (this.sceneData.SecretQuestion.Options.Count > 0)
            {
                for (int i = 0; i < this.sceneData.SecretQuestion.Options.Count; i++)
                {
                    this.DisplayImmediate($"  {i + 1}. {this.sceneData.SecretQuestion.Options[i]}");
                }
            }

            this.ConfigurePrompt(PromptKind.Secret, "Share your secret (number or text)");
        }

        private void ConfigurePrompt(PromptKind kind, string placeholder)
        {
            this.currentPrompt = kind;
            this.awaitingInput = true;
            this.inputField.PlaceholderText = placeholder;
            this.promptLabel.Text = placeholder;
            this.inputField.Text = string.Empty;
            this.inputField.GrabFocus();
        }

        private void OnSubmitPressed()
        {
            if (!this.awaitingInput)
            {
                return;
            }

            string rawInput = this.inputField.Text.Trim();
            this.inputField.Text = string.Empty;

            if (string.IsNullOrEmpty(rawInput))
            {
                return;
            }

            switch (this.currentPrompt)
            {
                case PromptKind.InitialChoice:
                    this.HandleThreadSelection(rawInput);
                    break;
                case PromptKind.StoryChoice:
                    this.HandleStoryChoice(rawInput);
                    break;
                case PromptKind.Freeform:
                    this.HandleFreeformResponse(rawInput);
                    break;
                case PromptKind.PlayerName:
                    this.HandlePlayerName(rawInput);
                    break;
                case PromptKind.Secret:
                    this.HandleSecret(rawInput);
                    break;
            }
        }

        private void HandleThreadSelection(string input)
        {
            DreamweaverChoice? choice = this.ResolveThreadChoice(input);
            if (choice is null)
            {
                this.DisplayImmediate("[color=#ffae42]Please choose a valid thread (number or hero/shadow/ambition).[/color]");
                return;
            }

            this.sceneManager.SetDreamweaverThread(choice.Id!);
            this.narratorEngine?.AddDialogue($"Thread locked: {choice.Text}");
            this.DisplayImmediate($"You lean toward the {choice.Text} path: {choice.Description}");

            this.gameState.DreamweaverThread = choice.Thread;

            this.awaitingInput = false;
            this.currentPrompt = PromptKind.None;
            this.currentBlockIndex = 0;

            this.PresentStoryBlock();
        }

        private DreamweaverChoice? ResolveThreadChoice(string input)
        {
            if (this.threadChoices.Count == 0)
            {
                return null;
            }

            if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int index))
            {
                index -= 1;
                if (index >= 0 && index < this.threadChoices.Count)
                {
                    return this.threadChoices[index];
                }
            }

            foreach (DreamweaverChoice option in this.threadChoices)
            {
                if ((option.Id != null && option.Id.Equals(input, StringComparison.OrdinalIgnoreCase)) ||
                    (option.Text != null && option.Text.Equals(input, StringComparison.OrdinalIgnoreCase)))
                {
                    return option;
                }
            }

            return null;
        }

        private void HandleStoryChoice(string input)
        {
            ChoiceOption? selection = this.ResolveChoiceOption(input);
            if (selection == null)
            {
                this.DisplayImmediate("[color=#ffae42]That option is unavailable. Try the listed number or text.[/color]");
                return;
            }

            this.awaitingInput = false;
            this.currentPrompt = PromptKind.None;

            int nextBlock = Mathf.Clamp(selection.NextBlock, 0, this.sceneData.StoryBlocks.Count);
            if (nextBlock == this.currentBlockIndex)
            {
                this.currentBlockIndex++;
            }
            else
            {
                this.currentBlockIndex = nextBlock;
            }

            this.PresentStoryBlock();
        }

        private ChoiceOption? ResolveChoiceOption(string input)
        {
            if (this.activeChoices.Count == 0)
            {
                return null;
            }

            if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int index))
            {
                index -= 1;
                if (index >= 0 && index < this.activeChoices.Count)
                {
                    return this.activeChoices[index];
                }
            }

            foreach (ChoiceOption option in this.activeChoices)
            {
                if (option.Text != null && option.Text.Equals(input, StringComparison.OrdinalIgnoreCase))
                {
                    return option;
                }
            }

            return null;
        }

        private void HandleFreeformResponse(string input)
        {
            this.RecordSceneResponse($"block-{this.currentBlockIndex}-response", input);
            this.awaitingInput = false;
            this.currentPrompt = PromptKind.None;
            this.currentBlockIndex++;
            this.PresentStoryBlock();
        }

        private void HandlePlayerName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                this.DisplayImmediate("[color=#ff5959]Please provide a name for the archives.[/color]");
                return;
            }

            this.sceneManager.SetPlayerName(input);
            this.DisplayImmediate($"Identity confirmed: [b]{input}[/b].");

            this.awaitingInput = false;
            this.currentPrompt = PromptKind.None;

            this.PromptForSecret();
        }

        private void HandleSecret(string input)
        {
            if (this.sceneData.SecretQuestion != null && this.sceneData.SecretQuestion.Options.Count > 0)
            {
                if (int.TryParse(input, out int index))
                {
                    index -= 1;
                    if (index >= 0 && index < this.sceneData.SecretQuestion.Options.Count)
                    {
                        input = this.sceneData.SecretQuestion.Options[index];
                    }
                }
            }

            this.RecordSceneResponse("secret", input);
            this.DisplayImmediate("A fragment has been secured in the archive.");

            this.awaitingInput = false;
            this.currentPrompt = PromptKind.None;

            this.CompleteNarrativeSceneAsync();
        }

        private void RecordSceneResponse(string key, string value)
        {
            string compositeKey = $"scene1_narrative.{key}";
            if (this.gameState.SceneData.ContainsKey(compositeKey))
            {
                this.gameState.SceneData[compositeKey] = value;
            }
            else
            {
                this.gameState.SceneData.Add(compositeKey, value);
            }
        }

        private async void CompleteNarrativeSceneAsync()
        {
            if (!string.IsNullOrWhiteSpace(this.sceneData.ExitLine))
            {
                await this.DisplayTextWithTypewriterAsync(this.sceneData.ExitLine).ConfigureAwait(false);
            }

            await this.DisplayTextWithTypewriterAsync("Moving to the next part of your journey...").ConfigureAwait(false);

            this.sceneManager.UpdateCurrentScene(2);
            var timer = new Godot.Timer { WaitTime = 2.5f, OneShot = true };
            timer.Timeout += () =>
            {
                this.sceneManager.TransitionToScene("Scene2NethackSequence");
                timer.Dispose();
            };
            this.AddChild(timer);
            timer.Start();
            timer.Dispose();
        }

        private void DisplayImmediate(string text)
        {
            if (this.outputLabel != null)
            {
                this.outputLabel.AppendText(text + "\n");
            }
            else
            {
                GD.Print(text);
            }
        }

        private async Task DisplayTextWithTypewriterAsync(string text)
        {
            this.outputLabel.AppendText("\n");
            foreach (char character in text)
            {
                this.outputLabel.AppendText(character.ToString());
                await this.ToSignal(this.GetTree().CreateTimer(0.025f), Godot.Timer.SignalName.Timeout);
            }

            this.outputLabel.AppendText("\n");
        }

        // ============================================================================
        // FUTURE: LLM_INTEGRATION - Dreamweaver Consultation Methods
        // ============================================================================
        // These methods will be implemented when DreamweaverSystem is integrated
        // See ADR-0003: docs/adr/adr-0003-nobodywho-llm-integration.md
        // ============================================================================

        // FUTURE: LLM_INTEGRATION - Dreamweaver Consultation Methods
        // ============================================================================
        // These methods will be implemented when DreamweaverSystem is integrated
        // See ADR-0003: docs/adr/adr-0003-nobodywho-llm-integration.md
        // ============================================================================

        // private void ConsultDreamweavers(string situation)
        // {
        //     if (_dreamweaverSystem != null && UseDynamicNarrative)
        //     {
        //         // Use LLM-powered Dreamweavers for dynamic narrative
        //         _dreamweaverSystem.Call("ConsultAllDreamweavers", situation);
        //     }
        //     else
        //     {
        //         // Fallback to static JSON narrative (current behavior)
        //         // Continue with existing story block display logic
        //     }
        // }

        // private void OnDreamweaversConsultation(Godot.Collections.Dictionary consultations)
        // {
        //     // Parse JSON responses from each Dreamweaver
        //     // string heroJson = consultations["hero"].AsString();
        //     // string shadowJson = consultations["shadow"].AsString();
        //     // string ambitionJson = consultations["ambition"].AsString();
        //     // string omegaJson = consultations["omega"].AsString();
        //
        //     // Display formatted Dreamweaver guidance in terminal
        //     // DisplayDreamweaverConsultation(heroJson, shadowJson, ambitionJson, omegaJson);
        //
        //     // Update game state based on player's alignment with each Dreamweaver
        //     // Continue narrative flow
        // }

        // private void DisplayDreamweaverConsultation(string heroJson, string shadowJson, string ambitionJson, string omegaJson)
        // {
        //     // Parse each JSON response according to schema defined in ADR-0003:
        //     // Hero: {advice, challenge, moral}
        //     // Shadow: {whisper, secret, cost}
        //     // Ambition: {strategy, goal, reward}
        //     // Omega: {narration, choice_context, consequence}
        // }
    }
}
