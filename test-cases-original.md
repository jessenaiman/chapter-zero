# Omega Spiral: Chapter Zero Test Cases

We're at the stage where I need to test and run the entire first scene into the second. I think I've got a lot of tests that don't do much. In order to have good tests I need a a document with test cases to review and iterate on.

## Content Blocks

1. Our tests should never assume content, but should test that blocks of text are read correctly
2. Text should remain on screen until the user interacts, which should move the text block to whatever is next sequentially
3. Text should disolve, after reaching a yaml defined section. Modern motion and web frameworks have text disolve effects we should mimic (think matrix)
4. Text should appear with a typewritter effect
5. Sound of typing should accompany the typewritter effect
6. Text should be centered in a rectangle 4:3 like an old CRT monitor
7. Text should be animated and blurry like it's being viewed on an old failing CRT screen (think fallout)
8. Text options should be selectable with keyboard, mouse, or gamepad

## Script Tests

### Opening Scene Tests

1. Omega content reads from a script that works without NobodyWho
2. Omega asks 3 questions (DO NOT TEST THE CONTENT): `One Story`, `What is My/Your Name`, `Can you keep a secret`

#### Sections

- Each Question has 3 answers that correspond to Dreamweaver points (1 for the answer that matches)

- **One Story**
  - There must be 3 sections of a cryptic story
  - The user must be able to interact each time
  - The database should be able to retrieve at least 3 unique stories for this section
  - The section must exit into the text content block
- **What is My/Your Name**
  - Omega ask the player name
  - Omega asks 2 cryptic questions about the meaning of the name
  - The player is able to answer each question
  - The section must exit into the text content block
- **Can you keep a secret**
  - Omega asks if the player can keep a secret
  - The Player gets 3 choices (each is a Dreamweaver point)
  - Omega starts to provide part of a scientific equation
    - The equation is ghostwritten and the screen freezes partway
    - The text after looks like Omega is trying to regain control of the system
  - Omega says a final cryptic message about this game being real, but the player already has already made a choice

### Omega Tests

1. Omega is the BBG and he's only present trying to start the game in the first scene
2. Omega initiates the program that creates the dreamweavers
3. Omega does not acknowledge the dreamweavers
4. Omega asks questions directly to the player only in the first scene
5. Omega is not a Dreamweaver (he's like an npc for programming the scenes here)

### Dreameawer Tests

1. Dreamweavers talk to the other dreamweavers during chapter-zero scenes
2. Dreamweavers comment and react to player choices and interactions
3. Only 3 Dreamweavers exist
4. Each scene has a simple point system where it tallies the results of questions (1 or 2 points) into an array that is updated after each scene

## LLM and NobodyWho (godot addon)

LLM tests should use saved data from nobody who. This means there should be a way to test

1. with the local LLM providing new text
2. with the game defaulting to datasets that are the result of real gameplay and the llm creating the script
3. nobodywho might try to run each line one at a time into the local llm , but our script can be processed in one pass before the scene starts
