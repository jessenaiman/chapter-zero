# Stage 2 Implementation Gap Analysis

## Overview
This document identifies what's **already implemented** vs. what **needs to be built** for Stage 2 (Echo Chamber), with specific focus on integrating the existing godot-open-rpg combat system.

---

## ✅ What's Already Complete

### 1. Data Layer (100% Complete)
- ✅ `stage2.json` - Complete 3 interludes + 3 chambers + finale
- ✅ `EchoChamberDirector` - JSON loader with validation
- ✅ `NarrativeSceneFactory` - Parses JSON → C# data structures
- ✅ `NarrativeSceneData` - Complete data models (EchoChamberData, EchoChamberInterlude, EchoChamberChamber, etc.)

### 2. Orchestration Layer (100% Complete)
- ✅ `IEchoSequence` - Interface for interlude/chamber controllers
- ✅ `EchoAffinityTracker` - Scoring system (+2 harmony, +1 cross-alignment)
- ✅ `EchoOrchestratorBeat` - 8-beat sequence definition
- ✅ `EchoHub` - State machine orchestrator (mirrors GhostTerminalDirector)

### 3. Existing Combat System (godot-open-rpg)
- ✅ `Combat.cs` - Combat initiator & transition manager
- ✅ `CombatArena.cs` - Main combat scene controller
- ✅ `CombatActor.cs` - Base class for battlers (abstract)
- ✅ `CombatAI.cs` + `CombatRandomAI.cs` - Enemy behavior system
- ✅ `CombatTurnQueue.cs` - Turn order management
- ✅ `ActiveTurnQueue.cs` - Active time battle system
- ✅ `UICombatMenus.cs` - Combat action menus
- ✅ `UICombatLog.cs` - Combat event logging
- ✅ `CombatSceneData.cs` - Combat configuration data model
- ✅ `Elements.cs` - Elemental damage system
- ✅ Scene files: `combat_arena.tscn`, `combat_ai.tscn`

### 4. Placeholder Combat (Needs Refactoring)
- ⚠️ `PixelCombatController.cs` - Simplified combat with 8 placeholder TODOs
  - Has basic HP tracking, action buttons, damage calculation
  - **NOT integrated with godot-open-rpg system**
  - **Should be replaced or refactored**

---

## 🔴 Critical Gaps (Must Implement)

### Gap 1: EchoInterlude UI Controller
**Status**: Stub exists, needs full implementation  
**File**: `source/stages/stage_2/EchoInterlude.cs` (currently 29 lines)

**What Exists**:
```csharp
public void Bind(EchoChamberInterlude interlude)
{
    if (this.promptLabel != null)
    {
        this.promptLabel.Text = interlude.Prompt;
    }
}
```

**What's Needed**:
1. **UI Binding** (30 min)
   - Render prompt text
   - Create 3 choice buttons dynamically from `interlude.Options`
   - Apply Dreamweaver accent colors to buttons
   - Display owner label ("Light's Question", "Shadow's Riddle", etc.)

2. **User Interaction** (30 min)
   - Connect button signals to choice handler
   - Emit `ChoiceMade(choiceId, alignment)` signal
   - Disable buttons after selection
   - Show visual feedback (button hover/press states)

3. **Banter Display** (30 min)
   - Display approve line from aligned Dreamweaver
   - Display dissent lines from other 2 Dreamweavers
   - Typewriter effect for dialogue (optional polish)
   - Delay before transitioning to next beat

**Estimated Time**: 1.5-2 hours  
**Priority**: HIGH (blocks stage playthrough)

---

### Gap 2: EchoDungeon Renderer
**Status**: Empty stub (125 lines of signals, zero implementation)  
**File**: `source/stages/stage_2/EchoDungeon.cs`

**What Exists**:
- Signal definitions only
- No rendering logic
- No movement handling
- No object interaction

**What's Needed**:
1. **ASCII Map Rendering** (1-2 hours)
   - Parse chamber `style.template` (light_default, shadow_maze, ambition_forge)
   - Generate ASCII grid based on template
   - Place objects at designated slots (D = door, M = monster, C = chest)
   - Place decoys (2 per chamber)
   - Render player '@' glyph
   - Use `AsciiRoomRenderer` or create simplified version

2. **Player Movement** (1 hour)
   - Arrow keys / WASD input handling
   - Grid-based movement validation (no walking through walls)
   - Player position tracking
   - Camera follows player (if needed)

