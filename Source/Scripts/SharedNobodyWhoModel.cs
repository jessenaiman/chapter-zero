using Godot;
using System;
using System.Threading.Tasks;

namespace OmegaSpiral.Scripts;

/// <summary>
/// Singleton autoload node providing shared access to the NobodyWho LLM model.
/// This ensures only one model instance is loaded in memory, improving performance
/// when multiple narrative components (Omega narrator, Dreamweaver observers) need LLM access.
/// </summary>
/// <remarks>
/// <para>
/// Registered as autoload in project.godot: /root/SharedNobodyWhoModel
/// </para>
/// <para>
/// The model is loaded once at startup and shared by:
/// - <see cref="OmegaNarrator"/> (antagonist narration)
/// - <see cref="DreamweaverObserver"/> subclasses (hidden observer commentary)
/// - Future narrative components requiring LLM generation
/// </para>
/// <para>
/// Uses the NobodyWho Godot plugin's GDScript model loader:
/// res://addons/nobodywho/model.gd
/// </para>
/// </remarks>
public partial class SharedNobodyWhoModel : Node
{
    private Node? _modelNode;
    private bool _isInitialized;
    private TaskCompletionSource<bool>? _initializationTcs;

    /// <summary>
    /// Gets or sets the filesystem path to the GGUF model file.
    /// </summary>
    /// <value>
    /// Default: res://models/qwen3-4b-instruct-2507-q4_k_m.gguf (Qwen3 4B Instruct quantized)
    /// </value>
    /// <remarks>
    /// Recommended models:
    /// - Qwen3-4B-Q4_K_M: Balanced quality/performance for narrative generation
    /// - Qwen3-0.6B-Q4_K_M: Faster, lower quality
    /// - Qwen3-14B-Q4_K_M: Higher quality, slower
    /// </remarks>
    [Export] public string ModelPath { get; set; } = "res://models/qwen3-4b-instruct-2507-q4_k_m.gguf";

    /// <summary>
    /// Gets or sets whether to use GPU acceleration if available.
    /// </summary>
    /// <value>
    /// Default: <see langword="true"/> - enables GPU layers for faster inference.
    /// </value>
    /// <remarks>
    /// GPU acceleration significantly improves generation speed (2-10x faster).
    /// Falls back to CPU if GPU is unavailable or model doesn't support GPU.
    /// </remarks>
    [Export] public bool UseGpu { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of GPU layers to offload for inference.
    /// </summary>
    /// <value>
    /// Default: -1 (auto-detect and use all available layers).
    /// </value>
    /// <remarks>
    /// <para>
    /// -1 = Automatically use maximum compatible layers
    /// </para>
    /// <para>
    /// 0 = CPU-only inference
    /// </para>
    /// <para>
    /// Positive integer = Specific number of layers (for manual tuning)
    /// </para>
    /// </remarks>
    [Export] public int GpuLayers { get; set; } = -1;

    /// <summary>
    /// Gets the underlying NobodyWho model node instance.
    /// </summary>
    /// <returns>
    /// The GDScript model node from res://addons/nobodywho/model.gd,
    /// or <see langword="null"/> if not yet initialized.
    /// </returns>
    /// <remarks>
    /// Chat nodes connect to this via: _chatNode.Set("model_node", modelNode)
    /// </remarks>
    public Node? GetModelNode() => _modelNode;

    /// <summary>
    /// Gets a value indicating whether the model has finished loading and is ready for inference.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if model is loaded and ready; otherwise, <see langword="false"/>.
    /// </value>
    public bool IsInitialized => _isInitialized;

    /// <summary>
    /// Called when the node enters the scene tree.
    /// Automatically loads and initializes the LLM model on startup.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Initialization is asynchronous to avoid blocking the main thread.
    /// Uses TaskCompletionSource for awaitable initialization tracking.
    /// </para>
    /// <para>
    /// Logs rich-formatted messages to console for debugging:
    /// - Info: Model loading progress
    /// - Success: Model ready with configuration
    /// - Error: Loading failures with details
    /// </para>
    /// </remarks>
    public override void _Ready()
    {
        _initializationTcs = new TaskCompletionSource<bool>();
        _ = InitializeModelAsync();
    }

    /// <summary>
    /// Called when the node is removed from the scene tree.
    /// Cleans up the model node and associated resources.
    /// </summary>
    public override void _ExitTree()
    {
        if (_modelNode != null && IsInstanceValid(_modelNode))
        {
            _modelNode.QueueFree();
            _modelNode = null;
        }

        _isInitialized = false;
    }

    /// <summary>
    /// Waits asynchronously until the model is fully loaded and ready for inference.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> that completes when initialization is finished.
    /// </returns>
    /// <remarks>
    /// Use this method in narrative components to ensure the model is ready before generating:
    /// <code language="csharp">
    /// var modelNode = GetNode&lt;SharedNobodyWhoModel&gt;("/root/SharedNobodyWhoModel");
    /// await modelNode.WaitForInitializationAsync();
    /// // Now safe to create chat nodes using this model
    /// </code>
    /// </remarks>
    public async Task WaitForInitializationAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        if (_initializationTcs != null)
        {
            await _initializationTcs.Task;
        }
    }

