// <copyright file="GlobalSuppressions.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

// Note: In Godot 4+, use the [GlobalClass] attribute on classes that need to be
// visible in the Godot editor. This allows proper C# namespace usage while
// maintaining editor integration.
//
// Example:
//   namespace OmegaSpiral.Combat;
//
//   [GlobalClass]
//   public partial class MyNode : Node
//   {
//       // ...
//   }

// Suppressions for code analysis rules that conflict with Godot requirements
// or are not applicable to game development patterns

// CA1805: Do not initialize unnecessarily - Explicit null initialization improves clarity
[assembly: SuppressMessage("Performance", "CA1805:Do not initialize unnecessarily", Justification = "Explicit initialization improves code clarity and intent", Scope = "namespaceanddescendants", Target = "~N:OmegaSpiral")]
