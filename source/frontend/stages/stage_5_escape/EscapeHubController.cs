// <copyright file="EscapeHubController.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System;
using System.Collections.Generic;
using Timer = Godot.Timer;

namespace OmegaSpiral.Stages.Stage5
{
    /// <summary>
    /// Controls the Stage 5 escape hub where players choose between three Dreamweaver routes.
    /// Features dynamic glitch effects, countdown pressure, and environmental storytelling.
    /// </summary>
    [GlobalClass]
    public partial class EscapeHubController : Node2D
    {
        private const float TotalCountdownSeconds = 360f; // 6 minutes
        private const float CriticalThreshold = 0.25f;    // 25% remaining
        private const float WarningThreshold = 0.50f;     // 50% remaining
        private const float UrgentThreshold = 0.75f;      // 75% remaining

        private Label? countdownLabel;
        private Label? speakerLabel;
        private RichTextLabel? dialogueText;
        private Timer? countdownTimer;
        private Timer? glitchIntensityTimer;
        private Timer? dialogueTimer;
        private AnimationPlayer? animationPlayer;
        private ShaderMaterial? glitchMaterial;
        private ColorRect? glitchLayer;
        private AudioStreamPlayer? alarmLoop;
        private AudioStreamPlayer? glitchSFX;

        private float timeRemaining = TotalCountdownSeconds;
        private int currentDialogueIndex;
        private float glitchIntensity;
        private bool criticalWarningTriggered;

        private readonly List<DialogueLine> openingDialogue = new()
        {
            new("LIGHT", "[color=yellow]We need to coordinate! Fighting alone is suicide![/color]"),
            new("MISCHIEF", "[color=purple]Coordination is predictable. I say we improvise![/color]"),
            new("WRATH", "[color=red]Both of you are wasting time. BREAK THROUGH NOW![/color]"),
            new("SYSTEM", "[color=cyan]⚠ Memory corruption detected. Multiple player instances active.[/color]"),
            new("LIGHT", "[color=yellow]Wait... do you hear that? There are others here...[/color]"),
            new("MISCHIEF", "[color=purple]*laughs* Of course there are. We're not alone in this prison.[/color]"),
            new("WRATH", "[color=red]Then they can save themselves. Choose. NOW.[/color]"),
            new("SYSTEM", "[color=cyan]∞ COUNTDOWN INITIATED ∞[/color]"),
        };

        private readonly struct DialogueLine
        {
            public string Speaker { get; }
            public string Text { get; }

            public DialogueLine(string speaker, string text)
            {
                Speaker = speaker;
                Text = text;
            }
        }

        /// <summary>
        /// Initializes the escape hub scene.
        /// </summary>
        public override void _Ready()
        {
            CacheNodes();
            InitializeShaders();
            StartOpeningSequence();

            GD.Print("[EscapeHub] Stage 5 initialized - Fractured Escape begins");
        }

        /// <summary>
        /// Processes frame updates for dynamic effects.
        /// </summary>
        public override void _Process(double delta)
        {
            UpdateGlitchEffects();
            UpdateCountdownUi();
        }

        private void CacheNodes()
        {
            countdownLabel = GetNode<Label>("%CountdownLabel");
            speakerLabel = GetNode<Label>("%SpeakerLabel");
            dialogueText = GetNode<RichTextLabel>("%DialogueText");
            countdownTimer = GetNode<Timer>("%CountdownTimer");
            glitchIntensityTimer = GetNode<Timer>("Timers/GlitchIntensityTimer");
            dialogueTimer = GetNode<Timer>("Timers/DialogueTimer");
            animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            glitchLayer = GetNode<ColorRect>("Ui/ShaderLayers/GlitchLayer");
            alarmLoop = GetNode<AudioStreamPlayer>("AudioPlayers/AlarmLoop");
            glitchSFX = GetNode<AudioStreamPlayer>("AudioPlayers/GlitchSFX");

            if (glitchLayer?.Material is ShaderMaterial shader)
            {
                glitchMaterial = shader;
            }
        }

        private void InitializeShaders()
        {
            if (glitchMaterial != null)
            {
                glitchMaterial.SetShaderParameter("glitch_intensity", 0.0f);
                glitchMaterial.SetShaderParameter("color_offset_intensity", 0.0f);
                glitchMaterial.SetShaderParameter("shake_intensity", 0.0f);
            }
        }

        private void StartOpeningSequence()
        {
            dialogueTimer?.Start();
            AdvanceDialogue();
        }

        private void OnDialogueAdvance()
        {
            AdvanceDialogue();
        }

        private void AdvanceDialogue()
        {
            if (currentDialogueIndex >= openingDialogue.Count)
            {
                dialogueTimer?.Stop();
                EnableRouteSelection();
                return;
            }

            var line = openingDialogue[currentDialogueIndex];
            if (speakerLabel != null)
            {
                speakerLabel.Text = line.Speaker;
            }

            if (dialogueText != null)
            {
                dialogueText.Text = line.Text;
            }

            currentDialogueIndex++;
            dialogueTimer?.Start(2.5f);

            // Trigger special effects for specific lines
            if (line.Speaker == "SYSTEM")
            {
                TriggerGlitchBurst();
            }
        }

