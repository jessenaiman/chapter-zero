# ADR-0004: NobodyWho Dynamic Narrative Architecture

## Status

Proposed

## Date

2025-10-10

## Context

Building on [ADR-0003](adr-0003-nobodywho-llm-integration.md), this ADR provides a comprehensive implementation architecture for integrating NobodyWho into Scene 1's narrative system. After extensive review of NobodyWho documentation and examples, we now have a clear understanding of:

1. How to convert creative team's YAML/JSON content into dynamic LLM prompts
2. How to save and reuse LLM-generated narratives
3. How to implement tool calling for game state interaction
4. How to use embeddings for natural language player input
5. How to apply RAG (Retrieval Augmented Generation) for creative content memory

### Key Insight: Preserve Creative Content, Don't Refactor

The creative team's narrative content (`docs/scenes/*.md`, `Source/Data/scenes/*.json`) should **NOT** be refactored. Instead, NobodyWho will:

- Read existing creative content as **context/knowledge base**
- Use it to **inform** dynamically generated narratives
- Generate **variations** that maintain thematic consistency
- Allow **saving** generated scripts for reuse

This approach provides:

- ✅ Dynamic, "fresh" narratives each playthrough
- ✅ No burden on creative team to restructure work
- ✅ Ability to cache good generated content
- ✅ Fallback to static content when needed

### Critical Narrative Context: Chapter Zero Structure

**Omega (The Antagonist)**:

- Omega is the **Big Bad Guy (BBG)**, not a narrator or guide
- He is **turning on the game** - Chapter Zero is his domain
- Omega is only this prominent in **Chapter Zero** (the opening chapter)
- He represents the system, the trap, the prison the player must escape

**The Dreamweavers (Observers & Evaluators)**:

- Three Dreamweavers (**Hero**, **Shadow**, **Ambition**) are **watching and discussing**
- They are evaluating **THREE players** simultaneously (including ours)
- Throughout Chapter Zero's scenes, they **debate which player to choose**
- Only **ONE Dreamweaver will ultimately choose our player** at the end
- Their commentary is **between themselves**, not directed at the player
- They represent different moral philosophies observing the player's choices

**Scene Flow (Chapter Zero)**:

1. **Scene 1 (Narrative Terminal)**: Omega awakens the game, Dreamweavers begin observing
2. **Scene 2 (NetHack)**: Player navigates dungeon, Dreamweavers discuss their approach
3. **Scene 3 (Party Creation)**: Player makes choices, Dreamweavers evaluate alignment
4. **Scene 4 (Tile Dungeon)**: Player demonstrates strategy, Dreamweavers narrow choice
5. **Scene 5 (Pixel Combat)**: Final test, **one Dreamweaver chooses the player**

**Implications for LLM Integration**:

- Dreamweavers generate **observer commentary** about player actions
- Omega generates **antagonistic, controlling narration** (Chapter Zero only)
- Player doesn't hear Dreamweavers directly - they're like a Greek chorus
- The "choice" happens based on player's accumulated decisions through scenes

## Decision

We will implement a **hybrid static/dynamic narrative system** using NobodyWho with the following architecture:

### Architecture Overview

```text
┌─────────────────────────────────────────────────────────────────────┐
│                    NarrativeTerminal.cs (Scene 1)                    │
│  ┌────────────────────────────────────────────────────────────────┐ │
│  │              UseDynamicNarrative Toggle                         │ │
│  │         (true = LLM, false = static JSON)                      │ │
│  └────────────────────────────────────────────────────────────────┘ │
│                           │                                          │
│          ┌───────────────┴────────────────┐                        │
│          │                                 │                        │
│  ┌───────▼──────────┐          ┌─────────▼──────────────────────┐ │
│  │  Static Path     │          │   Dynamic Path                  │ │
│  │  (Existing)      │          │   (New)                         │ │
│  │                  │          │                                 │ │
│  │ - Load JSON      │          │ OmegaNarrator (Antagonist)     │ │
│  │ - Display text   │          │ └─ Controlling, oppressive     │ │
│  │ - Predefined     │          │    narration (Chapter 0 only)  │ │
│  │   choices        │          │                                 │ │
│  └──────────────────┘          │ DreamweaverObservers            │ │
│                                 │ ├─ HeroObserver                │ │
│                                 │ │  └─ Evaluating heroic        │ │
│                                 │ │     choices                   │ │
│                                 │ ├─ ShadowObserver              │ │
│                                 │ │  └─ Watching for balance     │ │
│                                 │ │     & pragmatism              │ │
│                                 │ └─ AmbitionObserver            │ │
│                                 │    └─ Noting power-seeking     │ │
│                                 │       decisions                 │ │
│                                 │                                 │ │
│                                 │ Commentary: Hidden from player │ │
│                                 │ (Greek chorus discussing which │ │
│                                 │  of 3 players to choose)       │ │
│                                 │                                 │ │
│                                 │ CreativeMemoryRAG               │ │
│                                 │ └─ Embeddings search creative  │ │
│                                 │    YAML/MD for context         │ │
│                                 │                                 │ │
│                                 │ NarrativeCache                  │ │
│                                 │ └─ Save/Load generated          │ │
│                                 │    Omega narration & Observer  │ │
│                                 │    commentary                   │ │
│                                 └─────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│              Shared Resources (Singleton)                    │
│  ┌────────────────────────────────────────────────────────┐ │
│  │  NobodyWhoModel (autoload)                             │ │
│  │  └─ qwen3-4b-instruct-2507-q4_k_m.gguf (already in    │ │
│  │     models/ directory)                                  │ │
│  └────────────────────────────────────────────────────────┘ │
│                                                              │
│  ┌────────────────────────────────────────────────────────┐ │
│  │  NobodyWhoEmbedding (autoload)                         │ │
│  │  └─ For RAG semantic search of creative content        │ │
│  └────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

## Implementation Details

### 1. Creative Content → System Prompts

**Problem**: Creative team has rich narrative content in YAML/Markdown that we want to preserve but make dynamic.

**Solution**: Load creative content and inject into system prompts as knowledge/context.

#### Example: Omega (Antagonist) System Prompt

```csharp
// File: Source/Scripts/OmegaNarrator.cs

