---
description: "Task list for Ωmega Spiral Godot 4 Implementation"
---

# Tasks: Ωmega Spiral Godot 4 Implementation

**Input**: Design documents from `/specs/002-using-godot-4/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Tests are REQUIRED following player-driven TDD approach. Tests validate player interactions and game state changes, enabling MVP validation while implementation completes. Contract tests validate data schemas; integration tests validate player experience scenarios.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Godot Project**: `/Source/Scenes/`, `/Source/Scripts/`, `/Source/Data/`, `/Source/UI/`, `/Tests/`
- Paths shown below follow the planned Godot project structure

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic Godot structure

 - [ ] T001 [P] Configure pre-commit git hooks for build error prevention (Constitution Principle III)
 - [X] T002 Initialize Godot 4.5 project with C# 14 and .NET 10 RC configuration
 - [X] T003 [P] Configure Godot project settings for Windows and Linux export
 - [X] T004 [P] Set up Godot autoload singleton for SceneManager in project settings
 - [X] T005 Create basic directory structure: /Source/Scenes/, /Source/Scripts/, /Source/Data/, /Source/UI/, /Tests/


## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

 - [X] T006 Create GameState singleton script in /Source/Scripts/GameState.cs
 - [X] T007 [P] Implement JSON schema validation system in /Source/Scripts/JsonSchemaValidator.cs
 - [X] T008 [P] Create SceneLoader system with async LoadSceneAsync method in /Source/Scripts/SceneLoader.cs
 - [X] T009 Create SceneManager autoload singleton in /Source/Scripts/SceneManager.cs
 - [X] T010 Create NarratorEngine for dialogue processing in /Source/Scripts/NarratorEngine.cs
 - [X] T011 Set up input action mappings in Godot Project Settings
 - [X] T012 Create base manifest.json file in /Source/Data/manifest.json
 - [X] T013 Configure JSON.NET dependency for C# JSON handling
 - [ ] T013A [P] Refactor JSON loading to use async patterns (LoadJsonAsync methods) per Constitution Principle VI

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Narrative Terminal Scene (Priority: P1) 🎯 MVP

**Goal**: Implement Godot-driven narrative terminal interface where players make choices that determine Dreamweaver alignment

**Independent Test**: Can be fully tested by running the narrative terminal scene inside Godot, making thread choices, answering questions, and verifying correct narrative progression and state updates without relying on hardcoded text.

### Tests for User Story 1 (REQUIRED - Player-Driven)

- [X] T014 [P] [US1] Contract test for narrative terminal schema validation in /Tests/NarrativeTerminalSchemaTests.cs
- [X] T015 [P] [US1] Integration test for player choice interactions and state updates in /Tests/NarrativeTerminalIntegrationTests.cs

### Implementation for User Story 1

- [X] T016 [P] [US1] Create NarrativeSceneData model in /Source/Scripts/NarrativeSceneData.cs
- [X] T017 [P] [US1] Create DreamweaverChoice model in /Source/Scripts/DreamweaverChoice.cs
- [X] T018 [US1] Create Scene1Narrative.tscn scene file in /Source/Scenes/Scene1Narrative.tscn
- [X] T019 [US1] Create NarrativeTerminal.cs script in /Source/Scripts/NarrativeTerminal.cs
- [X] T020 [US1] Create NarrativeTerminal UI scene in /Source/UI/NarrativeTerminal.tscn
- [X] T021 [US1] Add typewriter effect implementation to NarrativeTerminal.cs
- [X] T022 [US1] Implement choice handling and thread selection in NarrativeTerminal.cs
- [X] T023 [US1] Add player name input and secret question handling
- [X] T024 [US1] Implement JSON data loading for narrative variations in Scene1Narrative.tscn
- [X] T025 [US1] Add scene transition to NetHack scene in NarrativeTerminal.cs
- [X] T026 [US1] Create hero.json, shadow.json, ambition.json data files in /Source/Data/scenes/scene1_narrative/

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - NetHack ASCII Dungeon Exploration (Priority: P1)

**Goal**: Implement three sequential ASCII dungeon rooms where each room is owned by a different Dreamweaver and contains three interactive objects that influence Dreamweaver alignment scoring

**Independent Test**: Can be fully tested by navigating each dungeon's ASCII grid, interacting with objects (door/monster/chest), verifying Dreamweaver scoring, and confirming the final Dreamweaver selection.

### Tests for User Story 2 (REQUIRED - Player-Driven)

- [ ] T027 [P] [US2] Contract test for ASCII dungeon schema validation in /Tests/AsciiDungeonSchemaTests.cs
- [ ] T028 [P] [US2] Integration test for player interactions, Dreamweaver scoring, and alignment selection in /Tests/DreamweaverScoringTests.cs

### Implementation for User Story 2

- [X] T029 [P] [US2] Create DungeonSequenceData model in /Source/Scripts/DungeonSequenceData.cs
- [X] T030 [P] [US2] Create DungeonRoom model in /Source/Scripts/DungeonRoom.cs
- [X] T031 [P] [US2] Create DungeonObject model in /Source/Scripts/DungeonObject.cs
- [X] T032 [US2] Create Scene2NethackSequence.tscn scene file in /Source/Scenes/Scene2NethackSequence.tscn
- [X] T033 [US2] Create AsciiRoomRenderer.cs script in /Source/Scripts/AsciiRoomRenderer.cs
- [X] T034 [US2] Implement ASCII grid rendering in AsciiRoomRenderer.cs
- [X] T035 [US2] Add player movement and collision detection in AsciiRoomRenderer.cs
- [X] T036 [US2] Implement object interaction system in AsciiRoomRenderer.cs
- [X] T037 [US2] Add Dreamweaver scoring logic in GameState.cs
- [X] T038 [US2] Create dungeon layout JSON files in /Source/Data/scenes/scene2_nethack/
- [X] T039 [US2] Implement scoring calculation and alignment bonuses
- [X] T040 [US2] Add scene transition to party creation scene

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently

---

## Phase 5: User Story 3 - Wizardry Party Character Creation (Priority: P2)

**Goal**: Implement party character creation with classes, races, and stats using classic CRPG UI patterns

**Independent Test**: Can be fully tested by selecting party members, assigning classes/races, viewing stats, and verifying the party data is correctly saved and persisted.

### Tests for User Story 3 (REQUIRED - Player-Driven)

- [ ] T041 [P] [US3] Contract test for party creation schema validation in /Tests/PartyCreationSchemaTests.cs
- [ ] T042 [P] [US3] Integration test for player party creation flow and data persistence in /Tests/PartyPersistenceTests.cs

### Implementation for User Story 3

- [X] T043 [P] [US3] Create PartyData model in /Source/Scripts/PartyData.cs
- [X] T044 [P] [US3] Create Character model in /Source/Scripts/Character.cs
- [X] T045 [P] [US3] Create CharacterStats model in /Source/Scripts/CharacterStats.cs
- [X] T046 [US3] Create Scene3WizardryParty.tscn scene file in /Source/Scenes/Scene3WizardryParty.tscn
- [X] T047 [US3] Create PartyCreator.cs script in /Source/Scripts/PartyCreator.cs
- [X] T048 [US3] Create PartyCreatorUI.tscn in /Source/UI/PartyCreatorUI.tscn
- [X] T049 [US3] Implement class and race selection UI in PartyCreator.cs
- [X] T050 [US3] Add stat calculation and validation in PartyCreator.cs
- [X] T051 [US3] Implement party confirmation and data saving in PartyCreator.cs
- [X] T052 [US3] Create party schema and variants in /Source/Data/scenes/scene3_wizardry/
- [X] T053 [US3] Add scene transition to tile dungeon scene
- [X] T054 [US3] Implement party data persistence in GameState.cs

**Checkpoint**: At this point, User Stories 1, 2, AND 3 should all work independently

---

## Phase 6: User Story 4 - RPG 2D Tile Dungeon (Priority: P2)

**Goal**: Implement 2D tile-based dungeon exploration in the style of Final Fantasy 1-3 with proper navigation and interactions

**Independent Test**: Can be fully tested by navigating the tile map, interacting with doors and objects, using UI panels, and verifying correct scene transitions.

### Tests for User Story 4 (REQUIRED - Player-Driven)

- [ ] T055 [P] [US4] Contract test for tile dungeon schema validation in /Tests/TileDungeonSchemaTests.cs
- [ ] T056 [P] [US4] Integration test for player tile navigation, object interactions, and exit conditions in /Tests/TileNavigationTests.cs

### Implementation for User Story 4

- [X] T057 [P] [US4] Create TileDungeonData model in /Source/Scripts/TileDungeonData.cs
- [X] T058 [P] [US4] Create TileDefinition model in /Source/Scripts/TileDefinition.cs
- [X] T059 [P] [US4] Create DungeonUI model in /Source/Scripts/DungeonUI.cs
- [X] T060 [US4] Create Scene4TileDungeon.tscn scene file in /Source/Scenes/Scene4TileDungeon.tscn
- [X] T061 [US4] Create TileDungeonController.cs script in /Source/Scripts/TileDungeonController.cs
- [X] T062 [US4] Create TileDungeonUI.tscn in /Source/UI/TileDungeonUI.tscn
- [X] T063 [US4] Implement TileMap navigation and collision in TileDungeonController.cs
- [X] T064 [US4] Add door and object interaction system in TileDungeonController.cs
- [X] T065 [US4] Implement UI panel toggling (inventory, map, stats) in TileDungeonController.cs
- [X] T066 [US4] Create tile dungeon JSON files in /Source/Data/scenes/scene4_tile_dungeon/
- [X] T067 [US4] Add exit condition checking and scene transition logic
- [X] T068 [US4] Implement exit condition satisfaction (open_door, find_key, etc.)

**Checkpoint**: At this point, User Stories 1, 2, 3, AND 4 should all work independently

---

## Phase 7: User Story 5 - Final Fantasy Turn-Based Combat (Priority: P2)

**Goal**: Implement turn-based combat with pixel art sprites and retro sound effects using classic JRPG mechanics

**Independent Test**: Can be fully tested by initiating combat, selecting actions, executing turns, and verifying win/loss conditions and rewards.

### Tests for User Story 5 (REQUIRED - Player-Driven)

- [ ] T069 [P] [US5] Contract test for pixel combat schema validation in /Tests/PixelCombatSchemaTests.cs
- [ ] T070 [P] [US5] Integration test for player combat actions, turn queue execution, and victory/defeat conditions in /Tests/CombatMechanicsTests.cs

### Implementation for User Story 5

- [X] T071 [P] [US5] Create CombatSceneData model in /Source/Scripts/CombatSceneData.cs
- [X] T072 [P] [US5] Create CombatEnemy model in /Source/Scripts/CombatEnemy.cs
- [X] T073 [P] [US5] Create CombatAction model in /Source/Scripts/CombatAction.cs
- [X] T074 [US5] Create Scene5PixelCombat.tscn scene file in /Source/Scenes/Scene5PixelCombat.tscn
- [X] T075 [US5] Create PixelCombatController.cs script in /Source/Scripts/PixelCombatController.cs
- [X] T076 [US5] Create PixelCombatUI.tscn in /Source/UI/PixelCombatUI.tscn
- [X] T077 [P] [US5] Implement turn-based combat system in PixelCombatController.cs
- [X] T078 [US5] Add action selection and resolution (FIGHT, MAGIC, ITEM, RUN) in PixelCombatController.cs
- [X] T079 [US5] Implement HP management and victory/defeat conditions in PixelCombatController.cs
- [X] T080 [US5] Create combat JSON files in /Source/Data/scenes/scene5_ff_combat/
- [X] T081 [US5] Add sprite rendering and animation system
- [X] T082 [US5] Implement final Dreamweaver selection based on scoring

**Checkpoint**: All user stories should now be independently functional

---

## Phase 8: User Story 6 - Cross-Scene State Management (Priority: P3)

**Goal**: Implement state persistence across all game scenes so choices, inventory, and progress persist with lasting consequences

**Independent Test**: Can be fully tested by playing through multiple scenes, verifying state persistence, and checking that choices affect subsequent scenes.

### Tests for User Story 6 (OPTIONAL) ⚠️

- [X] T083 [P] [US6] Integration test for cross-scene state persistence in /Tests/StatePersistenceTests.cs
- [X] T084 [P] [US6] Test for save/load functionality in /Tests/SaveLoadTests.cs

### Implementation for User Story 6

- [X] T085 [P] [US6] Enhance GameState.cs with comprehensive save/load functionality
- [X] T086 [US6] Implement FileAccess-based save system in GameState.cs
- [X] T087 [US6] Add scene progression tracking in GameState.cs
- [X] T088 [US6] Implement shard collection and inventory management in GameState.cs
- [X] T089 [US6] Add state validation across scene transitions in SceneManager.cs
- [X] T090 [US6] Create save game JSON schema in /Source/Data/schemas/savegame_schema.json

**Checkpoint**: Complete game flow with persistent state across all scenes

---

## Phase 9: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [X] T091 [P] Documentation updates in /Source/Scripts/README.md
- [ ] T099 [P] Performance benchmarking and optimization validation in /Tests/PerformanceTests.cs
- [ ] T100 [P] Cross-platform compatibility testing for Windows and Linux in /Tests/CrossPlatformTests.cs
- [ ] T092 Performance optimization across all scenes (maintain 60 FPS, optimize JSON loading under 100ms, scene transitions under 500ms)
- [ ] T093 [P] Asset pipeline setup for sprites, audio, and fonts
- [ ] T094 [P] Additional unit tests in /Tests/ (if requested)
- [X] T095 Security hardening and input validation
- [X] T096 Run quickstart.md validation and cross-platform testing
- [X] T097 Export configuration for Windows and Linux
- [ ] T101 TDD compliance verification and test coverage validation per constitution requirements
- [X] T098 Final integration testing across all user stories

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3+)**: All depend on Foundational phase completion
  - User stories can then proceed in parallel (if staffed)
  - Or sequentially in priority order (P1 → P2 → P3)
- **Polish (Final Phase)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 (P1)**: Can start after Foundational (Phase 2) - May integrate with US1 but should be independently testable
- **User Story 3 (P2)**: Can start after Foundational (Phase 2) - May integrate with US1/US2 but should be independently testable
- **User Story 4 (P2)**: Can start after Foundational (Phase 2) - May integrate with US1/US2/US3 but should be independently testable
- **User Story 5 (P2)**: Can start after Foundational (Phase 2) - May integrate with US1/US2/US3/US4 but should be independently testable
- **User Story 6 (P3)**: Can start after Foundational (Phase 2) - Integrates with all previous stories for state management

### Within Each User Story

- Models before services
- Services before scene implementation
- Core implementation before integration
- Story complete before moving to next priority

### Parallel Opportunities

- All Setup tasks marked [P] can run in parallel
- All Foundational tasks marked [P] can run in parallel (within Phase 2)
- Once Foundational phase completes, all user stories can start in parallel (if team capacity allows)
- All models within a story marked [P] can run in parallel
- Different user stories can be worked on in parallel by different team members

---

## Parallel Example: User Story 1

```bash
# Launch all models for User Story 1 together:
Task: "Create NarrativeSceneData model in /Source/Scripts/NarrativeSceneData.cs"
Task: "Create DreamweaverChoice model in /Source/Scripts/DreamweaverChoice.cs"

# Launch scene and script creation together:
Task: "Create Scene1Narrative.tscn scene file in /Source/Scenes/Scene1Narrative.tscn"
Task: "Create NarrativeTerminal.cs script in /Source/Scripts/NarrativeTerminal.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Test User Story 1 independently
5. Deploy/demo if ready

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 → Test independently → Deploy/Demo (MVP!)
3. Add User Story 2 → Test independently → Deploy/Demo
4. Add User Story 3 → Test independently → Deploy/Demo
5. Add User Story 4 → Test independently → Deploy/Demo
6. Add User Story 5 → Test independently → Deploy/Demo
7. Add User Story 6 → Test independently → Deploy/Demo
8. Each story adds value without breaking previous stories

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1
   - Developer B: User Story 2
   - Developer C: User Story 3
   - Developer D: User Story 4
   - Developer E: User Story 5
3. Stories complete and integrate independently

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Each user story maintains the Godot project structure and C# 14 architecture

## MVP Focus

- **Functional completeness** over performance optimization
- **Player experience validation** over strict metrics
- **Iterative improvement** - optimize after core gameplay validates
- Performance targets deferred to post-MVP: Focus on responsiveness, not specific FPS/timing
- Test coverage target: >30% for MVP, >50% for production (Constitution compliance)
