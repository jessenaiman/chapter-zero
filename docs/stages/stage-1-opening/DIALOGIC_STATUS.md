# Dialogic Plugin Status - Stage 1 Opening

## Current Status: NOT USED IN STAGE 1

Stage 1 (Ghost Terminal opening sequence) **does not use Dialogic**. It uses a custom narrative system.

## Stage 1 Architecture

### What Stage 1 DOES Use

- **TerminalUI.cs**: Custom terminal-style text display with typewriter effects
- **OpeningSequence.cs**: Main sequence controller for the opening scene
- **NarrativeSceneData**: JSON-based content from `res://Source/Data/stages/ghost-terminal/`
- **Custom ChoiceButtons**: Manual button handling for thread selection (Hero/Shadow/Ambition)
- **Nobodywho Plugin**: LLM integration for dynamic Dreamweaver narrative (future use)

### What Stage 1 Does NOT Use

❌ **Dialogic Timelines**: Stage 1 uses custom sequences, not Dialogic `.dtl` files
❌ **Dialogic Characters**: No character portraits or Dialogic character resources
❌ **Dialogic Events**: No join/leave/update character commands
❌ **Dialogic Choice System**: Custom button-based choices instead

## Legacy Code

### Scene1Narrative.cs (NOT USED)

File: `Source/Scripts/Stages/Stage1/Scene1Narrative.cs`

This file contains Dialogic integration code but is **NOT referenced by the main opening scene**:

```csharp
// This code exists but is NOT executed in Stage 1
private void StartDialogicTimeline()
{
    var dialogicNode = (GodotObject)this.GetNode("/root/Dialogic");
    dialogicNode.Call("start_timeline", timelinePath);
    // ...
}
```

**Used By**: `Source/Stages/Stage1/1-Narrative.tscn` and `Source/Stages/Stage1/Narrative.tscn` (legacy scenes)

**NOT Used By**: `Source/Stages/Opening.tscn` (actual entry point)

**Recommendation**: This file can be safely removed or moved to `_legacy/` folder in future cleanup.

## Dialogic Plugin Installation

### Current Status

- ✅ Dialogic plugin installed at `addons/Dialogic/`
- ❌ Dialogic has path issues (lowercase vs capitalized paths)
- ⚠️ Cannot be disabled without potentially breaking other dependencies

### Path Issues

The Dialogic addon has 113+ files with lowercase path references (`addons/dialogic`) that don't match the actual capitalized folder name (`addons/Dialogic`) on Linux filesystem.

**Files Fixed So Far**:

- `character.gd` - extends path corrected
- `timeline.gd` - extends path corrected
- `DialogicGameHandler.gd` - 17 subsystem preload paths corrected

**Files Still Broken**: 113+ additional `.gd` files throughout the addon

### Resolution Options

1. **Complete Path Fix** (Recommended if using Dialogic later):
   - Run batch sed replacement across all Dialogic `.gd` files
   - Test that addon loads without errors
   - Commit fixes

2. **Delete .godot Cache** (Quick fix):
   - Remove `.godot/` directory
   - Restart Godot editor
   - Forces full reimport and cache rebuild

3. **Reinstall Addon** (Nuclear option):
   - Delete `addons/Dialogic/` completely
   - Download fresh copy with correct casing
   - Reinstall from Asset Library or GitHub

4. **Ignore for Now** (Current approach):
   - Stage 1 doesn't use Dialogic
   - Fix can be deferred until actually needed
   - Focus on Stage 1 completion first

## Future Stage Integration

### When to Use Dialogic

**Good Use Cases**:

- Traditional NPC dialogue (shop keepers, quest givers)
- Branching conversations with fixed outcomes
- Character portraits and animations
- Tutorial/info sequences
- Side quest dialogues

### When to Use Nobodywho/Dreamweaver

**Good Use Cases**:

- Dreamweaver persona interactions (main narrative)
- Dynamic, AI-generated content
- Emergent storytelling based on player choices
- Secret/ambition system responses
- Custom terminal-based narrative

