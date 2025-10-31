# Menu Architecture Analysis

## Current Setup

### GDScript Layer (Main)
- **main_menu.gd**: Base menu controller (GDScript)
  - Handles button connections
  - Manages sub-menu lifecycle
  - Signals: `sub_menu_opened`, `sub_menu_closed`, `game_started`, `game_exited`
  - Export properties: `game_scene_path`, `options_packed_scene`, `credits_packed_scene`
  - Methods called on button press:
    - `_on_new_game_button_pressed()` → `new_game()` → `load_game_scene()`
    - `_on_options_button_pressed()` → `_open_sub_menu(options_packed_scene)`
    - `_on_credits_button_pressed()` → `_open_sub_menu(credits_packed_scene)`
    - `_on_exit_button_pressed()` → `exit_game()`

### Scene Hierarchy
1. **main_menu_with_animations.tscn** - Root animated menu scene
   - Script: main_menu_with_animations.gd (extends MainMenu)
   - Loads: main_menu.tscn (as instance)
   - Provides animation state machine (currently commented out)

2. **main_menu.tscn** - Base menu structure with buttons
   - Script: main_menu.gd
   - Nodes:
     - MenuContainer/MenuButtonsMargin/MenuButtonsContainer/MenuButtonsBoxContainer
       - NewGameButton → `_on_new_game_button_pressed`
       - OptionsButton → `_on_options_button_pressed`
       - CreditsButton → `_on_credits_button_pressed`
       - ExitButton → `_on_exit_button_pressed`

### Unused C# Component
- **MainMenuController.cs** - Unused C# controller (not referenced in any scene)
  - No integration with the current GDScript-based menu system

## Current Workflow When Button is Pressed
1. Button emits `pressed` signal
2. GDScript connection calls the handler method (e.g., `_on_new_game_button_pressed`)
3. Handler calls business logic (e.g., `new_game()`)
4. Business logic calls system components (e.g., `SceneLoader.load_scene()`)

## Key Observations
- The button interactions already work without any C# controller
- Menu system is purely GDScript-based
- No C# integration currently needed for menu functionality
- The unused MainMenuController.cs could be removed or integrated if needed
