# Omega UI Architecture Investigation

## User's Vision
- OmegaUi is THE system - provides frame, layout, colors, styling EVERYTHING
- Subclasses (MenuUi, NarrativeUi, etc.) inherit styling, add their content
- Style comes from OmegaSpiralColors JSON config
- Need to understand why it's "not loading right"

## Current Questions
1. Where is style actually coming from? (Scene hardcoded values? Config? Not at all?)
2. How is MenuUi inheriting from OmegaUi in practice?
3. What related classes are part of the Omega UI system?
4. Why isn't the menu getting the style from OmegaUi?

## Next Steps
1. Find all classes related to OmegaUi system
2. Check scene hierarchy (main_menu.tscn, omega_ui.tscn relationship)
3. Trace color flow from JSON → OmegaSpiralColors → actual UI rendering
4. Identify the architectural disconnect
