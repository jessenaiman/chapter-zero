# Phase 2 Infrastructure Implementation - Complete

**Date**: October 10, 2025
**Session**: Completed remaining Phase 2 NobodyWho integration tasks

## Completed Tasks (7/11 total)

### Task 4: NarrativeCache.cs ✅
**File**: `Source/Scripts/NarrativeCache.cs` (346 lines)
**Purpose**: Caching system for LLM-generated narrative to avoid regenerating same content

**Key Features**:
- Dual-layer cache (memory + disk) for fast retrieval
- Cache keys generated from `stepId + personaId + gameSeed` via SHA256
- Validates cached entries against schema context lines (detects schema changes)
- LRU eviction policy when cache exceeds 500 entries
- JSON serialization to `user://narrative_cache/` directory
- `LoadCachedNarrativeAsync()` - retrieves cached content if valid
- `CacheNarrativeAsync()` - stores generated content
- `ClearCache()` - resets all cached data
- `GetCacheStats()` - debugging information

**Integration**: Used by DreamweaverSystem for all LLM generation (narrative + commentary)

### Task 6: SystemPromptBuilder.cs ✅
**File**: `Source/Scripts/SystemPromptBuilder.cs` (234 lines)
**Purpose**: Assembles context-aware system prompts for LLM personas

**Key Features**:
- `BuildOmegaNarratorPromptAsync()` - constructs prompts with:
  - Base persona system prompt
  - Current game state (player name, selected thread, seed)
  - Schema lines as "creative direction"
  - RAG-retrieved related narrative beats
  - Output format instructions
- `BuildObserverPromptAsync()` - constructs prompts for Dreamweaver observers with:
  - Observer role reminder (hidden commentary)
  - Player's choice context
  - Game state
  - RAG context
- `BuildSimplePrompt()` - for opening lines/one-shot generation
- `BuildChoiceGenerationPrompt()` - for dynamic choice generation
- `ProcessVariables()` - replaces {playerName}, {selectedThread}, {gameSeed}
- `GetContextAvailability()` - debugging helper

**Dependencies**: CreativeMemoryRAG (for related beats), GameState (for variables)

### Task 5: CreativeMemoryRAG.cs ✅
**File**: `Source/Scripts/CreativeMemoryRAG.cs` (365 lines)
**Purpose**: Retrieval-Augmented Generation for creative schema content

**Key Features**:
- `IndexSchemaAsync()` - indexes scene1-schema.json beats using embeddings:
  - Extracts content from dialogue, choice, input, effect steps
  - Generates embeddings via SharedNobodyWhoEmbedding
  - Stores as `NarrativeBeat` records with StepId, Type, Content, Embedding
- `FindRelatedBeatsAsync()` - semantic search by step ID:
  - Calculates cosine similarity between embeddings
  - Returns top-K most relevant beats
  - Used by SystemPromptBuilder to enrich prompts
- `FindRelatedBeatsFromTextAsync()` - search by arbitrary query text
  - Useful for player input analysis
- `GetStats()` - returns beat counts by type
- `ClearIndex()` - resets indexed content

**Dependencies**: SharedNobodyWhoEmbedding (for embedding generation)

### Task 7: DreamweaverSystem.cs Refactoring ✅
**File**: `Source/Scripts/DreamweaverSystem.cs` (refactored to 650+ lines)
**Purpose**: Orchestrates all NobodyWho integration components

**Major Changes**:
- Added infrastructure node references:
  - `NarrativeCache` - for caching
  - `SystemPromptBuilder` - for prompt assembly
  - `CreativeMemoryRAG` - for RAG retrieval
  - `DreamweaverChoiceTracker` - for choice tracking
- Added persona node references:
  - `OmegaNarrator` - BBG narrator
  - `HeroObserver`, `ShadowObserver`, `AmbitionObserver` - hidden observers

**New Methods**:
- `GenerateNarrativeAsync(stepId, contextLines)` - FULL ORCHESTRATION:
  1. Check NarrativeCache for cached version
  2. Build prompt via SystemPromptBuilder (includes RAG context)
  3. Generate via OmegaNarrator
  4. Cache result
  5. Emit signal
- `GenerateObserverCommentaryAsync(stepId, choiceText)` - PARALLEL GENERATION:
  1. Generates commentary from all 3 observers in parallel
  2. Caches each commentary
  3. Updates DreamweaverChoiceTracker with sentiment scores
  4. Returns dictionary of observer → commentary
- `LoadCachedNarrativeAsync()` - delegates to NarrativeCache
- `CacheNarrativeAsync()` - delegates to NarrativeCache
- `IndexSceneSchemaAsync(schemaPath)` - delegates to CreativeMemoryRAG
- `GetSystemStats()` - comprehensive system health check
- `GenerateSingleObserverCommentaryAsync()` - private helper for individual observer

