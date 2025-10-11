namespace OmegaSpiral.Source.Scripts.UI.Dialogue
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Godot;
    using OmegaSpiral.Source.Scripts.Interfaces;
    using OmegaSpiral.Source.Scripts.Models;
    using OmegaSpiral.Source.Scripts.Services;

    /// <summary>
    /// Main controller for the narrative terminal that coordinates scene flow, user interaction, and visual presentation.
    /// This is a refactored version that delegates responsibilities to focused service and controller classes.
    /// </summary>
    public partial class NarrativeTerminalController : Control
    {
        /// <summary>
        /// Gets or sets a value indicating whether generated narrative should be supplied by the Dreamweaver system instead of static JSON.
        /// </summary>
        [Export]
        public bool UseDynamicNarrative { get; set; } = false;

        /// <summary>
        /// Gets or sets the number of characters per second used by the typewriter renderer when animating dialogue lines.
        /// </summary>
        [Export(PropertyHint.Range, "10,120,1")]
        public float TypewriterCharactersPerSecond { get; set; } = 40f;

        /// <summary>
        /// Gets or sets a value indicating whether the typewriter should bypass animation and render lines instantly (handy for tests).
        /// </summary>
        [Export]
        public bool ForceInstantRender { get; set; }

        private readonly PackedScene choiceButtonScene = GD.Load<PackedScene>("res://Source/UI/TerminalChoiceButton.tscn");

        // Services
        private INarrativeSceneService sceneService = default!;
        private ITypewriterService typewriterService = default!;

        // Controllers
        private NarrativeRenderer renderer = default!;
        private NarrativeInputHandler inputHandler = default!;
        private NarrativeEffectsController effectsController = default!;

        // Godot autoload singletons
        private SceneManager sceneManager = default!;
        private GameState gameState = default!;
        private NarratorEngine? narratorEngine;
        private DreamweaverSystem? dreamweaverSystem;

        // UI components
        private RichTextLabel outputLabel = default!;
        private ScrollContainer outputScroll = default!;
        private VBoxContainer choiceContainer = default!;
        private HBoxContainer inputRow = default!;
        private LineEdit inputField = default!;
        private Button submitButton = default!;
        private Label promptLabel = default!;
        private ColorRect pixelDissolveOverlay = default!;
        private ColorRect asciiStaticOverlay = default!;

        private readonly Dictionary<string, string> variables = new();
        private string lastGeneratedNarrative = string.Empty;

        /// <inheritdoc/>
        public override void _Ready()
        {
            this.InitializeUIComponents();
            this.InitializeServices();
            this.InitializeControllers();
            this.InitializeSingletons();
            this.ConnectDreamweaverSignals();

            this.choiceContainer.Visible = false;

            this.CallDeferred(nameof(this.StartNarrativeAsync));
        }

        /// <inheritdoc/>
        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event.IsActionPressed("ui_accept") || @event.IsActionPressed("ui_select"))
            {
                if (this.renderer.IsRendering)
                {
                    this.renderer.RequestSkip();
                }
            }
        }

        private void InitializeUIComponents()
        {
            this.outputScroll = this.GetNode<ScrollContainer>("%OutputScroll");
            this.outputLabel = this.GetNode<RichTextLabel>("%OutputLabel");
            this.choiceContainer = this.GetNode<VBoxContainer>("%ChoiceContainer");
            this.inputRow = this.GetNode<HBoxContainer>("%InputRow");
            this.inputField = this.GetNode<LineEdit>("%InputField");
            this.submitButton = this.GetNode<Button>("%SubmitButton");
            this.promptLabel = this.GetNode<Label>("%PromptLabel");
            this.pixelDissolveOverlay = this.GetNode<ColorRect>("%PixelDissolveOverlay");
            this.asciiStaticOverlay = this.GetNode<ColorRect>("%AsciiStaticOverlay");
        }

        private void InitializeServices()
        {
            this.typewriterService = new TypewriterService(
                this.TypewriterCharactersPerSecond,
                this.ForceInstantRender);

            this.sceneService = new NarrativeSceneService();
        }

        private void InitializeControllers()
        {
            this.renderer = new NarrativeRenderer();
            this.renderer.Initialize(this.outputLabel, this.outputScroll, this.typewriterService);
            this.AddChild(this.renderer);

            this.inputHandler = new NarrativeInputHandler();
            this.inputHandler.Initialize(this.inputField, this.submitButton, this.promptLabel, this.inputRow);
            this.AddChild(this.inputHandler);

            this.effectsController = new NarrativeEffectsController();
            this.effectsController.Initialize(this.pixelDissolveOverlay, this.asciiStaticOverlay);
            this.AddChild(this.effectsController);
        }

        private void InitializeSingletons()
        {
            this.sceneManager = this.GetNode<SceneManager>("/root/SceneManager");
            this.gameState = this.GetNode<GameState>("/root/GameState");
            this.narratorEngine = this.GetNodeOrNull<NarratorEngine>("/root/NarratorEngine");
            this.dreamweaverSystem = this.GetNodeOrNull<DreamweaverSystem>("/root/DreamweaverSystem");
        }

        private void ConnectDreamweaverSignals()
        {
            if (this.dreamweaverSystem != null)
            {
                this.dreamweaverSystem.Connect("NarrativeGenerated", new Callable(this, nameof(this.OnNarrativeGenerated)));
                this.dreamweaverSystem.Connect("GenerationError", new Callable(this, nameof(this.OnGenerationError)));
            }
        }

        private async void StartNarrativeAsync()
        {
            const string schemaPath = "res://docs/scenes/scene1-schema.json";

            if (!this.sceneService.LoadSceneSchema(schemaPath))
            {
                this.renderer.DisplayImmediate("[color=#ff5959]Unable to load scene schema. Please verify content files.[/color]");
                return;
            }

            await this.ExecuteSceneAsync();
        }

        private async Task ExecuteSceneAsync()
        {
            while (this.sceneService.HasMoreSteps())
            {
                SceneStep? step = this.sceneService.CurrentStep;
                if (step == null)
                {
                    break;
                }

                await this.ExecuteStepAsync(step);

                // Only advance automatically for non-branching steps
                if (string.IsNullOrEmpty(step.NextStep))
                {
                    this.sceneService.AdvanceToNextStep();
                }
            }

            await this.CompleteNarrativeSceneAsync();
        }

        private async Task ExecuteStepAsync(SceneStep step)
        {
            switch (step.Type)
            {
                case "dialogue":
                    await this.ExecuteDialogueStepAsync(step);
                    break;
                case "choice":
                    await this.ExecuteChoiceStepAsync(step);
                    break;
                case "input":
                    await this.ExecuteInputStepAsync(step);
                    break;
                case "effect":
                    await this.ExecuteEffectStepAsync(step);
                    break;
                default:
                    GD.PrintErr($"Unknown step type: {step.Type}");
                    break;
            }
        }

        private async Task ExecuteDialogueStepAsync(SceneStep step)
        {
            if (this.UseDynamicNarrative && this.dreamweaverSystem != null)
            {
                await this.ExecuteDynamicDialogueAsync(step);
            }
            else
            {
                foreach (string line in step.Lines)
                {
                    string processedLine = this.ProcessVariables(line);
                    await this.renderer.DisplayTextAsync(processedLine);
                }
            }

            if (step.Delay > 0)
            {
                await Task.Delay(step.Delay);
            }
        }

        private async Task ExecuteDynamicDialogueAsync(SceneStep step)
        {
            if (this.dreamweaverSystem == null)
            {
                return;
            }

            string stepId = step.Id;
            string[] contextLines = step.Lines.Select(line => this.ProcessVariables(line)).ToArray();

            string? cached = await this.dreamweaverSystem.LoadCachedNarrativeAsync(stepId, "omega", contextLines);
            if (!string.IsNullOrEmpty(cached))
            {
                await this.renderer.DisplayTextAsync(cached);
                return;
            }

            GD.Print($"Generating dynamic narrative for step: {stepId}");
            string generated = await this.dreamweaverSystem.GenerateNarrativeAsync(stepId, contextLines);
            this.lastGeneratedNarrative = generated;

            await this.renderer.DisplayTextAsync(generated);
        }

        private async Task ExecuteChoiceStepAsync(SceneStep step)
        {
            this.ClearChoices();

            if (!string.IsNullOrWhiteSpace(step.Prompt))
            {
                this.renderer.DisplayImmediate($"[b]{step.Prompt}[/b]");
            }

            foreach (var (option, index) in step.Options.Select((opt, idx) => (opt, idx)))
            {
                TerminalChoiceButton button = this.choiceButtonScene.Instantiate<TerminalChoiceButton>();
                button.Configure(option, index);
                button.Pressed += () => this.inputHandler.CompleteChoice(button.Option);
                this.choiceContainer.AddChild(button);
            }

            if (step.Options.Count == 0)
            {
                GD.PrintErr($"Choice step {step.Id} has no options.");
                this.renderer.DisplayImmediate("[color=#ff5959]No options available for this choice.[/color]");
                return;
            }

            this.choiceContainer.Visible = true;
            if (this.choiceContainer.GetChildCount() > 0 && this.choiceContainer.GetChild(0) is Control firstChoice)
            {
                firstChoice.GrabFocus();
            }

            string promptText = string.IsNullOrWhiteSpace(step.Prompt)
                ? "Select an option (number or text)"
                : step.Prompt!;

            ChoiceOption? selection = await this.inputHandler.AwaitChoiceAsync(step.Options, promptText);
            if (selection == null)
            {
                return;
            }

            await this.HandleChoiceSelectionAsync(step, selection);
        }

        private async Task ExecuteInputStepAsync(SceneStep step)
        {
            string prompt = string.IsNullOrWhiteSpace(step.Prompt) ? ">" : step.Prompt!;
            string input = await this.inputHandler.AwaitInputAsync(prompt);

            await this.HandleInputAsync(step, input);
        }

        private async Task ExecuteEffectStepAsync(SceneStep step)
        {
            this.inputHandler.HidePrompt();
            this.choiceContainer.Visible = false;

            string effect = step.Effect.ToLowerInvariant();
            int durationMs = Math.Max(step.Duration, 500);

            switch (effect)
            {
                case "pixel-dissolve":
                    await this.effectsController.PlayPixelDissolveAsync(durationMs);
                    break;
                case "ascii-static":
                    await this.effectsController.PlayAsciiStaticAsync(durationMs);
                    break;
                default:
                    GD.Print($"Executing placeholder effect: {effect}");
                    await Task.Delay(durationMs);
                    break;
            }
        }

        private async Task HandleChoiceSelectionAsync(SceneStep step, ChoiceOption selection)
        {
            string label = selection.Text ?? selection.Id ?? string.Empty;
            if (!string.IsNullOrEmpty(label))
            {
                this.renderer.DisplayImmediate($"> [ {label} ]");
                this.renderer.DisplayImmediate(string.Empty);
            }

            this.ClearChoices();
            this.choiceContainer.Visible = false;
            this.inputHandler.HidePrompt();

            this.gameState.SceneData[$"choice_{step.Id}"] = selection.Id ?? label;

            if (step.Id == "thread-confirm")
            {
                string threadLabel = selection.Text ?? selection.Id ?? string.Empty;
                this.variables["selectedThread"] = threadLabel;
                if (Enum.TryParse<DreamweaverThread>(threadLabel, true, out var dreamweaverThread))
                {
                    this.gameState.DreamweaverThread = dreamweaverThread;
                }
            }

            if (this.UseDynamicNarrative && this.dreamweaverSystem != null && step.Type == "choice")
            {
                _ = this.GenerateObserverCommentaryAsync(step.Id, selection.Text ?? selection.Id ?? string.Empty);
            }

            // Handle step navigation based on choice
            if (!string.IsNullOrEmpty(selection.NextStep))
            {
                this.NavigateToStep(selection.NextStep);
            }
            else if (!string.IsNullOrEmpty(step.NextStep))
            {
                this.NavigateToStep(step.NextStep);
            }
        }

        private Task HandleInputAsync(SceneStep step, string input)
        {
            this.renderer.DisplayImmediate($"> {input}");

            this.gameState.SceneData[$"input_{step.Id}"] = input;
            this.sceneService.SetVariable("playerName", input);
            this.gameState.PlayerName = input;

            this.inputHandler.HidePrompt();
            return Task.CompletedTask;
        }

        private void NavigateToStep(string targetStepId)
        {
            if (this.sceneService.CurrentSchema == null)
            {
                return;
            }

            for (int i = 0; i < this.sceneService.CurrentSchema.Steps.Count; i++)
            {
                if (this.sceneService.CurrentSchema.Steps[i].Id.Equals(targetStepId, StringComparison.OrdinalIgnoreCase))
                {
                    // Reset to start then advance to target
                    this.sceneService.ResetToStart();
                    for (int j = 0; j < i; j++)
                    {
                        this.sceneService.AdvanceToNextStep();
                    }

                    return;
                }
            }

            GD.PrintErr($"Target step not found: {targetStepId}");
        }

        private void ClearChoices()
        {
            foreach (Node child in this.choiceContainer.GetChildren())
            {
                child.QueueFree();
            }

            this.choiceContainer.Visible = false;
        }

        private string ProcessVariables(string text)
        {
            text = text.Replace("{selectedThread}", this.variables.GetValueOrDefault("selectedThread", string.Empty), StringComparison.Ordinal);
            text = text.Replace("{playerName}", this.variables.GetValueOrDefault("playerName", string.Empty), StringComparison.Ordinal);
            return text;
        }

        private async Task GenerateObserverCommentaryAsync(string stepId, string selectedChoiceText)
        {
            if (this.dreamweaverSystem == null)
            {
                return;
            }

            try
            {
                var commentary = await this.dreamweaverSystem.GenerateObserverCommentaryAsync(stepId, selectedChoiceText);
                foreach (KeyValuePair<string, string> entry in commentary)
                {
                    GD.Print($"Observer {entry.Key}: {entry.Value}");
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Failed to generate observer commentary: {ex.Message}");
            }
        }

        private async Task CompleteNarrativeSceneAsync()
        {
            await this.renderer.DisplayTextAsync("Moving to the next part of your journey...");

            this.sceneManager.UpdateCurrentScene(2);
            var timer = new Timer { WaitTime = 2.5f, OneShot = true };
            timer.Timeout += () => this.sceneManager.TransitionToScene("Scene2NethackSequence");
            this.AddChild(timer);
            timer.Start();
        }

        private void OnNarrativeGenerated(string personaId, string generatedText)
        {
            GD.Print($"Dreamweaver narrative generated for {personaId}: {generatedText}");
            this.lastGeneratedNarrative = generatedText;
        }

        private void OnGenerationError(string personaId, string errorMessage)
        {
            GD.PrintErr($"Dreamweaver generation error for {personaId}: {errorMessage}");
            this.UseDynamicNarrative = false;
        }
    }
}
