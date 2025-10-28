# Ghost Terminal YAML - Inconsistency Review

## Critical Inconsistencies Found

### 1. **Redundant ID vs Owner (Lines 49-56, 77-89, 160-173)**
**Status:** INCONSISTENT - Mixed patterns

- **Moment 3 (First Choice):** Uses `owner` field ONLY ✓ CORRECT
  ```yaml
  options:
  - owner: light
    text: Yes. Names are promises...
  ```

- **Moment 4 (Bridge Choice):** Uses `id` field ONLY ✗ WRONG
  ```yaml
  options:
  - id: light
    theme: faith through doubt
    scores: {...}
  ```
  Should use `owner: light` instead

- **Moment 5 (Secret Question):** Uses `id` AND `dreamweaver` ✗ DUPLICATE
  ```yaml
  options:
  - dreamweaver: LIGHT
    id: light
  ```
  Should consolidate to `owner: light` only

- **Moment 6 (Secret Response):** Uses only `id` ✗ INCONSISTENT
  ```yaml
  options:
  - id: 'yes'
    response: Good. Because...
  ```
  These don't map to light/shadow/ambition. Need `owner` semantics clarified.

### 2. **Field Naming: "theme" vs "philosophical" (Line 78)**
**Status:** INCONSISTENT - Two different names

- **Moment 4:** Uses `theme: faith through doubt`
- **Expected:** Should be `philosophical` (as was in design)
- **Problem:** Field name is non-standard and doesn't match C# model

### 3. **Field Case Inconsistency: "dreamweaver" Field (Line 160)**
**Status:** INCONSISTENT - Wrong casing

- **Moment 8:** Uses `dreamweaver: LIGHT` (uppercase LIGHT)
- **Expected:** Should be lowercase `owner: light`
- **Pattern:** All other moments use lowercase (light, shadow, ambition)

### 4. **Missing Scores on First Choice (Lines 49-56)**
**Status:** INCONSISTENT - Only moment 3 lacks scores

- **Moment 3 options:** NO scores at all ✗
  ```yaml
  - owner: light
    text: Yes. Names are promises...
    # NO scores field!
  ```
- **All other choice moments:** Have scores ✓
- **Problem:** Breaks consistent scoring pattern. Should add `scores: {light: X, shadow: X, ambition: X}`

### 5. **Response Field Inconsistency (Lines 116-129)**
**Status:** INCONSISTENT - Only some moments have it

- **Moment 5 (Can you keep a secret?):** Has `response` field on each option
  ```yaml
  - id: 'yes'
    response: Good. Because this is all...
  ```
- **Other choice moments:** NO response field
- **Problem:** Breaks consistency. Either all moments should have responses, or none should.

### 6. **Context vs Setup Semantics (Line 48)**
**Status:** POTENTIALLY INCONSISTENT - Unclear usage

- **Moment 3:** Has `context: "(Not YOUR name. The question is: do names matter?)"`
- **All other composite/question moments:** Use `setup` array instead
- **Problem:** Is `context` for brief meta-narrative and `setup` for detailed narrative? Unclear.

### 7. **Owner Field on Narrative Blocks (Lines 16, 33, 42)**
**Status:** CORRECT but not used everywhere

- **Moments 0-2:** All narrative blocks have `owner` field
- **Moment 6 (Code Fragment):** NO owner field ✗
- **Moment 9 (Final):** NO owner field ✗
- **Pattern:** Should all have owner for consistency

### 8. **Timing Field Only on One Moment (Line 34)**
**Status:** INCONSISTENT - Only Moment 1 has it

- **Moment 1:** `timing: slow_burn`
- **All others:** NO timing field
- **Question:** Should other moments have timing hints too?

## Recommended Fixes

### Priority 1: Standardize Owner/ID Pattern
```yaml
# REMOVE dreamweaver, id (when redundant)
# USE owner: light|shadow|ambition|omega|system
# Format for ALL choice options:

- owner: light
  scores:
    light: 2
    shadow: 0
    ambition: 1
  text: "Choice text here"
```

### Priority 2: Add Missing Scores
**Moment 3 options:** Add scores field to all three options

### Priority 3: Standardize Field Names
- Replace `theme` with `philosophical` (or remove if not used)
- Replace `dreamweaver: LIGHT` with `owner: light`
- Consolidate case: all lowercase (light, shadow, ambition, omega, system)

### Priority 4: Clarify Context vs Setup
- `context`: Optional brief meta-narrative (one-liner framing)
- `setup`: Required narrative array before choice (detailed scene-setting)

### Priority 5: Response Field Consistency
- Either: Add to ALL choice moments
- Or: Remove from Moment 5 (keep only in narrative flow)

### Priority 6: Owner on All Narrative Blocks
- Add `owner: omega` to Moments 6 (Code Fragment) and 9 (Final)

## Valid Owner Values (4 Total)
1. `light` - Light Dreamweaver persona
2. `shadow` - Shadow Dreamweaver persona
3. `ambition` - Ambition Dreamweaver persona
4. `omega` - Omega/System persona
5. `system` - System messages (Moment 0)

(Note: system and omega may need consolidation too)
