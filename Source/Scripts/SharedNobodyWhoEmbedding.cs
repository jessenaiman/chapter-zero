using Godot;
using System;
using System.Threading.Tasks;

namespace OmegaSpiral.Scripts;

/// <summary>
/// Singleton autoload node providing shared access to the NobodyWho embedding model.
/// Enables semantic similarity searches for natural language player input and creative content retrieval.
/// </summary>
/// <remarks>
/// <para>
/// Registered as autoload in project.godot: /root/SharedNobodyWhoEmbedding
/// </para>
/// <para>
/// Use cases in Omega Spiral:
/// - RAG pattern: Search creative content (YAML/MD) for relevant narrative material
/// - Natural language input: Convert player text to embeddings for intent matching
/// - Semantic choice selection: Find similar choices based on meaning vs exact text
/// - Memory retrieval: Find relevant narrative context from previous scenes
/// </para>
/// <para>
/// Uses the NobodyWho Godot plugin's GDScript embedding loader:
/// res://addons/nobodywho/embedding.gd
/// </para>
/// <para>
/// Architecture: CreativeMemoryRAG.cs will use this to implement the RAG pattern
/// for converting creative team content into LLM system prompts.
/// </para>
/// </remarks>
public partial class SharedNobodyWhoEmbedding : Node
{
    private Node? _embeddingNode;
    private bool _isInitialized;
    private TaskCompletionSource<bool>? _initializationTcs;

    /// <summary>
    /// Gets or sets the filesystem path to the GGUF embedding model file.
    /// </summary>
    /// <value>
    /// Default: res://models/qwen3-4b-instruct-2507-q4_k_m.gguf (same as chat model for simplicity)
    /// </value>
    /// <remarks>
    /// <para>
    /// For optimal performance, can use a smaller dedicated embedding model:
    /// - nomic-embed-text-v1.5-Q4_K_M.gguf (137M params, fast)
    /// - all-MiniLM-L6-v2-Q4_K_M.gguf (23M params, faster)
    /// </para>
    /// <para>
    /// Using the same model as chat simplifies deployment and reduces disk space.
    /// Qwen3 models support embedding extraction natively.
    /// </para>
    /// </remarks>
    [Export] public string ModelPath { get; set; } = "res://models/qwen3-4b-instruct-2507-q4_k_m.gguf";

    /// <summary>
    /// Gets the underlying NobodyWho embedding node instance.
    /// </summary>
    /// <returns>
    /// The GDScript embedding node from res://addons/nobodywho/embedding.gd,
    /// or <see langword="null"/> if not yet initialized.
    /// </returns>
    public Node? GetEmbeddingNode() => _embeddingNode;

    /// <summary>
    /// Gets a value indicating whether the embedding model has finished loading and is ready.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if model is loaded and ready; otherwise, <see langword="false"/>.
    /// </value>
    public bool IsInitialized => _isInitialized;

    /// <summary>
    /// Called when the node enters the scene tree.
    /// Automatically loads and initializes the embedding model on startup.
    /// </summary>
    public override void _Ready()
    {
        _initializationTcs = new TaskCompletionSource<bool>();
        _ = InitializeEmbeddingAsync();
    }

    /// <summary>
    /// Called when the node is removed from the scene tree.
    /// Cleans up the embedding node and associated resources.
    /// </summary>
    public override void _ExitTree()
    {
        if (_embeddingNode != null && IsInstanceValid(_embeddingNode))
        {
            _embeddingNode.QueueFree();
            _embeddingNode = null;
        }

        _isInitialized = false;
    }

    /// <summary>
    /// Waits asynchronously until the embedding model is fully loaded and ready.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> that completes when initialization is finished.
    /// </returns>
    /// <remarks>
    /// Use this method in RAG components to ensure the model is ready before querying:
    /// <code language="csharp">
    /// var embeddingNode = GetNode&lt;SharedNobodyWhoEmbedding&gt;("/root/SharedNobodyWhoEmbedding");
    /// await embeddingNode.WaitForInitializationAsync();
    /// var embedding = await embeddingNode.GetEmbeddingAsync("player chooses mercy");
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
    /// Generates a vector embedding for the given text.
    /// </summary>
    /// <param name="text">The text to embed.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation,
    /// with a <see cref="float"/> array result containing the embedding vector,
    /// or an empty array if embedding fails.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Embedding dimensions depend on the model:
    /// - Qwen3-4B: 3584 dimensions
    /// - nomic-embed-text: 768 dimensions
    /// - all-MiniLM-L6-v2: 384 dimensions
    /// </para>
    /// <para>
    /// Use embeddings for:
    /// - Cosine similarity: Compare two texts semantically
    /// - Vector search: Find similar content in a corpus
    /// - Clustering: Group related narrative elements
    /// </para>
    /// <example>
    /// <code language="csharp">
    /// // Compare player input to expected intents
    /// var playerEmbedding = await GetEmbeddingAsync("I want to help them");
    /// var mercyEmbedding = await GetEmbeddingAsync("show mercy");
    /// var similarity = CosineSimilarity(playerEmbedding, mercyEmbedding);
    /// </code>
    /// </example>
    /// </remarks>
    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        if (!_isInitialized || _embeddingNode == null)
        {
            GD.PrintErr("[SharedNobodyWhoEmbedding] Cannot generate embedding - model not initialized");
            return Array.Empty<float>();
        }

