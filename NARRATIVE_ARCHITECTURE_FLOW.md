# Narrative Architecture Analysis & Stage 1 Execution Flow

## Architecture Overview

The **Narrative namespace** is responsible for:
1. **Loading & Parsing unknown YAML files** into strongly-typed `NarrativeScriptElement` objects
2. **Generic scene sequencing** through the `NarrativeEngine`
3. **UI abstraction** via `INarrativeHandler` interface
4. **Stage independence** - any stage can use this pattern

---

## Component Responsibilities

### 1. **NarrativeScriptLoader** (Entry Point)
**Responsibility:** Read ANY unknown YAML file and deserialize it

```csharp
static NarrativeScript LoadYamlScript(string yamlFilePath)
  ↓
  Opens file at path (e.g., "res://source/stages/stage_1_ghost/ghost.yaml")
  ↓
  YamlDotNet deserializer (CamelCaseNamingConvention)
  ↓
  Returns NarrativeScript object (completely generic)
  ↓
  Never knows what stage it is, doesn't care
```

**Why it works for unknown YAML:**
- Uses `YamlDotNet` library with `CamelCaseNamingConvention` for flexible property mapping
- `IgnoreUnmatchedProperties()` - silently ignores extra YAML keys (future-proof)
- Maps to concrete `NarrativeScript` and `NarrativeScene` classes
- `ChoiceOption` as base with `Owner` property for scoring

---

### 2. **NarrativeScript** (Data Schema)
**Responsibility:** Define universal YAML structure

```yaml
title: Ghost Terminal              # Metadata
speaker: Omega
description: ...

scenes:                           # Array of scenes
  - lines: [...]                  # Lines to display
    question: "?"                 # Optional question
    owner: omega                  # Speaker
    answers:                       # Optional choices
      - owner: light
        text: "Choice"
      - owner: shadow
        text: "Choice"
    pause: 2.0                     # Optional timing
```

**Universal Properties:**
- `title`, `speaker`, `description` - metadata
- `scenes[]` - flat array (not nested)
- Each scene has `lines`, optional `question`, optional `answers`
- `Owner` on answers = Dreamweaver thread (for auto-scoring)

---

### 3. **CinematicDirector<TPlan>** (Stage Pattern)
**Responsibility:** Bridge loader → stage-specific plan with caching

```csharp
GhostDataLoader : CinematicDirector<GhostTerminalCinematicPlan>
  ├─ GetDataPath() → "res://source/stages/stage_1_ghost/ghost.yaml"
  ├─ BuildPlan(NarrativeScript script) → GhostTerminalCinematicPlan
  └─ GetPlan() → cached plan
```

**Why this pattern:**
- Each stage inherits from `CinematicDirector<TPlan>`
- Lazy-loads YAML on first access
- Caches result (thread-safe)
- Stage can wrap/enhance script in custom plan type

---

### 4. **NarrativeEngine** (Orchestrator)
**Responsibility:** Sequence scenes generically, delegate UI work

```csharp
engine.PlayAsync(NarrativeScript script, INarrativeHandler handler)
  ├─ handler.PlayBootSequenceAsync()
  ├─ foreach scene in script.Scenes:
  │   ├─ Filter command lines ([GLITCH], [FADE_TO_STABLE])
  │   ├─ handler.DisplayLinesAsync(displayLines)
  │   ├─ handler.ApplySceneEffectsAsync(scene)
  │   ├─ if scene.Question:
  │   │   ├─ selected = handler.PresentChoiceAsync(question, choices)
  │   │   └─ handler.ProcessChoiceAsync(selected)
  │   └─ if scene.Pause:
  │       └─ Task.Delay(pause)
  └─ handler.NotifySequenceCompleteAsync()
```

**Why it's universal:**
- Doesn't know what stage it is
- Only knows `NarrativeScript` shape (which all stages share)
- All UI work delegated to handler
- Timing, effects, choice handling all configurable per-stage

---

### 5. **INarrativeHandler** (UI Contract)
**Responsibility:** Implement stage-specific presentation

