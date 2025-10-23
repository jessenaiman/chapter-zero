# Stage 2: Data-Driven NetHack/Rogue-like Architecture

**Goal:** Implement Stage 2 (Echo Chamber) using the same data-driven, manifest-based architecture as Stage 1, but with NetHack/Rogue-like dungeon crawler gameplay mechanics.

---

## Architecture Overview

Stage 2 will follow the **three-layer separation of concerns** established in Stage 1:

```
1. STRUCTURE (.tscn files)
   └─ Visual layout templates for hub, interlude, dungeon, finale

2. CONTENT (stage_2.json)
   └─ Dreamweavers, interludes, chambers, dungeons, banter, alignment scores

3. LOGIC (C# scripts)
   └─ Generic infrastructure in source/scripts/infrastructure/
   └─ Stage 2 specific in source/stages/stage_2/
```

### Key Differences from Stage 1

| Aspect | Stage 1 (Terminal Cinematic) | Stage 2 (Echo Chamber) |
|--------|------------------------------|----------------------|
| Gameplay | Linear narrative choices | Dungeon exploration + tactical choices |
| Beat Structure | 8 sequential beats | Variable beats: interludes, chambers, finale |
| Player Agency | Choice-driven | Movement + exploration + choice |
| Display | Pure text/terminal | ASCII map + combat overlay + terminal UI |
| Progression | Story-driven | Objective-driven (clear chambers, gain alignment) |

---

## TODO Checklist

### Phase 1: Data Model & Infrastructure ✓ (Completed)
- [x] `stage_2.json` exists with metadata, dreamweavers, interludes, chambers, banter
- [x] Understand schema structure and alignment scoring system
- [x] Review existing EchoChamberData structures in EchoChamberDirector.cs
- [x] Confirm NarrativeDataLoader can handle polymorphic JSON deserialization

### Phase 2: Stage 2-Specific Data Structures (To Do)
- [ ] Create `Stage2NarrativeData.cs` matching stage_2.json schema
  - [ ] `Stage2Metadata` (iteration, interface, status, systemIntro)
  - [ ] `DreamweaverDefinition` (id, name, accentColor, textTheme)
  - [ ] `InterludioData` (id, owner, prompt, options with alignment/banter)
  - [ ] `ChamberDefinition` (id, owner, layoutTemplate, objects, decoyCount, rewards)
  - [ ] `BanterLine` (speaker, text)
  - [ ] `FinaleData` (introLines, conclusions by alignment)

- [ ] Create `Stage2NarrativeDataValidator.cs` to verify:
  - [ ] All alignment scores sum correctly
  - [ ] Dreamweaver IDs are consistent across interludes/chambers/banter
  - [ ] Chamber layout templates exist
  - [ ] No circular chamber dependencies

### Phase 3: Stage Manifest & Beat Director (To Do)
- [ ] Create `source/stages/stage_2/stage_manifest.json` defining beat sequence
  - [ ] beat_1_hub_introduction
  - [ ] beat_2_interlude_light (or dynamically choose based on player?)
  - [ ] beat_3_chamber_1
  - [ ] beat_4_interlude_shadow
  - [ ] beat_5_chamber_2
  - [ ] ... (N chambers)
  - [ ] beat_N_finale

- [ ] Create `Stage2Director.cs` to orchestrate beats:
  - [ ] Load stage_2.json
  - [ ] Generate beat sequence based on player choices
  - [ ] Track cumulative alignment scores
  - [ ] Trigger chamber reveals based on progression
  - [ ] Assemble finale dialogue based on final alignment vector

### Phase 4: Scene Templates (To Do)
- [ ] Review/refactor `echo_hub.tscn`
  - [ ] Terminal window with boot dialogue
  - [ ] Button to start first interlude
  - [ ] Status display (current alignment, chamber count)

- [ ] Create/refactor `interlude.tscn` beat scene
  - [ ] Display interlude prompt from JSON
  - [ ] Render three choice options as buttons
  - [ ] Show banter based on choice
  - [ ] Track alignment change

- [ ] Create/refactor `chamber.tscn` beat scene
  - [ ] ASCII dungeon map rendering
  - [ ] Player movement controls
  - [ ] Object interaction system
  - [ ] Combat overlay (auto-fight or tactical?)
  - [ ] Exit/completion check

- [ ] Create `finale.tscn` beat scene
  - [ ] Display ending sequence based on final alignment
  - [ ] Show Dreamweaver claim
  - [ ] Transition to Stage 3

