# Stage 4 Manual Gameplay - Pairing Session

## Current State
- **Build Status**: ✅ Passes with zero errors/warnings
- **Main Scene**: `res://source/ui/menus/stage_select.tscn` (StageSelectMenu.cs)
- **Stage 4 Scene**: `res://source/stages/stage_4/field_combat.tscn` (911 lines)
- **Godot Editor**: Now launching

## How to Start Testing
1. Godot loads → stage_select.tscn displays
2. Click "Stage 4: Town Exploration (godot-open-rpg)" button
3. Loads field_combat.tscn

## Original Source for Reference
- **Original Project**: `/home/adam/omega-spiral/godot-open-rpg` (77 GDScript files)
- **File Mapping**: All critical files exist in C# equivalent
  - ✅ Player movement (PlayerController.cs + Gameboard.cs)
  - ✅ NPCs (Gamepieces, 6 characters)
  - ✅ Combat UI (all components found)
  - ✅ Dialogue (Dialogic .dtl files)

## What to Test During Pairing
1. **Movement**: WASD keys move player around town
2. **Click-to-move**: Click on ground to pathfind
3. **NPC Interactions**: Space/click on NPCs to talk
4. **Combat**: Click combat triggers or roaming encounters
5. **Dialogue**: Dialogue system with Dialogic timelines

## Breaking Points = Test Targets
- Note everything that fails or behaves differently
- Create focused unit tests ONLY for failures found
- Don't test what works; test what breaks

## Files to Know
- **Field Script**: `source/scripts/field/PlayerController.cs`, `Gameboard.cs`
- **Combat Script**: `source/scripts/combat/CombatEvents.cs`, `CombatArena.cs`
- **Scene Config**: `project.godot` (autoloads configured)
- **Stage 4**: `source/stages/stage_4/field_combat.tscn`

## Quick Build Command (if needed)
```bash
dotnet build --no-restore  # Build succeeds
```

**Ready for gameplay testing!**
