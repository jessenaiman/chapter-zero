# **Ωmega Spiral – Scene Architecture & Narrative Implementation Plan**

---

## **1. Core Design Principles**

- **Narrative as Data**: All dialogue, choices, and scene logic are defined in external JSON files—**never hardcoded**.
- **Per-Scene Schema**: Each scene type (DOS terminal, ASCII dungeon, Wizardry UI, etc.) has its own JSON schema.
- **Dreamweaver Variants**: For any scene supporting narrative branching (e.g., Scene 1), **3 JSON files** exist—one per Dreamweaver thread (`hero.json`, `riddle.json`, `loom.json`).
- **API-Ready**: JSON structure mirrors future REST/GraphQL response format (e.g., `/api/scene/1?thread=hero`).
- **Plug-and-Play Scenes**: Adding Scene 5 = drop `scene5_ff_combat.json` + register in scene manifest.

---

## **2. Project Architecture**

### **File Structure**
```
/src
├── /scenes
│   ├── /scene1_dos_terminal
│   │   ├── schema.json          # Validation schema (optional)
│   │   ├── hero.json            # Dreamweaver: HERO
│   │   ├── riddle.json          # Dreamweaver: RIDDLE  
│   │   └── loom.json            # Dreamweaver: LOOM
│   ├── /scene2_nethack_room
│   │   └── room.json            # Static ASCII layout + shard logic
│   ├── /scene3_wizardry_party
│   │   └── party.json           # Class/race options
│   ├── /scene4_eye_beholder
│   │   └── dungeon.json         # First-person wall definitions
│   └── /scene5_ff_combat
│       └── combat.json          # Enemy, actions, pixel art refs
├── /core
│   ├── SceneLoader.js           # Loads JSON by sceneId + thread
│   ├── SceneManager.js          # Manages transitions, state
│   └── NarratorEngine.js        # Processes narrator rules from JSON
├── /ui
│   ├── DOSRenderer.jsx          # xterm.js + typewriter
│   ├── ASCIIRoomRenderer.jsx    # Grid-based ASCII
│   └── ...                      # One renderer per scene type
└── manifest.json                # Scene registry + metadata
```

