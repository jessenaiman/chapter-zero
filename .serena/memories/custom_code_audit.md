# Custom C# Code Audit - Conflicts with Maaack's Addon

## INPUT SYSTEM (DUPLICATE EFFORT)

### Custom Code:
- **OmegaInputRouter.cs** - Custom input router handling pause, navigation, confirm, back
  - Maps Godot input actions to semantic events
  - Manually processes `ui_accept`, `ui_cancel`, `pause` actions
  
**CONFLICT**: Maaack's addon already provides:
- Full input handling through AppSettings.InputSettings
- Input rebinding UI with visual mapping
- Gamepad support (Xbox, PS4, PS5, Switch, Steam Deck)
- Input icon mapping for different devices
- `override.cfg` adds UI actions for gamepads (Accept, Cancel, Page Up/Down)

**ACTION NEEDED**: Replace OmegaInputRouter with Maaack's input system or remove it

---

## AUDIO SYSTEM (DUPLICATE EFFORT)

### Custom Code:
- **GhostAudioManager.cs** - Custom audio management for Ghost stage

**CONFLICT**: Maaack's addon already provides:
- MusicController autoload (auto-blends music between scenes)
- UISound Controller (auto-attaches sounds to buttons, tabs, sliders)
- Audio bus system (Master, Music, SFX buses)
- AppSettings audio options (volume per bus, mute)

**ACTION NEEDED**: Use ProjectMusicController and ProjectUISoundController instead

---

## SCENE MANAGEMENT (DUPLICATE EFFORT)

### Custom Code:
- **OmegaSceneManager.cs** - Custom scene management
- **GhostSceneManager.cs** - Ghost-specific scene manager
- **SceneManager.cs** (StorySceneRunner.cs) - Story scene runner
- **LoadingScreenController.cs** - Custom loading screen controller

**CONFLICT**: Maaack's addon already provides:
- SceneLoader autoload (async scene loading with loading screen)
- LevelLoader (loads scenes into containers)
- LevelManager (manages level progression)
- LoadingScreen (customizable with fade animations)

**ACTION NEEDED**: Use SceneLoader for all scene transitions, LevelLoader for loading levels

---

## SETTINGS & PERSISTENCE (DUPLICATE EFFORT)

### Custom Code:
- May have custom config/persistence code

**CONFLICT**: Maaack's addon already provides:
- PlayerConfig (persistent user://player_config.cfg)
- AppSettings (sections: Input, Audio, Video, Game, Application, Custom)
- GlobalState (resource-based persistence with versioning)

**ACTION NEEDED**: Use PlayerConfig and GlobalState instead of custom persistence

---

## GAMEPLAY SYSTEMS (UNIQUE - KEEP)

### Custom Code to KEEP:
- **DreamweaverSystem.cs** - Omega-specific dreamweaver scoring (no conflict)
- **Combat system** (ActiveTurnQueue, etc.) - Game-specific combat logic (no conflict)
- **Gamepiece controllers** - Field movement/interaction (no conflict)
- **Party creation** - Character/party management (no conflict)
- **Story/narrative system** - Dialogic integration (no conflict)

---

## WHAT WE NEED TO DO

### High Priority:
1. ❌ Remove/replace OmegaInputRouter - Use Maaack's input system
2. ❌ Remove/replace GhostAudioManager - Use ProjectMusicController + ProjectUISoundController
3. ❌ Remove custom SceneManagers - Use SceneLoader + LevelLoader

### Medium Priority:
4. ⚠️ Audit custom persistence - Migrate to GlobalState + PlayerConfig
5. ⚠️ Wire up LevelManager for Ghost stage progression
6. ⚠️ Use WinLoseManager for combat end conditions

### Keep (Extend, don't replace):
7. ✅ DreamweaverSystem - Extend GlobalState to save scores
8. ✅ Combat system - Hook into WinLoseManager signals
9. ✅ Party system - Use PlayerConfig.CUSTOM_SECTION for party data
10. ✅ Narrative system - Use SceneLoader for transitions

---

## Extension Points for Omega-Specific Logic

1. **OmegaGlobalState** (extends GlobalState)
   - Save dreamweaver scores per level
   - Save party composition
   - Save dialogue choices
   
2. **Custom AppSettings.CUSTOM_SECTION**
   - Difficulty settings
   - Dreamweaver preference
   - Custom gameplay options

3. **Custom LevelManager extension**
   - Manage Ghost encounter progression
   - Track which dreamweavers participated
   - Record narrative choices

4. **Custom OptionControl scenes**
   - Dreamweaver difficulty selector
   - Character creation UI
   - Party management UI
