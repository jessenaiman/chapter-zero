// <copyright file="TerminalUI.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Scripts.Field.Narrative;

namespace OmegaSpiral.Source.Scripts.Field.Narrative;

/// <summary>
/// Custom terminal UI component that combines godot-xterm with 2D/3D effects for narrative presentation.
/// Features include ghost writing animation, dissolve transitions, and elevated 3D display effects.
/// </summary>
    [GlobalClass]
    public partial class TerminalUI : Control
    {
        /// <summary>Terminal node from godot-xterm addon.</summary>
        private Node terminalNode = default!;

        /// <summary>PTY node for terminal functionality.</summary>
        private Node ptyNode = default!;

        /// <summary>3D display box for elevated terminal presentation.</summary>
        private TerminalDisplayBox terminalDisplayBox = default!;

        /// <summary>3D scene container for terminal display.</summary>
        private Node3D terminalDisplay3D = default!;

        /// <summary>Mesh instance for the 3D screen.</summary>
        private MeshInstance3D screenMesh = default!;

        /// <summary>Camera for 3D depth perception effects.</summary>
        private Camera3D camera3D = default!;

        /// <summary>Directional light for screen illumination.</summary>
        private DirectionalLight3D screenLight = default!;

        /// <summary>Enhanced 3D scene container for advanced display effects.</summary>
        private Node3D enhancedTerminalDisplay3D = default!;

        /// <summary>Enhanced 3D display box component.</summary>
        private TerminalDisplayBox enhancedTerminalDisplayBox = default!;

        /// <summary>Directional light for enhanced screen effects.</summary>
        private DirectionalLight3D enhancedScreenLight = default!;

        /// <summary>Enhanced 2D text overlay label.</summary>
        private RichTextLabel enhancedOverlayText = default!;

        /// <summary>Canvas layer for enhanced overlay rendering.</summary>
        private CanvasLayer enhancedOverlayCanvas = default!;

        /// <summary>Glow texture for enhanced screen effects.</summary>
        private TextureRect enhancedScreenGlow = default!;

        /// <summary>Timer for enhanced typewriter effect.</summary>
        private Godot.Timer enhancedTypewriterTimer = default!;

        /// <summary>Timer for enhanced dissolve transitions.</summary>
        private Godot.Timer enhancedDissolveTimer = default!;

        /// <summary>Timer for enhanced text scrambling.</summary>
        private Godot.Timer enhancedScrambleTimer = default!;

        /// <summary>Timer for enhanced glow pulsing.</summary>
        private Godot.Timer enhancedGlowPulseTimer = default!;

        /// <summary>Audio player for enhanced typewriter sound effects.</summary>
        private AudioStreamPlayer enhancedTypewriterAudio = default!;

        /// <summary>Rich text label for 2D text overlay.</summary>
        private RichTextLabel overlayText = default!;

        /// <summary>Canvas layer for 2D overlay rendering.</summary>
        private CanvasLayer overlayCanvas = default!;

        /// <summary>Texture rect for screen glow effect.</summary>
        private TextureRect screenGlow = default!;

        /// <summary>Timer for typewriter text effect.</summary>
        private Godot.Timer typewriterTimer = default!;

        /// <summary>Timer for dissolve transition effect.</summary>
        private Godot.Timer dissolveTimer = default!;

        /// <summary>Timer for text scrambling animation.</summary>
        private Godot.Timer scrambleTimer = default!;

        /// <summary>Timer for glow pulsing animation.</summary>
        private Godot.Timer glowPulseTimer = default!;

        /// <summary>Audio player for typewriter sound effects.</summary>
        private AudioStreamPlayer typewriterAudio = default!;

        /// <summary>Current text being displayed.</summary>
        private string currentText = string.Empty;

        /// <summary>Target text to display.</summary>
        private string targetText = string.Empty;

        /// <summary>Current character index in typewriter animation.</summary>
        private int currentCharIndex;

        /// <summary>Indicates if typewriter effect is active.</summary>
        private bool isTyping;

        /// <summary>Indicates if dissolve transition is active.</summary>
        private bool isDissolving;

        /// <summary>Indicates if text scrambling is active.</summary>
        private bool isScrambling;

        /// <summary>Indicates if glow pulsing is active.</summary>
        private bool isGlowPulsing;

        /// <summary>Queue of text to display sequentially.</summary>
        private Queue<string> textQueue = new();

        /// <summary>Speed of character display in typewriter effect (seconds per character).</summary>
        [Export] public float TypewriterSpeed { get; set; } = 0.025f;

        /// <summary>Duration of dissolve transition effect (seconds).</summary>
        [Export] public float DissolveDuration { get; set; } = 0.75f;

        /// <summary>Speed of glow pulsing animation.</summary>
        [Export] public float GlowPulseSpeed { get; set; } = 2.0f;

        /// <summary>Intensity of the glow effect (0.0 to 1.0).</summary>
        [Export] public float GlowIntensity { get; set; } = 0.5f;

        /// <summary>Enables ghost writing animation for text display.</summary>
        [Export] public bool EnableGhostWriting { get; set; } = true;

        /// <summary>Enables dissolve transition effects between text blocks.</summary>
        [Export] public bool EnableDissolveEffects { get; set; } = true;

        /// <summary>Enables text scrambling effect during transitions.</summary>
        [Export] public bool EnableScrambleEffects { get; set; } = true;

        /// <summary>Enables typewriter audio sound effects.</summary>
        [Export] public bool EnableTypewriterAudio { get; set; } = true;

        /// <summary>Enables glow effects on terminal display.</summary>
        [Export] public bool EnableGlowEffects { get; set; } = true;

        /// <summary>Enables 3D depth perception effects for terminal display.</summary>
        [Export] public bool EnableDepthPerception { get; set; } = true;

        /// <summary>Emitted when typewriter text animation completes.</summary>
        [Signal] public delegate void TextTypedEventHandler();

        /// <summary>Emitted when a transition effect completes.</summary>
        [Signal] public delegate void TransitionCompletedEventHandler();

        /// <summary>Emitted when input is received from the terminal.</summary>
        /// <param name="input">The input string received from the terminal.</param>
        [Signal] public delegate void InputReceivedEventHandler(string input);

        /// <inheritdoc/>
        public override void _Ready()
        {
            this.InitializeTerminal();
            this.Initialize3DComponents();
            this.Initialize2DOverlay();
            this.InitializeEffects();
            this.InitializeAudio();
        }

        /// <summary>Initializes terminal node and signal connections.</summary>
        private void InitializeTerminal()
        {
            // Try to get the Terminal node from the scene (from godot-xterm addon)
            if (this.HasNode("Terminal"))
            {
                this.terminalNode = this.GetNode<Node>("Terminal");

                // Get PTY node for terminal functionality
                if (this.terminalNode.HasNode("PTY"))
                {
                    this.ptyNode = this.terminalNode.GetNode<Node>("PTY");

                    // Connect terminal signals
                    this.terminalNode.Connect("data_sent", Callable.From<string>(data => this.OnDataSent(data)));
                    this.ptyNode.Connect("data_received", Callable.From<string>(data => this.OnDataReceived(data)));
                    this.ptyNode.Connect("exited", Callable.From<int, int>((exitCode, signum) => this.OnPtyExited(exitCode, signum)));

                    // Connect size change to PTY resize
                    this.terminalNode.Connect("size_changed", Callable.From<Vector2I>(size => this.OnTerminalSizeChanged(size)));

                    GD.Print("TerminalUI: Successfully connected to godot-xterm addon");
                }
                else
                {
                    GD.PrintErr("TerminalUI: Terminal node found but PTY node missing");
                    this.InitializeFallbackTerminal();
                }
            }
            else
            {
                GD.Print("TerminalUI: godot-xterm addon not available, using fallback implementation");
                this.InitializeFallbackTerminal();
            }
        }

        /// <summary>Initializes a fallback terminal implementation when addon is not available.</summary>
        private void InitializeFallbackTerminal()
        {
            // Create basic nodes for fallback functionality
            this.terminalNode = new Control();
            this.terminalNode.Name = "Terminal";
            this.AddChild(this.terminalNode);

            this.ptyNode = new Node();
            this.ptyNode.Name = "PTY";
            this.terminalNode.AddChild(this.ptyNode);

            GD.Print("TerminalUI: Fallback terminal initialized");
        }

    /// <summary>Initializes 3D components for elevated terminal display effects.</summary>
    private void Initialize3DComponents()
    {
        // Create the terminal display box with realistic 3D geometry
        this.terminalDisplayBox = new TerminalDisplayBox
        {
            Name = "TerminalDisplayBox",
            ScreenWidth = 8.0f,
            ScreenHeight = 4.5f,
            BezelThickness = 0.15f,
            CaseThickness = 0.4f,
            ScreenGlowColor = new Color(0.2f, 1.0f, 0.2f),
            ScreenGlowIntensity = this.GlowIntensity,
            EnableScreenGlow = this.EnableGlowEffects,
            EnableAmbientLight = true
        };

        // Get the screen mesh from the display box for overlay positioning
        this.screenMesh = this.terminalDisplayBox.GetScreenMesh();

        // Get the camera from the display box
        this.camera3D = this.terminalDisplayBox.GetViewingCamera();

        // Add the display box to the 3D container
        this.terminalDisplay3D = new Node3D { Name = "TerminalDisplay3D" };
        this.terminalDisplay3D.AddChild(this.terminalDisplayBox);
        this.AddChild(this.terminalDisplay3D);

        // Add directional light to enhance screen glow (in addition to display box lights)
        this.screenLight = new DirectionalLight3D
        {
            Name = "ScreenLight",
            Position = new Vector3(0, -1, -2),
            Rotation = new Vector3(Mathf.Pi * 0.3f, 0, 0), // Pointing towards screen
            LightColor = new Color(0.2f, 1.0f, 0.2f),
            LightEnergy = 0.3f,
            LightIndirectEnergy = 0.5f
        };
        this.terminalDisplay3D.AddChild(this.screenLight);

        // Configure advanced elevation positioning
        this.ConfigureElevationPositioning();
    }

    /// <summary>Configures 3D elevation positioning and depth perception effects.</summary>
    private void ConfigureElevationPositioning()
    {
        // Position the 3D display in the center with enhanced elevation effect
        this.terminalDisplay3D.Position = new Vector3(this.Size.X / 2, this.Size.Y / 2, 0);

        // Add depth perception by positioning the terminal display box in 3D space
        this.terminalDisplayBox.Position = new Vector3(0, 0, 2.5f); // Elevation from background

        // Configure camera for better depth perception
        if (this.camera3D != null)
        {
            this.camera3D.Position = new Vector3(0, 0, 12); // Further back for perspective
            this.camera3D.Fov = 25.0f; // Narrower FOV for better depth perception
        }

        // Add subtle perspective tilt for enhanced 3D effect
        this.terminalDisplay3D.Rotation = new Vector3(-0.05f, 0, 0); // Slight downward tilt

        // Configure depth-based scaling for perspective
        this.terminalDisplay3D.Scale = new Vector3(0.95f, 0.95f, 1.0f); // Slight scaling for depth
    }

    /// <summary>Initializes 2D overlay components for text rendering on 3D screen.</summary>
    private void Initialize2DOverlay()
    {
        // Create overlay canvas for text rendering on 3D screen
        this.overlayCanvas = new CanvasLayer { Name = "OverlayCanvas" };
        this.AddChild(this.overlayCanvas);

        this.overlayText = new RichTextLabel
        {
            Name = "OverlayText",
            Size = new Vector2(750, 400),
            Position = new Vector2(25, 25),
            BbcodeEnabled = true,
            AutowrapMode = TextServer.AutowrapMode.WordSmart,
            ScrollActive = false
        };

        // Set terminal-style colors and styling
        var theme = new Theme();
        theme.SetColor("default_color", "RichTextLabel", new Color(0.2f, 1.0f, 0.2f, 0.95f)); // Slightly transparent for glow effect
        theme.SetColor("font_shadow_color", "RichTextLabel", new Color(0, 0.1f, 0, 1));
        theme.SetConstant("shadow_offset_x", "RichTextLabel", 1);
        theme.SetConstant("shadow_offset_y", "RichTextLabel", 1);
        theme.SetConstant("outline_size", "RichTextLabel", 1);
        theme.SetColor("outline_color", "RichTextLabel", new Color(0, 0.3f, 0, 0.5f));

        var font = ResourceLoader.Load<Font>("res://Source/assets/gui/font/terminal_font.tres");
        if (font != null)
        {
            theme.SetFont("normal_font", "RichTextLabel", font);
        }

        this.overlayText.Theme = theme;
        this.overlayCanvas.AddChild(this.overlayText);

        // Create screen glow effect texture
        this.screenGlow = new TextureRect
        {
            Name = "ScreenGlow",
            Size = new Vector2(800, 450),
            Position = new Vector2(0, 75), // Centered in the control
            Texture = new ImageTexture(),
            Modulate = new Color(0.2f, 1.0f, 0.2f, 0.1f),
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            Visible = this.EnableGlowEffects
        };

        // Create a simple glow texture programmatically
        this.CreateGlowTexture();

        this.AddChild(this.screenGlow);
    }

    /// <summary>Creates a procedural radial gradient glow texture.</summary>
    private void CreateGlowTexture()
    {
        // Create a procedural glow texture
        var image = Image.CreateEmpty(256, 256, false, Image.Format.Rgba8);
        image.Fill(new Color(0.2f, 1.0f, 0.2f, 0.3f));

        // Add radial gradient for glow effect
        for (int x = 0; x < 256; x++)
        {
            for (int y = 0; y < 256; y++)
            {
                var centerX = 128f;
                var centerY = 128f;
                var distance = Mathf.Sqrt(Mathf.Pow(x - centerX, 2) + Mathf.Pow(y - centerY, 2));
                var intensity = Mathf.Clamp(1.0f - (distance / 128f), 0f, 1f);
                var alpha = intensity * 0.3f;
                image.SetPixel(x, y, new Color(0.2f, 1.0f, 0.2f, alpha));
            }
        }

        var texture = ImageTexture.CreateFromImage(image);
        this.screenGlow.Texture = texture;
    }

    /// <summary>Initializes animation timers and connects their timeout signals.</summary>
    private void InitializeEffects()
    {
        this.typewriterTimer = new Godot.Timer { Name = "TypewriterTimer" };
        this.dissolveTimer = new Godot.Timer { Name = "DissolveTimer" };
        this.scrambleTimer = new Godot.Timer { Name = "ScrambleTimer" };
        this.glowPulseTimer = new Godot.Timer { Name = "GlowPulseTimer" };

        this.AddChild(this.typewriterTimer);
        this.AddChild(this.dissolveTimer);
        this.AddChild(this.scrambleTimer);
        this.AddChild(this.glowPulseTimer);

        this.typewriterTimer.Timeout += this.OnTypewriterTimeout;
        this.dissolveTimer.Timeout += this.OnDissolveTimeout;
        this.scrambleTimer.Timeout += this.OnScrambleTimeout;
        this.glowPulseTimer.Timeout += this.OnGlowPulseTimeout;

        // Start glow pulse effect if enabled
        if (this.EnableGlowEffects)
        {
            this.glowPulseTimer.WaitTime = 1.0f / this.GlowPulseSpeed;
            this.glowPulseTimer.Start();
        }
    }

    /// <summary>Initializes audio player for typewriter sound effects.</summary>
    private void InitializeAudio()
    {
        if (this.EnableTypewriterAudio)
        {
            this.typewriterAudio = new AudioStreamPlayer
            {
                Name = "TypewriterAudio",
                Bus = "SFX"
            };

            // Load typewriter sound effect
            // var typewriterSound = ResourceLoader.Load<AudioStream>("res://Source/assets/audio/typewriter_key.wav");
            // if (typewriterSound != null)
            // {
            //     this.typewriterAudio.Stream = typewriterSound;
            // }

            this.AddChild(this.typewriterAudio);
        }
    }

    /// <summary>
    /// Displays text with ghost writing animation effect.
    /// </summary>
    /// <param name="text">The text to display.</param>
    public async Task DisplayTextWithGhostWritingAsync(string text)
    {
        if (this.EnableGhostWriting)
        {
            await this.PlayGhostWritingAnimation(text).ConfigureAwait(false);
        }
        else
        {
            this.overlayText.Text = text;
            this.terminalNode.Call("write", text + "\n");
        }
    }

    /// <summary>
    /// Displays text with typewriter effect.
    /// </summary>
    /// <param name="text">The text to display character by character.</param>
    public async Task DisplayTextWithTypewriterAsync(string text)
    {
        this.targetText = text;
        this.currentText = string.Empty;
        this.currentCharIndex = 0;
        this.isTyping = true;

        this.typewriterTimer.WaitTime = this.TypewriterSpeed;
        this.typewriterTimer.Start();

        // Wait for typing to complete
        while (this.isTyping)
        {
            await this.ToSignal(this.typewriterTimer, Godot.Timer.SignalName.Timeout);
        }

        this.EmitSignal(SignalName.TextTyped);
    }

    /// <summary>
    /// Applies dissolve transition effect between text blocks.
    /// </summary>
    /// <param name="callback">Action to perform during transition.</param>
    public async Task ApplyDissolveTransitionAsync(Action callback)
    {
        if (!this.EnableDissolveEffects)
        {
            callback?.Invoke();
            return;
        }

        this.isDissolving = true;

        // Apply dissolve effect with shader-like alpha fade
        var baseTheme = (Godot.Theme?)this.overlayText.Theme ?? new Godot.Theme();
        var originalColor = baseTheme.GetColor("default_color", "RichTextLabel");
        var theme = (Godot.Theme?)baseTheme.Duplicate() ?? new Godot.Theme();

        // Fade out
        for (float alpha = 1.0f; alpha >= 0.0f; alpha -= 0.1f)
        {
            var fadeColor = new Color(originalColor.R, originalColor.G, originalColor.B, alpha);
            theme.SetColor("default_color", "RichTextLabel", fadeColor);
            this.overlayText.Theme = theme;
            await this.ToSignal(this.GetTree().CreateTimer(0.05f), Godot.Timer.SignalName.Timeout);
        }

        callback?.Invoke();

        // Fade in
        for (float alpha = 0.0f; alpha <= 1.0f; alpha += 0.1f)
        {
            var fadeColor = new Color(originalColor.R, originalColor.G, originalColor.B, alpha);
            theme.SetColor("default_color", "RichTextLabel", fadeColor);
            this.overlayText.Theme = theme;
            await this.ToSignal(this.GetTree().CreateTimer(0.05f), Godot.Timer.SignalName.Timeout);
        }

        this.isDissolving = false;
        this.EmitSignal(SignalName.TransitionCompleted);
    }

    /// <summary>
    /// Applies text scrambling effect between scenes.
    /// </summary>
    public async Task ApplyScrambleEffectAsync()
    {
        if (!this.EnableScrambleEffects)
        {
            return;
        }

        this.isScrambling = true;

        // Display random characters during scramble with increasing intensity
        var originalText = this.overlayText.Text;
        var random = new Random();

        for (int intensity = 1; intensity <= 10; intensity++)
        {
            var scrambledText = new char[originalText.Length];
            for (int j = 0; j < originalText.Length; j++)
            {
                if (originalText[j] != '\n' && originalText[j] != ' ')
                {
                    // Increase scrambling as intensity increases
                    if (random.Next(100) < intensity * 10)
                    {
                        scrambledText[j] = (char)('A' + random.Next(26));
                    }
                    else
                    {
                        scrambledText[j] = originalText[j];
                    }
                }
                else
                {
                    scrambledText[j] = originalText[j];
                }
            }

            this.overlayText.Text = new string(scrambledText);
            await this.ToSignal(this.GetTree().CreateTimer(0.02f + (intensity * 0.005f)), Godot.Timer.SignalName.Timeout);
        }

        this.overlayText.Text = originalText;

        // Brief pause after scrambling
        await this.ToSignal(this.GetTree().CreateTimer(0.1f), Godot.Timer.SignalName.Timeout);

        this.isScrambling = false;
    }

    /// <summary>Clears the terminal display and resets the prompt.</summary>
    public void ClearTerminal()
    {
        this.overlayText.Text = string.Empty;
        this.terminalNode.Call("clear");
        this.terminalNode.Call("write", "> ");
    }

    /// <summary>Writes text directly to the terminal without effects.</summary>
    /// <param name="text">Text to write to terminal.</param>
    public void WriteToTerminal(string text)
    {
        this.terminalNode.Call("write", text);
    }

    /// <summary>Handles typewriter timer timeout event and displays next character.</summary>
    private async void OnTypewriterTimeout()
    {
        if (this.currentCharIndex < this.targetText.Length)
        {
            this.currentText += this.targetText[this.currentCharIndex];
            this.overlayText.Text = this.currentText;
            this.terminalNode.Call("write", this.targetText[this.currentCharIndex].ToString());

            if (this.EnableTypewriterAudio && this.typewriterAudio != null)
            {
                this.typewriterAudio.Play();
            }

            this.currentCharIndex++;
        }
        else
        {
            this.typewriterTimer.Stop();
            this.isTyping = false;
            this.terminalNode.Call("write", "\n> ");
        }
    }

    /// <summary>Handles dissolve timer timeout event.</summary>
    private void OnDissolveTimeout()
    {
        this.dissolveTimer.Stop();
        this.isDissolving = false;
    }

    /// <summary>Handles scramble timer timeout event.</summary>
    private void OnScrambleTimeout()
    {
        this.scrambleTimer.Stop();
        this.isScrambling = false;
    }

    /// <summary>Handles glow pulse timer timeout event and updates glow intensity.</summary>
    private void OnGlowPulseTimeout()
    {
        if (this.EnableGlowEffects && this.terminalDisplayBox != null)
        {
            // Use the display box's built-in glow pulsing
            var time = (float)Time.GetTicksMsec() / 1000.0f;
            var pulse = (Mathf.Sin(time * this.GlowPulseSpeed) * 0.1f) + 1.0f;
            var intensity = this.GlowIntensity * pulse;

            this.terminalDisplayBox.UpdateScreenGlow(intensity);

            // Update overlay glow alpha
            if (this.screenGlow != null)
            {
                var glowAlpha = 0.1f + (Mathf.Sin(time * this.GlowPulseSpeed * 0.5f) * 0.05f);
                this.screenGlow.Modulate = new Color(0.2f, 1.0f, 0.2f, glowAlpha);
            }
        }
    }

    /// <summary>Handles terminal data sent signal and forwards to PTY.</summary>
    /// <param name="data">Data sent from terminal.</param>
    private void OnDataSent(string data)
    {
        this.ptyNode.Call("write", data);
        this.EmitSignal(SignalName.InputReceived, data);
    }

    /// <summary>Handles PTY data received signal and writes to terminal display.</summary>
    /// <param name="data">Data received from PTY.</param>
    private void OnDataReceived(string data)
    {
        this.terminalNode.Call("write", data);
    }

    /// <summary>Handles PTY process exit signal.</summary>
    /// <param name="exitCode">The exit code of the process.</param>
    /// <param name="signum">The signal number that terminated the process.</param>
    private void OnPtyExited(int exitCode, int signum)
    {
        // Note: Must remain instance method for signal connection
        _ = this;
        GD.PrintErr($"[TerminalUI] PTY process terminated with exit code: {exitCode}, signal: {signum}");
    }

    /// <summary>Handles terminal size changed signal and resizes PTY.</summary>
    /// <param name="size">New terminal size in columns and rows.</param>
    private void OnTerminalSizeChanged(Vector2I size)
    {
        this.ptyNode.Call("resizev", size.X, size.Y);
    }

    /// <summary>Plays ghost writing animation with fade and shimmer effects.</summary>
    /// <param name="text">Text to display with ghost effect.</param>
    private async Task PlayGhostWritingAnimation(string text)
    {
        // Display text with a ghost-like fading and shimmering effect
        var theme = (Godot.Theme?)this.overlayText.Theme.Duplicate() ?? new Godot.Theme();
        var originalColor = theme.GetColor("default_color", "RichTextLabel");

        // Rapid fade in with shimmer effect
        for (float alpha = 0.0f; alpha <= 1.0f; alpha += 0.2f)
        {
            // Add random shimmer to the alpha for ghost effect
            var shimmerAlpha = alpha + ((float)(new Random().NextDouble() - 0.5) * 0.1f);
            shimmerAlpha = Mathf.Clamp(shimmerAlpha, 0f, 1f);

            var fadeColor = new Color(originalColor.R, originalColor.G, originalColor.B, shimmerAlpha);
            theme.SetColor("default_color", "RichTextLabel", fadeColor);
            this.overlayText.Theme = theme;
            await this.ToSignal(this.GetTree().CreateTimer(0.02f), Godot.Timer.SignalName.Timeout);
        }

        this.overlayText.Text = text;
        this.terminalNode.Call("write", text + "\n");

        // Brief shimmer after text appears
        for (int i = 0; i < 5; i++)
        {
            var shimmerColor = new Color(
                originalColor.R + ((float)(new Random().NextDouble() - 0.5) * 0.1f),
                originalColor.G + ((float)(new Random().NextDouble() - 0.5) * 0.1f),
                originalColor.B + ((float)(new Random().NextDouble() - 0.5) * 0.1f),
                originalColor.A
            );
            theme.SetColor("default_color", "RichTextLabel", shimmerColor);
            this.overlayText.Theme = theme;
            await this.ToSignal(this.GetTree().CreateTimer(0.05f), Godot.Timer.SignalName.Timeout);
        }

        // Restore original color
        theme.SetColor("default_color", "RichTextLabel", originalColor);
        this.overlayText.Theme = theme;
    }

    /// <inheritdoc/>
    public override void _Process(double delta)
    {
        // Update 3D camera position for dynamic perspective and depth perception
        if (this.camera3D != null && this.terminalDisplayBox != null)
        {
            var time = (float)Time.GetTicksMsec() / 1000.0f;

            // Get the screen mesh position from the display box
            var screenPosition = this.terminalDisplayBox.GetScreenMesh().Position;

            // Enhanced camera movement for better depth perception
            this.camera3D.Position = new Vector3(
                (float)Math.Sin(time * 0.2) * 0.2f, // Subtle horizontal movement
                (float)Math.Cos(time * 0.15) * 0.1f, // Subtle vertical movement
                12.0f + ((float)Math.Sin(time * 0.4) * 0.3f) // Pulsing depth for perception
            );

            // Gentle rotation for more dynamic feel and depth
            this.camera3D.Rotation = new Vector3(
                (float)Math.Sin(time * 0.1) * 0.03f, // Subtle pitch movement
                (float)Math.Cos(time * 0.08) * 0.02f, // Subtle yaw movement
                0
            );

            // Enhance depth perception by adjusting camera properties dynamically
            var fovVariation = 25.0f + ((float)Math.Sin(time * 0.3) * 2.0f);
            this.camera3D.Fov = Mathf.Clamp(fovVariation, 20.0f, 35.0f);
        }

        // Update 3D positioning for enhanced elevation effect
        if (this.terminalDisplay3D != null)
        {
            var time = (float)Time.GetTicksMsec() / 1000.0f;

            // Subtle floating effect to enhance elevation perception
            var floatOffset = (float)Math.Sin(time * 0.5) * 0.05f;
            var newZPosition = 2.5f + floatOffset;

            // Apply subtle rotation for depth perception
            var rotationOffset = (float)Math.Sin(time * 0.2) * 0.01f;

            this.terminalDisplay3D.Position = new Vector3(
                (this.Size.X / 2) + ((float)Math.Sin(time * 0.1) * 0.1f),
                (this.Size.Y / 2) + ((float)Math.Cos(time * 0.12) * 0.1f),
                0
            );

            this.terminalDisplay3D.Rotation = new Vector3(
                -0.05f + rotationOffset,
                (float)Math.Sin(time * 0.05) * 0.01f,
                0
            );
        }
    }
}
