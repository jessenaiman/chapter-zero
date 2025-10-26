// <copyright file="MenuUi.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Linq;
using Godot;
using OmegaSpiral.Source.Ui.Omega;

namespace OmegaSpiral.Source.Ui.Menus;

/// <summary>
/// Menu Ui that extends OmegaUi and adds static menu behavior for traditional game menus.
/// Base class for all static game menus (Main Menu, Pause, Options, etc.) - NOT for sequential narrative scenes.
/// Key difference from OmegaUi: No sequential logic, no auto-transitions, pure user agency.
/// <para><strong>Three Core Responsibilities:</strong></para>
/// <list type="number">
/// <item><description>Standard button container structure with sensible defaults (via NodePath exports)</description></item>
/// <item><description>Navigation helpers for keyboard/gamepad focus management</description></item>
/// <item><description>Menu-specific shader presets (less glitchy than narrative terminals)</description></item>
/// </list>
/// </summary>
[GlobalClass]
public partial class MenuUi : OmegaUi
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
        /// <summary>Modal menu that blocks interaction with other Ui.</summary>
        Modal
    }

    [Export] public MenuMode Mode { get; set; } = MenuMode.Standard;

    /// <summary>
    /// Path to the main button container. Defaults to standard menu structure.
    /// </summary>
    [ExportGroup("Menu Node Paths")]
    [Export] public NodePath? MenuButtonContainerPath { get; set; } = "ContentContainer/MenuButtonContainer";

    /// <summary>
    /// Path to the action bar (back/confirm buttons). Defaults to standard menu structure.
    /// </summary>
    [Export] public NodePath? MenuActionBarPath { get; set; } = "ContentContainer/MenuActionBar";

    /// <summary>
    /// Path to the menu title label. Defaults to standard menu structure.
    /// </summary>
    [Export] public NodePath? MenuTitlePath { get; set; } = "ContentContainer/MenuTitle";

    // Menu-specific node references
    private VBoxContainer? _MenuButtonContainer;
    private HBoxContainer? _MenuActionBar;
    private Label? _MenuTitle;
    private Control? _ContentContainer;

    /// <summary>
    /// Public access to the content container for testing purposes.
    /// </summary>
    public Control? ContentContainer => _ContentContainer;

    /// <summary>
    /// Public access to the menu button container for testing purposes.
    /// </summary>
    public VBoxContainer? MenuButtonContainer => _MenuButtonContainer;

    /// <summary>
    /// Public access to the menu action bar for testing purposes.
    /// </summary>
    public HBoxContainer? MenuActionBar => _MenuActionBar;

    /// <summary>
    /// Public access to the menu title label for testing purposes.
    /// </summary>
    public Label? MenuTitle => _MenuTitle;

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

        // Base OmegaUi initialization is called automatically by Godot's lifecycle.
        // This ensures CacheRequiredNodes and InitializeComponentStates complete first.
        base._Ready();

        // AFTER base initialization, call the hook for subclasses to populate buttons
        PopulateMenuButtons();
    }

    /// <summary>
    /// Virtual method for subclasses to override and populate their menu buttons.
    /// Called AFTER MenuBase initialization is complete, so all containers are ready.
    /// Override this method to add buttons specific to your menu type.
    /// </summary>
    protected virtual void PopulateMenuButtons()
    {
        // Default: no buttons. Subclasses override to add their buttons.
    }

    /// <summary>
    /// Caches menu-specific node references in addition to base OmegaUi nodes.
    /// Uses NodePath exports for flexibility while providing sensible defaults.
    /// </summary>
    protected override void CacheRequiredNodes()
    {
        // First, let the base class cache its required nodes (TextDisplay, shader layers, etc.)
        base.CacheRequiredNodes();

        // Try to cache from scene first
        _MenuButtonContainer = MenuButtonContainerPath != null && !string.IsNullOrEmpty(MenuButtonContainerPath.ToString())
            ? GetNodeOrNull<VBoxContainer>(MenuButtonContainerPath)
            : null;

        // If not found, CREATE the default structure programmatically
        _ContentContainer = GetNodeOrNull<Control>("ContentContainer");
        if (_ContentContainer == null)
        {
            _ContentContainer = new VBoxContainer
            {
                Name = "ContentContainer",
                AnchorLeft = 0, AnchorTop = 0, AnchorRight = 1, AnchorBottom = 1,
                GrowHorizontal = GrowDirection.Both,
                GrowVertical = GrowDirection.Both
            };
            AddChild(_ContentContainer);
        }

        // Create MenuTitle if it doesn't exist
        _MenuTitle = MenuTitlePath != null && !string.IsNullOrEmpty(MenuTitlePath.ToString())
            ? GetNodeOrNull<Label>(MenuTitlePath)
            : null;

        if (_MenuTitle == null && _ContentContainer != null)
        {
            _MenuTitle = new Label
            {
                Name = "MenuTitle",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                SizeFlagsVertical = SizeFlags.ShrinkBegin
            };
            _ContentContainer.AddChild(_MenuTitle);
        }

        // Create MenuButtonContainer if it doesn't exist
        if (_MenuButtonContainer == null && _ContentContainer != null)
        {
            _MenuButtonContainer = new VBoxContainer
            {
                Name = "MenuButtonContainer",
                SizeFlagsHorizontal = SizeFlags.ShrinkCenter,
                SizeFlagsVertical = SizeFlags.ExpandFill,
                Alignment = BoxContainer.AlignmentMode.Center
            };
            _ContentContainer.AddChild(_MenuButtonContainer);
        }

        // Create MenuActionBar if it doesn't exist
        _MenuActionBar = MenuActionBarPath != null && !string.IsNullOrEmpty(MenuActionBarPath.ToString())
            ? GetNodeOrNull<HBoxContainer>(MenuActionBarPath)
            : null;

        if (_MenuActionBar == null && _ContentContainer != null)
        {
            _MenuActionBar = new HBoxContainer
            {
                Name = "MenuActionBar",
                SizeFlagsHorizontal = SizeFlags.ShrinkCenter,
                SizeFlagsVertical = SizeFlags.ShrinkEnd,
                Alignment = BoxContainer.AlignmentMode.Center
            };
            _ContentContainer.AddChild(_MenuActionBar);
        }

        if (_MenuButtonContainer == null)
            GD.PushWarning("[MenuUi] MenuButtonContainer not found at path. Buttons cannot be added.");
    }

    /// <summary>
    /// Initializes menu-specific components after base OmegaUi initialization.
    /// </summary>
    private void InitializeMenuComponents()
    {
        // No longer needed; initialization is handled in InitializeComponentStates.
    }

    /// <summary>
    /// Initializes menu-specific Ui states.
    /// </summary>
    protected override void InitializeComponentStates()
    {
        // First, let the base class initialize its components (e.g., clear text)
        base.InitializeComponentStates();

        // Now, initialize menu-specific states
        if (_MenuButtonContainer != null) _MenuButtonContainer.Visible = true;
        if (_MenuActionBar != null) _MenuActionBar.Visible = true;
        if (_MenuTitle != null) _MenuTitle.Visible = true;

        // CORRECT: Use the protected properties from OmegaUi to style the shader layers
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
        if (_MenuTitle != null)
        {
            _MenuTitle.Text = title;
        }
    }

    /// <summary>
    /// Gets the menu button container for adding/managing buttons.
    /// </summary>
    /// <returns>The VBoxContainer for menu buttons, or null if not found.</returns>
    protected VBoxContainer? GetMenuButtonContainer()
    {
        return _MenuButtonContainer;
    }

    /// <summary>
    /// Factory method for creating menu-specific buttons.
    /// Extends OmegaUi's CreateButton() with menu-specific styling and layout.
    /// This method integrates the button into the menu's button container automatically.
    /// </summary>
    /// <param name="buttonName">The name to assign to the button node.</param>
    /// <param name="buttonText">The display text for the button.</param>
    /// <returns>A new Button node ready for the menu, already added to the menu container.</returns>
    protected Button CreateMenuButton(string buttonName, string buttonText = "")
    {
        var button = base.CreateButton(buttonName, buttonText);

        if (_MenuButtonContainer != null)
        {
            _MenuButtonContainer.AddChild(button);
        }
        else
        {
            GD.PushWarning($"[MenuUi] Cannot add button '{buttonName}' - MenuButtonContainer is not set.");
        }

        return button;
    }

    /// <summary>
    /// Adds a button to the menu.
    /// </summary>
    /// <param name="buttonText">The text to display on the button.</param>
    /// <param name="onPressed">The callback when button is pressed.</param>
    /// <returns>The created Button node.</returns>
    protected Button AddMenuButton(string buttonText, Action onPressed)
    {
        if (_MenuButtonContainer == null)
        {
            throw new InvalidOperationException("[MenuUi] Cannot add button: MenuButtonContainer is not set.");
        }
        if (StyledButtonScene == null)
        {
            throw new InvalidOperationException("[MenuUi] Cannot add button: StyledButtonScene is not set in the Inspector.");
        }

        var button = StyledButtonScene.Instantiate<Button>();
        button.Text = buttonText;

        button.Pressed += () => onPressed();
        _MenuButtonContainer.AddChild(button);

        return button;
    }

    /// <summary>
    /// Clears all buttons from the menu.
    /// </summary>
    protected void ClearMenuButtons()
    {
        if (_MenuButtonContainer != null)
        {
            foreach (var child in _MenuButtonContainer.GetChildren())
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
        if (_MenuButtonContainer != null)
        {
            _MenuButtonContainer.MouseFilter = enabled ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
            _MenuButtonContainer.Modulate = enabled ? Colors.White : OmegaSpiralColors.DisabledGray;
        }

        if (_MenuActionBar != null)
        {
            _MenuActionBar.MouseFilter = enabled ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
        }
    }

    /// <summary>
    /// Gets the action bar for menu controls (back, confirm, etc.).
    /// </summary>
    /// <returns>The HBoxContainer for action buttons, or null if not found.</returns>
    protected HBoxContainer? GetMenuActionBar()
    {
        return _MenuActionBar;
    }

    // --- NAVIGATION HELPERS (Responsibility 2: Focus Management) ---

    /// <summary>
    /// Sets focus to the first button in the menu for keyboard/gamepad navigation.
    /// Call this when the menu becomes active.
    /// </summary>
    public void FocusFirstButton()
    {
        if (_MenuButtonContainer == null) return;

        var firstButton = _MenuButtonContainer.GetChildren().OfType<Button>().FirstOrDefault();
        firstButton?.GrabFocus();
    }

    /// <summary>
    /// Gets the currently focused button in the menu, if any.
    /// </summary>
    /// <returns>The focused button, or null if no button has focus.</returns>
    public Button? GetFocusedButton()
    {
        if (_MenuButtonContainer == null) return null;

        return _MenuButtonContainer.GetChildren()
            .OfType<Button>()
            .FirstOrDefault(btn => btn.HasFocus());
    }

    /// <summary>
    /// Handles input for menu navigation (up/down for keyboard/gamepad).
    /// Override this to customize navigation behavior.
    /// </summary>
    /// <param name="event">The input event to process.</param>
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Mode == MenuMode.Disabled) return;
        if (_MenuButtonContainer == null) return;

        // Allow Godot's built-in focus navigation to handle ui_up/ui_down
        // This is just a hook for custom navigation if needed
    }
}
