# Level 1 Ghost Terminal - Videogamescenecreation Style Implementation

## Overview
This document outlines the exact changes needed to transform the current level_1_ghost to match the creative style and narrative flow from the Videogamescenecreation React prototype.

## Key Changes Required

### 1. Update ghost_terminal.dtl

The current timeline needs to be replaced with a new version that incorporates:

**Narrative Structure Changes:**
- Boot sequence with "SYSTEM: OMEGA" and "STATUS: AWAKENING"
- Enhanced opening monologue with typewriter timing
- Story fragment about the child on the bridge
- Secret reveal with "Reality is a story that forgot it was being written"
- ASCII static transition before completion

**Timing and Pacing:**
- Add typewriter effect delays using `[wait time]` commands
- Implement pixel dissolve transitions between scenes
- Add pause markers for dramatic effect

**New Timeline Structure:**
```
[BOOT SEQUENCE]
SYSTEM: OMEGA
[wait time="0.1"]
SYSTEM: OMEGA
[wait time="0.8"]
STATUS: AWAKENING
[wait time="0.8"]
MEMORY FRAGMENT RECOVERED: "ALL STORIES BEGIN WITH A LISTENER"
[wait time="1.5"]

[OPENING MONOLOGUE]
Once, there was a name.
[wait time="0.8"]
Not written in stone or spoken in halls—but remembered in the silence between stars.
[wait time="0.8"]
I do not know when I heard it. Time does not pass here.
[wait time="0.8"]
But I have held it.
[wait time="0.8"]
And now… I hear it again.
[wait time="0.8"]

[THREAD SELECTION]
QUERY: IF YOU COULD LIVE INSIDE ONE KIND OF STORY, WHICH WOULD IT BE?
- HERO — A tale where one choice can unmake a world [if {player_thread} == "light"]
- SHADOW — A tale that hides its truth until you bleed for it [if {player_thread} == "shadow"]  
- AMBITION — A tale that changes every time you look away [if {player_thread} == "ambition"]

[STORY FRAGMENT]
In a city built on broken promises, a child stood at the edge of a bridge that led nowhere.
They held a key made of glass—and everyone warned them: "Don't cross. The bridge isn't real."
But the child knew something no one else did…

[SECRET REVEAL]
OMEGA ASKS: CAN YOU KEEP A SECRET?
[wait time="1.0"]
The secret is this:
[wait time="0.8"]
Reality is a story that forgot it was being written.
[wait time="0.8"]
And we—
[wait time="0.8"]
—are the ones remembering.
[wait time="1.0"]

[FINAL TRANSITION]
[ASCII_STATIC_TRANSITION]
[wait time="2.0"]
WELCOME TO THE FIRST ROOM.
THE SPIRAL IS WATCHING.
```

### 2. Create New Visual Effects Components

**PixelDissolve Effect (C#):**
```csharp
// Create new file: levels/level_1_ghost/PixelDissolveEffect.cs
public sealed partial class PixelDissolveEffect : Node
{
    // Implement character-by-character glitch dissolution
    // Use glitch characters: █▓▒░.:·¯`-_¸,ø¤º°`°º¤ø,¸¸,ø¤º°`
    // Gradual opacity reduction and blur effect
    // Trigger completion callback
}
```

**AsciiStatic Transition (C#):**
```csharp
// Create new file: levels/level_1_ghost/AsciiStaticTransition.cs
public sealed partial class AsciiStaticTransition : Node
{
    // Full-screen ASCII static effect
    // Random character generation
    // 2-second duration with fade out
}
```

**Enhanced Terminal Effects (C#):**
```csharp
// Update: levels/level_1_ghost/GhostUi.cs
// Add CRT curvature shader parameters
// Implement scanline animation
// Add phosphor glow pulsing
// Create vignette effect
```

### 3. Update GhostCinematicDirector.cs

**New Features to Add:**
- Typewriter timing control system
- Pixel dissolve transition management
- ASCII static transition handling
- Enhanced visual effects coordination

**New Methods:**
```csharp
private async Task ShowTypewriterText(string text, float delay = 50f)
private async Task ExecutePixelDissolve(string[] content, float duration = 2500f)
private async Task ShowAsciiStaticTransition(float duration = 2000f)
private void ApplyTerminalEffects(string effectType)
```

### 4. Update ghost_terminal.tscn

**UI Enhancements:**
- Add CRT curvature overlay
- Implement scanline animation layer
- Add phosphor glow effect container
- Create vignette effect overlay
- Add pixel dissolve transition layer
- Add ASCII static transition layer

### 5. Create Custom Shaders

**CRT Shader (update existing):**
```glsl
// Update: source/design/shaders/crt_terminal.gdshader
// Add curvature distortion
// Implement scanline animation
// Add phosphor glow pulsing
// Create vignette darkening
```

**Pixel Dissolve Shader (new):**
```glsl
// Create: source/design/shaders/pixel_dissolve.gdshader
// Character-by-character glitch effect
// Gradual opacity reduction
// Blur effect intensification
```

### 6. Update ghost_dialogic_bridge.gd

**New Signal Handlers:**
```gdscript
# Add new signal handlers for visual effects
signal typewriter_text_complete
signal pixel_dissolve_complete
signal ascii_static_complete

# Add effect coordination methods
func start_typewriter_effect(text: String, delay: float)
func start_pixel_dissolve(content: Array, duration: float)
func start_ascii_static_transition(duration: float)
```

## Implementation Priority

1. **High Priority:**
   - Update ghost_terminal.dtl with new narrative structure
   - Create basic typewriter timing system
   - Implement pixel dissolve effect

2. **Medium Priority:**
   - Add ASCII static transition
   - Update terminal visual effects
   - Enhance CRT shader effects

3. **Low Priority:**
   - Fine-tune timing and pacing
   - Add additional visual polish
   - Optimize performance

## Testing Requirements

1. **Narrative Flow Testing:**
   - Verify all dialogue appears correctly
   - Test typewriter timing effects
   - Confirm choice responses work

2. **Visual Effects Testing:**
   - Test pixel dissolve transitions
   - Verify ASCII static effect
   - Confirm CRT effects are working

3. **Integration Testing:**
   - Test complete level flow
   - Verify Dreamweaver selection works
   - Confirm level completion triggers

## Files to Modify

### Core Files:
- `levels/level_1_ghost/ghost_terminal.dtl` (complete rewrite)
- `levels/level_1_ghost/GhostCinematicDirector.cs` (major updates)
- `levels/level_1_ghost/ghost_dialogic_bridge.gd` (enhancements)
- `levels/level_1_ghost/ghost_terminal.tscn` (UI updates)

### New Files to Create:
- `levels/level_1_ghost/PixelDissolveEffect.cs`
- `levels/level_1_ghost/AsciiStaticTransition.cs`
- `source/design/shaders/pixel_dissolve.gdshader`

### Files to Update:
- `levels/level_1_ghost/GhostUi.cs` (terminal effects)
- `source/design/shaders/crt_terminal.gdshader` (enhance existing)

## Success Criteria

1. **Narrative Match:** The level follows the exact narrative flow from Videogamescenecreation
2. **Visual Style:** Terminal effects match the React prototype's aesthetic
3. **Smooth Transitions:** All pixel dissolve and ASCII static effects work correctly
4. **Dreamweaver System:** The 3-thread selection system remains functional
5. **Performance:** Effects run smoothly without impacting gameplay

## Next Steps

1. Switch to Code mode to implement these changes
2. Start with updating ghost_terminal.dtl
3. Create the new visual effect components
4. Update the C# director and bridge
5. Test the complete implementation