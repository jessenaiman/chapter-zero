# NobodyWho Integration Quick Reference

## Overview

Integration plan for adding local LLM-powered Dreamweaver personas to Omega Spiral using the [nobodywho](https://github.com/nobodywho-ooo/nobodywho) framework.

## Quick Links

- **Full ADR**: [adr-0003-nobodywho-llm-integration.md](../adr/adr-0003-nobodywho-llm-integration.md)
- **Serena Memory**: `nobodywho-llm-integration-plan.md`
- **NobodyWho Docs**: https://nobodywho-ooo.github.io/nobodywho/
- **NobodyWho GitHub**: https://github.com/nobodywho-ooo/nobodywho

## System Architecture

### Four AI Personas

1. **Hero Dreamweaver** - Courage & Honor
2. **Shadow Dreamweaver** - Mystery & Power
3. **Ambition Dreamweaver** - Achievement & Legacy
4. **Omega System** - Meta-Narrator (synthesizes all three)

### Node Structure

```
DreamweaverCore (Node)
├── SharedModel (NobodyWhoModel) - loads GGUF model once
├── HeroChat (NobodyWhoChat) - Hero persona
├── ShadowChat (NobodyWhoChat) - Shadow persona
├── AmbitionChat (NobodyWhoChat) - Ambition persona
└── OmegaChat (NobodyWhoChat) - Omega narrator
```

### Key Files to Create

- `Source/Scripts/DreamweaverSystem.cs` - Orchestrates all consultations
- `Source/Scripts/DreamweaverPersona.cs` - C# wrapper for chat nodes
- `Source/Scenes/DreamweaverCore.tscn` - Scene with 4 chat nodes + 1 model
- Enhanced: `Source/Scripts/NarrativeTerminal.cs` - Display integration

## Installation Steps

### 1. Download Plugin

```bash
cd /home/adam/Dev/omega-spiral/chapter-zero
mkdir -p addons
cd addons
# Download from: https://github.com/nobodywho-ooo/nobodywho/releases
```

### 2. Download Model

Recommended: **Qwen3-4B-Q4_K_M.gguf** (~2.5GB)

```bash
mkdir -p Source/Data/models
wget -P Source/Data/models/ \
  https://huggingface.co/Qwen/Qwen3-4B-GGUF/resolve/main/Qwen3-4B-Q4_K_M.gguf
```

### 3. Enable in Godot

1. Project → Project Settings → Plugins
2. Enable "NobodyWho"
3. Restart editor

## Response Formats

### Hero Dreamweaver

```json
{
  "advice": "Direct, encouraging guidance",
  "challenge": "A call to action or trial",
  "moral": "The ethical principle at stake"
}
```

### Shadow Dreamweaver

```json
{
  "whisper": "A secret or hidden truth",
  "secret": "A method or shortcut available",
  "cost": "What must be sacrificed or risked"
}
```

### Ambition Dreamweaver

```json
{
  "strategy": "Tactical approach to victory",
  "goal": "Specific achievement to pursue",
  "reward": "What success will bring"
}
```

### Omega System

```json
{
  "narration": "The overarching story being told",
  "choice_context": "How this moment fits the larger journey",
  "consequence": "What hangs in the balance"
}
```

## Usage Example

```csharp
// In NarrativeTerminal.cs or scene controller
_dreamweaverSystem = GetNode<DreamweaverSystem>("/root/DreamweaverSystem");
_dreamweaverSystem.AllDreamweaversResponded += OnConsultation;

// Trigger consultation
_dreamweaverSystem.ConsultAllDreamweavers("The player faces a moral dilemma...");

// Handle responses
private void OnConsultation(Godot.Collections.Dictionary consultations)
{
    string heroJson = consultations["hero"].AsString();
    string shadowJson = consultations["shadow"].AsString();
    string ambitionJson = consultations["ambition"].AsString();
    string omegaJson = consultations["omega"].AsString();

    // Parse and display
    DisplayDreamweaverGuidance(heroJson, shadowJson, ambitionJson, omegaJson);
}
```

## Key Features

### Structured Output

- Enforces JSON format via grammar
- Guarantees parseable responses
- Fallback on parse errors

### Tool Calling

```csharp
// Let Dreamweavers query game state
chatNode.Call("add_tool",
    new Callable(this, nameof(CheckInventory)),
    "Check player's inventory",
    new Dictionary());
```

### Embeddings

```gdscript
# Match player input to game actions semantically
var similarity = cosine_similarity(player_input_embedding, action_embedding)
if similarity > 0.7:
    trigger_action()
```

### Hybrid Mode

```csharp
// LLM + static JSON fallback
if (_useDynamicNarrative && _dreamweaverSystem != null)
{
    _dreamweaverSystem.ConsultAllDreamweavers(situation);
}
else
{
    LoadStaticNarrativeFromJSON();
}
```

## Performance Settings

### Sampler Configuration

```csharp
var sampler = new NobodyWhoSampler();
sampler.Temperature = 0.7f;  // Creativity (0.0-2.0)
sampler.TopK = 40;           // Token selection pool
sampler.TopP = 0.9f;         // Cumulative probability
sampler.MinP = 0.05f;        // Minimum probability threshold
```

### Context Length

- **Dreamweavers**: 4096 tokens
- **Omega**: 8192 tokens (needs more context)

### GPU Acceleration

```gdscript
# In NobodyWhoModel node
use_gpu_if_available = true
```

## Timeline

- **Week 1**: Setup, installation, basic integration
- **Week 2**: Core persona implementation, C# wrappers
- **Week 3**: Advanced features (tools, embeddings)
- **Week 4**: Testing, optimization, documentation

## Success Criteria

- [ ] Response quality >80% coherent and on-theme
- [ ] Performance <3s from query to first token
- [ ] All three Dreamweavers + Omega respond successfully
- [ ] JSON parsing works 100% of the time (grammar enforcement)
- [ ] Hybrid mode fallback functions correctly
- [ ] No memory leaks after extended play sessions

## Common Pitfalls

1. **Model Path Issues**: Use `res://` or absolute paths
2. **GPU Not Detected**: Check Vulkan/Metal drivers
3. **Slow Responses**: Reduce context length or use smaller model
4. **Invalid JSON**: Enable grammar enforcement in sampler
5. **Memory Issues**: Call `reset_context()` periodically

## Troubleshooting

### Model Won't Load

```bash
# Check model file exists
ls -lh Source/Data/models/qwen3-4b.gguf

# Check Godot can access it
# In Godot console, verify path resolution
```

### Slow Inference

1. Enable GPU: `use_gpu_if_available = true`
2. Use smaller model: Qwen3-0.6B instead of 4B
3. Reduce context: 2048 instead of 4096
4. Optimize sampler: Lower temperature, smaller TopK

### Invalid Responses

1. Check system prompt formatting
2. Enable grammar enforcement
3. Add response validation/fallback
4. Log all failures for analysis

## Resources

### Models to Try

- **Qwen3-0.6B-Q6_K_XL.gguf** (~600MB) - Fastest, lowest quality
- **Qwen3-4B-Q4_K_M.gguf** (~2.5GB) - Recommended balance
- **Qwen3-14B-Q4_K_M.gguf** (~8GB) - Best quality, slowest

### Documentation

- [Getting Started Guide](https://nobodywho-ooo.github.io/nobodywho/getting-started/)
- [Simple Chat Tutorial](https://nobodywho-ooo.github.io/nobodywho/chat/simple-chat/)
- [Structured Output Guide](https://nobodywho-ooo.github.io/nobodywho/chat/structured-output/)
- [Tool Calling Examples](https://nobodywho-ooo.github.io/nobodywho/chat/tools/)

### Community

- GitHub Issues: https://github.com/nobodywho-ooo/nobodywho/issues
- Examples: https://github.com/nobodywho-ooo/nobodywho/tree/main/examples

## Status

**Current**: Research and planning complete
**Next**: Deferred until core game systems stable
**Priority**: Medium (enhancement, not blocker)

---

*Last Updated: October 9, 2025*
*For full implementation details, see ADR-0003*
