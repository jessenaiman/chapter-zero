# GhostUi Refactoring Complete

## What We Did

**Refactored GhostUi from 425 lines to ~180 lines** by removing ALL presentation logic that NarrativeUi already handles.

### Before (Over-engineered):
```csharp
GhostUi.PresentNarrativeMomentAsync()
  ├─ Manually calls ShaderController.ApplyVisualPresetAsync()
  ├─ Manually applies pause timing
  ├─ Manually handles typewriter effect
  └─ Duplicates NarrativeUi.PlayNarrativeSequenceAsync()

GhostUi.PresentQuestionMomentAsync()  
  ├─ Manually loops through choices
  ├─ Manually calls AppendTextAsync() for each
  └─ Duplicates NarrativeUi.PresentChoicesAsync()
```

### After (Clean Architecture):
```csharp
GhostUi.PresentNarrativeMomentAsync()
  └─ Calls base.PlayNarrativeSequenceAsync() 
     (handles shader, pause, typewriter, everything)

GhostUi.PresentQuestionMomentAsync()
  └─ Calls base.PresentChoicesAsync()
     (handles all choice UI)
```

## Key Changes

1. **Removed all shader management** - NarrativeUi handles this via `ApplyVisualPresetAsync()`
2. **Removed all text rendering** - NarrativeUi handles via `AppendTextAsync()`
3. **Removed all pause logic** - NarrativeUi handles via `PlayNarrativeSequenceAsync()`
4. **Added ConvertToNarrativeBeats()** - Simple converter from ContentBlock → NarrativeBeat[]
5. **Kept ONLY Ghost-specific logic:**
   - Script loading from ghost.yaml
   - Dreamweaver score tracking
   - Secret reveal ceremony (special visual logic)
   - Determining dominant thread at completion

## What Now Happens

### Flow:
1. GhostUi loads ghost.yaml → NarrativeScript with ContentBlocks
2. For each ContentBlock:
   - Convert to NarrativeBeat[]
   - Call base.PlayNarrativeSequenceAsync(beats)
   - **NarrativeUi handles EVERYTHING**: shaders, text, timing

### Shader Effects Now Work Correctly:
- Preset applied via `ApplyVisualPresetAsync()`
- 500ms delay allows shader to settle (`DelaySeconds`)
- Then typewriter text displays over the shader
- Player sees the glitch effect DURING text, not before

## Files Still to Clean Up

- **GhostNarrativeData.cs** - DELETE (old JSON structure, unused)
- **GhostNarrator.cs** - DELETE (only has TryParsePause(), already in base)

## Benefits

✅ **Simpler code** - 425 → ~180 lines  
✅ **No duplication** - Base class handles presentation  
✅ **Better maintainability** - Changes to presentation logic in one place  
✅ **Correct architecture** - Ghost is just a script orchestrator  
✅ **Shader effects work** - Timing is now correct (preset applies, settles, text renders)
