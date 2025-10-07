# Feature Specification: Ωmega Spiral Game Implementation

**Feature Branch**: `002-using-godot-4`
**Created**: 2025-10-07
**Status**: Draft
**Input**: User description: "using godot 4.5 and dotnet 10 RC"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Narrative Terminal Scene (Priority: P1)

As a player, I want to experience a Godot-driven narrative terminal interface where I make choices that determine my Dreamweaver alignment (Hero, Shadow, or Ambition), so I can feel immersed in a retro computing aesthetic handled entirely within the game.

**Why this priority**: This establishes the game's unique identity, narrative framework, and player agency. Without this core experience, there's no game.

**Independent Test**: Can be fully tested by running the narrative terminal scene inside Godot, making thread choices, answering questions, and verifying correct narrative progression and state updates without relying on hardcoded text.

**Acceptance Scenarios**:

1. **Given** the game starts, **When** the narrative terminal loads, **Then** the interface renders via Godot (typewriter effect, cursor, input echo) and remains responsive
2. **Given** I'm presented with Dreamweaver choices, **When** I select Hero/Shadow/Ambition, **Then** the game state updates, loads the corresponding JSON data variation, and stores the chosen thread
3. **Given** I'm in the story flow, **When** I answer questions and provide my name, **Then** the narrative adapts based on JSON configuration and persists the responses in global state
4. **Given** I complete the narrative scene, **When** the scene ends, **Then** it transitions to the NetHack room scene using Godot's scene tree with the correct thread metadata

---

### User Story 2 - NetHack ASCII Dungeon Exploration (Priority: P1)

As a player, I want to explore three sequential ASCII dungeon rooms where each room is owned by a different Dreamweaver and contains three interactive objects (door, monster, chest) that influence Dreamweaver alignment scoring, so I can experience a narrative alignment test disguised as classic roguelike gameplay.

**Why this priority**: This provides the core gameplay loop and Dreamweaver alignment mechanic that drives narrative branching throughout the game.

**Independent Test**: Can be fully tested by navigating each dungeon's ASCII grid, interacting with objects (door/monster/chest), verifying Dreamweaver scoring, and confirming the final Dreamweaver selection.

**Acceptance Scenarios**:

1. **Given** I enter Light's dungeon, **When** I see the three objects (door/monster/chest), **Then** they are positioned in left/mid/right zones with proper ASCII representation
2. **Given** I'm in a dungeon, **When** I move toward an object and interact, **Then** the aligned Dreamweaver speaks with their interpretation and alignment points are awarded
3. **Given** I choose an object aligned with the dungeon owner, **When** the choice is made, **Then** that Dreamweaver receives +2 points (bonus for harmony)
4. **Given** I choose an object aligned with another Dreamweaver, **When** the choice is made, **Then** that other Dreamweaver receives +1 point (cross-alignment)
5. **Given** I complete all three dungeons, **When** scoring finishes, **Then** the highest-scoring Dreamweaver is selected and revealed as the player's guide

---

### User Story 3 - Wizardry Party Character Creation (Priority: P2)

As a player, I want to create a party of fantasy characters with classes, races, and stats using classic CRPG UI patterns, so I can feel like I'm building a proper adventuring party.

**Why this priority**: This provides character customization and ties into the classic CRPG aesthetic, enhancing player investment and replayability.

**Independent Test**: Can be fully tested by selecting party members, assigning classes/races, viewing stats, and verifying the party data is correctly saved and persisted.

**Acceptance Scenarios**:

1. **Given** I'm in party creation, **When** I select 3 party members, **Then** I can assign classes (Fighter/Mage/Priest/etc.) and races (Human/Elf/Dwarf/etc.)
2. **Given** I have party members, **When** I view their stats, **Then** I see STR/INT/WIS/DEX/CON/CHR values calculated correctly
3. **Given** my party is complete, **When** I confirm, **Then** the party data is saved and the scene transitions to the Eye of the Beholder dungeon
4. **Given** I return to party creation, **When** I load previous data, **Then** my party selections are restored

---

### User Story 4 - Eye of the Beholder 2D Tile Dungeon (Priority: P2)

As a player, I want to explore a dungeon level with 2D tile-based layout in the style of Final Fantasy 1-3, so I can experience classic dungeon exploration with proper navigation and interactions.

