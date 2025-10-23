# Stage Architecture Overview

This document provides visual representations of how all the loader and controller components work together to orchestrate the game's stage progression system.

## 1. Game Level - Stage Selection Flow

```mermaid
graph TD
    A["MainMenu.cs"] -->|loads| B["manifest.json<br/>(Game Level)"]
    B -->|returns List&lt;ManifestStage&gt;| A
    A -->|displays stages| C["Stage Buttons"]
    C -->|user clicks| D["Stage 1, 2, 3, etc."]
    D -->|transition to| E["Scene: Stage Scene Path"]
    
    style B fill:#FFE6E6
    style A fill:#E6F3FF
    style E fill:#E6FFE6
```

**What happens:**
- `MainMenu` uses `ManifestLoader` to load `/source/data/manifest.json`
- Manifest contains list of all 5 stages with their root scene paths
- When user clicks a stage, MainMenu transitions to that stage's root scene

---

## 2. Stage Level - Beat Progression Flow

```mermaid
graph TD
    A["Stage Root Scene<br/>(e.g., opening.tscn)"] -->|has root node| B["StageController Subclass<br/>(e.g., Stage1Controller)"]
    B -->|_Ready| C["LoadStageManifest"]
    C -->|loads| D["stage_manifest.json<br/>(Stage Level)"]
    D -->|returns StageManifest| B
    B -->|StageManifest.Scenes| E["List of Beat Scenes"]
    E -->|First scene| F["Beat Scene 1"]
    F -->|BeatSceneBase._Ready| G["Load Current Scene Config"]
    F -->|user completes beat| H["Signal OnBeatComplete"]
    H -->|StageController receives| I["Advance to Next Beat"]
    I -->|Scene transition| J["Beat Scene 2"]
    J -->|last beat completes| K["OnStageCompleteAsync"]
    K -->|report scores| L["GameState"]
    L -->|transition to| M["Next Stage"]
    
    style B fill:#FFE6E6
    style D fill:#FFE6E6
    style F fill:#E6FFE6
    style K fill:#FFE6CC
    style L fill:#E6F3FF
```

**What happens:**
1. Stage root scene loads with `StageController` subclass as root node
2. `StageController._Ready()` calls `LoadStageManifest()` 
3. `StageManifestLoader` loads `stage_manifest.json` from that stage's data folder
4. Manifest defines all beat scenes in order for that stage
5. Each beat scene inherits from `BeatSceneBase`
6. When a beat completes, it signals back to `StageController`
7. `StageController` advances to next beat (or completes stage)
8. On stage completion, scores are reported to `GameState`

---

## 3. Scene Flow Loaders (Legacy vs Current)

```mermaid
graph LR
    A["StageController.cs"] -->|needs beat progression| B{Which Manifest?}
    B -->|Modern approach| C["StageManifestLoader"]
    B -->|Legacy approach| D["SceneFlowLoader"]
    
    C -->|loads| E["stage_manifest.json<br/>(one per stage)"]
    D -->|loads| F["scene_flow.json<br/>(one per stage)"]
    
    E -->|returns| G["StageManifest<br/>- stageId<br/>- stageName<br/>- scenes[]<br/>- nextStagePath"]
    F -->|returns| H["StageSceneFlow<br/>- stageName<br/>- stageId<br/>- scenes[]"]
    
    G -.->|used by| I["Stage1Controller"]
    H -.->|used by| J["SceneNavigator<br/>(Legacy)"]
    
    style C fill:#E6FFE6
    style E fill:#E6FFE6
    style D fill:#FFCCCC
    style F fill:#FFCCCC
```

**Key Difference:**
- **`StageManifestLoader` + `stage_manifest.json`** = Modern, current approach
- **`SceneFlowLoader` + `scene_flow.json`** = Legacy approach, still in codebase
- Both do the same thing (define beat progression), different JSON format

---

## 4. Complete Architecture: Game → Stage → Beat

```mermaid
graph TD
    subgraph "Game Level (MainMenu)"
        ML["ManifestLoader"]
        MF["manifest.json"]
        MM["MainMenu.cs"]
    end
    
    subgraph "Stage Level (StageController)"
        SC["StageController<br/>(Base Class)"]
        S1C["Stage1Controller"]
        S2C["EchoHub<br/>: StageController"]
        S3C["Stage3Controller<br/>(TODO)"]
        S4C["Stage4Controller"]
    end
    
    subgraph "Beat Level (Individual Scenes)"
        BSB["BeatSceneBase<br/>(Base Class)"]
        SML["StageManifestLoader"]
        SMF["stage_manifest.json<br/>(each stage)"]
        BS1["Beat Scene 1"]
        BS2["Beat Scene 2"]
        BSN["Beat Scene N"]
    end
    
    subgraph "Global State"
        GS["GameState"]
    end
    
    MM -->|loads| MF
    MF -->|returns stages| ML
    ML -->|returns List| MM
    MM -->|transitions to| S1C
    
    S1C -->|inherits| SC
    S2C -->|inherits| SC
    S4C -->|inherits| SC
    
    SC -->|_Ready loads| SML
    SML -->|loads| SMF
    SMF -->|returns manifest| SC
    SC -->|instantiates beats| BSB
    
    BSB -->|_Ready loads| SML
    BSB -->|used by| BS1
    BSB -->|used by| BS2
    BSB -->|used by| BSN
    
    BS1 -->|OnBeatComplete signal| SC
    SC -->|advance beat| BS2
    BSN -->|OnStageComplete| SC
    SC -->|report scores| GS
    
    style MF fill:#FFE6E6
    style SMF fill:#FFE6E6
    style SC fill:#E6F3FF
    style BSB fill:#E6FFE6
    style GS fill:#E6F3FF
```

