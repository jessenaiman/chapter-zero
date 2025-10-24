# Ghost Terminal · Bonus Archive

This folder preserves the experimental Ghost Terminal content that is no longer part of the
chapter-zero playable demo. Use it as reference material or bonus content when we want to surface
the earlier interactive explorations.

- `legacy_opening/` — Godot scenes from the pre-integration build. These still point at the old
  `res://source/scripts/stages/stage_1/` script locations and intentionally remain untouched so the
  historical setup can be inspected in the editor without affecting the shipping flow.
- `alternate_intro.json` — The original JSON narrative block that predates the current cinematic
  schema. It keeps the three-question structure documented in the first design sprint.
- `prototype_ghost_terminal_content.json` — Snapshot of the TypeScript prototype that powered the
  browser-based Ghost Terminal concept (`ghost-terminal/source/data/ghost-terminal-content.json`).

None of these assets are loaded by the game at runtime. They exist purely so the creative work is
easy to revisit when we want to surface alternate intros or bonus materials.
