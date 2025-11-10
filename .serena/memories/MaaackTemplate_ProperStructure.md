# Maaack's Game Template - Proper Project Structure

## Official Template Layout (from GitHub repo)

### Autoloads (in project.godot)
These MUST point to the addon location per official structure:
- `AppConfig`: `*res://addons/maaacks_game_template/base/scenes/autoloads/app_config.tscn`
- `SceneLoader`: `*res://addons/maaacks_game_template/base/scenes/autoloads/scene_loader.tscn`
- `ProjectMusicController`: `*res://addons/maaacks_game_template/base/scenes/autoloads/project_music_controller.tscn`
- `ProjectUISoundController`: `*res://addons/maaacks_game_template/base/scenes/autoloads/project_ui_sound_controller.tscn`

### Example Project Structure
```
scenes/
├── game_scene/
│   ├── levels/
│   │   ├── level_1.tscn
│   │   ├── level_2.tscn
│   │   └── level_3.tscn
│   ├── input_display_label.gd
│   └── tutorial_manager.gd
├── menus/
│   ├── level_select_menu/
│   │   ├── level_select_menu.gd
│   │   └── level_select_menu.tscn
│   └── options_menu/
│       ├── audio/
│       │   └── audio_input_option_control.gd
│       └── video/
│           ├── video_options_menu.gd
│           └── video_options_menu_with_extras.tscn
└── opening/
    └── opening_with_logo.tscn
```

### Key Insight
- NO `source/autoloads/` folder in official template
- Autoloads point to ADDON files, not project files
- Project-specific scenes live in `source/scenes/` (NOT autoloads)
- Only custom Godot nodes/scenes that need to be global singletons belong in project autoloads

## Omega Spiral Current State Issues

### Files in `source/autoloads/` that shouldn't be there:
1. `omega_app_config.gd` + `.tscn` - Rename and move to `source/scenes/` or remove if just wrapping addon
2. `omega_global_state.gd` - Likely should be in `source/scripts/` (NOT autoload)
3. `omega_music_controller.gd` + `.tscn` - Should inherit from addon's ProjectMusicController
4. `omega_scene_loader.gd` + `.tscn` - Should inherit from addon's SceneLoader
5. `omega_ui_sound_controller.gd` - Should inherit from addon's ProjectUISoundController
6. `level_and_state_manager.gd` - Should be in `source/scripts/`
7. `app_settings.gd` - Should be in `source/scripts/`
8. `Player.cs` - Should be in `source/scripts/` or `source/characters/`
9. `.uid` files - Cleanup artifacts

### Files that were moved out:
- `game_state.gd` - NOW in `source/scripts/game_state.gd` ✓
- `level_state.gd` - NOW in `source/scripts/level_state.gd` ✓

## Plan
1. Audit each file in source/autoloads/
2. Move non-autoload files to proper locations
3. For Omega-specific autoloads, create them as overrides of addon base files
4. Update project.godot autoload paths to point to correct locations
5. Verify no "hides global script class" errors