**Why this priority**: This provides the environmental storytelling and exploration mechanics that complete the retro gaming experience.

**Independent Test**: Can be fully tested by navigating the tile map, interacting with doors and objects, using UI panels, and verifying correct scene transitions.

**Acceptance Scenarios**:

1. **Given** I'm in the dungeon, **When** I use arrow keys, **Then** I can move through the 2D tile grid with proper collision detection
2. **Given** I face a door tile, **When** I press space, **Then** the door opens and I can proceed if conditions are met
3. **Given** I have UI panels enabled, **When** I check inventory/map/stats, **Then** they display correctly and update in real-time
4. **Given** I reach the exit, **When** the condition is met, **Then** the scene transitions to the Final Fantasy combat scene

---

### User Story 5 - Final Fantasy Turn-Based Combat (Priority: P2)

As a player, I want to engage in turn-based combat with pixel art sprites and retro sound effects using classic JRPG mechanics, so I can experience satisfying battle resolution.

**Why this priority**: This provides the combat resolution system and completes the retro gaming experience with familiar mechanics.

**Independent Test**: Can be fully tested by initiating combat, selecting actions, executing turns, and verifying win/loss conditions and rewards.

**Acceptance Scenarios**:

1. **Given** combat starts, **When** I see the enemy sprite, **Then** HP bars and action menu (FIGHT/MAGIC/ITEM/RUN) display correctly
2. **Given** it's my turn, **When** I select FIGHT, **Then** damage is calculated and applied to the enemy
3. **Given** I select MAGIC, **When** I choose a spell, **Then** MP is consumed and effects are applied
4. **Given** I defeat the enemy, **When** HP reaches 0, **Then** victory text shows and shards/rewards are awarded
5. **Given** I lose combat, **When** my HP reaches 0, **Then** game over screen appears with restart options

---

### User Story 6 - Cross-Scene State Management (Priority: P3)

As a player, I want my choices, inventory, and progress to persist across all game scenes, so I can experience a cohesive adventure with lasting consequences.

**Why this priority**: This enables the narrative branching and progression systems that make the game replayable and meaningful.

**Independent Test**: Can be fully tested by playing through multiple scenes, verifying state persistence, and checking that choices affect subsequent scenes.

**Acceptance Scenarios**:

1. **Given** I chose a Dreamweaver thread, **When** I progress through scenes, **Then** the thread influences narrative and scene variations
2. **Given** I collected shards, **When** I check inventory, **Then** they persist across scene transitions
3. **Given** I created a party, **When** I return to party view, **Then** the characters are available in combat and exploration
4. **Given** I complete scenes, **When** I restart, **Then** I can resume from any completed scene

## Clarifications

### Session 2025-10-07

- Q: What are the actual mechanics for the NetHack dungeon scene? → A: Three sequential dungeons owned by different Dreamweavers (Light/Mischief/Wrath), each containing three interactive objects (door/monster/chest) aligned with Dreamweavers, scoring system (+2 for owner match, +1 for cross-alignment), no shards collected
- Q: Include Detailed JSON Schemas → A: Specs should include multiple variations of the sample data to ensure the tests are not locked to content. DO NOT CODE tests to CONTENT, always test against user interactions and expectations
- Q: Add File Structure and Manifest → A: We need to update unified-blueprint to use godot and not mention a DOS terminal, and godot should handle everything, so option A - The Eye of the Beholder scene is now a dungeon level that is the same rough shape, perhaps bigger in a final fantasy 1-3 2d tile map
- Q: Include Global State Definitions → A: Refactor all typescript to C# and godot where terminal or phaser was mentioned. We must do that for accuracy in our documentation
- Q: Add Runtime Flow Details → A: Use godot where libraries exist and C# where we need custom logic
- Q: Include Extensive Test Cases → A: Write new test cases based on the accurately rewritten unified-blueprint, spec and plan
- Q: How should scene transitions handle data passing between scenes? → A: Each scene explicitly saves its complete state to GameState before transitioning, enabling C# application API calls and dynamic content generation for next scenes
- Q: How should JSON schema validation failures be handled in the user experience? → A: Show detailed error messages to help developers debug content issues (demo environment with extensive debug content)
- Q: How should game assets (sprites, audio, fonts) be organized relative to JSON data? → A: Use Godot's optimal resource system for assets with unified folder structure and dynamic scene generation
- Q: How should input handling be architected across different scene types? → A: Use Godot's Input Map system with scene-specific action overrides for flexibility
- Q: When should game state be automatically saved during gameplay? → A: At the end of each scene automatically to preserve progress at natural breakpoints

