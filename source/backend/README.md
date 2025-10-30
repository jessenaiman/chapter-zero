# Omega Spiral Scripts Documentation

This directory contains all the C# scripts for the Omega Spiral game, built with Godot 4.5 and .NET 10 and C# 14.

## High‑level components
+-------------------+ +-------------------+ +-------------------+
| Stage‑specific | ----> | Stage Manager | ----> | Godot Scene |
| Director (e.g. | (StageManager) | (ghost_terminal.tscn) |
| GhostCinematic | – Handles scene | – UI root inherits |
| Director) | transitions, | NarrativeUi |
+-------------------+ player state, +-------------------+
validation

### Core Systems

#### GameState.cs
**Purpose**: Global game state management and persistence

**Key Features**:
- Singleton autoload accessible from all scenes
- Save/load functionality using JSON
- Cross-scene state persistence
- Dreamweaver scoring and selection
- Party and inventory management
-
#### StageManager.cs
**Purpose**: Manages scene transitions and state validation

**Key Features**:
- Validates state before allowing scene transitions
- Ensures proper game flow and prerequisites
- Integrates with GameState for persistence

**Public API**:
- `TransitionToScene(scenePath)`: Transitions to new scene with validation
- `ValidateStateForTransition(scenePath)`: Checks if transition is allowed
- `UpdateCurrentScene(sceneId)`: Updates current scene in GameState

### Scene Controllers

#### NarrativeTerminal.cs
**Purpose**: Controls the narrative introduction scene

**Features**:
- Terminal-style text display
- Player name input
- Dreamweaver thread selection
- Story progression

#### PartyCreator.cs
**Purpose**: Character creation and party management

**Features**:
- Character generation with racial modifiers
- Class selection
- Party composition
- Stat calculation

#### TileDungeonController.cs
**Purpose**: Manages tile-based dungeon exploration

**Features**:
- Grid-based movement
- Room generation
- Interactive objects (chests, doors, monsters)
- Dreamweaver alignment choices

#### PixelCombatController.cs
**Purpose**: Turn-based pixel art combat system

**Features**:
- Party vs enemy combat
- Action selection (attack, defend, special)
- Damage calculation
- Victory/defeat conditions

#### AsciiRoomRenderer.cs
**Purpose**: Renders ASCII art dungeon rooms

**Features**:
- Text-based room display
- Interactive object highlighting
- Input handling for room navigation

### Data Models

#### PartyData.cs
**Purpose**: Represents player party and inventory

**Properties**:
- `Members`: List of Character objects
- `Gold`: Currency amount
- `Inventory`: Collection of items

#### Character.cs
**Purpose**: Individual character representation

**Properties**:
- `Name`: Character name
- `Race`: Character race (Human, Elf, Dwarf, etc.)
- `CharacterClass`: Character class (Fighter, Mage, etc.)
- `Stats`: Character statistics

#### CharacterStats.cs
**Purpose**: Character attribute system
**Features**:
- Base stats (Strength, Intelligence, etc.)
- Racial modifiers
- Level progression
- Combat calculations

#### DungeonSequenceData.cs
**Purpose**: Dungeon room and sequence definitions
**Features**:
- Room templates
- Interactive object definitions
- Dreamweaver alignment effects

#### TileDungeonData.cs
**Purpose**: Tile-based dungeon data structures
**Features**:
- Room layouts
- Entity positions
- Navigation data

### Utility Scripts

#### SceneLoader.cs
**Purpose**: Handles scene loading and resource management
**Features**:
- Asynchronous scene loading
- Resource caching
- Error handling

#### NarratorEngine.cs
**Purpose**: Manages narrative text and dialogue
**Features**:
- Text queuing system
- Display timing
- Narrative state tracking

### Enums and Constants

#### Enums.cs
**Purpose**: Game-wide enumerations
**Definitions**:
- `DreamweaverType`: Light, Mischief, Wrath
- `DreamweaverThread`: Hero, Villain, Trickster
- `CharacterRace`: Human, Elf, Dwarf, Orc, etc.
- `CharacterClass`: Fighter, Mage, Rogue, etc.

## Performance Considerations