3. **Object Interaction** (1-2 hours)
   - Collision detection with objects (door/monster/chest)
   - Display `object.Prompt` text
   - Show `object.InteractionLog` lines
   - **Trigger combat for "monster" slot** (see Gap 3)
   - Display banter (approve/dissent) after interaction
   - Emit `InteractionResolved(glyph, alignment, change)` signal

4. **Decoy Handling** (30 min)
   - Clicking decoys shows `revealText` ("[STATIC] {???}")
   - Decoys disappear after interaction
   - No scoring impact

**Estimated Time**: 3.5-5.5 hours  
**Priority**: HIGH (core gameplay mechanic)  
**Blocker**: TerminalDisplayBox missing (see Gap 4)

---

### Gap 3: Combat Integration for "Monster" Objects
**Status**: Not implemented (design choice needed)

**Current State**:
- Stage 2 JSON has "monster" objects with `interactionLog` like:
  ```json
  "interactionLog": [
    "> FIGHT: You trade three blows.",
    "> FIGHT: Static scatters like snow."
  ]
  ```
- Looks like **narrative combat**, not full RPG battle

**Design Options**:

#### Option A: Narrative Combat (Simpler - RECOMMENDED for Stage 2)
**Pros**: Matches Stage 2's philosophical tone, faster to implement  
**Cons**: Doesn't use godot-open-rpg system  
**Implementation** (30 min):
```csharp
// In EchoDungeon.cs when player touches 'M' glyph
private void HandleMonsterInteraction(EchoChamberObject monster)
{
    // Display interaction log as narration
    foreach (string line in monster.InteractionLog)
    {
        await DisplayTextAsync(line); // Typewriter effect
        await Task.Delay(500);
    }
    
    // Auto-resolve (player always "wins")
    await DisplayBanterAsync(monster.Banter);
    
    // Record alignment choice
    tracker.RecordChamberChoice(chamberOwner, "monster", monster.Alignment);
    
    // Remove monster from map
    RemoveObjectFromGrid('M');
}
```

#### Option B: Simplified Turn-Based Combat (Medium complexity)
**Pros**: Uses combat system infrastructure, more engaging  
**Cons**: Requires combat data setup, may feel too "gamey" for Stage 2  
**Implementation** (2-3 hours):
```csharp
// Create lightweight combat encounters per chamber
private async Task HandleMonsterInteraction(EchoChamberObject monster)
{
    // Build CombatSceneData from chamber metadata
    var combatData = new CombatSceneData
    {
        Type = "echo_chamber_skirmish",
        Enemy = new CombatEnemy
        {
            Name = monster.Alignment + " Echo",
            HP = 30, // Low HP for quick combat
            Attack = 10,
            Defense = 5,
            Sprite = $"res://source/assets/combat/{monster.Alignment}_echo.png"
        },
        PlayerSprite = "res://source/assets/combat/player_shard.png"
    };
    
    // Trigger combat via existing system
    var combatEvents = GetNode<CombatEvents>("/root/CombatEvents");
    combatEvents.CombatTriggered(CreateCombatScene(combatData));
    
    // Wait for combat result
    bool victory = await WaitForCombatFinished();
    
    // Record alignment based on outcome
    if (victory)
    {
        tracker.RecordChamberChoice(chamberOwner, "monster", monster.Alignment);
    }
}
```

#### Option C: Full godot-open-rpg Integration (Complex)
**Pros**: Production-ready combat, reusable for Stage 4  
**Cons**: Overkill for Stage 2's narrative focus, 5-8 hour implementation  
**Not recommended for Stage 2** - save for Stage 4 (Pixel Combat phase)

**Recommendation**: **Option A (Narrative Combat)** for Stage 2  
- Aligns with "NetHack-inspired simulation" theme
- Faster implementation (30 min vs. 2-3 hours)
- Can upgrade to Option B/C in Stage 4

---

### Gap 4: TerminalDisplayBox Blocker
**Status**: CRITICAL BLOCKER - component removed during Stage 1 cleanup  
**Files**: `source/scripts/field/AsciiRoomRenderer.cs` (lines 30, 165, 191)

**Problem**:
```csharp
// Line 30: Commented out
// private TerminalDisplayBox? terminalDisplayBox;

// Line 165: Disabled 3D rendering
// TODO: Stage2 - Temporarily disabled 3D effects

// Line 191: Lighting also disabled
// TODO: Stage2 - Lighting also disabled
```