## Data Model

### JSON Schemas and Sample Variations

All scene data is defined in JSON files loaded by Godot. Tests must validate user interactions and expectations, not specific content. Below are schema definitions with multiple sample variations to ensure flexibility.

#### Scene 1: Narrative Terminal (Godot-Handled Interface)

**Schema**: `scenes/scene1_narrative/schema.json`

```json
{
  "type": "narrative_terminal",
  "openingLines": ["string"],
  "initialChoice": {
    "prompt": "string",
    "options": [{"id": "string", "label": "string", "description": "string"}]
  },
  "storyBlocks": [
    {
      "paragraphs": ["string"],
      "question": "string",
      "choices": [{"text": "string", "nextBlock": "number"}]
    }
  ],
  "namePrompt": "string",
  "secretQuestion": {
    "prompt": "string",
    "options": ["string"]
  },
  "exitLine": "string"
}
```

#### Sample Variation 1: Hero Thread

```json
{
  "type": "narrative_terminal",
  "openingLines": [
    "Once, there was a name.",
    "Not written in stone or spoken in halls—but remembered in the silence between stars.",
    "I do not know when I heard it. Time does not pass here.",
    "But I have held it.",
    "And now… I hear it again."
  ],
  "initialChoice": {
    "prompt": "If you could live inside one kind of story, which would it be?",
    "options": [
      {"id": "hero", "label": "HERO", "description": "A tale where one choice can unmake a world"},
      {"id": "shadow", "label": "SHADOW", "description": "A tale that hides its truth until you bleed for it"},
      {"id": "ambition", "label": "AMBITION", "description": "A tale that changes every time you look away"}
    ]
  },
  "storyBlocks": [
    {
      "paragraphs": [
        "In a city built on broken promises, a child stood at the edge of a bridge that led nowhere.",
        "They held a key made of glass—and everyone warned them: 'Don't cross. The bridge isn't real.'",
        "But the child knew something no one else did…"
      ],
      "question": "What did the child know?",
      "choices": [
        {"text": "The bridge appears only when you stop believing in it.", "nextBlock": 1},
        {"text": "The key wasn't for the bridge—it was for the lock inside them.", "nextBlock": 1}
      ]
    }
  ],
  "namePrompt": "What is your name?",
  "secretQuestion": {
    "prompt": "Omega asks: Can you keep a secret?",
    "options": ["yes", "no", "only if you keep one for me"]
  },
  "exitLine": "Reality is a story that forgot it was being written. And we—are the ones remembering."
}
```

#### Sample Variation 2: Shadow Thread

```json
{
  "type": "narrative_terminal",
  "openingLines": [
    "The shadows remember everything you forget.",
    "They whisper in the corners of your mind.",
    "They know the names you buried.",
    "They keep the secrets you denied.",
    "And now… they call to you."
  ],
  "initialChoice": {
    "prompt": "What darkness calls to you most?",
    "options": [
      {"id": "hero", "label": "HERO", "description": "The darkness of sacrifice and lost light"},
      {"id": "shadow", "label": "SHADOW", "description": "The darkness that hides within itself"},
      {"id": "ambition", "label": "AMBITION", "description": "The darkness of endless hunger"}
    ]
  },
  "storyBlocks": [
    {
      "paragraphs": [
        "In the deepest shadow, a figure waited.",
        "Not for rescue, not for light.",
        "But for someone who understood the comfort of darkness.",
        "The figure held out a hand, and in its palm was a single, burning ember."
      ],
      "question": "What did you see in the ember?",
      "choices": [
        {"text": "The last light before eternal night.", "nextBlock": 1},
        {"text": "A reflection of my own hidden fire.", "nextBlock": 1}
      ]
    }
  ],
  "namePrompt": "What name do the shadows call you?",
  "secretQuestion": {
    "prompt": "Can you face what hides in the dark?",
    "options": ["yes", "no", "the dark is where I belong"]
  },
  "exitLine": "Some stories are written in shadow. And some shadows… write themselves."
}
```

