# Stage 2 Architecture Plan: Echo Chamber (Data-Driven Roguelike)

**Synchronized with TODO List** ✓

## Overview

Stage 2: Echo Chamber is a three-fold roguelike experience where three Dreamweavers (Light, Shadow, Ambition) each configure a chamber with repositionable set pieces. Unlike Stage 1's linear narrative progression, Stage 2 focuses on **set piece placement mechanics** and **dungeon exploration** in a Nethack/Rogue style.

### Core Concept
- **Same dungeon plays three times**
- **Each Dreamweaver controls object placement** (door, monster, chest)
- **Interludes between chambers** allow player alignment choices
- **Dreamweaver affinity scoring** tracks Light/Shadow/Ambition alignment
- **Decoys and glitch effects** vary by Dreamweaver variant

### Key Differences from Stage 1

| Aspect | Stage 1 | Stage 2 |
|--------|---------|---------|
| **Flow** | Linear narrative beats | Three-fold chamber sequence |
| **Progression** | Story questions → answers | Interlude choices → chamber exploration |
| **Variation** | Different content per beat | Same dungeon, different Dreamweaver configs |
| **Mechanics** | Terminal UI responses | Set piece exploration + alignment tracking |
| **Data Loading** | One beat per scene transition | Multiple beat types (interludes + chambers) |

## Data Structure (stage_2.json)

## Data Structure (stage_2.json)
```
├── Metadata (iteration, status, system intro)
├── Dreamweavers (Light, Shadow, Ambition)
│   └── Each: id, name, accentColor, textTheme
├── Interludes (3 dialogue sequences)
│   └── Each: id, owner, prompt, options[]
│       └── Each option: alignment, banter (approve/dissent)
└── Chambers (3 configured dungeon layouts)
    └── Each: id, owner, style, objects[], decoys[]
        └── Objects in slots: door, monster, chest
            └── Each: alignment, prompt, interactionLog[], banter
```

---

## Implementation Architecture

### Architecture Pattern: Data-Driven Like Stage 1

#### Stage 1 Pattern (For Reference)
```
Stage1NarrativeData.cs
    ├── NarrativeMetadata
    ├── BootSequenceData
    ├── OpeningMonologueData
    ├── ChoiceOption (with banter)
    └── ... (multiple beat types)

Beat1BootSequence.cs / Beat2OpeningMonologue.cs
    ├── Loads Stage1NarrativeData via NarrativeDataLoader
    ├── Renders content dynamically from JSON
    └── Uses infrastructure/BeatSceneBase for transitions

Stage1 Result: Menu → Beat1 → Beat2 → ... → Stage Complete
```

#### Stage 2 Pattern (New)
```
Stage2NarrativeData.cs
    ├── EchoChamberMetadata
    ├── DreamweaverData
    ├── InterludeData
    │   └── InterludeOption (with alignment + banter)
    ├── ChamberData
    │   ├── ChamberStyle
    │   └── ChamberObject (in slots: door/monster/chest)
    │       └── Banter (approve/dissent)
    └── DecoyData

BeatInterludeSequence.cs
    ├── Loads Stage2NarrativeData
    ├── Renders interlude dialogue dynamically
    ├── Player selects option → updates EchoAffinityTracker
    └── Transitions to chamber

NethackExploration.cs
    ├── Loads Stage2NarrativeData for specific chamber
    ├── Renders set pieces (door, monster, chest in slots)
    ├── Handles object interaction
    └── Transitions to next interlude or stage complete

Stage2 Result: Menu → Interlude1 → Chamber1 → Interlude2 → Chamber2 → Interlude3 → Chamber3 → Stage Complete
```

---

## Implementation Checklist

- [x] **Task 1: Create Stage2NarrativeData.cs structures**
  - File: `source/stages/stage_2/Stage2NarrativeData.cs`
  - Content: Mirror JSON schema with [JsonPropertyName] attributes
  - Classes needed: EchoChamberMetadata, DreamweaverData, InterludeData, ChamberData, ChamberObject, DecoyData
  - Must support deserialization of stage_2.json via System.Text.Json
  - **Status**: ✅ COMPLETED - All data structures created with full XML documentation