### Phase 5: Beat Scene Scripts (To Do)
- [ ] Create `Stage2HubBeat.cs` (inherits BeatSceneBase)
  - [ ] Loads stage_2.json
  - [ ] Renders system intro from metadata.systemIntro
  - [ ] Shows "Begin Echo Chamber" button
  - [ ] Transitions to first interlude beat

- [ ] Create `InterludioBeat.cs` (reusable for all interludes)
  - [ ] Inherits BeatSceneBase
  - [ ] Loads current interlude from stage_2.json
  - [ ] Renders prompt + 3 options
  - [ ] Handles option selection
  - [ ] Records alignment change via GameState
  - [ ] Displays banter
  - [ ] Transitions to next beat (chamber or another interlude)

- [ ] Create `ChamberBeat.cs` (reusable for all chambers)
  - [ ] Inherits BeatSceneBase
  - [ ] Loads chamber definition from stage_2.json
  - [ ] Renders ASCII dungeon map
  - [ ] Handles player input (movement, interact)
  - [ ] Manages combat encounters
  - [ ] Tracks completion state
  - [ ] Transitions to next beat when chamber cleared

- [ ] Create `FinaleBeat.cs` (inherits BeatSceneBase)
  - [ ] Loads finale data from stage_2.json
  - [ ] Determines ending based on final alignment vector
  - [ ] Renders conclusion dialogue
  - [ ] Records chosen Dreamweaver to GameState
  - [ ] Transitions to Stage 3

### Phase 6: Tests (To Do)
- [ ] Create `Stage2DataStructureTests.cs`
  - [ ] Verify Stage2NarrativeData deserializes from stage_2.json
  - [ ] Test all alignment scores parse correctly
  - [ ] Validate banter lines are accessible by choice ID

- [ ] Create `Stage2DirectorTests.cs`
  - [ ] Test beat sequence generation
  - [ ] Verify alignment accumulation
  - [ ] Test finale determination logic

- [ ] Create `Stage2IntegrationTests.cs`
  - [ ] Load Hub beat from manifest
  - [ ] Verify all interlude .tscn files can load
  - [ ] Verify all chamber .tscn files can load
  - [ ] Verify all beat transitions work

- [ ] Create `InterludioBeatTests.cs`
  - [ ] Test option rendering from JSON
  - [ ] Test alignment tracking
  - [ ] Test banter display logic

- [ ] Create `ChamberBeatTests.cs`
  - [ ] Test dungeon map generation from chamber definition
  - [ ] Test object placement logic
  - [ ] Test completion detection

### Phase 7: Integration & Polish (To Do)
- [ ] Update `MainMenu.cs` to load Stage 2 from manifest
  - [ ] Add Stage 2 button
  - [ ] Wire Stage 2 button to load Stage2Hub beat

- [ ] Update `stage_manifest.json` (Stage 2 root) with beat sequence

- [ ] Create `STAGE_2_README.md` documenting:
  - [ ] Data format (stage_2.json schema)
  - [ ] How to add new interludes/chambers
  - [ ] How to customize banter
  - [ ] Testing commands

- [ ] Create `IMPLEMENTATION_STATUS.md` documenting:
  - [ ] Completed: Data-driven architecture
  - [ ] In Progress: Beat scene implementations
  - [ ] Outstanding: Art/audio/shader work
  - [ ] Dependencies: Dreamweaver asset pack, dungeon tileset

### Phase 8: Runtime Validation (To Do)
- [ ] Test in Godot Editor
  - [ ] Load MainMenu
  - [ ] Click Stage 2 button
  - [ ] Verify Hub beat loads with system intro
  - [ ] Verify first interlude renders correctly
  - [ ] Verify choice selection works and updates alignment
  - [ ] Verify banter displays
  - [ ] Verify chamber loads and movement works
  - [ ] Verify finale generates based on alignment vector

---

## File Structure

```
source/stages/stage_2/
├── stage_2.json                          # Content: Narrative, chambers, banter
├── stage_manifest.json                   # Beat sequence definition
├── Stage2NarrativeData.cs                # Data structures (NEW)
├── Stage2NarrativeDataValidator.cs       # Data validation (NEW)
├── Stage2Director.cs                     # Beat orchestration (NEW)
├── Stage2HubBeat.cs                      # Hub scene logic (NEW)
├── InterludioBeat.cs                     # Interlude scene logic (NEW)
├── ChamberBeat.cs                        # Chamber scene logic (NEW)
├── FinaleBeat.cs                         # Finale scene logic (NEW)
├── echo_hub.tscn                         # Hub visual template
├── interlude.tscn                        # Interlude visual template (NEW)
├── chamber.tscn                          # Chamber visual template (NEW)
├── finale.tscn                           # Finale visual template (NEW)
├── nethack/                              # ASCII dungeon assets
└── IMPLEMENTATION_PLAN.md                # Original plan

tests/stages/stage_2/
├── Stage2DataStructureTests.cs           # Data model tests (NEW)
├── Stage2DirectorTests.cs                # Beat orchestration tests (NEW)
├── Stage2IntegrationTests.cs             # Full integration tests (NEW)
├── InterludioBeatTests.cs                # Interlude logic tests (NEW)
└── ChamberBeatTests.cs                   # Chamber logic tests (NEW)

docs/
├── STAGE_2_README.md                     # User documentation (NEW)
└── IMPLEMENTATION_STATUS.md              # Progress tracking (NEW)
```