#### Scene 2: NetHack Dungeon Sequence

**Schema**: `scenes/scene2_nethack/schema.json`

```json
{
  "type": "ascii_dungeon_sequence",
  "dungeons": [
    {
      "owner": "string",
      "map": ["string"],
      "legend": {"string": "string"},
      "objects": {
        "D": {"type": "door", "text": "string", "aligned_to": "string"},
        "M": {"type": "monster", "text": "string", "aligned_to": "string"},
        "C": {"type": "chest", "text": "string", "aligned_to": "string"}
      }
    }
  ]
}
```

#### Sample Variation 1: Standard Layout

```json
{
  "type": "ascii_dungeon_sequence",
  "dungeons": [
    {
      "owner": "light",
      "map": [
        "########################",
        "#......................#",
        "#.@....................#",
        "#........D...M...C....#",
        "#......................#",
        "########################"
      ],
      "legend": {
        "@": "player",
        ".": "floor",
        "#": "wall",
        "D": "door",
        "M": "monster",
        "C": "chest"
      },
      "objects": {
        "D": {"type": "door", "text": "What is the first story you ever loved?", "aligned_to": "light"},
        "M": {"type": "monster", "text": "A spectral wolf appears! It lunges...", "aligned_to": "wrath"},
        "C": {"type": "chest", "text": "You open the chest. Inside: a broken compass.", "aligned_to": "mischief"}
      }
    }
  ]
}
```

#### Sample Variation 2: Alternative Layout

```json
{
  "type": "ascii_dungeon_sequence",
  "dungeons": [
    {
      "owner": "light",
      "map": [
        "########################",
        "#..~..~..~..~..~..~..#",
        "#.@....................#",
        "#........C...D...M....#",
        "#..~..~..~..~..~..~..#",
        "########################"
      ],
      "legend": {
        "@": "player",
        ".": "floor",
        "#": "wall",
        "~": "water",
        "D": "door",
        "M": "monster",
        "C": "chest"
      },
      "objects": {
        "D": {"type": "door", "text": "Is chaos kinder than order?", "aligned_to": "mischief"},
        "M": {"type": "monster", "text": "A guardian of light blocks your path!", "aligned_to": "light"},
        "C": {"type": "chest", "text": "The chest giggles. It's empty... or is it?", "aligned_to": "wrath"}
      }
    }
  ]
}
```

#### Scene 3: Wizardry Party Creation

**Schema**: `scenes/scene3_wizardry/schema.json`

```json
{
  "type": "wizardry_party",
  "title": "string",
  "classes": ["string"],
  "races": ["string"],
  "stats": ["string"],
  "confirmText": "string"
}
```

#### Sample Variation 1: Standard Party

```json
{
  "type": "wizardry_party",
  "title": "CREATE YOUR PARTY (3 MEMBERS)",
  "classes": ["Fighter", "Mage", "Priest", "Thief", "Bard"],
  "races": ["Human", "Elf", "Dwarf", "Gnome"],
  "stats": ["STR", "INT", "WIS", "DEX", "CON", "CHR"],
  "confirmText": "BIND SOULS TO THE SPIRAL?"
}
```

#### Sample Variation 2: Expanded Options

```json
{
  "type": "wizardry_party",
  "title": "FORGE YOUR LEGEND (4 MEMBERS)",
  "classes": ["Fighter", "Mage", "Priest", "Thief", "Bard", "Paladin", "Ranger"],
  "races": ["Human", "Elf", "Dwarf", "Gnome", "Halfling", "Half-Elf"],
  "stats": ["STR", "INT", "WIS", "DEX", "CON", "CHR", "LCK"],
  "confirmText": "SEAL YOUR DESTINY?"
}
```

#### Scene 4: Eye of the Beholder 2D Tile Dungeon

**Schema**: `scenes/scene4_eye_beholder/schema.json`

```json
{
  "type": "tile_dungeon",
  "tilemap": ["string"],
  "legend": {"string": "string"},
  "uiPanels": {
    "inventory": "boolean",
    "map": "boolean",
    "stats": ["string"]
  },
  "controls": "string",
  "exitCondition": "string"
}
```

#### Sample Variation 1: Basic Dungeon

