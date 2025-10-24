# NobodyWho LLM Integration for Dreamweaver Personas

## Overview

Research completed on October 9, 2025 for integrating local LLM capabilities into Omega Spiral's narrative system using the [nobodywho](https://github.com/nobodywho-ooo/nobodywho) framework.

## Key Architectural Components

### NobodyWho Framework Features

- **Local LLM execution**: No cloud dependencies, runs GGUF models via llama.cpp
- **Godot 4.x native**: Provides `NobodyWhoChat`, `NobodyWhoModel`, `NobodyWhoEmbedding` nodes
- **GPU acceleration**: Vulkan/Metal backends for fast inference
- **Structured output**: Grammar-enforced JSON responses
- **Tool calling**: LLMs can query game state (inventory, stats, etc.)
- **Embeddings**: Semantic similarity matching for natural language input

### Dreamweaver System Design

#### Three Dreamweaver Personas

1. **Hero Dreamweaver** (Courage & Honor)
   - System Prompt: Inspiring, adventurous, noble choices
   - JSON Format: `{"advice": "...", "challenge": "...", "moral": "..."}`
   - Tone: Encouraging, bold, traditional hero archetype

2. **Shadow Dreamweaver** (Mystery & Power)
   - System Prompt: Pragmatic, cunning, hidden knowledge
   - JSON Format: `{"whisper": "...", "secret": "...", "cost": "..."}`
   - Tone: Mysterious, tempting, morally ambiguous

3. **Ambition Dreamweaver** (Achievement & Legacy)
   - System Prompt: Strategic, competitive, legacy-focused
   - JSON Format: `{"strategy": "...", "goal": "...", "reward": "..."}`
   - Tone: Motivational, results-oriented, visionary

#### Omega System (Meta-Narrator)

- System Prompt: Omniscient, philosophical, weaves narrative threads
- JSON Format: `{"narration": "...", "choice_context": "...", "consequence": "..."}`
- Tone: Cosmic, reflective, literary
- Context Length: 8192 tokens (vs 4096 for Dreamweavers)

### Technical Architecture

```
Project Structure:
├── addons/nobodywho/ (plugin)
├── Source/
│   ├── Data/models/
│   │   └── qwen3-4b.gguf (~2.5GB)
│   ├── Scripts/
│   │   ├── DreamweaverSystem.cs (orchestrator)
│   │   ├── DreamweaverPersona.cs (C# wrapper)
│   │   ├── OmegaOracle.cs (meta-narrator)
│   │   └── NarrativeTerminal.cs (enhanced)
│   └── Scenes/
│       └── DreamweaverCore.tscn (4 chat nodes + 1 model node)
```

### Integration Points

1. **DreamweaverSystem.cs**
   - Orchestrates consultations with all three Dreamweavers
   - Aggregates responses and consults Omega for synthesis
   - Emits `AllDreamweaversResponded` signal with complete consultation

2. **DreamweaverPersona.cs**
   - C# wrapper around `NobodyWhoChat` GDScript node
   - Handles signal connections (`response_updated`, `response_finished`)
   - Manages persona-specific system prompts and response parsing

3. **NarrativeTerminal.cs Enhancement**
   - Hybrid mode: LLM-powered OR static JSON fallback
   - Displays Dreamweaver consultations in terminal format
   - Parses JSON responses into game-relevant data

### Recommended LLM Model

**Primary**: Qwen3-4B-Q4_K_M.gguf
- Size: ~2.5GB
- Quality: Excellent for narrative generation
- Speed: Fast on mid-range GPU
- Tool calling: Supported (critical feature)
- Source: https://huggingface.co/Qwen/Qwen3-4B-GGUF

**Alternatives**:
- Qwen3-0.6B-Q6_K_XL.gguf (~600MB): Faster, lower quality
- Qwen3-14B-Q4_K_M.gguf (~8GB): Slower, higher quality

## Implementation Phases

### Phase 1: Setup (Week 1)
- Install nobodywho plugin from GitHub releases
- Download Qwen3-4B GGUF model
- Enable plugin in Godot project settings
- Create `DreamweaverCore.tscn` with 4 chat nodes

