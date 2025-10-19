# Stage 1 Implementation - Quick Start Guide

**For the next agent continuing this work**

## 📋 What's Already Done

✅ **Design Complete**
- Full narrative script: `opening.json`
- Complete visual/audio spec: `opening-design.md`

✅ **Foundation Built**
- Base scene: `TerminalBase.tscn` (shader layers, audio buses, UI structure)
- Base controller: `TerminalBase.cs` (244 lines, compiles, full docs)
- 2 of 3 shaders: `crt_phosphor.gdshader` + `crt_scanlines.gdshader`

✅ **Standards Established**
- Shader documentation matches C# XML doc standard
- Naming convention: `snake_case` for shaders (GDScript convention)
- Scene naming: No numeric prefixes (folder indicates sequence)

## 🎯 Your Mission: Build 9 Sequence Scenes

### Priority Order

**1. Complete Shader System** (1-2 hours)
- Create `crt_glitch.gdshader` following existing pattern
- Load all 3 shaders into `TerminalBase.tscn` 
- Test shader stack compositing

**2. Build Core Scenes** (2-3 days)
- `BootSequence.tscn` - Opening with iteration counter
- `OpeningMonologue.tscn` - "You should not be here..."
- `Question1_Name.tscn` - First philosophical choice
- `Question2_Bridge.tscn` - Second choice
- `Question3_Darkness.tscn` - Third choice
- `SecretQuestion.tscn` - With hidden "???" hover detection
- `SecretReveal.tscn` - Ancient symbols display
- `NameQuestion.tscn` - NO TEXT INPUT (choice buttons only)
- `ThreadLockIn.tscn` - Final thread selection + transition

**3. Support Systems** (1-2 days)
- `DreamweaverScore.cs` - Point tracking singleton
- `ChoiceButton.cs` - Styled UI button with thread colors
- `SequenceCoordinator.cs` - Scene flow orchestration

## 📖 Read These First

1. **`IMPLEMENTATION_PLAN.md`** (this folder) - Full implementation guide with code examples
2. **`opening-design.md`** (this folder) - Visual states, shader parameters, audio requirements
3. **`opening.json`** (this folder) - Complete narrative script with choices/scores
4. **`Source/Shaders/README.md`** - Shader documentation standard and thread presets
5. **`.github/copilot-instructions.md`** - Project rules and conventions

## ⚡ Quick Commands

```bash
# Build
dotnet build

# Test
dotnet test --filter "FullyQualifiedName~Stage1"

# Find Hero→Light references needing update
grep -r "Hero\|HERO" --include="*.cs" Source/Scripts/Stages/Stage1/

# List current Stage1 scenes (many need cleanup)
find Source/Stages/Stage1 -name "*.tscn"
```

## 🚨 Critical Rules

1. **NO TEXT INPUT** - NameQuestion uses choice buttons, not text field
2. **Shader Documentation** - Must match `crt_phosphor.gdshader` pattern exactly
3. **Hero→Light** - JSON uses "LIGHT", C# uses `DreamweaverType.Light`
4. **Secret Choice** - "???" in SecretQuestion requires 3-second hover detection
5. **Balance Ending** - <60% in any thread triggers secret ending (not failure)
6. **Scene Naming** - Remove numeric prefixes (1-TerminalBase.tscn → TerminalBase.tscn)

## 🔧 Implementation Pattern

Every sequence scene follows this pattern:

```csharp
public partial class BootSequence : TerminalBase
{
    public override async void _Ready()
    {
        base._Ready();
        
        // Set visual state (shader parameters)
        SetShaderParameter(ShaderLayer.Glitch, "glitch_intensity", 0.8f);
        
        // Display dialogue from opening.json
        await DisplayTextAsync("ITERATION 847,294 INITIALIZING...", false);
        
        // Show choices
        ShowChoices(new[] { "Y", "N" });
    }
}
```

## 📦 Assets Needed (Not Your Job)

- `ancient_symbols.png` (1024x1024 texture) - Requires artist
- Voice samples via NobodyWho - Requires audio engineer
- Audio effects - Requires AudioSynthesizer implementation decision

## 🎮 Scene Flow

```
BootSequence
    ↓
OpeningMonologue
    ↓
Question1_Name → (record score)
    ↓
Question2_Bridge → (record score)
    ↓
Question3_Darkness → (record score)
    ↓
SecretQuestion → (if "???" chosen)
    ↓
SecretReveal (conditional)
    ↓
NameQuestion → (record score)
    ↓
ThreadLockIn → (calculate dominant thread)
    ↓
Stage 2 (Dungeon/Combat)
```

## 🧪 Test-Driven Approach

For each scene you create:

1. Write scene with basic functionality
2. Create corresponding test file in `Tests/Stages/Stage1/`
3. Run tests: `dotnet test --filter "FullyQualifiedName~[SceneName]"`
4. Fix errors
5. Build: `dotnet build --warnaserror`
6. Move to next scene

## 💡 Pro Tips

- **Start with BootSequence** - Simplest scene, tests shader loading
- **Use existing TerminalBase methods** - DisplayTextAsync(), SetShaderParameter(), etc.
- **Copy/paste shader preset values** from `opening-design.md` visual states
- **Test incrementally** - Don't build all 9 scenes then test
- **Check PROBLEMS tab** in VSCode after every change
- **Codacy will auto-analyze** - Fix issues immediately

## 📞 Get Unstuck

If you're stuck on:

- **Shader creation**: See `crt_phosphor.gdshader` as complete example
- **Scene structure**: See `TerminalBase.tscn` for node hierarchy
- **JSON loading**: See `GhostTerminalCinematicDirector.cs` for existing pattern
- **Testing**: See existing test files in `Tests/Stages/Stage1/`
- **Naming/style**: See `.github/instructions/` folder

## 🎯 Success = MVP

Minimum Viable Product complete when:

- [ ] All 9 scenes created and functional
- [ ] All 3 shaders complete and loaded
- [ ] DreamweaverScore tracks points correctly
- [ ] Full sequence plays start-to-finish
- [ ] Thread selection determines final shader state
- [ ] No compilation errors
- [ ] All tests pass
