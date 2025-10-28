# Ghost Terminal Script - Creative Review

## What's Working Well ✓

1. **Boot Sequence (Moment 0)** - Clean iteration/system message opening
   - Technical tone appropriate for terminal interface
   - "...how many times must I ask before one of you finally answers..." - good existential hook

2. **Dreamweaver Introduction (Moments 1-2)** - Strong philosophical voice
   - "Once, there was a name" opening is excellent
   - "Not written in stone or spoken in halls" - poetic, mysterious
   - Warning/greeting arc (approach then caution) works well

3. **Three-Question Structure** - Clear escalation
   - Q1: Identity (Do you have a name?)
   - Q2: Knowledge (What did the child know?)
   - Q3: Essence (If you could give me a name, what story would it tell?)

4. **Code Fragment Revelation (Moment 6)** - Strong mystery moment
   - Symbol revelation (∞ ◊ Ω ≋ ※) is effective
   - "First fragment of five" creates meta-narrative drive

## Critical Issues to Find in Original ✗

### 1. **"Stone/Halls" Reference (Line 20-21)**
- Current: "Not written in stone or spoken in halls--"
- **Problem**: This was added as embellishment; contradicts your vision
- **Original likely**: Simpler, more direct opening about the name
- **Action**: Look for version that just introduces Omega's identity/nature

### 2. **Parable Overload (Moments 4-5)**
- **Problem**: Too many nested parables/stories (child → bridge → key → city)
- Multiple layers of metaphor may obscure the core questions
- **Current flow**: Setup → Q1 → Parable Q2 → Parable Q3
- **Original likely**: More direct philosophical questions, fewer fantasy elements
- **Action**: Find version with cleaner narrative structure

### 3. **"The ones who were chosen" (Line 41)**
- **Problem**: Adds player-selected-by-fate framing not established elsewhere
- **Original likely**: Simpler warning without fate implications
- **Action**: Look for version that's more "be careful in this system"

### 4. **Question Framing Issues**

#### Q1: "Do you have a name?" (Line 53)
- **Problem**: Context says "(Not YOUR name. The question is: do names matter?)"
- This explicitly tells player the answer doesn't matter
- **Original likely**: Doesn't spoil the question's intent
- **Action**: Find version where question feels genuine

#### Q2: "What did the child know?" (Line 98)
- **Problem**: Abstract question about a character, not about player
- Player is being asked to interpret a story, not reveal something about themselves
- **Original likely**: Question directed at player's philosophy/nature
- **Action**: Look for version that's more self-referential

#### Q3: "If you could give me a name, what story would it tell?" (Line 189)
- **Problem**: Questions Omega's essence through player's interpretation
- Good question conceptually, but context awkward
- **Issue**: Comes after already determining dominant thread; redundant?
- **Action**: Verify this is after thread selection in original

### 5. **Thread/Dreamweaver Semantics**
- Current: 3 threads (light, shadow, ambition)
- **Problem**: No clear narrative reason given for why these 3
- **Original likely**: Explains Omega's nature in terms of these 3 aspects
- **Action**: Find version that establishes thread mythology

### 6. **Response Fields Not Used**
- Current: Response text added to Moment 5 options (e.g., "Good. Because this is all...")
- **Problem**: GhostUi doesn't use response field - just ignored
- **Original likely**: Responses integrated into continuation narrative instead
- **Action**: Check if responses belong or should be in continuation

### 7. **Final Question (Moment 8) Placement**
- **Problem**: Asks player to name Omega AFTER already selecting dominant thread
- Feels backwards - asking for essence/name after already choosing?
- **Original likely**: Questions in different order OR Q3 is just the thread selection
- **Action**: Check if Q3 should be the final step (no Q4)

### 8. **Code Fragment Context Missing**
- Current: Moment 6 appears without narrative setup
- Just suddenly appears after Q2
- **Original likely**: Integrated into narrative flow (maybe after Q1 or Q2 choice?)
- **Action**: Find if fragment placement was different

## Structural Issues

### Flow Problem
```
CURRENT:
M0: Boot
M1-2: Intro to Omega
M3: Q1 (Names)          <- Direct question
M4-5: Q2 (Bridge)       <- Story-wrapped question
M6: Code Fragment       <- Discovery
M7: Q3 (What story?)    <- Direct question
M8: Final (closure)     <- "Until you unmake it"
```

**Issue**: Questions feel like setup → story problem → back to direct questions

### Missing Elements
- **Omega's Self-Awareness**: Why does Omega ask these questions?
- **Stakes/Consequence**: What happens with player's answers?
- **Thread Explanation**: Why light/shadow/ambition specifically?
- **Journey Arc**: Is this opening a game? Warning? Test?

## Recommendations for Finding Original

1. **Search for**: Files named with "Jason" in the path
2. **Look in**: `/docs/stages/` or `/docs/` for design documents
3. **Search for**: "ghost_terminal.yaml" or "ghost.md" in git history
4. **Key phrases to search**:
   - "questions three" or "three questions"
   - "Dreamweaver" or "thread"
   - "∞ ◊ Ω ≋ ※" (symbols)
   - "847,294" (iteration count)

5. **Questions to answer in original**:
   - What's the stage's narrative purpose? (Is it a test? A story? A warning?)
   - How many questions are there really? (3? 4? Just 1?)
   - Are parables intentional or added complexity?
   - What's the relationship between player choices and thread selection?

## Next Steps

1. Find original ghost terminal script
2. Compare against current version
3. Remove embellishments (parables, metaphors, poetry)
4. Keep: Boot, Omega intro, core questions, thread selection, code fragment reveal
5. Rebuild with clean narrative flow