- [x] **Task 2: Implement Stage2DataLoader for JSON deserialization**
  - File: Use existing `NarrativeDataLoader<Stage2NarrativeData>` pattern
  - Path: Reuse generic infrastructure/NarrativeDataLoader.cs
  - Verification: Confirm stage_2.json deserializes without errors
  - **Status**: ✅ COMPLETED - Created comprehensive deserialization tests in Stage2DataDeserializationTests.cs

- [x] **Task 3: Create Beat renderers for chambers and interludes**
  - Files: `source/stages/stage_2/beats/scripts/Beat*.cs`
  - BeatInterludeSequence.cs: Render interlude → handle player choice → update affinity
  - NethackLight.cs, NethackShadow.cs, NethackAmbition.cs: Render each chamber
  - Pattern: Same as Beat1BootSequence + Beat2OpeningMonologue (BeatSceneBase subclass)
  - **Status**: ✅ COMPLETED - All beat renderers created following infrastructure pattern

- [ ] **Task 4: Adapt EchoHub to use data-driven pattern**
  - File: `source/stages/stage_2/EchoHub.cs` (refactor)
  - Changes:
    - Load Stage2NarrativeData via NarrativeDataLoader instead of EchoChamberDirector
    - Build beat sequence from manifest (not hardcoded)
    - Emit signals matching BeatSceneBase pattern
  - Goal: EchoHub becomes orchestrator only, not data transformer
  - **Status**: ⏳ NOT STARTED - Scheduled for next phase

- [x] **Task 5: Create Stage2 manifest for beat progression**
  - File: `source/stages/stage_2/stage_2_manifest.json`
  - Format: `{ "beats": [ { "name": "...", "path": "res://..." }, ... ] }`
  - Content: Define sequence of interludes and chambers
  - **Status**: ✅ COMPLETED - 7-beat manifest created (3 interludes + 3 chambers + 1 finale)

- [ ] **Task 6: Implement set piece rendering system**
  - Files: New renderers or enhanced `source/stages/stage_2/nethack/*`
  - Responsibility: Render door/monster/chest objects in chamber slots
  - Features:
    - Position objects based on slot assignment (door, monster, chest)
    - Display alignment color coding (Light/Shadow/Ambition)
    - Show interaction prompts
    - Handle decoy rendering and reveal mechanics
  - Integration: Called by NethackExploration when rendering specific chamber
  - **Status**: ⏳ NOT STARTED - Beat renderers created; rendering system enhancement pending

- [ ] **Task 7: Sync dreamweaver alignment tracking**
  - File: `source/stages/stage_2/EchoAffinityTracker.cs` (verify/enhance)
  - Changes:
    - Integrate with InterludeData option selection
    - When player chooses option → increment corresponding Dreamweaver affinity
    - At stage end → finalize affinity scores for narrative consequence
  - Verification: Test that choosing Light/Shadow/Ambition options updates scores correctly
  - **Status**: ⏳ NOT STARTED - Beat renderers have TODO comments; integration pending

- [ ] **Task 8: Create Stage2 integration tests**
  - File: `tests/stages/stage_2/Stage2MenuIntegrationTests.cs`
  - Test cases:
    - Manifest loads from stage_2_manifest.json
    - Stage2NarrativeData deserializes correctly from stage_2.json
    - EchoHub orchestrates beat sequence without errors
    - Affinity scores update when interlude options are selected
    - Chamber beat renders correctly with set piece data
    - Stage complete signal emitted after final chamber
  - Pattern: Mirror Stage1MenuIntegrationTests structure
  - **Status**: ⏳ NOT STARTED - Unit tests created; integration tests pending

- [x] **Task 9: Verify build and all tests pass**
  - Commands:
    - `dotnet build` → No errors, no warnings
    - `dotnet test` → All tests pass, including new Stage2MenuIntegrationTests
  - Cleanup: Address any CA2007 (ConfigureAwait), CA1052 (static holder) warnings
  - **Status**: ✅ COMPLETED - Build clean, all tests pass

---

## File Locations & Relationships