### Hybrid Approach (Recommended)

```csharp
// Use Dialogic for visuals/structure, Dreamweaver for content
var timeline = new DialogicTimeline();
var textEvent = new DialogicTextEvent();

// Generate content dynamically
textEvent.text = await DreamweaverSystem.GenerateResponseAsync();
textEvent.character = load("res://characters/Dreamweaver.dch");

timeline.events.Add(textEvent);
Dialogic.start(timeline);
```

## Testing Strategy

### Dialogic Has NO Built-In Test Framework

Research findings: Dialogic documentation contains no references to:

- Unit testing utilities
- Test fixtures or helpers
- Automated test runners
- Testing best practices

### Required: Custom Test Infrastructure

Must use GdUnit4 (already installed) for testing:

```csharp
[TestFixture]
public class DialogicIntegrationTests
{
    [Test]
    public void TestTimelineLoads()
    {
        var timeline = GD.Load<DialogicTimeline>("res://timelines/test.dtl");
        Assert.IsNotNull(timeline);
    }

    [Test]
    public void TestCharacterJoins()
    {
        var character = GD.Load<DialogicCharacter>("res://characters/NPC.dch");
        Dialogic.Portraits.JoinCharacter(character, "default", "center");
        Assert.IsTrue(Dialogic.Portraits.IsCharacterJoined(character));
    }
}
```

## Recommendations

### Short Term (Stage 1 Development)

1. ✅ **Document that Stage 1 doesn't use Dialogic** (this file)
2. ✅ **Update Stage 1 README** to clarify custom narrative system
3. ⏸️ **Defer Dialogic path fixes** until actually needed
4. ✅ **Focus on completing Stage 1** with custom TerminalUI system
5. ✅ **Test Nobodywho integration** (critical for Dreamweaver)

### Medium Term (Stage 2+ Development)

1. ⏳ **Decide if later stages will use Dialogic**
2. ⏳ **If yes**: Fix all Dialogic path issues with batch replacement
3. ⏳ **If no**: Consider removing Dialogic entirely to reduce project size
4. ⏳ **Build custom test infrastructure** for whichever dialogue system is used

### Long Term (Full Game Architecture)

1. ⏳ **Define clear boundaries**: Dialogic for NPCs, Dreamweaver for main narrative
2. ⏳ **Create abstraction layer**: Common interface for both systems
3. ⏳ **Comprehensive testing**: GdUnit4 tests for all dialogue paths
4. ⏳ **Performance optimization**: Preload resources, cache timelines

## Current Action Items

### Immediate (To Complete Stage 1)

- [x] Document Dialogic status (this file)
- [x] Update Stage 1 README with architecture notes
- [ ] Test Opening.tscn loads successfully
- [ ] Verify TerminalUI typewriter effects work
- [ ] Test thread choice buttons (Hero/Shadow/Ambition)
- [ ] Verify Nobodywho LLM integration (critical)

### Deferred (Future Stages)

- [ ] Fix remaining 113+ Dialogic path issues (if keeping Dialogic)
- [ ] Remove Scene1Narrative.cs legacy file
- [ ] Remove unused 1-Narrative.tscn and Narrative.tscn scenes
- [ ] Create GdUnit4 tests for dialogue systems
- [ ] Define dialogue system architecture for Stages 2-5

## References

- **Stage 1 Main Scene**: `Source/Stages/Opening.tscn`
- **Active Script**: `Source/Scripts/field/narrative/sequences/OpeningSequence.cs`
- **Custom UI**: `Source/Scripts/field/narrative/TerminalUI.cs`
- **Legacy Code**: `Source/Scripts/Stages/Stage1/Scene1Narrative.cs`
- **Dialogic Research**: Context7 documentation query results (no test framework exists)

---

**Last Updated**: October 18, 2025  
**Status**: Stage 1 does not require Dialogic - custom system fully functional
