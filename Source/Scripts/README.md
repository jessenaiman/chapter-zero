# Omega Spiral Scripts Documentation

This directory contains all the C# scripts for the Omega Spiral game, built with Godot 4.5 and .NET 10.

## Architecture Overview

The game follows an MVC-like architecture with the following key components:

- **GameState.cs**: Singleton autoload that manages global game state
- **SceneManager.cs**: Handles scene transitions and state validation
- **Controller Scripts**: Handle scene-specific logic and user interactions
- **Data Models**: Represent game entities and data structures
- **Utility Scripts**: Provide common functionality

## Script Reference

### Core Systems

#### GameState.cs
**Purpose**: Global game state management and persistence

**Key Features**:
- Singleton autoload accessible from all scenes
- Save/load functionality using JSON
- Cross-scene state persistence
- Dreamweaver scoring and selection
- Party and inventory management

**Public API**:
- `SaveGame()`: Saves current state to user://savegame.json
- `LoadGame()`: Loads state from save file
- `UpdateDreamweaverScore(type, points)`: Updates alignment scores
- `GetHighestScoringDreamweaver()`: Returns dominant dreamweaver type

#### SceneManager.cs
**Purpose**: Manages scene transitions and state validation

**Key Features**:
- Validates state before allowing scene transitions
- Ensures proper game flow and prerequisites
- Integrates with GameState for persistence

**Public API**:
- `TransitionToScene(scenePath)`: Transitions to new scene with validation
- `ValidateStateForTransition(scenePath)`: Checks if transition is allowed
- `UpdateCurrentScene(sceneId)`: Updates current scene in GameState

### Scene Controllers

#### NarrativeTerminal.cs
**Purpose**: Controls the narrative introduction scene

**Features**:
- Terminal-style text display
- Player name input
- Dreamweaver thread selection
- Story progression

#### PartyCreator.cs
**Purpose**: Character creation and party management

**Features**:
- Character generation with racial modifiers
- Class selection
- Party composition
- Stat calculation

#### TileDungeonController.cs
**Purpose**: Manages tile-based dungeon exploration

**Features**:
- Grid-based movement
- Room generation
- Interactive objects (chests, doors, monsters)
- Dreamweaver alignment choices

#### PixelCombatController.cs
**Purpose**: Turn-based pixel art combat system

**Features**:
- Party vs enemy combat
- Action selection (attack, defend, special)
- Damage calculation
- Victory/defeat conditions

#### AsciiRoomRenderer.cs
**Purpose**: Renders ASCII art dungeon rooms

**Features**:
- Text-based room display
- Interactive object highlighting
- Input handling for room navigation

### Data Models

#### PartyData.cs
**Purpose**: Represents player party and inventory

**Properties**:
- `Members`: List of Character objects
- `Gold`: Currency amount
- `Inventory`: Collection of items

#### Character.cs
**Purpose**: Individual character representation

**Properties**:
- `Name`: Character name
- `Race`: Character race (Human, Elf, Dwarf, etc.)
- `CharacterClass`: Character class (Fighter, Mage, etc.)
- `Stats`: Character statistics

#### CharacterStats.cs
**Purpose**: Character attribute system
**Features**:
- Base stats (Strength, Intelligence, etc.)
- Racial modifiers
- Level progression
- Combat calculations

#### DungeonSequenceData.cs
**Purpose**: Dungeon room and sequence definitions
**Features**:
- Room templates
- Interactive object definitions
- Dreamweaver alignment effects

#### TileDungeonData.cs
**Purpose**: Tile-based dungeon data structures
**Features**:
- Room layouts
- Entity positions
- Navigation data

### Utility Scripts

#### JsonSchemaValidator.cs
**Purpose**: Validates JSON data against schemas
**Features**:
- Schema-based validation
- Error reporting
- Data integrity checks

#### SceneLoader.cs
**Purpose**: Handles scene loading and resource management
**Features**:
- Asynchronous scene loading
- Resource caching
- Error handling

#### NarratorEngine.cs
**Purpose**: Manages narrative text and dialogue
**Features**:
- Text queuing system
- Display timing
- Narrative state tracking

### Enums and Constants

#### Enums.cs
**Purpose**: Game-wide enumerations
**Definitions**:
- `DreamweaverType`: Light, Mischief, Wrath
- `DreamweaverThread`: Hero, Villain, Trickster
- `CharacterRace`: Human, Elf, Dwarf, Orc, etc.
- `CharacterClass`: Fighter, Mage, Rogue, etc.

## Dependencies

- **Godot 4.5** (Mono version)
- **.NET 10.0**
- **Newtonsoft.Json** (for complex JSON operations)
- **NUnit** (for testing)

## Testing

Unit tests are located in the `/Tests/` directory:
- `StatePersistenceTests.cs`: Cross-scene state management
- `SaveLoadTests.cs`: Save/load functionality
- `NarrativeTerminalSchemaTests.cs`: Schema validation
- `NarrativeTerminalIntegrationTests.cs`: Integration testing

## Performance Considerations

- JSON loading should complete in <100ms
- Scene transitions should complete in <500ms
- Maintain 60 FPS target
- Minimize allocations in hot paths

## File Structure

```
Source/Scripts/
├── GameState.cs              # Global state management
├── SceneManager.cs           # Scene transitions
├── [Scene]Controller.cs      # Scene-specific logic
├── [Entity]Data.cs          # Data models
├── [Utility].cs             # Helper classes
└── Enums.cs                 # Game constants
```

## Development Notes

- All scripts inherit from Godot classes (Node, Node2D, etc.)
- Use Godot's autoload system for singletons
- Follow C# naming conventions
- Use Godot's resource system for assets
- Implement proper error handling and logging
