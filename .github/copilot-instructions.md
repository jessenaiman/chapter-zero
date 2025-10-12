# C# Documentation Rules

**MUST USE SERANA TO FIND AND REPLACE, and FOR PROJECT MEMORIES**
**ALWAYS CHECK THE BUILD AFTER EVERY NEW FILE AND CHANGE**
**USE CODACY TO CHECK FOR ISSUES AND FIXES**
**NEVER MOVE ON TO ANOTHER FILE UNTIL THE BUILD IS CLEAN**
**MUST ENFORCE XML Documentation Comments**

- Public members should be documented with XML comments.
- It is encouraged to document internal members as well, especially if they are complex or not self-explanatory.
- Use `<summary>` for method descriptions. This should be a brief overview of what the method does.
- Use `<param>` for method parameters.
- Use `<returns>` for method return values.
- Use `<remarks>` for additional information, which can include implementation details, usage notes, or any other relevant context.
- Use `<exception>` to document exceptions thrown by methods.
- Use `<see langword>` for language-specific keywords like `null`, `true`, `false`, `int`, `bool`, etc.
- Use `<see cref>` to reference other types or members inline (in a sentence).
- Use `<inheritdoc/>` to inherit documentation from base classes or interfaces.
  - Unless there is major behavior change, in which case you should document the differences.
- Use `<typeparam>` for type parameters in generic types or methods.
- Use `<typeparamref>` to reference type parameters in documentation.
- Use `<c>` for inline code snippets.
- Use `<code>` for code blocks. `<code>` tags should be placed within an `<example>` tag. Add the language of the code example using the `language` attribute, for example, `<code language="csharp">`.

