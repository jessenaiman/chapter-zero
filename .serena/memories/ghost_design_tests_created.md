# Ghost Terminal Design-Driven Tests Created

## Test File Created
`tests/integration/stages/stage_1_ghost/GhostTerminalDesignTests.cs`

## Test Categories (Following Design Doc States)

### STATE 1: BOOT SEQUENCE Tests
1. **BootSequenceAppliesGlitchShader** - Validates shader params match design
   - glitch_intensity = 1.0
   - scanline_speed = 3.0
   - rgb_split = 7.0

2. **BootSequenceDisplaysIterationCounter** - Tests iteration display
3. **BootSequenceScanlineSpeedMatchesDesign** - Scanline speed validation

### STATE 2: STABLE BASELINE Tests
1. **TerminalPresetProvidesCleenCrtDisplay** - Clean terminal rendering
2. **AllDesignDocPresetsAreRegistered** - All required presets exist

### STATE 3: SECRET REVEAL Tests
1. **SecretRevealAppliesCodeFragmentPreset** - Code fragment preset validation
2. **SecretRevealContains5Symbols** - 5 symbols (∞ ◊ Ω ≋ ※)
3. **SecretRevealAudioBuildupDurationIsCorrect** - 4-second buildup timing

### STATE 4: DREAMWEAVER SELECTION Tests
1. **DreamweaverThreadColorsAreDefined** - Thread colors exist
2. **ThreeDreamweaverThreadsExist** - light, shadow, ambition threads

### NARRATIVE CONTENT Tests
1. **GhostYamlLoadsSuccessfully** - YAML loads properly
2. **GhostScriptContainsThreeQuestions** - 3+ questions in script
3. **ChoicesIncludeDreamweaverScoring** - All choices have scores

### NARRATIVE UI INTEGRATION Tests
1. **NarrativeUiAppliesPresetsBeforeText** - Proper method ordering
2. **NoTextInputFieldsInDesign** - No text input (design requirement)

### SHADER TIMING Tests
1. **ShaderEffectTimingIsSeparateFromText** - Shader animation during text
2. **NarrativeBeatHasVisualPresetProperties** - Beat structure validation

## How to Run Tests
```bash
dotnet test --no-build --settings .runsettings
```

## What These Tests Verify
✓ All design doc requirements are testable
✓ Tests are RED first (will fail until implementation matches)
✓ Each test maps to specific design doc requirement
✓ Tests validate: shader params, narrative content, UI behavior, timing

## Next Steps
1. Run tests to identify which ones fail
2. Fix implementation to make tests GREEN
3. Scene will render correctly once all tests pass
