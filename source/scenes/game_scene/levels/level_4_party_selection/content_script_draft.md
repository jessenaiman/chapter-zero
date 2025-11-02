# Stage 3: The Echo Vault — Content Script (Draft)

> *Internal design reference. Mirrors are treated as "echo slots" in the public-facing copy.*

---

## Scene Card

```
Stage 3 — The Echo Vault
Subtitle: Selections from Omega’s archive
Palette: high-contrast monochrome → glitch color as tiers advance
```

## Opening Beat

```
[SYSTEM] PROCESS: ECHO_ARCHIVE // STATUS: DEGRADED
The chamber breathes. Dust motes drift through a dark lattice of framed light.
Each frame hums with a half-remembered silhouette. None of them are your own.

Light:  “The archive stirs. We have so little time.”
Shadow: “Omega stockpiles failures here. We call them… opportunities.”
Ambition: “Choose quickly. Every echo you pull thins the bars.”
```

## Simple Point Tally

- Hidden ledger tracks Light, Shadow, and Ambition across the three echo selections.
- Each decision point is “owned” by one Dreamweaver. Choosing their favored echo awards them **+1 point**.
- Selecting an echo aligned with a different Dreamweaver sends the point to that patron instead.
- Totals remain unseen until the Stage 3 finale summary.

| Decision Point | Owner | Owner-Favored Choice (example) | Alternate Outcome |
| -------------- | ----- | -------------------------------- | ----------------- |
| Beat 1 – First Echo | Light | Gate Sentinel, Spark Archivist, Scribe → Light +1 | Courier (Shadow) → Shadow +1 |
| Beat 3 – Second Echo | Shadow | Veiled Jester or reroll chaos → Shadow +1 | Auric Mentor → Light +1 / Ashen Vanguard → Ambition +1 |
| Beat 5 – Final Echo | Ambition | Ashen Vanguard pairing or Glitched Shade → Ambition +1 | Twin Eclipse → Light +1 / Duplicate Guardian → Shadow +1 |

## Structure Overview

- Three selection loops (“echo slots”) → combat → return.
- Presentation layer upgrades each loop (Tier 0 → Tier 2).
- Omega absent; system logs warn about instability.
- Player’s Stage 2 Dreamweaver champion provides extra asides.
- Each echo is an “archived moment” (not just a class).

---

## Tier 0 — First Echo Slot (Decision Point 1: Light-owned)

### Vault presentation

- Ui minimal: black background, white text, static hum.
- Three frames flicker, each showing a faint silhouette.

### Dreamweaver intro

```
Light:   “Omega has trapped players here before. These frames are what remains.”
Shadow: “Tap the glass and they wake. But you’ll owe them. You’ll owe yourself.”
Ambition:  “We take three. No more, or the loop noticed us last time.”
Champion whisper (depends on Stage 2 winner):
    Light champ:   “I trust your hand. Slow is safe; safe is death. Decide.”
    Shadow champ:  “Remember the pack. We move together or we fade.”
    Ambition champ:“Speed. Precision. Take the ones who fight back.”
```

**Owner:** Light
**Point Rule:** Accept one of Light’s favored echoes (Gate Sentinel, Spark Archivist, Scribe) → Light +1. Choose Shadow’s Courier → Shadow +1.
**Dreamweaver reaction variants:**

- Light (supportive): “You felt that steadiness? That was hope returning.”
- Light (concerned): “Even with them beside you, do not forget what you surrendered.”
- Shadow (supportive): “Ha! Solid opener. Let’s see the loop chew through that.”
- Shadow (skeptical): “You sure? I’d have gone flashier. Keep them entertained.”
- Ambition (supportive): “Good spine. Aim them at the front and push.”
- Ambition (warning): “If they waver, you replace them. No sentiment.”

### Echo options (Tier 0)

1. **“Gate Sentinel // Iteration 235”**
   - Visual: Player barring a door, alone.
   - Description: “Held the northern gate for eight minutes before the code swallowed them.”
   - Mechanics: Defender archetype (Guardian).
   - Cost line (on selection):
     ```
     Frame empties. The silhouette steps forward, helm cracked.
     Light: “They remember your grip from the gate. Take it.”
     Ambition: “Do not waste their sacrifice. Again.”
     Shadow: “Look, the gap they left… same shape as you.”
     ```

