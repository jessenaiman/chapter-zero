# Terminal Window System - TDD Test Checklist

_References: `gdunit4-tools.instructions.md`, `integration-testing.md`, `paramerized_tests.md`, `gdUnit4Net-API.mdx`, `scene-runner.instructions.md`_

---

## GDUnit4 C# Test Framework Standards

### Required Using Statements
```csharp
using GdUnit4;
using Godot;
using static GdUnit4.Assertions;
using OmegaSpiral.Source.UI.Terminal;  // Component under test
```

### Test Suite Structure
- **[TestSuite]** — Marks class as a GDUnit4 test suite
- **[RequireGodotRuntime]** — Required for all tests accessing Godot nodes/scenes
- **[Before]** — Setup method called before each test (replaces [Setup])
- **[After]** — Cleanup method called after each test (replaces [Teardown])

### Test Case Attributes

| Attribute | Purpose | Example |
|-----------|---------|---------|
| **[TestCase]** | Single test case | `[TestCase] public void TestName()` |
| **[TestCase(args)]** | Parameterized test | `[TestCase(400, 300)] public void TestSize(int w, int h)` |
| **[DataPoint]** | Dynamic test data | `[TestCase] [DataPoint(nameof(FrameSizes))] public void Test(int size)` |
| **[GodotExceptionMonitor]** | Monitor node exceptions | `[TestCase] [GodotExceptionMonitor]` |
| **[ThrowsException]** | Expected exception | `[TestCase] [ThrowsException(typeof(ArgumentNullException))]` |

### Assertion Patterns (Fluent API)

All assertions use the pattern: `AssertThat(actual).Is...()` or `AssertThat(actual).Is...().Is...()` (chainable)

| Assertion | Purpose |
|-----------|---------|
| `AssertThat(value).IsEqual(expected)` | Value equality |
| `AssertThat(value).IsNotEqual(expected)` | Value inequality |
| `AssertThat(value).IsNull()` / `IsNotNull()` | Null checks |
| `AssertThat(value).IsTrue()` / `IsFalse()` | Boolean checks |
| `AssertThat(value).IsGreater(threshold)` | Numeric comparisons |
| `AssertThat(value).IsLess(threshold)` | |
| `AssertThat(value).IsInstanceOf<T>()` | Type checks |
| `AssertThat(collection).Contains(item)` | Collection checks |
| `AssertThat(value).StartsWith(prefix)` | String operations |
| `AssertThrown(() => code).IsInstanceOf<ExceptionType>()` | Exception verification |

### Memory Management

**AutoFree()** — Automatically releases objects after test scope ends. Use for all Godot Object instances:
```csharp
[Before]
public void Setup()
{
    _mockNode = AutoFree(new Node2D())!;  // Marked for auto-free
    _renderer = AutoFree(new TerminalTextRenderer(_mockNode))!;
}
```

### Parameterized Test Patterns

**Static TestCase attributes** (simplest):
```csharp
[TestCase(400, 300, 0.92f)]
[TestCase(1920, 1080, 0.92f)]
[TestCase(3840, 2160, 0.92f)]
public void TestFrameSizes(int width, int height, float maxRatio)
{
    // Test with different frame sizes
}
```

**DataPoint with static property** (dynamic data):
```csharp
public static IEnumerable<object[]> FrameSizeTestData => 
[
    [new Vector2I(400, 300), "SmallDialog"],
    [new Vector2I(1920, 1080), "FullScreen"],
    [new Vector2I(3840, 2160), "MaxResolution"]
];

[TestCase]
[DataPoint(nameof(FrameSizeTestData))]
public void TestFrameAdaptation(Vector2I frameSize, string context)
{
    // Test with parameterized data
}
```

**TestCase with TestName** (custom naming):
```csharp
[TestCase(400, 300, TestName = "SmallDialog_400x300")]
[TestCase(1920, 1080, TestName = "FullScreen_1920x1080")]
public void TestContentFitting(int w, int h)
{
    // Named test cases appear separately in results
}
```

### Common Test Structure Pattern

```csharp
[TestCase]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange: Set up test data and objects
    var terminal = AutoFree(new TerminalWindow())!;
    terminal.SetFrameSize(new Vector2I(400, 300));
    
    // Act: Execute the code being tested
    terminal.SetContent(testContent);
    var contentWidth = terminal.GetContentRect().Size.X;
    
    // Assert: Verify the expected behavior
    var frameWidth = terminal.GetFrameRect().Size.X;
    var ratio = contentWidth / frameWidth;
    AssertThat(ratio).IsLess(0.92f);
}
```

### Test File Naming & Organization

- **File naming:** `{ComponentName}Tests.cs` (e.g., `TerminalWindowTests.cs`)
- **Namespace:** `OmegaSpiral.Tests.{Unit|Integration}.{Category}` (e.g., `OmegaSpiral.Tests.Unit.UI.Terminal`)
- **Class naming:** `{ComponentName}Tests` with `[TestSuite]` attribute
- **XML documentation:** Summarize each test with `/// <summary>`

### Key Test Constants & Tolerance