**Options**:

#### Option A: Stub Out References (15 min - QUICKEST)
```csharp
// Comment out or remove all TerminalDisplayBox usage
// EchoDungeon can use simplified 2D rendering instead
```

#### Option B: Restore TerminalDisplayBox (1-2 hours)
- Find original implementation in git history
- Restore class and scene structure
- Wire back to AsciiRoomRenderer
- Test 3D terminal effects

#### Option C: Refactor AsciiRoomRenderer (2-3 hours)
- Remove dependency entirely
- Use godot-xterm directly
- Simplify rendering pipeline

**Recommendation**: **Option A (Stub)** for Stage 2  
- EchoDungeon doesn't need 3D terminal effects
- Simple 2D ASCII grid is sufficient
- Can restore later for polish phase

---

## 🟡 Medium Priority Gaps

### Gap 5: Scene File Wiring
**Status**: Scene files exist but not connected to controllers

**Files**:
- `source/stages/stage_2/echo_hub.tscn` - Needs UI nodes
- `source/stages/stage_2/echo_interlude.tscn` - Needs choice button layout
- `source/stages/stage_2/echo_dungeon.tscn` - Needs grid container

**What's Needed** (1-2 hours):
1. Open each `.tscn` in Godot editor
2. Add required Control nodes:
   - EchoHub: StatusLabel, ContentContainer
   - EchoInterlude: PromptLabel, ChoiceButton1/2/3, BanterLabel
   - EchoDungeon: GridContainer, LogLabel, InteractionPanel
3. Set unique names with `%` prefix for GetNode<T>() lookup
4. Save scenes

**Estimated Time**: 1-2 hours  
**Priority**: MEDIUM (can prototype without proper scenes first)

---

### Gap 6: EchoHub Scene Instantiation
**Status**: TODO placeholders in `EchoHub.cs`

**Current Code**:
```csharp
private async Task PlayInterludeAsync(EchoInterludeBeat beat)
{
    // TODO: Load and instance echo_interlude.tscn
    // TODO: Bind beat.Interlude data to the scene
    // TODO: Wait for user choice
    // TODO: Record choice to affinityTracker
    
    // Placeholder simulation
    await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
}
```

**What's Needed** (30 min per beat type):
```csharp
private async Task PlayInterludeAsync(EchoInterludeBeat beat)
{
    // Load scene
    var interludeScene = GD.Load<PackedScene>("res://source/stages/stage_2/echo_interlude.tscn");
    var interludeInstance = interludeScene.Instantiate<EchoInterlude>();
    
    // Add to tree
    contentContainer?.AddChild(interludeInstance);
    
    // Bind data
    interludeInstance.Bind(beat.Interlude);
    
    // Connect signal
    interludeInstance.ChoiceMade += (choiceId, alignment) =>
    {
        affinityTracker.RecordInterludeChoice(beat.Interlude.Owner, choiceId, alignment);
    };
    
    // Wait for completion
    await ToSignal(interludeInstance, IEchoSequence.SignalName.SequenceComplete);
    
    // Cleanup
    interludeInstance.QueueFree();
}
```

**Estimated Time**: 1.5 hours (3 beat types × 30 min)  
**Priority**: MEDIUM (orchestration works, just needs scene loading)

---

## 🟢 Low Priority / Polish

### Gap 7: Finale Display
**Status**: Placeholder timer in `PlayFinaleAsync()`

**What's Needed** (1 hour):
- Display claim dialogue from chosen Dreamweaver
- Show responses from other 2 Dreamweavers
- Display system outro
- Typewriter effects
- Transition to Stage 3

### Gap 8: Visual Polish
**Status**: Functional but needs aesthetics

