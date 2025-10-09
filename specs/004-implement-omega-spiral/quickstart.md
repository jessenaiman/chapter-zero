# Quickstart Guide: Ωmega Spiral Godot 4 Implementation

## Project Overview

Ωmega Spiral is a retro-styled narrative adventure game built with Godot 4.5 and C# 14. The game features five interconnected scenes that create a unique Dreamweaver alignment system where player choices influence narrative progression and gameplay.

- **Scene 1**: Godot-driven narrative terminal interface
- **Scene 2**: NetHack-style ASCII dungeon exploration
- **Scene 3**: Wizardry party character creation
- **Scene 4**: Eye of the Beholder 2D tile dungeon
- **Scene 5**: Final Fantasy turn-based pixel combat

## Prerequisites

### Required Software
- **Godot 4.5+** with Mono support enabled
- **.NET 10 RC SDK** for C# 14 features
- **Visual Studio 2022 17.12+** (recommended) or VS Code with C# extension
- **Git** for version control

### Recommended Tools
- **NUnit 3** testing framework
- **Godot NUnit Integration** for in-editor testing
- **JSON Schema Validator** for data validation

## Project Setup

### 1. Clone and Initialize
```bash
git clone <repository-url>
cd omega-spiral/chapter-zero
git checkout 002-using-godot-4
```

### 2. Verify Godot Installation
```bash
godot --version
# Should output: 4.5.stable.mono (or higher)
```

### 3. Install Dependencies
```bash
# Install .NET 10 RC SDK if not already installed
dotnet --version
# Should output: 10.0.100-rc.1.xxxxx (or similar)

# Install NUnit for testing
dotnet add package NUnit --version 3.14.0
dotnet add package NUnit3TestAdapter --version 4.5.0
```

### 4. Project Structure Verification
Verify the project structure matches the planned architecture:

```
specs/002-using-godot-4/
├── plan.md              # Implementation plan (this file)
├── research.md          # Technical research findings
├── data-model.md        # Data architecture documentation
├── quickstart.md        # This quickstart guide
└── contracts/           # JSON schemas for all scene types
    ├── manifest_schema.json
    ├── scene1_narrative_schema.json
    ├── scene2_nethack_schema.json
    ├── scene3_wizardry_schema.json
    ├── scene4_tile_dungeon_schema.json
    └── scene5_ff_combat_schema.json
```

## Development Workflow

### 1. Godot Project Setup
```bash
# Launch Godot editor
godot .

# In Godot Editor:
# 1. Create new project (if project.godot doesn't exist)
# 2. Enable Mono support in Project Settings > Mono
# 3. Set up autoload scripts for SceneManager
# 4. Configure input actions for game controls
```

### 2. Scene Development Process
For each scene, follow this development pattern:

#### Scene 1: Narrative Terminal
```csharp
// 1. Create scene file
godot --create-scene Scene1Narrative.tscn

// 2. Implement NarrativeTerminal.cs script
// 3. Add JSON data files for each thread (hero/shadow/ambition)
// 4. Test typewriter effects and choice handling
```

#### Scene 2: ASCII Dungeon Sequence
```csharp
// 1. Create AsciiRoomRenderer.cs script
// 2. Design dungeon layouts in JSON format
// 3. Implement movement and collision detection
// 4. Add object interaction system
```

#### Scene 3: Wizardry Party Creation
```csharp
// 1. Create PartyCreator.cs script
// 2. Design party creation UI in Godot editor
// 3. Implement stat generation and validation
// 4. Add party data persistence
```

#### Scene 4: Tile Dungeon
```csharp
// 1. Create TileDungeonController.cs script
// 2. Set up TileMap node structure
// 3. Implement collision detection and pathfinding
// 4. Add interactive objects and UI panels
```

#### Scene 5: Pixel Combat
```csharp
// 1. Create PixelCombatController.cs script
// 2. Implement turn-based combat system
// 3. Add sprite rendering and animation
// 4. Integrate with party data and stats
```

### 3. Data Integration
```csharp
// Example: Loading scene data
public void LoadSceneData(string scenePath)
{
    var jsonText = File.ReadAllText($"res://Source/Data/scenes/{scenePath}/data.json");
    var sceneData = Json.Parse(jsonText).AsGodotDictionary();

    // Validate against schema
    var schemaValidator = new JsonSchemaValidator();
    var isValid = schemaValidator.Validate(sceneData, scenePath + "_schema.json");

    if (!isValid) {
        GD.PrintErr($"Invalid scene data: {schemaValidator.ErrorMessage}");
        return;
    }

    // Load scene with validated data
    LoadSceneWithData(sceneData);
}
```