Define test constants at class level for consistency:
```csharp
private const float ToleranceMs = 10f;           // For timing tests
private const float CenteringTolerance = 0.01f;  // 1% tolerance for center alignment
private const float RatioTolerance = 0.02f;      // 2% tolerance for ratios
```

### Required Godot Integration

For Godot-dependent tests:
```csharp
[TestCase]
[RequireGodotRuntime]
[GodotExceptionMonitor]  // Catch exceptions during _Ready(), etc
public void TestNodeInitialization()
{
    var node = AutoFree(new MyTerminalNode())!;
    // Tests that run within Godot runtime
}
```

For logic-only tests (no Godot runtime needed):
```csharp
[TestCase]
public void TestLayoutCalculation()  // No [RequireGodotRuntime]
{
    var calculator = new LayoutCalculator();
    var result = calculator.CalculateContentBounds(400, 300);
    AssertThat(result).IsNotNull();
}
```

---

## Overview
The terminal window is a reusable UI component used throughout the game:
- **Stage Select Menu**: Full-screen menu interface
- **NPC Dialogs**: Smaller conversational windows
- **Settings Screens**: Full-screen configuration interfaces
- **Stage Content**: Various narrative presentation windows

---

## Phase 1: Terminal Window Architecture (Frame-Constrained Content)

### Core Principle: Frame-Constrained Content
The terminal frame establishes the maximum bounds. Inner content must always fit within the frame, adapting to whatever size the frame is configured for (400x300 dialog to 3840x2160 full-screen).

### Frame Boundary Enforcement
- [ ] Inner content width never exceeds terminal frame width
- [ ] Inner content height never exceeds terminal frame height
- [ ] Content margins are calculated relative to frame dimensions (not viewport)
- [ ] Frame acts as hard boundary - content clips or scrolls if necessary
- [ ] Content positioning is relative to frame edges, not viewport edges

### Frame Size as Content Constraint
- [ ] At 400x300 frame: Content adapts to fit within 400x300 bounds
- [ ] At 1920x1080 frame: Content utilizes full 1920x1080 space appropriately
- [ ] At 3840x2160 frame: Content scales to use available space without exceeding bounds
- [ ] Frame resize dynamically constrains content in real-time
- [ ] Content never renders outside frame boundaries at any frame size

### Content-to-Frame Relationship
- [ ] Content center aligns with frame center (not viewport center)
- [ ] Content margins scale with frame size (≥8% of frame dimensions)
- [ ] Frame decorations create safe zone - content stays clear of borders
- [ ] Content adapts proportionally when frame size changes
- [ ] Visual hierarchy maintains balance within frame constraints

### Frame-Constrained Centering Tests
- [ ] Content horizontal center matches frame horizontal center (ratio-based)
- [ ] Content vertical center matches frame vertical center (ratio-based)
- [ ] Centering maintains accuracy when frame resizes
- [ ] Nested elements center within their parent containers within frame
- [ ] Centering works at extreme frame sizes (400x300 to 3840x2160)

### Frame-Constrained Spacing Tests
- [ ] Content margins ≥8% of frame dimensions (not viewport)
- [ ] Internal spacing scales with frame size
- [ ] No hard-coded pixel values - all spacing relative to frame
- [ ] Spacing maintains visual balance within frame constraints
- [ ] Element relationships preserve hierarchy regardless of frame size

### Frame-Constrained Overflow Protection
- [ ] Text content fits within frame width at all frame sizes
- [ ] Interactive elements remain within frame bounds
- [ ] Content scrolls when exceeding frame height
- [ ] No content clipping outside frame boundaries
- [ ] Overflow indicators appear when content exceeds frame dimensions

## Phase 2: Content Management (Frame-Aware Adaptation)

### Frame Size Awareness
- [ ] Content layout adapts based on current frame dimensions
- [ ] Content reflows when frame size changes
- [ ] Content maintains usability within frame constraints
- [ ] Frame size determines content density and spacing
- [ ] Content scales appropriately for frame size (not viewport size)

### Dynamic Frame Response
- [ ] Content repositions instantly when frame resizes
- [ ] Content maintains proportions relative to new frame size
- [ ] No visual artifacts during frame size transitions
- [ ] Content state preserved during frame size changes
- [ ] Smooth transitions when frame size changes

## Phase 2: Content Management (Terminal Window Content)

### Dynamic Content Adaptation
- [ ] Window accepts different content types (text, buttons, images, mixed)
- [ ] Content layout adapts based on window size automatically
- [ ] Content maintains proper spacing regardless of amount
- [ ] Window handles empty content gracefully
- [ ] Window handles overflow content with appropriate UI patterns

### Content State Management
- [ ] Window preserves content state during resize operations
- [ ] Content focus/selection maintains during dynamic changes
- [ ] Window handles content updates without visual glitches
- [ ] Content transitions are smooth and professional
- [ ] Window maintains content integrity across size changes

## Phase 3: Visual Effects & Aesthetics

### Shader Effects Tests
- [ ] Phosphor effect renders correctly at all window sizes
- [ ] Scanline effect scales appropriately with window dimensions
- [ ] Glitch effects maintain visual impact across sizes
- [ ] Shader effects don't interfere with content readability
- [ ] Effects performance remains acceptable at all sizes

