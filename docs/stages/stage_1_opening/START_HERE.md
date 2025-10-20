# Stage 1: START HERE

## The Mission: Complete Stage 1 Opening in 5 Days

You have a working foundation. Now build 9 scenes + 3 support systems.

**Total code to write**: ~2,000 lines C#, 9 scenes (Godot editor)
**Estimated time**: 40-50 hours (5 days @ 8-10 hrs/day)

---

## Phase 1: Today (2 hours) - Complete Shader System

### Task 1.1: Create `crt_glitch.gdshader`
**File**: `Source/Shaders/crt_glitch.gdshader`

Copy the pattern from `crt_phosphor.gdshader`. Add:
- RGB channel displacement
- Random block corruption
- Time-based noise overlay
- `glitch_intensity` uniform (0-1 scale)

Use this as your guide:
```glsl
shader_type canvas_item;

// ============================================================================
// CRT Glitch Shader - Effect Overlay Layer for Stage 1 Terminal
// ============================================================================
// Purpose: RGB displacement, corruption, noise for boot/reveal sequences
// Layer Stack Position: Top (over phosphor + scanlines)
// Performance: Lightweight (~0.05ms per frame at 1080p)
// ============================================================================

uniform float glitch_intensity : hint_range(0.0, 1.0) = 0.0;
uniform float time;

// [Add functions and fragment shader following existing pattern]
```

**Deadline**: 30 minutes

### Task 1.2: Load Shaders into TerminalBase.tscn
**File**: `Source/Stages/Stage1/TerminalBase.tscn`

1. Open in Godot editor
2. Select PhosphorLayer ColorRect → Create ShaderMaterial → Load `crt_phosphor.gdshader`
3. Do same for ScanlineLayer + GlitchLayer
4. Save

**Deadline**: 30 minutes

### Task 1.3: Test Shader Stack
Run the scene in Godot. Verify:
- Phosphor curvature visible
- Scanlines animate smoothly
- No errors in console

**Deadline**: 30 minutes

**Phase 1 Complete when**: All 3 shaders loaded, no errors, scene displays correctly.

---

## Phase 2: Days 2-3 (16 hours) - Build Core Scenes

### The Pattern (Use for All 9 Scenes)

```csharp
public partial class SceneName : TerminalBase
{
    public override async void _Ready()
    {
        base._Ready();

        // 1. Set shader parameters for visual state
        SetShaderParameter(ShaderLayer.Glitch, "glitch_intensity", 0.0f);
        SetShaderParameter(ShaderLayer.Phosphor, "phosphor_tint", new Vector3(1.0f, 0.9f, 0.5f));

        // 2. Display dialogue
        await DisplayTextAsync("Your dialogue here", false);

        // 3. Show choices and record score
        ShowChoices(new[] { "Choice 1", "Choice 2", "Choice 3" });
    }
}
```

### Scene Sequence (Do in This Order)

**Scene 1: BootSequence** (2 hours)
- Heavy glitch (0.8), pulsing glow
- "ITERATION 847,294 INITIALIZING..."
- "DREAMWEAVER INTERFACE PROTOCOL ACTIVE"
- Show Y/N choice → Y continues, N exits
- **File**: `Source/Stages/Stage1/BootSequence.cs` + `.tscn`

**Scene 2: OpeningMonologue** (1.5 hours)
- No glitch (0.0), stable state
- "You should not be here..."
- Text only, space to advance
- **File**: `Source/Stages/Stage1/OpeningMonologue.cs` + `.tscn`

**Scene 3-5: Three Questions** (3 hours each)
- Question1_Name: "Do you have a name?"
- Question2_Bridge: "What did the child know?"
- Question3_Darkness: "What is the price of light?"
- Each has 3 choices, scores recorded
- **Files**: `Source/Stages/Stage1/Question[1-3]*.cs` + `.tscn`

**Scene 6: SecretQuestion** (2 hours)
- "Can you keep a secret?"
- 3 visible choices
- Hidden 4th choice "???" (appears after 3s hover in empty space)
- **File**: `Source/Stages/Stage1/SecretQuestion.cs` + `.tscn`

**Scene 7: SecretReveal** (1.5 hours)
- Glitch ramps 0→100%, ancient symbols fade in
- Display: ∞ ◊ Ω ≋ ※
- Hold 5 seconds (no rush), fade out
- **File**: `Source/Stages/Stage1/SecretReveal.cs` + `.tscn`

**Scene 8: NameQuestion** (1.5 hours)
- "If you could give me a name..."
- 3 philosophical choices (NO TEXT INPUT)
- **File**: `Source/Stages/Stage1/NameQuestion.cs` + `.tscn`

**Scene 9: ThreadLockIn** (2 hours)
- Calculate dominant thread from DreamweaverScore
- Phosphor tint crossfades to thread color (3s)
- Display thread message: "Welcome, [Thread]"
- Transition to Stage 2
- **File**: `Source/Stages/Stage1/ThreadLockIn.cs` + `.tscn`

---

