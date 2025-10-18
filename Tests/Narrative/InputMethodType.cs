// <copyright file="InputMethodType.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

#pragma warning disable SA1636

namespace OmegaSpiral.Tests.Functional.Narrative;

/// <summary>
/// Input method types for content block interaction.
/// </summary>
internal enum InputMethodType
{
    /// <summary>
    /// No input method yet used.
    /// </summary>
    None,

    /// <summary>
    /// Keyboard input.
    /// </summary>
    Keyboard,

    /// <summary>
    /// Gamepad/controller input.
    /// </summary>
    Gamepad,

    /// <summary>
    /// Mouse input.
    /// </summary>
    Mouse,
}
