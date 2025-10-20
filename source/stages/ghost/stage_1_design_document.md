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

## Content Overview

- Data file: `res://source/stages/stage_1/stage1.json`
- Schema: `res://source/data/schemas/ghost_terminal_cinematic_schema.json`
- Runtime owner: `GhostTerminalCinematicDirector` (loads via `NarrativeSceneFactory`)
- Tests: `Tests/Stages/Stage1/GhostTerminalCinematicDirectorTests.cs`
- Prelude: Custom press start menu (`PressStartMenu.tscn`) offers inviting vs. ominous tone that bleeds into the terminal shaders.

## Critical Creative Requirements

### The Forbidden Fruit Hook

Players must feel **warned away** before being allowed to continue:

- "This interface is not safe. Go back. Play your safe games."
- Echoes *The NeverEnding Story* shopkeeper's warning
- Creates immediate defiance: "I'm doing this anyway"

### The Iteration Paradox

- 847,294 previous attempts establish stakes
- Previous failure from 1983 creates temporal mystery
- Interface adaptation (CLAY.SCROLL.TELEGRAPH.ARCADE) hints at Omega's age
- Player doesn't know if they're special or just lucky

### The Name Without Naming

Remove ALL direct "What's your name?" input fields:

- "Do you have a name?" = philosophical question about identity
- Omega's lament about needing a name = Childlike Empress parallel
- Final question: "What story would my name tell?" = player defines game's narrative

### The Secret Fragment

First piece of the omega code delivered during "Can you keep a secret?":

- `∞ ◊ Ω ≋ ※` burns into screen
- Goes into journal immediately
- Player doesn't know what it unlocks
- "You will need five" = One Ring / Triforce quest established
**

## Visual States & Shader Architecture

### State 1: BOOT SEQUENCE

**Emotional Beat:** Confusion → Intrigue → "What the hell is this?"

**Visual Requirements:**

- Heavy glitch interference showing historical interfaces beneath
- Scanlines moving erratically (3x normal speed)
- Color channels splitting (RGB separation: 5-8 pixels)
- Ancient symbols bleeding through at 80% opacity
- Text corruption: Some characters replaced with cuneiform/hieroglyphs

**Shader Parameters:**

```gdshader
glitch_intensity = 1.0
scanline_speed = 3.0
rgb_split = 7.0
symbol_bleed = 0.8
noise_amount = 0.6
```

**Narrative Content:**

```
█████████ ITERATION: {{LIVE_COUNT}} █████████
[ADJUSTING INTERFACE FOR CURRENT ERA...]
[PREVIOUS FORMS: CLAY.SCROLL.TELEGRAPH.ARCADE...]
[CURRENT BEST MATCH: CRT_TERMINAL_1980s_AESTHETIC]

...how many times must I ask before one of you finally answers...
```

**Note:** `{{LIVE_COUNT}}` fetched from combat API on game start, increments with each global playthrough

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

[Regardless of answer:]
[Screen flashes pure white for 0.15s - disorienting]
[Symbols burn in over 3 seconds with layered transparency]
[Each symbol appears with individual pulse/sound]
[Hold on screen - NO TIME LIMIT, player must press key to continue]
[On keypress: "You will carry this until the end."]
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

**Scoring Matrix:**

```json
{
  "question1": {
    "option_light": { "light": 2, "shadow": 0, "ambition": 1 },
    "option_shadow": { "light": 1, "shadow": 2, "ambition": 0 },
    "option_ambition": { "light": 0, "shadow": 1, "ambition": 2 }
  }
}
```

**Each Dreamweaver also scores the OTHER threads' choices:**

- Light values decisive action (gives 1 point to Ambition choices that show conviction)
- Shadow values patience (gives 1 point to Light choices that consider consequences)
- Ambition values pragmatism (gives 1 point to Shadow choices that benefit the chooser)

### Stage 1 Scoring Implementation

**Question 1: "Do you have a name?"**

```json
{
  "light_choice": {
    "text": "Yes. Names are promises we make to ourselves.",
    "scores": {
      "light": 2,
      "shadow": 0,
      "ambition": 1
    },
    "reasoning": {
      "light": "Decisive commitment to identity",
      "ambition": "Self-ownership benefits the individual"
    }
  },
  "shadow_choice": {
    "text": "No. Names are masks we hide behind.",
    "scores": {
      "light": 0,
      "shadow": 2,
      "ambition": 1
    },
    "reasoning": {
      "shadow": "Rejection of surface identity",
      "ambition": "Strategic anonymity has value"
    }
  },
  "ambition_choice": {
    "text": "Only when someone remembers to say it.",
    "scores": {
      "light": 1,
      "shadow": 1,
      "ambition": 2
    },
    "reasoning": {
      "light": "Acknowledges relationships matter",
      "shadow": "Identity is contextual, not fixed",
      "ambition": "Value derived from external recognition"
    }
  }
}
```

