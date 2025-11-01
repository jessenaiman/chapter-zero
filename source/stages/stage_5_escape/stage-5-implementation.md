# Stage 5 · Fractured Escape

The simulation’s façade shatters. Light, Mischief, and Wrath split, each insisting their playbook is the only path to freedom. The player can roam a collapsing hub and commit to one of three mirrored escape routes—fight, slip through the alleys, or break the system—only to discover every path spirals back to the same showdown.

---

## Creative Snapshot

- **Fantasy**: Final sprint out of the ghost terminal. Dreamweavers argue, the world retextures itself in real time, and an implacable “collector” hunts for loose code.
- **Structure**: Free-roam panic plaza with three exits. Each exit uses the same underlying encounter skeleton but re-skins tiles and dialogue to reflect the chosen Dreamweaver. Countdown Ui and collector enemy add urgency.
- **Reveal**: Crescendo dialogue confirms there are *two other players* running parallel escapes. Regardless of route, the collector forces a final convergence that feeds the closing stage.

---

## Player Flow

1. **Hub Commotion** – Player regains control inside a warped version of the Stage 2 hub. Dreamweavers bicker and project their exit routes across the plaza. Environmental clues (duplicate footprints, discarded gear) hint that other players already made choices.
2. **Decision Nodes (3)** – Match Stage 2 philosophies:
   - `Fight`: Light’s direct assault through barricaded boulevards.
   - `Sneak`: Mischief’s back-alley slipstream with limited tile swaps.
   - `Break`: Wrath’s system overload, glitching collision and AI.
   Each shares the same encounter cadence: preamble → collector clash → traversal → convergence trigger.
3. **Collector Showdown** – A C# garbage collector avatar (nicknamed *The Sweeper*) materialises every few beats, iterating phrases like “Scanning for unreferenced heroes.” The third appearance is unavoidable and initiates the convergence.
4. **Countdown Pressure** – “Memory Overflow Imminent” timer appears as soon as the player crosses the first checkpoint. Total time generous (e.g., 6 minutes) but scripted VO and VFX escalate panic at thresholds. Timer Ui uses a tightening infinity loop motif.
5. **Spiral Back** – No matter the route, the player emerges onto the same glitching bridge stub. Dreamweavers realise the other two players chose differently, sparking the schism that leads into the closing stage.

---

## Key Systems & Content

- **Data File**: `res://source/stages/stage_5/stage5.json`
- **Schema**: `res://source/data/schemas/spiraling_horizon_schema.json` (revise to handle hub + three mirrored routes + convergence payload).
- **Countdown**: Data-driven thresholds that fire VO cues, FX bursts, and collector behaviour swaps.
- **Collector Enemy**:
  - Shared stats across routes; difficulty escalates with each encounter (adds clones, spawns area denial).
  - Dialogue bank references garbage collection metaphors (“Mark sweep commencing,” “Dispose orphaned thread”).
  - Final defeat triggers convergence scene event.
- **NobodyWho Tile Swap**: Each route exposes one designated room layout with `allowAiRemix = true`. AI may shuffle props/exits within a defined mask to demo live remix without breaking navigation.
- **Other Player Evidence**:
  - Audio logs referencing “Candidate B / Candidate C.”
  - Visual anomalies (after-images, dropped inventory cards) that persist across routes.
  - Dreamweaver reactions that acknowledge those anomalies and debate whether to turn back for them.

---

## Narrative & Dialogue Beats

- **Dreamweaver Argument**: Cold open dialogue where Light pleads for coordinated resistance, Mischief urges improvisation, Wrath demands decisive force. Argument escalates as timer appears.
- **Route Banter**: Each path features bespoke VO but reuses collector encounter scripting to maintain parity. Repeated lines intentionally highlight the illusion of choice.
- **Other Players Reveal**: Mid-route banter confirms at least two other players are active. Dreamweavers disagree on rescuing vs. escaping—sets up why they split for the finale.
- **Convergence Setup**: Last lines before hand-off foreshadow the system log voice (“Do you hear that other narrator? The one none of us command?”).

---

## Implementation Checklist

1. Update schema + JSON to capture hub metadata, three route templates, collector encounter stages, timer thresholds, and convergence trigger.
2. Extend `NarrativeSceneFactory.GhostTerminal` with helpers for `EscapeRoute`, `CollectorPhase`, and countdown events.
3. Build reusable encounter scene that accepts a layout skin (fight/break/sneak) while sharing logic and enemies.
4. Hook timer Ui, VO cues, and collector escalation to data-driven events.
5. Expose hooks for NobodyWho tile remix on the designated room per route.
6. Persist route choice, countdown completion time, and collector outcomes to feed the closing stage system log.

---

## QA / Validation

- Validate that each route shares identical combat pacing and collector behaviour (only presentation changes).
- Ensure timer never soft-locks the player but still fires escalating VO/FX at 75% / 50% / 25% remaining.
- Confirm evidence of other players appears in all routes and carries into convergence dialogue.
- Regression test NobodyWho remix room to ensure navigation meshes and enemy paths remain intact.
