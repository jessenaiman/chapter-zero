# SOLID Refactoring Roadmap - Omega Spiral

## Overview

This roadmap provides a prioritized, phased approach to addressing SOLID principle violations identified in the architecture assessment. The plan focuses on incremental improvements with clear success criteria and risk mitigation.

## Priority Matrix

| Priority | System | Impact | Effort | Risk | Timeline |
|----------|--------|--------|---------|------|----------|
| **P0** | Terminal System | High | Low | Low | 1 week |
| **P1** | Combat System | High | High | Medium | 2-3 weeks |
| **P2** | Scene Management | Medium | Medium | Low | 1 week |
| **P3** | Event System | Low | Medium | Low | 1 week |

## Phase 1: Complete Terminal Refactoring (P0)

### Objective
Complete the remaining 4 tasks from the existing Terminal SRP refactoring initiative.

### Tasks
- [ ] **Task 13**: Run full test suite and fix failing tests (47 tests currently failing)
- [ ] **Task 14**: Fix scene structure and signal emission issues
- [ ] **Task 15**: Resolve stage button test discrepancies (expects 5, finds 7)
- [ ] **Task 16**: Verify build with --warnaserror passes

### Success Criteria
- ✅ All 138 tests passing
- ✅ No orphan node warnings
- ✅ `dotnet build --warnaserror` exits with code 0
- ✅ All SOLID principles properly implemented

### Dependencies
- Existing terminal component architecture (75% complete)
- GdUnit4 testing framework

---

## Phase 2: Combat System Refactoring (P1)

### Objective
Break down the monolithic Combat.cs into focused, single-responsibility components following SOLID principles.

### Current Violations Addressed
- **SRP**: Combat.cs handles 6+ distinct responsibilities
- **OCP**: Event handling not extensible
- **DIP**: Direct dependencies on Godot APIs

### Proposed Architecture

#### Component Breakdown
```
Combat (Coordinator)
├── ICombatLifecycleManager
│   └── CombatLifecycleManager
├── ICombatMusicHandler
│   └── CombatMusicHandler
├── ICombatTransitionHandler
│   └── CombatTransitionHandler
├── ICombatResultHandler
│   └── CombatResultHandler
└── ICombatResultStrategy (Strategy Pattern)
    ├── ExperienceResultStrategy
    ├── LootResultStrategy
    └── NarrativeResultStrategy
```

#### Interface Definitions

**ICombatLifecycleManager.cs**
```csharp
public interface ICombatLifecycleManager
{
    Task StartCombatAsync(PackedScene arena);
    Task EndCombatAsync(bool isPlayerVictory);
    event EventHandler<bool> CombatEnded;
    CombatState CurrentState { get; }
}
```

**ICombatMusicHandler.cs**
```csharp
public interface ICombatMusicHandler
{
    Task SwitchToCombatMusicAsync(AudioStream combatMusic);
    Task RestorePreviousMusicAsync();
    AudioStream? GetCurrentTrack();
}
```

**ICombatTransitionHandler.cs**
```csharp
public interface ICombatTransitionHandler
{
    Task FadeToBlackAsync(float duration);
    Task FadeFromBlackAsync(float duration);
    Task ShowLoadingScreenAsync();
    Task HideLoadingScreenAsync();
}
```

### Implementation Plan

#### Sprint 1: Analysis & Design (Week 1)
- [ ] Analyze current Combat.cs dependencies
- [ ] Design interface contracts
- [ ] Create unit test specifications
- [ ] Plan integration test scenarios

#### Sprint 2: Component Extraction (Week 2)
- [ ] Extract CombatMusicHandler
- [ ] Extract CombatTransitionHandler
- [ ] Extract CombatResultHandler
- [ ] Create Strategy pattern for results

#### Sprint 3: Integration & Testing (Week 3)
- [ ] Refactor main Combat class to use composition
- [ ] Implement comprehensive unit tests
- [ ] Create integration tests
- [ ] Performance testing and optimization

### Risk Mitigation
- **Incremental rollout**: Extract one component at a time
- **Feature flags**: Keep original implementation as fallback
- **Comprehensive testing**: 100% unit test coverage for new components

---

## Phase 3: Scene Management Abstraction (P2)

