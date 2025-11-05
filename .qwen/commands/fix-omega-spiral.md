# Fix Omega Spiral project structure and configuration

Identify what needs to fix in the Godot project configuration to ensure proper integration of the open-rpg and game-template projects.

Examine:
1. file structure differences between omega-spiral, open-rpg, and game-template.
2. dependencies and references in the Godot project files.

## First step:
- For godot-game-template: "Call the mcp_godot-game-te_list_directory tool with path='/' to list the root directory of the godot-game-template repository."
- For godot-open-rpg: The available actions do not include a list_directory tool for mcp_godot-open-rp (only create_directory, move_file, and read_media_file are listed). If directory listing is required, add: 

- "First call activate_directory_navigation_tools to enable directory navigation for allowed directories, then use the resulting list_directory tool with path='/' for the godot-open-rpg repository. If this fails, note that the tool should be be configured for read access. DO NOT MODIFY ANY EXTERNAL REFERENCE PROJECTS"
**Do not read any local workspace files (including maaacks_game_template or source/**) until this succeeds.**If you use any other tool there will be no follow up the chat will terminate automatically.**

### Next Steps After Directory Listing:
1. List help files in game-template with full paths. Explain their purpose and show, with links, where omega-spiral diverges. Pause for user confirmation before continuing.
2. Repeat for godot-open-rpg: list help files, explain, and link divergences. Pause for confirmation.
3. For each divergence, create a numbered list: state file path, link to expected structure, describe the required change. Wait for user approval before proceeding.
4. Create an actionable plan in chat, with file path and link, how to add a start level 3 button to the main menu to load open-rpg’s level 3. Do not implement until confirmed.
5. Identify sound system files in game-template, link them, and list exact integration steps for open-rpg and omega-spiral. Wait for user confirmation before any changes.

### Tasks :
- In Omega Spiral, the open-rpg project is being refactored into C#.
- Level 3 must replicate the gameplay of open-rpg exactly.
- The game-template provides the architectural foundation for open-rpg, including proper sound system integration as a key step.
- Level 1 must start from the game-template's level 1 file at this point, and it should play the basic game as expected from the template.
- Assume all current logic and configuration in the project is incorrect and requires revision.
- Only suggest changes to project.godot in the chat; require user confirmation and manual application via the Godot editor.

### RULES:
- Your task is to ensure that the project structure follows the conventions set by the game-template and open-rpg projects. If C# code has been created where theme or other godot files needed to be then suggest replacing them; do not replace them unless you get confirmation.
- Custom levels are in the `/levels` directory and should not be used until
- project.godot changes are suggested by you and made by the user and should be made through the godot editor only.
- You must always demonstrate in chat where the files changed match the expected structure of the integrated projects with a link; do not provide summaries or lengthy explanations.

Your first step is to verify the that you have access to the directories and can read the files in these projects through `godot-open-rpg` and `godot-game-template` both are mcp tools with readonly access if they are configured correctly