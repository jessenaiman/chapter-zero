// <copyright file="UIListMenu.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// A list menu is a template menu that provides common functionality for the combat menus.
///
/// A list menu has a number of button entries that may be clicked or navigated to with the arrows,
/// buttons, D-pad, etc. A cursor follows the selected menu entry and player input is forwarded via
/// a simple set of signals.
/// </summary>
public partial class UIListMenu : VBoxContainer
{
    /// <summary>
    /// Gets or sets the scene representing the different menu entries. The scene must be some derivation of
    /// <see cref="BaseButton"/>.
    /// </summary>
    [Export]
    public PackedScene? EntryScene { get; set; }

    private bool isDisabled = true;

    /// <summary>
    /// Gets or sets a value indicating whether disables or enables clicking on/navigating to the various entries.
    /// Defaults to true, as most menus will animate into existence before being interactable.
    /// </summary>
    public bool IsDisabled
    {
        get => this.isDisabled;
        set
        {
            this.isDisabled = value;
            foreach (var entry in this.entries)
            {
                entry.Disabled = this.isDisabled;
            }

            this.FocusFirstEntry();
            this.menuCursor.Visible = !this.isDisabled;
        }
    }

    // Track all battler list entries in the following array.
    protected List<BaseButton> entries = new List<BaseButton>();

    private AnimationPlayer? anim;
    private UIMenuCursor? menuCursor;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.anim = this.GetNode<AnimationPlayer>("AnimationPlayer");
        this.menuCursor = this.GetNode<UIMenuCursor>("MenuCursor");
    }

    /// <summary>
    /// Bring the first entry into input focus, moving the cursor to its position.
    /// </summary>
    public void FocusFirstEntry()
    {
        if (this.entries.Count > 0)
        {
            this.entries[0].GrabFocus();
            this.menuCursor.Position = this.entries[0].GlobalPosition + new Vector2(0.0f, this.entries[0].Size.Y / 2.0f);
        }
    }

    /// <summary>
    /// Fades in the battler list, allowing input and focusing the first button only after the animation
    /// has finished.
    /// </summary>
    public async void FadeIn()
    {
        this.anim.Play("fade_in");
        await this.ToSignal(this.anim, AnimationPlayer.SignalName.AnimationFinished);
        this.IsDisabled = false;

        this.FocusFirstEntry();
    }

    /// <summary>
    /// Fades out the battler list, disabling input to the list beforehand.
    /// </summary>
    public async void FadeOut()
    {
        this.IsDisabled = true;
        this.anim.Play("fade_out");
        await this.ToSignal(this.anim, AnimationPlayer.SignalName.AnimationFinished);
    }

    /// <summary>
    /// Creates a button entry, based on the specified entry scene. Hooks up automatic callbacks to the
    /// button's signals that may be modified depending on the specific menu.
    /// Returns the created entry so that a menu may add additional functionality to the entry.
    /// </summary>
    /// <returns>The created entry.</returns>
    protected BaseButton CreateEntry()
    {
        var newEntry = this.EntryScene.Instantiate();
        System.Diagnostics.Debug.Assert(newEntry is BaseButton, "Entries to a UIMenuList must be derived from BaseButton!" +
            " A non-BaseButton entry_scene has been specified.");

        var buttonEntry = newEntry as BaseButton;
        this.AddChild(buttonEntry);

        // We're going to keep these as independent functions rather than inline lambdas, since each menu
        // will probably respond to these events differently. For example, a target menu will want to
        // highlight a specific battler when a new entry is focused and an action menu will want to
        // forward which action was selected.
        buttonEntry.FocusEntered += () => this.OnEntryFocused(buttonEntry);
        buttonEntry.MouseEntered += () => this.OnEntryFocused(buttonEntry);
        buttonEntry.Pressed += () => this.OnEntryPressed(buttonEntry);

        this.entries.Add(buttonEntry);

        if (this.IsDisabled)
        {
            buttonEntry.Disabled = true;
        }

        return buttonEntry;
    }

    protected void LoopFirstAndLastEntries()
    {
        System.Diagnostics.Debug.Assert(this.entries.Count > 0, "No action entries for the menu to connect!");

        var lastEntryIndex = this.entries.Count - 1;
        var firstEntry = this.entries[0];
        if (lastEntryIndex > 0)
        {
            var lastEntry = this.entries[lastEntryIndex];
            firstEntry.FocusNeighborTop = firstEntry.GetPathTo(lastEntry);
            lastEntry.FocusNeighborBottom = lastEntry.GetPathTo(firstEntry);
        }
        else if (lastEntryIndex == 0)
        {
            firstEntry.FocusNeighborTop = ".";
            firstEntry.FocusNeighborBottom = ".";
        }
    }

    /// <summary>
    /// Moves the <see cref="UIMenuCursor"/> to the focused entry. Derivative menus may want to add additional
    /// behaviour.
    /// </summary>
    /// <param name="entry">The entry that was focused.</param>
    protected void OnEntryFocused(BaseButton entry)
    {
        this.menuCursor.MoveTo(entry.GlobalPosition + new Vector2(0.0f, entry.Size.Y / 2.0f));
    }

    /// <summary>
    /// Hides (and disables) the menu. Derivative menus may want to add additional behaviour.
    /// </summary>
    /// <param name="entry">The entry that was pressed.</param>
    protected virtual void OnEntryPressed(BaseButton entry)
    {
        if (!this.IsDisabled)
        {
            this.FadeOut();
        }
    }
}