### Terminal Aesthetics
- [ ] Window maintains retro-futuristic visual identity at all sizes
- [ ] Border decorations scale proportionally
- [ ] Color scheme remains consistent across sizes
- [ ] Glow effects adapt to window size appropriately
- [ ] Visual noise/texture scales with window dimensions

## Phase 4: Accessibility & Usability

### Size-Aware Accessibility
- [ ] Minimum window size (400x300) maintains full accessibility
- [ ] Touch targets scale appropriately (minimum 44px)
- [ ] Text remains readable at all sizes (minimum 12px, contrast maintained)
- [ ] Focus indicators remain visible at all sizes
- [ ] Keyboard navigation works at all window sizes

### Content Accessibility
- [ ] Content hierarchy remains clear at all sizes
- [ ] Interactive elements maintain proper spacing
- [ ] Color contrast ratios preserved across sizes
- [ ] Screen reader compatibility maintained
- [ ] Motor accessibility considerations addressed

## Phase 5: Interaction & Navigation

### Input Handling
- [ ] Mouse/touch interactions work at all window sizes
- [ ] Keyboard navigation functions properly regardless of size
- [ ] Gamepad input adapts to window context
- [ ] Focus management works across size changes
- [ ] Input responsiveness maintained at all sizes

### Window Management
- [ ] Window can be resized during use without breaking interactions
- [ ] Content remains interactive during resize operations
- [ ] Focus transfers correctly when window changes size
- [ ] Modal behaviors work at different window sizes
- [ ] Window close/minimize/maximize functions work appropriately

## Phase 6: Integration & Performance

### Multi-Context Integration
- [ ] Window works as stage select menu (full screen)
- [ ] Window works as NPC dialog (small, centered)
- [ ] Window works as settings screen (full screen, complex content)
- [ ] Window works for stage content presentation (various sizes)
- [ ] Window integrates with different game contexts seamlessly

### Performance Tests
- [ ] Window renders efficiently at all supported sizes
- [ ] Shader effects maintain frame rate at maximum size
- [ ] Content loading/rendering performs well
- [ ] Memory usage remains reasonable across sizes
- [ ] Window doesn't cause performance degradation in game

## Implementation Notes

**Frame-Constrained Testing Strategy:**
- Test each requirement at multiple frame sizes: 400x300, 800x600, 1280x720, 1920x1080, 2560x1440
- Use `terminalFrame.GetRect()` as the boundary reference (not viewport)
- Verify content dimensions ≤ frame dimensions
- Test dynamic frame resizing during test execution
- Use ratios: `contentWidth / frameWidth ≤ 0.92` (allowing for margins)

**Key Distinction:**
- **Viewport**: The Godot window/screen size (can be larger than frame)
- **Frame**: The terminal window boundary (content must stay within this)
- **Content**: Adapts to fit within frame, never exceeding frame bounds

**Multi-Size Testing Strategy:**
- Test each requirement at minimum size (400x300) and maximum size (2560x1440)
- Use parameterized tests with different frame dimensions
- Verify proportional relationships hold across all frame sizes
- Test edge cases: extreme aspect ratios, minimum/maximum frame sizes

**Layout Measurement:**
- Use `Control.GetGlobalRect()` for absolute positioning
- Use `Control.Size` for dimensions
- Use `Rect2.GetCenter()` for center point calculations
- Use ratios and percentages for proportional testing (e.g., `contentWidth / frameWidth`)
- Use `Mathf.Abs(difference)` with tolerance for floating-point comparisons

**Centering Verification:**
- Test horizontal centering: `Mathf.Abs(contentCenter.X - frameCenter.X) / frameWidth < 0.01` (1% tolerance)
- Test vertical centering: `Mathf.Abs(contentCenter.Y - frameCenter.Y) / frameHeight < 0.01` (1% tolerance)
- Test ratio-based centering for nested elements within frame bounds

**Size Adaptation Testing:**
- Test frame resizing during test execution
- Verify layout recalculates correctly after frame size changes
- Test content reflow and repositioning within new frame bounds
- Verify no visual artifacts during frame resize operations

**Overflow Protection:**
- Test text width ratios: `textWidth / frameWidth ≤ 0.9` (90% maximum)
- Test content height ratios: `contentHeight / frameHeight ≤ 0.9` (90% maximum)
- Test button text ratios: `textWidth / buttonWidth ≤ 0.85` (85% maximum)
- Use frame dimensions as the constraint boundary

**Performance Considerations:**
- Test rendering performance at maximum window size
- Verify shader effects don't impact frame rate
- Test memory usage across different window sizes
- Ensure smooth resize operations without stuttering

**Context-Specific Testing:**
- Stage Select Menu: Full-screen, menu-focused content
- NPC Dialogs: Small windows, conversational content
- Settings Screens: Full-screen, complex configuration content
- Stage Content: Various sizes, narrative presentation

**Accessibility Testing:**
- Verify minimum touch targets (44px) at all sizes
- Test text readability (minimum 12px) across sizes
- Check color contrast ratios remain compliant
- Verify keyboard/gamepad navigation works at all sizes

