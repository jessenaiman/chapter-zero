# Dialogic Character Integration for Omega Spiral

## Overview
This refactor demonstrates how the 3 Dreamweaver characters work within Dialogic's timeline system, enabling LLM-generated dialogue that maintains character personality and thread consistency.

## Character Files Created

### `/characters/light.dch` - Light Dreamweaver
- **Color**: Golden/amber (`Color(1, 0.9, 0.5, 1)`)
- **Personality**: Bold risk-taker, believes in absolute good, takes definitive action
- **Custom Info**: Stores thread type and personality traits for LLM context

### `/characters/shadow.dch` - Shadow Dreamweaver  
- **Color**: Violet (`Color(0.6, 0.3, 0.8, 1)`)
- **Personality**: Non-interventionist, values tradition and patience, nostalgic
- **Custom Info**: Stores thread type and personality traits for LLM context

### `/characters/ambition.dch` - Ambition Dreamweaver
- **Color**: Crimson (`Color(1, 0.2, 0.05, 1)`)
- **Personality**: Self-interest as moral framework, transactional but not cruel
- **Custom Info**: Stores thread type and personality traits for LLM context

### `/characters/omega.dch` - System/Interface
- **Color**: Terminal amber (`Color(0.99, 0.79, 0.38, 1)`)
- **Personality**: The interface itself, weary but persistent, iteration 472

## Timeline Syntax Changes

### Before (No Characters)
```dialogic
If you could hear only one story...
What would it be?
```

### After (With Characters)
```dialogic
SYSTEM: If you could hear only one story...
SYSTEM: What would it be?
```

### Character Dialogue Pattern
```dialogic
[Character Name]: [Dialogue text with BBCode formatting]
```

**Examples:**
```dialogic
Light: A journey... far into the unknown.
Shadow: An enigma... riddles and hidden truths.
Ambition: A legend... heroes rising, worlds changing.
SYSTEM: > Choice logged: LIGHT affinity detected
```

## How LLM Generation Works

### 1. Character Context Injection
When generating dialogue, pass the character's `custom_info` as context:

```python
# Pseudocode for LLM generation
character_data = load_character("light.dch")
personality = character_data["custom_info"]["personality"]
thread = character_data["custom_info"]["thread"]

prompt = f"""
You are the {thread} Dreamweaver with this personality:
{personality}

Generate dialogue for this scenario:
Player chose: fantasy story (light affinity)
Scene: Introduction to the spiral journey
Tone: Warning but inviting
Previous context: Player selected their story preference

Respond in 2-3 short lines that:
- Match the character's personality
- Reference the player's choice
- Hint at consequences without explaining
- Use poetic/mysterious phrasing
"""

# LLM generates:
# "A journey... far into the unknown."
# "But some paths lead to places you can't return from."
# "Are you ready for the road that never ends?"
```

### 2. Rewritable Block Markers
Use labels to mark sections for LLM regeneration:

```dialogic
label scene_003
SYSTEM: In that story, who are you?
- The seeker— searching for truth [if {player_thread} == "light"]
    set {player_role} = "seeker"
    [wait time="0.3"]
    # BEGIN_LLM_BLOCK: light_role_response
    Light: The seeker... searching for choice.
    Light: But some choices bind you tighter than freedom allows.
    Light: Are you prepared to seek what finds you instead?
    # END_LLM_BLOCK: light_role_response
    [wait time="0.5"]
    SYSTEM: > Role archived: SEEKER
    => scene_004
```

External AI can parse blocks between `BEGIN_LLM_BLOCK` and `END_LLM_BLOCK` comments and regenerate them while preserving structure.

### 3. Variable-Driven Responses
Characters can reference player choices dynamically:

```dialogic
SYSTEM: Thread alignment: {player_thread}
SYSTEM: Role designation: {player_role}

[if {player_thread} == "light"]
    Light: I'm coming with you. Don't lose me this time.
[end_if]
```

LLM can generate variations based on accumulated variables:
```python
context = {
    "player_thread": "light",
    "player_role": "seeker", 
    "player_story_choice": "fantasy",
    "player_name_view": "promises"
}

# Generate finale dialogue that references all choices
```

## Advantages of This Approach

### ✅ Character Consistency
- Each Dreamweaver has distinct voice via character file
- Color coding provides visual identity
- Personality traits ensure LLM stays in character

### ✅ Modular Dialogue Generation
- Each character's lines can be regenerated independently
- Block markers make it clear what's LLM-editable vs. structural
- Variables track player state for context-aware responses

### ✅ Post-Game Narrative Generation
After gameplay, export the player's path:
```python
player_choices = {
    "thread": Dialogic.VAR.get("player_thread"),
    "role": Dialogic.VAR.get("player_role"),
    "story_choice": Dialogic.VAR.get("player_story_choice"),
    "name_view": Dialogic.VAR.get("player_name_view"),
    "name_story": Dialogic.VAR.get("player_name_story")
}

# Send to external AI:
# "Generate what Shadow would have said if they were the primary thread"
# "Generate what Ambition would have interpreted from these events"
```

### ✅ Single Timeline, Multiple Perspectives
- One linear playthrough for the player
- Character system enables distinct voices
- Conditionals filter appropriate responses per thread
- Post-game AI generates alternate thread narrations

## Testing the Refactored Timeline

1. **Load in Godot Editor**: Open Dialogic editor, navigate to timeline
2. **Preview**: Use Dialogic's preview feature to test branching
3. **Verify Variables**: Check that `{player_thread}`, `{player_role}`, etc. are set correctly
4. **Test Conditionals**: Ensure only the selected thread's character speaks
5. **Export Test**: Confirm bridge script captures all variables

## Next Steps

1. Replace `ghost_terminal.dtl` with `ghost_terminal_refactored.dtl`
2. Test in-game to verify character colors and dialogue display
3. Create LLM prompt templates for each character
4. Build external tool to parse/regenerate LLM blocks
5. Expand to other levels (Nethack, Town, etc.)

## Example LLM Prompt Template

```markdown
# Character: {character_name}
# Thread: {thread_type}
# Personality: {personality_traits}

## Context
- Player Choice: {player_choice}
- Previous Dialogue: {previous_lines}
- Current Scene: {scene_description}

## Task
Generate 2-3 lines of dialogue that:
1. React to the player's choice
2. Hint at consequences without being explicit
3. Match the character's personality and speaking style
4. Use poetic/mysterious phrasing appropriate for a CRT terminal aesthetic
5. End with a question or reflection that deepens the mystery

## Output Format
Return only the dialogue lines, one per line, no character names (they're added by the system).
```

---

This structure allows external AI to generate character-specific dialogue while maintaining the single-timeline gameplay experience. Post-game, the same system can generate "what if" narratives from the other Dreamweavers' perspectives.