**Integration**: Called by NarrativeTerminal.cs for dynamic narrative generation

### Task 3: DreamweaverChoiceTracker.cs ✅
**File**: `Source/Scripts/DreamweaverChoiceTracker.cs` (291 lines)
**Purpose**: Tracks player choices and determines winning Dreamweaver thread

**Key Features**:
- `RecordChoice()` - stores choice with observer commentary:
  - Analyzes sentiment for each observer (0-1 scale)
  - Updates cumulative scores for hero/shadow/ambition
  - Checks if winning threshold reached (60% + 3 minimum choices)
  - Emits `ChoiceRecorded` signal
- `AnalyzeSentiment()` - keyword-based sentiment analysis:
  - Hero keywords: honor, courage, noble, brave, sacrifice, duty, light, hope
  - Shadow keywords: balance, nature, wisdom, pragmatic, survival, adapt, cycle, truth
  - Ambition keywords: power, ambition, control, dominate, achieve, superior, strong, master
  - Negative keywords: weak, foolish, naive, mistake, wrong, doubt, hesitate
- `CheckForWinner()` - determines winning thread:
  - Calculates percentages of total score
  - If one thread ≥ 60%, declares winner
  - Updates GameState.SelectedThread
  - Emits `ThreadWinnerDetermined` signal
- `GetThreadScore(thread)` - returns score for specific thread
- `GetAllScores()` - returns all thread scores
- `GetLeadingThread()` - returns current leader (no threshold)
- `GetWinningThread()` - returns determined winner (with threshold)
- `GetChoiceHistory()` - full history with scores
- `GetStats()` - tracking statistics
- `Reset()` - clears all data for new playthrough

**Integration**: Called by DreamweaverSystem after observer commentary generation

## Build Verification

✅ **All new files compile successfully**
- No errors in NarrativeCache.cs
- No errors in SystemPromptBuilder.cs
- No errors in CreativeMemoryRAG.cs
- No errors in DreamweaverChoiceTracker.cs
- No errors in DreamweaverSystem.cs refactor
- Only pre-existing errors in Battler.cs, UIEquipment.cs, UICombatLog.cs

## Architecture Summary

```
NarrativeTerminal.cs (schema-driven execution)
    ↓ ExecuteDynamicDialogueAsync()
    ↓ GenerateObserverCommentaryAsync()
    ↓
DreamweaverSystem.cs (orchestration hub)
    ├─→ NarrativeCache (check/store)
    ├─→ SystemPromptBuilder (assemble prompts)
    │     └─→ CreativeMemoryRAG (get related beats)
    ├─→ OmegaNarrator (generate narrative)
    ├─→ HeroObserver (generate commentary)
    ├─→ ShadowObserver (generate commentary)
    ├─→ AmbitionObserver (generate commentary)
    └─→ DreamweaverChoiceTracker (record choices, track alignment)
```

## Data Flow

1. **Narrative Generation**:
   - NarrativeTerminal executes schema step (dialogue type)
   - If UseDynamicNarrative=true, calls DreamweaverSystem.GenerateNarrativeAsync()
   - DreamweaverSystem checks cache (by stepId + "omega" + gameSeed)
   - If cache miss, SystemPromptBuilder assembles prompt with RAG context
   - OmegaNarrator generates via LLM
   - Result cached and returned

2. **Observer Commentary**:
   - NarrativeTerminal executes schema step (choice type)
   - Player selects choice
   - NarrativeTerminal calls DreamweaverSystem.GenerateObserverCommentaryAsync()
   - DreamweaverSystem generates commentary from all 3 observers in parallel
   - Each observer uses SystemPromptBuilder for context-aware prompt
   - Results cached individually
   - DreamweaverChoiceTracker analyzes sentiment and updates scores
   - If winner determined, GameState.SelectedThread updated

3. **RAG Indexing**:
   - NarrativeTerminal.TryLoadSceneSchema() loads scene1-schema.json
   - Calls DreamweaverSystem.IndexSceneSchemaAsync("res://docs/scenes/scene1-schema.json")
   - CreativeMemoryRAG extracts content from each step
   - Generates embeddings via SharedNobodyWhoEmbedding
   - Stores indexed beats for later retrieval

## Remaining Work

- **Task 8**: Update NarrativeTerminal.cs (verify integration, likely minimal)
- **Task 10**: Testing (unit tests for all new components)

## Next Steps

1. Verify NarrativeTerminal.cs calls match DreamweaverSystem API
2. Register new autoload nodes in project.godot:
   - NarrativeCache
   - SystemPromptBuilder
   - CreativeMemoryRAG
   - DreamweaverChoiceTracker
3. Create unit tests for each component
4. Integration test: full scene 1 flow with dynamic narrative
5. Test cache persistence across sessions
6. Test RAG indexing and retrieval
7. Test choice tracking and winner determination
