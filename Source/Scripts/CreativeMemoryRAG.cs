using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Scripts;

// Stub namespace for NobodyWho types until GDExtension is loaded
namespace NobodyWho
{
    /// <summary>
    /// Placeholder for SharedNobodyWhoEmbedding from NobodyWho GDExtension.
    /// This will be replaced by the actual implementation at runtime.
    /// </summary>
    public class SharedNobodyWhoEmbedding : Godot.GodotObject
    {
    }
}

namespace OmegaSpiral
{
/// <summary>
/// Retrieval-Augmented Generation (RAG) system for creative schema content.
/// Indexes narrative beats from scene schemas and provides semantic search
/// to surface relevant context for LLM prompts.
/// </summary>
public partial class CreativeMemoryRAG : Node
{
    private NobodyWho.SharedNobodyWhoEmbedding? embeddingService;
    private List<NarrativeBeat> indexedBeats = new();
    private bool isIndexed = false;

    /// <summary>
    /// Represents an indexed narrative beat with its embedding.
    /// </summary>
    private record NarrativeBeat
    {
        public required string StepId { get; init; }
        public required string StepType { get; init; }
        public required string Content { get; init; }
        public required float[] Embedding { get; init; }
        public required string SceneId { get; init; }
    }

    /// <summary>
    /// Represents a RAG search result.
    /// </summary>
    public record RAGResult
    {
        public required string StepId { get; init; }
        public required string Content { get; init; }
        public required float Similarity { get; init; }
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.embeddingService = this.GetNode<SharedNobodyWhoEmbedding>("/root/SharedNobodyWhoEmbedding");
        GD.Print("CreativeMemoryRAG initialized");
    }

