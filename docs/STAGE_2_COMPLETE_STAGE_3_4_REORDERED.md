# Omega Spiral - Architecture Reordering Summary

## Session Summary: Stage 2 Complete, Stages 3 & 4 Reordered

**Date**: October 22, 2025
**Status**: Stage 2 implementation complete. Stage 3 & 4 order swapped for better narrative flow.

---

## Stage 2: Echo Chamber (COMPLETE ✅)

**Status**: Fully implemented, tested, and verified clean.

**Components**:
- ✅ Stage2NarrativeData.cs (12 data classes)
- ✅ BeatInterludeSequence.cs + 3 Nethack variants
- ✅ EchoAffinityTracker.cs (affinity scoring system)
- ✅ stage_2_manifest.json (7-beat sequence)
- ✅ Stage2MenuIntegrationTests.cs (8 test cases)
- ✅ EchoHub.cs (refactored to data-driven)

**Architecture Pattern**:
- Data-driven: All content in stage_2.json
- Manifest-based: Beats orchestrated via stage_2_manifest.json
- Infrastructure shared: BeatSceneBase, NarrativeDataLoader, StageManifestLoader
- Clean separation: No knowledge of downstream stages

**Output to Stage 3**:
- Winning Dreamweaver (light, shadow, or ambition)
- Affinity scores (hidden ledger)

---

## Critical Reordering Decision

### Problem with Original Order (3→4)
1. Echo Vault (epic dungeon raid) → Liminal Township (calm village) feels anticlimactic
2. Party already recruited → going to town to "commit" feels disjointed
3. Commitment after recruitment = narrative backtrack
4. Town visit is reflection/wind-down, not progression

### Solution: Swap to (4→3)
1. **Stage 3 NOW**: Liminal Township (grounding, introduction)
2. **Stage 4 NOW**: Echo Vault Party Build (quest, progression)

**Benefits**:
- ✅ Town provides context BEFORE party building
- ✅ Dreamweaver commitment is FIRST major decision
- ✅ Party building becomes "follow your chosen path"
- ✅ Escalating stakes: grounding → commitment → action
- ✅ Thematic unity: "I chose this path, now I build a team for it"

---

## New Stage Order & Architecture

### Stage 3: Liminal Township (Free-Roam Hub)
- **Type**: Top-down JRPG (no TerminalBase)
- **Ui**: OmegaBase dialogs for NPC conversations
- **Gameplay**: Free-roam with light gating
- **Key Events**: NPCs hint at loops/other players, Dreamweaver avatars at POIs
- **Output**: Chosen Dreamweaver + optional affinity nudges

### Stage 4: Echo Vault Party Build (Dungeon Crawler)
- **Type**: Top-down dungeon crawler (no TerminalBase)
- **Ui**: OmegaBase menus for character selection & combat
- **Gameplay**: 3 decision beats (pick character) + 3 combat beats (loseable until full party)
- **Key Events**: Presentation tiers (B&W → color → glitch), Dreamweaver guidance
- **Output**: Final party + affinity scores + claimed Dreamweaver

---

## Data Flow Architecture

```
Stage 1 (Ghost Terminal)
    ↓ emit: dreamweaver_champion
    ↓
Stage 2 (Echo Chamber)
    ↓ input: dreamweaver_champion
    ↓ emit: (champion, affinity_scores_light_shadow_ambition)
    ↓
Stage 3 (Liminal Township) ← REORDERED (was Stage 4)
    ↓ input: (champion, affinity_scores)
    ↓ runs: free-roam NPC interactions
    ↓ emit: (chosen_dreamweaver, affinity_scores_after_optional_nudges)
    ↓
Stage 4 (Echo Vault Party Build) ← REORDERED (was Stage 3)
    ↓ input: (chosen_dreamweaver, affinity_scores)
    ↓ runs: 3 character selections + 3 fights
    ↓ emit: (party_composition, affinity_scores, claimed_dreamweaver)
    ↓
Stage 5 (Dreamweaver Fracture)
    ↓ input: (party, affinity_scores, claimed_dreamweaver)
```

---

## No TerminalBase: Why?

Both Stage 3 and Stage 4 are **gameplay-driven**, not **narrative-driven**:

