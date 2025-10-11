# Scene Transition Implementation - Scene 1 to Scene 2

## Current State Analysis

### Scene 1 (NarrativeTerminal)
- **File**: `Source/Scripts/NarrativeTerminal.cs`
- **Scene**: `Source/Scenes/Scene1Narrative.tscn`
- **Flow**: 
  1. Display opening lines with typewriter effect
  2. Present initial choice (Hero/Shadow/Ambition)
  3. Show story blocks with branching choices
  4. Prompt for player name
  5. Ask secret question
  6. Complete scene via `CompleteNarrativeSceneAsync()`
- **Transition Trigger**: Calls `sceneManager.TransitionToScene("Scene2NethackSequence")`

### Scene 2 (AsciiRoomRenderer)
- **File**: `Source/Scripts/AsciiRoomRenderer.cs`
- **Scene**: `Source/Scenes/Scene2NethackSequence.tscn`
- **Data**: `Source/Data/scenes/scene2_nethack/dungeon_sequence.json`
- **Flow**:
  1. Load dungeon data with 3 rooms (one per Dreamweaver)
  2. Initialize player position (@)
  3. Render ASCII grid
  4. Handle player movement (arrow keys)
  5. Check for object interactions (door/monster/chest)
  6. Transition to next scene after completion

### SceneManager Transition Logic
- **Method**: `TransitionToScene(string scenePath)`
- **Steps**:
  1. Validate state with `ValidateStateForTransition()`
  2. Save current state via `SaveCurrentState()`
  3. Load new PackedScene from res://Source/Scenes/{scenePath}.tscn
  4. Instantiate and add to scene tree
  5. Remove old scene

## Implementation Tasks

### Task 1: Ensure Scene 1 properly completes
- Verify `CompleteNarrativeSceneAsync()` saves all required state
- Ensure DreamweaverThread is set correctly
- Confirm player name is stored in GameState

### Task 2: Create transition bridge
- Add visual transition effect (fade to black, CRT static, etc.)
- Display "Entering the Echo Chamber..." message
- Brief narrative hook connecting the scenes

### Task 3: Enhance Scene 2 data structure
- Update dungeon_sequence.json to include Dreamweaver-specific narrative
- Add metadata for each room (Light/Shadow/Ambition alignment)
- Include object descriptions that match chosen thread

### Task 4: Implement Scene 2 initialization
- Load appropriate dungeon based on DreamweaverThread
- Display intro text: "I remember every cycle..." dialogue
- Position player at starting location

### Task 5: Test full flow
- Scene 1 → Scene 2 transition
- State persistence across scenes
- Dreamweaver continuity

## JSON Data Structure Updates

### Scene 2 Enhanced Format
```json
{
  "scene_type": "nethack_sequence",
  "dungeons": [
    {
      "id": "hero_chamber",
      "alignment": "light",
      "intro_text": "Light: I remember every cycle.",
      "grid": ["#####", "#.@.#", "#.*.#", "#####"],
      "objects": {
        "door": {"pos": [1,1], "desc": "A door of pure light"},
        "monster": {"pos": [2,1], "desc": "Your own shadow"},
        "chest": {"pos": [1,2], "desc": "A crystal chest"}
      }
    }
  ]
}
```

## Next Implementation Step
Start with Task 1: Review and enhance CompleteNarrativeSceneAsync() method
