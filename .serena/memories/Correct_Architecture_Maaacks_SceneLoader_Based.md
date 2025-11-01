# Correct Architecture: Maaacks SceneLoader-Based Flow

## CRITICAL RULE
- **DO NOT modify anything in `addons/maaacks_game_template/`** - It's a battle-tested package, like npm/pip
- Focus ONLY on `source/` folder (your game code)
- Inherit/instance Maaacks components in source/, don't modify them

---

## THE CORRECT FLOW

```
Opening (source/scenes/opening/opening.tscn)
  ↓ [instances Maaacks addon base, sets next_scene_path]
  SceneLoader.load_scene() loads Main Menu in background
  ↓
Main Menu (source/scenes/menus/main_menu/main_menu.tscn)
  ↓ [instances Maaacks addon base, attaches custom script]
  User clicks "New Game"
  ↓
Main Menu Script calls:
  SceneLoader.load_scene("res://stages/stage_1_ghost.tscn")
  ↓
Stage 1 Ghost (.tscn scene)
  ↓ [runs GhostCinematicDirector]
  GhostCinematicDirector.RunAsync()
    → NarrativeUi displays lines
    → GameState tracks dreamweaver points
  ↓ [stage completes]
  Calls: SceneLoader.load_scene("res://stages/stage_2_nethack.tscn")
  ↓
Stage 2 Nethack (.tscn scene)
  ↓ [same pattern]
  ...continues through Stage 3, 4, 5...
  ↓
End Credits (source/scenes/end_credits/end_credits.tscn)
```

---

## WHAT THIS MEANS

### ✅ KEEP (Battle-Tested Maaacks Systems)
- `SceneLoader` (async scene loading, loading screens, progress)
- `AppConfig` (global settings/paths autoload)
- `ProjectMusicController` (music blending)
- `ProjectUISoundController` (UI sounds)
- All Maaacks menu infrastructure

### ✅ KEEP (Your Unique Game Code)
- `CinematicDirector` subclasses (Stage1Director, Stage2Director, etc.)
- `NarrativeUi` (displays dialogue/choices)
- `GameState` (tracks dreamweaver points)
- Story JSON scripts
- Stage-specific logic

### ❌ REMOVE/SIMPLIFY (Redundant Custom Code)
- `GameManager` orchestration loop (SceneLoader replaces this)
- `StorySceneRunner` (merge into CinematicDirector directly)
- Custom scene loading logic (use SceneLoader instead)
- Any duplicate transition handling

---

## KEY REFACTORING WORK

1. **Delete GameManager.cs** - SceneLoader becomes the transition system
2. **Simplify CinematicDirector** - Remove scene loading layer, call NarrativeUi directly
3. **Add stage entry scripts** - Each stage calls SceneLoader.load_scene() when done
4. **Wire MainMenu to stages** - MainMenu.gd calls SceneLoader.load_scene(stage_1_path)
5. **Use AppConfig** - Store stage paths in AppConfig for centralized management

---

## THE PATTERN (Per Stage)

Each stage scene (.tscn) has a root script that:
1. Runs its CinematicDirector in _Ready()
2. Awaits completion
3. Calls SceneLoader.load_scene(next_stage_path)

This is decentralized, simple, and uses proven Maaacks infrastructure.

---

## WHY THIS WORKS

- Maaacks SceneLoader handles all threading/loading screens
- No central orchestrator needed
- Each stage is self-contained and independent
- GameState autoload persists between scenes
- Clear ownership: Maaacks handles transitions, your code handles narrative/gameplay
