# Stage 3 · Echo Vault

The retro dungeon crawler interlude where the player recruits echo party members, experiments with multi-tier presentation, and doubles down on Dreamweaver alignment.

---

## Creative Snapshot

- **Fantasy**: A Wizardry-style archive where “echoes” of legendary heroes are stored. The player frees echoes to assemble their Act I party while Dreamweavers compete for influence.
- **Structure**: Tiered vault runs—hub selection → turn-based encounter → debrief—each escalating presentation from wireframe text to full faux-8-bit.
- **Narrative Hook**: Echoes remember fragments of prior loops, dropping clues about other players trapped in the system (sets up Stage 5 revelation).

---

## Content & Data

- Data file: `res://source/stages/stage_3/stage3.json`
- Schema: `res://source/data/schemas/echo_vault_schema.json`
- Loader: `EchoVaultDirector` (wraps `NarrativeSceneFactory` mapping)
- Scenes: `EchoVaultHub.tscn`, `EchoVaultCombat.tscn`, `EchoVaultFinale.tscn`
- Tests: `Tests/Stages/Stage3/EchoVaultDirectorTests.cs`

---

## Design Notes

- **Presentation Tiers**: Three art layers (wireframe → vector neon → faux 8x8) reflect the system “remembering” nostalgia eras as the player digs deeper.
- **Echo Personalities**: Each rescued echo grants combat utility *and* narrative fragments hinting at the existence of parallel players.
- **Dreamweaver Banter**: Light, Mischief, and Wrath debate which echoes to release, foreshadowing the fracture in Stage 5.
- **Memory Cost Mechanic**: Rescuing echoes should visibly erode something (e.g., deduct lore entries, scramble Ui) to reinforce stakes.

---

## Implementation Checklist

1. Finalise JSON content: echo roster, tier beats, combat encounters, Omega log interrupts.
2. Extend `NarrativeSceneFactory` mapping for vault structures; add GDUnit coverage.
3. Build hub/combat scenes with tier-based shader toggles and Dreamweaver VO hooks.
4. Persist rescued echoes and memory costs into `GameState` for Stage 4 hand-off.

---

## QA / Validation

- Automated: `dotnet test --filter "FullyQualifiedName~EchoVaultDirectorTests"`
- Manual: Verify tier presentation swaps, echo recruitment flow, and Dreamweaver alignment deltas; confirm lore hints about additional players trigger per design.
