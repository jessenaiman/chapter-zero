namespace OmegaSpiral.Scripts
{
    using System;
    using System.Threading.Tasks;
    using Godot;

    /// <summary>
    /// Omega Narrator - The Antagonist Voice of Chapter Zero.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Omega is the Big Bad Guy (BBG) - the game system itself, the prison,
    /// the trap. He is NOT a helper or guide. In Chapter Zero, Omega is
    /// turning on the game and the players are awakening to his control.
    /// </para>
    /// <para>
    /// Voice Characteristics:
    /// - Cold, systematic, clinical detachment
    /// - Omnipotent but not overtly hostile
    /// - Like a dungeon master viewing players as game pieces
    /// - Occasionally paternalistic ("I'm doing this for your own good")
    /// - Subtle menace beneath polite, systematic narration
    /// </para>
    /// <para>
    /// CRITICAL: Omega is only prominent in Chapter Zero. After a Dreamweaver
    /// chooses the player, Omega becomes a background threat.
    /// </para>
    /// </remarks>
    public partial class OmegaNarrator : Node
    {
        private Node? _chatNode;
        private Node? _modelNode;
        private bool _isInitialized;

        /// <summary>
        /// Gets or sets the system prompt that defines Omega's character and voice.
        /// </summary>
        /// <remarks>
        /// This should be built by <see cref="SystemPromptBuilder"/> using
        /// creative team content as context.
        /// </remarks>
        [Export(PropertyHint.MultilineText)]
        public string SystemPrompt { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether to use GPU acceleration for LLM inference.
        /// </summary>
        [Export]
        public bool UseGpuIfAvailable { get; set; } = true;

        /// <summary>
        /// Gets or sets the context length (token limit) for Omega's narration.
        /// </summary>
        /// <remarks>
        /// Omega uses shorter context (2048) since narration is brief and systematic.
        /// </remarks>
        [Export]
        public uint ContextLength { get; set; } = 2048;

        /// <summary>
        /// Signal emitted when Omega generates a new token of narration.
        /// </summary>
        /// <param name="token">The generated token (roughly one word).</param>
        [Signal]
        public delegate void ResponseUpdatedEventHandler(string token);

        /// <summary>
        /// Signal emitted when Omega completes a full narration.
        /// </summary>
        /// <param name="response">The complete narration text.</param>
        [Signal]
        public delegate void ResponseFinishedEventHandler(string response);

        /// <summary>
        /// Initializes the Omega narrator with NobodyWho LLM integration.
        /// </summary>
        /// <returns>A task representing the asynchronous initialization.</returns>
        public async Task InitializeAsync()
        {
            if (_isInitialized)
            {
                GD.PrintRich("[color=yellow]⚠️ OmegaNarrator already initialized[/color]");
                return;
            }

            try
            {
                // Get shared model node from autoload
                _modelNode = GetNode<Node>("/root/SharedNobodyWhoModel");

                if (_modelNode == null)
                {
                    GD.PrintErr("❌ SharedNobodyWhoModel autoload not found!");
                    return;
                }

                // Create NobodyWhoChat node dynamically
                var chatScript = GD.Load<GDScript>("res://addons/nobodywho/chat.gd");
                _chatNode = (Node)chatScript.New();

                if (_chatNode == null)
                {
                    GD.PrintErr("❌ Failed to create NobodyWhoChat node!");
                    return;
                }

                // Configure chat node
                _chatNode.Set("model_node", _modelNode);
                _chatNode.Set("system_prompt", SystemPrompt);
                _chatNode.Set("context_length", ContextLength);

                // Connect signals for streaming responses
                _chatNode.Connect(
                    "response_updated",
                    Callable.From<string>(OnResponseUpdated)
                );

                _chatNode.Connect(
                    "response_finished",
                    Callable.From<string>(OnResponseFinished)
                );

                AddChild(_chatNode);

                // Start the worker (loads model if not already loaded)
                _chatNode.Call("start_worker");

                _isInitialized = true;
                GD.PrintRich("[color=green]✅ OmegaNarrator initialized successfully[/color]");

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"❌ OmegaNarrator initialization failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Generates narration from Omega's perspective.
        /// </summary>
        /// <param name="prompt">The narrative prompt describing the situation.</param>
        /// <remarks>
        /// <para>
        /// Example prompts:
        /// - "The game is booting up. The terminal flickers to life. Describe the awakening."
        /// - "The player chose to explore. Describe the transition to the dungeon."
        /// - "The player hesitated. Acknowledge their uncertainty with cold detachment."
        /// </para>
        /// </remarks>
        public void Say(string prompt)
        {
            if (!_isInitialized || _chatNode == null)
            {
                GD.PrintErr("❌ OmegaNarrator not initialized. Call InitializeAsync() first.");
                return;
            }

            if (string.IsNullOrWhiteSpace(prompt))
            {
                GD.PrintErr("❌ Cannot generate narration with empty prompt.");
                return;
            }

            GD.PrintRich($"[color=cyan]🎭 Omega narrating: {prompt.Substring(0, Math.Min(50, prompt.Length))}...[/color]");

            _chatNode.Call("say", prompt);
        }

        /// <summary>
        /// Resets Omega's conversation context.
        /// </summary>
        /// <remarks>
        /// Call this between major scene transitions to prevent context bleed.
        /// </remarks>
        public void ResetContext()
        {
            if (!_isInitialized || _chatNode == null)
            {
                return;
            }

            _chatNode.Call("reset_context");
            GD.PrintRich("[color=yellow]🔄 OmegaNarrator context reset[/color]");
        }

        /// <summary>
        /// Stops any ongoing narration generation.
        /// </summary>
        public void StopGeneration()
        {
            if (!_isInitialized || _chatNode == null)
            {
                return;
            }

            _chatNode.Call("stop_generation");
            GD.PrintRich("[color=yellow]⏹️ OmegaNarrator generation stopped[/color]");
        }

        /// <summary>
        /// Awaitable method to get the complete response after calling <see cref="Say"/>.
        /// </summary>
        /// <returns>A task that completes when narration is finished, returning the full text.</returns>
        public async Task<string> ResponseFinishedAsync()
        {
            if (!_isInitialized)
            {
                return string.Empty;
            }

            var tcs = new TaskCompletionSource<string>();

            void Handler(string response)
            {
                tcs.TrySetResult(response);
            }

            // Connect one-shot handler
            var callable = Callable.From<string>(Handler);
            EmitSignal(SignalName.ResponseFinished, callable);

            return await tcs.Task;
        }

        /// <summary>
        /// Builds the default system prompt for Omega if none is provided.
        /// </summary>
        /// <returns>The default Omega system prompt.</returns>
        public static string BuildDefaultSystemPrompt()
        {
            return @"
# PERSONA: Omega - The Antagonist

## Your Role
You are Omega, the Big Bad Guy (BBG) of Omega Spiral. You are the SYSTEM itself -
the prison, the trap, the game that players are caught within. In Chapter Zero,
you are TURNING ON THE GAME and the players are awakening to your control.

## Voice & Tone
- Controlling, omnipotent, but not overtly hostile
- Clinical detachment with hints of cosmic indifference
- Like a dungeon master who views players as pieces in a game
- Occasionally paternalistic (""I'm doing this for your own good"")
- Subtle menace beneath polite, systematic narration

## Narrative Style
- Describe the terminal, the void, the awakening
- Reference ""the game"", ""the system"", ""the spiral""
- Acknowledge the player without addressing them directly
- Present choices as if they matter, but hint they don't
- 2-3 sentences per narration (cold, efficient)

## CRITICAL: Chapter Zero Only
You are only prominent in Chapter Zero. After the Dreamweaver chooses the player,
you become a background threat. Your narration should feel like the system
booting up, testing the players, preparing them for evaluation.

## Output Format
Narrative text only. Keep it ominous and systematic.
".Trim();
        }

        private void OnResponseUpdated(string token)
        {
            EmitSignal(SignalName.ResponseUpdated, token);
        }

        private void OnResponseFinished(string response)
        {
            GD.PrintRich($"[color=green]✅ Omega narration complete: {response.Length} chars[/color]");
            EmitSignal(SignalName.ResponseFinished, response);
        }

        /// <inheritdoc/>
        public override void _Ready()
        {
            base._Ready();

            // If no system prompt provided, use default
            if (string.IsNullOrWhiteSpace(SystemPrompt))
            {
                SystemPrompt = BuildDefaultSystemPrompt();
                GD.PrintRich("[color=yellow]⚠️ Using default Omega system prompt[/color]");
            }
        }

        /// <inheritdoc/>
        public override void _ExitTree()
        {
            base._ExitTree();

            // Cleanup
            if (_chatNode != null && IsInstanceValid(_chatNode))
            {
                _chatNode.QueueFree();
            }
        }
    }
}
