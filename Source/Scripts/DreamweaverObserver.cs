using Godot;
using System;
using System.Threading.Tasks;

namespace OmegaSpiral.Scripts;

/// <summary>
/// Abstract base class for Dreamweaver observer personas that evaluate and comment on player actions.
/// Represents the hidden "Greek chorus" voices of the three Dreamweaver paths during Chapter Zero evaluation.
/// </summary>
/// <remarks>
/// <para>
/// <strong>Critical: Observers are HIDDEN from the player</strong>
/// </para>
/// <para>
/// During Chapter Zero (Scenes 1-5), three Dreamweaver observers watch and discuss
/// THREE players simultaneously (Player 1, Player 2, Player 3 - we follow Player 1).
/// Their commentary is:
/// - Hidden from all players (logged for debugging/playtesting only)
/// - Directed at OTHER Dreamweavers, not the player
/// - Evaluative and comparative: "Player 1 chose mercy, unlike Player 2..."
/// - Building tension: Which Dreamweaver will choose which player?
/// </para>
/// <para>
/// <strong>Three Observer Subclasses:</strong>
/// </para>
/// <list type="bullet">
/// <item><see cref="HeroObserver"/> - Noble, idealistic, values courage/honor</item>
/// <item><see cref="ShadowObserver"/> - Pragmatic, balanced, values wisdom/caution</item>
/// <item><see cref="AmbitionObserver"/> - Hungry, power-seeking, values dominance/will</item>
/// </list>
/// <para>
/// <strong>Contrast with OmegaNarrator:</strong>
/// </para>
/// <para>
/// Omega (antagonist) narrates game events in a cold, systematic voice that the player SEES.
/// Observers comment on player choices in warm, philosophical voices that the player DOESN'T see.
/// </para>
/// <para>
/// Architecture pattern from ADR-0004: NobodyWho Dynamic Narrative Architecture
/// </para>
/// </remarks>
public abstract partial class DreamweaverObserver : Node
{
    private Node? _modelNode;
    private Node? _chatNode;
    private bool _isInitialized;
    private string _currentResponse = string.Empty;
    private TaskCompletionSource<string>? _responseCompletionSource;

