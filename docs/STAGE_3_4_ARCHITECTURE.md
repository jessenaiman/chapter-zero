# Stage 3 & 4 Architecture - Reordered Sequence

## Overview

**Stage 3** and **Stage 4** have been reordered to improve narrative flow:

- **Stage 3** (formerly Stage 4): **Liminal Township** - Free-roam JRPG hub
- **Stage 4** (formerly Stage 3): **Echo Vault Party Build** - Classic top-down dungeon crawler

---

## Stage 3: Liminal Township

**Theme**: Calm before exodus. Nostalgic JRPG village, seams in the simulation, other players hinted.

**Type**: Free-roam top-down RPG hub (no TerminalBase)

**Player Journey**:
1. Arrive in village hub (spawn point)
2. Free-roam with light gating
3. Visit NPCs (inn, shop, training yard, town edge)
4. NPCs hint at loops, other players, choices that persist
5. Dreamweavers appear as translucent avatars near key POIs
6. Player commits to one Dreamweaver (Light, Shadow, or Ambition)
7. Exit to Stage 4

**Ui Architecture**:
- **Movement**: Top-down RPG controller (no TerminalBase)
- **Dialogs**: OmegaBase Ui menus for NPC conversations
- **Menus**: Equipment, status, world map all use OmegaBase components
- **No TerminalBase**: This is pure top-down gameplay, not terminal/narrative

**Key Components**:
- `Stage3Main.cs` - Hub orchestrator (already exists)
- Scene: `stage_4_main.tscn` (rename to `stage_3_main.tscn`)
- Data: `stage4.json` (rename to `stage3.json`)
- NPCs: Dialogue trees using OmegaBase Ui
- Dreamweaver avatars: Visual indicators near choice points

**Output**:
- Player selected Dreamweaver (Light, Shadow, Ambition)
- Current affinity scores passed to Stage 4

---

## Stage 4: Echo Vault Party Build

**Theme**: Quest to recruit party members following chosen Dreamweaver.

**Type**: Classic top-down dungeon crawler with JRPG party building (no TerminalBase)

**Player Journey**:
1. Enter vault guided by chosen Dreamweaver (3 decision points)
2. Pick character/class at Decision 1 (1 party member) → Fight 1 (loseable)
3. Dreamweaver offers help → Decision 2 (2 party members) → Fight 2 (loseable)
4. Dreamweaver urges urgency → Decision 3 (4 party members) → Fight 3 (winnable)
5. Archive collapses → Stage 5

**Ui Architecture**:
- **Movement**: Top-down RPG controller
- **Combat**: Turn-based, Wizardry-style battle log
- **Dialogs**: OmegaBase Ui menus for character selection and Dreamweaver guidance
- **No TerminalBase**: This is gameplay-driven, not narrative-driven
- **Presentation Tiers**: Tier 0 (B&W) → Tier 1 (colored) → Tier 2 (glitch)

**Key Components**:
- `Stage4Main.cs` - Party build orchestrator (new, adapting from current Stage 3)
- Scene: `stage_3_main.tscn` (rename to `stage_4_main.tscn`)
- Data: `stage3.json` (rename to `stage4.json`)
- Character selection: 3 beats with OmegaBase choice dialogs
- Combat: Turn-based encounter handler
- Affinity tracking: Hidden ledger (Light/Shadow/Wrath points per choice)

**Output**:
- Final party composition
- Affinity scores after 3 decisions
- Claimedw Dreamweaver (from Stage 3) confirmed

---

## Data Flow

```
Stage 2 (Echo Chamber)
    ↓ emit: (champion, affinity_scores)
    ↓
Stage 3 (Liminal Township)
    ↓ input: (champion, affinity_scores from Stage 2)
    ↓ runs: free-roam NPC interactions
    ↓ emit: (chosen_dreamweaver, affinity_scores_after_optional_nudges)
    ↓
Stage 4 (Echo Vault Party Build)
    ↓ input: (chosen_dreamweaver, affinity_scores)
    ↓ runs: 3 character selections + 3 fights
    ↓ emit: (party_composition, affinity_scores, claimed_dreamweaver)
    ↓
Stage 5 (Dreamweaver Fracture)
    ↓ input: (party, affinity_scores, claimed_dreamweaver)
```

---

## File Organization

### Stage 3 (Liminal Township)
```
source/stages/stage_3/
  ├── Stage3Main.cs          (was Stage4Main - renamed)
  ├── Stage3Main.cs.uid
  ├── stage3.json            (was stage4.json - renamed)
  ├── stage3.md              (was stage4.md - renamed)
  ├── stage_3_main.tscn      (was stage_4_main.tscn - renamed)
  ├── npc_dialogs/           (new folder for OmegaBase dialog scripts)
  │   ├── InkeeperDialog.cs
  │   ├── ShopkeeperDialog.cs
  │   └── TrainerDialog.cs
  ├── dreamweaver_avatars/   (new folder for DW visual representation)
  └── stage3_manifest.json   (beat order: spawn → free-roam → commitment)
```

### Stage 4 (Echo Vault Party Build)
```
source/stages/stage_4/
  ├── Stage4Main.cs          (new orchestrator, adapted from old Stage3NeverGoAlone)
  ├── Stage4Main.cs.uid
  ├── stage4.json            (was stage3.json - renamed, data-driven party options)
  ├── stage4.md              (was stage3.md - renamed)
  ├── stage_4_main.tscn      (was stage_3_main.tscn - renamed)
  ├── CharacterSelection.cs   (beat renderer for character choice)
  ├── CombatEncounter.cs      (beat renderer for combat)
  ├── Echo4AffinityTracker.cs (hidden ledger for Stage 4 alignment)
  └── stage4_manifest.json    (beat order: decision → fight → decision → fight → decision → fight → finale)
```

---

## Architecture Patterns

### Both Stages Use:
- **OmegaBase Ui** for all menu/dialog interactions
- **Data-driven approach** (stage3.json, stage4.json define content)
- **Manifest-based beat progression** (stage3_manifest.json, stage4_manifest.json)
- **BeatSceneBase** for individual scene orchestration
- **Top-down RPG controllers** (no TerminalBase)

### Neither Stage Uses:
- TerminalBase (TerminalUi, terminal rendering)
- Text-only narrative presentation
- Cinematic beat sequencing

---

## Implementation Notes

1. **Stage 3**: Primarily a free-roam hub with optional branching NPC interactions.
   - No forced progression until player commits to a Dreamweaver
   - Soft gating via NPC guidance and environmental barriers
   - Optional side activities grant minor affinity nudges

2. **Stage 4**: Data-driven party recruitment with escalating difficulty.
   - 3 decision beats (character/class selection)
   - 3 combat beats (loseable until party is full)
   - Presentation tiers provide visual progression
   - Affinity tracking hidden from player (revealed in Stage 5)

3. **Shared Ui**: Both stages use OmegaBase components for menus, dialogs, and status displays.
   - DialogBox for NPC/Dreamweaver speech
   - OptionButton for choice menus
   - Label/RichTextLabel for text content
   - No TerminalBase compatibility needed

---

## Next Steps

1. Rename files in source/stages/stage_3/ and source/stages/stage_4/ to reflect new order
2. Create Stage3Main.cs and Stage4Main.cs orchestrators
3. Implement stage3_manifest.json and stage4_manifest.json
4. Wire OmegaBase Ui components into both stages
5. Create data files (stage3.json, stage4.json) with new content structure
6. Build NPC dialog trees and character selection interfaces