```csharp
public interface INarrativeHandler
{
  Task PlayBootSequenceAsync()          // CRT boot effect
  Task DisplayLinesAsync(lines)          // Type text to terminal
  Task<bool> HandleCommandLineAsync()    // [GLITCH] → true if handled
  Task ApplySceneEffectsAsync(scene)     // Shader effects
  Task<ChoiceOption> PresentChoiceAsync()  // Show buttons, return choice
  Task ProcessChoiceAsync(selected)      // Update scores
  Task NotifySequenceCompleteAsync()     // Cleanup
}
```

**GhostUi implements this:**
- `DisplayLinesAsync()` → types to Terminal via `write()`
- `HandleCommandLineAsync()` → detects [GLITCH] / [FADE_TO_STABLE]
- `ApplySceneEffectsAsync()` → applies shader effects
- `PresentChoiceAsync()` → shows choice buttons
- `ProcessChoiceAsync()` → tracks Dreamweaver scores

---

## Stage 1 Ghost Execution Flow (Detailed)

### Phase 0: Boot (Godot _Ready)

```
GameManager / StageLoader
  ↓
Instantiates GhostCinematicDirector
  ↓
GhostCinematicDirector._Ready()
  ├─ Creates GhostDataLoader
  └─ Calls ExecuteStageAsync()
```

---

### Phase 1: Load YAML

```
GhostCinematicDirector.ExecuteStageAsync()
  ↓
var plan = _DataLoader.GetPlan()
  ↓
GhostDataLoader.GetPlan()  [CinematicDirector base]
  ├─ lock(_SyncRoot)
  ├─ if (_CachedPlan != null) return cached
  ├─ NarrativeScript script = LoadYamlScript()
  │   └─ "res://source/stages/stage_1_ghost/ghost.yaml"
  │       ↓
  │       NarrativeScriptLoader.LoadYamlScript(path)
  │       ├─ FileAccess.Open(path)
  │       ├─ yamlContent = file.GetAsText()
  │       ├─ deserializer = DeserializerBuilder()
  │       │   .WithNamingConvention(CamelCaseNamingConvention)
  │       │   .IgnoreUnmatchedProperties()
  │       │   .Build()
  │       └─ script = deserializer.Deserialize<NarrativeScript>(yaml)
  │           ↓
  │           NarrativeScript object
  │           ├─ title: "Ghost Terminal"
  │           ├─ speaker: "Omega"
  │           ├─ scenes: [8 scenes]
  │           │   ├─ Scene 1: lines + question + answers
  │           │   ├─ Scene 2: lines only
  │           │   ├─ Scene 3: lines + question + answers
  │           │   ...
  │           │   └─ Scene 8: lines + [TERMINAL SHUTS DOWN]
  │           └─ (fully parsed, strongly-typed)
  │
  ├─ _CachedPlan = BuildPlan(script)
  │   └─ GhostDataLoader.BuildPlan(script)
  │       └─ return new GhostTerminalCinematicPlan(script)
  │
  └─ return _CachedPlan
      ↓
      GhostTerminalCinematicPlan(script)
```

---

### Phase 2: Load Scene

```
GhostCinematicDirector.ExecuteStageAsync()
  ↓
var scenePath = "res://source/stages/stage_1_ghost/ghost_terminal.tscn"
var packedScene = ResourceLoader.Load<PackedScene>(scenePath)
var ghostTerminal = packedScene.Instantiate<Control>()
AddChild(ghostTerminal)
  ↓
ghost_terminal.tscn loads
├─ GhostUi script attached to root Control
├─ OmegaFrame scene instance (UI frame)
├─ NarrativeViewport (MarginContainer)
├─ NarrativeStack (VBoxContainer)
│   ├─ TextDisplay (RichTextLabel) [UNUSED - kept for reference]
│   ├─ Terminal (godot_xterm terminal node)
│   └─ ChoiceContainer (VBoxContainer) [hidden, for buttons]
└─ GhostUi._Ready() called
    ├─ Terminal = GetNode("Terminal")
    ├─ ChoiceContainer = GetNode("ChoiceContainer")
    └─ EnableBootSequence → CallDeferred(StartBootSequence)
```

---

### Phase 3: Initialize UI

```
GhostUi._Ready()
  ├─ Sets Terminal reference
  ├─ Sets ChoiceContainer reference
  └─ CallDeferred(StartBootSequence)
      ↓
      GhostUi.StartBootSequence()
      └─ PlayBootSequenceAsync()
          ├─ Terminal.Call("write", "[INITIALIZING GHOST TERMINAL...]\n")
          ├─ Terminal.Call("write", "[LOADING ARCHIVES...]\n")
          ├─ Terminal.Call("write", "[SYSTEM READY]\n")
          └─ BootSequencePlayed = true
```

