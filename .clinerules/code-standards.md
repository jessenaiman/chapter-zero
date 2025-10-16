# C# Documentation Rules

## [Code Standards](./../coding-conventions.instructions.md)

## Tools USE THEM FIRST

**NEVER MOVE ON TO ANOTHER FILE UNTIL THE `PROBLEMS` tab IS CLEAN**
**ALWAYS CHECK THE `TERMINAL` OUTPUT FOR WARNINGS AND ERRORS AND DISCUSS FIXES**

Use your damn tools, don't make the user waste tokens telling you about issues that could have been found with tools.

## Rules

FIX EVERYTHING

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
**MUST USE SERANA TO FIND PROJECT MEMORIES**
**MUST USE SERANA ONBOARD MCP TOOL FIRST**

---

- **use the `problems` tool to see the problems in the file and ide**
- **use the `test` tool to run tests and see failures**
- **MUST ENFORCE XML Documentation Comments**

## C# Documentation [Rules]

**Read all [Agents Rules](./../../AGENTS.md)**

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
