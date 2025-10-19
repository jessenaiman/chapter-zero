# AGENTS.md

**Omega Spiral** is an revolutionary and evolutionary narrative and turn based rpg game where players navigate through five distinct scenes, each representing a different era of gaming aesthetics. The game features dynamic AI-driven narrative personas (Dreamweavers) that adapt to player choices, creating emergent storytelling experiences.

## Technology Stack

- **Engine**: Godot 4.5.1 Stable (/home/adam/Godot_v4.5.1-stable_mono_linux_x86_64) with .NET/Mono support
- **Language**: C# 14 (using .NET 8.0) with preview language features
- **AI Integration**: NobodyWho plugin for local LLM inference

## Testing with GDUnit

- GDUnit4 supports logic-only tests that run without the Godot runtime for speed, and Godot-dependent tests using [RequireGodotRuntime] for scene/node integration.

*Note*: In Godot 4+, use the [GlobalClass] attribute on classes that need to be
visible in the Godot editor. This allows proper C# namespace usage while
maintaining editor integration.

## Project Guidelines

## Act 1 Specifics - Ghost Writer

- Narrative content is loaded exclusively from JSON assets at res://Source/Data/stages/ghost-terminal/. No hardcoded fallbacks exist in code.
- “GhostTerminalCinematicDirector must not synthesize content; it only transforms NarrativeSceneData into beats.”
- “Tests must construct NarrativeSceneData (via NarrativeSceneFactory) and validate translation only.”

 Example:

```cs
   namespace OmegaSpiral.Combat;

   [GlobalClass]
   public partial class MyNode : Node
   {
       // ...
   }
```

## C# style guide

Having well-defined and consistent coding conventions is important for every project, and Godot
is no exception to this rule.

This page contains a coding style guide, which is followed by developers of and contributors to Godot
itself. As such, it is mainly intended for those who want to contribute to the project, but since
the conventions and guidelines mentioned in this article are those most widely adopted by the users
of the language, we encourage you to do the same, especially if you do not have such a guide yet.

.. note:: This article is by no means an exhaustive guide on how to follow the standard coding
        conventions or best practices. If you feel unsure of an aspect which is not covered here,
        please refer to more comprehensive documentation, such as
        `C# Coding Conventions <https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions>`*or
        `Framework Design Guidelines <https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines>`*.

## Language specification

----------------------

This project currently uses **C# version 14.0** in its engine and example source code,
we are using a Release Candidate for C# (the current baseline requirement).
So, before we move to a newer version, care must be taken to avoid mixing
language features only available in C# 13.0 or later.

For detailed information on C# features in different versions, please see
`What's New in C# <https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/>`_.

## Formatting

----------------------

### General guidelines

----------------------

- Use line feed (**LF**) characters to break lines, not CRLF or CR.
- Use one line feed character at the end of each file, except for `csproj` files.
- Use **UTF-8** encoding without a `byte order mark <https://en.wikipedia.org/wiki/Byte_order_mark>`_.
- Use **4 spaces** instead of tabs for indentation (which is referred to as "soft tabs").
- Consider breaking a line into several if it's longer than 100 characters.

### Line breaks and blank lines

For a general indentation rule, follow `the "Allman Style" <https://en.wikipedia.org/wiki/Indentation_style#Allman_style>`_
which recommends placing the brace associated with a control statement on the next line, indented to
the same level:

```csharp
    if (x > 0)
    {
        DoSomething();
    }

    // NOT this:
    if (x > 0) {
        DoSomething();
    }
```

However, you may choose to omit line breaks inside brackets:

- For simple property accessors.
- For simple object, array, or collection initializers.
- For abstract auto property, indexer, or event declarations.

.. code-block:: csharp

```csharp
    // You may put the brackets in a single line in the following cases:
    public interface MyInterface
    {
        int MyProperty { get; set; }
    }

    public class MyClass : ParentClass
    {
        public int Value
        {
            get { return 0; }
            set
            {
                ArrayValue = new[] { value };
            }
        }
    }
```

### Insert a blank line

- After a list of using statements
- Between method, property, and inner type declarations.
- At the end of each file.

    Field and constant declarations can be grouped together according to relevance. In that case, consider inserting a blank line between the groups for easier reading.

    Avoid inserting a blank line:

- After `{` (the opening brace).
- Before `}` (the closing brace).
- After a comment block or a single-line comment.
- Adjacent to another blank line.

    ```csharp
    using System;
    using Godot;

    // Blank line after `using` list.
    public class MyClass
    { // No blank line after `{`.
        public enum MyEnum
        {
            Value,
            AnotherValue // No blank line before `}`.
        }

        // Blank line around inner types.
        public const int SomeConstant = 1;
        public const int AnotherConstant = 2;

        private Vector3 _x; // Related constants or fields can be
        private Vector3 _y; // grouped together.

        private float _width;
        private float _height;

        public int MyProperty { get; set; } // Blank line around properties.

        public void MyMethod()
        {
            // Some comment.
            AnotherMethod(); // No blank line after a comment.
        } // Blank line around methods.

        public void AnotherMethod()
        {
        }
    }
    ```

### Using spaces

Insert a space:

- Around a binary and ternary operator.
- Between an opening parenthesis and `if`, `for`, `foreach`, `catch`, `while`, `lock`, or `using` keywords.
- Before and within a single-line accessor block.
- Between accessors in a single-line accessor block.
- After a comma that is not at the end of a line.
- After a semicolon in a `for` statement.
- After a colon in a single-line `case` statement.
- Around a colon in a type declaration.
- Around a lambda arrow.
- After the single-line comment symbol (`//`), and before it if used at the end of a line.
- After the opening brace, and before the closing brace in a single-line initializer.

    Do not use a space:

