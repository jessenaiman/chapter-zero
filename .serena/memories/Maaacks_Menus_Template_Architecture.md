# Maaacks Menus Template - Architecture & Integration Guide

## Core Architecture Overview

The maaacks_menus_template is a **scene-loading and menu system addon** designed for professional game transitions. It's NOT just a menu UI - it's a full scene management framework.

### Key Autoloads (Global Singletons)

1. **AppConfig** (Godot node, persistent)
   - `main_menu_scene_path`: Path to main menu scene (our: `res://source/scenes/menus/main_menu/main_menu_with_animations.tscn`)
   - `ending_scene_path`: Path to end credits scene
   - Exported, configurable in project.godot and in tscn files

2. **SceneLoader** (Godot node, persistent)
   - Handles **asynchronous scene loading** in background
   - `loading_screen_path`: Path to loading screen shown during loads
   - Key methods:
     - `load_scene(path, in_background=false)` - Load scene
     - `change_scene_to_loading_screen()` - Show loading screen
     - `change_scene_to_resource()` - Switch to loaded scene
     - `get_status()` - Check if scene ready (returns ResourceLoader.ThreadLoadStatus)
     - `get_resource()` - Get loaded scene resource
   - Signal: `scene_loaded` - Emitted when background load completes

3. **ProjectMusicController** (Godot node)
   - Manages background music during scene transitions

4. **ProjectUISoundController** (Godot node)
   - Manages UI sound effects (button clicks, etc.)

## Opening Scene Pattern (Base Design)

### What Opening Does
```gdscript
extends Control
@export_file("*.tscn") var next_scene_path : String
@export var images : Array[Texture2D]

func _ready() -> void:
    # STEP 1: Start loading next scene in background (NON-BLOCKING)
    SceneLoader.load_scene(get_next_scene_path(), true)
    
    # STEP 2: Add image textures to container
    _add_textures_to_container(images)
    
    # STEP 3: Start fade-in animations
    _transition_in()

func _show_next_image(animated : bool = true) -> void:
    # Display images with fade-in/fade-out animations
    # When all images done, call _transition_out()

func _transition_out() -> void:
    await get_tree().create_timer(end_delay).timeout
    _load_next_scene()

func _load_next_scene() -> void:
    # Check if scene loaded in background
    var status = SceneLoader.get_status()
    
    if show_loading_screen or status != ResourceLoader.THREAD_LOAD_LOADED:
        # Not ready yet, show loading screen
        SceneLoader.change_scene_to_loading_screen()
    else:
        # Ready! Switch to it
        SceneLoader.change_scene_to_resource()
```

### Key Pattern: Two-Phase Loading
1. **Phase 1 (Async Background)**: Opening starts loading next scene in background while images play
2. **Phase 2 (Animation)**: Opening images fade in/out
3. **Phase 3 (Transition)**: When animations done, check if scene is loaded
   - If loaded → switch to it immediately
   - If not loaded → show loading screen (generic waiting UI)

### User Interactions (Optional Skipping)
- ESC key: Skip entire opening, jump to next scene
- SPACE/ENTER: Skip current image, show next
- Mouse click: Skip current image

## Loading Screen Pattern (Base Design)

### What Loading Screen Does
```gdscript
extends Control
# Simple UI that displays while scene loads in background
# Typically shows progress bar, "Loading..." text, tips, etc.
# When scene finishes loading, this disappears automatically
```

### Why It's Important
- Prevents game freeze if scene loads slower than opening animations
- Professional UX: "we're working on it" feedback to player
- Can show loading progress via `SceneLoader.get_progress()`

## Main Menu Pattern (Base Design)

### What Main Menu Does
1. Shows menu UI with buttons (New Game, Options, Credits, Quit)
2. Emits `game_started` signal when "New Game" button clicked
3. External controllers (like our MainMenuController) listen to this signal

### Signal Connection Pattern
```
Opening (with images) 
  → load_scene(main_menu, in_background=true)
  ↓ (animations finish)
  → SceneLoader changes to main_menu scene
  
MainMenu displays with buttons
  → User clicks "New Game"
  → MainMenu emits "game_started" signal
  → MainMenuController catches signal
  → MainMenuController calls GameManager.StartGame(0)
  → GameManager starts stage progression
```

