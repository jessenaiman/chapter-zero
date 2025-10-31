# Design Architecture Overview for Ωmega Spiral – Chapter Zero

## 1. High‑Level Layering

| Layer | Responsibility | Key Types / Services |
|-------|----------------|----------------------|
| **Core Engine** | Godot 4.6‑dev‑2 runtime, scene tree, input, rendering. | `Node`, `SceneTree`, `Input` |
| **Domain / Gameplay** | Game rules, turn‑based combat, narrative spirals, Dreamweaver threads. | `GameState`, `CombatManager`, `NarrativeEngine` |
| **Frontend UI** | UI widgets, HUD, menus, visual feedback. | `Control` nodes, `OmegaUI` classes, `DesignService` |
| **Design System** | Centralised colour palette, shader presets, UI tokens. | `omega_spiral_colors_config.json`, `DesignService`, `ColorsConfig`, `ShaderPreset` |
| **Backend / Services** | Persistence, AI‑driven narrative, data loading. | `DataService`, `AIClient`, `SaveLoadManager` |
| **Infrastructure** | Project configuration, build scripts, test harness (gdUnit4). | `.csproj`, `dotnet` tasks, `GdUnit4` tests |

The architecture follows **separation of concerns**: each layer only depends on the layer directly below it. UI never touches the raw JSON file; it asks `DesignService` for colours or shader presets.

## 2. Design System

* **Configuration file** – `omega_spiral_colors_config.json` stores the *design system* (colour palette) and *shader presets*.
* **Static façade** – `DesignService` lazily loads the JSON, validates required entries, and exposes:
  * `GetColor(string name)` → `Godot.Color`
  * `TryGetShaderPreset(string key, out ShaderPreset?)`
  * `TryGetShaderDefaults(string key, out IReadOnlyDictionary<string, object>)`
* **Strong typing** – `ColorsConfig`, `ColorValue`, `ShaderPreset` DTOs map the JSON structure, enabling compile‑time safety.
* **Fallbacks** – Missing keys emit a warning and fall back to white, ensuring the game never crashes because of a missing colour.

All UI components (buttons, panels, combat labels) reference colours by logical names (`warm_amber`, `light_thread`, …) rather than hard‑coded hex values. Shader materials are instantiated from the preset definitions, keeping visual tweaks data‑driven.

## 3. Scene & Node Organization

* **Root scenes** – `test_scene.tscn`, `boot_sequence.tscn`, etc., contain only the minimal node hierarchy required for that screen.
* **Component scripts** – Each node that needs behaviour has a dedicated C# script following **PascalCase** naming and placed next to the scene file.
* **Reusable UI components** – Encapsulated in separate scenes (e.g., `DialogBox.tscn`) and instantiated via `PackedScene`.
* **Runtime creation** – When a UI element needs a colour or shader, it calls `DesignService` at `_Ready` or when the theme changes.

## 4. Gameplay Flow

1. **Boot** – `BootSequence` loads the config via `DesignService`, applies the `boot_sequence` shader preset.
2. **Main Loop** – `GameState` drives turn‑based logic, invokes `NarrativeEngine` to fetch AI‑generated dialogue.
3. **Combat** – `CombatManager` uses `DesignService.GetColor("damage_color")` and `heal_color` for UI feedback.
4. **Scene Transitions** – `ISceneRunner` (gdUnit4) loads the next scene, disposing of the previous one via `AutoFree`.

## 5. Testing Strategy (gdUnit4)

* **Unit tests** – Pure C# classes (`GameState`, `CombatManager`) are tested with standard xUnit‑style assertions.
* **Integration tests** – Use `ISceneRunner` to load a scene, simulate input, and verify UI state.
* **Mocking** – `DesignService` can be mocked to return deterministic colours/presets, isolating UI tests from file I/O.

## 6. Extensibility Guidelines

* **Add a new colour** – Extend `omega_spiral_colors_config.json` under `design_system`, then reference it via `DesignService.GetColor("new_key")`.
* **Add a shader preset** – Add an entry under `shader_presets` with a `shader` path and `parameters`. Retrieve it with `TryGetShaderPreset`.
* **New UI component** – Create a scene, attach a C# script, and use the design service for theming.
* **New gameplay feature** – Implement it in the domain layer, expose a service, and keep UI‑only code in the frontend layer.

---

*This memory provides a concise reference for developers and for Serena when answering architecture‑related questions about the Ωmega Spiral project.*