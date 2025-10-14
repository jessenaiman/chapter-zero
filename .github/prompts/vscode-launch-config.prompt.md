---
mode: 'agent'
description: 'workflow for error free code with codacy'
tools: ['edit', 'search', 'runCommands', 'runTasks', 'context7/*', 'godot/*', 'serena/*', 'usages', 'vscodeAPI', 'problems', 'changes', 'testFailure', 'fetch', 'github.vscode-pull-request-github/copilotCodingAgent', 'extensions', 'todos', 'runTests']
---

## Serana Tool Use

Do this once per session:

`onboarding`: Call this tool if onboarding was not performed yet.
You will call this tool at most once per conversation. Returns instructions on how to create the onboarding information.

*This prompt is being run because one or many of these are true:*

- the vscode launch configuration is missing or incomplete.
- the ide is not showing problems in the tab that correspond with build errors and warnings
- tests

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

still showing as not run or not discovered:

`OUTPUT` tab in vscode:

```bash
Finding projects for pattern /home/adam/Dev/omega-spiral/chapter-zero/
Found 1 matches for pattern in folder /home/adam/Dev/omega-spiral/chapter-zero
Evaluating match /home/adam/Dev/omega-spiral/chapter-zero/
Adding directory /home/adam/Dev/omega-spiral/chapter-zero/
Executing dotnet test -t -v=q in /home/adam/Dev/omega-spiral/chapter-zero/
Finding projects for pattern /home/adam/Dev/omega-spiral/chapter-zero/OmegaSpiral.csproj
Found 1 matches for pattern in folder /home/adam/Dev/omega-spiral/chapter-zero
Evaluating match /home/adam/Dev/omega-spiral/chapter-zero/OmegaSpiral.csproj
Adding directory /home/adam/Dev/omega-spiral/chapter-zero
Executing dotnet test -t -v=q in /home/adam/Dev/omega-spiral/chapter-zero
[ERROR] Error while executing dotnet test -t -v=q - MSBUILD : error MSB1011: Specify which project or solution file to use because this folder contains more than one project or solution file.

[ERROR] Error while executing dotnet test -t -v=q - MSBUILD : error MSB1011: Specify which project or solution file to use because this folder contains more than one project or solution file.
```
