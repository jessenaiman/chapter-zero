# Omega Spiral - Chapter Zero - Project Context

## Project Overview

Omega Spiral - Chapter Zero is a narrative-driven Godot game that blends retro gaming aesthetics (DOS terminals, NetHack ASCII, Wizardry party management, dungeon crawling, pixel combat) with modern AI-powered storytelling. The game features dynamic AI-driven narrative personas (Dreamweavers) that adapt to player choices, creating emergent storytelling experiences through five distinct scenes, each representing a different era of gaming aesthetics.

### Key Features
- **5 Unique Scenes**: Each with distinct visual styles and gameplay mechanics
- **AI-Powered Narratives**: Three Dreamweaver personas (Hero, Shadow, Ambition) + Omega system for dynamic storytelling
- **Retro Aesthetics**: CRT effects, terminal interfaces, ASCII art, and pixel graphics
- **Character Progression**: Party management, stats, and persistent save system
- **LLM Integration**: Uses the NobodyWho plugin for local LLM inference

### Technologies Used
- **Engine**: Godot 4.5 with .NET/Mono support
- **Language**: C# for game logic
- **AI Integration**: NobodyWho plugin for local LLM inference
- **Testing**: NUnit + Godot test framework

## Project Structure

```
chapter-zero/
├── Source/
│   ├── Scenes/          # Godot scene files (.tscn)
│   ├── Scripts/         # C# game logic
│   ├── Shaders/         # CRT and visual effects
│   ├── Resources/       # Assets and resources
│   └── Data/            # Game data (JSON, YAML)
├── Tests/               # NUnit test files
├── addons/              # Godot addons including nobodywho
├── models/              # LLM models (gitignored)
├── docs/                # Documentation and ADRs
└── specs/               # Feature specifications
```

## Core Architecture

### Main Components

1. **SceneManager.cs**: Manages scene transitions and tracks current scene state across the game. Serves as a singleton autoload for centralized scene management.

2. **GameState.cs**: Global game state singleton managing player progress, Dreamweaver alignment, and persistence. Tracks LLM consultation history and dynamic narrative state.

3. **PartyData.cs**: Classic CRPG party creation and management system with support for up to 3 members and inventory management.

4. **Enums.cs**: Defines key enumerations including:
   - DreamweaverType: Light, Mischief, Wrath
   - DreamweaverThread: Hero, Shadow, Ambition
   - CharacterClass: Fighter, Mage, Priest, Thief, Bard, Paladin, Ranger
   - CharacterRace: Human, Elf, Dwarf, Gnome, Halfling, HalfElf

### Game Flow

The game progresses through 5 distinct scenes:
1. **Scene 1: Narrative Terminal** - DOS-style terminal with typewriter effect and CRT shader
2. **Scene 2: NetHack Sequence** - ASCII dungeon exploration
3. **Scene 3: Wizardry Party** - Party management and character creation
4. **Scene 4: Tile Dungeon** - Grid-based dungeon crawling
5. **Scene 5: Pixel Combat** - Turn-based combat system

## Development Workflow

### Prerequisites
- Godot 4.5 (Mono/.NET version)
- .NET 8.0 SDK
- Git

### Setup Instructions
1. Clone the repository
2. Download LLM model (Qwen3-4B-Instruct Q4_K_M ~2.5GB)
3. Open in Godot
4. Download NobodyWho plugin binaries
5. Enable NobodyWho plugin
6. Build the C# project

### Pre-Commit Workflow
The project enforces the following checks before every commit:
1. **Code Formatting**: `dotnet format --verify-no-changes`
2. **Static Analysis & Linting**: `dotnet build --warnaserror`
3. **Automated Tests**: `dotnet test`
4. **Security Scan**: Trivy (optional)

## Building and Running

### Running in Godot Editor
Press F5 or click the Play button in the top-right corner.

### From Command Line
```bash
godot --path . --verbose
```

### Testing
Run all tests with:
```bash
dotnet test
```

## Development Conventions

The project follows Microsoft's C# coding conventions with the following key points:
- Use 4 spaces for indentation
- Use Allman-style braces
- Use PascalCase for public members, camelCase for private members
- Use prefixes for private fields (underscore)
- Write XML documentation comments for public members
- Use async/await for I/O operations
- Follow async naming conventions (method names ending in "Async")

## AI/LLM Integration

The game uses the NobodyWho plugin to run local LLMs for:
- Three Dreamweaver Personas: Hero, Shadow, and Ambition guides
- Omega System: Meta-narrator weaving player choices into coherent narrative
- Dynamic Responses: Context-aware dialogue based on game state

## Key Scripts

- **SceneManager.cs**: Handles scene transitions
- **GameState.cs**: Manages global game state and persistence
- **PartyData.cs**: Manages party composition and inventory
- **Character.cs**: Defines character properties and methods
- **Enums.cs**: Contains key enumerations

## Important Notes

- The project uses an enforced pre-commit workflow that requires all checks to pass before committing
- LLM models are not included in the repository due to size constraints
- The project requires specific binaries for the NobodyWho plugin that are platform-specific
- The game uses Godot autoloads for global state management
- Save data is stored in a JSON format in the user's directory