```json
{
  "type": "tile_dungeon",
  "tilemap": [
    "####################",
    "#..................#",
    "#.@..............D.#",
    "#..................#",
    "#..................#",
    "#..................#",
    "####################"
  ],
  "legend": {
    "#": "wall",
    ".": "floor",
    "@": "player",
    "D": "door"
  },
  "uiPanels": {
    "inventory": true,
    "map": false,
    "stats": ["HP: 24/24", "MP: 8/8"]
  },
  "controls": "Use ARROW KEYS to navigate. SPACE to interact.",
  "exitCondition": "open_door"
}
```

#### Sample Variation 2: Complex Dungeon

```json
{
  "type": "tile_dungeon",
  "tilemap": [
    "########################",
    "#......................#",
    "#.@..................D.#",
    "#......................#",
    "#.............##########",
    "#.............#........#",
    "#.............#........#",
    "#.............##########",
    "#......................#",
    "########################"
  ],
  "legend": {
    "#": "wall",
    ".": "floor",
    "@": "player",
    "D": "door",
    "K": "key",
    "T": "treasure"
  },
  "uiPanels": {
    "inventory": true,
    "map": true,
    "stats": ["HP: 30/30", "MP: 12/12", "Keys: 0"]
  },
  "controls": "ARROW KEYS: Move | SPACE: Interact | I: Inventory | M: Map",
  "exitCondition": "find_treasure"
}
```

#### Scene 5: Final Fantasy Combat

**Schema**: `scenes/scene5_ff_combat/schema.json`

```json
{
  "type": "pixel_combat",
  "playerSprite": "string",
  "enemy": {
    "name": "string",
    "hp": "number",
    "sprite": "string"
  },
  "actions": ["string"],
  "music": "string",
  "victoryText": "string"
}
```

#### Sample Variation 1: Basic Combat

```json
{
  "type": "pixel_combat",
  "playerSprite": "warrior_16x16.png",
  "enemy": {
    "name": "Glitch Golem",
    "hp": 45,
    "sprite": "golem_16x16.png"
  },
  "actions": ["FIGHT", "MAGIC", "ITEM", "RUN"],
  "music": "ff1_battle_theme.chip",
  "victoryText": "The golem dissolves into raw code. A shard remains."
}
```

#### Sample Variation 2: Boss Combat

```json
{
  "type": "pixel_combat",
  "playerSprite": "hero_16x16.png",
  "enemy": {
    "name": "Omega Guardian",
    "hp": 120,
    "sprite": "guardian_32x32.png"
  },
  "actions": ["ATTACK", "SPELL", "SUMMON", "DEFEND"],
  "music": "epic_boss_theme.chip",
  "victoryText": "The Guardian falls. The Spiral stabilizes."
}
```

## Project Architecture

### File Structure (Godot + C#)

```
/Source
├── /Scenes
│   ├── Scene1Narrative.tscn            # Godot scene loading narrative terminal UI
│   ├── Scene2NethackSequence.tscn      # ASCII dungeon renderer scene
│   ├── Scene3WizardryParty.tscn        # Party creation UI
│   ├── Scene4TileDungeon.tscn          # 2D tile-based dungeon
│   └── Scene5PixelCombat.tscn          # Turn-based combat scene
├── /Scripts
│   ├── SceneLoader.cs                  # Loads scene JSON and instantiates scenes
│   ├── SceneManager.cs                 # Handles transitions and state persistence
│   ├── NarrativeTerminal.cs            # Godot terminal renderer + interaction logic
│   ├── AsciiRoomRenderer.cs            # ASCII grid rendering and movement handling
│   ├── PartyCreator.cs                 # Wizardry-style party builder
│   ├── TileDungeonController.cs        # Tile-based dungeon navigation
│   ├── PixelCombatController.cs        # Combat flow controller
│   └── NarratorEngine.cs               # Processes dialogue/choice rules from JSON
├── /Data
│   ├── scenes
│   │   ├── /scene1_narrative
│   │   │   ├── schema.json
│   │   │   ├── hero.json
│   │   │   ├── shadow.json
│   │   │   └── ambition.json
│   │   ├── /scene2_nethack
│   │   │   ├── schema.json
│   │   │   └── room_variants.json      # Array of dungeon layouts
│   │   ├── /scene3_wizardry
│   │   │   ├── schema.json
│   │   │   └── party_variants.json
│   │   ├── /scene4_tile_dungeon
│   │   │   ├── schema.json
│   │   │   └── dungeon_variants.json
│   │   └── /scene5_ff_combat
│   │       ├── schema.json
│   │       └── combat_variants.json
│   └── manifest.json                   # Scene registry + metadata
├── /UI
│   ├── NarrativeTerminal.tscn          # Godot Control-based terminal skin
│   ├── AsciiViewport.tscn
│   ├── PartyCreatorUI.tscn
│   ├── TileDungeonUI.tscn
│   └── PixelCombatUI.tscn
└── /Tests
    ├── NarrativeTerminalTests.cs
    ├── NethackSequenceTests.cs
    ├── PartyCreatorTests.cs
    ├── TileDungeonTests.cs
    └── PixelCombatTests.cs
```

