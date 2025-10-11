# NarrativeTerminal Architecture: Schema-Driven Dynamic Narrative

## Overview

`NarrativeTerminal.cs` now supports both **static** and **dynamic** narrative modes, using the scene schema as the source of truth for both.

## How It Works

### 1. Schema as Creative Direction

The `scene1-schema.json` file contains the narrative structure created by the creative team:

```json
{
  "sceneId": "scene1_terminal",
  "title": "Terminal Boot Sequence",
  "steps": [
    {
      "id": "boot",
      "type": "dialogue",
      "lines": [
        "SYSTEM INITIALIZING...",
        "OMEGA SPIRAL v0.1.0",
        "LOADING CONSCIOUSNESS MATRIX..."
      ],
      "delay": 2,
      "nextStep": "thread_choice"
    },
    {
      "id": "thread_choice",
      "type": "choice",
      "prompt": "Which thread will you follow?",
      "options": [
        { "id": "hero", "text": "The Hero's Path", "nextStep": "hero_dialogue" },
        { "id": "shadow", "text": "The Shadow's Way", "nextStep": "shadow_dialogue" }
      ]
    }
  ]
}
```

### 2. Two Execution Paths

#### Static Path (UseDynamicNarrative = false)
- Displays schema lines directly to player
- Fast, predictable, no LLM required
- Good for testing and development

#### Dynamic Path (UseDynamicNarrative = true)
- Uses schema lines as **context/direction** for LLM
- **OmegaNarrator** reads schema and generates cold, systematic narration
- **DreamweaverObservers** provide hidden commentary on choices
- Narration is cached to avoid regenerating same content
- More immersive, adapts to game state

### 3. Key Methods

#### `ExecuteDialogueStepAsync()`
```csharp
// Checks UseDynamicNarrative flag
if (UseDynamicNarrative && dreamweaverSystem != null)
{
    // Dynamic: Schema provides context, LLM generates narration
    await ExecuteDynamicDialogueAsync();
}
else
{
    // Static: Display schema lines directly
    foreach (string line in currentStep.Lines)
    {
        await DisplayTextWithTypewriterAsync(line);
    }
}
```

#### `ExecuteDynamicDialogueAsync()`
```csharp
// 1. Build context from schema
string schemaContext = string.Join("\n", currentStep.Lines);

// 2. Check cache
var cached = await dreamweaverSystem.LoadCachedNarrativeAsync(cacheKey);
if (cached != null) { /* use cache */ }

// 3. Generate with LLM
string prompt = $@"Scene context from creative team:
{schemaContext}

Generate cold, systematic narration for this moment.";

string narration = await dreamweaverSystem.GenerateNarrativeAsync("omega", prompt);

// 4. Cache and display
await dreamweaverSystem.CacheNarrativeAsync(cacheKey, narration);
await DisplayTextWithTypewriterAsync(narration);
```

#### `ExecuteChoiceStepAsync()`
```csharp
// Display choices from schema
for (int i = 0; i < currentStep.Options.Count; i++)
{
    DisplayImmediate($"  {i + 1}. {option.Text}");
}

// If dynamic, trigger hidden Observer commentary
if (UseDynamicNarrative && dreamweaverSystem != null)
{
    _ = GenerateObserverCommentaryAsync(); // Runs in background
}
```

#### `GenerateObserverCommentaryAsync()`
```csharp
// Build context about the choice
string choiceContext = $@"Choice presented: {currentStep.Prompt}
Options:
{string.Join("\n", currentStep.Options.Select((o, i) => $"{i + 1}. {o.Text}"))}";

// Request hidden commentary from Hero/Shadow/Ambition observers
var commentary = await dreamweaverSystem.GenerateObserverCommentaryAsync(choiceContext);

// Store for potential future use (not shown to player)
await dreamweaverSystem.CacheNarrativeAsync(cacheKey, commentary);
```

## Benefits of This Architecture

### 1. **Creative Team Workflow Unchanged**
- Writers create narrative structure in JSON
- No need to learn prompt engineering
- Schema is version-controlled and reviewable

### 2. **Dynamic Narrative Without Refactoring**
- Schema provides context to LLM
- LLM generates narration that fits the moment
- Narration adapts to game state (thread, player name, etc.)

### 3. **Observer Commentary**
- Hidden "Greek chorus" discussing player's choices
- Evaluates which Dreamweaver thread the player aligns with
- Can be revealed at end of game for reflection

### 4. **Caching for Performance**
- Generated narration is cached by step ID + game seed
- Subsequent playthroughs reuse cached content
- Only regenerates when game state changes significantly

### 5. **Easy Toggle**
- `UseDynamicNarrative` flag switches between modes
- Can test static version quickly
- Can deploy with dynamic disabled if LLM unavailable

## Example Flow

### Static Mode
```
Schema Line: "SYSTEM INITIALIZING..."
↓
Display directly to player
↓
Next step
```

### Dynamic Mode
```
Schema Line: "SYSTEM INITIALIZING..."
↓
OmegaNarrator receives: "Scene context: SYSTEM INITIALIZING..."
↓
OmegaNarrator generates: "Your consciousness flickers. My systems engage. Welcome to the spiral, subject 7734."
↓
Cache generated narration
↓
Display to player with typewriter effect
↓
Next step
```

### Choice with Observers
```
Schema: "Which thread will you follow?"
         1. Hero's Path
         2. Shadow's Way
↓
Display choices to player
↓
(In background) DreamweaverObservers receive choice context
↓
(In background) Hero: "This one shows promise for the heroic path..."
(In background) Shadow: "But notice the hesitation. They doubt themselves."
(In background) Ambition: "Weakness. I need someone with DRIVE."
↓
(In background) Cache observer commentary
↓
Wait for player input
```

## Integration with DreamweaverSystem

The `DreamweaverSystem` provides these methods (to be implemented):

```csharp
// Generate narration from a specific persona (omega, hero, shadow, ambition)
Task<string> GenerateNarrativeAsync(string personaId, string prompt);

// Generate hidden commentary from all three observers
Task<string> GenerateObserverCommentaryAsync(string choiceContext);

// Cache management
Task<string?> LoadCachedNarrativeAsync(string key);
Task CacheNarrativeAsync(string key, string content);
```

## Next Steps

1. **Test static mode** in Godot with schema
2. **Implement DreamweaverSystem methods** for dynamic generation
3. **Create prompt templates** for OmegaNarrator and Observers
4. **Add CreativeMemoryRAG** for embedding-based creative content search
5. **Implement narrative caching** system
6. **Connect Observers** to end-game reflection/analysis

## Summary

**No major refactoring needed!** The schema-based architecture is exactly right. We just:
- Added `ExecuteDynamicDialogueAsync()` to use schema as LLM context
- Added `GenerateObserverCommentaryAsync()` for hidden commentary
- Made both optional via `UseDynamicNarrative` flag

The creative team's schema is the single source of truth for both static and dynamic modes. In dynamic mode, the schema tells the LLM **what to narrate about**, and the LLM generates **how to narrate it** in Omega's voice.