---

### Phase 4: Engine Plays Script

```
GhostCinematicDirector.ExecuteStageAsync()
  ↓
var engine = new NarrativeEngine()
engine.PlayAsync(plan.Script, ghostUi)
  ↓
NarrativeEngine.PlayAsync(NarrativeScript script, INarrativeHandler handler)
  ├─ await handler.PlayBootSequenceAsync()  [already done above]
  │
  ├─ foreach scene in script.Scenes:  [8 scenes total]
  │   └─ PlaySceneAsync(scene, handler)
  │       ├─ Step 1: Filter lines
  │       │   ├─ "[FADE_TO_STABLE]" → HandleCommandLineAsync() → true
  │       │   └─ displayLines = [non-command lines]
  │       │
  │       ├─ Step 2: Display lines
  │       │   └─ handler.DisplayLinesAsync(displayLines)
  │       │       └─ GhostUi.DisplayLinesAsync(lines)
  │       │           ├─ foreach line in lines:
  │       │           │   └─ TypeTextAsync(line + "\n")
  │       │           │       ├─ foreach char in line:
  │       │           │       │   ├─ Terminal.Call("write", char)
  │       │           │       │   └─ await Task.Delay(1000/DefaultTypingSpeed)
  │       │           │       └─ [text appears on screen]
  │       │           └─ await Task.Delay(120ms between lines)
  │       │
  │       ├─ Step 3: Apply effects
  │       │   └─ handler.ApplySceneEffectsAsync(scene)
  │       │       └─ GhostUi.ApplySceneEffectsAsync(scene)
  │       │           ├─ if scene.Lines contains "[GLITCH]"
  │       │           │   └─ ApplyGlitchEffect()
  │       │           └─ if scene.Lines contains "[FADE_TO_STABLE]"
  │       │               └─ FadeToStable()
  │       │
  │       ├─ Step 4: Present choices (if question exists)
  │       │   ├─ scene.Question = "Choose the story that calls to you:"
  │       │   ├─ scene.Answers = [light choice, shadow choice, ambition choice]
  │       │   └─ handler.PresentChoiceAsync(question, speaker, answers)
  │       │       └─ GhostUi.PresentChoiceAsync(question, speaker, choices)
  │       │           ├─ TypeTextAsync(question + "\n")
  │       │           ├─ Create 3 buttons from choices
  │       │           ├─ foreach choice: AddChild(button)
  │       │           ├─ ChoiceContainer.Visible = true
  │       │           ├─ await tcs.Task  [WAIT FOR PLAYER]
  │       │           │   └─ Player clicks button
  │       │           │       └─ button.Pressed → tcs.TrySetResult(choice)
  │       │           ├─ ChoiceContainer.Visible = false
  │       │           ├─ QueueFree all buttons
  │       │           └─ return selected ChoiceOption
  │       │
  │       └─ Step 5: Process choice
  │           └─ handler.ProcessChoiceAsync(selected)
  │               └─ GhostUi.ProcessChoiceAsync(selected)
  │                   ├─ Extract selected.Owner (e.g., "light")
  │                   ├─ Track score: dreamweaverScores["light"] += 2
  │                   └─ (or +1 if not current dominant thread)
  │
  └─ await handler.NotifySequenceCompleteAsync()
      └─ GhostUi.NotifySequenceCompleteAsync()
          └─ [cleanup]
```

---

### Phase 5: Scene Loop Detail (8 Scenes)