    /// <summary>
    /// Indexes all narrative beats from a scene schema JSON file.
    /// </summary>
    /// <param name="schemaPath">Path to the scene schema JSON (e.g., "res://docs/scenes/scene1-schema.json").</param>
    /// <returns>True if indexing succeeded, false otherwise.</returns>
    public async Task<bool> IndexSchemaAsync(string schemaPath)
    {
        if (this.embeddingService == null)
        {
            GD.PrintErr("Embedding service not available");
            return false;
        }

        try
        {
            // Load schema file
            if (!Godot.FileAccess.FileExists(schemaPath))
            {
                GD.PrintErr($"Schema file not found: {schemaPath}");
                return false;
            }

            using var file = Godot.FileAccess.Open(schemaPath, Godot.FileAccess.ModeFlags.Read);
            if (file == null)
            {
                GD.PrintErr($"Failed to open schema file: {schemaPath}");
                return false;
            }

            var schemaJson = file.GetAsText();
            using var schemaDoc = JsonDocument.Parse(schemaJson);
            var root = schemaDoc.RootElement;

            if (!root.TryGetProperty("steps", out var stepsArray))
            {
                GD.PrintErr("Schema missing 'steps' array");
                return false;
            }

            var sceneId = root.TryGetProperty("sceneId", out var sceneIdProp)
                ? sceneIdProp.GetString() ?? "unknown"
                : "unknown";
            this.indexedBeats.Clear();

            // Index each step
            foreach (var stepElement in stepsArray.EnumerateArray())
            {
                var stepId = stepElement.GetProperty("id").GetString() ?? "unknown";
                var stepType = stepElement.GetProperty("type").GetString() ?? "unknown";

                // Extract content based on step type
                var content = this.ExtractStepContent(stepElement, stepType);
                if (string.IsNullOrWhiteSpace(content))
                {
                    continue;
                }

                // Generate embedding for this beat
                var embedding = await this.embeddingService.GetEmbeddingAsync(content).ConfigureAwait(false);
                if (embedding == null || embedding.Length == 0)
                {
                    GD.PrintErr($"Failed to generate embedding for step: {stepId}");
                    continue;
                }

                this.indexedBeats.Add(new NarrativeBeat
                {
                    StepId = stepId,
                    StepType = stepType,
                    Content = content,
                    Embedding = embedding,
                    SceneId = sceneId,
                });

                GD.Print($"Indexed beat: {stepId} ({content.Length} chars)");
            }

            this.isIndexed = true;
            GD.Print($"Indexing complete: {this.indexedBeats.Count} beats from {sceneId}");
            return true;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to index schema: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Finds narrative beats related to a given step ID.
    /// Uses semantic similarity search via embeddings.
    /// </summary>
    /// <param name="queryStepId">The step ID to find related content for.</param>
    /// <param name="topK">Number of top results to return.</param>
    /// <returns>List of related narrative beats, sorted by similarity.</returns>
    public Task<List<RAGResult>> FindRelatedBeatsAsync(string queryStepId, int topK = 5)
    {
        if (!this.isIndexed || this.indexedBeats.Count == 0)
        {
            GD.PrintErr("RAG not indexed yet - call IndexSchemaAsync() first");
            return Task.FromResult(new List<RAGResult>());
        }

        // Find the query beat
        var queryBeat = this.indexedBeats.FirstOrDefault(b => b.StepId == queryStepId);
        if (queryBeat == null)
        {
            GD.PrintErr($"Step not found in index: {queryStepId}");
            return Task.FromResult(new List<RAGResult>());
        }

        // Calculate cosine similarity with all other beats
        var results = new List<RAGResult>();
        foreach (var beat in this.indexedBeats)
        {
            // Skip the query beat itself
            if (beat.StepId == queryStepId)
            {
                continue;
            }

            var similarity = this.CosineSimilarity(queryBeat.Embedding, beat.Embedding);
            results.Add(new RAGResult
            {
                StepId = beat.StepId,
                Content = beat.Content,
                Similarity = similarity,
            });
        }

        // Sort by similarity (descending) and take top-K
        results.Sort((a, b) => b.Similarity.CompareTo(a.Similarity));
        return Task.FromResult(results.Take(topK).ToList());
    }

    /// <summary>
    /// Finds narrative beats related to arbitrary query text.
    /// Useful for player input analysis or freeform context retrieval.
    /// </summary>
    /// <param name="queryText">The text to find related beats for.</param>
    /// <param name="topK">Number of top results to return.</param>
    /// <returns>List of related narrative beats, sorted by similarity.</returns>
    public async Task<List<RAGResult>> FindRelatedBeatsFromTextAsync(string queryText, int topK = 5)
    {
        if (!this.isIndexed || this.indexedBeats.Count == 0)
        {
            GD.PrintErr("RAG not indexed yet - call IndexSchemaAsync() first");
            return new List<RAGResult>();
        }

        if (this.embeddingService == null)
        {
            GD.PrintErr("Embedding service not available");
            return new List<RAGResult>();
        }

        // Generate embedding for query text
        var queryEmbedding = await this.embeddingService.GetEmbeddingAsync(queryText).ConfigureAwait(false);
        if (queryEmbedding == null || queryEmbedding.Length == 0)
        {
            GD.PrintErr("Failed to generate query embedding");
            return new List<RAGResult>();
        }

        // Calculate cosine similarity with all beats
        var results = new List<RAGResult>();
        foreach (var beat in this.indexedBeats)
        {
            var similarity = this.CosineSimilarity(queryEmbedding, beat.Embedding);
            results.Add(new RAGResult
            {
                StepId = beat.StepId,
                Content = beat.Content,
                Similarity = similarity,
            });
        }

        // Sort by similarity (descending) and take top-K
        results.Sort((a, b) => b.Similarity.CompareTo(a.Similarity));
        return results.Take(topK).ToList();
    }

    /// <summary>
    /// Gets statistics about the indexed content.
    /// </summary>
    /// <returns>Dictionary with RAG statistics.</returns>
    public Dictionary<string, int> GetStats()
    {
        return new Dictionary<string, int>
        {
            ["TotalBeats"] = this.indexedBeats.Count,
            ["DialogueBeats"] = this.indexedBeats.Count(b => b.StepType == "dialogue"),
            ["ChoiceBeats"] = this.indexedBeats.Count(b => b.StepType == "choice"),
            ["InputBeats"] = this.indexedBeats.Count(b => b.StepType == "input"),
            ["EffectBeats"] = this.indexedBeats.Count(b => b.StepType == "effect"),
        };
    }

    /// <summary>
    /// Clears all indexed content.
    /// </summary>
    public void ClearIndex()
    {
        this.indexedBeats.Clear();
        this.isIndexed = false;
        GD.Print("RAG index cleared");
    }

    private string ExtractStepContent(JsonElement stepElement, string stepType)
    {
        // Extract meaningful content based on step type
        var contentParts = new List<string>();

        // Add step ID for context
        if (stepElement.TryGetProperty("id", out var idProp))
        {
            contentParts.Add($"[{idProp.GetString()}]");
        }

        switch (stepType)
        {
            case "dialogue":
                if (stepElement.TryGetProperty("lines", out var linesProp))
                {
                    foreach (var line in linesProp.EnumerateArray())
                    {
                        contentParts.Add(line.GetString() ?? string.Empty);
                    }
                }

                break;

            case "choice":
                if (stepElement.TryGetProperty("prompt", out var promptProp))
                {
                    contentParts.Add(promptProp.GetString() ?? string.Empty);
                }

                if (stepElement.TryGetProperty("options", out var optionsProp))
                {
                    foreach (var option in optionsProp.EnumerateArray())
                    {
                        if (option.TryGetProperty("text", out var textProp))
                        {
                            contentParts.Add(textProp.GetString() ?? string.Empty);
                        }
                    }
                }

                break;

            case "input":
                if (stepElement.TryGetProperty("prompt", out var inputPromptProp))
                {
                    contentParts.Add(inputPromptProp.GetString() ?? string.Empty);
                }

                break;

            case "effect":
                if (stepElement.TryGetProperty("action", out var actionProp))
                {
                    contentParts.Add($"Effect: {actionProp.GetString()}");
                }

                break;
        }

        return string.Join(" ", contentParts.Where(s => !string.IsNullOrWhiteSpace(s)));
    }

    private float CosineSimilarity(float[] vectorA, float[] vectorB)
    {
        if (vectorA.Length != vectorB.Length)
        {
            GD.PrintErr("Vector dimension mismatch");
            return 0f;
        }

        float dotProduct = 0f;
        float magnitudeA = 0f;
        float magnitudeB = 0f;

        for (int i = 0; i < vectorA.Length; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            magnitudeA += vectorA[i] * vectorA[i];
            magnitudeB += vectorB[i] * vectorB[i];
        }

        if (magnitudeA == 0f || magnitudeB == 0f)
        {
            return 0f;
        }

        return dotProduct / (MathF.Sqrt(magnitudeA) * MathF.Sqrt(magnitudeB));
    }
}
}
