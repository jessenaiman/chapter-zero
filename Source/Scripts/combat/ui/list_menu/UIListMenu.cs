using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
    /// The scene representing the different menu entries. The scene must be some derivation of
    /// <see cref="BaseButton"/>.
    /// </summary>
    [Export]
    public PackedScene EntryScene { get; set; }

    private bool _isDisabled = true;
    /// <summary>
    /// Disables or enables clicking on/navigating to the various entries.
    /// Defaults to true, as most menus will animate into existence before being interactable.
    /// </summary>
    public bool IsDisabled
    {
        get => _isDisabled;
        set
        {
            _isDisabled = value;
            foreach (var entry in _entries)
            {
                entry.Disabled = _isDisabled;
            }

            FocusFirstEntry();
            _menuCursor.Visible = !_isDisabled;
        }
    }

    // Track all battler list entries in the following array.
    protected List<BaseButton> _entries = new List<BaseButton>();

    private AnimationPlayer _anim;
    private UIMenuCursor _menuCursor;

    public override void _Ready()
    {
        _anim = GetNode<AnimationPlayer>("AnimationPlayer");
        _menuCursor = GetNode<UIMenuCursor>("MenuCursor");
    }

    /// <summary>
    /// Bring the first entry into input focus, moving the cursor to its position.
    /// </summary>
    public void FocusFirstEntry()
    {
        if (_entries.Count > 0)
        {
            _entries[0].GrabFocus();
            _menuCursor.Position = _entries[0].GlobalPosition + new Vector2(0.0f, _entries[0].Size.Y / 2.0f);
        }
    }

    /// <summary>
    /// Fades in the battler list, allowing input and focusing the first button only after the animation
    /// has finished.
    /// </summary>
    public async void FadeIn()
    {
        _anim.Play("fade_in");
        await ToSignal(_anim, AnimationPlayer.SignalName.AnimationFinished);
        IsDisabled = false;

        FocusFirstEntry();
    }

    /// <summary>
    /// Fades out the battler list, disabling input to the list beforehand.
    /// </summary>
    public async void FadeOut()
    {
        IsDisabled = true;
        _anim.Play("fade_out");
        await ToSignal(_anim, AnimationPlayer.SignalName.AnimationFinished);
    }

    /// <summary>
    /// Creates a button entry, based on the specified entry scene. Hooks up automatic callbacks to the
    /// button's signals that may be modified depending on the specific menu.
    /// Returns the created entry so that a menu may add additional functionality to the entry.
    /// </summary>
    /// <returns>The created entry</returns>
    protected BaseButton CreateEntry()
    {
        var newEntry = EntryScene.Instantiate();
        System.Diagnostics.Debug.Assert(newEntry is BaseButton, "Entries to a UIMenuList must be derived from BaseButton!" +
            " A non-BaseButton entry_scene has been specified.");

        var buttonEntry = newEntry as BaseButton;
        AddChild(buttonEntry);

        // We're going to keep these as independent functions rather than inline lambdas, since each menu
        // will probably respond to these events differently. For example, a target menu will want to
        // highlight a specific battler when a new entry is focused and an action menu will want to
        // forward which action was selected.
        buttonEntry.FocusEntered += () => _onEntryFocused(buttonEntry);
        buttonEntry.MouseEntered += () => _onEntryFocused(buttonEntry);
        buttonEntry.Pressed += () => _onEntryPressed(buttonEntry);

        _entries.Add(buttonEntry);

        if (IsDisabled)
        {
            buttonEntry.Disabled = true;
        }
        return buttonEntry;
    }

    protected void LoopFirstAndLastEntries()
    {
        System.Diagnostics.Debug.Assert(_entries.Count > 0, "No action entries for the menu to connect!");

        var lastEntryIndex = _entries.Count - 1;
        var firstEntry = _entries[0];
        if (lastEntryIndex > 0)
        {
            var lastEntry = _entries[lastEntryIndex];
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
    /// <param name="entry">The entry that was focused</param>
    protected void _onEntryFocused(BaseButton entry)
    {
        _menuCursor.MoveTo(entry.GlobalPosition + new Vector2(0.0f, entry.Size.Y / 2.0f));
    }

    /// <summary>
    /// Hides (and disables) the menu. Derivative menus may want to add additional behaviour.
    /// </summary>
    /// <param name="entry">The entry that was pressed</param>
    protected virtual void _onEntryPressed(BaseButton entry)
    {
        if (!IsDisabled)
        {
            FadeOut();
        }
    }
}
