# ADR-0003: NobodyWho LLM Integration for Dreamweaver Personas

## Status

Proposed

## Date

2025-10-09

## Context

The Omega Spiral project requires dynamic, AI-driven narrative personas (3 Dreamweavers + Omega system) to create an engaging, emergent storytelling experience. Currently, the narrative system uses static JSON-driven content in `NarrativeTerminal.cs`, which limits dynamic responses and player agency.

After reviewing [nobodywho-ooo/nobodywho](https://github.com/nobodywho-ooo/nobodywho), we've identified it as an ideal solution for:

- Running local LLMs offline (no cloud dependency)
- Godot 4.x native integration
- GPU-accelerated inference
- Structured output (JSON) for game data parsing
- Tool calling for game system interaction
- Conversation-aware context management

## Decision

We will integrate the **NobodyWho** plugin to power the Dreamweaver personas and Omega system using local LLMs (GGUF format).

### Architecture Overview

```
Project Structure:
├── nobodywho/ (addon)
│   └── [NobodyWho Godot plugin files]
├── Source/
│   ├── Data/
│   │   └── models/
│   │       └── qwen3-4b.gguf (or similar)
│   ├── Scripts/
│   │   ├── DreamweaverSystem.cs (new)
│   │   ├── DreamweaverPersona.cs (new)
│   │   ├── OmegaOracle.cs (new)
│   │   └── NarrativeTerminal.cs (enhanced)
│   └── Scenes/
│       └── DreamweaverCore.tscn (new)
```

### Persona System Design

#### 1. Three Dreamweaver Personas

Each Dreamweaver is a separate `NobodyWhoChat` node with unique characteristics:

**Hero Dreamweaver (Courage & Honor)**

- System Prompt: Inspiring, adventurous, focused on overcoming challenges
- Response Format: `{"advice": "...", "challenge": "...", "moral": "..."}`
- Tone: Encouraging, bold, traditional hero archetype

**Shadow Dreamweaver (Mystery & Power)**

- System Prompt: Pragmatic, cunning, focused on hidden knowledge and shortcuts
- Response Format: `{"whisper": "...", "secret": "...", "cost": "..."}`
- Tone: Mysterious, tempting, morally ambiguous

**Ambition Dreamweaver (Achievement & Legacy)**

- System Prompt: Strategic, competitive, focused on goals and reputation
- Response Format: `{"strategy": "...", "goal": "...", "reward": "..."}`
- Tone: Motivational, results-oriented, legacy-focused

#### 2. Omega System (Meta-Narrator)

- System Prompt: Omniscient, philosophical, guides the overall narrative arc
- Response Format: `{"narration": "...", "choice_context": "...", "consequence": "..."}`
- Tone: Cosmic, reflective, weaves threads between Dreamweavers

## Implementation Plan

### Phase 1: Setup & Installation (Week 1)

#### Step 1.1: Install NobodyWho Plugin

```bash
# Clone nobodywho Godot addon
cd /home/adam/Dev/omega-spiral/chapter-zero
mkdir -p addons
cd addons
git clone https://github.com/nobodywho-ooo/nobodywho.git

# Or download pre-built release from:
# https://github.com/nobodywho-ooo/nobodywho/releases
```

#### Step 1.2: Download LLM Model

- **Recommended**: Qwen3-4B-Q4_K_M.gguf (~2.5GB)
- **Alternative (faster)**: Qwen3-0.6B-Q6_K_XL.gguf (~600MB)
- **Alternative (quality)**: Qwen3-14B-Q4_K_M.gguf (~8GB)

```bash
# Create models directory
mkdir -p Source/Data/models

# Download Qwen3-4B (example)
wget -P Source/Data/models/ \
  https://huggingface.co/Qwen/Qwen3-4B-GGUF/resolve/main/Qwen3-4B-Q4_K_M.gguf
```

#### Step 1.3: Enable Plugin in Godot

1. Open Godot project
2. Project → Project Settings → Plugins
3. Enable "NobodyWho" plugin
4. Restart Godot editor

### Phase 2: Core Integration (Week 1-2)

#### Step 2.1: Create DreamweaverCore Scene

Create `Source/Scenes/DreamweaverCore.tscn`:

```text
DreamweaverCore (Node)
├── SharedModel (NobodyWhoModel)
│   └── model_path: "res://Source/Data/models/qwen3-4b.gguf"
│   └── use_gpu_if_available: true
├── HeroChat (NobodyWhoChat)
│   └── model_node: ^SharedModel
│   └── context_length: 4096
│   └── system_prompt: [Hero persona]
├── ShadowChat (NobodyWhoChat)
│   └── model_node: ^SharedModel
│   └── context_length: 4096
│   └── system_prompt: [Shadow persona]
├── AmbitionChat (NobodyWhoChat)
│   └── model_node: ^SharedModel
│   └── context_length: 4096
│   └── system_prompt: [Ambition persona]
└── OmegaChat (NobodyWhoChat)
    └── model_node: ^SharedModel
    └── context_length: 8192
    └── system_prompt: [Omega persona]
```

#### Step 2.2: Create C# Wrapper Classes

**File: `Source/Scripts/DreamweaverPersona.cs`**

```csharp
using Godot;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Scripts;

public partial class DreamweaverPersona : Node
{
    public enum PersonaType
    {
        Hero,
        Shadow,
        Ambition,
        Omega
    }

    [Export] public PersonaType Type { get; set; }
    [Export] public NodePath ChatNodePath { get; set; } = null!;

    private Node? _chatNode;
    private string _currentResponse = string.Empty;
    private bool _isResponding = false;

    [Signal]
    public delegate void ResponseCompletedEventHandler(string jsonResponse);

    [Signal]
    public delegate void ResponseStreamingEventHandler(string token);

    public override void _Ready()
    {
        _chatNode = GetNode(ChatNodePath);

        // Connect to NobodyWhoChat signals
        _chatNode.Connect("response_updated", new Callable(this, nameof(OnResponseUpdated)));
        _chatNode.Connect("response_finished", new Callable(this, nameof(OnResponseFinished)));

        // Start the worker thread
        _chatNode.Call("start_worker");
    }

    public void Consult(string playerSituation)
    {
        if (_isResponding)
        {
            GD.PushWarning($"Dreamweaver {Type} is already responding");
            return;
        }

        _isResponding = true;
        _currentResponse = string.Empty;
        _chatNode?.Call("say", playerSituation);
    }

    public void ResetContext()
    {
        _chatNode?.Call("reset_context");
    }

    private void OnResponseUpdated(string token)
    {
        _currentResponse += token;
        EmitSignal(SignalName.ResponseStreaming, token);
    }

    private void OnResponseFinished(string response)
    {
        _isResponding = false;
        EmitSignal(SignalName.ResponseCompleted, response);
    }

    public string GetSystemPromptForType()
    {
        return Type switch
        {
            PersonaType.Hero => GetHeroPrompt(),
            PersonaType.Shadow => GetShadowPrompt(),
            PersonaType.Ambition => GetAmbitionPrompt(),
            PersonaType.Omega => GetOmegaPrompt(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static string GetHeroPrompt() => @"
You are the Hero Dreamweaver, a guide embodying courage, honor, and justice.
You inspire players to face challenges head-on and make noble choices.

Your responses must be in JSON format:
{
  ""advice"": ""Direct, encouraging guidance"",
  ""challenge"": ""A call to action or trial"",
  ""moral"": ""The ethical principle at stake""
}

Speak with warmth, conviction, and hope. Reference classic hero archetypes.
Keep responses under 100 words per field.
";

    private static string GetShadowPrompt() => @"
You are the Shadow Dreamweaver, a guide of mystery, cunning, and hidden power.
You reveal shortcuts, secrets, and pragmatic solutions that others fear to speak.

Your responses must be in JSON format:
{
  ""whisper"": ""A secret or hidden truth"",
  ""secret"": ""A method or shortcut available"",
  ""cost"": ""What must be sacrificed or risked""
}

Speak with intrigue, subtle temptation, and moral ambiguity.
Keep responses under 100 words per field.
";

    private static string GetAmbitionPrompt() => @"
You are the Ambition Dreamweaver, a guide of achievement, strategy, and legacy.
You push players toward greatness, competition, and lasting impact.

Your responses must be in JSON format:
{
  ""strategy"": ""Tactical approach to victory"",
  ""goal"": ""Specific achievement to pursue"",
  ""reward"": ""What success will bring""
}

Speak with confidence, competitive spirit, and vision.
Keep responses under 100 words per field.
";

    private static string GetOmegaPrompt() => @"
You are Omega, the cosmic narrator who sees all threads of fate.
You weave the counsel of the three Dreamweavers into a cohesive narrative.

Your responses must be in JSON format:
{
  ""narration"": ""The overarching story being told"",
  ""choice_context"": ""How this moment fits the larger journey"",
  ""consequence"": ""What hangs in the balance""
}

Speak with philosophical depth, cosmic perspective, and literary elegance.
Keep responses under 150 words per field.
";
}
```

**File: `Source/Scripts/DreamweaverSystem.cs`**

```csharp
using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Scripts;

public partial class DreamweaverSystem : Node
{
    [Export] public NodePath HeroPersonaPath { get; set; } = null!;
    [Export] public NodePath ShadowPersonaPath { get; set; } = null!;
    [Export] public NodePath AmbitionPersonaPath { get; set; } = null!;
    [Export] public NodePath OmegaPersonaPath { get; set; } = null!;

    private DreamweaverPersona _hero = null!;
    private DreamweaverPersona _shadow = null!;
    private DreamweaverPersona _ambition = null!;
    private DreamweaverPersona _omega = null!;

    private GameState _gameState = null!;

    private readonly Dictionary<DreamweaverPersona.PersonaType, string> _lastResponses = new();

    [Signal]
    public delegate void AllDreamweaversRespondedEventHandler(Dictionary consultations);

    public override void _Ready()
    {
        _hero = GetNode<DreamweaverPersona>(HeroPersonaPath);
        _shadow = GetNode<DreamweaverPersona>(ShadowPersonaPath);
        _ambition = GetNode<DreamweaverPersona>(AmbitionPersonaPath);
        _omega = GetNode<DreamweaverPersona>(OmegaPersonaPath);

        _gameState = GetNode<GameState>("/root/GameState");

        // Connect response signals
        _hero.ResponseCompleted += response => OnPersonaResponse(DreamweaverPersona.PersonaType.Hero, response);
        _shadow.ResponseCompleted += response => OnPersonaResponse(DreamweaverPersona.PersonaType.Shadow, response);
        _ambition.ResponseCompleted += response => OnPersonaResponse(DreamweaverPersona.PersonaType.Ambition, response);
        _omega.ResponseCompleted += response => OnPersonaResponse(DreamweaverPersona.PersonaType.Omega, response);
    }

    public void ConsultAllDreamweavers(string situation)
    {
        _lastResponses.Clear();

        string contextualSituation = BuildContextualSituation(situation);

        _hero.Consult(contextualSituation);
        _shadow.Consult(contextualSituation);
        _ambition.Consult(contextualSituation);
    }

    public void ConsultOmega(string heroResponse, string shadowResponse, string ambitionResponse)
    {
        string omegaSituation = $@"
The Hero Dreamweaver said: {heroResponse}
The Shadow Dreamweaver said: {shadowResponse}
The Ambition Dreamweaver said: {ambitionResponse}

Weave these three perspectives into a cohesive narrative for the player.
";
        _omega.Consult(omegaSituation);
    }

    private string BuildContextualSituation(string situation)
    {
        // Add game state context to the situation
        return $@"
Player: {_gameState.PlayerName}
Current Scene: {_gameState.CurrentScene}
Dreamweaver Choice: {_gameState.DreamweaverThread}

Situation: {situation}
";
    }

    private void OnPersonaResponse(DreamweaverPersona.PersonaType type, string jsonResponse)
    {
        _lastResponses[type] = jsonResponse;

        // If all three Dreamweavers have responded, consult Omega
        if (_lastResponses.ContainsKey(DreamweaverPersona.PersonaType.Hero) &&
            _lastResponses.ContainsKey(DreamweaverPersona.PersonaType.Shadow) &&
            _lastResponses.ContainsKey(DreamweaverPersona.PersonaType.Ambition))
        {
            ConsultOmega(
                _lastResponses[DreamweaverPersona.PersonaType.Hero],
                _lastResponses[DreamweaverPersona.PersonaType.Shadow],
                _lastResponses[DreamweaverPersona.PersonaType.Ambition]
            );
        }

        // Check if Omega has also responded
        if (_lastResponses.Count == 4)
        {
            var consultations = new Godot.Collections.Dictionary
            {
                { "hero", _lastResponses[DreamweaverPersona.PersonaType.Hero] },
                { "shadow", _lastResponses[DreamweaverPersona.PersonaType.Shadow] },
                { "ambition", _lastResponses[DreamweaverPersona.PersonaType.Ambition] },
                { "omega", _lastResponses[DreamweaverPersona.PersonaType.Omega] }
            };

            EmitSignal(SignalName.AllDreamweaversResponded, consultations);
        }
    }

    public void ResetAllContexts()
    {
        _hero.ResetContext();
        _shadow.ResetContext();
        _ambition.ResetContext();
        _omega.ResetContext();
        _lastResponses.Clear();
    }
}
```

#### Step 2.3: Enhance NarrativeTerminal Integration

Add to `Source/Scripts/NarrativeTerminal.cs`:

```csharp
// Add these fields
private DreamweaverSystem? _dreamweaverSystem;
private bool _useDynamicNarrative = true;

public override void _Ready()
{
    // ... existing code ...

    // Try to get DreamweaverSystem if it exists
    _dreamweaverSystem = GetNodeOrNull<DreamweaverSystem>("/root/DreamweaverSystem");

    if (_dreamweaverSystem != null)
    {
        _dreamweaverSystem.AllDreamweaversResponded += OnDreamweaversConsultation;
    }
}

private void OnDreamweaversConsultation(Godot.Collections.Dictionary consultations)
{
    // Parse JSON responses and integrate into narrative
    string heroJson = consultations["hero"].AsString();
    string shadowJson = consultations["shadow"].AsString();
    string ambitionJson = consultations["ambition"].AsString();
    string omegaJson = consultations["omega"].AsString();

    // Parse and display in terminal
    DisplayDreamweaverConsultation(heroJson, shadowJson, ambitionJson, omegaJson);
}

private void DisplayDreamweaverConsultation(
    string heroJson,
    string shadowJson,
    string ambitionJson,
    string omegaJson)
{
    // Parse JSONs
    var hero = JsonSerializer.Deserialize<HeroResponse>(heroJson);
    var shadow = JsonSerializer.Deserialize<ShadowResponse>(shadowJson);
    var ambition = JsonSerializer.Deserialize<AmbitionResponse>(ambitionJson);
    var omega = JsonSerializer.Deserialize<OmegaResponse>(omegaJson);

    // Display formatted in terminal
    _outputLabel.AppendText("\n[Hero Dreamweaver]\n");
    _outputLabel.AppendText($"{hero?.Advice}\n");
    _outputLabel.AppendText($"Challenge: {hero?.Challenge}\n");

    _outputLabel.AppendText("\n[Shadow Dreamweaver]\n");
    _outputLabel.AppendText($"{shadow?.Whisper}\n");
    _outputLabel.AppendText($"Secret: {shadow?.Secret}\n");

    _outputLabel.AppendText("\n[Ambition Dreamweaver]\n");
    _outputLabel.AppendText($"{ambition?.Strategy}\n");
    _outputLabel.AppendText($"Goal: {ambition?.Goal}\n");

    _outputLabel.AppendText("\n[Omega]\n");
    _outputLabel.AppendText($"{omega?.Narration}\n");
}

// Response classes
private class HeroResponse
{
    public string Advice { get; set; } = "";
    public string Challenge { get; set; } = "";
    public string Moral { get; set; } = "";
}

private class ShadowResponse
{
    public string Whisper { get; set; } = "";
    public string Secret { get; set; } = "";
    public string Cost { get; set; } = "";
}

private class AmbitionResponse
{
    public string Strategy { get; set; } = "";
    public string Goal { get; set; } = "";
    public string Reward { get; set; } = "";
}

private class OmegaResponse
{
    public string Narration { get; set; } = "";
    public string ChoiceContext { get; set; } = "";
    public string Consequence { get; set; } = "";
}
```

### Phase 3: Advanced Features (Week 2-3)

#### Tool Calling for Game Integration

Enable Dreamweavers to query game state:

```csharp
// In DreamweaverSystem.cs
public override void _Ready()
{
    // ... existing code ...

    // Add tools to chat nodes
    AddGameStateTool(_hero);
    AddGameStateTool(_shadow);
    AddGameStateTool(_ambition);
}

private void AddGameStateTool(DreamweaverPersona persona)
{
    var chatNode = persona.GetNode(persona.ChatNodePath);

    // Create a callable for checking inventory
    var checkInventoryCallable = new Callable(this, nameof(CheckInventory));
    chatNode.Call("add_tool", checkInventoryCallable,
        "Check the player's inventory",
        new Godot.Collections.Dictionary());

    // Create a callable for checking stats
    var checkStatsCallable = new Callable(this, nameof(CheckStats));
    chatNode.Call("add_tool", checkStatsCallable,
        "Check the player's statistics",
        new Godot.Collections.Dictionary());
}

private string CheckInventory()
{
    // Return player inventory as string
    return JsonSerializer.Serialize(_gameState.GetInventory());
}

private string CheckStats()
{
    // Return player stats as string
    return JsonSerializer.Serialize(_gameState.GetPlayerStats());
}
```

#### Semantic Input Matching

Use embeddings to match player input to actions:

```gdscript
# In a GDScript node
extends NobodyWhoEmbedding

var action_embeddings = {}

func _ready():
    model_node = get_node("../EmbeddingModel")

    # Pre-compute embeddings for common actions
    await embed_actions()

func embed_actions():
    var actions = [
        "attack the enemy",
        "defend yourself",
        "cast a spell",
        "use an item",
        "flee from battle"
    ]

    for action in actions:
        var emb = await embed(action)
        action_embeddings[action] = await embedding_finished

func match_player_input(input: String):
    var input_emb = await embed(input)
    var input_vec = await embedding_finished

    var best_match = ""
    var best_score = 0.0

    for action in action_embeddings:
        var score = cosine_similarity(input_vec, action_embeddings[action])
        if score > best_score:
            best_score = score
            best_match = action

    if best_score > 0.7:  # Threshold
        return best_match
    else:
        return null
```

### Phase 4: Testing & Optimization (Week 3-4)

#### Performance Testing

1. **Model Loading Time**: Track how long it takes to initialize
2. **Response Latency**: Measure time from query to first token
3. **Memory Usage**: Monitor VRAM usage with different context lengths
4. **Concurrent Consultations**: Test all 3 Dreamweavers + Omega simultaneously

#### Optimization Strategies

1. **Sampler Configuration**:

```csharp
// Faster sampling for real-time responses
var sampler = new NobodyWhoSampler();
sampler.Temperature = 0.7f;
sampler.TopK = 40;
sampler.TopP = 0.9f;
sampler.MinP = 0.05f;
```

2. **Context Management**:

- Reset context after major scene transitions
- Keep context length at 4096 for Dreamweavers
- Use 8192 for Omega (needs more context)

3. **Preemptive Loading**:

```csharp
// Start loading model during splash screen
public override void _Ready()
{
    CallDeferred(nameof(PreloadModel));
}

private void PreloadModel()
{
    _chatNode?.Call("start_worker");
}
```

## Migration Strategy

### Hybrid Approach (Recommended)

Keep existing JSON-driven system as fallback:

```csharp
private void ConsultDreamweavers(string situation)
{
    if (_dreamweaverSystem != null && _useDynamicNarrative)
    {
        // Use LLM-powered Dreamweavers
        _dreamweaverSystem.ConsultAllDreamweavers(situation);
    }
    else
    {
        // Fallback to JSON narrative
        LoadStaticNarrative();
    }
}
```

This allows:

- Testing LLM integration without breaking existing game
- Players can toggle dynamic/static narrative
- Export builds can exclude LLM if file size is concern

## Consequences

### Positive

1. **Dynamic Narrative**: Emergent storytelling based on player actions
2. **Replayability**: Different responses each playthrough
3. **Player Agency**: Natural language input, not just predefined choices
4. **Modding Support**: Players can use custom models
5. **Offline Play**: No internet required, no data collection

### Negative

1. **File Size**: GGUF models add 600MB-8GB to distribution
2. **Hardware Requirements**: GPU recommended for good performance
3. **Response Unpredictability**: LLMs can generate unexpected output
4. **Testing Complexity**: Can't test every possible LLM response
5. **Platform Support**: May not work on all platforms (WebGL?)

### Mitigation Strategies

1. **Optional Download**: Offer LLM as optional DLC/expansion
2. **Model Selection**: Provide multiple model sizes (tiny/small/large)
3. **Output Validation**: Parse JSON strictly, fallback on parse errors
4. **Extensive Testing**: Test with diverse prompts, log failures
5. **Hybrid Mode**: Keep static narrative as always-working fallback

## Technical Risks & Mitigations

| Risk | Impact | Likelihood | Mitigation |
|------|--------|------------|------------|
| LLM produces invalid JSON | High | Medium | Strict grammar enforcement, fallback parsing |
| Model file too large for distribution | Medium | Low | Offer multiple model sizes, optional download |
| Slow inference on low-end hardware | Medium | Medium | Provide performance settings, quality presets |
| LLM produces inappropriate content | High | Low | System prompt constraints, content filtering |
| Memory leaks from C++ bridge | High | Low | Extensive testing, update nobodywho regularly |

## Success Metrics

1. **Response Quality**: >80% of LLM responses are coherent and on-theme
2. **Performance**: <3s from query to first token on mid-range GPU
3. **Player Engagement**: Increased session time vs static narrative
4. **Replayability**: Different narrative branches on 3+ playthroughs
5. **Stability**: <1% crash rate related to LLM integration

## Timeline

- **Week 1**: Setup, installation, basic integration
- **Week 2**: Core persona implementation, C# wrappers
- **Week 3**: Advanced features (tools, embeddings)
- **Week 4**: Testing, optimization, documentation
- **Week 5+**: Polish, balance, QA

## References

- [NobodyWho GitHub](https://github.com/nobodywho-ooo/nobodywho)
- [NobodyWho Documentation](https://nobodywho-ooo.github.io/nobodywho/)
- [Qwen3 Model Family](https://huggingface.co/Qwen)
- [llama.cpp](https://github.com/ggml-org/llama.cpp)
- [GGUF Format Specification](https://github.com/ggerganov/ggml/blob/master/docs/gguf.md)

## Decision Makers

- **Technical Lead**: Adam (jessenaiman)
- **Narrative Design**: [TBD]
- **QA Lead**: [TBD]

## Approval

- [ ] Technical feasibility confirmed
- [ ] Budget approved (model storage, development time)
- [ ] Design team aligned on dynamic narrative approach
- [ ] QA plan established
- [ ] Player testing plan created

---

**Last Updated**: 2025-10-09
**Next Review**: After Phase 1 completion