## Our Custom Wiring (MainMenuController)

### Why We Need It
The addon's MainMenu only emits `game_started` signal. It doesn't know about GameManager.
Our MainMenuController bridges the gap:

```csharp
// In scene: main_menu_with_animations.tscn as child of MainMenu
// Listens to parent's game_started signal
// When signal received → finds/creates GameManager autoload
// Then calls GameManager.StartGame(0)
```

This is **correct architecture** because:
- MainMenu is addon template (shouldn't know about our GameManager)
- MainMenuController customizes it for our game (GameManager integration)
- Follows separation of concerns

## File Paths & Configuration

### Required for Opening → Menu → Game Flow

1. **Opening Scene**
   - Location: `res://source/frontend/scenes/opening/opening.tscn`
   - Must inherit/instance: `res://source/frontend/ui/menu/nodes/opening/opening.tscn`
   - Must set: `next_scene_path = "res://source/frontend/scenes/menus/main_menu/main_menu_with_animations.tscn"`
   - Must set: `images = [Array of Texture2D]` (Godot logo, etc.)
   - Status: ✅ Configured correctly

2. **Main Menu Scene**
   - Location: `res://source/frontend/scenes/menus/main_menu/main_menu_with_animations.tscn`
   - Must inherit/instance: `res://source/frontend/ui/menu/nodes/main_menu/main_menu.tscn`
   - Must have: MainMenuController.cs as child to handle game_started signal
   - Status: ✅ Configured

3. **Loading Screen Scene**
   - Location: `res://source/frontend/scenes/loading_screen/loading_screen.tscn`
   - Must inherit/instance: `res://source/frontend/ui/menu/nodes/loading_screen/loading_screen.tscn`
   - Status: ❓ File exists but SceneLoader can't load it (UID issue likely)

4. **AppConfig (Global Setting)**
   - In `source/frontend/ui/menu/nodes/autoloads/app_config/app_config.tscn`
   - `main_menu_scene_path = "res://source/frontend/scenes/menus/main_menu/main_menu_with_animations.tscn"` ✅
   - `ending_scene_path = "res://source/scenes/end_credits/end_credits.tscn"` ⚠️ May need to set

5. **SceneLoader (Global Setting)**
   - In `source/frontend/ui/menu/nodes/autoloads/scene_loader/scene_loader.tscn`
   - `loading_screen_path = "res://source/frontend/scenes/loading_screen/loading_screen.tscn"` ⚠️ Broken UID ref

## Current Debug Errors Analysis

### Error 1: Loading Screen Not Found
```
ERROR: Cannot open file 'res://source/scenes/loading_screen/loading_screen.tscn'
```
- File EXISTS but has broken UID references (from external script references)
- Solution: Recreate or fix the UIDs in loading_screen.tscn

### Error 2: Animation Track Index Out of Bounds
```
ERROR: Index (uint32_t)track = 4 is out of bounds (tracks.size() = 4)
```
- Indicates opening.tscn has corrupted animation data
- Likely from recent file edits (you mentioned user edited it)
- Solution: Review opening.tscn structure, may need to recreate animations

## Integration Checklist

- ✅ GameManager exists as autoload in project.godot
- ✅ Opening scene chains to MainMenu (via next_scene_path)
- ✅ MainMenuController wires signal to GameManager
- ⚠️ Loading screen has UID resolution issue
- ⚠️ Opening.tscn has animation track corruption
- ❓ Need to verify end_credits path in AppConfig

## Design Principles (Why Maaacks Built It This Way)

1. **Async Loading**: Scenes load in background while UI animates (no stutter)
2. **Optional Loading Screen**: Professional fallback if scene takes too long
3. **Skippable Opening**: Players can always skip to menu/game
4. **Persistent Autoloads**: Music, config, settings survive scene transitions
5. **Extensible Signals**: Game code listens to addon signals (loose coupling)

## Next Steps to Fix

1. Investigate loading_screen.tscn UID issue (may need file re-save)
2. Check opening.tscn animation tracks (were they recently modified?)
3. Test flow: Opening → Menu → GameManager.StartGame() → Stage 1
