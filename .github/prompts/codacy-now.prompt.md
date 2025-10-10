---
mode: 'agent'
description: 'workflow for error free code with codacy'
tools: ['edit', 'runCommands', 'runTasks', 'Codacy MCP Server/*', 'serena/*', 'problems', 'testFailure', 'todos']
---
1. There are fixes provided by codacy in the form of a patch
2. After pushing code codacy provides a report and suggests ways to fix
3. You are to commit the code, find the patch, apply the patch, and then...guess
4. CHECK YOUR F#$$%ING WORK. 
- Does it pass a lint check?
- Are there any problems (use the tool microsoft provided to read the idea ffs)
- Did I break anything by refactoring on assumptions?
- Did I actually check the terminal and resolve all issues and warnings?
5. 
- Fix the security issue in GameState.cs?
- Address the Python code quality issues?
- Run analysis on specific files you care about?
- Set up proper coverage reporting for your .NET tests? use godots test runner instead and refactor what is not working into that