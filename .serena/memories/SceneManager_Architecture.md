# SceneManager Architecture

## Overview
`SceneManager` is a Godot `Node` that orchestrates the lifecycle of a narrative scene. It loads a `.tscn` file, creates a UI handler, displays narrative lines, applies scene effects, presents choices, processes the selected choice, and handles optional pause timing.

## Key Responsibilities
- **Scene Data Management**: Holds a `StoryScriptElement` (`SceneData`) and an `object` (`AdditionalData`).
- **Handler Resolution**: `GetOrCreateUiHandler` searches the scene tree for a node implementing `IStoryHandler` (e.g., `NarrativeUi` or `GhostUi`).
- **Execution Flow** (`RunSceneAsync`):
  1. Retrieve the main `SceneTree`.
  2. Resolve a UI handler.
  3. Delegate to `RunSceneWithHandlerAsync`.
- **Narrative Playback** (`RunSceneWithHandlerAsync`):
  - Print debug info.
  - Display lines via `handler.DisplayLinesAsync`.
  - Apply scene effects via `handler.ApplySceneEffectsAsync`.
  - If a question exists, present choices and process the selected `ChoiceOption` using `handler.ProcessChoiceAsync` (guarded against null).
  - Perform a final pause if specified.

## Error Handling & Logging
- Uses `GD.Print`/`GD.PrintErr` for diagnostics.
- Wraps the whole flow in a `try/catch` to log exceptions and stack traces.

## Extensibility
- Subclasses can override:
  - `GetOrCreateUiHandler` to provide custom UI handlers.
  - `RunSceneWithHandlerAsync` for bespoke scene processing.
- The class is `partial`, allowing additional generated code (e.g., Godot's partial class for signals).

## Interaction with Other Systems
- **`IStoryHandler`**: Interface defining UI operations (`DisplayLinesAsync`, `ApplySceneEffectsAsync`, `PresentChoiceAsync`, `ProcessChoiceAsync`).
- **`ChoiceOption`**: Represents a selectable choice; passed to the handler for processing.
- **`StoryScriptElement`**: Data model containing narrative lines, question text, choice list, pause duration, etc.

---
*Memory captured for future reference by the Serena system.*
