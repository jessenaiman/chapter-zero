# Chapter Zero · Storyboard

High-level walkthrough of Act I (“Chapter Zero”) with key creative beats, decision points, and data assets per stage_. Use this as the canonical reference when syncing narrative, design, and schema changes.

---

## stage_ 1 · Ghost Terminal (`stage_1/stage_1.md` · `stage_1/stage_1.json`)

- **Focus**: AI-controlled onboarding terminal that interrogates the player’s philosophy.
- **Decisions**: Philosophical prompts hide Dreamweaver alignment seeds.
- **Content Notes**: Boot sequence glitches, opening monologue, secret question. Uses `ghost_terminal_cinematic_schema.json`.
- **Hand-off**: Records initial alignment; transitions to stage_ 2’s Echo Chamber trial.

## stage_ 2 · Echo Chamber (`stage_2/stage_2.md` · `stage_2/stage_2.json`)

- **Focus**: NetHack-inspired simulation with three approaches (fight / sneak / break).
- **Decisions**: Choice of chamber path + optional lore pickups influence alignment totals.
- **Content Notes**: Scene templates for hub, interlude, dungeon; NobodyWho remix room reserved.
- **Hand-off**: Dreamweaver commentary sets expectation that these philosophies matter later.

## stage_ 3 · Echo Vault (`stage_3/stage_3.md` · `stage_3/stage_3.json`)

- **Focus**: Retro dungeon crawl rescuing echo party members across presentation tiers.
- **Decisions**: Which echoes to free, how to pay memory costs, optional Omega log interruptions.
- **Content Notes**: Tiered art swaps, echo roster data, Dreamweaver banter about other players.
- **Hand-off**: Party + memory debt persisted for stage_ 4; reinforce notion of parallel candidates.

## stage_ 4 · Liminal Township (`stage_4/stage_4.md` · `stage_4/stage_4.json`)

- **Focus**: Free-roam nostalgia town with cryptic NPC dialogue.
- **Decisions**: Light interactions nudge alignment; player chooses which Dreamweaver to trust heading into escape.
- **Content Notes**: Hub metadata, NPC scripting, environmental hints at repeated loops.
- **Hand-off**: Locks Dreamweaver selection snapshot and funnels player toward stage_ 5 plaza.

## stage_ 5 · Fractured Escape (`stage_5/stage_5.md` · `stage_5/stage_5.json`)

- **Focus**: Collapsing hub with three mirrored escape routes; timer and Omega Sweeper enforce urgency.
- **Decisions**: Select fight / sneak / break route (mirrors stage_ 2), manage countdown, respond to collector encounters.
- **Content Notes**: Hub argument, route templates, countdown thresholds, NobodyWho remix hooks, evidence of two other players.
- **Hand-off**: Records route choice + timer outcome, then triggers convergence leading into closing stage_.

## stage_ 6 · System Log Epilogue (`stage_6/stage_6.md` · `stage_6/stage_6.json`)

- **Focus**: Non-interactive system log that finishes the hero scroll while the world crashes.
- **Decisions**: None—player watches log seal the chapter.
- **Content Notes**: Essay interleaved with system errors referencing Frodo, Bastion, and bold heroism. Final render reveals Omega Spiral logo.
- **Hand-off**: Saves alignment snapshot and exit data for Act II prototype; rolls into credits/teaser.

---

## Cross-stage_ Themes & Requirements

- **Infinity Spiral**: Every route loops back—ensure each stage_ surfaces this symbolically or literally.
- **Other Players**: From stage_ 2 onward, breadcrumbs must suggest at least two additional candidates.
- **Schema Discipline**: When content changes, keep `stage_N.json` aligned with its declared schema; update `NarrativeSceneFactory` mappings + tests in lockstep.
- **NobodyWho Integration**: Limit live remix to designated rooms (stage_ 2 and stage_ 5) to preserve designer control while demoing capability.
- **Data Ownership**: All narrative text resides in JSON; directors transform only.
