# Ghost Stage - Real Architecture Understanding

## What Ghost SHOULD Be

Ghost should be **just a script loader and scene controller**, NOT a complex presenter.

### The Stack (Correct):
```
NarrativeUi (base class - handles ALL presentation)
  ├─ PlayNarrativeSequenceAsync(beats[]) - handles shader + text rendering
  ├─ TransitionPersonaAsync(thread) - handles color transitions  
  ├─ PresentChoicesAsync(prompt, choices[]) - handles choice UI
  └─ All shader/audio/text logic is here
    
GhostUi (stage-specific, minimal)
  ├─ Load ghost.yaml
  ├─ Convert ContentBlock[] to NarrativeBeat[] for base class
  ├─ Track Dreamweaver scores (stage-specific logic only)
  ├─ Handle secret reveal ceremony (stage-specific visual)
  └─ Call base class methods to do the actual presentation
```

## What Ghost Should NOT Do

Ghost should NOT:
- ❌ Apply shader presets directly (base class does it)
- ❌ Handle typewriter effects (base class does it)
- ❌ Manage pause timing (base class does it)
- ❌ Handle choice rendering (base class does it)
- ❌ Call ShaderController (base class does it)

## The Real Problem

GhostUi is **reimplementing what NarrativeUi already provides**. It should be:

1. Load script from ghost.yaml
2. Convert each ContentBlock into the appropriate base class call:
   - "narrative" block → convert to NarrativeBeat[], call PlayNarrativeSequenceAsync()
   - "question" block → extract choices, call PresentChoicesAsync()
   - "composite" block → combine above

3. Handle stage-specific logic only:
   - Track dreamweaver scores
   - Special secret reveal ceremony
   - Determine dominant thread at end

## What Needs to Happen

Rewrite GhostUi to be ~150 lines instead of 425 by delegating to base class. The base class is already perfect - we just need to use it.

ContentBlock already has:
- `VisualPreset` - for shader application
- `Lines` - for text display  
- `Pause` - for timing
- `Setup`, `Prompt`, `Options`, `Continuation` - for questions

Ghost just needs to convert these to calls that the base class already handles.
