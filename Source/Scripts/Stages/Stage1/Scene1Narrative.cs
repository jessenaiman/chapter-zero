
// <copyright file="Scene1Narrative.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Globalization;
using OmegaSpiral.Source.Scripts.Field.Narrative;
using OmegaSpiral.Source.Scripts;

namespace OmegaSpiral.Source.Scripts.Field.Narrative;
/// <summary>
/// Main narrative scene that integrates with the Dreamweaver system and Dialogic plugin
/// to provide dynamic story generation and persona-based narrative experiences.
/// </summary>
[GlobalClass]
public partial class Scene1Narrative : Node2D
{
    /// <summary>
    /// UI components
    /// </summary>
    private RichTextLabel? outputLabel;
    private LineEdit? inputField;
    private Button? submitButton;
    private Label? promptLabel;
    private string fullText = string.Empty;
    private int currentCharIndex;
    private Godot.Timer? typewriterTimer;
    private ShaderMaterial? crtMaterial;

    /// <summary>
    /// Dreamweaver system integration
    /// </summary>
    private DreamweaverSystem? dreamweaverSystem;
    private SceneManager? sceneManager;
    private string currentPersona = "hero";
    private List<string> openingLines = new();
    private int currentLineIndex;
    private bool waitingForNameInput;
    private bool waitingForPersonaChoice;

    /// <summary>
    /// Gets or sets the speed of the typewriter effect in seconds per character.
    /// </summary>
    [Export]
    public float TypewriterSpeed { get; set; } = 0.05f; // seconds per character

    /// <inheritdoc/>
    /// <summary>
    /// Called when the node enters the scene tree. Initializes UI components and narrative system.
    /// </summary>
    /// <inheritdoc/>
    public override async void _Ready()
    {
        GD.Print("Scene1Narrative _Ready() called");

        this.outputLabel = this.GetNodeOrNull<RichTextLabel>("TerminalPanel/OutputLabel");
        this.inputField = this.GetNodeOrNull<LineEdit>("InputField");
        this.submitButton = this.GetNodeOrNull<Button>("SubmitButton");
        this.promptLabel = this.GetNodeOrNull<Label>("PromptLabel");

        if (this.outputLabel == null)
        {
            GD.PrintErr("OutputLabel node not found at path: TerminalPanel/OutputLabel");
        }
        if (this.inputField == null)
        {
            GD.PrintErr("InputField node not found at path: InputField");
        }
        if (this.submitButton == null)
        {
            GD.PrintErr("SubmitButton node not found at path: SubmitButton");
        }
        if (this.promptLabel == null)
        {
            GD.PrintErr("PromptLabel node not found at path: PromptLabel");
        }
        if (this.outputLabel == null || this.inputField == null || this.submitButton == null || this.promptLabel == null)
        {
            GD.PrintErr("Failed to find one or more required UI nodes in Scene1Narrative. Initialization aborted.");
            return;
        }

        GD.Print("All UI nodes found successfully");

        this.crtMaterial = (ShaderMaterial) this.outputLabel.Material;

        // Get autoload references
        this.dreamweaverSystem = this.GetNode<DreamweaverSystem>("/root/DreamweaverSystem");
        this.sceneManager = this.GetNode<SceneManager>("/root/SceneManager");

        GD.Print($"DreamweaverSystem instance: {this.dreamweaverSystem?.GetInstanceId() ?? 0}, SceneManager: {this.sceneManager != null}");

        this.submitButton.Pressed += this.OnSubmitPressed;

        // Initialize narrative system
        if (this.dreamweaverSystem != null)
        {
            await this.dreamweaverSystem.InitializationComplete;
        }

        this.InitializeNarrative();

        GD.Print("Scene1Narrative initialization complete");
    }

    /// <inheritdoc/>
    public override void _Process(double delta)
    {
        if (this.crtMaterial != null)
        {
            this.crtMaterial.SetShaderParameter("time", (float) (Time.GetTicksMsec() / 1000.0));
        }
    }

