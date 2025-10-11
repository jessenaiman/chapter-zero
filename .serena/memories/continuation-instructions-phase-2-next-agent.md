# Continuation Instructions: NobodyWho Integration Phase 2

## Current State Summary

**Project:** Omega Spiral - Narrative RPG with dynamic LLM-driven storytelling  
**Branch:** 004-implement-omega-spiral  
**Phase:** Phase 2 Implementation (ADR-0004)  
**Progress:** Task 1 of 10 complete

### What Has Been Done

1. **ADR-0004 Created** (900+ lines comprehensive architecture document)
   - Narrative structure: Omega (antagonist) + 3 Dreamweaver observers (hidden)
   - System prompt examples for all entities
   - Dual-track narration architecture
   - Implementation plan (5 phases, Phase 2 active)
   - Visual theme integration with logo symbolism

2. **OmegaNarrator.cs Implemented** (Task 1 ✅)
   - Antagonist narrator for Chapter Zero
   - NobodyWhoChat integration for LLM generation
   - Signal-based streaming responses
   - Default system prompt with BBG character
   - Async initialization pattern

### Critical Narrative Context

**READ THIS FIRST - Common Misunderstanding:**

- **Omega** = Big Bad Guy (BBG), the antagonist, the prison/system
  - NOT a helper or guide
  - Cold, systematic, clinical voice
  - Prominent in Chapter Zero only
  - Player SEES Omega's narration

- **Dreamweavers** = Three observers (Hero, Shadow, Ambition)
  - Hidden commentary, Greek chorus
  - Speak to EACH OTHER, not to player
  - Refer to "Player 1/2/3" (3 players competing)
  - Player DOES NOT SEE observer commentary
  - Commentary logged for debugging/playtesting
  - ONE observer chooses our player at end of Scene 5

- **Chapter Zero Structure**
  - 5 scenes evaluating player
  - Omega narrates game events (visible)
  - Observers discuss player choices (hidden)
  - Scoring system determines which Dreamweaver chooses player

### What Needs to Be Done Next

**Immediate Task Options:**

**Option A: Task 2 - DreamweaverObserver.cs** (Natural progression)
- Base class for Hero/Shadow/Ambition observers
- Similar to OmegaNarrator but for hidden commentary
- Speaks to other Dreamweavers, evaluates choices
- Longer context (4096 tokens vs 2048)
- Three subclasses needed: HeroObserver, ShadowObserver, AmbitionObserver

**Option B: Task 9 - Autoload Nodes** (Unblock testing)
- Create SharedNobodyWhoModel.cs singleton
- Create SharedNobodyWhoEmbedding.cs singleton
- Register in project.godot as autoloads
- Allows testing OmegaNarrator immediately
- Quick win (~30 minutes)

**Recommendation:** Do Task 9 first to unblock testing, then Task 2

### Remaining Phase 2 Tasks

- [ ] Task 2: DreamweaverObserver.cs + 3 subclasses (IN PROGRESS)
- [ ] Task 3: DreamweaverChoiceTracker.cs (scoring system)
- [ ] Task 4: NarrativeCache.cs (save/load JSON)
- [ ] Task 5: CreativeMemoryRAG.cs (embeddings search)
- [ ] Task 6: SystemPromptBuilder.cs (creative → prompts)
- [ ] Task 7: Refactor DreamweaverSystem.cs (orchestration)
- [ ] Task 8: Update NarrativeTerminal.cs (dual-track display)
- [ ] Task 9: Autoload nodes (SharedModel + Embedding)
- [ ] Task 10: Testing (unit + integration + playtest)

### Key Files to Review

**Must Read:**
1. `docs/adr/adr-0004-nobodywho-dynamic-narrative-architecture.md`
   - Complete architecture specification
   - System prompt examples for all entities
   - Implementation patterns

2. `docs/adr/adr-0004-SUMMARY.md`
   - Quick reference with visual diagrams
   - Omega vs. Observer comparison table

3. `Source/Scripts/OmegaNarrator.cs`
   - Reference implementation for Task 2
   - Async/signal patterns to replicate

