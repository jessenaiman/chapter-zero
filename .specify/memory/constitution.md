<!--
Sync Impact Report:
- Version: 1.0.0 (Initial Constitution)
- Sections Added: 7 Core Principles + 3 Additional Sections
- Ratification Date: 2025-10-10
- Templates Requiring Updates:
  ✅ plan-template.md - Constitution Check section aligns with principles
  ✅ spec-template.md - Requirements align with Data-Driven and Documentation principles
  ✅ tasks-template.md - Task structure supports Test-First and Schema-Driven development
- Follow-up TODOs: None - All placeholders filled
-->

# Omega Spiral - Chapter Zero Constitution

A narrative-driven Godot game blending retro aesthetics with modern AI-powered storytelling,
built on principles of data-driven design, quality enforcement, and comprehensive documentation.

## Core Principles

### I. Data-Driven Architecture (NON-NEGOTIABLE)

All narrative content, game configuration, and scene definitions MUST be externalized as JSON/YAML data files:

- **Narrative as Data**: Dialogue, choices, and scene logic defined in external JSON—never hardcoded
- **Schema Validation**: Every JSON file MUST have a corresponding schema for validation
- **Per-Scene Schemas**: Each scene type has its own JSON schema with strict validation
- **Plug-and-Play Scenes**: Adding new scenes = drop JSON file + register in manifest
- **API-Ready Structure**: JSON structure mirrors future REST/GraphQL response format

**Rationale**: Enables content teams to iterate on narrative and game data without code changes,
supports rapid testing, and provides clear contracts between game systems.

### II. Documentation First

All public code elements MUST be documented with XML comments before implementation:

- **XML Documentation**: Public members MUST have `<summary>`, `<param>`, `<returns>`, `<remarks>`
- **Internal Documentation**: Complex internal members SHOULD be documented for maintainability
- **ADR for Decisions**: Architecture decisions MUST be captured in ADRs (docs/adr/)
- **Code Examples**: Use `<example>` tags with `<code language="csharp">` for complex APIs
- **Cross-References**: Use `<see cref>` and `<seealso>` to link related types/members

**Rationale**: Documentation-first ensures code intent is clear before implementation, supports
onboarding, and provides IntelliSense support for developers.

### III. Quality Gates (NON-NEGOTIABLE)

Zero tolerance for build errors; warnings are addressed before commit:

- **Zero Build Errors**: Code MUST compile without errors (enforced by pre-commit hooks)
- **StyleCop Compliance**: SA-series warnings MUST be resolved (member ordering, documentation, formatting)
- **FxCop Compliance**: CA-series warnings MUST be addressed (code quality, security, globalization)
- **Nullability Safety**: All CS8xxx nullability warnings MUST be resolved with proper null handling
- **Pre-Commit Enforcement**: Git hooks MUST block commits containing build errors

**Rationale**: Consistent code quality prevents technical debt accumulation, improves maintainability,
and ensures codebase remains healthy for long-term development.

### IV. Test-Driven Development

Tests MUST be written and verified before implementation:

- **Contract Tests**: JSON schema validation tests for all data contracts
- **Integration Tests**: Scene loading, state persistence, system interaction tests
- **Unit Tests**: Core logic and utility function tests using NUnit framework
- **Test Coverage**: >50% coverage for core systems (narrative, combat, state management)
- **Test Organization**: Tests mirror source structure in Tests/ directory

**Rationale**: TDD ensures requirements are clear, code is testable, and regressions are caught early.
The project already has comprehensive tests for narrative, persistence, and schema validation.

### V. Schema-Driven Contracts

All data interfaces between systems MUST use validated schemas:

- **JSON Schema**: All JSON files MUST have corresponding JSON Schema Draft 2020-12 definitions
- **Validation on Load**: JsonSchemaValidator MUST validate all loaded data against schemas
- **Contract Versioning**: Breaking schema changes require version increments and migration paths
- **Shared Schemas**: Common entities (stats, abilities, items) use shared schema references
- **Error Handling**: Schema validation failures MUST be logged with clear diagnostic messages

**Rationale**: Schema-driven development ensures data contracts are explicit, validated, and
versioned, reducing integration errors and supporting API evolution.

### VI. Async-First Programming

Asynchronous operations MUST follow C# best practices:

- **Async Suffix**: All async methods MUST use the 'Async' suffix
- **Task Return Types**: Return `Task<T>` or `Task`, never async void (except event handlers)
- **Cancellation Support**: Long-running operations MUST accept CancellationToken parameters
- **ConfigureAwait**: Use `ConfigureAwait(false)` in library code to prevent deadlocks
- **Exception Handling**: Use try/catch around await expressions; propagate with `Task.FromException()`

**Rationale**: Proper async patterns prevent deadlocks, improve responsiveness, and align with
modern C# practices. Critical for LLM integration and scene loading operations.

### VII. Modular System Architecture

Systems MUST communicate through well-defined interfaces with minimal coupling:

- **Interface Segregation**: Systems depend on narrow interfaces (ICombatFeed, INarrativeFeed, etc.)
- **Adapter Pattern**: Use adapters to translate between domain models and Godot nodes
- **Autoload Singletons**: Core systems (GameState, SceneManager, DreamweaverSystem) are autoloaded
- **Signal-Based Communication**: Use Godot signals for cross-system event notification
- **Namespace Organization**: Follow noun-based namespace hierarchy (entities, systems, adapters)

**Rationale**: Clean separation of concerns enables independent development, testing, and iteration
of game systems. Follows SOLID principles and DIP (Dependency Inversion Principle).

## Performance Standards

All game systems MUST meet these performance targets:

- **Frame Rate**: Maintain 60 FPS during gameplay with <16ms frame time
- **Scene Transitions**: Complete scene transitions in <500ms including data loading
- **JSON Loading**: Load and validate JSON data files in <100ms per file
- **LLM Response Time**: First token from LLM within 3s on mid-range GPU
- **Memory Footprint**: Keep total memory usage <2GB including LLM model
- **Build Time**: Full C# rebuild completes in <30s on development machines

**Testing**: Performance metrics MUST be validated during QA phases and optimized before release.

## Technology Stack Requirements

Core technologies are mandated for consistency and support:

- **Engine**: Godot 4.5 (Mono/.NET version) - enables C# scripting
- **Language**: C# 14 (preview features) with .NET 10 RC runtime
- **Testing**: NUnit 3.14+ with NUnit3TestAdapter for test execution
- **Serialization**: System.Text.Json for JSON parsing (Newtonsoft.Json as fallback)
- **Validation**: JsonSchemaValidator for schema validation of all data files
- **LLM Integration**: NobodyWho plugin for local GGUF model inference
- **Code Quality**: StyleCop.Analyzers + FxCopAnalyzers for static analysis

**Upgrade Policy**: Technology updates require ADR approval and migration plan.

## Development Workflow

All code changes MUST follow this workflow:

1. **Branch Creation**: Feature branches follow `###-feature-name` convention
2. **Specification**: Create spec in `/specs/###-feature-name/spec.md` with user stories
3. **Planning**: Generate implementation plan with `/speckit.plan` command
4. **Implementation**: Write tests first, implement features, document all public APIs
5. **Quality Check**: Run `dotnet build` (zero errors), `dotnet test` (all passing)
6. **Commit**: Pre-commit hooks verify build success and test passage
7. **Review**: Pull requests require spec adherence verification
8. **Integration**: Merge to default branch after approval

**Enforcement**: Pre-commit hooks block commits with errors; CI/CD blocks merges with failures.

## Governance

This constitution supersedes all other development practices and guidelines.

**Amendment Process**:

- Constitution changes require ADR documenting rationale and impact
- Version increments follow semantic versioning:
  - **MAJOR**: Backward incompatible principle removals or redefinitions
  - **MINOR**: New principles added or materially expanded guidance  
  - **PATCH**: Clarifications, wording fixes, non-semantic refinements
- All amendments MUST update Sync Impact Report (HTML comment at top of file)
- Template files MUST be updated to reflect principle changes

**Compliance Verification**:

- All feature specifications MUST include "Constitution Check" section
- Implementation plans MUST verify adherence to core principles
- Code reviews MUST verify documentation, testing, and quality gate compliance
- Complexity violations MUST be justified with rationale in plan.md

**Guidance Files**:

- Runtime development guidance: `.github/copilot-instructions.md`
- C# specific standards: `.github/instructions/csharp.instructions.md`
- Architecture patterns: `.github/instructions/dotnet-architecture-good-practices.instructions.md`
- Codacy integration: `.github/instructions/codacy.instructions.md`

**Version**: 1.0.0 | **Ratified**: 2025-10-10 | **Last Amended**: 2025-10-10
