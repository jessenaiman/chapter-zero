# C# Documentation Rules

## [Code Standards](./../../coding-conventions.instructions.md)

**use the serana onboard mcp tool first**
**MUST USE SERANA TO FIND PROJECT MEMORIES**
**ALWAYS CHECK THE TESTS AND BUILD AFTER EVERY NEW FILE AND CHANGE**
**USE CODACY TO CHECK FOR ISSUES AND FIXES**
**NEVER MOVE ON TO ANOTHER FILE UNTIL THE BUILD IS CLEAN**
**MUST ENFORCE XML Documentation Comments**
**MUST DOUBLE CHECK FOR ALL RULES**

## XML Documentation Rules

- Public members should be documented with XML comments.
- It is encouraged to document internal members as well, especially if they are complex or not self-explanatory.
- Use `<summary>` for method descriptions. This should be a brief overview of what the method does.
- Use `<param>` for method parameters.
- Use `<paramref>` to reference parameters in documentation.
- Use `<returns>` for method return values.
- Use `<exception>` to document exceptions thrown by methods.
- Use `<see langword>` for language-specific keywords like `null`, `true`, `false`, `int`, `bool`, etc.
- Use `<see cref>` to reference other types or members inline (in a sentence).
- Use `<seealso>` for standalone (not in a sentence) references to other types or members in the "See also" section of the online docs.
- Use `<inheritdoc/>` to inherit documentation from base classes or interfaces.
  - Unless there is major behavior change, in which case you should document the differences.
- Use `<typeparam>` for type parameters in generic types or methods.
- Use `<typeparamref>` to reference type parameters in documentation.
- Use `<c>` for inline code snippets.
- Use `<code>` for code blocks. `<code>` tags should be placed within an `<example>` tag. Add the language of the code example using the `language` attribute, for example, `<code language="csharp">`.
