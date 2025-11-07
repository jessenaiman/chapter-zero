# Stage 1 · Ghost Terminal

## Vision Statement

The opening sequence is the **hook that defies convention**. It must:

- Feel like an archaeological artifact adapting to 2025
- Create immediate mystery through iteration counter and historical interfaces
- Establish Omega as a character through voice, not exposition
- Hide the Dreamweaver selection mechanism in philosophical conversation
- Reward replay through subtle differences in interpretation
**

---

## Critical Creative Requirements

### The Forbidden Fruit Hook

Players must feel **warned away** before being allowed to continue:

- "This interface is not safe. Go back. Play your safe games."
- Echoes *The NeverEnding Story* shopkeeper's warning
- Creates immediate defiance: "I'm doing this anyway"

**Shader Parameters:**

```gdshader
glitch_intensity = 1.0
scanline_speed = 3.0
rgb_split = 7.0
symbol_bleed = 0.8
noise_amount = 0.6
```

### State 2: STABLE BASELINE

**Emotional Beat:** Settling into false comfort → Building unease

**Visual Requirements:**

- Consistent phosphor glow (green-tinted, not pure green)
- Subtle scanline movement (1x speed, barely visible)
- Occasional single-frame glitches (every 3-5 seconds)
- Text has slight RGB glow halo
- Screen curvature barely noticeable (15% strength)

**Shader Parameters:**

```gdshader
glitch_intensity = 0.0 (spikes to 0.3 on key beats)
scanline_speed = 1.0
scanline_intensity = 0.4
phosphor_glow = 1.2
phosphor_tint = vec3(0.2, 1.0, 0.4) // Green CRT
vignette_strength = 0.3
```

**Trigger Points for Glitch Spikes:**

- "You should not be here" → `glitch_intensity = 0.5` for 0.3s
- "They never do." → `rgb_split = 2.0` for 0.2s
- "The ones who were chosen." → `flicker_frequency = 5.0` for 0.5s

### State 3: SECRET REVEAL

**Emotional Beat:** This is not a game anymore → I need to protect this knowledge

**Visual Requirements:**

- Ancient symbols overlay at 100% opacity
- Phosphor glow intensifies to near-white, edges burn brighter
- Scanlines freeze momentarily, then pulse rhythmically like a heartbeat
- Code fragment (`∞ ◊ Ω ≋ ※`) rendered with custom corona shader
- Screen brightness increases 20%, vignette darkens to focus attention
- Subtle audio: Low resonant tone (like a Tibetan singing bowl) builds during reveal

**What Makes This Legendary:**
The secret must feel **dangerous to know**. Not "cool lore" but "I just picked up something that will change me."

**Enhanced Shader Parameters:**

```gdshader
symbol_bleed = 1.0
phosphor_glow = 2.5  // Increased for intensity
scanline_speed = 0.0 (frozen for 1.5s, then 2.0 heartbeat pulse)
custom_glow_intensity = 3.0 (corona effect on symbols)
vignette_strength = 0.6  // Darker edges, spotlight on symbols
chromatic_aberration = 2.0  // Reality slightly unstable
```

**Enhanced Cinematic Timing:**

```text
[PAUSE: 4s with building resonant hum]
[TEXT GLITCHES: Ancient symbols flash beneath, frequency increases]
[Terminal colors desaturate slightly, world feels "wrong"]

"I need to tell you something. But first—"

[Player choice: Can you keep a secret?]

[On keypress: 'TODO: add ominous message']
[Fade symbol_bleed to 0.3 over 4 seconds - symbols remain faintly visible]
[Journal notification: "Fragment 1/5 recorded"]
```

**Why This Works:**

- No time pressure = Player can screenshot, draw, memorize
- Forced interaction = Must acknowledge receiving the fragment
- Persistent ghost = Symbols remain faintly visible in background for rest of game
- Audio signature = Each fragment has unique resonance (collectible "notes")

### State 4: DREAMWEAVER SELECTION

**Emotional Beat:** My choice is locking in something I don't fully understand

**Visual Requirements:**

- Phosphor tint crossfades to thread-specific color over 3 seconds
- Scanline behavior shifts to match thread personality
- Final aesthetic "locks in" with subtle screen shake