**Context Files:**
4. `docs/scenes/*.md` - Creative team content for system prompts
5. `Source/Scripts/DreamweaverSystem.cs` - Current orchestrator (needs refactor)
6. `Source/Scripts/NarrativeTerminal.cs` - UI component (needs update)

### Technical Patterns to Follow

**C# Standards (from .github/copilot-instructions.md):**
- XML documentation on all public members
- `<summary>`, `<param>`, `<returns>`, `<remarks>` tags
- `<see cref>` for type references
- Async suffix on async methods
- Return `Task<T>` or `Task`, never `void` (except event handlers)
- Use `ConfigureAwait(false)` in library code

**Godot Integration:**
- Inherit from `Node` for scene tree
- Use `[Export]` for editor properties
- Use `[Signal]` delegate for events
- `_Ready()` for initialization
- `_ExitTree()` for cleanup
- `GD.PrintRich()` for colored console output

**NobodyWho Patterns (from OmegaNarrator):**
```csharp
// Get shared model from autoload
_modelNode = GetNode<Node>("/root/SharedNobodyWhoModel");

// Create chat node dynamically
var chatScript = GD.Load<GDScript>("res://addons/nobodywho/chat.gd");
_chatNode = (Node)chatScript.New();

// Configure and connect
_chatNode.Set("model_node", _modelNode);
_chatNode.Set("system_prompt", SystemPrompt);
_chatNode.Connect("response_updated", Callable.From<string>(OnResponseUpdated));
_chatNode.Connect("response_finished", Callable.From<string>(OnResponseFinished));

// Add to scene tree and start
AddChild(_chatNode);
_chatNode.Call("start_worker");
```

### DreamweaverObserver.cs Specification (Task 2)

**Requirements from ADR-0004:**

1. **Base Class Structure**
   ```csharp
   public abstract class DreamweaverObserver : Node
   {
       // Same NobodyWhoChat integration as OmegaNarrator
       // But with abstract methods for subclass customization
       public abstract string GetSystemPrompt();
       public abstract string GetObserverName(); // "Hero", "Shadow", "Ambition"
   }
   ```

2. **Voice Characteristics (Abstract, Defined in Subclasses)**
   - **HeroObserver**: Noble, idealistic, values courage/honor
     - "Player 1 chose mercy. A heart that leads with compassion..."
   - **ShadowObserver**: Pragmatic, balanced, values wisdom/nuance
     - "Player 1's hesitation shows awareness of complexity..."
   - **AmbitionObserver**: Hungry, power-seeking, values dominance
     - "Player 1 seizes power without hesitation. Promising..."

3. **Key Differences from OmegaNarrator**
   - **Context Length**: 4096 tokens (vs 2048)
   - **Audience**: Hidden, logged only (vs visible)
   - **References**: "Player 1/2/3" (vs "the player")
   - **Purpose**: Evaluate choices (vs narrate events)
   - **Subclasses**: 3 concrete implementations needed

4. **Methods to Implement**
   - `ObserveChoice(string choiceDescription)`: Generate hidden commentary
   - `GetInterestLevel()`: Return 0.0-1.0 sentiment score
   - `ResetObservation()`: Clear context for new scene

5. **Signals**
   - `ObservationFinished(string commentary, float interest)`
   - `ObservationUpdated(string token)`

### Autoload Nodes Specification (Task 9)

**SharedNobodyWhoModel.cs:**
```csharp
public partial class SharedNobodyWhoModel : Node
{
    private Node? _modelNode;
    
    [Export] public string ModelPath { get; set; } = "res://models/qwen3-4b-instruct-2507-q4_k_m.gguf";
    [Export] public bool UseGpu { get; set; } = true;
    
    public override async void _Ready()
    {
        // Load NobodyWhoModel.gd
        // Configure model_path, use_gpu, n_gpu_layers
        // Start worker
    }
    
    public Node GetModelNode() => _modelNode;
}
```

**SharedNobodyWhoEmbedding.cs:**
```csharp
public partial class SharedNobodyWhoEmbedding : Node
{
    private Node? _embeddingNode;
    
    [Export] public string ModelPath { get; set; } = "res://models/qwen3-4b-instruct-2507-q4_k_m.gguf";
    
    public override async void _Ready()
    {
        // Load NobodyWhoEmbedding.gd
        // Configure model_path
        // Start worker
    }
    
    public async Task<float[]> GetEmbeddingAsync(string text) { /* ... */ }
}
```

