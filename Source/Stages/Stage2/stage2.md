# Stage 2 · Echo Chamber

Act I’s first playable “Dreamweaver trial.” Light, Mischief, and Wrath guide the player through combat, stealth, or subversion challenges to seed alignment scores and foreshadow the finale’s mirrored escape routes.

---

## Creative Snapshot

- **Fantasy**: NetHack-inspired simulation run by the Dreamweavers to test the player’s instincts.
- **Structure**: Three-choice interlude → bespoke chamber encounter → debrief. Choices map cleanly to the fight / sneak / break philosophies that repeat in Stage 5.
- **Player Goal**: Discover the Dreamweaver that resonates most while harvesting lore from environmental terminals and NPC echoes.

---

## Content & Data

- Authoritative data file: `res://Source/Stages/Stage2/stage2.json`
- Schema: `res://Source/Data/Schemas/echo_chamber_schema.json`
- Loader: `EchoChamberDirector` (turns JSON into runtime plan via `NarrativeSceneFactory`)
- Scenes: `EchoHub.tscn`, `EchoInterlude.tscn`, `EchoDungeon.tscn`
- Tests: `Tests/Stages/Stage2/EchoChamberDirectorTests.cs` (schema + factory regression)

---

## Design Notes

- **Mirrors Stage 5**: Ensure each path’s narrative beats and mechanics line up with the finale illusions so foreshadowing lands.
- **NPC Chatter**: Drop hints that other “candidates” have attempted the trial (helps set up Stage 5’s revelation).
- **Scoring**: Every significant interaction must call `DreamweaverAlignmentTracker` to keep Stage 5 payoff honest.
- **NobodyWho hooks**: Reserve one tile-room template per path that the AI can remix (preview for Stage 5’s live tile swap).

---

## Implementation Checklist

1. Flesh out chamber encounter scripts with banter, scoring deltas, and optional lore pickups.
2. Wire hub → encounter transitions via `SceneManager`.
3. Hook audio, shader, and animation polish (glitch reveals, echo ambience).
4. Expand GDUnit coverage to include scoring permutations and banter branches.

---

## QA / Validation

Run the targeted suite to ensure data compatibility:

```bash
dotnet test --filter "FullyQualifiedName~EchoChamberDirectorTests"
```

Manually verify that each path awards the appropriate Dreamweaver alignment and that optional lore breadcrumbs mention other players in the simulation.