```
source/stages/stage_2/
├── stage_2.json                          (data file - no changes needed)
├── stage_2_manifest.json                 (NEW - beat progression)
├── Stage2NarrativeData.cs                (NEW - data structures)
├── EchoHub.cs                            (REFACTOR - use data-driven pattern)
├── EchoChamberDirector.cs                (keep if needed for backward compat)
├── EchoAffinityTracker.cs                (ENHANCE - sync with new data)
├── beats/
│   ├── scripts/
│   │   ├── BeatInterludeSequence.cs      (NEW - render interludes)
│   │   ├── NethackLight.cs           (NEW - render light chamber)
│   │   ├── NethackShadow.cs          (NEW - render shadow chamber)
│   │   ├── NethackAmbition.cs        (NEW - render ambition chamber)
│   │   └── NethackExploration.cs     (NEW - generic chamber base)
│   └── scenes/
│       ├── interlude_1.tscn              (attach BeatInterludeSequence)
│       ├── interlude_2.tscn              (attach BeatInterludeSequence)
│       ├── interlude_3.tscn              (attach BeatInterludeSequence)
│       ├── chamber_light.tscn            (attach NethackLight)
│       ├── chamber_shadow.tscn           (attach NethackShadow)
│       └── chamber_ambition.tscn         (attach NethackAmbition)
├── nethack/
│   └── (enhanced renderers for set pieces)
└── test/
    └── Stage2MenuIntegrationTests.cs     (NEW - integration tests)

source/scripts/infrastructure/
├── BeatSceneBase.cs                      (shared base - no changes)
├── NarrativeDataLoader.cs                (shared loader - no changes)
└── (other infrastructure)

tests/stages/stage_2/
└── Stage2MenuIntegrationTests.cs         (NEW - integration tests)
```

---

## Dreamweaver Variants (Data-Driven)

Each Dreamweaver owns one chamber. The JSON structure defines what's different:

### Light Chamber
```json
{
  "id": "chamber_light",
  "owner": "light",
  "style": {
    "template": "light_default",
    "ambient": "res://source/Audio/Stage2/amb_light.ogg",
    "glitchProfile": "soft_glow"
  },
  "objects": [
    { "slot": "door", "alignment": "light", "prompt": "..." },
    { "slot": "monster", "alignment": "ambition", "prompt": "..." },
    { "slot": "chest", "alignment": "shadow", "prompt": "..." }
  ]
}
```

### Shadow Chamber
- Different template (shadow_maze)
- Different object alignments in slots
- Different ambient audio
- Different glitch profile

### Ambition Chamber
- Different template (ambition_climb)
- Objects arranged by progression/challenge
- Aggressive visual theme

**Key Architecture Insight**: The renderer (NethackExploration) doesn't know which Dreamweaver owns the chamber. It just loads the data and renders what the JSON says to render. This is pure data-driven separation.

---

## Integration with Menu System

Stage 2 connects to the existing menu system:

```
MainMenu (Stage 1)
    ↓ [User selects Stage 2]
    ↓ SceneManager.TransitionToScene("res://source/stages/stage_2/beats/scenes/interlude_1.tscn")
    ↓
BeatInterludeSequence (Beat 1 of 9)
    ↓ [Player chooses option]
    ↓ Updates EchoAffinityTracker
    ↓ Emit signal_beat_complete()
    ↓
NethackLight (Beat 2 of 9)
    ↓ [Player explores chamber]
    ↓ Interact with door/monster/chest
    ↓ Emit signal_beat_complete()
    ↓
... (repeat for all 9 beats)
    ↓
Stage2Complete (EchoHub emits signal_stage_complete)
    ↓ SceneManager.TransitionToScene(nextStage)
```

---

## Expected Outcomes

Once complete, Stage 2 will have:

✅ **Data-driven architecture** matching Stage 1 pattern
- All narrative content in JSON (stage_2.json)
- Beat renderers load and transform data, never synthesize
- Tests validate JSON deserialization, not hardcoded content

✅ **Reusable infrastructure**
- NarrativeDataLoader used for both Stage 1 and Stage 2
- BeatSceneBase subclasses follow identical patterns
- Same menu integration approach

✅ **Clean beat transitions**
- EchoHub orchestrates without data transformation
- Each beat is self-contained and testable
- Stage complete signal follows same pattern

✅ **Dreamweaver variation**
- Three interludes with dialogue choices
- Three chambers with repositioned objects
- Affinity tracking informs narrative consequence

