# Ωmega Spiral Test Suite

This directory contains the comprehensive test suite for the Ωmega Spiral: Chapter Zero game. The tests are organized by category and purpose to ensure thorough coverage of all game systems.

## Test Organization

### Unit Tests

- **Narrative**: Tests for narrative terminal functionality and dialogue systems
- **SaveLoad**: Tests for save/load functionality and persistence
- **Dungeon**: Tests for dungeon generation and gameplay mechanics
- **Common**: Tests for shared utilities and common components

### Integration Tests

- **SceneIntegration**: Tests for scene loading, transitions, and integration
- **SystemIntegration**: Tests for cross-system integration and workflows
- **EndToEnd**: Complete end-to-end gameplay scenarios

### UI Tests

- **ContentBlock**: Tests for content presentation and rendering
- **InputHandling**: Tests for various input methods and controls
- **Accessibility**: Tests for accessibility features and UX compliance

### Functional Tests

- **NarrativeFlow**: Tests for complete narrative progression and branching
- **DungeonGameplay**: Tests for dungeon exploration and combat systems
- **ErrorHandling**: Tests for error conditions and edge cases

### Audio/Visual Tests

- **Audio**: Tests for sound effects, music, and audio systems
- **Visual**: Tests for visual effects, rendering, and graphics

### Localization Tests

- **TextHandling**: Tests for text rendering and internationalization
- **Internationalization**: Tests for multi-language support

### Performance Tests

- **LoadTime**: Tests for scene loading and asset loading performance
- **Runtime**: Tests for runtime performance and optimization

## Test Categories Mapping

The tests are mapped to the comprehensive test matrix defined in `test-cases-clean.md`:

### Narrative & Dialogue Systems (SC-XXX)

- Scene loading and instantiation
- Dialogue prompt evaluation
- Choice response handling
- Narrative flow validation

### Content Block Presentation (CB-XXX)

- Text persistence and timing
- Input handling for progression
- Visual presentation and effects
- Choice selection mechanisms

### Dreamweaver Scoring (DW-XXX)

- Affinity calculation and tracking
- Choice-to-score mapping
- Scoring persistence and restoration
- Dominant Dreamweaver determination

### LLM/ NobodyWho Integration (NL-XXX)

- Dynamic narrative generation
- Fallback mechanism validation
- Response caching and replay

### Dungeon Stage Sequences (STG-XXX)

- Stage loading and validation
- Ownership and alignment mechanics
- Stage progression and transitions

### Save/Load & Persistence (SL-XXX)

- Game state preservation
- Score and progress restoration
- Error handling for corrupted data

### Error Handling & Edge Cases (ERR-XXX)

- Invalid input validation
- Recovery from interruption
- Graceful degradation

### Accessibility & UX (ACC-XXX)

- Text scaling and readability
- Input method support
- Navigation and skip functionality

### Audio/Visual Polish (AV-XXX)

- Sound effect mixing
- Visual effects and transitions
- Performance optimization

### Localization (LOC-XXX)

- Text wrapping and rendering
- Unicode and special character support
- Internationalization features

## Running Tests

Due to Godot integration complexities, tests are organized to run in different contexts:

1. **Unit Tests**: Can run independently with standard .NET test runner
2. **Integration Tests**: Require Godot engine context
3. **UI Tests**: Require running game instance
4. **Performance Tests**: Run in profiling context

## Test Data and Fixtures

Test data is organized in corresponding directories under each test category. Shared fixtures and helper classes are available in the `TestInfrastructure` directory.

## Continuous Integration

Tests are integrated with the CI/CD pipeline to ensure:

- Automated test execution on commit
- Coverage reporting and quality gates
- Performance benchmarking
- Cross-platform compatibility verification

## Coverage Goals

The test suite aims for comprehensive coverage across all 79 test cases defined in the test matrix, with particular focus on:

1. **Core Narrative Flow** (Priority 1)
2. **Save/Load Systems** (Priority 1)
3. **Dungeon Gameplay** (Priority 2)
4. **UI/UX Compliance** (Priority 2)
5. **Error Handling** (Priority 3)
6. **Performance Optimization** (Priority 3)

## Contributing

To add new tests:

1. Identify the appropriate test category
2. Follow the existing naming conventions
3. Use the established test patterns and helpers
4. Ensure tests are deterministic and isolated
5. Add appropriate assertions and documentation

## Test Execution Pipeline

The test execution follows this pipeline:

1. Unit tests run first for fast feedback
2. Integration tests validate system interactions
3. UI tests verify presentation and behavior
4. Performance tests ensure optimization goals
5. Coverage reports generated for quality metrics