## Phase 3: Day 4 (8 hours) - Support Systems

### Task 3.1: DreamweaverScore.cs (2 hours)
**File**: `Source/Scripts/Stages/Stage1/DreamweaverScore.cs`

```csharp
public partial class DreamweaverScore : Node
{
    public static DreamweaverScore Instance { get; private set; }
    public int LightScore { get; private set; }
    public int ShadowScore { get; private set; }
    public int AmbitionScore { get; private set; }

    public void AddPoints(string thread, int points) { /* impl */ }
    public string GetDominantThread() { /* returns "light", "shadow", "ambition", or "balance" */ }
    public bool IsBalanced() { /* returns true if <60% in any thread */ }
    public void Reset() { /* clear scores */ }
}
```

Copy structure from `IMPLEMENTATION_PLAN.md` Task 4.1.

### Task 3.2: ChoiceButton.cs (2 hours)
**File**: `Source/Scripts/UI/ChoiceButton.cs`

```csharp
[GlobalClass]
public partial class ChoiceButton : Button
{
    [Export] public DreamweaverType Thread { get; set; }

    public override void _Ready()
    {
        // Set hover color based on thread
        // Connect signals for click/hover
    }
}
```

Copy from `IMPLEMENTATION_PLAN.md` Task 4.2.

### Task 3.3: SequenceCoordinator.cs (2 hours)
**File**: `Source/Scripts/Stages/Stage1/SequenceCoordinator.cs`

Manages scene flow: Boot → Monologue → Q1 → Q2 → Q3 → Secret → SecretReveal (conditional) → Name → ThreadLock → Stage2

Copy from `IMPLEMENTATION_PLAN.md` Task 4.3.

### Task 3.4: Write Tests (2 hours)
Create test files in `Tests/Stages/Stage1/` for each component:
- DreamweaverScoreTests.cs
- ChoiceButtonTests.cs
- Each scene has basic test

**Phase 3 Complete when**: All tests pass, `dotnet build --warnaserror` succeeds.

---

## Phase 4: Day 5 (4 hours) - Polish & Handoff

### Task 4.1: Integration Test (2 hours)
Full sequence playthrough:
- Boot → Monologue → All questions → Secret path (optional) → Thread lock
- Verify scoring works
- Verify transitions smooth
- Verify no crashes

### Task 4.2: Code Review (1 hour)
Check against standards:
- XML doc comments on all public members
- No `var` where type isn't obvious
- Proper error handling
- Tests are not false positives

### Task 4.3: Build & Deploy (1 hour)
```bash
dotnet build --warnaserror
dotnet test
# All green? You're done.
```

---

## Execution Checklist

### Before Starting
- [ ] Read this document (5 min)
- [ ] Skim `QUICK_START.md` (5 min)
- [ ] Review `crt_phosphor.gdshader` pattern (10 min)
- [ ] Review `TerminalBase.cs` pattern (10 min)

### Daily Standup (Keep It Simple)

**Day 1 End**: Shader system complete, BootSequence working
**Day 2 End**: Questions 1-3 complete, all scenes following pattern
**Day 3 End**: Secret path + NameQuestion + ThreadLockIn complete
**Day 4 End**: Support systems complete, all tests passing
**Day 5 End**: Integration test complete, ready to ship

### When Stuck
1. Check `IMPLEMENTATION_PLAN.md` for that component (has full code)
2. Look at existing implementation (`TerminalBase.cs`, `crt_phosphor.gdshader`)
3. Copy pattern, fill in specifics
4. Test immediately

---

## Tools You Have

```bash
# Build
dotnet build --warnaserror

# Test one component
dotnet test --filter "FullyQualifiedName~BootSequence"

# Test everything Stage1
dotnet test --filter "FullyQualifiedName~Stage1"

# Run in Godot headless
/home/adam/Godot_v4.5.1-stable_mono_linux_x86_64/Godot_v4.5.1-stable_mono_linux.x86_64 \
  --path /home/adam/Dev/omega-spiral/chapter-zero --headless --test

# Find all TODO comments
grep -r "TODO" Source/Scripts/Stages/Stage1/
```

---

## Success Criteria (MVP)

When you're done:
- [ ] All 9 scenes exist and play in sequence
- [ ] All 3 shaders loaded and working
- [ ] DreamweaverScore tracks correctly
- [ ] No compilation errors
- [ ] All tests pass
- [ ] Player can complete full opening → get locked into thread → see appropriate shader color

That's it. Everything else is optional polish.

---

## You've Got This

The hard parts (design, architecture, foundation code) are done. You're just implementing the pattern 9 times.

Each scene takes ~2 hours.
Each support system takes ~2 hours.
Total: 5 days, 40-50 hours.

**Start with BootSequence today. Get it working. The rest will flow.**

Questions? Check `IMPLEMENTATION_PLAN.md` section for that task.

**Now go build something legendary.** 🚀

---

**Started**: [Date]
**Finished**: [Date]
**Status**: [Not started / In progress / Complete]