- After type cast parentheses.

    The following example shows a proper use of spaces, according to some of the above conventions:

```csharp
public class MyClass<A, B> : Parent<A, B>
{
    public float MyProperty { get; set; }

    public float AnotherProperty
    {
        get { return MyProperty; }
    }

    public void MyMethod()
    {
        int[] values = { 1, 2, 3, 4 };
        int sum = 0;

        // Single line comment.
        for (int i = 0; i < values.Length; i++)
        {
            switch (i)
            {
                case 3: return;
                default:
                    sum += i > 2 ? 0 : 1;
                    break;
            }
        }

        i += (int)MyProperty; // No space after a type cast.
    }
}
```

### Naming conventions

Use PascalCase for all namespaces, type names, and member-level identifiers (methods, properties, constants, events), except for private fields:

```csharp
namespace ExampleProject
{
    public class PlayerCharacter
    {
        public const float DefaultSpeed = 10f;

        public float CurrentSpeed { get; set; }

        protected int HitPoints;

        private void CalculateWeaponDamage()
        {
        }
    }
}
```

Use camelCase for all other identifiers (local variables, method arguments), and use an underscore (`_`) as a prefix for private fields (but not for methods or properties):

```csharp
private Vector3 _aimingAt; // Use an `_` prefix for private fields.

private void Attack(float attackStrength)
{
    Enemy targetFound = FindTarget(_aimingAt);

    targetFound?.Hit(attackStrength);
}
```

There's an exception for two-letter acronyms (e.g., `UI`), which should be uppercase where PascalCase is expected and lowercase otherwise.

Note that `id` is not an acronym and should be treated as a normal identifier:

```csharp
public string Id { get; }

public UIManager UI
{
    get { return uiManager; }
}
```

It is generally discouraged to use a type name as a prefix of an identifier (for example, `string strText`). An exception is interfaces, which should be prefixed with `I` (for example, `IInventoryHolder` or `IDamageable`).

Lastly, prefer descriptive names over excessive shortening when it impacts readability.

### Implicitly typed local variables

Consider using implicit typing (`var`) for local variables only when the type is evident from the right side of the assignment.

```csharp
// You can use `var` for these cases:
var direction = new Vector2(1, 0);
var value = (int)speed;
var text = "Some value";

for (var i = 0; i < 10; i++)
{
}

// But not for these:
var valueFromMethod = GetValue();
var velocity = direction * 1.5;

// It's generally better to use explicit typing for numeric literals where ambiguity exists:
var numericValue = 1.5;
```

var direction = new Vector2(1, 0);

var value = (int)speed;

var text = "Some value";

for (var i = 0; i < 10; i++)
{
}

// But not for these:

var value = GetValue();

var velocity = direction * 1.5;

// It's generally a better idea to use explicit typing for numeric values, especially with
// the existence of the `real_t` alias in Godot, which can either be double or float
// depending on the build configuration.

var value = 1.5;

### Other considerations

- Use explicit access modifiers.
- Use properties instead of non-private fields.
- Use modifiers in this order:
   ``public``/``protected``/``private``/``internal``/``virtual``/``override``/``abstract``/``new``/``static``/``readonly``.
- Avoid using fully-qualified names or ``this.`` prefix for members when it's not necessary.
- Remove unused ``using`` statements and unnecessary parentheses.
- Consider omitting the default initial value for a type.
- Consider using null-conditional operators or type initializers to make the code more compact.
- Use safe cast when there is a possibility of the value being a different type, and use direct cast otherwise.

## Build/Lint/Test Commands

### Build Commands

```bash
dotnet build  # Standard build
dotnet build --warnaserror  # Enforce warnings as errors
```

### Test Commands

```bash
dotnet test  # Run all tests
# Note: Some tests require Godot runtime and must be run through Godot's test runner or GdUnit4
```

### Lint/Format Commands

```bash
dotnet format  # Format code
dotnet format --verify-no-changes # Verify formatting without changes
dotnet build --warnaserror # Static analysis with Roslyn/StyleCop analyzers
```

### Pre-commit Checks (automatically enforced)

1. Code formatting: `dotnet format --verify-no-changes`
2. Static analysis: `dotnet build --warnaserror`
3. Tests: `dotnet test`
4. Security (optional): `trivy fs --exit-code 1 --severity HIGH,CRITICAL .`

## Code Style & Conventions

### Non-Obvious Patterns

- **Godot C# Integration**: Use `Godot.Collections.Dictionary` and `Godot.Collections.Array` for signal parameters, not System.Collections
- **Type Aliases**: `Range = Godot.Range`, `Timer = Godot.Timer` to resolve System/Godot conflicts
- **Singleton Autoloads**: SceneManager and GameState are autoloaded singletons defined in project.godot
- **Scene Transitions**: Use SceneManager.TransitionToScene() for all scene changes (not direct Godot methods)

### XML Documentation Required

- All public members must have XML documentation comments
- Use `<inheritdoc/>` for overriding methods when behavior is unchanged
- Document exceptions with `<exception>` tags

### Async Programming

- Use 'Async' suffix for all async methods
- Avoid `.Wait()`, `.Result`, or `.GetAwaiter().GetResult()` in async code
- Use `Task.WhenAll()` for parallel execution

### Godot-Specific Conventions

- Use PascalCase for Godot signals and node paths
- Access Godot built-in types through Godot namespace (not System)
- Use `GD.Print()` for logging, not Console.WriteLine()
- File paths in Godot use `res://` scheme for project resources
