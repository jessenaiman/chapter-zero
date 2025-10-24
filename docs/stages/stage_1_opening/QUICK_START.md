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

        // Display dialogue from stage1.json
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



## 💡 Pro Tips

- **Start with BootSequence** - Simplest scene, tests shader loading
- **Use existing TerminalBase methods** - DisplayTextAsync(), SetShaderParameter(), etc.
- **Copy/paste shader preset values** from `opening-design.md` visual states
- **Test incrementally** - Don't build all 9 scenes then test
- **Check PROBLEMS tab** in VSCode after every change

## 🎯 Success = MVP

Minimum Viable Product complete when:

- [ ] All 9 scenes created and functional
- [ ] All 3 shaders complete and loaded
- [ ] DreamweaverScore tracks points correctly
- [ ] Full sequence plays start-to-finish
- [ ] Thread selection determines final shader state
- [ ] No compilation errors
- [ ] All tests pass
