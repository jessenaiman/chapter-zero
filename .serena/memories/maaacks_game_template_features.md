# Maaack's Game Template - COMPLETE Feature List for Omega Spiral

## Base Features (From README)

1. **Main Menu** - Start, Continue, Settings, Credits, Quit
2. **Options Menus** - Separate menus for audio, video, input, and custom options
3. **Pause Menu** - Triggered on `ui_cancel`, pauses game, returns to previous focus
4. **Credits** - Auto-generated from ATTRIBUTION.md, scrolling display
5. **Loading Screen** - Customizable with fade animations and progress
6. **Opening Scene** - Image fade sequences, auto-transition to main menu
7. **Persistent Settings** - All settings saved to `user://player_config.cfg`
8. **Simple Config Interface** - PlayerConfig (section-based key-value system)
9. **Extensible Overlay Menus** - Framework for custom overlaid UI elements
10. **Keyboard/Mouse Support** - Full keyboard and mouse input handling
11. **Gamepad Support** - Full gamepad/joypad input support
12. **UI Sound Controller** - Auto-applies sounds to UI interactions
13. **Background Music Controller** - Auto-blends music across scene transitions

## Extra Systems

14. **Level Loaders** - LevelLoader for async scene loading into containers
15. **Level Progress Manager** - LevelManager for tracking progression and win/lose states
16. **Win / Lose Manager** - WinLoseManager for custom end-game screens
17. **Scene Listing** - SceneLister for organizing linear level progression

## Core Components Behind These Features

- **AppConfig**: Scene path configuration (main menu, game, ending)
- **PlayerConfig**: Persistent configuration file management
- **AppSettings**: Manages input, audio, video, game, and application settings
- **GlobalState**: Resource-based state persistence with versioning
- **SceneLoader**: Async scene loading with optional loading screen
- **Focus Management** (capture_focus.gd): Auto-focuses UI for keyboard/gamepad nav
- **Audio Bus System**: Manages audio routing and volume per bus
- **Input Mapping**: Full rebinding with visual UI and persistence
- **Resolution Scaling**: Supports 640x360 to 4K with automatic scaling

## What Omega Spiral Gets "For Free"

1. ✅ Fully functional main menu (Start, Continue, Settings, Credits, Quit)
2. ✅ Pause menu on ESC/gamepad cancel
3. ✅ Audio options (volume control per bus, mute)
4. ✅ Video options (resolution, fullscreen, v-sync)
5. ✅ Input rebinding with visual key display
6. ✅ Game state persistence (with versioning)
7. ✅ Scene loading with loading screens
8. ✅ Credits auto-generated from ATTRIBUTION.md
9. ✅ UI sound effects on interactions
10. ✅ Music seamless transitions between scenes
11. ✅ Full keyboard and gamepad support
12. ✅ Focus management for UI navigation
13. ✅ Win/Lose screen framework
14. ✅ Level progression framework

## What We Need to Build for Omega Spiral

1. **Custom Options**: Dreamweaver difficulty, character select, party composition
2. **Ghost Stage Director**: Custom scene loader that instantiates GhostCinematicDirector
3. **Dreamweaver Scoring**: Custom state tracking for all 3 Dreamweavers
4. **Combat Integration**: Win/lose conditions tied to combat C# code
5. **Story Progression**: Level states tied to narrative choices
6. **Custom Menus**: Override/extend main menu to show party/dreamweaver info
