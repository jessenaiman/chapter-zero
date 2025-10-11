namespace OmegaSpiral.Source.Scripts
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Godot;

    /// <summary>
    /// Presents the opening narrative terminal with layered CRT styling and interactive flow.
    /// Loads JSON scene schemas, renders lines with a typewriter effect, surfaces clickable
    /// choices, and triggers dissolve/static overlays to mirror the NobodyWho React prototype.
    /// </summary>
    public partial class NarrativeTerminal : Control
    {
        private enum PromptKind
        {
            None,
            StoryChoice,
            PlayerName,
            Freeform,
        }

        private readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

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

        private RichTextLabel outputLabel = default!;
        private ScrollContainer outputScroll = default!;
        private VBoxContainer choiceContainer = default!;
        private HBoxContainer inputRow = default!;
        private LineEdit inputField = default!;
        private Button submitButton = default!;
        private Label promptLabel = default!;
        private ColorRect pixelDissolveOverlay = default!;
        private ColorRect asciiStaticOverlay = default!;

        private readonly PackedScene choiceButtonScene = GD.Load<PackedScene>("res://Source/UI/TerminalChoiceButton.tscn");

        private SceneManager sceneManager = default!;
        private GameState gameState = default!;
        private NarratorEngine? narratorEngine;
        private DreamweaverSystem? dreamweaverSystem;

        private SceneSchema sceneSchema = new();
        private SceneStep? currentStep;
        private int currentStepIndex;
        private readonly Dictionary<string, string> variables = new();

        private readonly List<ChoiceOption> activeChoices = new();
        private PromptKind currentPrompt = PromptKind.None;
        private bool awaitingInput;
        private bool isTypewriterRunning;
        private bool skipTypewriterRequested;
        private TaskCompletionSource<ChoiceOption?>? choiceCompletion;
        private TaskCompletionSource<string>? inputCompletion;
        private string lastGeneratedNarrative = string.Empty;

        /// <inheritdoc/>
        public override void _Ready()
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

            this.submitButton.Pressed += this.OnSubmitPressed;
            this.inputField.TextSubmitted += _ => this.OnSubmitPressed();

            this.sceneManager = this.GetNode<SceneManager>("/root/SceneManager");
            this.gameState = this.GetNode<GameState>("/root/GameState");
            this.narratorEngine = this.GetNodeOrNull<NarratorEngine>("/root/NarratorEngine");
            this.dreamweaverSystem = this.GetNodeOrNull<DreamweaverSystem>("/root/DreamweaverSystem");

            if (this.dreamweaverSystem != null)
            {
                this.dreamweaverSystem.Connect("NarrativeGenerated", new Callable(this, nameof(this.OnNarrativeGenerated)));
                this.dreamweaverSystem.Connect("GenerationError", new Callable(this, nameof(this.OnGenerationError)));
            }

            this.HidePrompt();
            this.choiceContainer.Visible = false;
            this.pixelDissolveOverlay.Visible = false;
            this.asciiStaticOverlay.Visible = false;

            this.CallDeferred(nameof(this.InitializeNarrativeAsync));
        }

        /// <inheritdoc/>
        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event.IsActionPressed("ui_accept") || @event.IsActionPressed("ui_select"))
            {
                if (this.isTypewriterRunning)
                {
                    this.skipTypewriterRequested = true;
                }
            }
        }

        // Dreamweaver callbacks ----------------------------------------------------

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

        // Initialization ----------------------------------------------------------

        private async void InitializeNarrativeAsync()
        {
            if (!this.TryLoadSceneSchema())
            {
                this.DisplayImmediate("[color=#ff5959]Unable to load scene schema. Please verify content files.[/color]");
                return;
            }

            this.currentStepIndex = 0;
            await this.ExecuteCurrentStepAsync();
        }

        private bool TryLoadSceneSchema()
        {
            const string schemaPath = "res://docs/scenes/scene1-schema.json";
            if (!FileAccess.FileExists(schemaPath))
            {
                GD.PrintErr($"Scene schema not found: {schemaPath}");
                return false;
            }

            try
            {
                string json = FileAccess.GetFileAsString(schemaPath);
                SceneSchema? schema = JsonSerializer.Deserialize<SceneSchema>(json, this.jsonOptions);
                if (schema == null)
                {
                    GD.PrintErr("Scene schema parsed as null.");
                    return false;
                }

                this.sceneSchema = schema;
                return true;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Failed to parse scene schema: {ex.Message}");
                return false;
            }
        }

        // Step execution ----------------------------------------------------------

        private async Task ExecuteCurrentStepAsync()
        {
            if (this.currentStepIndex >= this.sceneSchema.Steps.Count)
            {
                await this.CompleteNarrativeSceneAsync();
                return;
            }

            this.currentStep = this.sceneSchema.Steps[this.currentStepIndex];

            switch (this.currentStep.Type)
            {
                case "dialogue":
                    await this.ExecuteDialogueStepAsync();
                    break;
                case "choice":
                    await this.ExecuteChoiceStepAsync();
                    break;
                case "input":
                    await this.ExecuteInputStepAsync();
                    break;
                case "effect":
                    await this.ExecuteEffectStepAsync();
                    break;
                default:
                    GD.PrintErr($"Unknown step type: {this.currentStep.Type}");
                    this.AdvanceToNextStep(this.currentStep.NextStep);
                    break;
            }
        }

        private async Task ExecuteDialogueStepAsync()
        {
            if (this.currentStep == null)
            {
                return;
            }

            if (this.UseDynamicNarrative && this.dreamweaverSystem != null)
            {
                await this.ExecuteDynamicDialogueAsync();
            }
            else
            {
                foreach (string line in this.currentStep.Lines)
                {
                    string processedLine = this.ProcessVariables(line);
                    await this.DisplayTextWithTypewriterAsync(processedLine);
                }
            }

            if (this.currentStep.Delay > 0)
            {
                await Task.Delay(this.currentStep.Delay);
            }

            this.AdvanceToNextStep(this.currentStep.NextStep);
        }

        private async Task ExecuteDynamicDialogueAsync()
        {
            if (this.currentStep == null || this.dreamweaverSystem == null)
            {
                return;
            }

            string stepId = this.currentStep.Id;
            string[] contextLines = this.currentStep.Lines.Select(line => this.ProcessVariables(line)).ToArray();

            string? cached = await this.dreamweaverSystem.LoadCachedNarrativeAsync(stepId, "omega", contextLines);
            if (!string.IsNullOrEmpty(cached))
            {
                await this.DisplayTextWithTypewriterAsync(cached);
                return;
            }

            GD.Print($"Generating dynamic narrative for step: {stepId}");
            string generated = await this.dreamweaverSystem.GenerateNarrativeAsync(stepId, contextLines);
            this.lastGeneratedNarrative = generated;

            await this.DisplayTextWithTypewriterAsync(generated);
        }

        private async Task ExecuteChoiceStepAsync()
        {
            if (this.currentStep == null)
            {
                return;
            }

            this.ClearChoices();
            this.activeChoices.Clear();

            if (!string.IsNullOrWhiteSpace(this.currentStep.Prompt))
            {
                this.DisplayImmediate($"[b]{this.currentStep.Prompt}[/b]");
            }

            for (int i = 0; i < this.currentStep.Options.Count; i++)
            {
                ChoiceOption option = this.currentStep.Options[i];
                this.activeChoices.Add(option);

                TerminalChoiceButton button = this.choiceButtonScene.Instantiate<TerminalChoiceButton>();
                button.Configure(option, i);
                button.Pressed += () => this.OnChoiceButtonPressed(button.Option);
                this.choiceContainer.AddChild(button);
            }

            if (this.activeChoices.Count == 0)
            {
                GD.PrintErr($"Choice step {this.currentStep.Id} has no options.");
                this.DisplayImmediate("[color=#ff5959]No options available for this choice.[/color]");
                this.AdvanceToNextStep(this.currentStep.NextStep);
                return;
            }

            this.choiceContainer.Visible = true;
            if (this.choiceContainer.GetChildCount() > 0 && this.choiceContainer.GetChild(0) is Control firstChoice)
            {
                firstChoice.GrabFocus();
            }

            string promptText = string.IsNullOrWhiteSpace(this.currentStep.Prompt)
                ? "Select an option (number or text)"
                : this.currentStep.Prompt!;
            this.ConfigurePrompt(PromptKind.StoryChoice, promptText);

            this.choiceCompletion = new TaskCompletionSource<ChoiceOption?>();
            ChoiceOption? selection = await this.choiceCompletion.Task;
            this.choiceCompletion = null;

            if (selection == null)
            {
                return;
            }

            await this.HandleChoiceSelectionAsync(selection);
        }

        private async Task ExecuteInputStepAsync()
        {
            if (this.currentStep == null)
            {
                return;
            }

            string prompt = string.IsNullOrWhiteSpace(this.currentStep.Prompt) ? ">" : this.currentStep.Prompt!;
            this.ConfigurePrompt(PromptKind.PlayerName, prompt);

            this.inputCompletion = new TaskCompletionSource<string>();
            string input = await this.inputCompletion.Task;
            this.inputCompletion = null;

            await this.HandleInputAsync(input);
        }

        private async Task ExecuteEffectStepAsync()
        {
            if (this.currentStep == null)
            {
                return;
            }

            this.HidePrompt();
            this.choiceContainer.Visible = false;

            string effect = this.currentStep.Effect.ToLowerInvariant();
            int durationMs = Math.Max(this.currentStep.Duration, 500);

            switch (effect)
            {
                case "pixel-dissolve":
                    await this.PlayPixelDissolveAsync(durationMs);
                    break;
                case "ascii-static":
                    await this.PlayAsciiStaticAsync(durationMs);
                    break;
                default:
                    GD.Print($"Executing placeholder effect: {effect}");
                    await Task.Delay(durationMs);
                    break;
            }

            this.AdvanceToNextStep(this.currentStep.NextStep);
        }

        // Choice/input helpers ---------------------------------------------------

        private void OnChoiceButtonPressed(ChoiceOption? option)
        {
            if (option == null || this.choiceCompletion == null)
            {
                return;
            }

            this.choiceCompletion.TrySetResult(option);
        }

        private ChoiceOption? ResolveChoiceOption(string input, List<ChoiceOption> options)
        {
            if (options.Count == 0)
            {
                return null;
            }

            if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int index))
            {
                index -= 1;
                if (index >= 0 && index < options.Count)
                {
                    return options[index];
                }
            }

            foreach (ChoiceOption option in options)
            {
                if ((option.Id != null && option.Id.Equals(input, StringComparison.OrdinalIgnoreCase)) ||
                    (option.Text != null && option.Text.Equals(input, StringComparison.OrdinalIgnoreCase)))
                {
                    return option;
                }
            }

            return null;
        }

        private Task HandleChoiceSelectionAsync(ChoiceOption selection)
        {
            string label = selection.Text ?? selection.Id ?? string.Empty;
            if (!string.IsNullOrEmpty(label))
            {
                this.DisplayImmediate($"> [ {label} ]");
                this.DisplayImmediate(string.Empty);
            }

            this.activeChoices.Clear();
            this.ClearChoices();
            this.choiceContainer.Visible = false;
            this.HidePrompt();

            if (this.currentStep != null)
            {
                this.gameState.SceneData[$"choice_{this.currentStep.Id}"] = selection.Id ?? label;
            }

            if (this.currentStep?.Id == "thread-confirm")
            {
                string threadLabel = selection.Text ?? selection.Id ?? string.Empty;
                this.variables["selectedThread"] = threadLabel;
                if (Enum.TryParse<DreamweaverThread>(threadLabel, true, out var dreamweaverThread))
                {
                    this.gameState.DreamweaverThread = dreamweaverThread;
                }
            }

            if (this.UseDynamicNarrative && this.dreamweaverSystem != null && this.currentStep?.Type == "choice")
            {
                _ = this.GenerateObserverCommentaryAsync(selection.Text ?? selection.Id ?? string.Empty);
            }

            string? next = selection.NextStep ?? this.currentStep?.NextStep;
            this.AdvanceToNextStep(next);
            return Task.CompletedTask;
        }

        private Task HandleInputAsync(string input)
        {
            this.DisplayImmediate($"> {input}");

            if (this.currentStep != null)
            {
                this.gameState.SceneData[$"input_{this.currentStep.Id}"] = input;
            }

            this.variables["playerName"] = input;
            this.gameState.PlayerName = input;

            this.HidePrompt();
            this.AdvanceToNextStep(this.currentStep?.NextStep);
            return Task.CompletedTask;
        }

        private void ClearChoices()
        {
            foreach (Node child in this.choiceContainer.GetChildren())
            {
                child.QueueFree();
            }

            this.choiceContainer.Visible = false;
        }

        private void HidePrompt()
        {
            this.awaitingInput = false;
            this.currentPrompt = PromptKind.None;
            this.promptLabel.Visible = false;
            this.inputRow.Visible = false;
            this.submitButton.Visible = false;
            this.inputField.Text = string.Empty;
        }

        private void ConfigurePrompt(PromptKind promptKind, string promptText)
        {
            this.currentPrompt = promptKind;
            this.awaitingInput = promptKind != PromptKind.None;

            this.promptLabel.Visible = true;
            this.promptLabel.Text = promptText;

            bool needsInput = promptKind is PromptKind.PlayerName or PromptKind.Freeform or PromptKind.StoryChoice;
            this.inputRow.Visible = needsInput;
            this.submitButton.Visible = needsInput;
            this.inputField.Visible = needsInput;

            if (needsInput)
            {
                this.inputField.Text = string.Empty;
                this.inputField.PlaceholderText = promptKind switch
                {
                    PromptKind.PlayerName => "Type your response…",
                    PromptKind.StoryChoice => "Type number or choice label…",
                    _ => ">",
                };

                this.inputField.GrabFocus();
            }
        }

        // Effect helpers ---------------------------------------------------------

        private async Task PlayPixelDissolveAsync(int durationMs)
        {
            if (this.pixelDissolveOverlay.Material is ShaderMaterial material)
            {
                this.pixelDissolveOverlay.Visible = true;
                material.SetShaderParameter("progress", 0f);

                Tween tween = this.CreateTween();
                tween.TweenProperty(material, "shader_parameter/progress", 1f, durationMs / 1000f);
                await ToSignal(tween, Tween.SignalName.Finished);

                material.SetShaderParameter("progress", 0f);
                this.pixelDissolveOverlay.Visible = false;
            }
            else
            {
                await Task.Delay(durationMs);
            }
        }

        private async Task PlayAsciiStaticAsync(int durationMs)
        {
            this.asciiStaticOverlay.Visible = true;
            await Task.Delay(durationMs);
            this.asciiStaticOverlay.Visible = false;
        }

        // Advance & prompt handling -----------------------------------------------

        private void AdvanceToNextStep(string? nextStepOverride)
        {
            if (!string.IsNullOrEmpty(nextStepOverride))
            {
                int index = this.sceneSchema.Steps.FindIndex(s => s.Id.Equals(nextStepOverride, StringComparison.OrdinalIgnoreCase));
                this.currentStepIndex = index >= 0 ? index : this.currentStepIndex + 1;
            }
            else
            {
                this.currentStepIndex++;
            }

            this.CallDeferred(nameof(this.ExecuteCurrentStepAsync));
        }

        private string ProcessVariables(string text)
        {
            text = text.Replace("{selectedThread}", this.variables.GetValueOrDefault("selectedThread", string.Empty), StringComparison.Ordinal);
            text = text.Replace("{playerName}", this.variables.GetValueOrDefault("playerName", string.Empty), StringComparison.Ordinal);
            return text;
        }

        private void OnSubmitPressed()
        {
            if (!this.awaitingInput)
            {
                return;
            }

            string rawInput = this.inputField.Text.Trim();
            if (string.IsNullOrEmpty(rawInput))
            {
                return;
            }

            this.inputField.Text = string.Empty;

            switch (this.currentPrompt)
            {
                case PromptKind.StoryChoice:
                    ChoiceOption? choice = this.ResolveChoiceOption(rawInput, this.activeChoices);
                    if (choice != null)
                    {
                        this.choiceCompletion?.TrySetResult(choice);
                    }
                    else
                    {
                        this.DisplayImmediate("[color=#ffae42]Please choose a valid option (number or label).[/color]");
                    }

                    break;

                case PromptKind.PlayerName:
                case PromptKind.Freeform:
                    this.inputCompletion?.TrySetResult(rawInput);
                    break;
            }
        }

        private async Task GenerateObserverCommentaryAsync(string selectedChoiceText)
        {
            if (this.currentStep == null || this.dreamweaverSystem == null)
            {
                return;
            }

            try
            {
                var commentary = await this.dreamweaverSystem.GenerateObserverCommentaryAsync(this.currentStep.Id, selectedChoiceText);
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
            await this.DisplayTextWithTypewriterAsync("Moving to the next part of your journey...").ConfigureAwait(false);

            this.sceneManager.UpdateCurrentScene(2);
            var timer = new Timer { WaitTime = 2.5f, OneShot = true };
            timer.Timeout += () => this.sceneManager.TransitionToScene("Scene2NethackSequence");
            this.AddChild(timer);
            timer.Start();
        }

        private void DisplayImmediate(string text)
        {
            if (this.outputLabel != null)
            {
                this.outputLabel.AppendText(string.IsNullOrEmpty(text) ? "\n" : $"{text}\n");
                this.ScrollToBottom();
            }
            else
            {
                GD.Print(text);
            }
        }

        private async Task DisplayTextWithTypewriterAsync(string text)
        {
            string line = text ?? string.Empty;

            if (string.IsNullOrEmpty(line))
            {
                this.DisplayImmediate(string.Empty);
                await Task.CompletedTask;
                return;
            }

            if (this.ForceInstantRender || this.TypewriterCharactersPerSecond <= 0f)
            {
                this.DisplayImmediate(line);
                await Task.CompletedTask;
                return;
            }

            this.isTypewriterRunning = true;
            float delay = 1.0f / this.TypewriterCharactersPerSecond;

            int index = 0;
            while (index < line.Length)
            {
                if (this.skipTypewriterRequested)
                {
                    string remainder = line[index..];
                    this.outputLabel.AppendText(remainder);
                    index = line.Length;
                    break;
                }

                this.outputLabel.AppendText(line[index].ToString());
                index++;
                this.ScrollToBottom();
                await ToSignal(this.GetTree().CreateTimer(delay), Timer.SignalName.Timeout);
            }

            this.outputLabel.AppendText("\n");
            this.ScrollToBottom();

            this.skipTypewriterRequested = false;
            this.isTypewriterRunning = false;
        }

        private void ScrollToBottom()
        {
            this.outputLabel.ScrollToLine(this.outputLabel.GetLineCount());
            if (this.outputScroll.GetVScrollBar() is VScrollBar vScroll)
            {
                vScroll.Value = vScroll.MaxValue;
            }
        }
    }

    /// <summary>
    /// Schema for the opening narrative terminal scene.
    /// </summary>
    public partial class SceneSchema
    {
        /// <summary>
        /// Gets or sets the unique identifier for the scene referenced by save data and transitions.
        /// </summary>
        public string SceneId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the human-readable title of the scene.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ordered steps executed by the terminal flow.
        /// </summary>
        public List<SceneStep> Steps { get; set; } = new();
    }

    /// <summary>
    /// Individual scene step description.
    /// </summary>
    public partial class SceneStep
    {
        /// <summary>
        /// Gets or sets the unique identifier of the step used in branching.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the step type (dialogue, choice, input, or effect).
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the text lines rendered during dialogue steps.
        /// </summary>
        public List<string> Lines { get; set; } = new();

        /// <summary>
        /// Gets or sets the prompt shown above choice or input widgets.
        /// </summary>
        public string Prompt { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of available choice options for choice steps.
        /// </summary>
        public List<ChoiceOption> Options { get; set; } = new();

        /// <summary>
        /// Gets or sets the effect identifier for effect steps.
        /// </summary>
        public string Effect { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the effect duration in milliseconds.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the delay in milliseconds applied after the step completes.
        /// </summary>
        public int Delay { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the next step to execute, or <see langword="null"/> to advance sequentially.
        /// </summary>
        public string? NextStep { get; set; }
    }

    /// <summary>
    /// Choice data used within scene steps.
    /// </summary>
    public partial class ChoiceOption
    {
        /// <summary>
        /// Gets or sets the unique identifier for the choice.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the label shown to the player for this choice.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Gets or sets optional descriptive text displayed beneath the label.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the legacy story block index associated with the choice.
        /// </summary>
        public int NextBlock { get; set; }

        /// <summary>
        /// Gets or sets the next step identifier to jump to when this option is chosen, or <see langword="null"/> to follow the step default.
        /// </summary>
        public string? NextStep { get; set; }
    }
}