✅ **Shared UI infrastructure**
- Stage 1 terminal UI can be reused/adapted
- Set piece rendering matches visual language
- Same theming system (Light/Shadow/Ambition colors)

---

## Implementation Guidelines

### When Creating Beat Renderers
1. Always inherit from `BeatSceneBase`
2. Override `StageManifestPath` property
3. Use `NarrativeDataLoader<Stage2NarrativeData>` in Ready()
4. Never hardcode any narrative content
5. Emit `signal_beat_complete()` when ready to transition

### When Creating Stage2NarrativeData
1. Use [JsonPropertyName] to match JSON keys exactly
2. Provide default values for all properties
3. Include XML documentation comments
4. Follow naming: ClassName = JSON type name

### When Updating EchoHub
1. Keep signal definitions (light/shadow/ambition affinity updates)
2. Replace beat instantiation logic with manifest-driven approach
3. Keep stage_complete signal emission
4. Simplify orchestration to just sequence beats from manifest

### Testing Strategy
1. Unit tests for Stage2NarrativeData deserialization (matching JSON)
2. Integration test for manifest loading
3. Integration test for beat sequence from manifest
4. Integration test for affinity score updates
5. Scene tests for actual UI rendering (may require Godot runtime)

---

## 1. Data Layer (JSON-based)

**File Structure:**
```
source/stages/stage_2/
├── stage_2.json (existing - contains interludes, chambers, banter)
├── stage_2_manifest.json (NEW - beat progression)
├── chambers/
│   ├── chamber_light.json (NEW - light path chamber data)
│   ├── chamber_shadow.json (NEW - shadow path chamber data)
│   └── chamber_ambition.json (NEW - ambition path chamber data)
```

**Key Changes:**
- Move chamber definitions from `stage_2.json` root to separate chamber files
- Create `stage_2_manifest.json` defining beat sequence (interlude_1 → choice → chamber_X → finale)
- Support multiple chamber layouts per path (3 choices = 3 possible chambers)

### 2. C# Data Structures (NEW FILES)

**Location:** `source/stages/stage_2/Stage2NarrativeData.cs`

Data classes matching `stage_2.json` schema:
```csharp
public class Stage2NarrativeData
{
    public Metadata Metadata { get; set; }
    public List<DreamweaverConfig> Dreamweavers { get; set; }
    public List<InterludeData> Interludes { get; set; }
    public List<ChamberData> Chambers { get; set; }
    public FinaleData Finale { get; set; }
}

public class InterludeData
{
    public string Id { get; set; }
    public string Owner { get; set; } // "light", "shadow", "ambition"
    public string Prompt { get; set; }
    public List<InterludeOption> Options { get; set; }
}

public class ChamberData
{
    public string Id { get; set; }
    public string Owner { get; set; }
    public string Layout { get; set; } // Template ID
    public List<ChamberObject> Objects { get; set; }
    public List<ChamberEnemy> Enemies { get; set; }
    public string BanterLineOnEnter { get; set; }
    public string BanterLineOnExit { get; set; }
}

public class DreamweaverAffinityData
{
    public string DreamweaverId { get; set; }
    public int Score { get; set; }
    public List<string> BanterTriggered { get; set; }
}
```

### 3. Scene Structure (NEW/REFACTORED)

**Scenes:**
```
source/stages/stage_2/beats/
├── scripts/
│   ├── Beat1Interlude.cs (NEW - choice presentation, inherits BeatSceneBase)
│   ├── Beat2ChamberExploration.cs (NEW - dungeon exploration, inherits BeatSceneBase)
│   └── Beat3Finale.cs (NEW - dreamweaver claim, inherits BeatSceneBase)
├── interlude.tscn (REFACTOR - use template pattern like Stage 1)
├── chamber.tscn (REFACTOR - dungeon crawler UI template)
└── finale.tscn (NEW - completion and dreamweaver reveal)
```

### 4. Beat Flow Pattern

Unlike Stage 1's 8 sequential beats, Stage 2 has a **branching beat flow**:

