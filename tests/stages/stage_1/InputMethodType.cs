// <copyright file="InputMethodType.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Narrative;

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