### 4. Testing Strategy
```csharp
// Unit tests for individual systems
[Test]
public void TestDreamweaverScoring()
{
    var gameState = new GameState();
    gameState.ProcessDungeonChoice(DreamweaverType.Light, "door");
    Assert.AreEqual(2, gameState.DreamweaverScores[DreamweaverType.Light]);
}

// Integration tests for scene transitions
[Test]
public void TestSceneTransitionWithState()
{
    var sceneManager = new SceneManager();
    var gameState = CreateTestGameState();

    sceneManager.TransitionToScene("scene2_nethack", gameState);
    Assert.AreEqual(2, gameState.CurrentScene);
}

// Data validation tests
[Test]
public void TestSceneDataValidation()
{
    var validator = new SceneDataValidator();
    var sceneData = LoadTestSceneData();

    Assert.IsTrue(validator.ValidateSceneData(sceneData));
}
```

## Key Development Commands

### Godot Commands
```bash
# Launch editor
godot .

# Export for Windows
godot --export "Windows Desktop"

# Export for Linux
godot --export "Linux/X11"

# Run headless (for CI/testing)
godot --headless --script test_runner.cs
```

### .NET Commands
```bash
# Build all scripts
dotnet build

# Run tests
dotnet test

# Run specific test category
dotnet test --filter "Category=Scene1"

# Check code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Data Management
```bash
# Validate all JSON schemas
find specs/002-using-godot-4/contracts/ -name "*.json" -exec validate_json_schema {} \;

# Generate test data variations
generate_scene_variants --scene scene1_narrative --threads hero,shadow,ambition

# Backup game data
cp -r Source/Data/ backups/$(date +%Y%m%d_%H%M%S)_data
```

## Quality Assurance Checklist

### Before Starting Development
- [ ] Godot 4.5 installed and configured with Mono support
- [ ] .NET 10 RC SDK installed and verified
- [ ] All JSON schemas validate successfully
- [ ] Project structure matches plan specifications
- [ ] Git branch is clean and up to date

### During Development
- [ ] All C# scripts compile without errors
- [ ] Scene data validates against schemas
- [ ] Game state persists correctly between scenes
- [ ] 60 FPS performance maintained across all scenes
- [ ] Input handling works with keyboard only
- [ ] Save/load functionality works correctly

### Before Committing
- [ ] All tests pass
- [ ] Code follows C# naming conventions
- [ ] Godot scene files are saved and committed
- [ ] JSON data files are validated
- [ ] No hardcoded strings or magic numbers
- [ ] Documentation is updated

## Performance Targets

- **Frame Rate**: 60 FPS minimum across all scenes
- **Load Times**: Scene transitions under 500ms
- **Memory Usage**: Under 500MB during gameplay
- **JSON Loading**: Under 100ms for all scene data
- **Input Latency**: Under 16ms for responsive controls

## Support and Troubleshooting

### Common Issues

**Godot Mono Issues**:
```bash
# Clear Mono cache
rm -rf .mono/

# Rebuild Mono assemblies
godot --build-mono-glue
```

**C# Compilation Errors**:
```bash
# Clean and rebuild
dotnet clean
dotnet build --force
```

**Scene Loading Issues**:
```csharp
# Enable Godot debugging
OS.SetWindowTitle($"Debug: {scenePath}")
GD.Print($"Loading scene: {scenePath}")
```

### Getting Help
1. Check the Godot documentation for API usage
2. Review the JSON schema validation errors
3. Consult the data-model.md for entity relationships
4. Check the research.md for technical decisions and alternatives

## Next Steps

1. **Complete Scene Implementation**: Follow the order in the manifest (Scene 1→2→3→4→5)
2. **Add Asset Pipeline**: Set up texture, audio, and font import workflows
3. **Implement Save System**: Add persistent game state management
4. **Performance Optimization**: Profile and optimize for target platforms
5. **Cross-Platform Testing**: Test on Windows and Linux target platforms

This quickstart guide provides the foundation for developing the Ωmega Spiral game. Refer to the detailed specifications in `spec.md` and the technical research in `research.md` for comprehensive implementation guidance.