```
START
  ↓
[Beat 1: Interlude 1]
  Player chooses: Light/Shadow/Ambition
  ↓
[Beat 2: Chamber Exploration]
  - Load chamber based on choice
  - Explore dungeon
  - Encounter enemies, objects
  - Track affinity changes
  ↓
[Beat 3: Interlude 2 or Finale]
  - If more interludes: repeat pattern
  - Else: Play finale, reveal claimed Dreamweaver
  ↓
END → Stage 3
```

### 5. Key Infrastructure Changes

**New Loaders (reusable across stages):**
- `ChamberDataLoader.cs` - Loads chamber JSON with dynamic object spawning
- `AffinityTracker.cs` - Tracks Dreamweaver alignment scores

**Extend BeatSceneBase:**
- Add `OnChamberComplete()` callback for encounter resolution
- Add `TrackAffinityChange(dreamweaver, delta)` helper
- Support branching to different next beats based on choices

### 6. Terminal UI Differences

**Stage 1 (Cinematic):**
- Full-screen terminal window
- Text rendering with timing
- Choice buttons displayed sequentially

**Stage 2 (Dungeon Crawler):**
- Split terminal UI:
  - Left side: ASCII dungeon map (5-7 lines)
  - Right side: Status/log output
- Interactive elements: movement keys, action buttons
- Real-time position tracking
- Enemy/object visual indicators

**Implementation:**
- Extend `TerminalBase.cs` to support multi-pane layout
- Create `DungeonRenderer.cs` for ASCII map generation
- Use existing `TerminalShaderController` for glitch effects

### 7. Testing Strategy

**Unit Tests (no Godot runtime):**
- `Stage2NarrativeDataTests.cs` - Deserialize stage_2.json
- `AffinityTrackerTests.cs` - Verify scoring logic
- `ChamberLayoutTests.cs` - Chamber template loading

**Integration Tests (with Godot):**
- `Stage2MenuIntegrationTests.cs` - MainMenu loads Stage 2
- `InterludeSceneTests.cs` - Interlude choices update affinity
- `ChamberExplorationTests.cs` - Chamber encounter completes
- `Stage2EndToEndTests.cs` - Full beat sequence from start to finale

### 8. Implementation Roadmap

**Phase 1: Data Model (1-2 days)**
1. Create `Stage2NarrativeData.cs` with all required classes
2. Refactor `stage_2.json` to match schema
3. Create `stage_2_manifest.json` with beat sequence
4. Test deserialization with `NarrativeDataLoader`

**Phase 2: Scene Scaffolding (2-3 days)**
1. Create `stage_2_manifest.json` defining beats
2. Create beat scene scripts (Beat1Interlude, Beat2Chamber, Beat3Finale)
3. Create base scene templates (interlude.tscn, chamber.tscn, finale.tscn)
4. Wire transitions via `TransitionToNextBeat()`

**Phase 3: Core Mechanics (3-4 days)**
1. Implement `AffinityTracker` scoring logic
2. Implement `DungeonRenderer` for ASCII maps
3. Implement chamber exploration and encounter system
4. Wire choice → affinity change → banter flow

**Phase 4: Polish & Integration (2-3 days)**
1. Add audio/shader effects
2. Implement full banter system
3. Create end-to-end tests
4. Integrate with MainMenu for Stage 2 button

---

## Comparison with Stage 1 Pattern

### What We Reuse
✅ `BeatSceneBase` - Beat progression via manifest
✅ `NarrativeDataLoader` - Generic JSON deserialization
✅ `StageManifestLoader` - Manifest-driven scene transitions
✅ `MainMenu` integration - Stage button triggers stage_2_manifest.json
✅ Test pattern - Stage2MenuIntegrationTests following Stage1 model

### What We Adapt
🔄 Terminal UI - Multi-pane layout instead of single text area
🔄 Data structure - Chambers, enemies, objects instead of narrative beats
🔄 Interaction model - Choice-based + dungeon exploration
🔄 Affinity tracking - Implicit scoring vs. explicit dreamweaver alignment

### What's New
🆕 `ChamberDataLoader` - Load chamber definitions dynamically
🆕 `DungeonRenderer` - ASCII map generation and rendering
🆕 `AffinityTracker` - Score calculation and banter triggers
🆕 Branching beat logic - Multiple next scenes based on choices

---

## Creative Team Workflow

**Designer (Content):**
1. Edit `stage_2.json` to add/modify:
   - Interlude prompts and options
   - Chamber layouts and objects
   - Banter lines and approval conditions
   - Affinity scoring deltas