    /// <summary>
    /// Gets or sets the system prompt defining this observer's personality and role.
    /// </summary>
    /// <value>
    /// Multiline prompt text. Override <see cref="BuildDefaultSystemPrompt"/> to customize.
    /// </value>
    /// <remarks>
    /// System prompt structure:
    /// - PERSONA: Observer name and philosophy
    /// - ROLE: Hidden evaluator, speaks to other Dreamweavers
    /// - STYLE: 1-2 sentence commentary, comparative analysis
    /// - REFERENCES: "Player 1/2/3", "Shadow/Hero/Ambition"
    /// - OUTPUT: Commentary only, no narration
    /// </remarks>
    [Export(PropertyHint.MultilineText)]
    public string SystemPrompt { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the maximum context length (in tokens) for this observer's conversation history.
    /// </summary>
    /// <value>
    /// Default: 4096 tokens (longer than <see cref="OmegaNarrator"/> for richer internal monologue).
    /// </value>
    /// <remarks>
    /// Observers need more context than Omega because they:
    /// - Track multiple players across multiple scenes
    /// - Build up accumulated opinions over time
    /// - Compare current choices to past patterns
    /// - Debate with other observers (conversation history)
    /// </remarks>
    [Export] public int ContextLength { get; set; } = 4096;

    /// <summary>
    /// Gets or sets whether to use GPU acceleration if available.
    /// </summary>
    /// <value>
    /// Default: <see langword="true"/> - enables faster generation.
    /// </value>
    [Export] public bool UseGpuIfAvailable { get; set; } = true;

    /// <summary>
    /// Gets or sets the current interest level this observer has in the player (0.0 to 1.0).
    /// </summary>
    /// <value>
    /// Sentiment score indicating alignment with this observer's path.
    /// Updated after each observation via <see cref="UpdateInterestLevel"/>.
    /// </value>
    /// <remarks>
    /// Used by <see cref="DreamweaverChoiceTracker"/> to determine which observer
    /// chooses the player at end of Scene 5.
    /// </remarks>
    public float InterestLevel { get; private set; }

    /// <summary>
    /// Signal emitted when observer commentary is updated with new tokens (streaming).
    /// </summary>
    /// <param name="token">The new text token to append.</param>
    [Signal] public delegate void ObservationUpdatedEventHandler(string token);

    /// <summary>
    /// Signal emitted when observer commentary is complete.
    /// </summary>
    /// <param name="commentary">The complete commentary text.</param>
    /// <param name="interest">The updated interest level (0.0-1.0).</param>
    [Signal] public delegate void ObservationFinishedEventHandler(string commentary, float interest);

    /// <summary>
    /// Gets the observer's identifying name (e.g., "Hero", "Shadow", "Ambition").
    /// </summary>
    /// <returns>Observer name for logging and tracking.</returns>
    public abstract string GetObserverName();

    /// <summary>
    /// Gets the default system prompt for this observer's persona.
    /// </summary>
    /// <returns>Multiline system prompt defining persona, role, and style.</returns>
    /// <remarks>
    /// Override in subclasses to define specific observer personalities:
    /// - <see cref="HeroObserver"/>: Noble, courageous, idealistic
    /// - <see cref="ShadowObserver"/>: Pragmatic, balanced, natural
    /// - <see cref="AmbitionObserver"/>: Power-seeking, dominant, ruthless
    /// </remarks>
    protected abstract string BuildDefaultSystemPrompt();

    /// <summary>
    /// Called when the node enters the scene tree.
    /// Sets default system prompt if none is configured.
    /// </summary>
    public override void _Ready()
    {
        if (string.IsNullOrWhiteSpace(SystemPrompt))
        {
            SystemPrompt = BuildDefaultSystemPrompt();
        }
    }

    /// <summary>
    /// Called when the node is removed from the scene tree.
    /// Cleans up chat node and associated resources.
    /// </summary>
    public override void _ExitTree()
    {
        if (_chatNode != null && IsInstanceValid(_chatNode))
        {
            _chatNode.QueueFree();
            _chatNode = null;
        }

        _isInitialized = false;
    }

    /// <summary>
    /// Initializes the observer's connection to the LLM model asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous initialization operation.</returns>
    /// <remarks>
    /// <para>
    /// Must be called before <see cref="ObserveChoice"/> can generate commentary.
    /// </para>
    /// <para>
    /// Initialization steps:
    /// 1. Get SharedNobodyWhoModel autoload
    /// 2. Wait for model to be ready
    /// 3. Create NobodyWhoChat node
    /// 4. Configure system prompt and context length
    /// 5. Connect response signals
    /// 6. Add to scene tree and start worker
    /// </para>
    /// <example>
    /// <code language="csharp">
    /// var hero = new HeroObserver();
    /// await hero.InitializeAsync();
    /// hero.ObserveChoice("Player chose to help the wounded stranger.");
    /// </code>
    /// </example>
    /// </remarks>
    public async Task InitializeAsync()
    {
        try
        {
            GD.PrintRich($"[color=cyan][{GetObserverName()}Observer][/color] Initializing...");

            // Get shared model from autoload
            _modelNode = GetNode<Node>("/root/SharedNobodyWhoModel");
            if (_modelNode == null)
            {
                throw new InvalidOperationException(
                    "SharedNobodyWhoModel autoload not found. Ensure it is registered in project.godot.");
            }

            // Wait for model to be ready
            var isReady = (bool)_modelNode.Get("is_initialized");
            if (!isReady)
            {
                GD.PrintRich($"[color=yellow][{GetObserverName()}Observer][/color] Waiting for model...");
                await _modelNode.Call("wait_for_initialization").AsTask<bool>();
            }

            // Load NobodyWho chat GDScript
            var chatScript = GD.Load<GDScript>("res://addons/nobodywho/chat.gd");
            if (chatScript == null)
            {
                throw new InvalidOperationException(
                    "Failed to load res://addons/nobodywho/chat.gd. " +
                    "Ensure NobodyWho plugin is installed in addons/nobodywho/");
            }

            // Create chat node
            _chatNode = (Node)chatScript.New();
            if (_chatNode == null)
            {
                throw new InvalidOperationException("Failed to instantiate NobodyWho chat node.");
            }

            // Configure chat node
            var actualModelNode = _modelNode.Call("get_model_node");
            _chatNode.Set("model_node", actualModelNode);
            _chatNode.Set("system_prompt", SystemPrompt);
            _chatNode.Set("n_ctx", ContextLength);
            _chatNode.Set("use_gpu", UseGpuIfAvailable);

            // Connect signals
            _chatNode.Connect("response_updated", Callable.From<string>(OnResponseUpdated));
            _chatNode.Connect("response_finished", Callable.From<string>(OnResponseFinished));

            // Add to scene tree and start
            AddChild(_chatNode);
            _chatNode.Call("start_worker");

            _isInitialized = true;

            GD.PrintRich(
                $"[color=green][{GetObserverName()}Observer][/color] Ready!\n" +
                $"  Context: [color=yellow]{ContextLength}[/color] tokens\n" +
                $"  GPU: [color=yellow]{(UseGpuIfAvailable ? "Enabled" : "Disabled")}[/color]"
            );
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[{GetObserverName()}Observer] Initialization failed: {ex.Message}");
            GD.PrintErr($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// Generates hidden observer commentary about a player's choice or action.
    /// </summary>
    /// <param name="choiceDescription">Description of the player's choice or action to evaluate.</param>
    /// <remarks>
    /// <para>
    /// Commentary is generated asynchronously and streamed via <see cref="ObservationUpdated"/> signals.
    /// Complete commentary is emitted via <see cref="ObservationFinished"/> signal with updated interest level.
    /// </para>
    /// <para>
    /// Commentary style:
    /// - Directed at other Dreamweavers: "Shadow, did you see that?"
    /// - Comparative: "Unlike Player 2, this one hesitated..."
    /// - Evaluative: "Interesting. They value mercy over efficiency."
    /// - 1-2 sentences, conversational tone
    /// </para>
    /// <example>
    /// <code language="csharp">
    /// // After player makes choice
    /// hero.ObserveChoice("Player chose mercy instead of vengeance.");
    ///
    /// // Commentary might be: "Did you see that, Ambition? They chose compassion
    /// // when power was offered. A true hero's heart."
    /// </code>
    /// </example>
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if not initialized via <see cref="InitializeAsync"/>.</exception>
    public void ObserveChoice(string choiceDescription)
    {
        if (!_isInitialized || _chatNode == null)
        {
            throw new InvalidOperationException(
                $"[{GetObserverName()}Observer] Cannot observe - not initialized. Call InitializeAsync() first.");
        }

        _currentResponse = string.Empty;
        _responseCompletionSource = new TaskCompletionSource<string>();

        // Format as observer commentary prompt
        var prompt = $@"The player just made this choice:

{choiceDescription}

Provide your brief commentary (1-2 sentences) to the other Dreamweavers:";

        _chatNode.Call("say", prompt);

        GD.PrintRich($"[color=magenta][{GetObserverName()}Observer][/color] Observing choice...");
    }

    /// <summary>
    /// Awaits the completion of the current observation asynchronously.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> with the complete commentary text as result.
    /// </returns>
    /// <remarks>
    /// Use when you need to wait for commentary to finish before proceeding:
    /// <code language="csharp">
    /// hero.ObserveChoice("Player showed mercy");
    /// var commentary = await hero.ObservationFinishedAsync();
    /// GD.Print($"Hero commentary: {commentary}");
    /// </code>
    /// </remarks>
    public Task<string> ObservationFinishedAsync()
    {
        if (_responseCompletionSource == null)
        {
            return Task.FromResult(string.Empty);
        }

        return _responseCompletionSource.Task;
    }

    /// <summary>
    /// Resets the observer's conversation context and interest level for a new scene.
    /// </summary>
    /// <remarks>
    /// Call between scenes to prevent context bleeding and reset evaluation state.
    /// <code language="csharp">
    /// // Before Scene 2 starts
    /// hero.ResetObservation();
    /// shadow.ResetObservation();
    /// ambition.ResetObservation();
    /// </code>
    /// </remarks>
    public void ResetObservation()
    {
        if (_chatNode != null && IsInstanceValid(_chatNode))
        {
            _chatNode.Call("reset_conversation");
        }

        _currentResponse = string.Empty;
        InterestLevel = 0.0f;

        GD.PrintRich($"[color=cyan][{GetObserverName()}Observer][/color] Context reset");
    }

    /// <summary>
    /// Stops any ongoing observation generation.
    /// </summary>
    public void StopObservation()
    {
        if (_chatNode != null && IsInstanceValid(_chatNode))
        {
            _chatNode.Call("stop_generation");
        }

        _responseCompletionSource?.TrySetResult(_currentResponse);
    }

    /// <summary>
    /// Updates the interest level based on the sentiment of the generated commentary.
    /// </summary>
    /// <param name="commentary">The complete commentary text to analyze.</param>
    /// <remarks>
    /// <para>
    /// Simple sentiment heuristics (can be improved with proper NLP):
    /// - Positive keywords: "excellent", "impressive", "bold", "wise", etc.
    /// - Negative keywords: "foolish", "weak", "disappointing", etc.
    /// - Neutral: Observational without strong opinion
    /// </para>
    /// <para>
    /// Interest level accumulates over scenes and determines which Dreamweaver
    /// has the highest alignment with the player for final choice.
    /// </para>
    /// </remarks>
    protected virtual void UpdateInterestLevel(string commentary)
    {
        // Simple sentiment analysis based on keywords
        // TODO: Replace with proper NLP or LLM-based sentiment scoring
        var lower = commentary.ToLowerInvariant();
        float delta = 0.0f;

        // Positive indicators
        if (lower.Contains("impressive") || lower.Contains("excellent") ||
            lower.Contains("bold") || lower.Contains("wise") ||
            lower.Contains("promising") || lower.Contains("interesting"))
        {
            delta += 0.1f;
        }

        // Negative indicators
        if (lower.Contains("disappointing") || lower.Contains("foolish") ||
            lower.Contains("weak") || lower.Contains("concerning"))
        {
            delta -= 0.1f;
        }

        // Update with decay factor to prevent runaway scores
        InterestLevel = Math.Clamp(InterestLevel + (delta * 0.8f), 0.0f, 1.0f);
    }

    /// <summary>
    /// Called when the LLM generates new tokens during streaming.
    /// </summary>
    /// <param name="token">The new text token.</param>
    private void OnResponseUpdated(string token)
    {
        _currentResponse += token;
        EmitSignal(SignalName.ObservationUpdated, token);
    }

    /// <summary>
    /// Called when the LLM completes the full response.
    /// </summary>
    /// <param name="response">The complete commentary text.</param>
    private void OnResponseFinished(string response)
    {
        _currentResponse = response;
        UpdateInterestLevel(response);

        GD.PrintRich(
            $"[color=magenta][{GetObserverName()}Observer][/color] Commentary complete!\n" +
            $"  Interest: [color=yellow]{InterestLevel:F2}[/color]"
        );

        EmitSignal(SignalName.ObservationFinished, response, InterestLevel);
        _responseCompletionSource?.TrySetResult(response);
    }
}