**Thread-Specific Parameters:**

#### Light Thread (Golden Certainty)

```gdshader
phosphor_tint = vec3(1.0, 0.9, 0.5) // Warm gold/amber - hopeful but grounded
scanline_intensity = 0.25 // Cleaner, more stable - confidence in clarity
flicker_frequency = 0.8 // Minimal flicker - steady conviction
vignette_strength = 0.15 // Brightest overall - illumination without shadow
glow_spread = 1.4 // Soft halo - warmth that extends outward
```

**Personality:** Bold risk-taker who believes in absolute good

- Takes definitive action based on moral conviction
- "One choice can unmake a world" = Weight of decisive intervention
- Will sacrifice personal safety for the greater good
- Confident but not arrogant; certain but willing to be wrong once

**NobodyWho Voice Tags:** `[VOICE:LIGHT_DECISIVE]`, `[VOICE:LIGHT_HOPEFUL]`

#### Shadow Thread (Violet Patience)

```gdshader
phosphor_tint = vec3(0.6, 0.3, 0.8) // Deep violet - twilight memory
scanline_intensity = 0.45 // Moderate distortion - history layered over present
rgb_split = 1.5 // Slight reality shimmer - past bleeding through
flicker_frequency = 2.0 // Nostalgic pulse - remembering what was
vignette_strength = 0.5 // Darker edges - comfort in the periphery
```

**Personality:** Non-interventionist who values tradition and patience

- Believes most problems resolve themselves given time
- "Truth hides until you bleed for it" = Knowledge earned through endurance, not force
- Nostalgic for simpler times, wary of hasty change
- Intervention is the last resort, not the first instinct

**NobodyWho Voice Tags:** `[VOICE:SHADOW_REFLECTIVE]`, `[VOICE:SHADOW_NOSTALGIC]`

#### Ambition Thread (Crimson Self)

```gdshader
phosphor_tint = vec3(1.0, 0.2, 0.05) // Deep red-orange - controlled burn
scanline_intensity = 0.55 // Aggressive movement - always pushing forward
flicker_frequency = 3.5 // Intense but rhythmic - calculated urgency
vignette_strength = 0.45 // Dark edges, burning center - focus on the prize
glow_spread = 0.8 // Sharp edges - precise, no wasted energy
```

**Personality:** Self-interest as moral framework (not evil, strategic)

- Every choice must benefit you first; altruism that costs you is foolish
- "Changes every time you look away" = Adaptability is survival
- Not cruel, but transactional: "What do I gain from this?"
- Can be heroic if heroism serves your goals

**NobodyWho Voice Tags:** `[VOICE:AMBITION_PRAGMATIC]`, `[VOICE:AMBITION_DIRECT]`

---

## Point Scoring System: The Hidden Fourth Path

### Core Mechanism

Each stage has **3 decision points**. Each decision has **3 choices** (one per Dreamweaver).


## Audio Architecture: Communication Through Time

### Core Concept: "Ghost in the Machine"

Omega's interface should sound like **communication artifacts from every era it's existed**:

- Layers of historical technology bleeding through
- Modem handshakes as "digital archaeology"
- CRT hum as the constant baseline (Omega's heartbeat)
- Keys typing that aren't being pressed (ghost input from failed attempts)

### Audio State Layers

#### Layer 1: Ambient Foundation (Always Present)

**CRT Hum (60Hz Tone)**

- Base frequency: 60Hz (US power) with harmonics at 120Hz, 180Hz
- Volume: -24dB (subliminal, felt more than heard)
- Slight flutter (0.5Hz wobble) to simulate aging capacitors
- **Accessibility Note:** Must be disableable for players with tinnitus/audio sensitivity

**Electrical Interference Pattern**

- Periodic crackle every 3-7 seconds (random interval)
- Simulates degraded power supply
- Volume: -18dB spikes

#### Layer 2: Boot Sequence (State 1)

**Modem Handshake Symphony**

- 56k dialup modem negotiation (3 seconds)
- Layered with:
  - Telegraph clicks (Morse code patterns)
  - Fax machine tones (high-pitched squeal)
  - Dot-matrix printer mechanical sounds
- Creates "digital evolution" soundscape
- Each historical layer fades in/out during glitch sequence

**Sample Timing:**

```text
0.0s: Telegraph clicks (dit-dit-dah pattern)
0.5s: Rotary phone dial tone enters
1.0s: Modem carrier tone begins
1.5s: Fax machine squeal overlays
2.0s: All layers crescendo
2.5s: Hard cut to clean CRT hum
```

**Audio Files Needed:**

- `telegraph_morse_pattern.ogg` (public domain)
- `modem_56k_handshake.ogg` (Creative Commons)
- `fax_machine_negotiation.ogg`
- `dotmatrix_printer_feed.ogg`

#### Layer 3: Stable State (State 2)

**Ghost Typing**

- Mechanical typewriter keys (IBM Selectric sound)
- Random 1-3 key presses every 5-10 seconds
- Sounds like someone else is typing just out of sync
- Volume: -12dB (noticeable but not intrusive)
- **Narrative Purpose:** "847,293 failed attempts still echoing"

**Cursor Blink Tone**

- Subtle electronic pulse synchronized with visual cursor
- 600Hz sine wave, 50ms duration, every 0.53 seconds
- Volume: -30dB (barely audible, subliminal rhythm)

#### Layer 4: Choice Moments

**Selection Hover Sound**

- Soft electrical charge-up (rising pitch)
- 200Hz → 400Hz over 0.2s
- Volume: -18dB

**Selection Confirm Sound**

- Mechanical relay click + electronic confirmation
- Two-part sound:
  1. Physical: Relay switch click (100ms)
  2. Electronic: 800Hz tone burst (80ms)
- Volume: -12dB

**Dreamweaver Thread-Specific Tones:**

- **Light:** Warm analog synth (C major chord, 528Hz fundamental)
- **Shadow:** Detuned bells (Eb minor, 432Hz with slight dissonance)
- **Ambition:** Distorted sawtooth (F# power chord, 639Hz aggressive)

#### Layer 5: Secret Reveal (State 3)

**THE MOST IMPORTANT AUDIO MOMENT**

**Build-Up (4 seconds):**

```text
0.0s: CRT hum increases 200% volume
0.5s: Low sub-bass drone (40Hz) enters
1.0s: Modem tones return (fragmented, glitching)
1.5s: Multiple ghost typewriters typing simultaneously
2.0s: All historical sounds layer (cacophony)
2.5s: Sudden silence (0.3s)
3.0s: Clean Tibetan singing bowl tone begins
```

**Singing Bowl Resonance:**

- Fundamental: 432Hz (natural resonance frequency)
- Overtones: 864Hz, 1728Hz (octave harmonics)
- 6-second sustain with natural decay
- Each code symbol punctuates with a different overtone:
  - `∞` → 432Hz (base)
  - `◊` → 540Hz (perfect fifth)
  - `Ω` → 648Hz (perfect fourth above fifth)
  - `≋` → 810Hz (major third)
  - `※` → 972Hz (resolution)

**Post-Reveal Ambience:**

- Symbols remain on screen with faint harmonic drone
- All 5 tones sustained at -36dB (barely audible hum)
- Creates subconscious "presence" of the fragment

#### Layer 6: Dreamweaver Lock-In (State 4)

**Thread Transformation Sound:**

- 3-second crossfade of CRT hum modulating to thread frequency
- **Light:** Warm analog warmth (slight tape saturation)
- **Shadow:** Vinyl crackle overlay (nostalgic texture)
- **Ambition:** Digital aliasing (sharp, modern edge)

**Final Lock Sound:**

- Mechanical vault lock turning (2 rotations)
- Heavy metallic *thunk* at end
- Conveys: "Your choice is permanent"


### Audio File Structure

```file
res://source/Assets/Audio/Stage1/
├── Ambient/
│   ├── crt_hum_60hz.ogg (looping)
│   ├── electrical_crackle_01.ogg
│   ├── electrical_crackle_02.ogg
│   └── electrical_crackle_03.ogg
├── Historical/
│   ├── telegraph_morse.ogg
│   ├── modem_56k_handshake.ogg
│   ├── fax_negotiation.ogg
│   ├── dotmatrix_printer.ogg
│   └── typewriter_ghost_keys.ogg (randomized samples)
├── Ui/
│   ├── choice_hover.ogg
│   ├── choice_confirm_light.ogg
│   ├── choice_confirm_shadow.ogg
│   ├── choice_confirm_ambition.ogg
│   └── cursor_blink_pulse.ogg
├── SecretReveal/
│   ├── buildup_layers.ogg (pre-mixed cacophony)
│   ├── singing_bowl_432hz.ogg
│   ├── symbol_tone_01_infinity.ogg (432Hz)
│   ├── symbol_tone_02_diamond.ogg (540Hz)
│   ├── symbol_tone_03_omega.ogg (648Hz)
│   ├── symbol_tone_04_wave.ogg (810Hz)
│   └── symbol_tone_05_asterisk.ogg (972Hz)
└── ThreadThemes/
    ├── light_lock_warmth.ogg
    ├── shadow_lock_vinyl.ogg
    ├── ambition_lock_digital.ogg
    └── vault_lock_mechanical.ogg
```

---

## Player Experience Design: Mouse & Keyboard Input

### Core Design Philosophy

**NO TEXT INPUT. EVER.**

This is crucial for:

1. **Accessibility:** Text input is a barrier for many players
2. **Consistency:** Mouse/keyboard interface from moment one
3. **Narrative:** Omega is asking philosophical questions, not requesting data entry
4. **Localization:** Choice buttons translate cleanly, text input doesn't

### Input Methods

**Mouse/Keyboard Players:**

- **Navigation:** Mouse hover highlights choices
- **Selection:** Left-click to confirm
- **Keyboard Alternative:** Arrow keys to navigate, Enter to confirm, Escape to go back
- **Visual Feedback:** Highlight outline + soft glow on hover

### Choice Button Design Spec

```
┌─────────────────────────────────────────────────────┐
│  [ICON] Yes. Names are promises we make to         │
│         ourselves.                                   │
│                                                      │
│         [Light Thread Affinity: ██████░░░░]        │
└─────────────────────────────────────────────────────┘
```

**Visual Elements:**

- **Icon:** Small symbolic glyph representing philosophical stance
  - Light: ☀ (sun/illumination)
  - Shadow: ☽ (moon/reflection)
  - Ambition: ⚡ (lightning/transformation)
- **Text:** Choice content (word-wrapped, left-aligned)
- **Affinity Bar:** Subtle indicator showing which thread this choice favors (optional, can be hidden for mystery)
- **Hover State:** Phosphor glow around border, icon pulses
- **Selected State:** Border thickens, phosphor glow intensifies

### Accessibility Enhancements

**Visual Clarity:**

- High contrast mode option (black background, white text, no phosphor effects)
- Adjustable text size (Small/Medium/Large/Extra Large)
- Dyslexia-friendly font option (OpenDyslexic as alternative)
- Focus indicators must be 2px minimum, high contrast

**Mouse/Keyboard Navigation:**

- Large click targets (minimum 48px height per choice)
- Hover state must appear within 100ms
- No double-click required anywhere
- Right-click opens context menu with "Read Choice Aloud" option (if audio description enabled)

### Player Flow Specification

**Boot Sequence (Non-Interactive):**

- No choices
- Player cannot skip (intentional discomfort)
- Press any key to continue after "...how many times must I ask..."

**Opening Monologue (Minimal Interaction):**

- Auto-advances between lines (2.5s pause between)
- Player CAN press Space to speed up (but not skip entirely)
- Final line "The ones who were chosen." holds until player acknowledges

**Choice Presentation:**

After each philosophical question, player sees:
- Question prompt with context
- 3 choice options displayed as buttons
- Each choice aligned with a Dreamweaver philosophy
- After clicking choice, brief pause (0.3s) before Omega responds
- Choice fades out, response fades in (smooth transition)

**Secret Fragment Reveal:**

**CRITICAL: NO SKIP ALLOWED**

- After player selects "Can you keep a secret?" answer, 4-second audio buildup MUST complete
- Disable all input during symbol reveal
- Symbols appear one at a time (cannot rush)
- After all 5 symbols visible, display: `[Press any key to acknowledge]`
- Only THEN does game continue
- Screenshot-friendly pause allows players to capture the moment
