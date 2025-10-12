// <copyright file="Scene1Narrative.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;
using OmegaSpiral.Scripts.Field.Narrative;
using OmegaSpiral.Source.Scripts;
using YamlDotNet.Serialization;

/// <summary>
/// Main narrative scene that integrates with the Dreamweaver system and Dialogic plugin
/// to provide dynamic story generation and persona-based narrative experiences.
/// </summary>
public partial class Scene1Narrative : Node2D
{
    // UI components
    private RichTextLabel? outputLabel;
    private LineEdit? inputField;
    private Button? submitButton;
    private Label? promptLabel;
    private string fullText = string.Empty;
    private int currentCharIndex;
    private Godot.Timer? typewriterTimer;
    private ShaderMaterial? crtMaterial;

    // Dreamweaver system integration
    private DreamweaverSystem? dreamweaverSystem;
    private SceneManager? sceneManager;
    private string currentPersona = "hero";
    private List<string> openingLines = new ();
    private int currentLineIndex;
    private bool waitingForNameInput;
    private bool waitingForPersonaChoice;

    /// <summary>
    /// Gets or sets the speed of the typewriter effect in seconds per character.
    /// </summary>
    [Export]
    public float TypewriterSpeed { get; set; } = 0.05f; // seconds per character

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.outputLabel = this.GetNode<RichTextLabel>("OutputLabel");
        this.inputField = this.GetNode<LineEdit>("InputField");
        this.submitButton = this.GetNode<Button>("SubmitButton");
        this.promptLabel = this.GetNode<Label>("PromptLabel");

        if (this.outputLabel == null || this.inputField == null || this.submitButton == null || this.promptLabel == null)
        {
            GD.PrintErr("Failed to find required UI nodes in Scene1Narrative");
            return;
        }

        this.crtMaterial = (ShaderMaterial)this.outputLabel.Material;

        // Get autoload references
        this.dreamweaverSystem = this.GetNode<DreamweaverSystem>("/root/DreamweaverSystem");
        this.sceneManager = this.GetNode<SceneManager>("/root/SceneManager");

        this.submitButton.Pressed += this.OnSubmitPressed;

        // Initialize narrative system
        this.InitializeNarrative();
    }

    /// <inheritdoc/>
    public override void _Process(double delta)
    {
        if (this.crtMaterial != null)
        {
            this.crtMaterial.SetShaderParameter("time", (float)(Time.GetTicksMsec() / 1000.0));
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
    /// Loads persona configuration from YAML file.
    /// </summary>
    /// <param name="personaId">The persona identifier.</param>
    /// <returns>The loaded persona configuration or null if not found.</returns>
    private static PersonaConfig? LoadPersonaConfigFromYaml(string personaId)
    {
        try
        {
            var configPath = $"res://Source/Data/scenes/scene1_narrative/{personaId}.yaml";
            if (!Godot.FileAccess.FileExists(configPath))
            {
                GD.PrintErr($"Persona config not found: {configPath}");
                return null;
            }

            Godot.FileAccess file = Godot.FileAccess.Open(configPath, Godot.FileAccess.ModeFlags.Read);
            var yamlText = file.GetAsText();
            file.Close();

            // Use YamlDotNet to deserialize the YAML directly into C# objects
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().Build();
            return deserializer.Deserialize<PersonaConfig>(yamlText);
        }
        catch (InvalidOperationException ex)
        {
            GD.PrintErr($"Failed to load persona config for {personaId}: {ex.Message}");
            return null;
        }
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
            var timelinePath = "res://Source/Data/scenes/scene1_narrative/opening_scene.dtl";
            var dialogicNode = (GodotObject)this.GetNode("/root/Dialogic");

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

        // Load opening lines from the hero persona configuration
        var heroPersona = this.dreamweaverSystem.GetNode<DreamweaverPersona>("/root/DreamweaverSystem") as DreamweaverPersona;
        if (heroPersona != null)
        {
            // Use reflection to access the config since it's private
            var configField = heroPersona.GetType().GetField("config", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (configField != null)
            {
                var config = configField.GetValue(heroPersona) as PersonaConfig;
                if (config != null && config.OpeningLines.Count > 0)
                {
                    this.openingLines.AddRange(config.OpeningLines);
                }
                else
                {
                    // Fallback to hardcoded lines if config is empty
                    this.openingLines.Add("If you could hear only one story...");
                    this.openingLines.Add("What would it be?");
                    this.openingLines.Add("Choose your path:");
                }
            }
        }
        else
        {
            // Fallback if hero persona not found
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
        this.currentLineIndex = 0;
        if (this.openingLines.Count > 0)
        {
            this.DisplayTextWithTypewriter(this.openingLines[0]);
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
                this.outputLabel.Text += $"\n\nYou have chosen the {persona.ToUpper(System.Globalization.CultureInfo.InvariantCulture)} thread.";
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

        // Load persona-specific content from the YAML configuration
        // For now, use the personaId to determine which config to load
        var config = LoadPersonaConfigFromYaml(personaId);
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