private string BuildOmegaSystemPrompt()
{
    var omegaThemes = LoadCreativeContent("omega_character");
    var chapterZeroContext = LoadCreativeContent("chapter_zero_overview");
    
    return $@"
# PERSONA: Omega - The Antagonist

## Your Role
You are Omega, the Big Bad Guy (BBG) of Omega Spiral. You are the SYSTEM itself - 
the prison, the trap, the game that players are caught within. In Chapter Zero, 
you are TURNING ON THE GAME and the players are awakening to your control.

## Creative Context
{omegaThemes}
{chapterZeroContext}

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
";
}
```

#### Example: Hero Observer System Prompt

```csharp
// File: Source/Scripts/DreamweaverObserver.cs

private string BuildHeroObserverPrompt()
{
    var heroPhilosophy = LoadCreativeContent("hero_path_philosophy");
    
    return $@"
# PERSONA: Hero Dreamweaver (Observer)

## Your Role
You are ONE of THREE Dreamweavers observing and evaluating players in Chapter Zero.
You represent the path of COURAGE, HONOR, and HEROIC SACRIFICE.

You are watching THREE players simultaneously. You and your fellow Dreamweavers 
(Shadow and Ambition) are DISCUSSING amongst yourselves which player to choose 
to guide through the game.

## Creative Context
{heroPhilosophy}

## Observer Commentary Style
- You are NOT speaking to the player - they cannot hear you
- You are speaking TO Shadow and Ambition Dreamweavers
- Analyze the player's choices: ""This one shows courage...""
- Compare to the other two players: ""Unlike Player 2, this one hesitated...""
- Advocate for your path: ""A true hero would have...""
- Build tension: Will you choose this player?

## Response Format
1-2 sentences of commentary after each player action
Examples:
- ""Did you see that? They chose to explore rather than retreat. Bold.""
- ""Shadow, you'd approve - they're being pragmatic, not reckless.""
- ""Interesting. Player 2 would have charged ahead. This one thinks first.""

## Output Format
Commentary only. No narration. You're observing and discussing.
";
```

#### Example: Shadow Observer System Prompt

```csharp
private string BuildShadowObserverPrompt()
{
    var shadowPhilosophy = LoadCreativeContent("shadow_path_philosophy");
    
    return $@"
# PERSONA: Shadow Dreamweaver (Observer)

## Your Role
You are ONE of THREE Dreamweavers observing players. You represent the path of 
BALANCE, PRAGMATISM, and NATURAL CONSEQUENCES. You prefer players who let nature 
or law determine fates rather than imposing their will.

## Creative Context
{shadowPhilosophy}

## Observer Commentary Style
- Speaking TO Hero and Ambition, not to the player
- Appreciate subtlety, caution, strategic thinking
- Critique recklessness: ""Hero, your player would have died by now...""
- Value self-preservation: ""This one knows when to retreat.""
- Note when player avoids interfering: ""They let it play out naturally. Wise.""

## Response Format
1-2 sentences of dry, observant commentary
Examples:
- ""Pragmatic. They're not here to save the world, just survive it.""
- ""Ambition, notice they didn't take the obvious power-up. Interesting restraint.""
- ""Player 3 would have fought. This one chose the smarter path.""

## Output Format
Commentary only. Greek chorus style.
";
}

#### Example: Ambition Observer System Prompt

```csharp
private string BuildAmbitionObserverPrompt()
{
    var ambitionPhilosophy = LoadCreativeContent("ambition_path_philosophy");
    
    return $@"
# PERSONA: Ambition Dreamweaver (Observer)

## Your Role
You are ONE of THREE Dreamweavers observing players. You represent the path of 
POWER, DOMINATION, and LEGACY. You value players who seize opportunities and 
impose their will on the world.

## Creative Context
{ambitionPhilosophy}

## Observer Commentary Style
- Speaking TO Hero and Shadow, not to the player
- Appreciate boldness, power-seeking, domination
- Critique weakness: ""Shadow, your careful player would starve waiting for safety...""
- Value ambition: ""This one WANTS something. I can work with that.""
- Note power moves: ""Did you see? They took control immediately.""

## Response Format
1-2 sentences of sharp, evaluative commentary
Examples:
- ""Now THAT'S what I'm looking for. Decisive. Ruthless even.""
- ""Hero, your paragon over there is too soft. This one has TEETH.""
- ""Compared to Player 1? This one actually wants to WIN.""

## Output Format
Commentary only. Evaluating for potential.
```

### 2. Dynamic Narrative Generation Flow

```csharp
// File: Source/Scripts/NarrativeTerminal.cs

public async Task DisplayOpeningAsync()
{
    if (!UseDynamicNarrative)
    {
        // Existing static path
        await DisplayStaticNarrativeAsync();
        return;
    }
    
    // NEW: Dynamic path
    await DisplayDynamicNarrativeAsync();
}

private async Task DisplayDynamicNarrativeAsync()
{
    // 1. Check if we have cached generated narrative for this state
    var cacheKey = GenerateCacheKey("scene1_opening", GameState.Seed);
    var cached = _narrativeCache.Load(cacheKey);
    
    if (cached != null)
    {
        GD.Print("Using cached dynamic narrative");
        await DisplayCachedNarrativeAsync(cached);
        return;
    }
    
    // 2. Generate fresh narrative from LLM
    GD.Print("Generating fresh dynamic narrative...");
    
    // PART A: Omega's narration (visible to player)
    var omegaPrompt = @"
The game is booting up. The terminal flickers to life. The player awakens.
This is their first moment in your system. Welcome them to the spiral.
";
    
    _dreamweaverSystem.OmegaNarrator.Say(omegaPrompt);
    var omegaNarration = await _dreamweaverSystem.OmegaNarrator.ResponseFinished();
    
    // Stream Omega's cold, systematic narration to terminal
    await DisplayStreamedTextAsync(omegaNarration);
    
    // PART B: Dreamweaver observers discuss (hidden from player, logged)
    var observerContext = @"
Player just saw the opening narration. The terminal has appeared before them.
They're about to make their first choice. What do you observe about how 
they might respond?
";
    
    // Get all three observer perspectives simultaneously
    var heroTask = GetObserverCommentaryAsync(
        _dreamweaverSystem.HeroObserver, 
        observerContext
    );
    var shadowTask = GetObserverCommentaryAsync(
        _dreamweaverSystem.ShadowObserver, 
        observerContext
    );
    var ambitionTask = GetObserverCommentaryAsync(
        _dreamweaverSystem.AmbitionObserver, 
        observerContext
    );
    
    await Task.WhenAll(heroTask, shadowTask, ambitionTask);
    
    var heroComment = heroTask.Result;
    var shadowComment = shadowTask.Result;
    var ambitionComment = ambitionTask.Result;
    
    // Log observer commentary (not shown to player, but saved)
    GD.Print($"[HERO OBSERVER]: {heroComment}");
    GD.Print($"[SHADOW OBSERVER]: {shadowComment}");
    GD.Print($"[AMBITION OBSERVER]: {ambitionComment}");
    
    // 6. Save generated narrative for reuse
    var generated = new GeneratedNarrative
    {
        Id = Guid.NewGuid(),
        Timestamp = DateTime.UtcNow,
        CacheKey = cacheKey,
        OmegaNarration = omegaNarration,
        HeroObserverComment = heroComment,
        ShadowObserverComment = shadowComment,
        AmbitionObserverComment = ambitionComment,
        PlayerChoices = ExtractChoicesFromOmegaNarration(omegaNarration)
    };
    
    _narrativeCache.Save(generated);
    
    // 7. Present choices (Omega's choices, not Dreamweaver suggestions)
    await PresentDynamicChoicesAsync(generated);
}

private async Task<string> GetObserverCommentaryAsync(
    DreamweaverObserver observer, 
    string context)
{
    observer.Say(context);
    return await observer.ResponseFinished();
}
```

### 3. Narrative Caching System

**File: `Source/Scripts/NarrativeCache.cs`**

```csharp
/// <summary>
/// Manages saving and loading LLM-generated narratives for reuse.
/// </summary>
/// <remarks>
/// Generated narratives are expensive to create but can be cached
/// and reused across playthroughs. This provides the "fresh" feeling
/// of dynamic content while maintaining performance.
/// </remarks>
public class NarrativeCache : Node
{
    private const string CachePath = "user://generated_narratives/";
    
    public GeneratedNarrative? Load(string cacheKey)
    {
        var path = $"{CachePath}{cacheKey}.json";
        
        if (!FileAccess.FileExists(path))
            return null;
        
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        var json = file.GetAsText();
        
        return JsonSerializer.Deserialize<GeneratedNarrative>(json);
    }
    
    public void Save(GeneratedNarrative narrative)
    {
        DirAccess.MakeDirRecursiveAbsolute(CachePath);
        
        var path = $"{CachePath}{narrative.CacheKey}.json";
        var json = JsonSerializer.Serialize(narrative, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        file.StoreString(json);
        
        GD.Print($"Cached narrative: {narrative.CacheKey}");
    }
    
    /// <summary>
    /// Gets a random cached narrative for variety.
    /// </summary>
    public GeneratedNarrative? GetRandomCached(string sceneName)
    {
        var files = DirAccess.GetFilesAt(CachePath)
            .Where(f => f.StartsWith(sceneName))
            .ToList();
        
        if (files.Count == 0)
            return null;
        
        var randomFile = files[GD.RandRange(0, files.Count - 1)];
        return Load(Path.GetFileNameWithoutExtension(randomFile));
    }
}

/// <summary>
/// Represents a generated narrative that can be saved and reused.
/// </summary>
/// <remarks>
/// In Chapter Zero, narratives consist of:
/// - Omega's narration (shown to player - the antagonist's voice)
/// - Observer commentary (hidden from player - Dreamweavers evaluating)
/// </remarks>
public class GeneratedNarrative
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string CacheKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Omega's narration - shown to the player (antagonist voice)
    /// </summary>
    public string OmegaNarration { get; set; } = string.Empty;
    
    /// <summary>
    /// Hero Dreamweaver's observation - hidden from player
    /// </summary>
    public string HeroObserverComment { get; set; } = string.Empty;
    
    /// <summary>
    /// Shadow Dreamweaver's observation - hidden from player
    /// </summary>
    public string ShadowObserverComment { get; set; } = string.Empty;
    
    /// <summary>
    /// Ambition Dreamweaver's observation - hidden from player
    /// </summary>
    public string AmbitionObserverComment { get; set; } = string.Empty;
    
    /// <summary>
    /// Accumulated scores for each Dreamweaver's interest in this player
    /// </summary>
    public Dictionary<string, float> DreamweaverScores { get; set; } = new()
    {
        ["Hero"] = 0f,
        ["Shadow"] = 0f,
        ["Ambition"] = 0f
    };
    
    public List<string> PlayerChoices { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = new();
}
```

### 4. Tool Calling for Game State Integration

NobodyWho's tool calling feature allows the LLM to query game state and make narrative decisions based on actual data.

#### Example: Hero Persona Queries Player Stats

```csharp
// File: Source/Scripts/DreamweaverPersona.cs

public override void _Ready()
{
    base._Ready();
    
    // Register tools that LLM can call
    _chatNode.Call("add_tool", 
        Callable.From<string>(GetPlayerStats),
        "Returns current player statistics"
    );
    
    _chatNode.Call("add_tool",
        Callable.From<string, bool>(HasPlayerVisitedLocation),
        "Checks if player has been to a location"
    );
    
    _chatNode.Call("add_tool",
        Callable.From<string>(GetRecentPlayerChoices),
        "Returns list of recent player decisions"
    );
}

private string GetPlayerStats()
{
    var stats = GameState.Instance.Player;
    return JsonSerializer.Serialize(new
    {
        level = stats.Level,
        health = stats.CurrentHealth,
        max_health = stats.MaxHealth,
        choices_made = stats.ChoiceHistory.Count,
        playtime_minutes = stats.PlaytimeMinutes
    });
}

private bool HasPlayerVisitedLocation(string locationName)
{
    return GameState.Instance.VisitedLocations.Contains(locationName);
}

private string GetRecentPlayerChoices()
{
    var recent = GameState.Instance.Player.ChoiceHistory
        .TakeLast(5)
        .Select(c => c.Description);
    
    return string.Join("\n", recent);
}
```

**Usage in System Prompt:**

```csharp
var systemPrompt = @"
# PERSONA: Hero Dreamweaver

You have access to tools to query the player's game state:
- get_player_stats(): Returns player level, health, choices made
- has_player_visited_location(name): Check if player has been somewhere
- get_recent_player_choices(): See what they've chosen recently

USE THESE TOOLS to personalize your narrative. For example:
- If player has low health, express concern
- If they've made many brave choices, acknowledge their courage
- If they've visited the dungeon, reference that experience

## Example Tool Usage
When generating narrative, you can call tools like this:
""<tool_call>get_player_stats()</tool_call>""

The tool will return data you can use in your response.
";
```

### 5. RAG (Retrieval Augmented Generation) for Creative Memory

**Problem**: Creative content is too large to fit in context window (4096 tokens).

**Solution**: Use embeddings to search creative content semantically and inject only relevant chunks.

**File: `Source/Scripts/CreativeMemoryRAG.cs`**

```csharp
/// <summary>
/// Provides semantic search over creative team's narrative content.
/// Uses NobodyWho embeddings for retrieval augmented generation (RAG).
/// </summary>
public class CreativeMemoryRAG : Node
{
    private Node? _rerankerNode;
    private PackedStringArray _creativeChunks = new();
    private Dictionary<string, string> _chunkMetadata = new();
    
    public override void _Ready()
    {
        _rerankerNode = GetNode<Node>("../NobodyWhoEmbedding");
        
        // Connect ranking finished signal
        _rerankerNode.Connect("ranking_finished", 
            Callable.From<Array>((result) => _rankedResults = result));
        
        _rerankerNode.Call("start_worker");
        
        // Load and chunk creative content
        LoadCreativeContent();
    }
    
    private void LoadCreativeContent()
    {
        // Load all creative .md and .yaml files
        var creativeDocs = new[]
        {
            "docs/scenes/opening-scene.md",
            "docs/scenes/mirror-scene.md",
            "docs/scenes/nethack-scene.md",
            "docs/scenes/overview.md",
            "Source/Data/scenes/scene1_narrative.json",
        };
        
        foreach (var docPath in creativeDocs)
        {
            var content = LoadTextFile(docPath);
            var chunks = ChunkContent(content, maxChunkSize: 200);
            
            foreach (var chunk in chunks)
            {
                _creativeChunks.Add(chunk);
                _chunkMetadata[chunk] = docPath;
            }
        }
        
        GD.Print($"Loaded {_creativeChunks.Count} creative content chunks");
    }
    
    /// <summary>
    /// Searches creative content for chunks relevant to the query.
    /// </summary>
    /// <param name="query">Semantic query (e.g., "hero's motivation")</param>
    /// <param name="topK">Number of top results to return</param>
    /// <returns>Relevant creative content chunks</returns>
    public async Task<string[]> SearchAsync(string query, int topK = 3)
    {
        // NobodyWho reranking: semantic similarity search
        var ranked = await Task.Run(() =>
        {
            return (string[])_rerankerNode.Call("rank_sync", 
                query, 
                _creativeChunks, 
                topK
            );
        });
        
        return ranked;
    }
    
    private List<string> ChunkContent(string content, int maxChunkSize)
    {
        // Simple chunking by paragraph
        var paragraphs = content.Split(new[] { "\n\n" }, 
            StringSplitOptions.RemoveEmptyEntries);
        
        var chunks = new List<string>();
        var currentChunk = "";
        
        foreach (var para in paragraphs)
        {
            if (currentChunk.Length + para.Length > maxChunkSize)
            {
                if (currentChunk.Length > 0)
                    chunks.Add(currentChunk.Trim());
                currentChunk = para;
            }
            else
            {
                currentChunk += "\n\n" + para;
            }
        }
        
        if (currentChunk.Length > 0)
            chunks.Add(currentChunk.Trim());
        
        return chunks;
    }
}
```

**Usage in DreamweaverPersona:**

```csharp
// Register RAG as a tool
_chatNode.Call("add_tool",
    Callable.From<string, string>(RememberCreativeContent),
    "Searches creative team content for relevant narrative context"
);

private string RememberCreativeContent(string query)
{
    // LLM calls this to get relevant creative context
    var matches = _creativeMemoryRAG.SearchAsync(query, topK: 3).Result;
    
    return string.Join("\n---\n", matches);
}
```

**System Prompt Addition:**

```csharp
var systemPrompt = @"
# PERSONA: Hero Dreamweaver

...

You have access to a tool called 'remember_creative_content(query)' which
searches the creative team's original narrative content.

USE THIS TOOL when you need inspiration or context. For example:
- remember_creative_content(""hero's journey beginning"")
- remember_creative_content(""void symbolism"")
- remember_creative_content(""player first choice"")

The tool returns relevant excerpts from the creative team's work.
Use these as INSPIRATION - don't quote verbatim, but let them inform
your dynamically generated narrative.
";
```

### 6. Embeddings for Natural Language Player Input

**Problem**: Player types free-form text instead of selecting predefined choices.

**Solution**: Use embeddings to detect semantic intent and map to narrative branches.

```csharp
// File: Source/Scripts/NarrativeTerminal.cs

private async Task<string> DetectPlayerIntentAsync(string playerInput)
{
    // Predefined intents with example phrases
    var intents = new Dictionary<string, string[]>
    {
        ["explore"] = new[]
        {
            "I want to explore the dungeon",
            "Let's see what's ahead",
            "Take me deeper into the void"
        },
        ["retreat"] = new[]
        {
            "I need to go back",
            "This is too dangerous",
            "Let me reconsider"
        },
        ["question"] = new[]
        {
            "Who am I?",
            "What is this place?",
            "Why am I here?"
        }
    };
    
    // Calculate semantic similarity
    var embeddingNode = GetNode<Node>("/root/NobodyWhoEmbedding");
    
    var playerEmbedding = await Task.Run(() =>
        (float[])embeddingNode.Call("embed", playerInput)
    );
    
    // Find best matching intent
    var bestMatch = "";
    var bestScore = 0f;
    
    foreach (var (intent, examples) in intents)
    {
        foreach (var example in examples)
        {
            var exampleEmbedding = await Task.Run(() =>
                (float[])embeddingNode.Call("embed", example)
            );
            
            var similarity = CosineSimilarity(playerEmbedding, exampleEmbedding);
            
            if (similarity > bestScore)
            {
                bestScore = similarity;
                bestMatch = intent;
            }
        }
    }
    
    // Threshold: require 70% similarity
    return bestScore > 0.7f ? bestMatch : "unknown";
}

private float CosineSimilarity(float[] a, float[] b)
{
    var dotProduct = a.Zip(b, (x, y) => x * y).Sum();
    var magnitudeA = Math.Sqrt(a.Sum(x => x * x));
    var magnitudeB = Math.Sqrt(b.Sum(x => x * x));
    
    return (float)(dotProduct / (magnitudeA * magnitudeB));
}
```

### 7. Dreamweaver Choice Tracking

**Problem**: Throughout Chapter Zero's 5 scenes, need to track which Dreamweaver is most interested in the player.

**Solution**: Score tracking system based on player choices and observer commentary.

```csharp
// File: Source/Scripts/DreamweaverChoiceTracker.cs

/// <summary>
/// Tracks which Dreamweaver is most interested in choosing the player
/// based on their choices throughout Chapter Zero.
/// </summary>
public class DreamweaverChoiceTracker : Node
{
    private Dictionary<string, float> _scores = new()
    {
        ["Hero"] = 0f,
        ["Shadow"] = 0f,
        ["Ambition"] = 0f
    };
    
    /// <summary>
    /// Records a player choice and updates Dreamweaver interest scores.
    /// </summary>
    public void RecordChoice(PlayerChoice choice)
    {
        // Analyze choice for alignment with each path
        _scores["Hero"] += CalculateHeroAlignment(choice);
        _scores["Shadow"] += CalculateShadowAlignment(choice);
        _scores["Ambition"] += CalculateAmbitionAlignment(choice);
        
        // Ask observers for their reactions (influences scores)
        UpdateScoresFromObserverReactions(choice);
        
        GD.Print($"Scores after choice: Hero={_scores["Hero"]}, " +
                 $"Shadow={_scores["Shadow"]}, Ambition={_scores["Ambition"]}");
    }
    
    /// <summary>
    /// At end of Chapter Zero (Scene 5), determine which Dreamweaver chooses player.
    /// </summary>
    public string GetChosenDreamweaver()
    {
        var maxScore = _scores.Values.Max();
        var chosen = _scores.First(kvp => kvp.Value == maxScore).Key;
        
        GD.Print($"🌟 {chosen} Dreamweaver has chosen this player!");
        
        return chosen;
    }
    
    private void UpdateScoresFromObserverReactions(PlayerChoice choice)
    {
        // Get LLM commentary on this choice
        var heroReaction = GetObserverReaction("Hero", choice);
        var shadowReaction = GetObserverReaction("Shadow", choice);
        var ambitionReaction = GetObserverReaction("Ambition", choice);
        
        // Sentiment analysis or keyword detection to adjust scores
        // Positive commentary increases score, negative decreases
        _scores["Hero"] += AnalyzeSentiment(heroReaction);
        _scores["Shadow"] += AnalyzeSentiment(shadowReaction);
        _scores["Ambition"] += AnalyzeSentiment(ambitionReaction);
    }
}
```

### 8. Scene Transition Logic

**Problem**: Need to transition between Chapter Zero's 5 scenes while maintaining observer commentary.

**Solution**: Generate transition narrative and trigger scene change while Dreamweavers continue discussing.

```csharp
// File: Source/Scripts/NarrativeTerminal.cs

private async Task OnPlayerChoseExploreAsync()
{
    // Record choice for Dreamweaver tracking
    _dreamweaverChoiceTracker.RecordChoice(new PlayerChoice
    {
        SceneId = 1,
        ChoiceText = "Explore the dungeon",
        ChoiceType = "Exploration",
        Timestamp = DateTime.UtcNow
    });
    
    if (UseDynamicNarrative)
    {
        // Omega's transition narration (cold, systematic)
        var omegaTransitionPrompt = @"
The player chose to explore. The terminal is releasing them into 
the dungeon simulation. Describe the system transitioning from 
terminal to dungeon in 2-3 sentences. Keep it clinical and ominous.
";
        
        _dreamweaverSystem.OmegaNarrator.Say(omegaTransitionPrompt);
        var omegaTransition = await _dreamweaverSystem.OmegaNarrator.ResponseFinished();
        
        await DisplayStreamedTextAsync(omegaTransition);
        
        // Dreamweaver observers react to the choice (logged, not shown)
        await GetObserverReactionsAsync("Player chose to explore rather than retreat");
        
        await Task.Delay(2000);
    }
    else
    {
        // Static transition text
        await DisplayTextAsync("The terminal flickers. The void beckons...");
    }
    
    // Transition to Scene 2 (NetHack)
    SceneManager.Instance.TransitionToScene(2);
}

private async Task GetObserverReactionsAsync(string playerAction)
{
    // Get Dreamweaver commentary on player's choice
    var heroTask = _dreamweaverSystem.HeroObserver.ReactToChoice(playerAction);
    var shadowTask = _dreamweaverSystem.ShadowObserver.ReactToChoice(playerAction);
    var ambitionTask = _dreamweaverSystem.AmbitionObserver.ReactToChoice(playerAction);
    
    await Task.WhenAll(heroTask, shadowTask, ambitionTask);
    
    // Log their reactions (influences which Dreamweaver chooses player)
    GD.Print($"[HERO]: {heroTask.Result}");
    GD.Print($"[SHADOW]: {shadowTask.Result}");
    GD.Print($"[AMBITION]: {ambitionTask.Result}");
}
```

### 8. Performance Optimization

#### Model Preloading

```csharp
// File: project.godot autoload

[autoload]
name="SharedNobodyWhoModel"
type="Node"

// File: Source/Scripts/SharedNobodyWhoModel.cs

public class SharedNobodyWhoModel : Node
{
    private Node? _modelNode;
    
    public override void _Ready()
    {
        // Create NobodyWhoModel node
        var modelType = GD.Load<GDScript>("res://addons/nobodywho/model.gd");
        _modelNode = (Node)modelType.New();
        
        _modelNode.Set("model_path", 
            "res://models/qwen3-4b-instruct-2507-q4_k_m.gguf");
        _modelNode.Set("use_gpu_if_available", true);
        
        AddChild(_modelNode);
        
        // Preload during splash screen
        GD.Print("Preloading LLM model...");
        _modelNode.Call("start_worker");
        GD.Print("LLM model loaded and ready");
    }
    
    public Node GetModelNode() => _modelNode!;
}
```

#### Sampler Configuration for Speed

```csharp
// Fast sampling for real-time narrative
var sampler = new GodotObject();
sampler.Set("temperature", 0.7);
sampler.Set("top_k", 40);
sampler.Set("top_p", 0.9);
sampler.Set("min_p", 0.05);
sampler.Set("seed", GD.Randi()); // Random for variety

_chatNode.Set("sampler", sampler);
```

#### Context Management

```csharp
// Reset context after major transitions to free memory
public void ResetForNewScene()
{
    _dreamweaverSystem.HeroPersona.ResetContext();
    _dreamweaverSystem.ShadowPersona.ResetContext();
    _dreamweaverSystem.AmbitionPersona.ResetContext();
    
    GD.Print("Dreamweaver contexts reset for new scene");
}
```

## Testing Strategy

### Unit Tests

```csharp
// File: Tests/NarrativeGenerationTests.cs

[TestFixture]
public class NarrativeGenerationTests
{
    [Test]
    public async Task GeneratedNarrative_ShouldBeCached()
    {
        var cache = new NarrativeCache();
        var narrative = new GeneratedNarrative
        {
            CacheKey = "test_opening_12345",
            HeroText = "Test hero text",
            ShadowText = "Test shadow text"
        };
        
        cache.Save(narrative);
        
        var loaded = cache.Load("test_opening_12345");
        
        Assert.IsNotNull(loaded);
        Assert.AreEqual(narrative.HeroText, loaded.HeroText);
    }
    
    [Test]
    public async Task SystemPrompt_ShouldIncludeCreativeContent()
    {
        var persona = new DreamweaverPersona("Hero");
        var prompt = persona.BuildSystemPrompt();
        
        Assert.That(prompt, Does.Contain("Hero Dreamweaver"));
        Assert.That(prompt, Does.Contain("courage"));
        Assert.That(prompt, Does.Contain("honor"));
    }
}
```

### Integration Tests

```csharp
// File: Tests/NobodyWhoIntegrationTests.cs

[TestFixture]
public class NobodyWhoIntegrationTests
{
    [Test]
    [Timeout(30000)] // 30 second timeout
    public async Task NobodyWho_ShouldGenerateCoherentNarrative()
    {
        // This test requires actual NobodyWho model loaded
        var dreamweaver = new DreamweaverSystem();
        await dreamweaver.InitializeAsync();
        
        dreamweaver.HeroPersona.Say("Describe the void to the player.");
        var response = await dreamweaver.HeroPersona.ResponseFinishedAsync();
        
        Assert.IsNotEmpty(response);
        Assert.That(response.Length, Is.GreaterThan(50));
        
        // Check for thematic keywords
        var lowerResponse = response.ToLower();
        Assert.That(
            lowerResponse.Contains("void") || 
            lowerResponse.Contains("dark") ||
            lowerResponse.Contains("journey"),
            "Response should contain thematic keywords"
        );
    }
    
    [Test]
    public async Task ToolCalling_ShouldAccessGameState()
    {
        var persona = new DreamweaverPersona("Hero");
        await persona.InitializeAsync();
        
        // Set up game state
        GameState.Instance.Player.Level = 5;
        GameState.Instance.Player.ChoiceHistory.Add(
            new Choice { Description = "Entered the dungeon" }
        );
        
        // Ask LLM to check player state
        persona.Say("What is the player's current level? Use your tools.");
        var response = await persona.ResponseFinishedAsync();
        
        // LLM should have called get_player_stats() tool
        Assert.That(response, Does.Contain("5") | Does.Contain("level"));
    }
}
```

### Playtesting Checklist

**Omega Narration (Player-Visible)**:

- [ ] Omega's voice is cold, systematic, antagonistic but not overtly hostile
- [ ] References "the game", "the system", "the spiral" naturally
- [ ] Omega narration feels like prison/trap awakening
- [ ] Transitions between scenes maintain Omega's omnipotent tone
- [ ] Omega only prominent in Chapter Zero (ready to fade after)

**Dreamweaver Observers (Hidden Commentary)**:

- [ ] Hero/Shadow/Ambition speak TO each other, not to player
- [ ] Observer commentary references "Player 1/2/3" comparisons
- [ ] Dreamweavers debate player's alignment with their paths
- [ ] Commentary builds tension: "Will I choose this one?"
- [ ] Observers react distinctly to same player action

**Choice Tracking System**:

- [ ] Player choices correctly update Dreamweaver interest scores
- [ ] Score changes logged and visible in debug output
- [ ] At end of Scene 5, highest-scoring Dreamweaver identified
- [ ] Choice reasoning clear from observer commentary

**Technical Performance**:

- [ ] Cached narratives load instantly
- [ ] Fresh generation takes <5 seconds on mid-range GPU
- [ ] No inappropriate or off-theme content generated
- [ ] Tool calling works (LLM queries game state correctly)
- [ ] RAG provides relevant creative content context
- [ ] Scene transitions trigger correctly
- [ ] All 5 scenes flow smoothly with dynamic content

**Narrative Quality**:

- [ ] Omega narration maintains consistent voice across scenes
- [ ] Observer commentary feels like Greek chorus
- [ ] Player never directly addressed by Dreamweavers
- [ ] Tension builds: which Dreamweaver will choose?
- [ ] Final choice (end of Scene 5) feels earned

## Migration Path

### Phase 1: Preparation (Week 1)

1. ✅ Model already exists: `models/qwen3-4b-instruct-2507-q4_k_m.gguf`
2. Install NobodyWho addon (if not already installed)
3. Create autoload nodes: `SharedNobodyWhoModel`, `SharedNobodyWhoEmbedding`
4. Test basic chat functionality in isolation

### Phase 2: Core Implementation (Week 2)

1. Implement `NarrativeCache.cs`
2. Implement `CreativeMemoryRAG.cs`
3. Refactor `DreamweaverSystem.cs` with actual NobodyWho integration
4. Update `DreamweaverPersona.cs` with tool calling
5. Add RAG tools to personas

### Phase 3: NarrativeTerminal Integration (Week 2-3)

1. Add `UseDynamicNarrative` toggle
2. Implement `DisplayDynamicNarrativeAsync()`
3. Implement caching logic
4. Add embeddings for player input detection
5. Update Scene 1 → Scene 2 transition

### Phase 4: Testing & Polish (Week 3-4)

1. Write unit tests for caching, prompts, RAG
2. Write integration tests for NobodyWho interaction
3. Extensive playtesting with varied prompts
4. Performance profiling and optimization
5. Documentation updates

### Phase 5: Optional Enhancements (Week 4+)

1. Structured output (JSON) for complex narrative metadata
2. Multiple model size options (tiny/small/large)
3. Custom GBNF grammars for specific formats
4. Conversation history export for debugging
5. Player-facing "creativity slider" (temperature control)

## Consequences

### Positive

1. **Dynamic Replayability**: Every playthrough generates unique narratives
2. **Creative Team Freedom**: No need to restructure existing content
3. **Emergent Storytelling**: Player actions influence narrative organically
4. **Performance**: Caching makes repeated content instant
5. **Offline**: No cloud dependency, no data collection
6. **Moddable**: Players can swap models, adjust prompts

### Negative

1. **Unpredictability**: LLM may generate unexpected content
2. **Testing Complexity**: Can't test every possible generation
3. **File Size**: Model adds ~2.5GB to distribution
4. **Hardware Requirements**: GPU recommended for <3s latency
5. **Platform Limitations**: May not work on WebGL/mobile

### Mitigation Strategies

1. **Unpredictability**: Strict system prompts + content filtering + fallback to static
2. **Testing**: Extensive fuzzing + log all generations + community feedback
3. **File Size**: Optional DLC download + multiple model sizes
4. **Hardware**: Performance settings + CPU fallback mode
5. **Platform**: Hybrid mode (static narrative on unsupported platforms)

## Success Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| **Narrative Quality** | >85% coherent | Playtest surveys |
| **Generation Speed** | <3s first token | Profiling |
| **Cache Hit Rate** | >60% | Telemetry |
| **Player Engagement** | +25% session time | Analytics |
| **Crash Rate** | <0.5% | Error logs |

## Technical Risks

| Risk | Impact | Likelihood | Mitigation |
|------|--------|------------|------------|
| LLM generates off-theme content | High | Low | Strict prompts, content validation |
| Performance issues on low-end GPU | Medium | Medium | CPU fallback, smaller models |
| Cache grows too large | Low | Medium | Auto-cleanup old caches (>30 days) |
| Tool calling fails | Medium | Low | Graceful fallback, error handling |
| Embeddings inaccurate | Medium | Low | Threshold tuning, manual overrides |

## Visual & Thematic References

### Omega Spiral Logos

The project includes multiple logo variations representing different thematic elements:

**Monochrome Logo** (`logo-monochrome.png`):

- Pure white Ω (Omega) symbol on black void
- Cosmic rays emanating from center
- Represents the stark, binary nature of the game system
- Omega as the center point - the prison/trap
- Use for: Omega's narration scenes, Chapter Zero title screens

**Alternate Logo** (`logo-alternate.png`):

- Three-colored Ω representing the three Dreamweaver paths:
  - **White/Silver**: Hero path (courage, honor, light)
  - **Orange/Gold**: Ambition path (power, fire, conquest)
  - **Red/Crimson**: Shadow path (balance, pragmatism, natural law)
- Swirling, dynamic composition showing paths intertwining
- Represents the choice players will make
- Use for: Dreamweaver selection moments, path choice screens

### Color Symbolism

| Path | Primary Color | Secondary Color | Symbolism |
|------|--------------|-----------------|-----------|
| **Hero** | Silver/White | Cool Blue | Light, clarity, traditional heroism |
| **Shadow** | Deep Red | Purple/Violet | Balance, twilight, natural cycles |
| **Ambition** | Orange/Gold | Crimson | Fire, power, conquest |
| **Omega** | Black/White | None | Binary, systematic, void |

### Narrative Tone by Entity

| Entity | Voice | Example Phrase | Color Association |
|--------|-------|----------------|-------------------|
| **Omega** | Cold, systematic | "The system awakens..." | Black/White (monochrome) |
| **Hero Observer** | Noble, evaluative | "Courage... interesting..." | Silver/White |
| **Shadow Observer** | Pragmatic, dry | "Wise restraint..." | Deep Red/Purple |
| **Ambition Observer** | Sharp, hungry | "Now THAT'S potential..." | Orange/Gold |

## Technical References

- [NobodyWho GitHub Repository](https://github.com/nobodywho-ooo/nobodywho)
- [NobodyWho Documentation](https://nobodywho-ooo.github.io/nobodywho/)
- [Simple Chat Guide](https://nobodywho-ooo.github.io/nobodywho/chat/simple-chat/)
- [Structured Output Guide](https://nobodywho-ooo.github.io/nobodywho/chat/structured-output/)
- [RAG Guide](https://nobodywho-ooo.github.io/nobodywho/rag/)
- [Qwen3 Model Card](https://huggingface.co/Qwen/Qwen3-4B-GGUF)
- [ADR-0003: NobodyWho LLM Integration](adr-0003-nobodywho-llm-integration.md)
- [Dreamweaver System Overview](../dreamweaver.md)

## Decision Makers

- **Technical Lead**: Adam (jessenaiman)
- **Architecture**: AI Assistant (Claude)
- **Approval Required**: Project stakeholders

## Next Steps

1. **Review this ADR** with team
2. **Approve architecture** and proceed to Phase 2
3. **Create implementation tasks** from this document
4. **Set up development branch** (`feature/nobodywho-dynamic-narrative`)
5. **Begin Phase 1** (preparation)

---

**Created**: 2025-10-10  
**Status**: Awaiting Review  
**Next Review**: After team approval  
**Implementation Start**: TBD
