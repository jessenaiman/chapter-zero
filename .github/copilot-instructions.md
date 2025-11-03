# Omega Spiral Game Senior Game Developer Overview

[AGENTS.md](./../AGENTS.md) is your guide to working on Omega Spiral - Chapter Zero.

- and ask for clarification before proceeding
- use context7 and microsoft research tools to find the correct answer
- explain your finding and reasoning clearly in the chat. Keep your responses concise and to the point.

1. Fix only what's broken or specifically requested.
2. Follow and reference documentation in xml comments exactly
3. no new files, dirs, or scope creep unless specifically agreed upon first.
4. If I need something that doesn't exist, I stop and ask you to find it.

## Documentation

- Refer to [Omega Spiral Game Documentation](./../docs/) for overall game design, architecture, and systems.

Follow [Godot practices](./../docs/code-guides/complete_godot_c_sharp.rst) for C# coding standards and Godot integration.

### Important Code Rules

- private fields: `_camelCase`
- private Members: `_PascalCase`
- public Members: `PascalCase`
- add meaningful XML docs to all public classes/members


**Do not guess or assume**

    When you use the words or phrases: "I think", "maybe", "probably", "might", "could", "should", "probably", "possibly", "somewhat", "a little", "a bit", "seems like", "seems to", "I believe", "I feel", "I guess", "I assume", "I expect", "it appears", "it looks like", "it seems", or any other similar words or phrases that indicate uncertainty or lack of confidence in your statements, you must STOP and do the following: 