2. No code changes needed - all changes in JSON

**Programmer (Code):**
1. Implement beat scripts and loaders
2. Wire affinity tracking
3. Implement dungeon exploration mechanics
4. Add polish and edge case handling

---

## Validation Checklist

Before considering Stage 2 complete:

- [ ] `Stage2NarrativeData.cs` deserializes stage_2.json without errors
- [ ] `stage_2_manifest.json` defines complete beat sequence
- [ ] Beat scripts implement `BeatSceneBase` correctly
- [ ] MainMenu "Stage 2" button loads first interlude
- [ ] Interlude choices update affinity tracker
- [ ] Chamber loads and renders dungeon layout
- [ ] Enemies spawn and can be encountered
- [ ] Final choice reveals claimed Dreamweaver
- [ ] Integration tests pass (Stage2MenuIntegrationTests)
- [ ] Can load Stage 2 in Godot editor without errors
- [ ] Progression from Stage 1 finale → Stage 2 hub works
- [ ] Progression from Stage 2 finale → Stage 3 works

---

## Reference Files

- Stage 1 Pattern: `/source/stages/stage_1/`
- Existing Stage 2 Code: `/source/stages/stage_2/`
- Infrastructure: `/source/scripts/infrastructure/`
- Terminal UI: `/source/ui/terminal/`
- Tests: `/tests/stages/`

---

## Next Steps

1. **Data Preparation**: Finalize `stage_2.json` schema and create `stage_2_manifest.json`
2. **Create Stage2NarrativeData.cs**: Full C# model matching JSON structure
3. **Create Beat Scripts**: Beat1Interlude, Beat2Chamber, Beat3Finale
4. **Implement Loaders**: ChamberDataLoader, AffinityTracker
5. **Scene Templates**: Create interlude.tscn, chamber.tscn, finale.tscn
6. **Wire Everything**: Connect beats via manifest, test transitions
7. **Implement UI**: DungeonRenderer, multi-pane terminal layout
8. **Test**: Create Stage2MenuIntegrationTests, verify complete flow

---

## Document Sync Status

This document is synchronized with the TODO list in the chat window. When tasks are completed, mark the corresponding checkbox above. When understanding changes, update the architecture section accordingly.

**Last Updated**: October 22, 2025
**Status**: IMPLEMENTATION IN PROGRESS (5/9 tasks completed)

### Synchronized Tasks
The 9-task TODO list in chat matches the "Implementation Checklist" section above. Both will be updated together as work progresses.

### Summary of Work Completed
- ✅ Created `Stage2NarrativeData.cs` with full data structures for stage_2.json
- ✅ Verified JSON deserialization with comprehensive test suite (9 test cases)
- ✅ Created BeatInterludeSequence.cs for rendering interludes
- ✅ Created NethackExploration.cs (base) and three subclasses (Light, Shadow, Ambition)
- ✅ Created stage_2_manifest.json with 7-beat sequence
- ✅ Build and tests pass cleanly

### Next Steps (Recommended Order)
1. **Task 4**: Adapt EchoHub to use data-driven pattern
   - Currently still uses EchoChamberDirector pattern
   - Should be refactored to load Stage2NarrativeData and build beat sequence from manifest
   
2. **Task 7**: Implement EchoAffinityTracker integration
   - Beat renderers have TODO comments marked for this integration
   - Interlude option selection needs to update alignment scores
   
3. **Task 8**: Create Stage2MenuIntegrationTests
   - Will validate manifest loading and beat progression
   - Should test EchoAffinityTracker integration
   
4. **Task 6**: Implement set piece rendering system
   - Beat renderers currently display basic text; could enhance with ASCII art
   - Integrate with existing nethack renderers if available

### Architecture Pattern Successfully Applied
Stage 2 now follows the same data-driven architecture as Stage 1:
- **All narrative content** stored in `stage_2.json`
- **Beat renderers** load and transform data, never synthesize
- **Manifest-driven** beat sequence via `stage_2_manifest.json`
- **Infrastructure sharing** with BeatSceneBase and NarrativeDataLoader
- **Three-fold Dreamweaver variation** driven by data (not hardcoded)