---

## Key Design Decisions

### 1. Beat Sequence is Dynamic
Unlike Stage 1 (8 predetermined beats), Stage 2's beat sequence can vary based on:
- Player choices in interludes (which alignment path to take)
- Chamber completion order
- Optional hidden chambers

**Solution:** `Stage2Director.GenerateBeatSequence()` creates the manifest dynamically after analyzing stage_2.json and player's alignment vector.

### 2. Alignment Tracking is Silent
Players don't see a numerical score; alignment influences banter and chamber behavior subtly.

**Solution:** 
- Record each choice via `GameState.RecordChoice(choiceId, alignmentChange)`
- Accumulate alignment vector internally
- Use vector in banter selection and finale determination
- Display only qualitative hints: "Light seems pleased", "Shadow is amused", etc.

### 3. ASCII Dungeons are Data-Driven
Chamber layout, objects, and enemies come from stage_2.json.

**Solution:**
- `ChamberDefinition.layoutTemplate` references a tileset layout
- `ChamberDefinition.objects` defines interactive elements (chests, doors, NPCs)
- `ChamberBeat.RenderDungeon()` parses JSON and creates ASCII map dynamically
- No hardcoded chambers in .tscn files

### 4. Banter is Context-Aware
Each choice triggers banter from the choosing Dreamweaver (approval) and potentially opponents (dissent).

**Solution:**
- `InterludioOption.banter.approve` plays if player aligns with that Dreamweaver
- `InterludioOption.banter.dissent[]` plays opposing Dreamweaver reactions
- `InterludioBeat.ShowBanter()` selects banter based on player's alignment vector and choice

### 5. Reusability Across Stages
The infrastructure supports future stages (Stage 3-5) with different gameplay styles.

**Solution:**
- Keep `BeatSceneBase`, `NarrativeDataLoader`, `StageManifestLoader` generic in infrastructure/
- Create new data structure classes (Stage3NarrativeData, Stage4NarrativeData, etc.) for each stage
- Subclass beat scenes from BeatSceneBase as needed
- Pattern is proven and repeatable

---

## Comparison: Stage 1 vs Stage 2 Architecture

| Component | Stage 1 | Stage 2 |
|-----------|---------|---------|
| **Data File** | `opening.json` (fixed content) | `stage_2.json` (dynamic routing) |
| **Data Structures** | Stage1NarrativeData | Stage2NarrativeData |
| **Director** | None (manifest is linear) | Stage2Director (generates beats dynamically) |
| **Manifest** | 8 fixed beats | Dynamic generation per playthrough |
| **Beat Scenes** | Beat1BootSequence, Beat2OpeningMonologue, ... | Stage2HubBeat, InterludioBeat, ChamberBeat, FinaleBeat |
| **Reusable Beats** | No (each beat is unique) | Yes (InterludioBeat and ChamberBeat reused) |
| **Player Agency** | Linear progression | Branching choices + exploration |
| **Alignment Tracking** | Optional (for future use) | **Active** (influences banter, finale, chamber behavior) |

---

## Next Steps (Priority Order)

1. **Create Stage2NarrativeData.cs** - Deserialize stage_2.json schema
2. **Create Stage2Director.cs** - Implement beat sequence generation logic
3. **Create Stage2HubBeat.cs** - First playable beat
4. **Create InterludioBeat.cs** - Reusable interlude handler
5. **Create Stage2IntegrationTests.cs** - Validate wiring
6. **Create ChamberBeat.cs** - Dungeon exploration logic
7. **Runtime testing in Godot** - Verify player flow end-to-end
8. **Documentation** - STAGE_2_README.md, IMPLEMENTATION_STATUS.md

---

## Synchronization Notes

This document should be updated whenever:
- A TODO item is completed (mark with `[x]`)
- New files are created (add to File Structure section)
- Architecture decisions change (update Key Design Decisions section)
- Phases are added or removed (keep checklist in sync with team's task list)

**Last Updated:** October 22, 2025
**Status:** Ready for Phase 1 → Phase 2 transition
