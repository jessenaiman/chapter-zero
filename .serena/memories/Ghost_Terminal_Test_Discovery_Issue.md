# Ghost Terminal Test Discovery Issue

## Problem
GhostTerminalDesignTests.cs and other stage_1_ghost tests (ContentBlockTests, GhostTerminalCinematicDirectorTests) are NOT appearing in test results, even though:
- Files compile cleanly
- No build errors
- Tests follow correct [TestSuite] and [TestCase] pattern
- Same test adapter (gdUnit4) used by other tests

## Test Results Evidence
- `NethackDirectorTests` (Stage 2) shows 1 test: GetPlan_ReturnsNonNull - PASSED
- `OmegaShaderControllerTests`, `OmegaTextRendererTests`, UI tests all run successfully
- BUT: NO stage_1_ghost tests appear in test-result.trx XML file

## Possible Root Causes
1. Test namespace mismatch (uses `OmegaSpiral.Tests.Stages.Stage1`)
2. [RequireGodotRuntime] attribute preventing discovery
3. Test class is static (should be public class instead)
4. Godot scene/resource loading issue that prevents initialization

## Solution Path
1. **Immediate**: Convert GhostTerminalDesignTests to a public non-static class
2. **Fallback**: Remove [RequireGodotRuntime] and run pure unit tests first
3. **Debug**: Check gdUnit4 adapter logs for discovery errors
4. **Verify**: Run single simple test to confirm namespace/class structure works

## Files Affected
- `/tests/integration/stages/stage_1_ghost/GhostTerminalDesignTests.cs` - STATIC class with [RequireGodotRuntime]
- `/tests/integration/stages/stage_1_ghost/ContentBlockTests.cs` - STATIC class, no runtime requirement
- `/tests/integration/stages/stage_1_ghost/GhostTerminalCinematicDirectorTests.cs` - regular public class with [RequireGodotRuntime]
