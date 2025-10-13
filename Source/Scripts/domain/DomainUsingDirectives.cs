// <copyright file="DomainUsingDirectives.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

// Resolve Range ambiguity if needed
using Range = Godot.Range;

// Resolve Timer ambiguity with System.Threading.Timer
using Timer = Godot.Timer;

namespace OmegaSpiral.Domain
{
    /// <summary>
    /// This static class provides common using directives and type aliases
    /// to resolve namespace conflicts and ensure proper Godot C# API usage.
    /// </summary>
    public static class DomainUsingDirectives
    {
        /// <summary>
        /// Gets the string representation of Godot Timer type.
        /// </summary>
        public const string TimerAlias = "Godot.Timer";

        /// <summary>
        /// Gets the string representation of Godot Range type.
        /// </summary>
        public const string RangeAlias = "Godot.Range";

        /// <summary>
        /// Gets the Godot Array type for signal parameters.
        /// </summary>
        public static readonly Type ArrayType = typeof(Godot.Collections.Array);

        /// <summary>
        /// Gets the Godot Dictionary type for signal parameters.
        /// </summary>
        public static readonly Type DictionaryType = typeof(Godot.Collections.Dictionary);

        /// <summary>
        /// Gets the Godot Collections Array type for signal parameters.
        /// </summary>
        public static readonly Type VariantArrayType = typeof(Godot.Collections.Array);
    }
}