---

## 5. Loader Responsibility Matrix

| Loader | Loads | Returns | Used By | Scope |
|--------|-------|---------|---------|-------|
| **ManifestLoader** | `/source/data/manifest.json` | `List<ManifestStage>` | `MainMenu.cs` | **GAME LEVEL** - Stage selection |
| **StageManifestLoader** | `stage_manifest.json` (each stage) | `StageManifest` | `StageController` | **STAGE LEVEL** - Beat progression within a stage |
| **SceneFlowLoader** | `scene_flow.json` (each stage) | `StageSceneFlow` | `SceneNavigator` | **STAGE LEVEL** - Legacy beat progression |
| **NarrativeDataLoader** | `narrative_data.json` (Stage 1) | `NarrativeSceneData` | `NarrativeSceneDirector` | **BEAT LEVEL** - Dialogue/narrative content within a beat scene |

---

## 6. StageController Hierarchy (Target State)

```mermaid
graph TD
    SC["StageController<br/>(Abstract Base)"]
    S1["Stage1Controller"]
    S2["EchoHub"]
    S3["Stage3Controller<br/>(TODO)"]
    S4["Stage4Controller"]
    S5["Stage5Controller<br/>(TODO)"]
    
    SC -->|implements| M1["OnStageInitializeAsync"]
    SC -->|implements| M2["OnStageCompleteAsync"]
    SC -->|provides| M3["LoadStageManifest"]
    SC -->|provides| M4["AdvanceToNextBeat"]
    
    S1 -->|extends| SC
    S2 -->|extends| SC
    S3 -->|extends| SC
    S4 -->|extends| SC
    S5 -->|extends| SC
    
    S1 -.->|✅ Already done| SC
    S2 -.->|✅ Recently refactored| SC
    S3 -.->|❌ TODO| SC
    S4 -.->|⏳ Partially done| SC
    S5 -.->|❌ Not started| SC
    
    style SC fill:#E6F3FF
    style S1 fill:#CCE6CC
    style S2 fill:#CCE6CC
    style S3 fill:#FFCCCC
    style S4 fill:#FFFFCC
    style S5 fill:#FFCCCC
```

---

## 7. Data Flow: User Clicks Stage 1

```mermaid
sequenceDiagram
    participant User
    participant MainMenu
    participant ManifestLoader
    participant Stage1Scene as Stage1<br/>opening.tscn
    participant Stage1Ctrl as Stage1Controller
    participant SML as StageManifestLoader
    participant BeatScene1
    participant GameState

    User->>MainMenu: Click "Stage 1"
    MainMenu->>ManifestLoader: LoadManifest(/source/data/manifest.json)
    ManifestLoader-->>MainMenu: {id:1, path:"res://source/stages/stage_1/opening.tscn", ...}
    MainMenu->>Stage1Scene: TransitionTo(path)
    
    Stage1Scene->>Stage1Ctrl: _Ready()
    Stage1Ctrl->>SML: LoadManifest(stage_manifest.json)
    SML-->>Stage1Ctrl: StageManifest{scenes: [Boot, Monologue, Q1, Q2, ...]}
    
    Stage1Ctrl->>BeatScene1: Instantiate(Boot scene)
    BeatScene1->>BeatScene1: _Ready()
    BeatScene1-->>User: [Display Boot sequence]
    
    User->>BeatScene1: [Complete interaction]
    BeatScene1->>Stage1Ctrl: EmitSignal("OnBeatComplete")
    Stage1Ctrl->>Stage1Ctrl: AdvanceToNextBeat()
    
    Stage1Ctrl->>BeatScene1: QueueFree()
    
    Note over Stage1Ctrl: ... repeat for Monologue, Q1, Q2, Q3 ...
    
    BeatScene1->>Stage1Ctrl: EmitSignal("OnStageComplete")
    Stage1Ctrl->>GameState: UpdateDreamweaverScores(tracker)
    Stage1Ctrl->>MainMenu: TransitionToNextStage()
```

---

## 8. File Organization

