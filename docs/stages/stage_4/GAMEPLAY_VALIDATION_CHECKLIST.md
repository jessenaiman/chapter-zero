# Stage 4 Gameplay Validation Checklist

**Scene**: `res://source/stages/stage_4/field_combat.tscn`  
**Date**: October 19, 2025  
**Tester**: _______________

## Pre-Test Setup

- [ ] Build project successfully: `dotnet build --warnaserror`
- [ ] Open Godot 4.5.1 editor
- [ ] Load scene: `source/stages/stage_4/field_combat.tscn`
- [ ] Press F6 to run the scene directly

---

## 1. Player Spawn & Camera (5 minutes)

### Expected Behavior

- Player character spawns in town area
- Camera follows player smoothly
- Camera respects boundaries (doesn't show empty space)

### Test Steps

- [ ] Player visible on spawn
- [ ] Camera centered on player
- [ ] Camera follows when player moves
- [ ] Camera stops at map edges

**Notes**:

```


```

---

## 2. WASD Movement (10 minutes)

### Expected Behavior

- W/A/S/D or Arrow keys move player on grid
- Player snaps to grid cells (not free movement)
- Collision detection with walls/objects
- Player faces direction of movement

### Test Steps

- [ ] W/Up moves player north (up)
- [ ] S/Down moves player south (down)
- [ ] A/Left moves player west (left)
- [ ] D/Right moves player east (right)
- [ ] Player cannot walk through walls
- [ ] Player cannot walk through NPCs
- [ ] Player faces movement direction
- [ ] Holding key continues movement

**Notes**:

```


```

---

## 3. Click-to-Move (10 minutes)

### Expected Behavior

- Left-click empty cell → player pathfinds to that cell
- Pathfinding avoids obstacles
- Path visualization shows route
- Click destination marker appears

### Test Steps

- [ ] Click nearby empty cell → player moves there
- [ ] Click distant cell → player pathfinds correctly
- [ ] Path goes around walls/obstacles
- [ ] Click unreachable cell → player doesn't move
- [ ] Destination marker shows target
- [ ] Cancel movement by pressing another key

**Notes**:

```


```

---

## 4. NPC Interactions (30 minutes)

### Expected Behavior

- Spacebar near NPC triggers dialogue
- Dialogic window appears with character portrait
- Dialogue text types out with sound effects
- Quest tokens collected when NPC gives them

### NPC 1: Warrior

- [ ] Walk adjacent to Warrior
- [ ] Press Space
- [ ] Dialogue appears: "You may ask, why am I so popular..."
- [ ] Warrior gives token on first interaction
- [ ] `TokenCount` increases to 1
- [ ] Repeat interaction shows different dialogue

**NPC 2: Thief**

- [ ] Walk adjacent to Thief
- [ ] Press Space
- [ ] Thief dialogue appears
- [ ] Token received (if quest active)

**NPC 3: Monk**

- [ ] Walk adjacent to Monk
- [ ] Press Space
- [ ] Monk dialogue appears
- [ ] Token received (if quest active)

**NPC 4: Smith**

- [ ] Walk adjacent to Smith
- [ ] Press Space
- [ ] Smith dialogue appears

**NPC 5: Wizard**

- [ ] Walk adjacent to Wizard
- [ ] Press Space
- [ ] Wizard dialogue appears
- [ ] Token received (if quest active)

**NPC 6: Ghost Runner (Roaming)**

- [ ] Runner moves in loop pattern
- [ ] Walk adjacent to Runner
- [ ] Press Space
- [ ] Runner dialogue appears

**Notes**:

```


```

---

## 5. Combat System (20 minutes)

### Expected Behavior

- Walking near roaming ghost triggers combat
- Screen transition to combat arena
- Turn-based combat UI appears
- Victory/defeat returns to field

### Test Steps

- [ ] Find roaming ghost encounter (position: 200, 264)
- [ ] Walk into ghost's collision area
- [ ] "Before combat" dialogue appears
- [ ] Press Space to continue
- [ ] Screen transitions to black
- [ ] Combat arena loads
- [ ] Turn queue shows player & enemies
- [ ] Attack/Defend actions available
- [ ] Defeat enemy → victory dialogue
- [ ] Return to field at same position

**Conversation Encounter**:

- [ ] Find NPC with red exclamation (position: 83, 6)
- [ ] Interact to trigger combat
- [ ] Pre-combat dialogue appears
- [ ] Combat starts after dialogue
- [ ] Win/lose dialogue plays after combat

**Notes**:

```


```

---

## 6. Door & Key System (15 minutes)

### Expected Behavior

- Door is initially locked
- Key pickup enables door opening
- Door transitions to house interior
- Music changes when entering house

### Test Steps

- [ ] Find locked door (position: 120, 176)
- [ ] Try to open door → "Door is locked" message
- [ ] Find key pickup (position: 40, 24)
- [ ] Walk over key → key collected
- [ ] Return to door
- [ ] Press Space at door
- [ ] Door unlocks and opens
- [ ] Screen transitions
- [ ] Player spawns in house interior (position: 824, 40)
- [ ] Music changes to house theme

**Notes**:

```


```

---

## 7. House Interior (15 minutes)

### Expected Behavior

- House has 3 wand pedestals (Red, Green, Blue)
- Treasure chest contains wand
- Solving pedestal puzzle opens secret
- Exit door returns to town

### Test Steps

- [ ] Explore house interior
- [ ] Find treasure chest (position: 840, 120)
- [ ] Open chest → wand collected
- [ ] Find 3 pedestals (positions: 856/872/888, 40)
- [ ] Interact with Red Pedestal
- [ ] Interact with Green Pedestal
- [ ] Interact with Blue Pedestal
- [ ] Puzzle completion animation plays
- [ ] Exit door (position: 824, 24)
- [ ] Return to town

**Notes**:

```


```

---

## 8. Strange Tree Puzzle (10 minutes)

### Expected Behavior

- First interaction → "It's too bad this tree is in the way..."
- Second interaction → "Wait, what if I went around the tree!?"
- Tree tiles disappear, creating path

### Test Steps

- [ ] Find strange tree (position: 56, 72)
- [ ] First interaction → tree blocking dialogue
- [ ] Variable `StrangeTreeExamined` set to 1
- [ ] Second interaction → realization dialogue
- [ ] Tree tiles disappear (animation)
- [ ] Path now clear
- [ ] Can walk through former tree location

**Notes**:

```


```

---

## 9. Area Transitions (15 minutes)

### Expected Behavior

- Town ↔ Grove transition
- Grove ↔ House transition  
- Music changes per area
- Player spawns at correct position

### Test Steps

**Town to Grove**:

- [ ] Walk to grove entrance (position: 200, 280)
- [ ] Trigger area transition
- [ ] Screen fades
- [ ] Player spawns in grove (position: 632, 424)
- [ ] Music changes to grove theme

**Grove to House**:

- [ ] Walk to house entrance in grove (position: 952, 440)
- [ ] Trigger transition
- [ ] Player spawns in house (position: 936, 24)

**House to Town**:

- [ ] Use house exit
- [ ] Return to town (position: 120, 184)

**Notes**:

```


```

---

## 10. Forest/Grove Endgame (10 minutes)

### Expected Behavior

- Ghost NPC in forest
- Wand pickup available
- Endgame trigger completes game

### Test Steps

- [ ] Navigate to forest area
- [ ] Find ghost NPC (position: 840, 488)
- [ ] Interact with ghost
- [ ] Ghost dialogue appears
- [ ] Find wand pickup (position: 776, 424)
- [ ] Collect wand
- [ ] Walk to endgame trigger (position: 888, 472)
- [ ] Endgame dialogue/animation plays
- [ ] Victory screen appears

**Notes**:

```


```

---

## 11. UI Systems (10 minutes)

### Expected Behavior

- Inventory display shows collected items
- Dialogue window renders correctly
- Combat UI is readable and functional
- No visual glitches

### Test Steps

- [ ] Inventory icons appear when items collected
- [ ] Dialogue box renders with proper font
- [ ] Character portraits appear in dialogues
- [ ] Dialogue typing sound effects play
- [ ] Combat UI turn bar shows correctly
- [ ] No text overflow or clipping

**Notes**:

```


```

---

## 12. Performance (10 minutes)

### Expected Behavior

- Smooth 60 FPS gameplay
- No lag during movement
- Fast scene transitions
- No memory leaks

### Test Steps

- [ ] Enable FPS counter (F3 in Godot debug)
- [ ] Walk around town → stable 60 FPS
- [ ] Trigger combat → no frame drops
- [ ] Return to field → no frame drops
- [ ] Interact with multiple NPCs → smooth
- [ ] Play for 5 minutes → no performance degradation

**FPS Readings**:

```
Town exploration: ___ FPS
Combat: ___ FPS
House: ___ FPS
Forest: ___ FPS
```

---

## 13. Edge Cases & Bugs (15 minutes)

### Test Steps

- [ ] Press Space away from any interaction → nothing happens
- [ ] Click on NPC → pathfind to NPC and interact
- [ ] Spam movement keys → no glitches
- [ ] Pause during dialogue → resumes correctly
- [ ] Exit and reload scene → state preserved
- [ ] Collect same item twice → only one collected
- [ ] Try to walk off map → blocked by boundaries

**Bugs Found**:

```


```

---

## Summary & Sign-Off

### Test Duration

Start Time: ________  
End Time: ________  
Total Time: ________ minutes

### Results

- [ ] ✅ **PASS**: All systems working correctly
- [ ] ⚠️ **PASS with Issues**: Minor bugs found (list below)
- [ ] ❌ **FAIL**: Critical bugs prevent gameplay (list below)

### Issues Found

| # | Severity | System | Description | Repro Steps |
|---|----------|--------|-------------|-------------|
| 1 |          |        |             |             |
| 2 |          |        |             |             |
| 3 |          |        |             |             |
| 4 |          |        |             |             |
| 5 |          |        |             |             |

**Severity**: Critical / Major / Minor

### Comparison to Original godot-open-rpg

- [ ] Movement feels identical to original
- [ ] NPCs behave as expected
- [ ] Combat system matches original
- [ ] Dialogue system works correctly
- [ ] No regressions from original

### Refactoring Quality Score

**Functionality**: ___/10 (Does it work?)  
**Fidelity**:___/10 (Matches original?)  
**Performance**: ___/10 (Runs well?)  
**Polish**:___/10 (Feel/UX quality?)

**Overall**: ___/40

### Approval

- [ ] Ready for automated testing
- [ ] Ready for further development
- [ ] Requires fixes before proceeding

**Tester Signature**: _______________  
**Date**: _______________

---

## Next Steps

After completing this checklist:

1. If **PASS**: Proceed to write GdUnit4 automated tests
2. If **PASS with Issues**: File bug reports, then proceed with testing
3. If **FAIL**: Fix critical bugs, then re-test

**Test Files to Create** (if passing):

- `PlayerMovementTests.cs` - WASD, click-to-move, collision
- `NpcInteractionTests.cs` - All 6 NPCs, dialogue triggers
- `CombatSystemTests.cs` - Combat initiation, turn queue, victory/defeat
- `TownExplorationTests.cs` - Doors, chests, puzzles, transitions
- `PerformanceTests.cs` - FPS, memory, scene load times
