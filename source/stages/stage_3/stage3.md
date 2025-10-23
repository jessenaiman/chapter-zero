# Stage 4 · Liminal Township

The calm before the exodus. A nostalgic JRPG village that lets the player wander freely, soak up lore, and notice the seams in the simulation before the Dreamweavers split.

---

## Creative Snapshot

- **Fantasy**: Wake up in a “perfect” RPG town that clearly remembers too many eras at once. NPCs are friendly but speak in riddles about loops, other wanderers, and choices that refuse to stay finished.
- **Structure**: Free-roam hub with light gating. Players can visit the inn, shop, training yard, and town edge before committing to the Dreamweaver they’ll follow in Stage 5.
- **Theme Delivery**: Dialogue seeds the idea that *other* players are in the simulation and that every road still spirals back to this town.

---

## Content & Data

- Data file: `res://source/stages/stage_4/stage4.json` (scene configuration stub; extend with NPC, dialogue, and trigger metadata).
- Schema: `res://source/data/schemas/dungeon_sequence_schema.json` (re-use for hub layout until dedicated schema lands).
- Scenes: Town hub map, interior scenes (inn/shop/dojo), and gate trigger that hands off to Stage 5.

---

## Design Notes

- **Player Agency**: Let players roam and interact without pressure; Dreamweavers appear as translucent avatars near key POIs offering persuasive arguments.
- **NPC Clues**: Tropes with a twist—shopkeeper recounts “another hero” who already bought a weapon, innkeeper resets the player’s room “again,” children hum the Stage 2 trial theme.
- **Foreshadowing**: Environmental storytelling (duplicate footprints, fading banners) evidence that multiple players exist.
- **Boundaries**: Invisible walls disguised as polite NPC guidance or weather anomalies to keep scope manageable.

---

## Implementation Checklist

1. Populate `stage4.json` with hub metadata (spawn points, interactable IDs, Dreamweaver encounter triggers).
2. Script NPC dialogues that escalate references to the other players.
3. Create minor side activities (e.g., sparring dummy, lore tomes) that grant small alignment nudges.
4. Build the Dreamweaver commitment scene at the town edge; record alignment snapshot for Stage 5.

---

## QA / Validation

- Manual roam: confirm all POIs reachable, no soft locks when player delays commitment.
- Alignment audit: ensure optional interactions subtly adjust Light/Mischief/Wrath values without locking Stage 5 options.
- Narrative flow: verify Dreamweaver arguments set up the Stage 5 fracture and mention the impending countdown.
