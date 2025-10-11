# WASM Ghost Terminal Integration Plan

## Objective
Implement a WebAssembly (WASM) scene using the design team's ghost-terminal components with:
1. Scriptable content system for different scenes
2. Integration with Nobodywho character for dynamic narrative
3. Scene transition from Scene 1 (Opening Hook) to Scene 2 (Nethack)
4. Basic Nethack hero integration with existing Godot C# logic

## Current Project State

### Existing Components
- **Scene 1**: `Scene1Narrative.cs` with typewriter effect and CRT material
  - Located at: `Source/Scripts/Scene1Narrative.cs`
  - Scene file: `Source/Scenes/Scene1Narrative.tscn`
  - Data: Three JSON variants in `Source/Data/scenes/scene1_narrative/` (hero.json, shadow.json, ambition.json)
  
- **Scene 2**: `AsciiRoomRenderer.cs` for Nethack-style dungeon
  - Located at: `Source/Scripts/AsciiRoomRenderer.cs`
  - Scene file: `Source/Scenes/Scene2NethackSequence.tscn`
  - Data: `Source/Data/scenes/scene2_nethack/dungeon_sequence.json`
  
- **Scene Management**: `SceneManager.cs` handles transitions
  - Methods: `TransitionToScene()`, `ValidateStateForTransition()`, `SetDreamweaverThread()`

- **State Management**: `GameState.cs` singleton stores player data
  - Tracks: PlayerName, DreamweaverThread, CurrentScene, Shards

- **Ghost Terminal Components**: Available in `docs/ghost-terminal/components/`
  - TypewriterText.tsx, TerminalChoice.tsx, TerminalInput.tsx
  - CRT effects: AsciiStatic.tsx, PixelDissolve.tsx

## Implementation Strategy

### Phase 1: Hybrid Architecture (Recommended)
Keep Godot C# as the main game engine but create a WASM overlay for enhanced terminal UI:

1. **Godot Core** (Backend)
   - Scene management and game logic remain in C#
   - State persistence and validation
   - Nobodywho LLM integration via DreamweaverSystem
   
2. **WASM Frontend** (UI Layer)
   - Ghost terminal rendering with React/TypeScript
   - CRT effects and typewriter animations
   - WebView integration in Godot using HTMLRect or browser embed

### Phase 2: Script System Design
Create a unified JSON script format that works for both systems:

```json
{
  "scene_id": "scene1_opening",
  "script_version": "1.0",
  "nodes": [
    {
      "type": "dialogue",
      "speaker": "narrator",
      "text": "Once, there was a name...",
      "effects": ["typewriter", "crt_flicker"]
    },
    {
      "type": "choice",
      "prompt": "If you could live inside one kind of story...",
      "options": [
        {"id": "hero", "text": "HERO", "next": "hero_path"},
        {"id": "shadow", "text": "SHADOW", "next": "shadow_path"}
      ]
    }
  ]
}
```

### Phase 3: Nobodywho Integration Points
- DreamweaverSystem generates dynamic narrative
- Script system can include placeholders for LLM-generated content
- Fallback to static JSON if LLM unavailable

## Next Steps
1. Create ScriptLoader service in C# to parse unified script format
2. Extend NarrativeTerminal to support new script nodes
3. Update Scene2 transition to use script system
4. Document integration points for WASM overlay (future enhancement)
