# **Ωmega Spiral – Unified Blueprint for Phaser & C# Plans**

This unified blueprint consolidates shared design decisions from the Phaser and C# project plans, using the Phaser plan as the reference for latest revisions. Both plans can reference this for duplicated elements like schemas, state, and interactions. Adaptations for each plan are noted.

---

## **1. Core Design Principles**

- **Narrative as Data**: All dialogue, choices, and scene logic are defined in external JSON files—**never hardcoded**.
- **Per-Scene Schema**: Each scene type (DOS terminal, ASCII dungeon, Wizardry UI, etc.) has its own JSON schema.
- **Dreamweaver Variants**: For scenes supporting narrative branching (e.g., Scene 1), **3 JSON files** exist—one per Dreamweaver thread: **Hero**, **Shadow**, and **Ambition**.
- **API-Ready**: JSON structure mirrors future REST/GraphQL response format (e.g., `/api/scene/1?thread=hero`).
- **Plug-and-Play Scenes**: Adding Scene 5 = drop `scene5_ff_combat.json` + register in scene manifest.

---

## **2. Project Architecture**

### **Shared File Structure**
```
bash
/src (or /Source for C#)
/scenes
├── /scene1_dos_terminal
│   ├── schema.json          # Validation schema (optional)
│   ├── hero.json            # Dreamweaver: Hero
│   ├── shadow.json          # Dreamweaver: Shadow
│   └── ambition.json        # Dreamweaver: Ambition
├── /scene2_nethack_room
│   └── room.json            # Static ASCII layout + shard logic
├── /scene3_wizardry_party
│   └── party.json           # Class/race options
├── /scene4_eye_beholder
│   └── dungeon.json         # First-person wall definitions
└── /scene5_ff_combat
    └── combat.json          # Enemy, actions, pixel art refs
/core
├── SceneLoader.ts (Phaser) / SceneLoader.cs (C#)  # Loads JSON by sceneId + thread
├── SceneManager.ts (Phaser) / SceneManager.cs (C#)  # Manages transitions, state
└── NarratorEngine.ts (Phaser) / NarratorEngine.cs (C#)  # Processes narrator rules from JSON
/ui
├── DOSRenderer.tsx (Phaser) / DOSRenderer.cs (C#)  # xterm.js + typewriter
├── ASCIIRoomRenderer.tsx (Phaser) / ASCIIRoomRenderer.cs (C#)  # Grid-based ASCII
└── ...                      # One renderer per scene type
manifest.json                # Scene registry + metadata
```

### **Phaser Plan Adaptations**

- Use **TypeScript** (.ts/.tsx) instead of JavaScript.
- State managed via Zustand (TypeScript-compatible).
- Renderers use React components with Phaser integration.

### **C# Plan Adaptations**

- Use **C# 10 RC** (installed) for scripting and backend hub.
- State managed via Godot's Node system or custom classes.
- Renderers as Godot UI nodes with C# scripts.
- Integrate with Godot 4.5's Mono runtime.

### **Shared Global State**

```typescript
// Phaser (TypeScript)
interface GameState {
  currentScene: number;
  dreamweaverThread: 'hero' | 'shadow' | 'ambition';
  playerName: string;
  shards: string[];
  sceneData: any; // Loaded from JSON
  narratorQueue: string[]; // Array of narrator lines to speak
}
```

```csharp
// C# (Godot)
public class GameState : Godot.Object {
    public int CurrentScene { get; set; }
    public string DreamweaverThread { get; set; } // "hero", "shadow", "ambition"
    public string PlayerName { get; set; }
    public List<string> Shards { get; set; }
    public Godot.Collections.Dictionary SceneData { get; set; }
    public List<string> NarratorQueue { get; set; }
}
```

---

---

## **3. Scene Schemas & Sample JSON**

### **Scene 1: DOS Terminal (Teletype Story Generator)**

**Schema**: `scene1_dos_terminal/schema.json`