- **Stage 1**: Narrative-driven (text-based, choices shape story) → ✅ TerminalBase
- **Stage 2**: Hybrid (exploration + dialogue + affinity) → ✅ Manifest/beat-based
- **Stage 3**: Gameplay-driven (free-roam, NPC interactions) → ❌ No TerminalBase, OmegaBase Ui
- **Stage 4**: Gameplay-driven (combat, party building) → ❌ No TerminalBase, OmegaBase Ui
- **Stage 5**: Likely narrative-driven (final fracture) → Will determine when designing

**OmegaBase Ui is sufficient** for both Stage 3 and Stage 4 because:
- Both use OmegaBase DialogBox for character speech
- Both use OmegaBase OptionButton for player choices
- Neither needs TerminalBase's terminal rendering or cinematic sequencing
- Both are primarily gameplay with Ui overlays

---

## Build Status

✅ **Build**: Clean (no Stage 2-specific errors)
✅ **Tests**: All passing
✅ **No Warnings**: Stage 2 code verified error-free

---

## Next Steps (In Priority Order)

### Phase 1: Architecture Foundation
1. Create stage3_manifest.json and stage4_manifest.json
2. Create stage3.json and stage4.json (data files)
3. Create Stage3Main.cs and Stage4Main.cs orchestrators

### Phase 2: Stage 3 (Liminal Township)
1. Implement free-roam top-down controller
2. Wire OmegaBase Ui dialogs for NPCs
3. Create Dreamweaver avatar system
4. Build NPC dialog trees
5. Implement optional affinity nudges

### Phase 3: Stage 4 (Echo Vault Party Build)
1. Create character selection Ui (OmegaBase)
2. Implement combat encounter system
3. Wire presentation tier progression
4. Create Echo4AffinityTracker (hidden ledger)
5. Implement decision/combat beat sequences

### Phase 4: Integration
1. Test data flow between Stage 2 → 3 → 4
2. Verify Dreamweaver commitment carries through
3. Validate affinity scores persist correctly
4. Integration tests for new stages

---

## Files Already Exist (To Be Reordered)

**Current Stage 3 (becoming Stage 4)**:
- source/stages/stage_3/stage3.json → source/stages/stage_4/stage4.json
- source/stages/stage_3/stage3.md → source/stages/stage_4/stage4.md
- source/stages/stage_3/NeverGoAloneController.cs → source/stages/stage_4/Stage4Main.cs (refactor)
- source/stages/stage_3/MirrorSelectionController.cs → source/stages/stage_4/ (refactor)
- source/stages/stage_3/NeverGoAloneCombatController.cs → source/stages/stage_4/ (refactor)

**Current Stage 4 (becoming Stage 3)**:
- source/stages/stage_4/stage4.json → source/stages/stage_3/stage3.json
- source/stages/stage_4/stage4.md → source/stages/stage_3/stage3.md
- source/stages/stage_4/Stage4Main.cs → source/stages/stage_3/Stage3Main.cs (refactor)
- source/stages/stage_4/stage_4_main.tscn → source/stages/stage_3/stage_3_main.tscn

---

## Naming Convention Clarification

To avoid confusion during implementation:

**Stage Numbers** = Gameplay progression order (what player sees)
- Stage 1 = Ghost Terminal
- Stage 2 = Echo Chamber
- **Stage 3 = Liminal Township** (free-roam hub)
- **Stage 4 = Echo Vault** (party building)
- Stage 5 = Dreamweaver Fracture

**File Paths** = Match stage numbers
- `source/stages/stage_3/` = Liminal Township code
- `source/stages/stage_4/` = Echo Vault Party Build code
- `stage3.json` = Liminal Township data
- `stage4.json` = Echo Vault Party Build data

**Classes** = Describe function, include stage number
- `Stage3Main.cs` = Liminal Township orchestrator
- `Stage4Main.cs` = Echo Vault Party Build orchestrator
- `Echo4AffinityTracker.cs` = Hidden ledger for Stage 4 decisions

---

## Summary

**Session Achievements**:
✅ Stage 2 complete and verified (9/9 tasks done)
✅ Stage 3 & 4 reordered for better narrative flow
✅ Architecture documentation created
✅ OmegaBase Ui integration strategy defined
✅ Data flow pipeline established
✅ No TerminalBase needed for gameplay-driven stages

**Status**: Ready to implement Stage 3 & 4 with clear specifications and no scope creep.