```
SCENE 1: "If you could hear only one story..."
├─ Lines: [FADE_TO_STABLE], metadata, question text
├─ Question: "Choose the story that calls to you:"
├─ Answers: [light, shadow, ambition]
├─ Player selects (e.g., light)
└─ Scores: light += 2

SCENE 2: "The spiral remembers all stories..."
├─ Lines only (narrative progression)
├─ No question
└─ No scoring

SCENE 3: "The spiral asks its first echo..."
├─ Lines + question
├─ Question: "In that story, who are you?"
├─ Player selects (e.g., shadow)
└─ Scores: shadow += 1 (not dominant thread)

SCENE 4: "Your role crystallizes..."
├─ Lines only
└─ No question

SCENE 5: "Do names define us or deceive us?"
├─ Lines + question
├─ Player selects
└─ Scores updated

SCENE 6: "[GLITCH] - ∞ ◊ Ω ≋ ※"
├─ Lines contain [GLITCH] tag
├─ TriggerGlitchEffect()
└─ Display secret symbols

SCENE 7: "One last thing - give me a name"
├─ Lines + final question
├─ Player selects (determines dominant Dreamweaver thread)
└─ Scores finalized

SCENE 8: "[SYSTEM: Dreamweaver thread selected]"
├─ Display final narrative
├─ Include {{THREAD_NAME}} substitution
├─ [TERMINAL SHUTS DOWN]
└─ [STAGE 1 COMPLETE]
```

---

### Phase 6: Completion

```
NarrativeEngine.PlayAsync() completes
  ↓
GhostCinematicDirector.ExecuteStageAsync()
  ├─ ghostTerminal.QueueFree()
  ├─ EmitStageComplete() signal
  │   └─ GameManager receives signal
  │       └─ Load Stage 2
  └─ [Stage 1 done]
```

---

## Critical Observations

### ✅ Strengths
1. **Universal YAML loader** - `NarrativeScriptLoader` reads ANY YAML matching the schema
2. **Generic engine** - `NarrativeEngine` doesn't know stage type, only `NarrativeScript` shape
3. **Clean abstraction** - `INarrativeHandler` isolates UI from narrative logic
4. **Flexible naming** - `CamelCaseNamingConvention` + `IgnoreUnmatchedProperties` = future-proof
5. **Automatic scoring** - Answer `.Owner` drives Dreamweaver point allocation

### ⚠️ Issues & Gaps

#### 1. **YAML Alias Mismatch** (CRITICAL)
```yaml
# ghost.yaml uses "choice" (line 18)
choice:
  - owner: light
    text: "..."

# But NarrativeScript expects "answers" (with proxy)
answers:
  - owner: light
    text: "..."
```

**Status:** Handled via `_ChoiceProxy` in `NarrativeScene.cs` but not tested.

#### 2. **Command Line Handling Incomplete**
```csharp
if (line.StartsWith("[") && line.EndsWith("]"))
{
    bool handled = await handler.HandleCommandLineAsync(line).ConfigureAwait(false);
    if (!handled) displayLines.Add(line);  // ← Falls back to display if not handled
}
```

**Status:** `GhostUi.HandleCommandLineAsync()` returns `false` for all commands, so they display as text.
**Should:** Return `true` when handling `[GLITCH]` or `[FADE_TO_STABLE]`.

#### 3. **No Dreamweaver Score Tracking**
`NarrativeEngine` doesn't know about Dreamweaver scoring. Each `ProcessChoiceAsync()` must implement it independently.

**Status:** Must implement in `GhostUi.ProcessChoiceAsync()`.

#### 4. **{{THREAD_NAME}} Substitution**
Scene 8 has placeholder text that needs replacing with determined thread name.

**Status:** Must implement in `GhostUi.ProcessChoiceAsync()` or after sequence complete.

---

## YAML File Format (Summary)

```yaml
title: Stage Title
speaker: Primary Speaker
description: Stage description

scenes:
  # Narrative-only scene
  - lines:
      - "Text line"
      - "[COMMAND]"
      - ""

  # Question scene
  - lines:
      - "Setup text"
    question: "The question?"
    owner: omega  # Speaker (doesn't score)
    choice:       # (maps to "answers" via proxy)
      - owner: light      # Dreamweaver thread (scores)
        text: "Choice 1"
      - owner: shadow
        text: "Choice 2"
      - owner: ambition
        text: "Choice 3"
```

---

## Next Steps for Implementation

1. **Verify YAML loads correctly** - Test `NarrativeScriptLoader` directly
2. **Wire Terminal output** - Ensure `Terminal.Call("write", text)` works
3. **Implement command handling** - `[GLITCH]` and `[FADE_TO_STABLE]` → return true
4. **Implement scoring** - Track scores in dictionary, determine dominant thread
5. **Implement thread substitution** - Replace `{{THREAD_NAME}}` in scene 8
6. **Test full flow** - Play through all 8 scenes with choices

---