```json
{
  "type": "dos_terminal",
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

**Sample: `scene1_dos_terminal/hero.json`**

```json
{
  "type": "dos_terminal",
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

---

### **Scene 2: NetHack Room**

**File**: `scene2_nethack_room/room.json`

```json
{
  "type": "ascii_dungeon",
  "grid": [
    "#########",
    "#.......#",
    "#.@...*.#",
    "#.......#",
    "#########"
  ],
  "legend": {
    "@": "player",
    ".": "floor",
    "#": "wall",
    "*": "shard"
  },
  "shardMessage": "The shard pulses with forgotten code. Collect it to stabilize the Spiral.",
  "exitTrigger": "shard_collected"
}
```

---

### **Scene 3: Wizardry Party Selection**

**File**: `scene3_wizardry_party/party.json`

```json
{
  "type": "wizardry_ui",
  "title": "CREATE YOUR PARTY (3 MEMBERS)",
  "classes": ["Fighter", "Mage", "Priest", "Thief", "Bard"],
  "races": ["Human", "Elf", "Dwarf", "Gnome"],
  "stats": ["STR", "INT", "WIS", "DEX", "CON", "CHR"],
  "confirmText": "BIND SOULS TO THE SPIRAL?"
}
```

---

### **Scene 4: Eye of the Beholder UI**

**File**: `scene4_eye_beholder/dungeon.json`

```json
{
  "type": "first_person_dungeon",
  "walls": {
    "north": "brick",
    "east": "stone",
    "south": "door",
    "west": "brick"
  },
  "uiPanels": {
    "inventory": true,
    "map": false,
    "stats": ["HP: 24/24", "MP: 8/8"]
  },
  "controls": "Use ARROW KEYS to navigate. SPACE to interact.",
  "exitCondition": "open_south_door"
}
```

---

### **Scene 5: Final Fantasy Combat**

**File**: `scene5_ff_combat/combat.json`

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

---

---

## **4. Scene Manifest (`manifest.json`)**

```json
{
  "scenes": [
    {
      "id": 1,
      "type": "dos_terminal",
      "path": "scene1_dos_terminal",
      "supportsThreads": true
    },
    {
      "id": 2,
      "type": "ascii_dungeon",
      "path": "scene2_nethack_room",
      "supportsThreads": false
    },
    {
      "id": 3,
      "type": "wizardry_ui",
      "path": "scene3_wizardry_party",
      "supportsThreads": false
    },
    {
      "id": 4,
      "type": "first_person_dungeon",
      "path": "scene4_eye_beholder",
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

---

## **5. Runtime Flow**

1. **Player selects thread** in Scene 1 → `dreamweaverThread` set to 'hero', 'shadow', or 'ambition'.
2. **SceneLoader** fetches: `scenes/scene1_dos_terminal/${dreamweaverThread}.json`.
3. **DOSRenderer** processes: `openingLines` → typewriter effect, `storyBlocks` → interactive narrative, `namePrompt` → input with echo.
4. On completion → **SceneManager** loads Scene 2 (`scene2_nethack_room/room.json`).
5. Each subsequent scene loads its **single JSON file** (no thread variants).

---

## **6. Future API Compatibility**

Current JSON structure = **exact shape** of future API response:

```http
GET /api/v1/scene/1?thread=hero
→ 200 { "type": "dos_terminal", "openingLines": [...], ... }
```

No code changes needed—just swap local JSON loading for `fetch()` (Phaser) or `HttpClient` (C#).

---

---

## **7. Benefits of This Architecture**

- ✅ **Narrative writers** edit JSON—no coding required.
- ✅ **3 Dreamweaver variants** (Hero, Shadow, Ambition) = 3 JSON files, zero code duplication.
- ✅ **New scenes** = drop folder + update manifest.
- ✅ **Localization-ready**: swap `hero.json` → `hero_fr.json`.
- ✅ **A/B testing**: serve different JSON variants via API.
- ✅ **Alpha flexibility**: tweak story without rebuilding.

---

## **8. Extensive Test Cases for User Interactions**

These test cases cover all user interactions across scenes, ensuring >50% coverage. They are framework-agnostic and can be adapted to Jest (Phaser), NUnit (C#), or Godot's GUT.

### **Test Cases: Scene 1: DOS Terminal**

- **TC-1.1**: Player selects 'hero' thread → State updates to `dreamweaverThread: 'hero'`, loads `hero.json`, displays opening lines with typewriter effect.
- **TC-1.2**: Player selects 'shadow' thread → Loads `shadow.json`, verifies different storyBlocks.
- **TC-1.3**: Player selects 'ambition' thread → Loads `ambition.json`, checks unique choices.
- **TC-1.4**: Invalid thread selection (e.g., 'invalid') → Fallback to 'hero', logs warning.
- **TC-1.5**: Player enters name → `playerName` updates, echoes input correctly.
- **TC-1.6**: Player answers secret question → Validates options, updates state.
- **TC-1.7**: Story block navigation → Chooses option, advances to nextBlock, displays paragraphs.
- **TC-1.8**: End of story → Displays exitLine, triggers scene transition to Scene 2.
- **TC-1.9**: JSON load failure → Displays error message, retries load.
- **TC-1.10**: Typewriter effect timing → Verifies text appears character-by-character within 100ms intervals.

### **Test Cases: Scene 2: NetHack Room**

- **TC-2.1**: Scene loads → Renders grid with legend, places player at '@'.
- **TC-2.2**: Player moves on floor (.) → Updates position, no collision.
- **TC-2.3**: Player hits wall (#) → Blocks movement, no state change.
- **TC-2.4**: Player collects shard (*) → Removes from grid, adds to `shards`, displays message.
- **TC-2.5**: Shard collected → Triggers `exitTrigger`, transitions to Scene 3.
- **TC-2.6**: Invalid grid data → Fallback to default layout, logs error.
- **TC-2.7**: Legend mismatch → Ignores unknown symbols, renders as default.
- **TC-2.8**: Multiple shards → Collects all, updates state array.
- **TC-2.9**: No shards present → Prevents exit, shows hint.
- **TC-2.10**: Grid resize → Adapts renderer, maintains aspect ratio.

### **Test Cases: Scene 3: Wizardry Party Selection**

- **TC-3.1**: Scene loads → Displays title, lists classes/races/stats.
- **TC-3.2**: Player selects class → Updates party member, validates against list.
- **TC-3.3**: Player selects race → Combines with class, calculates stats.
- **TC-3.4**: Incomplete party ( <3 members) → Disables confirm, shows warning.
- **TC-3.5**: Confirm party → Sets `confirmText`, transitions to Scene 4.
- **TC-3.6**: Invalid class/race → Reverts selection, logs error.
- **TC-3.7**: Stat recalculation → Updates on selection change.
- **TC-3.8**: UI interaction → Keyboard navigation works, no crashes.
- **TC-3.9**: Party reset → Clears selections, returns to start.
- **TC-3.10**: Localization swap → Loads alternate JSON, updates text.

### **Test Cases: Scene 4: Eye of the Beholder UI**

- **TC-4.1**: Scene loads → Renders walls, shows UI panels if enabled.
- **TC-4.2**: Player navigates (arrow keys) → Updates view, checks collisions.
- **TC-4.3**: Interact with door (south) → Opens if condition met, transitions.
- **TC-4.4**: Inventory panel → Toggles visibility, lists items.
- **TC-4.5**: Stats panel → Displays HP/MP, updates on change.
- **TC-4.6**: Map disabled → No panel shown, no errors.
- **TC-4.7**: Invalid wall data → Defaults to 'brick', logs warning.
- **TC-4.8**: Exit condition not met → Blocks transition, shows message.
- **TC-4.9**: Control remapping → SPACE works for interaction.
- **TC-4.10**: Panel overflow → Scrolls or truncates gracefully.

### **Test Cases: Scene 5: Final Fantasy Combat**

- **TC-5.1**: Scene loads → Displays player/enemy sprites, actions list.
- **TC-5.2**: Player selects FIGHT → Calculates damage, updates enemy HP.
- **TC-5.3**: Player selects MAGIC → Consumes MP, applies effect.
- **TC-5.4**: Player selects ITEM → Uses item, updates inventory.
- **TC-5.5**: Player selects RUN → 50% success, transitions or fails.
- **TC-5.6**: Enemy defeated → Plays victory text, collects shard, transitions.
- **TC-5.7**: Player defeated → Game over screen, restart option.
- **TC-5.8**: Invalid action → Ignores, logs error.
- **TC-5.9**: Music plays → Verifies audio load, no interruptions.
- **TC-5.10**: Sprite load failure → Fallback to placeholder, continues.

### **Test Cases: Global Interactions**

- **TC-G.1**: Scene transition → Saves state, loads next JSON, updates `currentScene`.
- **TC-G.2**: Narrator queue → Processes lines sequentially, clears on scene change.
- **TC-G.3**: Shard collection → Accumulates across scenes, validates uniqueness.
- **TC-G.4**: Thread persistence → Retains choice through game, affects future loads.
- **TC-G.5**: Error handling → Network/API failure → Offline mode with cached JSON.
- **TC-G.6**: Performance → 60 FPS maintained, no frame drops on interactions.
- **TC-G.7**: Accessibility → Keyboard-only navigation works.
- **TC-G.8**: Save/load → Exports state to JSON, imports correctly.
- **TC-G.9**: Multi-thread variants → Loads correct JSON per thread.
- **TC-G.10**: A/B testing → Swaps JSON variants, maintains compatibility.

These test cases ensure comprehensive coverage of user flows, edge cases, and error scenarios across both plans.
