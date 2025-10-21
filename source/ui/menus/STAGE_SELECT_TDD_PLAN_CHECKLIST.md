# Stage Select Menu - TDD Test Checklist

## Phase 1: Layout & Centering Tests

- [ ] Menu center should match viewport center horizontally
- [ ] Menu center should match viewport center vertically
- [ ] Title should be centered horizontally
- [ ] Stage 1 button should be within viewport bounds
- [ ] Stage 2 button should be within viewport bounds
- [ ] Quit button should be within viewport bounds

## Phase 2: Content Status Detection Tests

- [x] Stage 1 should report ready status
- [x] Stage 1 should report static content type
- [x] Stage 1 should display "Ready (Static)" status text
- [x] Stage 2 should report ready when LLM content file exists
- [x] Stage 2 should report LLM content type when file exists
- [x] Stage 2 should display "Ready (LLM Generated)" when content exists
- [x] Stage 2 should report not ready when content file is missing
- [x] Stage 2 should report missing content type when file is absent
- [x] Stage 2 should display "Generate Content" when content is missing

## Phase 3: Visual Status Indicators

- [ ] Status icon should display ✓ for static content
- [ ] Status icon should display ⚡ for LLM-generated content
- [ ] Status icon should display ○ for missing content
- [ ] Status color should be green for ready (static)
- [ ] Status color should be gold for LLM-generated
- [ ] Status color should be gray for missing
- [ ] Button should be enabled when stage is ready
- [ ] Button should be disabled when stage is not ready

## Phase 4: Professional Layout

- [ ] Button 1 and Button 2 should have consistent 16px spacing
- [ ] Button 2 and Button 3 should have consistent 16px spacing
- [ ] All stage buttons should have equal spacing (±2px tolerance)
- [ ] Stage 1 button should meet 48px minimum accessibility height
- [ ] Stage 2 button should meet 48px minimum accessibility height
- [ ] Quit button should meet 48px minimum accessibility height
- [ ] Title font should be larger than description font
- [ ] Title should be at least 2x description font size

## Phase 5: Keyboard/Gamepad Navigation

- [ ] Tab key should move focus to first button
- [ ] Second Tab key should move focus to second button
- [ ] Gamepad down arrow should move from Stage 1 to Stage 2
- [ ] Gamepad up arrow should move from Stage 2 to Stage 1
- [ ] Enter key should activate focused button
- [ ] Gamepad A button should activate focused button

## Phase 6: Integration Tests

- [ ] Complete menu flow: load → navigate → select Stage 1 → scene changes to Stage 1

## Implementation Notes

**Test Infrastructure:**
- Use `[TestSuite][RequireGodotRuntime]` for test class
- Use `[Before]` hook to clean up leftover test files
- Use `[After]` hook to clean up after each test
- Use `AutoFree(node)` to cleanup nodes
- Use GdUnit4 assertions: `AssertBool()`, `AssertString()`, `AssertObject()`

**File-Based Tests:**
- Stage 2 tests check for `user://stage_2_llm_generated.json`
- `[Before]` removes file if it exists from previous tests
- Create test file with `Godot.FileAccess.Open()` in each test that needs it

**Completed (✓):**
- Phase 2 all 9 tests implemented and passing
- `StageSelectMenu.cs` with `GetStageStatus()` API working
- `ContentType` enum and `StageStatus` class defined
- `.runsettings` and `tasks.json` properly configured
