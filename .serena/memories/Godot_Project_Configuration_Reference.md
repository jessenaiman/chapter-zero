# Godot Project Configuration Reference

## Correct Autoload Configuration (project.godot)

The Maaacks Game Template autoload pattern should follow this structure:

```
[autoload]

GlobalState="*res://source/autoloads/omega_global_state.gd"
AppConfig="*res://addons/maaacks_game_template/base/nodes/autoloads/app_config/app_config.tscn"
OmegaSettings="*res://source/config/omega_app_settings.gd"
SceneLoader="*res://addons/maaacks_game_template/base/nodes/autoloads/scene_loader/scene_loader.tscn"
ProjectMusicController="*res://addons/maaacks_game_template/base/nodes/autoloads/music_controller/music_controller.tscn"
ProjectUISoundController="*res://addons/maaacks_game_template/base/nodes/autoloads/ui_sound_controller/ui_sound_controller.tscn"
Dialogic="*res://addons/dialogic/Core/DialogicGameHandler.gd"
```

### Key Points:

1. **GlobalState** (Custom Extension):
   - Path: `source/autoloads/omega_global_state.gd`
   - Extends template's GlobalState for Omega-specific persistence
   - Available as autoload singleton: `GlobalState`

2. **AppConfig** (Template):
   - Points directly to template addon
   - NO custom omega_app_config.gd needed
   - Used for game configuration

3. **OmegaSettings** (Custom Extension):
   - Path: `source/config/omega_app_settings.gd`
   - Extends template's AppSettings with Omega-specific settings
   - Initializes dreamweaver preferences on startup

4. **SceneLoader** (Template):
   - Points directly to template addon
   - Handles scene transitions

5. **ProjectMusicController** (Template):
   - Points directly to template addon
   - Manages audio streaming and blending
   - Use `ProjectMusicController.play_stream()` to play music

6. **ProjectUISoundController** (Template):
   - Points directly to template addon
   - Manages UI sound effects

7. **Dialogic** (Custom Addon):
   - Custom addon for dialogue system
   - Configuration in [dialogic] section

### DO NOT:
- ❌ Create duplicate autoload entries (e.g., OmegaGlobalState + GlobalState)
- ❌ Create omega_* versions of template autoloads (app_config, scene_loader, music_controller, ui_sound_controller)
- ❌ Use preload() for GlobalState - it's an autoload singleton
- ❌ Point to non-existent paths

### DO:
- ✅ Extend template files in source/ folder for custom behavior
- ✅ Use autoload singletons directly (e.g., `GlobalState.save()` not `_GlobalState.save()`)
- ✅ Point template autoloads to their official addon paths
- ✅ Add only Omega-specific custom autoloads (like OmegaSettings)
