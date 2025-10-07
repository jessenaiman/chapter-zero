# Ωmega Spiral Constitution

## Core Principles

### I. SOLID Architecture (NON-NEGOTIABLE)
All code must follow SOLID principles: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion. Every class and method should have a clear, single purpose and be easily testable in isolation.

### II. YAGNI (NON-NEGOTIABLE) 
Always implement things when you actually need them, never when you just foresee that you need them. No speculative generality or premature optimization. Features added must be immediately useful.

### III. Performance First
All scenes must maintain 60 FPS target. Performance benchmarks must be established before implementation. No feature should be added that degrades performance below specified targets without explicit performance justification.

### IV. Test-Driven Development (NON-NEGOTIABLE)
All code must be testable and tested. Tests written before implementation. Red-Green-Refactor cycle strictly enforced. Every user story must have corresponding tests before being considered complete.

### V. DRY (Don't Repeat Yourself)
All code should be reusable and maintainable. Duplication of logic, constants, or business rules is prohibited. Common functionality must be extracted into reusable components.

## Development Workflow

All code changes must be backed by tests. Code reviews must verify SOLID compliance and performance requirements. Performance regression testing is mandatory for all features.

## Governance

This constitution supersedes all other practices. Any deviation must be documented with explicit justification and approved by the development team. All PRs must verify compliance with these principles.

**Version**: 1.0.0 | **Ratified**: 2025-10-07 | **Last Amended**: 2025-10-07