### Phase 2: Core Integration (Week 1-2)
- Implement `DreamweaverPersona.cs` wrapper class
- Implement `DreamweaverSystem.cs` orchestrator
- Define system prompts for each persona
- Enhance `NarrativeTerminal.cs` with LLM display

### Phase 3: Advanced Features (Week 2-3)
- Add tool calling for game state queries (inventory, stats)
- Implement embedding-based semantic input matching
- Add context management and memory optimization
- Create sampler configurations for quality/performance tuning

### Phase 4: Testing & Optimization (Week 3-4)
- Performance testing (loading time, response latency, memory)
- Response quality validation (coherence, theme adherence)
- Fallback behavior testing (invalid JSON, errors)
- Platform compatibility testing (Linux, Windows, macOS)

## Key Code Patterns

### Consultation Flow

```csharp
// In NarrativeTerminal.cs
_dreamweaverSystem.ConsultAllDreamweavers("Player faces a moral dilemma...");

// DreamweaverSystem orchestrates:
// 1. Query all three Dreamweavers simultaneously
// 2. Collect their JSON responses
// 3. Feed aggregated responses to Omega
// 4. Emit AllDreamweaversResponded signal

// NarrativeTerminal receives:
private void OnDreamweaversConsultation(Dictionary consultations)
{
    var hero = ParseHeroResponse(consultations["hero"]);
    var shadow = ParseShadowResponse(consultations["shadow"]);
    var ambition = ParseAmbitionResponse(consultations["ambition"]);
    var omega = ParseOmegaResponse(consultations["omega"]);

    DisplayInTerminal(hero, shadow, ambition, omega);
}
```

### Tool Calling Pattern

```csharp
// Enable Dreamweavers to query game state
chatNode.Call("add_tool",
    new Callable(this, nameof(CheckInventory)),
    "Check player's inventory",
    new Dictionary());

private string CheckInventory()
{
    return JsonSerializer.Serialize(_gameState.GetInventory());
}
```

### Hybrid Mode Pattern

```csharp
private void ConsultDreamweavers(string situation)
{
    if (_dreamweaverSystem != null && _useDynamicNarrative)
    {
        // LLM-powered dynamic narrative
        _dreamweaverSystem.ConsultAllDreamweavers(situation);
    }
    else
    {
        // Fallback to static JSON narrative
        LoadStaticNarrative();
    }
}
```

## Benefits

1. **Dynamic Narrative**: Emergent storytelling, not scripted
2. **Replayability**: Different responses each playthrough
3. **Player Agency**: Natural language input, not just A/B/C choices
4. **Offline**: No internet, no data collection, no cloud fees
5. **Modding**: Players can use custom models/personas

## Challenges & Mitigations

| Challenge | Mitigation |
|-----------|------------|
| File size (2.5GB+ model) | Offer optional download, multiple model sizes |
| Response unpredictability | Strict JSON grammar, fallback on parse errors |
| Hardware requirements | Provide quality presets, fallback to CPU |
| Testing complexity | Extensive prompt testing, log all failures |
| Platform support | Test on Windows/Linux/macOS, skip WebGL |

## Success Metrics

- **Response Quality**: >80% coherent and on-theme
- **Performance**: <3s from query to first token (mid-range GPU)
- **Player Engagement**: Increased session time vs static narrative
- **Replayability**: Different narrative branches on 3+ playthroughs
- **Stability**: <1% crash rate from LLM integration

## Status

- **Current**: Research and planning complete
- **Next**: Defer implementation until core game systems stable
- **Priority**: Medium (enhancement, not blocker)
- **Timeline**: 4 weeks for full implementation when ready

## Notes

- Integration can be done incrementally (one Dreamweaver at a time)
- Hybrid mode allows testing without breaking existing narrative
- Tool calling enables deep game system integration
- Embedding system enables semantic input matching for intuitive UX
- This is a major enhancement that transforms static narrative into dynamic AI storytelling