        try
        {
            // Call the GDScript embedding node's embed method
            // Returns Variant which should be Array of floats
            var result = _embeddingNode.Call("embed", text);

            // Convert GDScript Array to C# float[]
            if (result.VariantType == Variant.Type.Array)
            {
                var gdArray = result.AsGodotArray();
                var embedding = new float[gdArray.Count];

                for (int i = 0; i < gdArray.Count; i++)
                {
                    embedding[i] = gdArray[i].AsSingle();
                }

                return embedding;
            }

            GD.PrintErr("[SharedNobodyWhoEmbedding] Unexpected result type from embed(): " + result.VariantType);
            return Array.Empty<float>();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[SharedNobodyWhoEmbedding] Error generating embedding: {ex.Message}");
            return Array.Empty<float>();
        }
    }

    /// <summary>
    /// Computes the cosine similarity between two embedding vectors.
    /// </summary>
    /// <param name="a">The first embedding vector.</param>
    /// <param name="b">The second embedding vector.</param>
    /// <returns>
    /// A similarity score from -1.0 (opposite) to 1.0 (identical),
    /// where 0.0 indicates orthogonal (unrelated) vectors.
    /// Returns 0.0 if vectors have different lengths or are empty.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Cosine similarity formula: (A · B) / (||A|| × ||B||)
    /// </para>
    /// <para>
    /// Typical thresholds for semantic search:
    /// - 0.8-1.0: Very similar (near-duplicates)
    /// - 0.6-0.8: Related content
    /// - 0.4-0.6: Somewhat related
    /// - Below 0.4: Unrelated
    /// </para>
    /// </remarks>
    public static float CosineSimilarity(float[] a, float[] b)
    {
        if (a.Length == 0 || b.Length == 0 || a.Length != b.Length)
        {
            return 0.0f;
        }

        float dotProduct = 0.0f;
        float magnitudeA = 0.0f;
        float magnitudeB = 0.0f;

        for (int i = 0; i < a.Length; i++)
        {
            dotProduct += a[i] * b[i];
            magnitudeA += a[i] * a[i];
            magnitudeB += b[i] * b[i];
        }

        magnitudeA = (float)Math.Sqrt(magnitudeA);
        magnitudeB = (float)Math.Sqrt(magnitudeB);

        if (magnitudeA == 0.0f || magnitudeB == 0.0f)
        {
            return 0.0f;
        }

        return dotProduct / (magnitudeA * magnitudeB);
    }

    /// <summary>
    /// Loads and configures the NobodyWho embedding model asynchronously.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous initialization operation.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Steps performed:
    /// 1. Load res://addons/nobodywho/embedding.gd GDScript
    /// 2. Instantiate embedding node
    /// 3. Configure model_path
    /// 4. Add to scene tree as child of this autoload
    /// 5. Start embedding worker thread
    /// 6. Mark as initialized
    /// </para>
    /// <para>
    /// Error handling:
    /// - Missing addon: Logs error with installation instructions
    /// - Invalid model path: Logs error with path details
    /// </para>
    /// </remarks>
    private async Task InitializeEmbeddingAsync()
    {
        try
        {
            GD.PrintRich("[color=cyan][SharedNobodyWhoEmbedding][/color] Loading embedding model from: [color=yellow]" + ModelPath + "[/color]");

            // Load the NobodyWho embedding GDScript
            var embeddingScript = GD.Load<GDScript>("res://addons/nobodywho/embedding.gd");
            if (embeddingScript == null)
            {
                throw new InvalidOperationException(
                    "Failed to load res://addons/nobodywho/embedding.gd. " +
                    "Ensure NobodyWho plugin is installed in addons/nobodywho/");
            }

            // Instantiate the embedding node
            _embeddingNode = (Node)embeddingScript.New();
            if (_embeddingNode == null)
            {
                throw new InvalidOperationException("Failed to instantiate NobodyWho embedding node.");
            }

            // Configure model parameters
            _embeddingNode.Set("model_path", ModelPath);

            // Add to scene tree and start worker
            AddChild(_embeddingNode);

            // Call start_worker to begin model loading
            _embeddingNode.Call("start_worker");

            // Wait a frame to ensure model is loaded
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

            _isInitialized = true;
            _initializationTcs?.SetResult(true);

            GD.PrintRich(
                "[color=green][SharedNobodyWhoEmbedding][/color] Embedding model initialized successfully!\n" +
                $"  Path: [color=yellow]{ModelPath}[/color]"
            );
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[SharedNobodyWhoEmbedding] Initialization failed: {ex.Message}");
            GD.PrintErr($"Stack trace: {ex.StackTrace}");

            _isInitialized = false;
            _initializationTcs?.SetException(ex);
        }
    }
}
