// <copyright file="Scene1Narrative.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using Godot;

public partial class Scene1Narrative : Node2D
{
    [Export]
    public string IntroductionText = "If you could hear only one story... what would it be?";
    [Export]
    public float TypewriterSpeed = 0.05f; // seconds per character

    private RichTextLabel? outputLabel;
    private LineEdit? inputField;
    private Button? submitButton;
    private Label? promptLabel;
    private string fullText = string.Empty;
    private int currentCharIndex;
    private Godot.Timer? typewriterTimer;

    private ShaderMaterial? crtMaterial;

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

        this.submitButton.Pressed += this.OnSubmitPressed;

        // Start typewriter effect
        this.StartTypewriterEffect();
    }

    /// <inheritdoc/>
    public override void _Process(double delta)
    {
        if (this.crtMaterial != null)
        {
            this.crtMaterial.SetShaderParameter("time", (float)(Time.GetTicksMsec() / 1000.0));
        }
    }

    private void StartTypewriterEffect()
    {
        this.fullText = this.IntroductionText;
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

            // After typewriter, show the prompt
            this.ShowNamePrompt();
        }
    }

    private void ShowNamePrompt()
    {
        if (this.promptLabel == null || this.inputField == null || this.submitButton == null)
        {
            return;
        }

        this.promptLabel.Text = "What is your name, traveler?";
        this.inputField.Visible = true;
        this.submitButton.Visible = true;
    }

    private void OnSubmitPressed()
    {
        if (this.inputField == null || this.outputLabel == null || this.promptLabel == null)
        {
            return;
        }

        string playerName = this.inputField.Text.Trim();
        if (!string.IsNullOrEmpty(playerName))
        {
            GD.Print($"Player name: {playerName}");

            // TODO: Save to GameState and transition to next scene
            // For now, just print
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

            // TODO: Wait for input to proceed
        }
    }

    /// <inheritdoc/>
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && this.inputField != null && this.inputField.Visible == false)
        {
            // Proceed to next scene
            GD.Print("Proceeding to next scene...");

            // TODO: Load next scene
        }
    }
}
