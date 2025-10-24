// <copyright file="MenuUI.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using Godot;
using OmegaSpiral.Source.UI.Omega;

namespace OmegaSpiral.Source.UI.Menus;

/// <summary>
/// Menu UI that extends OmegaUI and adds static menu behavior.
/// Provides button layouts, navigation, and menu-specific styling.
/// Base class for all static UI menus (Main Menu, Options, etc.) - NOT for sequential scenes.
/// Key difference from TerminalUI: No sequential logic, no auto-transitions, pure user agency.
/// </summary>
[GlobalClass]
public partial class MenuUI : OmegaUI
{
    /// <summary>
    /// Menu interaction modes.
    /// </summary>
    public enum MenuMode
    {
        /// <summary>No menu functionality (disabled).</summary>
        Disabled,
        /// <summary>Standard menu with buttons and navigation.</summary>
        Standard,
        /// <summary>Modal menu that blocks interaction with other UI.</summary>
        Modal
    }

    [Export] public MenuMode Mode { get; set; } = MenuMode.Standard;

    // Menu-specific node references
    private VBoxContainer? _menuButtonContainer;
    private HBoxContainer? _menuActionBar;
    private Label? _menuTitle;

    [ExportGroup("Menu Scenes")]
    [Export] public PackedScene? StyledButtonScene { get; set; }

    /// <inheritdoc/>
    public override void _Ready()
    {
        if (Mode == MenuMode.Disabled)
        {
            Visible = false; // Hide the menu completely if disabled
            return;
        }

        // Base OmegaUI initialization is called automatically by Godot's lifecycle.
        // We just need to call our menu-specific setup.
        base._Ready();
    }

    /// <summary>
    /// Caches menu-specific node references in addition to base OmegaUI nodes.
    /// </summary>
    protected override void CacheRequiredNodes()
    {
        // First, let the base class cache its required nodes (TextDisplay, shader layers, etc.)
        base.CacheRequiredNodes();

        // Now, cache the nodes specific to the menu
        _menuButtonContainer = GetNodeOrNull<VBoxContainer>("MenuButtonContainer");
        _menuActionBar = GetNodeOrNull<HBoxContainer>("MenuActionBar");
        _menuTitle = GetNodeOrNull<Label>("MenuTitle");

        if (_menuButtonContainer == null)
            GD.PushWarning("[MenuUI] MenuButtonContainer not found. Buttons cannot be added.");
    }

    /// <summary>
    /// Initializes menu-specific components after base OmegaUI initialization.
    /// </summary>
    private void InitializeMenuComponents()
    {
        // No longer needed; initialization is handled in InitializeComponentStates.
    }

    /// <summary>
    /// Initializes menu-specific UI states.
    /// </summary>
    protected override void InitializeComponentStates()
    {
        // First, let the base class initialize its components (e.g., clear text)
        base.InitializeComponentStates();

        // Now, initialize menu-specific states
        if (_menuButtonContainer != null) _menuButtonContainer.Visible = true;
        if (_menuActionBar != null) _menuActionBar.Visible = true;
        if (_menuTitle != null) _menuTitle.Visible = true;

        // CORRECT: Use the protected properties from OmegaUI to style the shader layers
        if (PhosphorLayer != null) PhosphorLayer.Modulate = Colors.White;
        if (ScanlineLayer != null) ScanlineLayer.Modulate = Colors.White;
        if (GlitchLayer != null) GlitchLayer.Modulate = Colors.White;
    }

    /// <summary>
    /// Initializes menu-specific component states.
    /// </summary>
    private void InitializeMenuStates()
    {
        // No longer needed; initialization is handled in InitializeComponentStates.
    }

    /// <summary>
    /// Sets the menu title text.
    /// </summary>
    /// <param name="title">The title to display.</param>
    protected void SetMenuTitle(string title)
    {
        if (_menuTitle != null)
        {
            _menuTitle.Text = title;
        }
    }

    /// <summary>
    /// Gets the menu button container for adding/managing buttons.
    /// </summary>
    /// <returns>The VBoxContainer for menu buttons, or null if not found.</returns>
    protected VBoxContainer? GetMenuButtonContainer()
    {
        return _menuButtonContainer;
    }

    /// <summary>
    /// Adds a button to the menu.
    /// </summary>
    /// <param name="buttonText">The text to display on the button.</param>
    /// <param name="onPressed">The callback when button is pressed.</param>
    /// <returns>The created Button node.</returns>
    protected Button AddMenuButton(string buttonText, Action onPressed)
    {
        if (_menuButtonContainer == null)
        {
            throw new InvalidOperationException("[MenuUI] Cannot add button: MenuButtonContainer is not set.");
        }
        if (StyledButtonScene == null)
        {
            throw new InvalidOperationException("[MenuUI] Cannot add button: StyledButtonScene is not set in the Inspector.");
        }

        var button = StyledButtonScene.Instantiate<Button>();
        button.Text = buttonText;

        button.Pressed += () => onPressed();
        _menuButtonContainer.AddChild(button);

        return button;
    }

    /// <summary>
    /// Clears all buttons from the menu.
    /// </summary>
    protected void ClearMenuButtons()
    {
        if (_menuButtonContainer != null)
        {
            foreach (var child in _menuButtonContainer.GetChildren())
            {
                if (child is Button button)
                {
                    button.QueueFree();
                }
            }
        }
    }

    /// <summary>
    /// Enables or disables the entire menu.
    /// </summary>
    /// <param name="enabled">Whether the menu should be enabled.</param>
    protected void SetMenuEnabled(bool enabled)
    {
        if (_menuButtonContainer != null)
        {
            _menuButtonContainer.MouseFilter = enabled ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
            _menuButtonContainer.Modulate = enabled ? Colors.White : new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        if (_menuActionBar != null)
        {
            _menuActionBar.MouseFilter = enabled ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
        }
    }

    /// <summary>
    /// Gets the action bar for menu controls (back, confirm, etc.).
    /// </summary>
    /// <returns>The HBoxContainer for action buttons, or null if not found.</returns>
    protected HBoxContainer? GetMenuActionBar()
    {
        return _menuActionBar;
    }
}