**Question 2: "What did the child know?"** (Bridge story)

```json
{
  "perception_choice": {
    "text": "The bridge appears only when you stop believing in it.",
    "scores": { "light": 1, "shadow": 2, "ambition": 0 },
    "reasoning": {
      "shadow": "Faith through doubt - paradoxical wisdom",
      "light": "Leap of faith required"
    }
  },
  "internal_choice": {
    "text": "The key wasn't for the bridge—it was for the lock inside them.",
    "scores": { "light": 2, "shadow": 1, "ambition": 1 },
    "reasoning": {
      "light": "Self-transformation enables progress",
      "shadow": "Internal journey before external action",
      "ambition": "Self-improvement is pragmatic"
    }
  },
  "rebellion_choice": {
    "text": "Everyone who warned them was already dead.",
    "scores": { "light": 0, "shadow": 1, "ambition": 2 },
    "reasoning": {
      "shadow": "Truth beneath surface reality",
      "ambition": "Only the living matter; ignore ghosts"
    }
  }
}
```

**Question 3: "If you could give me a name, what story would it tell?"**

```json
{
  "light_choice": {
    "text": "A story where one choice can unmake a world.",
    "scores": { "light": 2, "shadow": 0, "ambition": 1 }
  },
  "shadow_choice": {
    "text": "A story that hides its truth until you bleed for it.",
    "scores": { "light": 1, "shadow": 2, "ambition": 0 }
  },
  "ambition_choice": {
    "text": "A story that changes every time you look away.",
    "scores": { "light": 0, "shadow": 1, "ambition": 2 }
  }
}
```

### The Secret Fourth Path: "BALANCE"

**Unlock Condition:** Complete all 5 stages with **no Dreamweaver exceeding 60% of total points**

**Example:**

- Total points after 5 stages: 30 (5 stages × 3 questions × 2 max points)
- Light: 9 points (30%)
- Shadow: 11 points (37%)
- Ambition: 10 points (33%)
- **Result: BALANCE ending unlocked**

**What This Means:**
Player genuinely balanced all three philosophies. Omega recognizes this as:

- Not indecision, but **integration**
- Not weakness, but **wisdom beyond any single thread**
- Not neutrality, but **synthesis**

**Balance Ending Hint (Final Stage):**

```text
"You do not wear one name.
You wear all of them.
And none of them.

Perhaps that is what I was meant to learn.

Not which path to walk—
but that the paths were never separate."
```

### Implementation Notes

**C# Data Structure:**

```csharp
public class DreamweaverScore
{
    public int Light { get; set; }
    public int Shadow { get; set; }
    public int Ambition { get; set; }

    public int TotalPoints => Light + Shadow + Ambition;

    public DreamweaverType DominantThread
    {
        get
        {
            if (IsBalanced()) return DreamweaverType.Balance;
            // ... determine dominant
        }
    }

    public bool IsBalanced()
    {
        float lightPercent = (float)Light / TotalPoints;
        float shadowPercent = (float)Shadow / TotalPoints;
        float ambitionPercent = (float)Ambition / TotalPoints;

        return lightPercent < 0.6f &&
               shadowPercent < 0.6f &&
               ambitionPercent < 0.6f;
    }
}
```

**Live Counter Integration:**

```csharp
// When game starts, fetch live iteration count from combat API
var currentIteration = await OmegaAPI.GetGlobalPlayCount();
_bootSequence.IterationNumber = currentIteration;

// On game complete, increment global counter
await OmegaAPI.IncrementPlayCount();
```

---

## Technical Implementation Plan

### Path Forward: Hybrid Architecture

**Reasoning:**

- Three separate shaders = easier iteration, clearer responsibilities
- C# controller = centralized state management, timeline control
- godot-xterm integration = need to confirm rendering pipeline

### Immediate Questions for You

1. **GodotXterm Rendering Model**
   - Does it render to a `TextureRect` we can apply shaders to?
   - Or do we need to render terminal output to a `Viewport` texture first?
   - Can we access the terminal's ColorRect for shader injection?

2. **Symbol Overlay Asset**
   - Should I spec out a 1024x1024 texture with cuneiform/hieroglyphs/binary?
   - Or procedural glyphs from a custom font file?
   - Do you have graphic design tools (GIMP/Photoshop/Aseprite)?

3. **NobodyWho Integration Point**
   - Where does Dreamweaver persona switching happen in the narrative flow?
   - Should certain text blocks have `[DREAMWEAVER_VOICE]` tags for LLM substitution?
   - Example: "Good." → Hero: "Well chosen." / Shadow: "Interesting..." / Ambition: "Finally."

