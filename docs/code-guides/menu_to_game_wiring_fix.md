# Main Menu → Game Manager Wiring - Critical Issues & Fix

## Problems Found

### 1. **GameManager Not in Scene Tree** ❌

**File**: `main_menu_with_animations.tscn`
**Issue**: Signal connection references `GameManager` node, but it doesn't exist in the scene.

```
[connection signal="game_started" from="." to="GameManager" method="StartGame"]
```

**Result**: Signal connects to non-existent node. Game never starts.

### 2. **No C# Controller to Bridge Menu ↔ GameManager** ❌

**File**: `main_menu.tscn` references non-existent `res://source//scenes/menus/main_menu/MainMenu.cs`

```gdscene
[ext_resource type="Script" path="res://source//scenes/menus/main_menu/MainMenu.cs" id="2_custom"]
```

**Issue**: File doesn't exist. Menu has no C# logic to manage game startup.

### 3. **GDScript Placeholder Doesn't Trigger Anything** ❌

**File**: `main_menu_with_animations.gd`
**Problem**: All animation/game-start code is commented out:

```gdscript
func _on_continue_game_button_pressed() -> void:
    load_game_scene()

func _hide_new_game_if_unset() -> void:
    pass  # Keep the New Game button visible
```

Not connected to menu buttons. No action when buttons clicked.

### 4. **Maaacks Menu Emits Signal, But Nobody Listens** ❌

The maaacks template's `MainMenu` (GDScript) emits `game_started` signal:

```gdscript
signal game_started
# ... later in code:
game_started.emit()
```

**But**: No one is listening or forwarding to GameManager.

---

## Solution: Create Proper Wiring Layer

### Step 1: Create C# MainMenuController ✅

**Created**: `MainMenuController.cs`

Responsibilities:
- Runs in `_Ready()` to initialize after scene loads
- Finds the MainMenu node
- Finds or creates GameManager
- Connects MainMenu signal to GameManager.StartGame()
- Logs all steps for debugging

```csharp
public partial class MainMenuController : Node
{
    public override void _Ready()
    {
        // 1. Find MainMenu
        // 2. Find/Create GameManager
        // 3. Wire signals
        // 4. Log everything
    }

    private void OnGameStarted()
    {
        gameManagerNode.StartGame(0);
    }
}
```

### Step 2: Update main_menu.tscn ✅ (Instructions below)

Replace broken MainMenu.cs reference with working MainMenuController.cs:

```gdscene
[ext_resource type="Script" path="res://source//scenes/menus/main_menu/MainMenuController.cs" id="2_custom"]

[node name="MainMenuController" type="Node" parent="." index="5"]
script = ExtResource("2_custom")
```

### Step 3: Update main_menu_with_animations.tscn ✅ (Instructions below)

**Remove broken GameManager connection** and let MainMenuController handle it:

```
# DELETE this line - GameManager isn't in scene:
[connection signal="game_started" from="." to="GameManager" method="StartGame"]
```

---

## Implementation Plan

### Immediate Actions

1. **Update `main_menu.tscn`**:
   - Change MainMenu.cs → MainMenuController.cs
   - Add MainMenuController node

2. **Update `main_menu_with_animations.tscn`**:
   - Remove GameManager signal connection (line ~347)
   - Ensure MainMenuController is attached

3. **Add GameManager as Autoload** (Optional but Recommended):
   - Project Settings → Autoload → Add `/source/backend/GameManager.cs`
   - Ensures GameManager persists across scenes

4. **Test Flow**:
   - Run main menu scene
   - Click "New Game" button
   - Watch console logs
   - Stage 1 should load

### Console Debug Output Expected

```
[GameManager] Ready. Awaiting start command from MainMenu.
[MainMenuController] Initializing menu-to-game wiring...
[MainMenuController] MainMenu node found
[MainMenuController] GameManager autoload found
[MainMenuController] Connected MainMenu.game_started → GameManager.StartGame()
[MainMenuController] Menu triggered game start. Delegating to GameManager...
[GameManager] Starting game from stage 0
[GameManager] === Starting Stage 1: stage_1_ghost ===
```

---

## Architecture Diagram

```
┌─────────────────────────────────────────────────┐
│ main_menu_with_animations.tscn                  │
├─────────────────────────────────────────────────┤
│                                                 │
│  ┌──────────────────────────────────────────┐  │
│  │ MainMenu (GDScript from maaacks)         │  │
│  │ - Displays buttons                        │  │
│  │ - On button click: emit game_started     │  │
│  └──────────┬───────────────────────────────┘  │
│             │ signal                            │
│             ▼                                    │
│  ┌──────────────────────────────────────────┐  │
│  │ MainMenuController (C#) ✅ NEW           │  │
│  │ - Listens for game_started               │  │
│  │ - Finds/creates GameManager              │  │
│  │ - Calls GameManager.StartGame()          │  │
│  └──────────┬───────────────────────────────┘  │
│             │                                    │
└─────────────┼────────────────────────────────────┘
              │
              │ async call
              ▼
    ┌─────────────────────────────────────┐
    │ GameManager (C#)                    │
    │ - Manages stage loading/unloading   │
    │ - Runs game loop                    │
    │ - Tracks dreamweaver points         │
    └──────────┬────────────────────────────┘
               │ creates
               ▼
    ┌─────────────────────────────────────┐
    │ Stage 1 CinematicDirector           │
    │ - Loads scene                        │
    │ - Plays narrative                    │
    │ - Returns results                    │
    └─────────────────────────────────────┘
```

---

## Why This Broke

When reinstalling maaacks_menus_template, the scene references were not updated to match Omega Spiral's architecture. The template expects:
- A game scene path in export variable
- Game to auto-load on menu button click

But Omega Spiral needs:
- GameManager to orchestrate multi-stage loading
- Proper signal flow through C# controllers
- Async task-based stage progression

**Solution**: Create a translation layer (MainMenuController) that adapts maaacks template → Omega Spiral game flow.

---

## Files Modified

- ✅ **Created**: `MainMenuController.cs` - Wiring layer
- ⏳ **Update Needed**: `main_menu.tscn` - Change script reference
- ⏳ **Update Needed**: `main_menu_with_animations.tscn` - Remove broken connection
- ⏳ **Optional**: Add GameManager to Project Autoload

## Next Steps

1. Apply scene edits (see Step-by-Step below)
2. Build project
3. Run main menu
4. Verify console output
5. Click "New Game" button
6. Stage 1 should load
