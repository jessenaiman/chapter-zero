# Ωmega Spiral Project Overview

## Project Purpose
Ωmega Spiral is a retro-styled narrative adventure game that combines classic CRPG mechanics with modern Godot 4.5 + C# 14 implementation. The game features multiple interconnected scenes that test player alignment with Dreamweaver archetypes (Hero/Light, Shadow/Wrath, Ambition/Mischief) through narrative choices and gameplay mechanics.

## Key Features
- Narrative terminal with typewriter effects and choice-driven storytelling
- ASCII dungeon exploration with Dreamweaver alignment scoring
- Classic CRPG party creation with classes, races, and stats
- 2D tile-based dungeon navigation
- Turn-based combat with pixel art sprites
- Cross-scene state persistence and save/load functionality

## Technical Architecture
- **Engine**: Godot 4.5 with Mono/C# support
- **Language**: C# 14 with .NET 10 RC
- **Data**: JSON files for scene content and game state
- **State Management**: Singleton autoloads (GameState, SceneManager, NarratorEngine)
- **Scene Structure**: 5 main scenes with MVC-like controller architecture
- **Performance**: 60 FPS target, scene transitions under 500ms, JSON loading under 100ms

## Development Workflow
- Edit C# scripts in Source/Scripts/
- Design scenes in Godot editor (.tscn files)
- Configure JSON data in Source/Data/
- Test with NUnit framework
- Export to Windows/Linux platforms