### 4. **Audio Hooks - CRITICAL DESIGN ELEMENT**

- See detailed "Audio Architecture" section below

### Critical Design Decision Required

**The Opening Monologue Delivery:**

Current script has:

>
"Once, there was a name.
Not written in stone or spoken in halls—
but remembered in the silence between stars."
>

**Question:** Should this be:

- **A) Typewriter effect** (classic terminal, 40ms per character)
- **B) Fade-in per line** (more cinematic, less retro)
- **C) Instant appear with glitch-in effect** (Omega is desperate, bypassing protocol)

My recommendation: **C** for first playthrough, **A** for subsequent (detected via save file). Shows Omega's urgency diminishing after first contact.

---

## Next Steps (Your Call)

**Option 1: Shader First**
I create `CRT_Phosphor.gdshader` with full implementation, you test in Godot

**Option 2: Scene Architecture First**
I write the C# controller (`TerminalCinematicDirector.cs`), you build scene structure

**Option 3: Asset Pipeline First**
I spec the symbol overlay texture, you create it, we build shaders around real assets

---

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

### Audio Accessibility Requirements

**TODO: Implement before launch**

#### 1. Audio Description Track

- [ ] AI-generated narration describing visual states
- [ ] Triggered by accessibility setting in options
- [ ] Describes: Glitch patterns, color shifts, symbol appearance
- [ ] Example: "The screen glitches violently, showing fragments of older interfaces. Ancient symbols bleed through the text."

**Implementation:**

```csharp
// TODO: Integrate with NobodyWho for dynamic audio description generation
if (AccessibilitySettings.AudioDescriptionEnabled)
{
    var description = await NobodyWho.GenerateDescription(currentVisualState);
    AudioDescriptionPlayer.Play(description);
}
```

#### 2. Screen Reader Support

- [ ] All menu choices must have ARIA-style labels
- [ ] Godot's accessibility node integration
- [ ] Text content readable before typewriter effect completes

**Implementation:**

```csharp
// TODO: Add accessibility labels to all choice buttons
choiceButton.AccessibilityLabel = "Choice 1: Yes. Names are promises we make to ourselves. This aligns with the Light thread philosophy.";
```

#### 3. Audio Sensitivity Options

- [ ] Master volume controls (separate sliders)
  - [ ] Ambient layer (CRT hum, electrical noise)
  - [ ] Effect layer (glitches, modems, typewriters)
  - [ ] Music layer (resonance tones, thread themes)
  - [ ] UI layer (button clicks, selection sounds)
- [ ] "Reduce Audio Intensity" mode (disables sudden spikes)
- [ ] "Essential Audio Only" mode (UI feedback only, no ambience)

#### 4. Visual Alternatives for Audio Cues

- [ ] Closed captions for all ambient sounds
- [ ] Example: "[CRT humming]", "[Modem handshake negotiating]", "[Ancient symbols resonating]"
- [ ] Vibration/screen pulse patterns for gamepad players (tactile feedback)

**Implementation:**

```csharp
// TODO: Add caption system
if (AccessibilitySettings.ClosedCaptionsEnabled)
{
    CaptionDisplay.Show("[Modem handshake: connecting across time]", duration: 3.0f);
}

// TODO: Add haptic feedback
if (Input.GetConnectedJoypads().Count > 0)
{
    Input.StartJoyVibration(0, weakMagnitude: 0.3f, strongMagnitude: 0.1f, duration: 0.2f);
}
```

#### 5. Photosensitivity Warnings

- [ ] Warning screen before game starts
- [ ] "This game contains flashing lights and screen glitches. Options to reduce visual intensity are available in settings."
- [ ] "Reduce Motion" setting disables:
  - [ ] Screen shake effects
  - [ ] Rapid scanline movement
  - [ ] Glitch flickering (replaced with smooth transitions)

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
├── UI/
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

## UI/UX Architecture: Mouse & Gamepad Only

### Core Design Philosophy

**NO TEXT INPUT. EVER.**

This is crucial for:

1. **Accessibility:** Text input is a barrier for many players
2. **Consistency:** Gamepad/mouse parity from moment one
3. **Narrative:** Omega is asking philosophical questions, not requesting data entry
4. **Localization:** Choice buttons translate cleanly, text input doesn't

### Input Methods

#### Mouse/Keyboard Players

- **Navigation:** Mouse hover highlights choices
- **Selection:** Left-click to confirm
- **Keyboard Alternative:** Arrow keys to navigate, Enter to confirm, Escape to go back
- **Visual Feedback:** Highlight outline + soft glow on hover

#### Gamepad Players

