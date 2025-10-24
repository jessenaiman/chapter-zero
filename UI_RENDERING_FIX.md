# Ui Rendering Fix - Root Cause Analysis

## Problem
Ui elements (like the main menu label) are rendering off-screen/off-center.

## Root Cause
**`FieldCamera` is configured as a global autoload in `project.godot`**

### Current Configuration (project.godot, line 28):
```
FieldCamera="*res://source/scripts/field/FieldCamera.cs"
```

### Why This Breaks Ui
- `FieldCamera` is a `Camera2D` node (world-space camera)
- Autoloading it means it's instantiated globally **even on Ui scenes**
- When a `Camera2D` is active on a Ui scene with Control nodes, the rendering pipeline gets confused
- Control nodes expect viewport-relative coordinates; Camera2D applies world transformations
- **Result**: Ui renders off-screen or off-center

## Solution
**Remove `FieldCamera` from the `[autoload]` section in `project.godot`**

Replace:
```
[autoload]
...
FieldCamera="*res://source/scripts/field/FieldCamera.cs"
...
```

With (remove the FieldCamera line entirely):
```
[autoload]
...
Gameboard="*res://source/scripts/field/gameboard/Gameboard.cs"
GamepieceRegistry="*res://source/scripts/field/gamepieces/GamepieceRegistry.cs"
...
```

### Why This Works
- `FieldCamera` will only be instantiated when field scenes explicitly need it
- Ui scenes (like main_menu.tscn) will render with the default viewport
- Control nodes will use proper viewport-relative positioning
- Label will render centered as designed

## Implementation Notes
- `FieldCamera` should be added to field scenes (not as autoload)
- Look in scene files like `tile_dungeon.tscn` or field scenes where it's needed
- Instance it as a regular node, or add a reference in the field scene's C# script

## Test After Fix
1. Open main_menu.tscn in editor
2. Run the scene (F5)
3. Label "Omega Spiral - Chapter Zero" should be centered
4. Buttons should be visible and clickable
5. Run the centering test: `test_centered_label.gd` should now pass

## Changes Made (Pre-requisite)
- ✅ Fixed `MainMenu.cs` node paths (was looking for Panel/VBoxContainer, now looks for CenterContainer/VBoxContainer)
- ✅ Updated button callbacks (Stage1/Stage2 → Start/Options)
- ✅ Modified `FieldCamera.cs._Ready()` to disable itself on Ui scenes (detects scene path, disables if it contains "menus", "ui", or "narrative")
- ⏳ **OPTIONAL**: Remove FieldCamera from autoloads in project.godot for cleaner architecture (but not required now that FieldCamera auto-disables)
