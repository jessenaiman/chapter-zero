# Stage 1 Ghost YAML vs C# Model Mapping

## Field Mapping Status

### ContentBlock Fields (All Present ✓)
| YAML Field | C# Property | Status | YAML Examples |
|-----------|-----------|--------|---|
| `type` | `Type` | ✓ | narrative, question, composite |
| `lines` | `Lines` | ✓ | Array of strings |
| `visualPreset` | `VisualPreset` | ✓ | CODE_FRAGMENT_GLITCH_OVERLAY |
| `fadeToStable` | `FadeToStable` | ✓ | true (Moment 0) |
| `timing` | `Timing` | ✓ | slow_burn (Moment 1) |
| `pause` | `Pause` | ✓ | 2.5, 3.0, 2.0 (seconds) |
| `persistent` | `Persistent` | ✓ | true (Moment 6) |
| `journalEntry` | `JournalEntry` | ✓ | OMEGA_CODE_FRAGMENT_1 |
| `setup` | `Setup` | ✓ | Array of strings |
| `prompt` | `Prompt` | ✓ | Question text |
| `context` | `Context` | ✓ | Meta-narrative frame |
| `options` | `Options` | ✓ | Array of ChoiceOption |
| `continuation` | `Continuation` | ✓ | Array of strings |

### ChoiceOption Fields (Status Check)
| YAML Field | C# Property | Status | YAML Examples |
|-----------|-----------|--------|---|
| `id` | `Id` | ✓ | light, shadow, ambition, yes, no, trade |
| `text` | `Text` | ✓ | "Yes. Names are promises..." |
| `response` | `Response` | ✓ | "Good. Because this is all..." |
| `dreamweaver` | `Dreamweaver` | ✓ | LIGHT, SHADOW, AMBITION |
| `scores` | `Scores` | ✓ | {light: 0-2, shadow: 0-2, ambition: 0-2} |
| `owner` | **NOT IN ChoiceOption** | ✗ | system, omega, light, shadow, ambition |
| `philosophical` | **NOT IN ChoiceOption** | ✗ | faith_through_doubt, self_discovery, truth_beneath_lies |

### Stage-Level Fields (At Root of YAML)
| YAML Field | C# Property | Status |
|-----------|-----------|--------|
| `title` | `NarrativeScript.Title` | ✓ |
| `speaker` | `NarrativeScript.Speaker` | ✓ |
| `description` | `NarrativeScript.Description` | ✓ |
| `moments` | `NarrativeScript.Moments` | ✓ |

## Custom/Unhandled Fields

### 1. `owner` Field (On ContentBlock and ChoiceOption)
- **Where**: Appears in ghost.yaml on moment blocks and choice options
- **Problem**: Not a property in `ContentBlock` or `ChoiceOption`
- **Used By**: GhostUi for persona/Dreamweaver tracking
- **Need**: Either add to C# model OR handle via dynamic YAML deserialization

### 2. `philosophical` Field (On ChoiceOption)
- **Where**: Appears on Moment 4 options
- **Problem**: Not a property in `ChoiceOption`
- **Used By**: Potentially for advanced scoring or metadata
- **Need**: Either add to C# model OR handle separately

## Missing Implementation Gaps

1. **Template Variable Substitution**: `{{THREAD_NAME}}`
   - Appears in Moment 9 final text
   - GhostUi needs to replace before displaying

2. **Response Field Usage**:
   - `ChoiceOption.Response` field exists but not used by `GhostUi`
   - Should display response text after player selects choice

3. **Secret Reveal Ceremony**:
   - Handled specially in `PresentSecretRevealCeremonyAsync()`
   - Symbol array hardcoded in GhostUi
   - Should ideally load from YAML instead

## Recommendation

The YAML structure is mostly correct and maps well to C# models. Small gaps:
1. Add `owner` field to both `ContentBlock` and `ChoiceOption` (nullable string)
2. Add `philosophical` field to `ChoiceOption` (nullable string, metadata)
3. Implement response text display in choice handling
4. Implement template variable substitution for {{THREAD_NAME}}
