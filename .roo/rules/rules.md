# Omega Spiral Game Overview

**Omega Spiral** is an revolutionary and evolutionary narrative and turn based rpg game where players navigate through five distinct scenes, each representing a different era of gaming aesthetics. The game features dynamic AI-driven narrative personas (Dreamweavers) that adapt to player choices, creating emergent storytelling experiences.

## Engine Checklist

If you don't understand the codebase or project do not make changes until you do.

use the tools and work like a pair programmer carefully explaining in chat files to review. Do not assume, guess, or 'think', use the mcp tool context7 or ask direct questions.

1. What scenes already load?
2. What tests already run?
3. Did you check the project or build to understand the codebase?
4. Did you fix all issues, warnings, problems, broken tests?
5. Do you understand the game we're building
6. If the answer is no to any of these is no repeat steps 1-5 until the answer
7. I don't think so. Did you actually do steps 1-6? Check again

### Tools USE THEM FIRST

**NEVER MOVE ON TO ANOTHER FILE UNTIL THE `PROBLEMS` tab in vscode IS CLEAN**
**ALWAYS CHECK THE `TERMINAL` OUTPUT FOR WARNINGS AND ERRORS AND DISCUSS FIXES**

- Always read tool descriptions completely before using
- Don't make the user waste tokens telling you about issues that could have been found with tools.

## Documentation Rules

## [Code Standards](./../coding-conventions.instructions.md)

**FIX EVERYTHING**
- Present brief actionable next steps only
- Never justify incomplete work
- Demonstrate in chat and through test or terminal results success
- Never waste tokens and context on success stories
- Review your work and provide a constant updated prompt that can be used to reviewed the files and changes made:
  - brief single paragraph with file names
  - line numbers
  - reason and how it ties to the user request.
- Use TODO, and FIX, and similar tags where attention is required

**MUST ENFORCE XML Documentation Comments**
**MUST DOUBLE CHECK FOR ALL [CSHARP STYLE RULES](./../c_sharp_style_guide.md) and [CODING CONVENTIONS](./../coding-conventions.instructions.md)**
**DO NOT ADD NICE TO HAVE FEATURES**
**DO NOT PROVIDE SUCCESS STORIES**
**NEVER WRITE FALSE POSITIVE TESTS**
**NEVER HIDE ERRORS**
**ALWAYS CHECK THE TESTS AND BUILD AFTER EVERY NEW FILE AND CHANGE**
**USE CODACY TO CHECK FOR ISSUES AND FIXES**

## XML Documentation Rules

- Public members should be documented with XML comments.
- It is encouraged to document internal members as well, especially if they are complex or not self-explanatory.
- Use `<summary>` for method descriptions. This should be a brief overview of what the method does.
- Use `<param>` for method parameters.
- Use `<paramref>` to reference parameters in documentation.
- Use `<returns>` for method return values.
- Use `<exception>` to document exceptions thrown by methods.
- Use `<see langword>` for language-specific keywords like `null`, `true`, `false`, `int`, `bool`, etc.
- Use `<inheritdoc/>` to inherit documentation from base classes or interfaces.
  - Unless there is major behavior change, in which case you should document the differences.
- Use `<typeparam>` for type parameters in generic types or methods.
- Use `<typeparamref>` to reference type parameters in documentation.
- Use `<c>` for inline code snippets.
- Use `<code>` for code blocks. `<code>` tags should be placed within an `<example>` tag. Add the language of the code example using the `language` attribute, for example, `<code language="csharp">`.
