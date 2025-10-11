# ADR-0004 Summary: NobodyWho Dynamic Narrative Architecture

## Quick Reference

**Status**: Ready for Review  
**Date**: 2025-10-10  
**Full Document**: [adr-0004-nobodywho-dynamic-narrative-architecture.md](adr-0004-nobodywho-dynamic-narrative-architecture.md)

## Critical Narrative Corrections

### What Changed from ADR-0003

**OLD Understanding (Incorrect)**:

- ❌ Three Dreamweavers all speaking to player simultaneously
- ❌ Omega as a helpful narrator/guide
- ❌ Dreamweavers prominent throughout entire game

**NEW Understanding (Correct)**:

- ✅ **Omega is the antagonist (BBG)** - the system/prison/trap
- ✅ **Omega only prominent in Chapter Zero** (opening chapter)
- ✅ **Three Dreamweavers are OBSERVERS** watching 3 players simultaneously
- ✅ **Dreamweavers discuss amongst themselves** (Greek chorus, hidden from player)
- ✅ **One Dreamweaver chooses the player** at end of Chapter Zero (Scene 5)
- ✅ **Player never hears Dreamweavers** during Chapter Zero - only Omega's narration

## Chapter Zero Narrative Structure

```text
Scene 1-5: The Evaluation
┌──────────────────────────────────────────────────────────┐
│  PLAYER SEES:                                            │
│  - Omega's cold, systematic narration                    │
│  - Terminal interface, dungeon, choices                  │
│  - Omega presents the game, the tests, the spiral       │
└──────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────┐
│  PLAYER DOESN'T SEE (Hidden Commentary):                 │
│  - Hero Observer: "This one shows courage..."            │
│  - Shadow Observer: "Pragmatic, not reckless..."         │
│  - Ambition Observer: "I see potential here..."          │
│                                                           │
│  They're comparing THIS player to 2 others               │
│  Debating which to choose as their guided player         │
└──────────────────────────────────────────────────────────┘

End of Scene 5: The Choice
┌──────────────────────────────────────────────────────────┐
│  ONE Dreamweaver chooses this player based on:           │
│  - Accumulated scores from player choices                │
│  - Observer commentary sentiment analysis                │
│  - Alignment with that Dreamweaver's path                │
│                                                           │
│  Result: Player learns which path chose them             │
│  (Hero, Shadow, or Ambition)                             │
└──────────────────────────────────────────────────────────┘
```

## Key Architectural Components

### 1. OmegaNarrator (Antagonist)

- **Voice**: Cold, systematic, clinical
- **Role**: The game system itself - prison/trap
- **Visibility**: Player SEES this (displayed on terminal)
- **Prominence**: Chapter Zero only
- **Example**: "The system awakens. You are Player 1. Begin."

### 2. DreamweaverObserver (3x - Hero/Shadow/Ambition)

- **Voice**: Distinct per path (noble/pragmatic/hungry)
- **Role**: Evaluating players, deciding who to choose
- **Visibility**: Player DOESN'T see this (logged commentary)
- **Prominence**: Chapter Zero evaluation period
- **Example**: "[Hero to Shadow] Did you see that courage? Impressive."

### 3. DreamweaverChoiceTracker

- **Purpose**: Score which Dreamweaver is most interested in player
- **Mechanism**: Track choices, analyze observer sentiment
- **Output**: At end of Scene 5, one Dreamweaver has highest score
- **Result**: That Dreamweaver becomes player's guide post-Chapter Zero

## Visual Theme References

### Logo Color Symbolism

| Element | Colors | Meaning |
|---------|--------|---------|
| **Monochrome Logo** | Black/White | Omega's binary system |
| **Hero Path** | Silver/White | Light, clarity, honor |
| **Shadow Path** | Deep Red/Purple | Balance, twilight, nature |
| **Ambition Path** | Orange/Gold | Fire, power, conquest |

### Narrative Tone by Voice

| Voice | Tone | Speaking To | Visibility |
|-------|------|-------------|------------|
| **Omega** | Cold, systematic | Player (directly) | ✅ Visible |
| **Hero Observer** | Noble, evaluative | Other Dreamweavers | ❌ Hidden |
| **Shadow Observer** | Pragmatic, dry | Other Dreamweavers | ❌ Hidden |
| **Ambition Observer** | Sharp, hungry | Other Dreamweavers | ❌ Hidden |

## Implementation Plan (5 Phases)

### Phase 1: ✅ **COMPLETE** - ADR Documentation

- Created comprehensive 900+ line architecture document
- Corrected narrative structure (Omega as BBG, observers hidden)
- Documented all systems: caching, RAG, tool calling, embeddings
- Ready for team review

### Phase 2: Refactor DreamweaverSystem

- Create `OmegaNarrator.cs` (antagonist voice)
- Create `DreamweaverObserver.cs` (3x instances)
- Create `DreamweaverChoiceTracker.cs` (scoring system)
- Implement `NarrativeCache.cs` and `CreativeMemoryRAG.cs`

### Phase 3: System Prompt Builder

- Load creative content (YAML/MD/JSON)
- Generate Omega's antagonist prompts
- Generate observer evaluation prompts
- Test prompt quality and consistency

### Phase 4: Dynamic Generation & Caching

- Update `NarrativeTerminal.cs` with dual-track narration
- Omega narration visible to player
- Observer commentary hidden (logged)
- Cache both for performance

### Phase 5: Scene Transitions & Choice Tracking

- Record player choices across all 5 scenes
- Update Dreamweaver interest scores
- Scene 5 finale: reveal which Dreamweaver chose player
- Transition out of Chapter Zero with chosen guide

## Success Metrics

| Metric | Target | Purpose |
|--------|--------|---------|
| **Omega Voice Consistency** | >90% thematic coherence | BBG tone maintained |
| **Observer Distinctiveness** | 3 clearly different voices | Each path recognizable |
| **Choice Tracking Accuracy** | Player alignment detected | Correct Dreamweaver chosen |
| **Performance** | <3s generation time | Real-time feel |
| **Cache Hit Rate** | >60% | Replayability performance |

## Next Steps

1. ✅ **Review this ADR** - gather team feedback
2. Create implementation branch: `feature/nobodywho-chapter-zero-narrative`
3. Begin Phase 2: Build OmegaNarrator and DreamweaverObserver classes
4. Test voice consistency with actual NobodyWho model
5. Iterate on system prompts until tone is perfect

---

**Questions for Review**:

- Is the Omega-as-antagonist voice correct?
- Should Dreamweaver commentary be totally hidden or partially revealed?
- How to visualize the "choice moment" at end of Scene 5?
- Any additional context about the 3 competing players?