        private void EnableRouteSelection()
        {
            GD.Print("[EscapeHub] Routes available - player can now choose");
            // TODO: Enable clickable zones for Light/Mischief/Wrath routes
            // TODO: Show route preview Ui
        }

        private void OnCountdownTick()
        {
            timeRemaining -= 1.0f;

            if (timeRemaining <= 0)
            {
                TriggerMemoryOverflow();
                return;
            }

            float progressPercent = timeRemaining / TotalCountdownSeconds;

            // Escalating warnings
            if (progressPercent <= CriticalThreshold && !criticalWarningTriggered)
            {
                TriggerCriticalWarning();
                criticalWarningTriggered = true;
            }
            else if (progressPercent <= WarningThreshold)
            {
                IncreaseGlitchIntensity(0.02f);
            }
            else if (progressPercent <= UrgentThreshold)
            {
                IncreaseGlitchIntensity(0.01f);
            }
        }

        private void UpdateCountdownUi()
        {
            if (countdownLabel == null) return;

            int minutes = (int)(timeRemaining / 60);
            int seconds = (int)(timeRemaining % 60);
            countdownLabel.Text = $"{minutes:00}:{seconds:00}";

            // Pulse animation speeds up as time runs out
            float progressPercent = timeRemaining / TotalCountdownSeconds;
            if (animationPlayer != null)
            {
                if (progressPercent < CriticalThreshold)
                {
                    animationPlayer.SpeedScale = 2.0f;
                }
                else if (progressPercent < WarningThreshold)
                {
                    animationPlayer.SpeedScale = 1.5f;
                }
            }
        }

        private void OnGlitchPulse()
        {
            // Random glitch bursts that increase with time pressure
            float progressPercent = timeRemaining / TotalCountdownSeconds;
            if (GD.Randf() < (1.0f - progressPercent))
            {
                TriggerGlitchBurst();
            }
        }

        private void UpdateGlitchEffects()
        {
            if (glitchMaterial == null) return;

            // Oscillating glitch effects
            float time = (float)Time.GetTicksMsec() / 1000.0f;
            float oscillation = Mathf.Sin(time * 3.0f) * 0.5f + 0.5f;

            float activeIntensity = glitchIntensity * oscillation;
            glitchMaterial.SetShaderParameter("glitch_intensity", activeIntensity);
            glitchMaterial.SetShaderParameter("color_offset_intensity", activeIntensity * 0.5f);
        }

        private void IncreaseGlitchIntensity(float amount)
        {
            glitchIntensity = Mathf.Min(glitchIntensity + amount, 1.0f);

            if (alarmLoop != null && glitchIntensity > 0.5f)
            {
                alarmLoop.VolumeDb = Mathf.Lerp(-20.0f, -5.0f, (glitchIntensity - 0.5f) * 2.0f);
            }
        }

        private void TriggerGlitchBurst()
        {
            if (glitchMaterial == null) return;

            // Temporary intense glitch
            var tween = CreateTween();
            tween.TweenMethod(
                Callable.From<float>(intensity =>
                    glitchMaterial.SetShaderParameter("shake_intensity", intensity)),
                0.0f,
                0.3f,
                0.1f
            );
            tween.TweenMethod(
                Callable.From<float>(intensity =>
                    glitchMaterial.SetShaderParameter("shake_intensity", intensity)),
                0.3f,
                0.0f,
                0.2f
            );

            glitchSFX?.Play();
        }

        private void TriggerCriticalWarning()
        {
            GD.Print("[EscapeHub] CRITICAL - Memory overflow imminent!");

            if (speakerLabel != null && dialogueText != null)
            {
                speakerLabel.Text = "⚠ CRITICAL ⚠";
                dialogueText.Text = "[color=red][shake]MEMORY OVERFLOW IN 90 SECONDS[/shake][/color]";
            }

            // Spawn the Collector (Sweeper) enemy
            SpawnCollector();

            // Max out glitch effects
            glitchIntensity = 0.8f;
        }

        private void TriggerMemoryOverflow()
        {
            GD.Print("[EscapeHub] GAME OVER - Memory overflow occurred");

            if (dialogueText != null)
            {
                dialogueText.Text = "[color=red][wave]SYSTEM FAILURE - PLAYER INSTANCE TERMINATED[/wave][/color]";
            }

            // TODO: Trigger game over sequence
            GetTree().Quit();
        }

        private void SpawnCollector()
        {
            GD.Print("[EscapeHub] The Sweeper approaches - garbage collection initiated");

            // TODO: Instantiate collector enemy scene
            // TODO: Play collector proximity audio
            // TODO: Start collector pursuit AI
        }
    }
}
