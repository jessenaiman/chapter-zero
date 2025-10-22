# Two-Layer Layout Tests - Implementation Summary

## Test Cases Added (10 Tests)

### 1. **OuterFrame_FillsEntireViewport**
- Verifies `TerminalFrame` Panel fills the entire viewport (position 0,0 and matches viewport size)
- Ensures outer decorative layer properly covers all visible area

### 2. **Background_FillsEntireViewport**
- Verifies background `ColorRect` (with shader) fills the entire viewport
- Tests the bottommost visual layer

### 3. **InnerContent_IsCenteredInViewport**
- Verifies `Center` CenterContainer is properly centered within the viewport
- Uses 2px tolerance for floating-point center calculations
- Tests the functional content centering

### 4. **MenuVBox_IsNestedInCenterContainer**
- Verifies `MenuVBox` is correctly parented inside `CenterContainer`
- Tests the nesting hierarchy

### 5. **InnerContent_IsSmallerThanOuterFrame**
- Verifies `MenuVBox` is smaller than the viewport (proper spacing)
- Requires at least 100px margin on each dimension to ensure visible frame
- Tests visual hierarchy and frame visibility

### 6. **StagesPanel_IsNestedInMenuVBox**
- Verifies `StagesPanel` VBoxContainer is correctly parented inside `MenuVBox`
- Tests deeper nesting hierarchy

### 7. **StageButtons_AreWithinInnerContentBounds**
- Verifies all stage buttons are completely within the inner content bounds
- Iterates through all `StagesPanel` children
- Uses difference-based assertions to work with GdUnit4's `IsGreaterEqual` method

### 8. **ShaderLayers_FillViewport**
- Verifies `ShaderLayers` Control fills the entire viewport
- Tests the shader effect layer (Phosphor, Scanline, Glitch)

### 9. **CenterContainer_UsesProperLayoutPreset**
- Verifies `CenterContainer` uses anchors_preset=8 (center)
- Checks all 4 anchor points are 0.5f
- Tests proper Godot layout configuration

### 10. **RootControl_FillsViewport**
- Verifies root `StageSelectMenu` Control uses anchors_preset=8 with grow both
- Checks anchor configuration and grow direction
- Tests root-level layout settings

## Technical Details

### Scene Structure Tested
```
StageSelectMenu (Control, root)
├── ColorRect (Background with shader)
├── TerminalFrame (Panel, outer frame)
├── ShaderLayers (Control)
│   ├── PhosphorLayer (ColorRect)
│   ├── ScanlineLayer (ColorRect)
│   └── GlitchLayer (ColorRect)
└── Center (CenterContainer, inner content)
    └── MenuVBox (VBoxContainer)
        ├── TitleLabel
        ├── DescriptionLabel
        └── StagesPanel (VBoxContainer)
            ├── Stage1Button
            ├── Stage2Button
            ├── Stage3Button
            ├── Stage4Button
            ├── Stage5Button
            └── QuitButton
```

### GdUnit4 Assertion Patterns Used
- `IsEqual(expected)` - Exact value matching
- `IsLess(value)` - Less than comparison
- `IsGreater(value)` - Greater than comparison
- `IsGreaterEqual(value)` - Greater than or equal (used with difference pattern)
- `IsNotNull()` - Null checking

### Key Learning: GdUnit4 Assertion API
- GdUnit4 uses `IsGreaterEqual` not `IsGreaterOrEqual`
- For >= comparisons, use difference pattern: `AssertThat(a - b).IsGreaterEqual(-0.1f)`
- For <= comparisons, use difference pattern: `AssertThat(b - a).IsGreaterEqual(-0.1f)`

## Integration with TDD Checklist

These tests partially address **Phase 4: Professional Layout** from the Stage Select Menu TDD checklist:
- ✅ Outer frame fills viewport
- ✅ Inner content centered within frame
- ✅ Proper visual hierarchy (outer > inner sizing)
- ✅ All buttons within content bounds
- ✅ Layout preset verification

Still needed for Phase 4 completion:
- Button spacing consistency (16px)
- Accessibility height requirements (48px minimum)
- Font size relationships (title 2x description)

## Running the Tests

These tests require the Godot runtime and must be run through GdUnit4:
```bash
# From Godot editor
# Open test file and click "Run Tests" in GdUnit4 panel

# Or from command line (if configured)
godot --path /home/adam/Dev/omega-spiral/chapter-zero --headless --run-tests
```
