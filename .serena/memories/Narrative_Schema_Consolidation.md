# Narrative Schema Consolidation - Oct 29, 2025

## Single Source of Truth: NarrativeScriptElement.cs

All YAML narrative schema classes consolidated into ONE file:

### Classes Defined:
1. **ChoiceOption** - Single choice in a question
   - owner: string (light/shadow/ambition)
   - text: string

2. **NarrativeScriptElement** - Single scene/beat
   - id: string
   - owner: string (system, omega, light, shadow, ambition, npc, none)
   - lines: List<string>
   - question: string
   - choice: List<ChoiceOption>
   - glitchStart, glitchEnd: bool
   - timing: string
   - pause: float

3. **NarrativeScriptRoot** - Root document (top-level)
   - title: string
   - speaker: string
   - description: string
   - scenes: List<NarrativeScriptElement>

## Breaking Changes:
- ❌ OLD: NarrativeScript class (REMOVED)
- ❌ OLD: NarrativeScene class (REMOVED)
- ✅ NEW: All references updated to use NarrativeScriptRoot and NarrativeScriptElement

## Files Updated:
- ✅ NarrativeScriptElement.cs - Single source of truth
- ✅ NarrativeEngine.cs - Uses NarrativeScriptRoot, NarrativeScriptElement
- ✅ INarrativeHandler.cs - Uses NarrativeScriptElement
- ✅ NarrativeScriptLoader.cs - Returns NarrativeScriptRoot
- ✅ GhostStageManager.cs - Uses NarrativeScriptRoot, NarrativeScriptElement

## Files Still Need Updates:
- ⏳ NarrativeUi.cs - Update ApplySceneEffectsAsync parameter
- ⏳ GhostUi.cs - Implement INarrativeHandler with NarrativeScriptElement
- ⏳ GhostDataLoader.cs - Update to use NarrativeScriptRoot
- ⏳ CinematicDirector.cs - Update constraint to NarrativeScriptRoot
- ⏳ NethackDirector.cs - Update to use NarrativeScriptRoot

## Testing Strategy:
Three TDD tests created:
1. GhostTerminalSceneLoadTests - Scene loads with GhostStageManager
2. GhostTerminalTextDisplayTests - Terminal displays text
3. GhostTerminalTypewriterTests - Typewriter animation + first line
