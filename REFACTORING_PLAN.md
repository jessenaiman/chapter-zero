# Code Quality Refactoring Plan

## Overview

This document outlines the architectural refactoring to improve SOLID principles compliance, reduce file sizes, and improve maintainability of the Omega Spiral C# codebase.

## Identified Issues

### 1. Large Files Violating SRP

- `UIEquipment.cs` (2066 lines) - Mixed responsibilities: UI rendering, business logic, state management
- `UIInventory.cs` (1540 lines) - Similar issues to UIEquipment
- `UICharacterCreation.cs` (1228 lines) - Complex character creation flow mixed with UI
- `UIQuestLog.cs` (1071 lines) - Quest display and quest management logic combined
- `UICombatLog.cs` (1061 lines) - Combat logging and display logic combined
- `UIQuestTracker.cs` (1031 lines) - Quest tracking business logic + UI
- `NarrativeTerminal.cs` (809 lines) - Scene management, typewriter, input handling all mixed

### 2. Missing Abstractions

- No interfaces for services (equipment, inventory, quest management)
- Direct dependencies on Godot nodes throughout business logic
- No dependency injection or inversion of control

### 3. Poor Folder Organization

- All 100+ files in flat Scripts/ directory
- No separation between Models, Services, UI, Controllers

## Refactoring Strategy

### Phase 1: Extract Interfaces (Dependency Inversion)

Create interfaces for all major services:

- ✅ `IEquipmentService` - Equipment management operations
- ✅ `INarrativeSceneService` - Scene schema loading and navigation
- ✅ `ITypewriterService` - Text rendering with typewriter effect
- ✅ `IEquipmentDisplayFactory` - UI component creation
- ⏳ `IInventoryService` - Inventory management operations
- ⏳ `IQuestService` - Quest tracking and management
- ⏳ `ICombatService` - Combat flow and turn management
- ⏳ `ICharacterService` - Character creation and management

### Phase 2: Implement Service Classes (Single Responsibility)

Create focused service implementations:

- ✅ `EquipmentService` - Pure business logic for equipment
- ✅ `NarrativeSceneService` - JSON schema management
- ✅ `TypewriterService` - Text animation logic
- ⏳ `InventoryService` - Item management
- ⏳ `QuestService` - Quest state management
- ⏳ `CombatService` - Combat rules engine
- ⏳ `CharacterService` - Character operations

### Phase 3: Create Proper Folder Structure

Organize files logically:

```
Source/Scripts/
├── Core/                    # Core game systems
│   ├── GameState.cs
│   ├── SceneManager.cs
│   └── SceneLoader.cs
├── Models/                  # Data models and DTOs
│   ├── Character.cs
│   ├── Item.cs
│   ├── Quest.cs
│   ├── EquipmentSlot.cs
│   └── SceneSchema.cs
├── Services/                # Business logic services
│   ├── EquipmentService.cs
│   ├── InventoryService.cs
│   ├── QuestService.cs
│   ├── CombatService.cs
│   ├── CharacterService.cs
│   ├── NarrativeSceneService.cs
│   └── TypewriterService.cs
├── Interfaces/              # Service contracts
│   ├── IEquipmentService.cs
│   ├── IInventoryService.cs
│   └── ...
├── Controllers/             # Godot node controllers
│   ├── PlayerController.cs
│   ├── GamepieceController.cs
│   └── TileDungeonController.cs
├── UI/                      # UI components
│   ├── Components/          # Reusable UI widgets
│   ├── Character/           # Character-related UI
│   │   ├── UICharacterSheet.cs
│   │   └── UICharacterCreation.cs
│   ├── Combat/              # Combat UI
│   │   ├── UICombatHud.cs
│   │   ├── UICombatMenu.cs
│   │   ├── UICombatLog.cs
│   │   └── UITurnBar.cs
│   ├── Inventory/           # Inventory/equipment UI
│   │   ├── UIInventory.cs
│   │   └── UIEquipment.cs
│   ├── Quest/               # Quest UI
│   │   ├── UIQuestLog.cs
│   │   └── UIQuestTracker.cs
│   └── Dialogue/            # Dialogue/narrative UI
│       ├── UIDialogue.cs
│       └── NarrativeTerminal.cs
├── Combat/                  # Combat systems
│   ├── Battler.cs
│   ├── BattlerStats.cs
│   ├── Combat.cs
│   ├── CombatAI.cs
│   └── ActiveTurnQueue.cs
├── Data/                    # Data schemas and loaders
│   ├── NarrativeSceneData.cs
│   ├── CombatSceneData.cs
│   ├── DungeonSequenceData.cs
│   └── PartyData.cs
└── Observers/               # Dreamweaver observers
    ├── DreamweaverSystem.cs
    ├── HeroObserver.cs
    ├── ShadowObserver.cs
    └── AmbitionObserver.cs
```