**What's Needed** (4-6 hours):
- CRT shader effects for interludes
- Glitch profiles per chamber (soft_glow, sharp_distortion, heat_shimmer)
- Audio ambience per chamber (amb_light.ogg, amb_shadow.ogg, amb_ambition.ogg)
- Dreamweaver accent colors (#c9ffdd, #f38bff, #ff6868)
- Typewriter effect for all text
- Screen transitions between beats

---

## 📊 Implementation Roadmap

### Phase 1: Core Functionality (6-8 hours)
**Goal**: Playable stage from start to finish

1. **Fix TerminalDisplayBox blocker** (15 min) - Stub out references
2. **Implement EchoInterlude UI** (2 hours) - Choice rendering + banter
3. **Implement EchoDungeon renderer** (4 hours) - ASCII map + movement + interaction
4. **Wire scene loading in EchoHub** (1.5 hours) - Replace TODO placeholders
5. **Narrative combat for monsters** (30 min) - Option A implementation

**Deliverable**: Can play through all 3 interludes + 3 chambers + finale

---

### Phase 2: Combat Integration (Optional - 2-3 hours)
**Goal**: Use godot-open-rpg system for monster encounters

1. **Create combat data per chamber** (1 hour) - 3 CombatSceneData configs
2. **Hook combat trigger in EchoDungeon** (1 hour) - Option B implementation
3. **Handle combat outcomes** (30 min) - Victory/defeat scoring
4. **Test combat flow** (30 min) - Smoke tests

**Deliverable**: Monster encounters use turn-based combat

---

### Phase 3: Polish & Testing (4-6 hours)
**Goal**: Production-ready stage

1. **Finale display** (1 hour) - Complete PlayFinaleAsync()
2. **Visual effects** (2-3 hours) - CRT, glitch, colors
3. **Audio integration** (1 hour) - Ambient tracks per chamber
4. **Unit tests** (1 hour) - EchoAffinityTracker, EchoOrchestratorBeat
5. **Integration tests** (1 hour) - Full stage playthrough

**Deliverable**: Polished, tested Stage 2

---

## 🎯 Recommended Approach

### For Minimum Viable Stage 2 (MVP):
**Time**: 6-8 hours  
**Path**: Phase 1 only

1. Stub TerminalDisplayBox (15 min)
2. Build EchoInterlude UI (2 hours)
3. Build EchoDungeon with narrative combat (4.5 hours)
4. Wire EchoHub scene loading (1.5 hours)

**Result**: Fully playable Stage 2 with philosophical choices and narrative dungeon exploration

---

### For Full godot-open-rpg Integration:
**Time**: 8-11 hours  
**Path**: Phase 1 + Phase 2

1. Complete Phase 1 (6-8 hours)
2. Create combat encounters (1 hour)
3. Hook combat system (1 hour)
4. Test & polish (1-2 hours)

**Result**: Stage 2 with turn-based monster battles

---

### For Production-Ready Stage:
**Time**: 12-17 hours  
**Path**: All 3 phases

**Result**: Fully polished Stage 2 with visual effects, audio, tests

---

## 🔧 Technical Decisions Needed

### Decision 1: Combat Style
- [ ] **Option A**: Narrative combat (text-based, auto-resolve) - 30 min
- [ ] **Option B**: Simplified turn-based (uses CombatSceneData) - 2-3 hours
- [ ] **Option C**: Full godot-open-rpg (complex, reusable) - 5-8 hours

**Recommendation**: Option A for Stage 2, Option C for Stage 4

---

### Decision 2: TerminalDisplayBox
- [ ] **Option A**: Stub out (fastest) - 15 min
- [ ] **Option B**: Restore component - 1-2 hours
- [ ] **Option C**: Refactor AsciiRoomRenderer - 2-3 hours

**Recommendation**: Option A for Stage 2

---

### Decision 3: Scene Implementation Order
- [ ] **Top-down**: EchoHub → Interlude → Dungeon (mirrors data flow)
- [ ] **Bottom-up**: Dungeon → Interlude → Hub (test components first)
- [ ] **Parallel**: Split work across team members

**Recommendation**: Bottom-up (test Interlude/Dungeon in isolation, then integrate)

---

## 📝 Summary

### Already Built (559 lines):
- ✅ Complete data layer (stage2.json → C# models)
- ✅ Orchestration architecture (beats, tracker, hub)
- ✅ Existing combat system (godot-open-rpg)

### Must Build (Core - 6-8 hours):
- 🔴 EchoInterlude UI controller
- 🔴 EchoDungeon renderer + movement + interaction
- 🔴 Narrative combat for monsters
- 🔴 Scene loading in EchoHub

### Optional (Combat - 2-3 hours):
- 🟡 godot-open-rpg integration for monster battles

### Polish (4-6 hours):
- 🟢 Finale display
- 🟢 Visual effects + audio
- 🟢 Tests

**Total**: 12-17 hours for production-ready Stage 2  
**MVP**: 6-8 hours for playable Stage 2

---

**Next Step**: Choose combat style (Option A/B/C) and TerminalDisplayBox approach (A/B/C), then start with EchoInterlude UI.