2. **“Spark Archivist // Iteration 188”**
   - Visual: flickering glyphs, figure writing mid-air.
   - Mechanics: Mage archetype.
   - Selection cost: lose a remembered name (“SYSTEM// erasing personal field: FavoriteStory”).

3. **“Courier in Shadow // Iteration 91”**
   - Visual: sprinting silhouette carrying a data disk.
   - Mechanics: Rogue archetype ( high initiative).
   - Shadow: “Oh, I liked this one. Quick fingers, quicker feet.”

4. **Reroll node**
   - Text: “Shuffle Echo Frames (risk: introduces static).”
   - On use: system log spikes `[ERROR] ARCHIVE INDEX UNSTABLE` and Dreamweavers react (“Stop before the watchdog catches it!”).

### Combat 1

Encounter: Single Wolf Hybrid (protocol guard).
Battle log style: Wizardry-like (“> Sentinel braces. Damage 4 resisted.”).
On victory: upgrade Tier to 1.
On defeat: echo returns to frame with scars; Dreamweavers comment (“It hurt to lose them again.”).

---

## Tier 1 — Second Echo Slot (Decision Point 2: Shadow-owned)

### Presentation Upgrade

- Color accents appear (dominant Dreamweaver’s hue leaks in).
- Additional static in corners; frames oscillate.

### Dreamweaver intro

```
Light: “The first echo steadied the archive. The second strains it.”
Shadow: “Omega hears the glitch. Keep moving.”
Ambition: “Choose. Then fight. Then run.”
```

**Owner:** Shadow
**Point Rule:** Choose Shadow’s Veiled Jester (or trigger reroll chaos she endorses) → Shadow +1. Selecting Light’s Auric Mentor → Light +1. Selecting Ambition’s Ashen Vanguard → Ambition +1.
**Dreamweaver reaction variants:**

- Light (supportive): “Balance and wit. Perhaps this time we dance instead of crawl.”
- Light (uneasy): “Too much chaos invites Omega’s gaze. Keep them focused.”
- Shadow (supportive): “Oh yes, perfect glitch fuel. We can work with this.”
- Shadow (critical): “Boring pick, but maybe reliable. Prove me wrong.”
- Ambition (supportive): “Speed or flame—either breaks the guard. Use them.”
- Ambition (dismissive): “If they play games, leave them in the archive.”

### Echo options (Tier 1)

Unlocked by Stage2 champion:

- If Light champion:
  **“Auric Mentor // Iteration 42”** – support archetype, heal passive.
  Light: “They taught me patience. I never listened. Maybe you will.”
- If Shadow champion:
  **“Veiled Jester // Iteration 307”** – trickster archetype (dodge + debuff).
  Shadow: “Heh. They laughed at Omega and lived. Once.”
- If Ambition champion:
  **“Ashen Vanguard // Iteration 12”** – striker archetype (high damage, low defense).
  Ambition: “They burned everything but the goal. Perfect.”

Baseline echos remain (in case player wants duplicates).
Each selection scrubs a “memory field” from player: `[SYSTEM] Personal Artifact — Childhood Song: REDACTED`.

### Combat 2

Encounter: Wolf pack w/ static wisp (appears if shadow taken).
Omega log mid-fight: `[WATCHDOG] Party size exceeds baseline > adjusting difficulty.`
Dreamweavers push urgency: “Omega noticed. Finish it before he rewrites the arena!”

---

## Tier 2 — Final Echo Slot (Decision Point 3: Ambition-owned)

### Presentation Upgrade

- Half the frames flicker in full color; text overlays glitch.
- Dreamweaver characters flicker; lines overlap.

### Dreamweaver intro

```
Light: “Three echoes together? This never happened. In any loop.”
Shadow: “I love firsts. I hate endings. You see the problem.”
Ambition: “Final pull. After this, the archive collapses.”
Champion whisper: personalised pep talk.
```

**Owner:** Ambition
**Point Rule:** Accept Ambition’s Ashen Vanguard pairing or the unstable Glitched Shade → Ambition +1. Taking Twin Eclipse gives Light +1; duplicating Shadow-aligned echoes awards Shadow +1.
**Dreamweaver reaction variants:**

- Light (supportive): “With them, our line holds even when the code shakes.”
- Light (troubled): “We just traded another memory. Hold tight to what’s left.”
- Shadow (supportive): “That roster? Omega will choke on it. Delicious.”
- Shadow (worried): “Careful. Too much fury and we burn ourselves.”
- Ambition (supportive): “Now we hunt. Nothing escapes.”
- Ambition (critical): “If this fails, no more chances. Remember that.”

### Echo options (Tier 2)

Combo picks rather than single classes; picking one consumes two slots:

1. **“Twin Eclipse // Iteration 0 & 0”**
   - Paired echo: Guardian + Trickster synergy.
   - Mechanic: once per fight perform joint attack.
   - Price: `[SYSTEM] Removing Memory: Last Promise Made`
   - Dreamweaver reaction: Light & Shadow argue about what was lost.

2. **“Glitched Shade // Orphan Record”**
   - Omega-corrupted echo.
   - Effect: random ability each combat round (good or bad).
   - Shadow ecstatic; Light terrified; Ambition suspicious.

3. **“Duplicate Echo”** (if player wants to double up).
   - Dreamweavers disapprove but allow.

### Combat 3 — Code Guardian

Encounter: Code Guardian + fragments.
Omega interrupts mid-fight: `[OMEGA//...] INTERFERENCE DETECTED // dreamweaver leakage`.
If Glitched Shade selected, random Omega events more extreme.

### Finale

Regardless of victory:
```
[SYSTEM] ARCHIVE CAPACITY EXCEEDED
Light: “Hold on to them. The vault is failing.”
Shadow: “Stage four is raw code. If we get there.”
Ambition: “Run!”
```
Screen tears → Stage 4 boot sequence (JRPG town). Dreamweavers’ voices cut to static.

---

## Memory Debt Tracker (meta)

- After each selection, list which “memory fields” were erased. This can be used in Stage 4 to show missing dialogue choices or NPC responses.

## Omega Logs (example snippets)

- `[WATCHDOG://ALERT] PARTY SLOT THREE OCCUPIED // PATTERN NOT RECOGNIZED`
- `[ARCHIVE] Iteration 188 retrieval success // (note: subject sense of self diminishing)`
- `[FAILSAFE] Attempting containment > ERROR > System not responding`

---

## Next Steps

- Flesh out banter for all combinations (include champion-specific asides).
- Map each echo to combat stats in Stage 3 data file.
- Ensure GameState persistence captures:
  - echo_id
  - iteration origin (for future callbacks)
  - memoryLoss array
  - dreamweaver_affinity