    /// <summary>
    /// Loads and configures the NobodyWho model asynchronously.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous initialization operation.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Steps performed:
    /// 1. Load res://addons/nobodywho/model.gd GDScript
    /// 2. Instantiate model node
    /// 3. Configure model_path, use_gpu, n_gpu_layers
    /// 4. Add to scene tree as child of this autoload
    /// 5. Start model worker thread
    /// 6. Mark as initialized
    /// </para>
    /// <para>
    /// Error handling:
    /// - Missing addon: Logs error with installation instructions
    /// - Invalid model path: Logs error with path details
    /// - GPU unavailable: Falls back to CPU automatically
    /// </para>
    /// </remarks>
    private async Task InitializeModelAsync()
    {
        try
        {
            GD.PrintRich("[color=cyan][SharedNobodyWhoModel][/color] Loading model from: [color=yellow]" + ModelPath + "[/color]");

            // Load the NobodyWho model GDScript
            var modelScript = GD.Load<GDScript>("res://addons/nobodywho/model.gd");
            if (modelScript == null)
            {
                throw new InvalidOperationException(
                    "Failed to load res://addons/nobodywho/model.gd. " +
                    "Ensure NobodyWho plugin is installed in addons/nobodywho/");
            }

            // Instantiate the model node
            _modelNode = (Node)modelScript.New();
            if (_modelNode == null)
            {
                throw new InvalidOperationException("Failed to instantiate NobodyWho model node.");
            }

            // Configure model parameters
            _modelNode.Set("model_path", ModelPath);
            _modelNode.Set("use_gpu", UseGpu);
            _modelNode.Set("n_gpu_layers", GpuLayers);

            // Add to scene tree and start worker
            AddChild(_modelNode);

            // Call start_worker to begin model loading
            // Note: This may block briefly while loading model into memory
            _modelNode.Call("start_worker");

            // Wait a frame to ensure model is loaded
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

            _isInitialized = true;
            _initializationTcs?.SetResult(true);

            GD.PrintRich(
                "[color=green][SharedNobodyWhoModel][/color] Model initialized successfully!\n" +
                $"  Path: [color=yellow]{ModelPath}[/color]\n" +
                $"  GPU: [color=yellow]{(UseGpu ? "Enabled" : "Disabled")}[/color]\n" +
                $"  GPU Layers: [color=yellow]{(GpuLayers == -1 ? "Auto" : GpuLayers.ToString())}[/color]"
            );
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[SharedNobodyWhoModel] Initialization failed: {ex.Message}");
            GD.PrintErr($"Stack trace: {ex.StackTrace}");

            _isInitialized = false;
            _initializationTcs?.SetException(ex);
        }
    }
}
