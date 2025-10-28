# Stage 1 Ghost Terminal - Complete Architecture

## YAML Schema (Authoritative Definition)

**File:** `source/stages/stage_1_ghost/ghost.yaml`

```yaml
title: Stage Title
speaker: Primary Speaker
description: Stage description

scenes:
  # Narrative-only scene (no question)
  - lines:
      - 'Text line'
      - '[GLITCH]' or '[FADE_TO_STABLE]' as inline tags
      - ''

  # Question scene (lines + question + answers)
  - lines:
      - 'Setup narrative'
    question: 'The question text?'
    owner: omega  # Speaker (does not score)
    answers:
      - owner: light      # Dreamweaver thread (scores)
        text: 'Choice text'
      - owner: shadow
        text: 'Choice text'
      - owner: ambition
        text: 'Choice text'
```

### Key Rules:
- **Scenes array** (not moments)
- **Lines** = narrative text (can be narrative-only or setup before question)
- **Question** = prompt text (optional, signals question scene)
- **Answers** = choice options with `owner` (required for questions)
- **Inline tags**: `[GLITCH]`, `[FADE_TO_STABLE]` in lines → converted to visual presets
- **Owner field**:
  - On question: metadata (who's asking, no scoring)
  - On answer: triggers automatic scoring

## Automatic Scoring Logic

**Scoring calculation when answer is selected:**
```
if answer.owner == currentDominantDreamweaverThread:
    points = 2
else:
    points = 1

dreamweaverScores[answer.owner] += points
```

**Current dominant thread** = highest accumulated score (alphabetical tiebreaker)

## C# Architecture

### Data Model: `NarrativeScript.cs`
```csharp
public class NarrativeScript
{
    public string Title { get; set; }
    public string Speaker { get; set; }
    public List<ContentBlock> Scenes { get; set; }  // Not "Moments"!
}

public class ContentBlock : NarrativeElement
{
    public List<string>? Lines { get; set; }           // Narrative text
    public string? Question { get; set; }              // Question text
    public List<ChoiceOption>? Answers { get; set; }   // Answer options
    public string? Owner { get; set; }                 // Speaker/thread affiliation
}

public class ChoiceOption : NarrativeElement
{
    public string? Owner { get; set; }  // Light/shadow/ambition → triggers scoring
    public string? Text { get; set; }   // Display text
}
```

### UI Presentation Layer: `GhostUi.cs` (extends `NarrativeUi`)

**Key methods:**
1. `LoadGhostScript()` → loads YAML via `GhostDataLoader`
2. `PresentNextMomentAsync()` → routes scenes to correct handler
3. `PresentNarrativeSceneAsync()` → displays lines only
4. `PresentQuestionSceneAsync()` → displays lines + question + answers
5. `ConvertToNarrativeBeats()` → parses inline tags, converts to visual presets
6. `ParseLineForInlineTags()` → extracts `[GLITCH]`, `[FADE_TO_STABLE]`
7. `TrackDreamweaverScoresAutomatic()` → implements scoring logic
8. `GetCurrentDominantThread()` → determines leading thread

### Data Loader: `GhostDataLoader.cs`
```csharp
public sealed class GhostDataLoader : CinematicDirector<GhostTerminalCinematicPlan>
{
    protected override string GetDataPath() => "res://source/stages/stage_1_ghost/ghost.yaml";
    protected override GhostTerminalCinematicPlan BuildPlan(NarrativeScript script)
        => new(script);
}
```

### Stage Orchestrator: `GhostCinematicDirector.cs` (extends `StageBase`)
- Loads script via `GhostDataLoader`
- Instantiates `GhostUi`
- Waits for `SequenceComplete` signal
- Emits `StageComplete` to `GameManager`

## Flow Diagram

```
GameManager selects Stage 1
    ↓
GhostCinematicDirector._Ready()
    ↓
Load ghost.yaml → NarrativeScript with 8 scenes
    ↓
Instantiate GhostUi
    ↓
GhostUi.StartGhostSequence()
    ↓
Loop: PresentNextMomentAsync()
  ├─ Scene has Question? → PresentQuestionSceneAsync()
  │   ├─ Display Lines
  │   ├─ Present Choices (PresentChoicesAsync from base)
  │   ├─ TrackDreamweaverScoresAutomatic()
  │   └─ Advance
  └─ Scene lines only? → PresentNarrativeSceneAsync()
      ├─ Parse inline tags
      ├─ Convert to NarrativeBeats
      ├─ PlayNarrativeSequenceAsync()
      └─ Advance
    ↓
All scenes complete
    ↓
CompleteGhostSequenceAsync()
    ├─ Determine final Dreamweaver thread
    ├─ Save transcript & scores
    └─ Emit SequenceComplete signal
    ↓
GhostCinematicDirector receives signal
    ├─ Store stage results
    └─ Emit StageComplete to GameManager
    ↓
GameManager loads Stage 2
```

## Testing Strategy

**Unit Tests:** `GhostYamlPlaybackTests.cs`
- Verify YAML loads
- Verify scene structure
- Verify inline tag parsing

**Integration Tests:** (next phase)
- Full scene presentation
- Choice UI rendering
- Scoring verification

## For Stage 2+ Development

1. **Copy the pattern:**
   - Create `Stage2Script.cs` inheriting `NarrativeScript`
   - Create `Stage2Ui.cs` inheriting `NarrativeUi`
   - Create `Stage2DataLoader.cs` inheriting `CinematicDirector<Stage2Plan>`
   - Create `Stage2CinematicDirector.cs` inheriting `StageBase`

2. **Create YAML:**
   - File: `source/stages/stage_2_xxx/stage2.yaml`
   - Follow same schema: scenes array with lines/question/answers structure

3. **Override presentation (optional):**
   - Override `PresentNarrativeSceneAsync()` for stage-specific visuals
   - Override `TrackDreamweaverScoresAutomatic()` for custom scoring
   - Add inline tags as needed

4. **Register in manifest:**
   - Add to stage selection menu
   - Link to `Stage2CinematicDirector`

## Key Decisions Made

- **Scenes, not moments** - clearer semantics (a "scene" is a unit of presentation)
- **Automatic scoring** - no manual scores dict, just owner field
- **Inline tags** - creative freedom without complex nested structure
- **Simple question/answers** - not prompt/options/setup/continuation
- **Question owner never scores** - only answer owners trigger scoring
- **Current thread bonus** - if answer matches leading thread, +2 instead of +1

## Common Pitfalls to Avoid

❌ Using "moments" instead of "scenes"
❌ Using "prompt" instead of "question"
❌ Using "options" instead of "answers"
❌ Including scores dict on answers (automatic now)
❌ Having question owner score points
❌ Not parsing inline tags in ConvertToNarrativeBeats()
❌ Forgetting to emit StageComplete signal