- JSON loading should complete in <100ms
- Scene transitions should complete in <500ms
- Maintain 60 FPS target
- Minimize allocations in hot paths

## File Structure

```
Source/Scripts/
├── GameState.cs              # Global state management
├── SceneManager.cs           # Scene transitions
├── [Scene]Controller.cs      # Scene-specific logic
├── [Entity]Data.cs          # Data models
├── [Utility].cs             # Helper classes
└── Enums.cs                 # Game constants
```

## Development Notes

- All scripts inherit from Godot classes (Node, Node2D, etc.)
- Use Godot's autoload system for singletons
- Follow C# naming conventions
- Use Godot's resource system for assets
- Implement proper error handling and logging

### Component responsibilities

| Component | Responsibility |
|-----------|----------------|
| **Stage‑specific Director** (e.g. `GhostCinematicDirector`) | *Loads the YAML script*, builds a **handler** (Dreamweaver scoring, visual presets) and calls the **generic `CinematicDirector`**. |
| **`CinematicDirector`** (`source/infrastructure/CinematicDirector.cs`) | *Narrative engine*: iterates `NarrativeScript.Scenes`, forwards lines/choices to the handler, processes command tags (`[GLITCH]`, `[FADE_TO_STABLE]`). |
| **`NarrativeUi`** (`source/ui/narrative/NarrativeUi.cs`) | UI‑only: `EnqueueLineAsync`, `PresentChoicesAsync`, `ApplyCrtPresetAsync`, emits `Ready` / `SequenceComplete`. |
| **`StageManager`** (`source/services/StageManager.cs`) | Global manager: stores `PlayerName`, `DreamweaverThread`, `CurrentSceneIndex`; validates and performs Godot scene transitions (`TransitionToScene`). |
| **`NarrativeDataLoader`** (`source/infrastructure/NarrativeDataLoader.cs`) | Deserialises any stage YAML (`ghost.yaml`, `nethack.yaml`, …) into a `NarrativeScript`. |
| **`NarrativeHandler`** (tiny glue class per stage) | Implements `INarrativeHandler` – connects `NarrativeUi` to `StageManager` and `GameStateSignals` (Dreamweaver scoring, milestones). |

### Typical flow for a stage (e.g. Ghost Terminal)

1. **Director** → `NarrativeDataLoader` loads `ghost.yaml` → `NarrativeScript`.
2. **Director** → Instantiates `ghost_terminal.tscn` (root node = `NarrativeUi`).
3. **Director** → Creates a `NarrativeHandler` that holds references to:
   - The UI (`NarrativeUi`)
   - `StageManager` (for scene changes)
   - `GameStateSignals` (for global events)
4. **Director** → Calls `CinematicDirector.PlayAsync(script, handler)`.
5. **CinematicDirector** → Walks each scene:
   - Sends normal lines to `handler.DisplayLinesAsync`.
   - Handles command tags via `handler.HandleCommandLineAsync`.
   - When a scene has a `question` + `choice`, calls `handler.PresentChoiceAsync`.
   - After a choice, `handler.ProcessChoiceAsync` updates Dreamweaver scores.
6. When the script finishes, `handler.NotifySequenceCompleteAsync` fires the UI signal and the director tells `StageManager` to load the next stage (if any).

---

## How to add a new stage (e.g. Nethack)

1. **Create a YAML script** (`nethack.yaml`) that follows the common schema (`title`, `speaker`, `description`, `scenes` with `lines`, optional `question`, `owner`, `choice`).
2. **Add a new scene file** (`nethack.tscn`) whose root node inherits `NarrativeUi`.
3. **Write a tiny director** (`NethackCinematicDirector`) that:
   ```csharp
   var plan = new NarrativeDataLoader()
                 .Load<NethackCinematicPlan>("res://source/stages/stage_2_nethack/nethack.yaml");
   var ui   = LoadAndInstantiate<NarrativeUi>("res://source/stages/stage_2_nethack/nethack.tscn");
   var handler = new NethackHandler(ui, GetNode<StageManager>(), GetNode<GameStateSignals>());
   await new CinematicDirector().PlayAsync(plan.Script, handler);
   EmitStageComplete();
   ```
