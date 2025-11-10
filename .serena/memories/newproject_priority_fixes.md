# NewProject Priority Fixes - In Progress

## Current Status
We're fixing the 3 priority issues for NewProject setup:

### 1. Main Menu Title/Subtitle - IN PROGRESS
**Issue**: TitleLabel shows "Title", SubTitleLabel shows "Subtitle"  
**Action Needed**: Update to show "Ωmega Spiral" and proper subtitle  
**Design System**: User has omega_spiral_colors_config.json with warm_amber color  
**Files Involved**: 
- `/addons/maaacks_game_template/base/nodes/menus/main_menu/main_menu.tscn` (base template)
- `/source/scenes/menus/main_menu/main_menu_with_animations.tscn` (inherits from base)

### 2. Audio Busses - UNKNOWN STATUS
**Issue**: disable_install_audio_busses=true suggests manual setup  
**Unknowns**: 
- Do audio busses already exist?
- Where should they be created?
- Does default_bus_layout.tres already exist?
- What's the correct location for audio bus files?

### 3. UI Sound Effects - NOT STARTED
**Issue**: UISoundController needs sound effects assigned  
**Unknowns**:
- Which sound files are available?
- Where should sound files be placed?
- Which UI events need sounds?

## Key Questions to Answer:
1. **Audio Bus Location**: Where does Maaacks expect audio busses? Root? addons/?
2. **Current State**: Does default_bus_layout.tres already exist?
3. **Sound Files**: What UI sound files are available?
4. **Design Integration**: How to properly use omega_spiral_colors_config.json?

## Next Actions:
1. Verify audio bus current state before making changes
2. Check if default_bus_layout.tres exists
3. Locate UI sound files
4. Update main menu title/subtitle using design colors