```
/source/
├── /data/
│   ├── manifest.json ............................ GAME LEVEL
│   └── /stages/
│       ├── /stage_1/
│       │   ├── stage_manifest.json
│       │   └── /narrative_data/
│       │       └── narrative_data.json
│       ├── /stage_2/
│       │   └── stage_manifest.json
│       ├── /stage_3/
│       │   └── stage_manifest.json
│       ├── /stage_4/
│       │   └── stage_manifest.json
│       └── /stage_5/
│           └── stage_manifest.json
│
├── /stages/
│   ├── /stage_1/
│   │   ├── opening.tscn (root: Stage1Controller)
│   │   ├── Stage1Controller.cs
│   │   └── /scenes/
│   │       ├── boot_sequence.tscn (root: BeatSceneBase)
│   │       ├── opening_monologue.tscn (root: BeatSceneBase)
│   │       └── ...
│   ├── /stage_2/
│   │   ├── echo_hub.tscn (root: EchoHub : StageController)
│   │   ├── EchoHub.cs
│   │   └── /scenes/
│   │       └── ...
│   ├── /stage_3/
│   │   ├── stage_3_main.tscn
│   │   ├── Stage3Controller.cs (TODO)
│   │   └── /scenes/
│   │       └── ...
│   ├── /stage_4/
│   │   ├── stage_4_main.tscn
│   │   ├── Stage4Controller.cs
│   │   ├── Stage4BeatExecutor.cs
│   │   ├── Stage4PartyService.cs
│   │   ├── Stage4AffinityService.cs
│   │   └── /scenes/
│   │       └── ...
│   └── /stage_5/
│       └── (empty - not yet implemented)
│
└── /scripts/
    └── /infrastructure/
        ├── ManifestLoader.cs ..................... GAME LEVEL
        ├── StageController.cs ................... STAGE LEVEL (base)
        ├── StageManifestLoader.cs ............... STAGE LEVEL
        ├── SceneFlowLoader.cs ................... STAGE LEVEL (legacy)
        ├── BeatSceneBase.cs ..................... BEAT LEVEL (base)
        ├── NarrativeDataLoader.cs ............... BEAT LEVEL (Stage 1 specific)
        └── GameState.cs ......................... GLOBAL
```

---

## 9. Quick Reference: What Loads What

**At Game Start:**
```
MainMenu.cs 
  → ManifestLoader.LoadManifest("/source/data/manifest.json")
    → Returns: [Stage 1, Stage 2, Stage 3, Stage 4, Stage 5]
```

**When Stage 1 Loads:**
```
Stage1Controller._Ready() 
  → StageManifestLoader.LoadManifest("res://source/stages/stage_1/stage_manifest.json")
    → Returns: StageManifest with scenes=[Boot, Monologue, Q1, Q2, Q3, Secret, SecretReveal, Name, ThreadLock]
```

**When Boot Scene Loads:**
```
BootSequence._Ready() (extends BeatSceneBase)
  → StageManifestLoader.LoadManifest(StageManifestPath)
    → Returns: Current scene config for "Boot" beat
  → NarrativeDataLoader.LoadNarrativeData(narrativeDataPath)
    → Returns: Dialogue and scene data for Boot scene
```

---

## 10. Refactoring Status

| Stage | Root Scene | Controller Class | Status | Notes |
|-------|-----------|------------------|--------|-------|
| **1** | `opening.tscn` | `Stage1Controller` | ✅ **DONE** | Correctly uses `StageController` pattern |
| **2** | `echo_hub.tscn` | `EchoHub : StageController` | ✅ **DONE** | Recently refactored to inherit from `StageController` |
| **3** | `stage_3_main.tscn` | `Stage3Main` (Node2D) | ❌ **TODO** | Needs to become `Stage3Controller : StageController` |
| **4** | `stage_4_main.tscn` | `Stage4Controller` | ⏳ **PARTIAL** | Controller created, old `Stage4Main.cs` corrupted, needs cleanup |
| **5** | (Not started) | (Not started) | ❌ **TODO** | Folder exists but not implemented |

---

## 11. Key Insight: Three Levels of Orchestration

```
┌─────────────────────────────────────────────────────────────┐
│  LEVEL 1: GAME ORCHESTRATION (MainMenu)                     │
│  ─────────────────────────────────────────────────────────  │
│  Uses: ManifestLoader                                        │
│  Loads: manifest.json (defines 5 stages)                     │
│  Decides: Which stage to load                                │
│  Responsible: Stage selection UI, transitions to stage root  │
└─────────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────┐
│  LEVEL 2: STAGE ORCHESTRATION (StageController)             │
│  ─────────────────────────────────────────────────────────  │
│  Uses: StageManifestLoader                                   │
│  Loads: stage_manifest.json (defines beats within stage)     │
│  Decides: Which beat scene to load, when to advance          │
│  Responsible: Beat progression, score accumulation, next     │
│               stage transition                               │
└─────────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────┐
│  LEVEL 3: BEAT ORCHESTRATION (BeatSceneBase)                │
│  ─────────────────────────────────────────────────────────  │
│  Uses: NarrativeDataLoader, other beat-specific loaders      │
│  Loads: Beat-specific data (dialogue, choices, etc.)         │
│  Decides: How to present content, when beat is complete      │
│  Responsible: User interaction, beat presentation            │
└─────────────────────────────────────────────────────────────┘
```

This is the **separation of concerns** architecture that makes each level only care about its own orchestration!