### Objective
Create abstraction layer for scene loading to improve testability and reduce coupling.

### Current Issues
- Direct Godot SceneTree dependencies
- No interface abstraction
- Difficult to mock for testing

### Proposed Solution

#### ISceneLoader Interface
```csharp
public interface ISceneLoader
{
    Task<SceneLoadResult> LoadSceneAsync(string scenePath);
    bool ValidateSceneExists(string scenePath);
    Task<SceneLoadResult> PreloadSceneAsync(string scenePath);
    void UnloadScene(string scenePath);
}
```

#### GodotSceneLoader Implementation
```csharp
public class GodotSceneLoader : ISceneLoader
{
    private readonly SceneTree _sceneTree;

    public GodotSceneLoader(SceneTree sceneTree)
    {
        _sceneTree = sceneTree ?? throw new ArgumentNullException(nameof(sceneTree));
    }

    public async Task<SceneLoadResult> LoadSceneAsync(string scenePath)
    {
        // Implementation using Godot APIs
    }
}
```

### Implementation Plan
- [ ] Create ISceneLoader interface
- [ ] Implement GodotSceneLoader
- [ ] Create MockSceneLoader for testing
- [ ] Refactor SceneManager to use abstraction
- [ ] Add comprehensive unit tests

---

## Phase 4: Event System Improvements (P3)

### Objective
Reduce string-based event dependencies and improve type safety.

### Current Issues
- Heavy reliance on string-based signals
- No compile-time verification
- Difficult to refactor event names

### Proposed Solution

#### Typed Event System
```csharp
public interface IEventAggregator
{
    void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IGameEvent;
    void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IGameEvent;
    void Publish<TEvent>(TEvent gameEvent) where TEvent : IGameEvent;
}

public interface IGameEvent { }

public class CombatInitiatedEvent : IGameEvent
{
    public PackedScene Arena { get; }
    public CombatInitiatedEvent(PackedScene arena) => Arena = arena;
}
```

### Implementation Plan
- [ ] Design event type hierarchy
- [ ] Create IEventAggregator interface
- [ ] Implement GodotEventAggregator
- [ ] Create strongly-typed combat events
- [ ] Migrate existing string-based events

---

## Testing Strategy

### Unit Testing Requirements
- **Coverage Target**: > 80% for all new components
- **Mocking**: All external dependencies must be mockable
- **TDD Approach**: Write tests before implementation where possible

### Integration Testing
- **Component Integration**: Test component interactions
- **End-to-End Scenarios**: Full workflow validation
- **Performance Testing**: Ensure no regressions

### Quality Gates
- [ ] All tests passing before merge
- [ ] Code coverage above threshold
- [ ] Performance benchmarks met
- [ ] SOLID principles verified

---

## Rollout Strategy

### Incremental Deployment
1. **Component-by-component**: Extract and deploy one component at a time
2. **Feature flags**: Keep original implementation as fallback option
3. **Gradual migration**: New code can coexist with old during transition

### Monitoring & Rollback
- **Health checks**: Monitor system stability post-deployment
- **Performance monitoring**: Track key metrics for regressions
- **Rollback plan**: Clear steps to revert if issues arise

---

## Success Metrics

### Code Quality
- [ ] Cyclomatic complexity < 10 per method
- [ ] Class responsibility = 1 per class
- [ ] Interface methods < 5 per interface
- [ ] Dependencies < 3 per class

### Architecture Compliance
- [ ] SRP: Each class has one reason to change
- [ ] OCP: Extensions don't modify existing code
- [ ] LSP: Subtypes substitutable for base types
- [ ] ISP: No forced dependencies on unused methods
- [ ] DIP: Depend on abstractions, not concretions

### Operational
- [ ] Zero-downtime deployments
- [ ] No performance regressions
- [ ] All tests passing in CI/CD
- [ ] Code coverage above 80%

---

## Next Actions

1. **Immediate**: Complete Terminal refactoring (Tasks 13-16)
2. **Short-term**: Begin Combat system analysis and design
3. **Medium-term**: Create detailed Combat component specifications
4. **Long-term**: Implement Scene management abstractions

## Maintenance

This roadmap should be reviewed and updated quarterly as the codebase evolves and new architectural needs emerge.