### **Global State (Zustand)**
```js
{
  currentScene: 1,
  dreamweaverThread: 'hero', // 'hero' | 'riddle' | 'loom'
  playerName: '',
  shards: [],
  sceneData: null, // Loaded from JSON
  narratorQueue: [] // Array of narrator lines to speak
}
```

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
      {"id": "riddle", "label": "RIDDLE", "description": "A tale that hides its truth until you bleed for it"},
      {"id": "loom", "label": "LOOM", "description": "A tale that changes every time you look away"}
    ]
  },
  "storyBlocks": [
    {
      "paragraphs": [
        "In a city built on broken promises, a child stood at the edge of a bridge that led nowhere.",
        "They held a key made of glass—and everyone warned them: “Don’t cross. The bridge isn’t real.”",
        "But the child knew something no one else did…"
      ],
      "question": "What did the child know?",
      "choices": [
        {"text": "The bridge appears only when you stop believing in it.", "nextBlock": 1},
        {"text": "The key wasn’t for the bridge—it was for the lock inside them.", "nextBlock": 1}
      ]
    },
    {
      "paragraphs": [
        "Ah. Yes. That’s right.",
        "And so the child stepped forward—not onto stone, but onto possibility.",
        "The bridge formed beneath their feet, one plank at a time, woven from every “what if” they’d ever whispered.",
        "But then… a voice called from below."
      ],
      "question": "What did the voice say?",
      "choices": [
        {"text": "You don’t belong here.", "nextBlock": 2},
        {"text": "I’ve been waiting for you.", "nextBlock": 2}
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

1. **Player selects thread** in Scene 1 → `dreamweaverThread` set in state.
2. **SceneLoader** fetches:
   - `scenes/scene1_dos_terminal/${dreamweaverThread}.json`
3. **DOSRenderer** processes:
   - `openingLines` → typewriter effect
   - `storyBlocks` → interactive narrative
   - `namePrompt` → input with echo
4. On completion → **SceneManager** loads Scene 2 (`scene2_nethack_room/room.json`)
5. Each subsequent scene loads its **single JSON file** (no thread variants).

---

## **6. Future API Compatibility**

Current JSON structure = **exact shape** of future API response:
```http
GET /api/v1/scene/1?thread=hero
→ 200 { "type": "dos_terminal", "openingLines": [...], ... }
```

No code changes needed—just swap local JSON loading for `fetch()`.

---

## **7. Benefits of This Architecture**

---

## **8. Core Setup & Libraries**

### **Required Libraries**
- **Phaser 3.70+**: Main game framework for HTML5 canvas/WebGL rendering.
  - Install: `npm install phaser`
- **TypeScript 5.0+**: For type safety and better IDE support.
  - Install: `npm install typescript @types/node`
- **Zustand**: Lightweight state management for React-like state handling.
  - Install: `npm install zustand`
- **React 18+** (optional): For complex UI components if using React integration.
  - Install: `npm install react react-dom @types/react @types/react-dom`
- **xterm.js**: For DOS terminal simulation.
  - Install: `npm install xterm @xterm/addon-fit`
- **Axios**: For HTTP requests to backend API.
  - Install: `npm install axios`
- **Vite**: For fast development build tool.
  - Install: `npm install vite @vitejs/plugin-legacy`

### **Project Structure**
```
/src
├── /scenes
│   ├── /scene1_dos_terminal
│   │   ├── DOSTerminalScene.ts
│   │   ├── hero.json
│   │   ├── riddle.json
│   │   └── loom.json
│   ├── /scene2_nethack_room
│   │   ├── NethackRoomScene.ts
│   │   └── room.json
│   └── ...
├── /core
│   ├── GameState.ts
│   ├── SceneLoader.ts
│   ├── SceneManager.ts
│   └── NarratorEngine.ts
├── /ui
│   ├── DOSRenderer.ts
│   ├── ASCIIRenderer.ts
│   └── ...
├── /stores
│   └── gameStore.ts
├── /types
│   └── index.ts
└── main.ts
```

### **Development Setup**
1. Initialize with Vite: `npm create vite@latest omega-spiral -- --template vanilla-ts`
2. Install dependencies: `npm install phaser zustand axios xterm`
3. Configure TypeScript for Phaser types
4. Set up development server: `npm run dev`

---

## **9. Design Instructions for Phaser TypeScript System**

### **Principles**
- **Scene-Based Architecture**: Use Phaser Scenes for each game screen/state.
- **Component Architecture**: Break UI into reusable components with TypeScript classes.
- **State Management**: Use Zustand stores for global state, local state for components.
- **Event-Driven**: Use Phaser events and custom events for inter-scene communication.
- **Type Safety**: Define interfaces for all data structures and API responses.
- **Performance**: Use Phaser's object pooling for reusable game objects.
- **Testing**: Use Jest/Vitest for unit tests, Playwright for E2E tests.

### **Scene Design**
- **Phaser Scenes**: Extend `Phaser.Scene` for each screen (DOS terminal, combat, etc.).
- **UI Components**: Use Phaser's DOM elements or custom HTML overlays for complex UI.
- **Asset Management**: Load sprites, audio, and JSON via Phaser's loader.
- **Input Handling**: Use Phaser's input system for keyboard/mouse/touch.
- **Transitions**: Use Phaser's scene transitions or custom tweens.
- **Data Binding**: Load JSON data in `preload()`, bind to UI in `create()`.

### **Backend Integration**
- **API Calls**: Use Axios for REST/GraphQL calls to backend.
- **Caching**: Store JSON in localStorage or IndexedDB for offline play.
- **Schemas**: Validate JSON with custom TypeScript interfaces.
- **Real-time**: Use WebSockets for live updates if needed.

### **Code Style**
- Use PascalCase for classes, camelCase for variables/methods.
- Define interfaces for all data structures.
- Use async/await for asynchronous operations.
- Follow SOLID principles with dependency injection.
- Use Phaser's built-in logging for debug.

---

## **10. Code Examples**

### **GameState.ts (Zustand Store)**
```typescript
import { create } from 'zustand';

interface GameState {
  currentScene: number;
  dreamweaverThread: 'hero' | 'riddle' | 'loom';
  playerName: string;
  shards: string[];
  sceneData: any;
  narratorQueue: string[];
  setCurrentScene: (scene: number) => void;
  setDreamweaverThread: (thread: 'hero' | 'riddle' | 'loom') => void;
  setPlayerName: (name: string) => void;
  addShard: (shard: string) => void;
  setSceneData: (data: any) => void;
  addNarratorLine: (line: string) => void;
}

export const useGameStore = create<GameState>((set) => ({
  currentScene: 1,
  dreamweaverThread: 'hero',
  playerName: '',
  shards: [],
  sceneData: null,
  narratorQueue: [],
  setCurrentScene: (scene) => set({ currentScene: scene }),
  setDreamweaverThread: (thread) => set({ dreamweaverThread: thread }),
  setPlayerName: (name) => set({ playerName: name }),
  addShard: (shard) => set((state) => ({ shards: [...state.shards, shard] })),
  setSceneData: (data) => set({ sceneData: data }),
  addNarratorLine: (line) => set((state) => ({ 
    narratorQueue: [...state.narratorQueue, line] 
  })),
}));
```

### **SceneLoader.ts**
```typescript
import axios from 'axios';
import { useGameStore } from '../stores/gameStore';

export class SceneLoader {
  private static instance: SceneLoader;
  
  public static getInstance(): SceneLoader {
    if (!SceneLoader.instance) {
      SceneLoader.instance = new SceneLoader();
    }
    return SceneLoader.instance;
  }

  async loadScene(sceneId: number, thread?: string): Promise<any> {
    const store = useGameStore.getState();
    const threadParam = thread || store.dreamweaverThread;
    
    try {
      // Try API first
      const response = await axios.get(`/api/v1/scene/${sceneId}?thread=${threadParam}`);
      const data = response.data;
      store.setSceneData(data);
      return data;
    } catch (error) {
      // Fallback to local JSON
      console.warn('API failed, loading local JSON:', error);
      return this.loadLocalScene(sceneId, threadParam);
    }
  }

  private async loadLocalScene(sceneId: number, thread: string): Promise<any> {
    const response = await fetch(`/scenes/scene${sceneId}_${thread}.json`);
    const data = await response.json();
    useGameStore.getState().setSceneData(data);
    return data;
  }
}
```

### **DOSTerminalScene.ts (Scene 1)**
```typescript
import Phaser from 'phaser';
import { Terminal } from 'xterm';
import { FitAddon } from '@xterm/addon-fit';
import { useGameStore } from '../stores/gameStore';

export class DOSTerminalScene extends Phaser.Scene {
  private terminal!: Terminal;
  private fitAddon!: FitAddon;
  private outputElement!: HTMLElement;
  private inputElement!: HTMLInputElement;
  private currentLineIndex = 0;
  private openingLines: string[] = [];

  constructor() {
    super({ key: 'DOSTerminalScene' });
  }

  preload() {
    // Load any assets if needed
  }

  async create() {
    const store = useGameStore.getState();
    
    // Create terminal container
    this.createTerminalUI();
    
    // Load scene data
    const sceneData = await new (await import('../core/SceneLoader')).SceneLoader()
      .getInstance().loadScene(1, store.dreamweaverThread);
    
    this.openingLines = sceneData.openingLines;
    this.startTypewriter();
  }

  private createTerminalUI() {
    // Create DOM elements for terminal
    this.outputElement = document.createElement('div');
    this.outputElement.id = 'terminal-output';
    this.outputElement.style.cssText = `
      font-family: 'Courier New', monospace;
      background: black;
      color: green;
      padding: 20px;
      width: 800px;
      height: 400px;
      overflow-y: auto;
      white-space: pre-wrap;
    `;
    
    this.inputElement = document.createElement('input');
    this.inputElement.type = 'text';
    this.inputElement.placeholder = 'Type your response...';
    this.inputElement.style.cssText = `
      font-family: 'Courier New', monospace;
      background: black;
      color: green;
      border: 1px solid green;
      padding: 5px;
      width: 780px;
      margin-top: 10px;
    `;
    
    document.body.appendChild(this.outputElement);
    document.body.appendChild(this.inputElement);
    
    this.inputElement.addEventListener('keypress', (e) => {
      if (e.key === 'Enter') {
        this.handleInput(this.inputElement.value);
        this.inputElement.value = '';
      }
    });
  }

  private async startTypewriter() {
    for (const line of this.openingLines) {
      this.outputElement.textContent += line + '\n';
      await this.delay(500);
    }
    this.showChoices();
  }

  private showChoices() {
    const store = useGameStore.getState();
    const choices = store.sceneData.initialChoice.options;
    
    this.outputElement.textContent += '\n' + store.sceneData.initialChoice.prompt + '\n\n';
    
    choices.forEach((choice: any, index: number) => {
      this.outputElement.textContent += `${index + 1}. ${choice.label} - ${choice.description}\n`;
    });
    
    this.inputElement.placeholder = 'Enter choice number...';
  }

  private handleInput(input: string) {
    const store = useGameStore.getState();
    
    if (store.sceneData.initialChoice) {
      const choiceIndex = parseInt(input) - 1;
      const choices = store.sceneData.initialChoice.options;
      
      if (choiceIndex >= 0 && choiceIndex < choices.length) {
        const selectedChoice = choices[choiceIndex];
        store.setDreamweaverThread(selectedChoice.id);
        this.transitionToNextScene();
      }
    } else if (store.sceneData.namePrompt) {
      store.setPlayerName(input);
      this.outputElement.textContent += `\nHello, ${input}\n`;
      // Continue to next part of scene
    }
  }

  private transitionToNextScene() {
    // Clean up DOM elements
    document.body.removeChild(this.outputElement);
    document.body.removeChild(this.inputElement);
    
    // Transition to next scene
    this.scene.start('NethackRoomScene');
  }

  private delay(ms: number): Promise<void> {
    return new Promise(resolve => setTimeout(resolve, ms));
  }
}
```

### **CombatScene.ts (Scene 5)**
```typescript
import Phaser from 'phaser';
import { useGameStore } from '../stores/gameStore';

interface CombatData {
  playerSprite: string;
  enemy: {
    name: string;
    hp: number;
    sprite: string;
  };
  actions: string[];
  music: string;
  victoryText: string;
}

export class CombatScene extends Phaser.Scene {
  private playerSprite!: Phaser.GameObjects.Sprite;
  private enemySprite!: Phaser.GameObjects.Sprite;
  private playerHp = 100;
  private enemyHp!: number;
  private actionButtons: Phaser.GameObjects.Text[] = [];
  private combatData!: CombatData;

  constructor() {
    super({ key: 'CombatScene' });
  }

  init(data: CombatData) {
    this.combatData = data;
    this.enemyHp = data.enemy.hp;
  }

  preload() {
    this.load.image('player', `/assets/${this.combatData.playerSprite}`);
    this.load.image('enemy', `/assets/${this.combatData.enemy.sprite}`);
    this.load.audio('battleMusic', `/assets/${this.combatData.music}`);
  }

  create() {
    // Add sprites
    this.playerSprite = this.add.sprite(200, 300, 'player');
    this.enemySprite = this.add.sprite(600, 300, 'enemy');
    
    // Add HP text
    this.add.text(200, 250, `HP: ${this.playerHp}`, { fontSize: '24px' });
    this.add.text(600, 250, `HP: ${this.enemyHp}`, { fontSize: '24px' });
    
    // Create action buttons
    this.createActionButtons();
    
    // Play battle music
    this.sound.play('battleMusic', { loop: true });
  }

  private createActionButtons() {
    const buttonY = 500;
    const buttonSpacing = 150;
    
    this.combatData.actions.forEach((action, index) => {
      const button = this.add.text(
        200 + (index * buttonSpacing), 
        buttonY, 
        action, 
        { 
          fontSize: '24px',
          backgroundColor: '#000',
          padding: { x: 10, y: 5 }
        }
      );
      
      button.setInteractive();
      button.on('pointerdown', () => this.executeAction(action));
      this.actionButtons.push(button);
    });
  }

  private executeAction(action: string) {
    switch (action) {
      case 'FIGHT':
        const damage = Phaser.Math.Between(10, 20);
        this.enemyHp -= damage;
        this.showMessage(`You dealt ${damage} damage!`);
        break;
      case 'MAGIC':
        if (this.playerHp > 10) {
          this.enemyHp -= 25;
          this.playerHp -= 10;
          this.showMessage('Magic attack! 25 damage, but cost 10 HP.');
        } else {
          this.showMessage('Not enough HP for magic!');
          return;
        }
        break;
      case 'ITEM':
        this.showMessage('No items available!');
        return;
      case 'RUN':
        if (Phaser.Math.Between(0, 1) > 0.5) {
          this.endCombat(true);
          return;
        } else {
          this.showMessage('Couldn\'t escape!');
        }
        break;
    }

    // Enemy turn
    if (this.enemyHp > 0) {
      const enemyDamage = 10;
      this.playerHp -= enemyDamage;
      this.showMessage(`${this.combatData.enemy.name} dealt ${enemyDamage} damage!`);
      
      if (this.playerHp <= 0) {
        this.endCombat(false);
        return;
      }
    } else {
      this.endCombat(true);
    }
    
    // Update HP display
    this.updateHpDisplay();
  }

  private showMessage(message: string) {
    const messageText = this.add.text(400, 100, message, { 
      fontSize: '18px',
      color: '#fff',
      backgroundColor: '#000',
      padding: { x: 10, y: 5 }
    });
    
    this.time.delayedCall(2000, () => messageText.destroy());
  }

  private updateHpDisplay() {
    // Update HP text (simplified)
    this.children.getAll().forEach(child => {
      if (child instanceof Phaser.GameObjects.Text && child.text.includes('HP:')) {
        child.destroy();
      }
    });
    
    this.add.text(200, 250, `HP: ${this.playerHp}`, { fontSize: '24px' });
    this.add.text(600, 250, `HP: ${this.enemyHp}`, { fontSize: '24px' });
  }

  private endCombat(playerWon: boolean) {
    this.sound.stopAll();
    
    if (playerWon) {
      this.showMessage(this.combatData.victoryText);
      useGameStore.getState().addShard('combat_victory');
      
      this.time.delayedCall(3000, () => {
        this.scene.start('VictoryScene');
      });
    } else {
      this.showMessage('You were defeated...');
      this.time.delayedCall(3000, () => {
        this.scene.start('GameOverScene');
      });
    }
  }
}
```

---

## **11. Testing Instructions**

Use Jest for unit tests and Playwright for E2E tests.

### **Unit Tests Example**
```typescript
import { useGameStore } from '../stores/gameStore';
import { SceneLoader } from '../core/SceneLoader';

describe('GameState', () => {
  it('should initialize with default values', () => {
    const store = useGameStore.getState();
    expect(store.currentScene).toBe(1);
    expect(store.dreamweaverThread).toBe('hero');
  });

  it('should update player name', () => {
    const store = useGameStore.getState();
    store.setPlayerName('Test Player');
    expect(store.playerName).toBe('Test Player');
  });
});

describe('SceneLoader', () => {
  it('should load local scene data', async () => {
    const loader = SceneLoader.getInstance();
    const data = await loader.loadScene(1, 'hero');
    expect(data.type).toBe('dos_terminal');
  });
});
```

Run with `npm test`.

### **E2E Tests with Playwright**
```typescript
import { test, expect } from '@playwright/test';

test('DOS Terminal Scene interaction', async ({ page }) => {
  await page.goto('http://localhost:3000');
  
  // Wait for terminal to load
  await page.waitForSelector('#terminal-output');
  
  // Check opening lines appear
  await expect(page.locator('#terminal-output')).toContainText('Once, there was a name.');
  
  // Enter choice
  await page.fill('input', '1');
  await page.press('input', 'Enter');
  
  // Should transition to next scene
  await expect(page).toHaveURL(/.*nethack.*/);
});
```

---

## **12. Deployment & Build**

- **Build**: Use Vite for production builds: `npm run build`
- **Deployment**: Deploy to any static hosting (Netlify, Vercel, GitHub Pages)
- **Web Standards**: Ensure Phaser canvas works across browsers
- **Performance**: Use Phaser's texture atlas for sprite optimization
- **CI/CD**: Use GitHub Actions with `npm ci && npm run build && npm test`

This provides a complete, actionable Phaser TypeScript design with libraries, examples, and instructions. Ready for the competition!