### Scene Manifest (`/Data/manifest.json`)

```json
{
  "scenes": [
    {
      "id": 1,
      "type": "narrative_terminal",
      "path": "scene1_narrative",
      "supportsThreads": true
    },
    {
      "id": 2,
      "type": "ascii_dungeon_sequence",
      "path": "scene2_nethack",
      "supportsThreads": false
    },
    {
      "id": 3,
      "type": "wizardry_party",
      "path": "scene3_wizardry",
      "supportsThreads": false
    },
    {
      "id": 4,
      "type": "tile_dungeon",
      "path": "scene4_tile_dungeon",
      "supportsThreads": false
    },
    {
      "id": 5,
      "type": "pixel_combat",
      "path": "scene5_ff_combat",
      "supportsThreads": false
    }
  ]
}
```

### Godot Scene & Node Responsibilities

- **SceneLoader.cs**: Resolves manifest entries, loads JSON payloads, and instantiates the correct Godot scene.
- **SceneManager.cs**: Lives in an autoload singleton, preserves `GameState`, orchestrates transitions, and emits signals for UI updates.
- **NarrativeTerminal.cs**: Uses Godot Control nodes to display typewriter text, handle input focus, and emit choice events without hardcoded narrative.
- **AsciiRoomRenderer.cs**: Renders ASCII grids via Godot `RichTextLabel`, maps input to movement, and triggers object interactions.
- **TileDungeonController.cs**: Utilises Godot TileMap nodes for FF-style dungeon navigation with collision layers.
- **PixelCombatController.cs**: Manages turn order, action resolution, and animation playback using Godot animation players.
- **NarratorEngine.cs**: Converts JSON story blocks into queued lines, pushing them to UI scenes via signals.

## Global State

Game-wide data is stored in an autoload singleton (`GameState.cs`) to maintain continuity across scenes.

```csharp
public class GameState : Godot.Node
{
    public int CurrentScene { get; set; }
    public string DreamweaverThread { get; set; } = "hero"; // "hero", "shadow", "ambition"
    public string PlayerName { get; set; } = string.Empty;
    public Godot.Collections.Array<string> Shards { get; } = new();
    public Godot.Collections.Dictionary SceneData { get; set; } = new();
    public Godot.Collections.Array<string> NarratorQueue { get; } = new();
    public Godot.Collections.Dictionary<string, int> DreamweaverScores { get; } = new()
    {
        { "light", 0 },
        { "mischief", 0 },
        { "wrath", 0 }
    };
    public Godot.Collections.Dictionary<string, object> PartyData { get; set; } = new();

    public void ResetForNewRun()
    {
        CurrentScene = 1;
        DreamweaverThread = "hero";
        PlayerName = string.Empty;
        Shards.Clear();
        SceneData.Clear();
        NarratorQueue.Clear();
        foreach (var key in DreamweaverScores.Keys)
        {
            DreamweaverScores[key] = 0;
        }
        PartyData.Clear();
    }
}
```

## Runtime Flow (Godot)

