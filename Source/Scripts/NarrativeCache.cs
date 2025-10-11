using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;

namespace OmegaSpiral;

/// <summary>
/// Caches dynamically generated narrative content to avoid regenerating the same LLM responses.
/// Keys are generated from step ID + game seed to ensure consistent narrative per playthrough.
/// </summary>
public partial class NarrativeCache : Node
{
    private const string CacheDirectory = "user://narrative_cache";
    private const string CacheFileExtension = ".json";
    private const int MaxCacheEntries = 500; // Limit to prevent unbounded growth

    private Dictionary<string, CachedNarrative> memoryCache = new();
    private GameState? gameState;

    /// <summary>
    /// Represents a cached narrative entry.
    /// </summary>
    private record CachedNarrative
    {
        public required string StepId { get; init; }
        public required string PersonaId { get; init; }
        public required string GeneratedText { get; init; }
        public required string[] ContextLines { get; init; }
        public required long GameSeed { get; init; }
        public required DateTime GeneratedAt { get; init; }
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.gameState = this.GetNode<GameState>("/root/GameState");
        this.EnsureCacheDirectoryExists();
        GD.Print("NarrativeCache initialized");
    }

    /// <summary>
    /// Loads cached narrative for a specific step and persona if it exists.
    /// </summary>
    /// <param name="stepId">The step ID from the scene schema.</param>
    /// <param name="personaId">The persona generating the narrative (e.g., "omega", "hero").</param>
    /// <param name="contextLines">The schema lines used as context (for cache validation).</param>
    /// <returns>The cached narrative text, or null if not found or invalid.</returns>
    public async Task<string?> LoadCachedNarrativeAsync(string stepId, string personaId, string[] contextLines)
    {
        if (this.gameState == null)
        {
            GD.PrintErr("GameState not available, cannot load cached narrative");
            return null;
        }

        var cacheKey = this.GenerateCacheKey(stepId, personaId, this.gameState.GameSeed);

        // Check memory cache first
        if (this.memoryCache.TryGetValue(cacheKey, out var cached))
        {
            if (this.ValidateCachedEntry(cached, contextLines))
            {
                GD.Print($"Cache hit (memory): {stepId} / {personaId}");
                return cached.GeneratedText;
            }
            else
            {
                GD.Print($"Cache invalid (context changed): {stepId} / {personaId}");
                this.memoryCache.Remove(cacheKey);
            }
        }

        // Check disk cache
        var cacheFilePath = this.GetCacheFilePath(cacheKey);
        if (!Godot.FileAccess.FileExists(cacheFilePath))
        {
            return null;
        }

        try
        {
            using var file = Godot.FileAccess.Open(cacheFilePath, Godot.FileAccess.ModeFlags.Read);
            if (file == null)
            {
                return null;
            }

            var jsonText = file.GetAsText();
            var cachedEntry = JsonSerializer.Deserialize<CachedNarrative>(jsonText);

            if (cachedEntry == null || !this.ValidateCachedEntry(cachedEntry, contextLines))
            {
                GD.Print($"Cache invalid (disk): {stepId} / {personaId}");
                return null;
            }

            // Load into memory cache
            this.memoryCache[cacheKey] = cachedEntry;
            GD.Print($"Cache hit (disk): {stepId} / {personaId}");
            return cachedEntry.GeneratedText;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to load cached narrative: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Caches generated narrative for future use.
    /// </summary>
    /// <param name="stepId">The step ID from the scene schema.</param>
    /// <param name="personaId">The persona that generated the narrative.</param>
    /// <param name="generatedText">The LLM-generated narrative text.</param>
    /// <param name="contextLines">The schema lines used as context.</param>
    public async Task CacheNarrativeAsync(string stepId, string personaId, string generatedText, string[] contextLines)
    {
        if (this.gameState == null)
        {
            GD.PrintErr("GameState not available, cannot cache narrative");
            return;
        }

        var cacheKey = this.GenerateCacheKey(stepId, personaId, this.gameState.GameSeed);

        var cacheEntry = new CachedNarrative
        {
            StepId = stepId,
            PersonaId = personaId,
            GeneratedText = generatedText,
            ContextLines = contextLines,
            GameSeed = this.gameState.GameSeed,
            GeneratedAt = DateTime.UtcNow,
        };

        // Store in memory cache
        this.memoryCache[cacheKey] = cacheEntry;

        // Enforce cache size limit
        if (this.memoryCache.Count > MaxCacheEntries)
        {
            await this.EvictOldestEntriesAsync().ConfigureAwait(false);
        }

        // Persist to disk
        await this.WriteCacheToDiskAsync(cacheKey, cacheEntry).ConfigureAwait(false);

        GD.Print($"Cached narrative: {stepId} / {personaId}");
    }

    /// <summary>
    /// Clears all cached narratives (both memory and disk).
    /// Useful for testing or when schema changes significantly.
    /// </summary>
    public void ClearCache()
    {
        this.memoryCache.Clear();

        var cacheDir = ProjectSettings.GlobalizePath(CacheDirectory);
        if (Directory.Exists(cacheDir))
        {
            try
            {
                var files = Directory.GetFiles(cacheDir, $"*{CacheFileExtension}");
                foreach (var file in files)
                {
                    File.Delete(file);
                }

                GD.Print("Narrative cache cleared");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Failed to clear cache: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Gets cache statistics for debugging.
    /// </summary>
    /// <returns>Dictionary with cache stats (memory entries, disk entries, etc.).</returns>
    public Dictionary<string, int> GetCacheStats()
    {
        var cacheDir = ProjectSettings.GlobalizePath(CacheDirectory);
        var diskEntries = 0;

        if (Directory.Exists(cacheDir))
        {
            diskEntries = Directory.GetFiles(cacheDir, $"*{CacheFileExtension}").Length;
        }

        return new Dictionary<string, int>
        {
            ["MemoryEntries"] = this.memoryCache.Count,
            ["DiskEntries"] = diskEntries,
            ["MaxEntries"] = MaxCacheEntries,
        };
    }

    private string GenerateCacheKey(string stepId, string personaId, long gameSeed)
    {
        // Create a stable hash from step ID + persona ID + game seed
        var keyString = $"{stepId}|{personaId}|{gameSeed}";
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(keyString));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    private bool ValidateCachedEntry(CachedNarrative cached, string[] contextLines)
    {
        // Verify game seed matches
        if (this.gameState == null || cached.GameSeed != this.gameState.GameSeed)
        {
            return false;
        }

        // Verify context lines match (schema hasn't changed)
        if (cached.ContextLines.Length != contextLines.Length)
        {
            return false;
        }

        for (int i = 0; i < contextLines.Length; i++)
        {
            if (cached.ContextLines[i] != contextLines[i])
            {
                return false;
            }
        }

        return true;
    }

    private void EnsureCacheDirectoryExists()
    {
        var cacheDir = ProjectSettings.GlobalizePath(CacheDirectory);
        if (!Directory.Exists(cacheDir))
        {
            Directory.CreateDirectory(cacheDir);
            GD.Print($"Created cache directory: {cacheDir}");
        }
    }

    private string GetCacheFilePath(string cacheKey)
    {
        return $"{CacheDirectory}/{cacheKey}{CacheFileExtension}";
    }

    private async Task WriteCacheToDiskAsync(string cacheKey, CachedNarrative cacheEntry)
    {
        var cacheFilePath = this.GetCacheFilePath(cacheKey);

        try
        {
            var jsonText = JsonSerializer.Serialize(cacheEntry, new JsonSerializerOptions
            {
                WriteIndented = true,
            });

            using var file = Godot.FileAccess.Open(cacheFilePath, Godot.FileAccess.ModeFlags.Write);
            if (file != null)
            {
                file.StoreString(jsonText);
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to write cache to disk: {ex.Message}");
        }
    }

    private async Task EvictOldestEntriesAsync()
    {
        // Simple LRU eviction: remove 10% of oldest entries
        var entriesToRemove = (int)(MaxCacheEntries * 0.1);
        var sortedEntries = new List<KeyValuePair<string, CachedNarrative>>(this.memoryCache);
        sortedEntries.Sort((a, b) => a.Value.GeneratedAt.CompareTo(b.Value.GeneratedAt));

        for (int i = 0; i < Math.Min(entriesToRemove, sortedEntries.Count); i++)
        {
            var key = sortedEntries[i].Key;
            this.memoryCache.Remove(key);

            // Also remove from disk
            var cacheFilePath = this.GetCacheFilePath(key);
            var globalPath = ProjectSettings.GlobalizePath(cacheFilePath);
            if (File.Exists(globalPath))
            {
                try
                {
                    File.Delete(globalPath);
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"Failed to delete cache file: {ex.Message}");
                }
            }
        }

        GD.Print($"Evicted {entriesToRemove} old cache entries");
    }
}