### Phase 4: Refactor Large UI Classes

Break down large UI classes into smaller, focused components:

#### UIEquipment.cs (2066 lines) → Multiple classes

1. `UIEquipmentPanel.cs` (200 lines) - Main UI coordinator
2. `EquipmentSlotDisplay.cs` (150 lines) - Individual slot rendering
3. `EquipmentDetailsPanel.cs` (150 lines) - Item details display
4. `EquipmentActionsPanel.cs` (150 lines) - Action buttons (equip/unequip/drop)
5. Use `IEquipmentService` for business logic
6. Use `IEquipmentDisplayFactory` for UI creation

#### NarrativeTerminal.cs (809 lines) → Multiple classes

1. `NarrativeTerminalController.cs` (300 lines) - Main coordinator
2. `NarrativeInputHandler.cs` (150 lines) - Input and choice handling
3. `NarrativeRenderer.cs` (150 lines) - Text display and effects
4. Use `INarrativeSceneService` for scene management
5. Use `ITypewriterService` for text animation

#### UIInventory.cs (1540 lines) → Multiple classes

1. `UIInventoryPanel.cs` (250 lines) - Main UI coordinator
2. `InventoryGridDisplay.cs` (200 lines) - Grid rendering
3. `InventoryItemDisplay.cs` (150 lines) - Individual item rendering
4. `InventoryFilterPanel.cs` (150 lines) - Filtering and sorting
5. Use `IInventoryService` for business logic

### Phase 5: Add XML Documentation

- Add complete XML documentation to all public members
- Follow AGENTS.md guidelines strictly
- Include `<summary>`, `<param>`, `<returns>`, `<exception>`
- Use `<see cref>` for type references
- Use `<see langword>` for keywords

### Phase 6: Move Files to New Structure

- Create all necessary folders
- Move files systematically
- Update namespaces
- Update `using` directives
- Verify compilation after each batch

### Phase 7: Quality Verification

- Run `dotnet build` to ensure no compilation errors
- Run Codacy CLI analysis
- Run tests
- Verify pre-commit checks pass
- Update checklist items in specs/004-implement-omega-spiral/checklists/

## Implementation Priority

1. ✅ Create folder structure
2. ✅ Create core interfaces
3. ✅ Implement core services
4. ⏳ Refactor NarrativeTerminal (highest impact, referenced in Scene1)
5. ⏳ Move and organize existing files
6. ⏳ Refactor UI components one at a time
7. ⏳ Add comprehensive XML documentation
8. ⏳ Final quality verification

## Benefits

- **Single Responsibility**: Each class has one clear purpose
- **Dependency Inversion**: Business logic doesn't depend on UI/Godot
- **Interface Segregation**: Small, focused interfaces
- **Open/Closed**: Easy to extend without modification
- **Maintainability**: Easier to find, understand, and modify code
- **Testability**: Services can be unit tested independently
- **Scalability**: Clear structure for adding new features

## Next Steps

1. Continue implementing remaining service interfaces
2. Begin refactoring NarrativeTerminal as pilot
3. Move files into proper folder structure
4. Apply learnings to remaining large files