1. **Initialization**: `SceneManager` autoload reads `/Data/manifest.json`, loads `scene1_narrative/hero.json` by default (will be swapped when player selects thread).
2. **Narrative Terminal**: `NarrativeTerminal.cs` consumes the JSON story graph, plays typewriter animations, captures inputs, and updates `GameState` via signals.
3. **Thread Selection**: On Dreamweaver choice, `SceneManager` swaps the active JSON file (hero/shadow/ambition) while keeping the scene instance alive.
4. **Scene Transition with State Persistence**: Each scene explicitly saves its complete state to `GameState` before transitioning, enabling C# application API calls and dynamic content generation for subsequent scenes.
5. **Transition to NetHack Sequence**: `SceneManager` loads `Scene2NethackSequence.tscn`, hands it the array of dungeon variants, and resets player position/state per run.
6. **Dungeon Interaction**: `AsciiRoomRenderer.cs` processes movement, detects object tiles, and triggers `NarratorEngine` lines plus scoring updates in `GameState.DreamweaverScores`.
7. **Party Creation**: `Scene3WizardryParty.tscn` loads party schema, allows selections, and stores structured data in `GameState.PartyData`.
8. **Tile Dungeon**: `TileDungeonController.cs` instantiates configured TileMaps, manages collision layers, and signals completion once exit condition is satisfied.
9. **Pixel Combat**: `PixelCombatController.cs` loads combat variants, executes turn-based flow, and outputs combat resolution. On victory, it records the chosen Dreamweaver based on scores.
10. **Result Output**: After combat, `SceneManager` prints a structured JSON summary to stdout/log and prepares data for Scene 3+ follow-ups.

## Functional Requirements

### Core Gameplay Loop

- [ ] Scene progression: Narrative Terminal → NetHack 3-Dungeon Sequence → Wizardry Party → Tile Dungeon → Pixel Combat
- [ ] Dreamweaver thread selection and persistence across scenes
- [ ] Three sequential dungeons with Dreamweaver ownership and object alignments:
  - Light's Dungeon: Door (Light), Monster (Wrath), Chest (Mischief)
  - Mischief's Dungeon: Door (Mischief), Monster (Light), Chest (Wrath)
  - Wrath's Dungeon: Door (Wrath), Monster (Mischief), Chest (Light)
- [ ] Dreamweaver alignment scoring system (+2 for dungeon owner match, +1 for cross-alignment)
- [ ] Final Dreamweaver selection based on total accumulated points
- [ ] Player name input and storage
- [ ] Party creation and management
- [ ] Combat resolution with win/loss states

### Systems & Architecture

- [ ] Godot autoload `SceneManager` maintains `GameState` and manifest-backed scene routing
- [ ] `SceneLoader` resolves manifest entries and loads JSON variants without hardcoding content
- [ ] Each scene script (NarrativeTerminal, AsciiRoomRenderer, PartyCreator, TileDungeonController, PixelCombatController) consumes schema-validated JSON
- [ ] `NarratorEngine` processes dialogue queues and emits signals to active UI scenes
- [ ] Tile Dungeon scene uses Godot TileMap with collision layers for movement and interaction
- [ ] Pixel Combat scene manages turn order via deterministic state machine supporting multiple action lists

### Data Handling & Validation

- [ ] JSON schemas exist for each scene type and are validated on load (failures surface detailed error messages with extensive debug content for demo development)
- [ ] Tests exercise multiple JSON variations per scene to avoid coupling to specific narrative content
- [ ] Dreamweaver scoring persists in `GameState.DreamweaverScores` and reports final chosen guide
- [ ] Party data stored as structured dictionary containing class, race, and stat allocations
- [ ] Combat summaries emitted as structured JSON with scene, scores, and choice history
- [ ] Save/load system serialises `GameState` to disk and restores seamlessly

## Verification & Test Scenarios

Test coverage must target player interactions, state changes, and system expectations—not specific narrative content. Each scenario should run against multiple JSON variants to ensure robustness.

### Narrative Terminal

1. **VT-1**: Loading different thread JSON files renders interface elements correctly and keeps inputs responsive.
2. **VT-2**: Choosing each Dreamweaver option updates `GameState.DreamweaverThread` and reloads the corresponding JSON variation without re-instantiating the scene.
3. **VT-3**: Text entry (name + answers) propagates to `GameState` and persists through scene transitions.
4. **VT-4**: Invalid/missing JSON keys trigger validation errors with user-facing fallback messaging while keeping the game responsive.

### NetHack Dungeon Sequence

