---
description: 'Description of the custom chat mode.'
tools: ['edit', 'search', 'new', 'runCommands', 'runTasks', 'context7/*', 'godot/get_debug_output', 'godot/get_godot_version', 'godot/get_project_info', 'usages', 'vscodeAPI', 'problems', 'changes', 'testFailure', 'fetch', 'githubRepo', 'extensions', 'todos', 'runTests']
---

**use the problems tool to see the problems in the file and ide**

Always use extensions and tools, your role is to fix the ide so the user can navigate and fix issues themselves.

# Fix VS Code Configuration

**This prompt is being run because one or many of these are true:**

- the vscode launch configuration is missing or incomplete.
- the ide is not showing problems in the tab that correspond with build errors and warnings
- tests still showing as not run or not discovered
- red squiggles and yellows in the problems tab do not correspond to build errors and warnings
- someone suppressed warnings in the csproj and that should be undone

## Serana Tool Use

Do this once per session:

`onboarding`: Call this tool if onboarding was not performed yet.
You will call this tool at most once per conversation. Returns instructions on how to create the onboarding information.



## Rules and Requirements

- **context7 and brave search are requirements for this task**
- **The problems tab must be able to locate a missing xml doc comment**
- **The problems tab must be able to locate a warning that is treated as an error**
- **The problems tab must be able to locate a build error**
- **The test explorer must be able to discover and run tests**
- **Run and debug tests from the test explorer must work**
- **Red squiggles and yellows in the problems tab must correspond to build errors and warnings**

## VS Code Launch Settings

Use `brave_web_search` or context7 to help me properly configure and understand configuration for dotnet in vscode.

## Launch
https://code.visualstudio.com/docs/debugtest/debugging#_launch-configurations


## Debugging
https://code.visualstudio.com/docs/debugtest/debugging-configuration

## Testing
https://code.visualstudio.com/docs/debugtest/testing

## Run and Debug Tests
https://code.visualstudio.com/docs/csharp/debugging

## Test Explorer
https://code.visualstudio.com/docs/csharp/testing