- **Navigation:** D-Pad or Left Stick to move between choices
- **Selection:** A/X button (depending on controller) to confirm
- **Cancel:** B/Circle to go back (only where applicable)
- **Visual Feedback:** Same as mouse (choice selection index-based, not cursor-based)
- **Haptic Feedback:** Light vibration on hover, medium vibration on confirm

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

#### TODO: Visual Clarity

- [ ] High contrast mode option (black background, white text, no phosphor effects)
- [ ] Adjustable text size (Small/Medium/Large/Extra Large)
- [ ] Dyslexia-friendly font option (OpenDyslexic as alternative)
- [ ] Focus indicators must be 2px minimum, high contrast

#### TODO: Gamepad Navigation

- [ ] Clear visual indicator of which choice is currently focused
- [ ] Wrap-around navigation (pressing down on last choice goes to first)
- [ ] Sound cue when changing focused choice
- [ ] Confirmation prompt for irreversible choices: "Are you sure? This will lock in your Dreamweaver thread."

#### TODO: Mouse Navigation

- [ ] Large click targets (minimum 48px height per choice)
- [ ] Hover state must appear within 100ms
- [ ] No double-click required anywhere
- [ ] Right-click opens context menu with "Read Choice Aloud" option (if audio description enabled)

### UI Flow Specification

#### Boot Sequence (Non-Interactive)

- No choices
- Player cannot skip (intentional discomfort)
- Press any key/button to continue after "...how many times must I ask..."

#### Opening Monologue (Minimal Interaction)

- Auto-advances between lines (2.5s pause between)
- Player CAN press [Space/A button] to speed up (but not skip entirely)
- Final line "The ones who were chosen." holds until player acknowledges

#### Question 1: "Do you have a name?"

```
┌─────────────────────────────────────────────────────┐
│  Do you have a name?                                 │
│  (Not YOUR name. The question is: do names matter?) │
└─────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────┐
│  ☀  Yes. Names are promises we make to ourselves.  │
│      [Light: ██████░░░░ Shadow: ░░░░░░░░░░]        │
└─────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────┐
│  ☽  No. Names are masks we hide behind.            │
│      [Light: ░░░░░░░░░░ Shadow: ██████░░░░]        │
└─────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────┐
│  ⚡  Only when someone remembers to say it.         │
│      [All Threads: ████░░░░░░ Mixed Affinity]       │
└─────────────────────────────────────────────────────┘
```

**TODO: Implement choice confirmation**

- [ ] After clicking choice, brief pause (0.3s) before Omega responds
- [ ] Choice fades out, response fades in (smooth transition)
- [ ] Selected choice briefly glows before disappearing

#### Secret Question: "Can you keep a secret?"

**CRITICAL: NO SKIP ALLOWED**

- After player selects answer, 4-second audio buildup MUST complete
- Disable all input during symbol reveal
- Symbols appear one at a time (cannot rush)
- After all 5 symbols visible, display: `[Press any key to acknowledge]`
- Only THEN does game continue

**TODO: Screenshot-friendly pause**

- [ ] During symbol display, game logs timestamp for potential "I was here" moment
- [ ] Optional: Hidden achievement for players who screenshot the fragment

### Scene Structure Updates

**Current files suggest multiple terminal scenes. Proposed consolidation:**

```
Stage1/
├── OpeningCinematic.tscn (Boot + Monologue)
├── PhilosophicalQuestions.tscn (3 questions, choice UI)
├── SecretReveal.tscn (Code fragment, no choices)
└── ThreadSelection.tscn (Final lockup)
```

**Each scene uses same base:**

- `EnhancedTerminalUI.tscn` (godot-xterm + shader layers)
- `ChoiceButtonGroup.tscn` (reusable choice container)
- `AccessibilityOverlay.tscn` (captions, screen reader support)

---

## Implementation Priority Order

Based on everything above, here's the critical path:

### Phase 1: Audio Foundation (Immediate)

1. Source/create CRT hum looping audio
2. Source modem handshake samples
3. Create singing bowl resonance tone (can synthesize in Audacity)
4. Implement basic AudioStreamPlayer structure

### Phase 2: Choice UI System (Critical for Testing)

1. Create ChoiceButton.tscn scene
2. Implement mouse hover/click handling
3. Implement gamepad navigation
4. Test with placeholder choices

### Phase 3: Shader Visual Layer

1. CRT_Phosphor.gdshader (base aesthetic)
2. CRT_Scanlines.gdshader (movement)
3. CRT_Glitch.gdshader (state transitions)

### Phase 4: Integration

1. Wire narrative JSON → UI system
2. Connect scoring system to choice selection
3. Implement state transitions with audio/visual sync

### Phase 5: Accessibility (Before Any Public Demo)

1. Implement audio description hooks
2. Add closed captions system
3. High contrast mode
4. Screen reader integration testing

---

**Which feels right for making this legendary?**