**Register in project.godot:**
```ini
[autoload]
SharedNobodyWhoModel="*res://Source/Scripts/SharedNobodyWhoModel.cs"
SharedNobodyWhoEmbedding="*res://Source/Scripts/SharedNobodyWhoEmbedding.cs"
```

### Testing Strategy

**Once Autoloads Exist (Task 9):**
```csharp
// Test OmegaNarrator
var omega = new OmegaNarrator();
await omega.InitializeAsync();
omega.Say("The game awakens.");
// Verify ResponseUpdated/Finished signals fire
```

**Once Observer Exists (Task 2):**
```csharp
// Test HeroObserver
var hero = new HeroObserver();
await hero.InitializeAsync();
hero.ObserveChoice("Player chose mercy over vengeance.");
// Verify hidden commentary generated
// Verify interest level calculated
```

### Common Pitfalls to Avoid

1. **DON'T** have Dreamweaver observers speak directly to player
   - ❌ "You chose wisely, brave one"
   - ✅ "Player 1's choice shows courage. The Hero path strengthens."

2. **DON'T** make Omega helpful or friendly
   - ❌ "Welcome, hero! Let me guide you"
   - ✅ "The game initializes. Player 1 awakens. The spiral begins."

3. **DON'T** forget async/await patterns
   - All NobodyWho operations are async
   - Use `Task<T>` return types
   - Signal-based completion for Godot integration

4. **DON'T** skip XML documentation
   - Required for all public members
   - Follow standards in .github/copilot-instructions.md

5. **DON'T** forget context management
   - Reset between scenes to prevent bleeding
   - Different context lengths for different narrators

### Resources Available

**NobodyWho Plugin:**
- `res://addons/nobodywho/model.gd` - Model loader
- `res://addons/nobodywho/chat.gd` - Chat interface
- `res://addons/nobodywho/embedding.gd` - Embeddings
- Documentation: Check plugin repository for API details

**Existing Model:**
- `models/qwen3-4b-instruct-2507-q4_k_m.gguf` (2.5GB, already present)
- Qwen3 4B Instruct, quantized Q4_K_M
- Good for narrative generation, ~2-4 tokens/sec on CPU

**Creative Content:**
- `docs/scenes/*.md` - Scene descriptions, narrative beats
- Use for building system prompts with RAG (Task 5)

### Questions? Check These First

1. "What's Omega's role?" → BBG/antagonist, cold/systematic
2. "What do Dreamweavers do?" → Hidden observers, Greek chorus
3. "Do players see observers?" → NO, only Omega narration
4. "How many players?" → 3 total, we follow Player 1
5. "When does observer become guide?" → After Scene 5, when chosen
6. "What's Chapter Zero?" → 5-scene evaluation period

### Success Criteria for Next Session

**Minimum Viable:**
- [ ] Task 9 complete (autoloads working)
- [ ] OmegaNarrator tested successfully
- [ ] Task 2 started (DreamweaverObserver.cs created)

**Ideal Progress:**
- [ ] Task 9 complete
- [ ] Task 2 complete (base + 3 subclasses)
- [ ] Task 3 started (ChoiceTracker)

### How to Continue

1. **Read ADR-0004** (critical context)
2. **Choose Task 9 or Task 2** (recommendation: Task 9)
3. **Follow patterns from OmegaNarrator.cs**
4. **Test as you build** (don't wait until end)
5. **Update todo list** after each task
6. **Write progress memory** when stuck or switching

## Memories to Read

- `phase-2-omega-narrator-implementation-complete` (this session's work)
- Existing memories about NobodyWho research (if available)
- ADR-0004 (comprehensive architecture document)

## Contact Points

If stuck on:
- **NobodyWho API**: Check plugin docs, OmegaNarrator.cs reference
- **Narrative structure**: Re-read ADR-0004 sections 2-4
- **C# patterns**: Check .github/copilot-instructions.md
- **Godot integration**: Follow Node/Signal patterns in OmegaNarrator

Good luck! The architecture is solid, just follow the patterns. 🚀
