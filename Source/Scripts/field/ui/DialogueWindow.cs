// <copyright file="DialogueWindow.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Godot;

/// <summary>
/// A dialogue window that extends Dialogic's style layer functionality.
/// Shows and hides automatically when timelines start and end.
/// </summary>
[GlobalClass]
public partial class DialogueWindow : Node
{
    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        // Get the Dialogic singleton
        var dialogic = this.GetNode("/root/Dialogic");
        if (dialogic != null)
        {
            // Connect to timeline signals
            dialogic.Connect("timeline_started", Callable.From(() => Show()));
            dialogic.Connect("timeline_ended", Callable.From(() => Hide()));
        }

        Hide();
    }
}
