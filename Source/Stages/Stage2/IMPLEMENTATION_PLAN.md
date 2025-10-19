### Stage 2 – Echo Chamber Implementation Plan

**Vision:** Transform the AI Studio prototype into a fully data-driven Godot stage that mirrors Stage 1’s narrative pipeline while preserving the NetHack homage and Dreamweaver rivalry.

---

#### 1. Content Authoring
- Create `Source/Stages/Stage2/stage2.json` capturing:
  - Metadata (iteration, ambience).
  - Dreamweaver definitions (name, styling).
  - Interlude questions (prompt, three answers, alignment tags).
  - Chamber definitions (owner, layout template id, object scripts, decoy counts).
  - Banter lines (approval + dissent combos).
  - Finale dialogue.
- Maintain schema in `Source/Data/Schemas/echo_chamber_schema.json` (draft).

#### 2. Runtime Data Model
- Extend `NarrativeSceneData` with `EchoChamberData`.
- Update `NarrativeSceneFactory` to map JSON into strongly typed records.
- Introduce `EchoChamberDirector` to transform data into ordered beats (interludes + chambers + finale).

#### 3. Godot Scene Stack
- Hub scene: handles boot dialogue and orchestrates interludes + chambers (`EchoHub.tscn`).
- Interlude scene: renders three-choice prompts and plays banter.
- Dungeon scene: renders ASCII map/tiles, processes movement, reveals objects, plays auto-fight overlays.
- Shared UI widgets: typewriter logs, fight overlay, chamber complete banner.

#### 4. Scoring & Game State
- Use `GameState.RecordChoice` for interlude answers + chamber interactions.
- Keep scoring invisible; rely on banter/environment to hint at alignment.
- Final Dreamweaver claim updates `GameState.SelectedDreamweaver` (for Stage 3).

#### 5. Testing
- GDUnit logic tests validating:
  - JSON schema conformity.
  - Director outputs correct beat sequence and alignment mappings.
  - Scoring math for representative choice permutations.
- Scene smoke test to load hub + first chamber without runtime errors.

#### 6. Polish Targets
- Distinct ambience per chamber (audio bussing hooks).
- Shader glitches during object reveals.
- Banter variations reflecting cumulative choices.
- Accessibility: highlight interactables, allow text speed adjustments.

#### 7. Integration & Handoff
- Wire Stage 1 finale to Stage 2 hub via `SceneManager`.
- Provide `README` update summarizing new stage, data format, and testing commands.
- Document outstanding polish tasks + art/audio dependencies in `IMPLEMENTATION_STATUS.md`.

---

**Next Steps:** Finish JSON asset + schema, implement data model + director, then build scene scaffolding following this plan.