5. **VT-5**: Player movement respects ASCII grid collisions across all provided map variants.
6. **VT-6**: Interacting with each object type fires correct Dreamweaver dialogue and scoring adjustments.
7. **VT-7**: Alignment scoring applies +2 for owner matches and +1 for cross-alignment, including tie-breaking logic when totals equal.
8. **VT-8**: Layout randomisation (wall shifts) maintains object zones and does not trap the player.

### Wizardry Party Creation

9. **VT-9**: Class and race selections update stat panels dynamically regardless of option ordering.
10. **VT-10**: Party confirmation serialises structured data and guards against incomplete parties or duplicate slots when not allowed.
11. **VT-11**: Loading a saved party repopulates UI elements and ensures data integrity across Godot sessions.

### Tile Dungeon

12. **VT-12**: TileMap collisions prevent traversal through walls and respond to dynamic obstacles (e.g., locked doors) in each map variant.
13. **VT-13**: Key/treasure interactions satisfy exit conditions and trigger correct scene transitions.
14. **VT-14**: UI panels toggle on/off without frame drops, and map overlays reflect the player's discovered tiles.

### Pixel Combat

15. **VT-15**: Turn order alternates correctly between player and enemy actions, and queued animations finish before accepting new input.
16. **VT-16**: Action outcomes (damage, buffs, items, run attempts) update state consistently across combat variants.
17. **VT-17**: Combat summary JSON includes chosen Dreamweaver, per-scene scores, and action history, enabling downstream scenes to branch.

### Global Systems

18. **VT-18**: Saving and loading `GameState` mid-run preserves scene, thread, scores, party data, and inventory.
19. **VT-19**: Scene transitions complete in under 500ms and clean up unused nodes to avoid memory leaks.
20. **VT-20**: Accessibility audit confirms keyboard-only navigation across all scenes and verifies high-contrast modes where applicable.

### Technical Implementation

- [ ] Godot 4.5 Mono project setup with C# scripting
- [ ] .NET 10 RC runtime integration
- [ ] JSON schema validation for scene data
- [ ] Scene transition system with state preservation
- [ ] Asset loading and management (ASCII art, pixel art, audio)
- [ ] Input handling via Godot's Input Map system with scene-specific action overrides for flexible keyboard navigation
- [ ] Save/load system for game progress

### Performance Requirements

- [ ] 60 FPS gameplay maintained across all scenes
- [ ] Scene transitions under 500ms
- [ ] JSON loading under 100ms
- [ ] Memory usage under 500MB during gameplay
- [ ] No frame drops during combat or navigation

## Non-Functional Requirements

### Compatibility

- [ ] Windows 10+ support
- [ ] Linux support via Godot export
- [ ] Keyboard-only navigation (accessibility)
- [ ] Resolution independence (responsive UI)

### Quality Standards

- [ ] Retro aesthetic consistency across all scenes (fonts, palettes, UI motion handled inside Godot)
- [ ] Authentic recreation of classic game mechanics (terminal, roguelike navigation, party building, tile dungeon, JRPG combat)
- [ ] Comprehensive error handling and logging (detailed schema validation errors with extensive debug content for demo development, asset load failures, state mismatches)
- [ ] Clean C# code following SOLID principles with Godot node lifecycle discipline (ready, process, input)

## Dependencies

### Core Technologies

- [ ] Godot 4.5 with Mono enabled
- [ ] .NET 10 RC SDK
- [ ] C# 14 language features
- [ ] JSON.NET for data handling

### Development Tools

- [ ] Visual Studio 2022 17.12+ (C# 14 support)
- [ ] Godot 4.5 editor
- [ ] NUnit for unit testing

## Constraints

### Technology Stack

- Must use Godot 4.5 Mono runtime
- Must target .NET 10 RC specifically
- Must use C# 14 features where beneficial
- Cannot use older .NET versions

### Development Approach

- Each scene must be independently testable
- Retro authenticity takes precedence over modern conveniences
- Performance requirements are non-negotiable
- Classic game mechanics must be faithfully recreated

## Success Criteria

- [ ] All P1 user stories implemented and independently testable
- [ ] Game runs on Windows and Linux platforms
- [ ] 60 FPS performance maintained across all scenes
- [ ] Retro aesthetic and mechanics feel authentic to classic games
- [ ] Complete gameplay loop from start to combat victory
- [ ] State persistence works across scene transitions
- [ ] Code follows C# best practices and SOLID principles
