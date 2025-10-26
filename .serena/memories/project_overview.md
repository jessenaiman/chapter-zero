# Omega Spiral - Chapter Zero Project Overview

## Project Purpose
Omega Spiral - Chapter Zero is a narrative-driven turn-based RPG that blends retro gaming aesthetics with modern AI-powered storytelling. Players navigate through five distinct scenes representing different eras of gaming history, making choices that influence dynamic narratives guided by three AI Dreamweaver personas.

## Core Features
- **5 Unique Scenes**: DOS terminals, NetHack ASCII, Wizardry party management, dungeon crawling, pixel combat
- **AI-Driven Narratives**: Three Dreamweaver personas (Hero, Shadow, Ambition) + Omega meta-narrator
- **Retro Aesthetics**: CRT effects, terminal interfaces, ASCII art, pixel graphics
- **Character Progression**: Party management, stats, persistent save system
- **Spiral Storytelling**: Emergent narratives from player choices and AI adaptation

## Technical Architecture
- **Frontend**: Godot 4.6-dev2 game engine with C# scripting
- **Backend**: .NET 10 RC2 runtime
- **Language**: C# 14 (preview features enabled)
- **AI Integration**: NobodyWho plugin for local LLM inference (Qwen 3B model)
- **Testing**: GdUnit4 framework with NUnit compatibility
- **Persistence**: Entity Framework Core with SQLite/In-Memory options
- **UI Framework**: Godot's node-based UI with custom shaders
- **Audio**: Godot's audio system with custom bus layout

## Codebase Structure
```
source/
├── assets/          # Game assets and resources
├── audio/           # Audio files and management
├── combat/          # Turn-based combat system
├── common/          # Shared utilities and base classes
├── data/            # Data models and serialization
├── dialogue/        # Dialogue system and AI integration
├── domain/          # Domain logic and business rules
├── dreamweavers/    # AI persona implementations
├── field/           # Overworld/field exploration
├── infrastructure/  # Cross-cutting concerns (logging, DI, etc.)
├── media/           # Media processing and shaders
├── narrative/       # Narrative engine and story management
├── overworld/       # World map and navigation
├── persistence/     # Save/load system
├── resources/       # Godot resources and configurations
├── scripts/         # Utility scripts
├── services/        # Application services
├── shaders/         # CRT and visual effects
├── stages/          # Scene implementations (5 game stages)
└── ui/              # User interface components
```

## Development Principles
- **Clean Architecture**: Separation of concerns with domain-driven design
- **Test-Driven Development**: Comprehensive test coverage with GdUnit4
- **Performance-First**: Optimized for smooth 60fps gameplay
- **Accessibility**: Keyboard/mouse controls with future controller support
- **Modularity**: Plugin-based architecture for extensibility

## Target Platforms
- **Primary**: Linux (development platform)
- **Secondary**: Windows, macOS
- **Future**: Mobile platforms via Godot's export system

## Quality Standards
- **Code Coverage**: >80% target
- **Performance**: 60fps minimum, <100ms load times
- **Security**: Regular vulnerability scanning
- **Documentation**: XML docs for public APIs, ADR for architecture decisions

## Team Structure
- **Technical Lead**: Adam (jessenaiman)
- **AI Integration**: NobodyWho plugin ecosystem
- **Testing**: GdUnit4 community framework
- **LLM Models**: Alibaba Cloud Qwen series

## Development Workflow
1. Feature branch from main
2. Implement with tests
3. Code review and testing
4. Merge via PR with CI checks
5. Deploy and monitor

This project represents the intersection of retro gaming nostalgia and cutting-edge AI technology, creating unique emergent storytelling experiences.