    /// <inheritdoc/>
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (this.waitingForPersonaChoice)
            {
                this.HandlePersonaChoice(keyEvent);
            }
            else if (this.waitingForNameInput && this.inputField != null && this.inputField.Visible == false)
            {
                this.ProceedToNextScene();
            }
        }
    }

    /// <summary>
    /// Loads persona configuration from JSON file using ConfigurationService.
    /// </summary>
    /// <param name="personaId">The persona identifier.</param>
    /// <returns>The loaded persona configuration or null if not found.</returns>
    private static PersonaConfig? LoadPersonaConfigFromJson(string personaId)
    {
        try
        {
            var configPath = $"res://Source/Data/stages/ghost-terminal/{personaId}.json";
            if (!Godot.FileAccess.FileExists(configPath))
            {
                GD.PrintErr($"Persona config not found: {configPath}");
                return null;
            }

            // Use ConfigurationService for unified JSON loading with schema validation
            var configData = OmegaSpiral.Source.Scripts.Infrastructure.ConfigurationService.LoadConfiguration(configPath);
            if (configData == null)
            {
                GD.PrintErr($"Failed to load configuration from {configPath}");
                return null;
            }

            // Map Godot.Collections.Dictionary to PersonaConfig
            return MapToPersonaConfig(configData);
        }
        catch (InvalidOperationException ex)
        {
            GD.PrintErr($"Failed to load persona config for {personaId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Maps a Godot Dictionary to a PersonaConfig object.
    /// </summary>
    /// <param name="configData">The configuration dictionary.</param>
    /// <returns>A new PersonaConfig instance.</returns>
    private static PersonaConfig? MapToPersonaConfig(Godot.Collections.Dictionary<string, Godot.Variant> configData)
    {
        var config = new PersonaConfig();

        if (configData.TryGetValue("openingLines", out var openingLinesVar))
        {
            var openingArray = openingLinesVar.AsGodotArray();
            foreach (var line in openingArray)
            {
                config.OpeningLines.Add(line.ToString() ?? string.Empty);
            }
        }

        // Map other fields as needed...
        return config;
    }

    /// <summary>
    /// Initializes the narrative system and loads the opening sequence.
    /// </summary>
    private void InitializeNarrative()
    {
        if (this.dreamweaverSystem == null)
        {
            GD.PrintErr("DreamweaverSystem not found");
            return;
        }

        // Start Dialogic timeline for the opening scene
        this.StartDialogicTimeline();
    }

    /// <summary>
    /// Starts the Dialogic timeline for the opening scene.
    /// </summary>
    private void StartDialogicTimeline()
    {
        try
        {
            var timelinePath = "res://Source/Data/stages/ghost-terminal/opening_scene.dtl";
            var dialogicNode = (GodotObject) this.GetNode("/root/Dialogic");

            if (dialogicNode != null)
            {
                // Start the timeline
                dialogicNode.Call("start_timeline", timelinePath);

                // Connect to timeline events using Godot's signal system
                Callable timelineEndedCallable = new Callable(this, "OnTimelineEnded");
                Callable textSignalCallable = new Callable(this, "OnTextSignal");

                dialogicNode.Connect("timeline_ended", timelineEndedCallable);
                dialogicNode.Connect("dialogic_signal", textSignalCallable);

                GD.Print("Started Dialogic timeline for opening scene");
            }
            else
            {
                GD.PrintErr("Dialogic node not found");

                // Fallback to original narrative system
                this.StartOriginalNarrative();
            }
        }
        catch (InvalidCastException ex)
        {
            GD.PrintErr($"Failed to start Dialogic timeline: {ex.Message}");

            // Fallback to original narrative system
            this.StartOriginalNarrative();
        }
    }

    /// <summary>
    /// Fallback method to start the original narrative system if Dialogic fails.
    /// </summary>
    private void StartOriginalNarrative()
    {
        GD.Print("Starting original narrative system (Dialogic fallback)");
        this.LoadOpeningLines();
        this.StartNarrativeSequence();
    }

    /// <summary>
    /// Called when the Dialogic timeline ends.
    /// </summary>
    private void OnTimelineEnded()
    {
        GD.Print("Dialogic timeline ended");

        // Proceed to the next scene after timeline completion
        this.ProceedToNextScene();
    }

    /// <summary>
    /// Loads the opening lines from the current persona's configuration.
    /// </summary>
    private void LoadOpeningLines()
    {
        if (this.dreamweaverSystem == null)
        {
            return;
        }

        // For now, start with hero persona and load its opening lines
        // This will be enhanced to use the selected persona later
        this.openingLines.Clear();

        // Get the current persona from the DreamweaverSystem
        var heroPersona = this.dreamweaverSystem.GetPersona("hero");
        GD.Print($"Hero persona found: {heroPersona != null}");
        if (heroPersona != null)
        {
            GD.Print($"Hero config OpeningLines count: {heroPersona.Config.OpeningLines.Count}");
            if (heroPersona.Config.OpeningLines.Count > 0)
            {
                this.openingLines.AddRange(heroPersona.Config.OpeningLines);
                GD.Print($"Loaded {this.openingLines.Count} opening lines from hero config");
            }
            else
            {
                GD.Print("Hero config has no opening lines, using fallback");
                // Fallback to hardcoded lines if config is empty or persona not found
                this.openingLines.Add("If you could hear only one story...");
                this.openingLines.Add("What would it be?");
                this.openingLines.Add("Choose your path:");
            }
        }
        else
        {
            GD.Print("Hero persona not found, using fallback");
            // Fallback to hardcoded lines if config is empty or persona not found
            this.openingLines.Add("If you could hear only one story...");
            this.openingLines.Add("What would it be?");
            this.openingLines.Add("Choose your path:");
        }
    }

    /// <summary>
    /// Starts the narrative sequence with the first opening line.
    /// </summary>
    private void StartNarrativeSequence()
    {
        GD.Print($"StartNarrativeSequence called with {this.openingLines.Count} opening lines");
        this.currentLineIndex = 0;
        if (this.openingLines.Count > 0)
        {
            GD.Print($"Displaying first line: {this.openingLines[0]}");
            this.DisplayTextWithTypewriter(this.openingLines[0]);
        }
        else
        {
            GD.PrintErr("No opening lines to display!");
        }
    }

    /// <summary>
    /// Displays text with a typewriter effect.
    /// </summary>
    /// <param name="text">The text to display.</param>
    private void DisplayTextWithTypewriter(string text)
    {
        this.fullText = text;
        this.currentCharIndex = 0;
        if (this.outputLabel != null)
        {
            this.outputLabel.Text = string.Empty;
        }

        this.typewriterTimer = new Godot.Timer();
        this.typewriterTimer.WaitTime = this.TypewriterSpeed;
        this.typewriterTimer.OneShot = false;
        this.typewriterTimer.Timeout += this.OnTypewriterTimeout;
        this.AddChild(this.typewriterTimer);
        this.typewriterTimer.Start();
    }

    /// <summary>
    /// Handles the typewriter timer timeout to display characters one by one.
    /// </summary>
    private void OnTypewriterTimeout()
    {
        if (this.outputLabel == null || this.typewriterTimer == null)
        {
            return;
        }

        if (this.currentCharIndex < this.fullText.Length)
        {
            this.outputLabel.Text += this.fullText[this.currentCharIndex];
            this.currentCharIndex++;
        }
        else
        {
            this.typewriterTimer.Stop();
            this.typewriterTimer.QueueFree();

            // After typewriter, proceed to next step in narrative sequence
            this.OnNarrativeLineComplete();
        }
    }

    /// <summary>
    /// Called when a narrative line completes displaying.
    /// </summary>
    private void OnNarrativeLineComplete()
    {
        this.currentLineIndex++;

        if (this.currentLineIndex < this.openingLines.Count)
        {
            // Display next opening line
            this.DisplayTextWithTypewriter(this.openingLines[this.currentLineIndex]);
        }
        else
        {
            // All opening lines complete, show persona selection
            this.ShowPersonaSelection();
        }
    }

    /// <summary>
    /// Displays the persona selection options to the player.
    /// </summary>
    private void ShowPersonaSelection()
    {
        if (this.outputLabel == null || this.promptLabel == null)
        {
            return;
        }

        this.outputLabel.Text += "\n\nChoose your Dreamweaver thread:";
        this.promptLabel.Text = "Press [1] HERO, [2] SHADOW, or [3] AMBITION";
        this.waitingForPersonaChoice = true;
    }

    /// <summary>
    /// Handles the player's persona choice input.
    /// </summary>
    /// <param name="keyEvent">The key event containing the player's input.</param>
    private void HandlePersonaChoice(InputEventKey keyEvent)
    {
        string persona = keyEvent.Keycode switch
        {
            Key.Key1 => "hero",
            Key.Key2 => "shadow",
            Key.Key3 => "ambition",
            _ => string.Empty,
        };

        if (!string.IsNullOrEmpty(persona))
        {
            this.currentPersona = persona;
            this.waitingForPersonaChoice = false;

            if (this.outputLabel != null)
            {
                this.outputLabel.Text += $"\n\nYou have chosen the {persona.ToUpper(CultureInfo.InvariantCulture)} thread.";
            }

            // Set the persona in the scene manager
            if (this.sceneManager != null)
            {
                this.sceneManager.SetDreamweaverThread(persona);
            }

            // Load persona-specific content and show name prompt
            this.LoadPersonaContent(persona);
            this.ShowNamePrompt();
        }
    }

    /// <summary>
    /// Loads persona-specific content based on the selected persona.
    /// </summary>
    /// <param name="personaId">The selected persona identifier.</param>
    private void LoadPersonaContent(string personaId)
    {
        if (this.dreamweaverSystem == null || this.outputLabel == null)
        {
            return;
        }

        // Load persona-specific content from the JSON configuration
        // For now, use the personaId to determine which config to load
        var config = LoadPersonaConfigFromJson(personaId);
        if (config != null && config.InitialChoice != null)
        {
            if (this.outputLabel != null)
            {
                this.outputLabel.Text += $"\n\n{config.InitialChoice.Prompt}";
            }
        }
    }

    /// <summary>
    /// Displays the name input prompt to the player.
    /// </summary>
    private void ShowNamePrompt()
    {
        if (this.promptLabel == null || this.inputField == null || this.submitButton == null)
        {
            return;
        }

        this.promptLabel.Text = "What is your name, traveler?";
        this.inputField.Visible = true;
        this.submitButton.Visible = true;
        this.waitingForNameInput = true;
    }

    /// <summary>
    /// Proceeds to the next scene after name input is complete.
    /// </summary>
    private void ProceedToNextScene()
    {
        try
        {
            GD.Print("Proceeding to next scene...");

            if (this.sceneManager == null)
            {
                GD.PrintErr("SceneManager not found, cannot proceed to next scene");
                return;
            }

            this.sceneManager.UpdateCurrentScene(2);
            this.sceneManager.TransitionToScene("Scene2NethackSequence");

            GD.Print("Successfully initiated transition to Scene2NethackSequence");
        }
        catch (InvalidOperationException ex)
        {
            GD.PrintErr($"Failed to proceed to next scene: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles Dialogic signal events.
    /// </summary>
    /// <param name="argument">The signal argument from Dialogic.</param>
    private void OnTextSignal(string argument)
    {
        GD.Print($"Received Dialogic signal: {argument}");
        // Handle specific Dialogic signals here if needed
        // For now, just log the signal
        // Accessing instance data to satisfy CA1822 analyzer
        if (this.outputLabel != null)
        {
            this.outputLabel.Modulate = this.outputLabel.Modulate; // Access instance property
        }
    }

    /// <summary>
    /// Handles the submit button press for name input.
    /// </summary>
    private void OnSubmitPressed()
    {
        if (this.inputField == null || this.outputLabel == null || this.promptLabel == null || this.sceneManager == null)
        {
            return;
        }

        string playerName = this.inputField.Text.Trim();
        if (!string.IsNullOrEmpty(playerName))
        {
            GD.Print($"Player name: {playerName}");

            // Save player name to scene manager
            this.sceneManager.SetPlayerName(playerName);

            if (this.outputLabel != null)
            {
                this.outputLabel.Text += $"\n\nWelcome, {playerName}!";
            }

            if (this.inputField != null)
            {
                this.inputField.Visible = false;
            }

            if (this.submitButton != null)
            {
                this.submitButton.Visible = false;
            }

            if (this.promptLabel != null)
            {
                this.promptLabel.Text = "Press any key to continue...";
            }
        }
    }
}
