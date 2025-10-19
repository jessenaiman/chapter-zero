### Stage 3 – Echo Vault Implementation Plan

**Goal:** Deliver a retro Wizardry-style party assembly experience where the player rescues “echo” party members from Omega’s archive while Dreamweavers subtly guide the break from the loop.

---

#### 1. Content & Data Authoring

- Finalise `Source/Stages/Stage3/stage3.json`:
  - Stage metadata (presentation tiers, system log strings).
  - Beat sequence (echo slots, combats, finale summary).
  - Echo definitions with stats, unlock conditions, memory costs, dual Dreamweaver responses.
  - Combat configurations (enemy tables, Omega interrupts).
  - Stage 2 linkage flags and party persistence payload.
- Draft schema `Source/Data/Schemas/echo_vault_schema.json` for validation.
- Keep `content_script_draft.md` as the narrative reference; sync any changes back to JSON.

#### 2. Runtime Data Model

- Extend `NarrativeSceneData` with `EchoVaultData` (beats, echoes, combats, logs).
- Update `NarrativeSceneFactory` to map JSON into the new structures.
- Implement `EchoVaultDirector` akin to stages 1 & 2 (load, validate, expose ordered beats).

#### 3. Godot Scene Stack (Minimal First)

- **EchoVaultHub.tscn**: retro text UI showing echoes, Dreamweaver lines, hidden point tally, memory loss callouts.
  - Tier 0 art pass: monochrome wireframe (Apple II/PC‑88 vibe).
  - Tier 1 art pass: vector neon overlays (Vectrex arcade bleed) keyed to owning Dreamweaver.
  - Tier 2 art pass: Dragon Warrior / early FF homage (tile borders, faux 8x8 sprites, CRT shader).
- **EchoVaultCombat.tscn**: turn-based text battle (party stats, enemy stats, action log) with matching tier visuals.
- Simple scene coordinator to swap hub/combat, track tiers, handle victory/defeat flows.
- (Stretch) Overlay node for Omega log glitches and presentation effects.

#### 4. Gameplay Logic

- **Echo selection flow**
  - Display echoes per tier, show owner + memory cost on confirm.
  - Apply memory loss entries to `GameState`.
  - Allow reroll with log feedback and conditional spawns.
- **Combat loop**
  - Initiative order, single signature ability per echo, basic enemy AI.
  - Omega interruptions trigger scripted turn effects.
  - Victory/defeat feed into tier progression or echo scar logic.
- **Tier progression**
  - Win → tier++, update presentation theme, unlock new echoes.
  - Lose → revert to hub, mark selected echo as scarred (adjust dialogue).

#### 5. Persistence

- Extend `GameState.PlayerParty` to store `echo_id`, `iteration_origin`, `dreamweaver_affinity`, `memory_loss_entries`.
- Trigger persistence after final combat / finale beat so Stage 4 can reference the party.

- Ensure schema stored at `Source/Data/Schemas/echo_vault_schema.json`.

#### 6. Testing

- Unit tests for data mapping:
  - Ensure echo unlock conditions respect Stage 2 results.
  - Validate beat ordering (mirror → combat → mirror).
- Simulation tests (GDUnit):
  - Run through sample selection sequence, check memory loss accumulation and party data.
  - Combat resolution sanity checks.
- Regression tests on reroll (ensures system log triggers and no duplicate echoes when disallowed).

#### 7. Polish Targets (Post-VS)

- Dynamic Dreamweaver presentation shifts per tier (color glitch, text overlap).
- Memory debt UI indicator showing erased fields.
- Retro audio layer (loop hum, selection chime).
- Stage transition cinematic: Vault collapse → JRPG town boot.

#### 8. Integration Tasks

- Add Stage 3 entry to `SceneManager` (from Stage 2 finale).
- Ensure Stage 4 loader reads party data + memory loss.
- Update documentation (`README`, stage summary) once mechanics locked.
- Coordinate with narrative/audio/art for tier visuals + log strings.

---

**Phase Checklist (Lean Core → Polish):**

1. Schema + factory + director wired to `stage3.json`.
2. Tier 0 hub UI + Combat 1 working end-to-end (outnumbered test).
3. Expand to tiers 1 & 2, confirm point ledger + memory loss.
4. Persist party + memory debt; integrate Stage 4 handoff stub.
5. Layer polish (presentation upgrades, audio, Omega overlays).
6. QA pass and narrative script sync.
