# Stage Select Menu - TDD Redesign Plan

## Overview
Professional redesign of the stage select menu with perfect centering, status indicators for LLM-generated content, and polished visual design that matches the game's aesthetic.

## Design Goals

### Visual Excellence
- **Perfect centering** - All elements properly centered regardless of viewport size
- **Professional layout** - Clean hierarchy with proper spacing and alignment
- **Status indicators** - Clear visual feedback for content readiness (static/LLM-generated)
- **Consistent theming** - Match terminal aesthetic with CRT effects
- **Responsive design** - Adapt gracefully to different resolutions

### Functional Requirements
- **Content status display** - Show which stages have LLM-generated content loaded
- **Static baseline** - Stage 1 runs from static JSON (always ready)
- **LLM detection** - Stage 2+ check for generated content files
- **Accessibility** - Keyboard/gamepad navigation with focus indicators
- **Debug info** - Developer-friendly status messages


### Step 1: Create Test Infrastructure

### Step 2: Implement Content Status System

### Step 3: Redesign UI Layout
- [ ] Create new button template with status indicator
- [ ] Add status label/icon to each button
- [ ] Implement proper anchor/margin system for centering
- [ ] Add visual hierarchy with font sizes and spacing

### Step 4: Implement Status Display
- [ ] Update button appearance based on content status
- [ ] Add color coding (green/gold/gray)
- [ ] Add status icons (✓/⚡/○)
- [ ] Implement tooltip with detailed info

### Step 5: Polish & Accessibility
- [ ] Configure focus navigation order
- [ ] Add focus indicators (highlight on hover/focus)
- [ ] Test keyboard navigation
- [ ] Test gamepad navigation
- [ ] Add sound effects for navigation

### Step 6: Integration
- [ ] Connect to LLM content generation system
- [ ] Add "Generate Content" action for missing stages
- [ ] Implement real-time status updates
- [ ] Add debug logging for status changes

## Visual Design Specification

### Layout Structure
```
┌────────────────────────────────────────────┐
│                                            │
│              Ωmega Spiral                  │  ← 64px, centered
│        Stage Select - Dev Menu             │  ← 20px, centered
│                                            │
│  ┌──────────────────────────────────────┐ │
│  │ ✓ Stage 1: Ghost Terminal   [Ready] │ │  ← 48px height, 80% width
│  ├──────────────────────────────────────┤ │
│  │ ⚡ Stage 2: Echo Hub      [LLM Gen] │ │  ← 16px spacing
│  ├──────────────────────────────────────┤ │
│  │ ○ Stage 3: Echo Vault    [Generate] │ │
│  ├──────────────────────────────────────┤ │
│  │ ○ Stage 4: Town          [Generate] │ │
│  ├──────────────────────────────────────┤ │
│  │ ○ Stage 5: Fractured     [Generate] │ │
│  ├──────────────────────────────────────┤ │
│  │             Quit                      │ │
│  └──────────────────────────────────────┘ │
│                                            │
└────────────────────────────────────────────┘
```

### Color Palette
- **Background**: `#0a0a14` (deep terminal black)
- **Border**: `#40d980` (terminal green, 65% alpha)
- **Text Primary**: `#33ff66` (bright terminal green)
- **Text Secondary**: `#80ff99` (lighter green)
- **Status Ready**: `#33ff66` (green)
- **Status LLM**: `#ffc033` (gold/amber)
- **Status Missing**: `#808080` (gray)
- **Focus Glow**: `#33ff66` with 80% alpha blur

### Typography
- **Title**: Kenney Pixel, 64px, centered
- **Subtitle**: Kenney Pixel, 20px, centered
- **Buttons**: Kenney Pixel, 24px, left-aligned
- **Status**: Kenney Pixel, 16px, right-aligned

### Spacing System
- **Outer margin**: 48px from viewport edges
- **Title spacing**: 32px below title
- **Button spacing**: 16px between buttons
- **Status padding**: 12px internal padding
- **Icon spacing**: 8px between icon and text

## Success Criteria

### All Tests Pass
- ✓ Layout centering tests pass at all resolutions
- ✓ Content status detection works correctly
- ✓ Visual indicators display properly
- ✓ Navigation tests pass for keyboard/gamepad
- ✓ Integration tests complete successfully

### Visual Quality
- ✓ Menu perfectly centered in viewport
- ✓ No elements extend outside viewport bounds
- ✓ Professional visual hierarchy
- ✓ Consistent spacing and alignment
- ✓ Clear status indicators

### Functional Requirements
- ✓ Stage 1 always shows "Ready (Static)"
- ✓ Stage 2 shows LLM status when content exists
- ✓ Missing content clearly indicated
- ✓ Buttons disabled when content not ready
- ✓ Keyboard/gamepad navigation works flawlessly

## Test Summary

### Total Tests: 39

#### Phase 1: Layout & Centering (8 tests)
- Menu centering validation at multiple resolutions (5 tests)
- Button viewport bounds checking (3 tests)

#### Phase 2: Content Status Detection (9 tests)
- Stage 1 static content verification (3 tests)
- Stage 2 LLM content detection (3 tests)
- Stage 2 missing content handling (3 tests)

#### Phase 3: Visual Status Indicators (3 tests from TestCase)
- Status icon display (3 TestCase variants)
- Status color coding (3 TestCase variants)
- Button enabled state (2 TestCase variants)

#### Phase 4: Professional Layout (8 tests)
- Button spacing consistency (3 tests)
- Accessibility minimum sizes (3 tests)
- Typography hierarchy (2 tests)

#### Phase 5: Navigation (6 tests)
- Tab navigation (2 tests)
- Gamepad navigation (4 tests)

#### Phase 6: Integration (1 test)
- End-to-end menu flow (1 async test)

### Test Characteristics

**SRP Compliance**: Each test verifies ONE specific behavior
**Readability**: Clear naming convention: `Component_Behavior_Condition`
**Granularity**: Small, focused assertions
**Maintainability**: Easy to identify which feature broke when test fails


