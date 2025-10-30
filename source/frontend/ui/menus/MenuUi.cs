// <copyright file="MenuUi.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Design;
using OmegaSpiral.Source.InputSystem;
using OmegaSpiral.Source.Ui.Omega;

namespace OmegaSpiral.Source.Ui.Menus;

/// <summary>
/// Base Menu UI with Omega theming and menu-specific behavior.
/// Base class for all static game menus (Main Menu, Pause, Options, etc.) - NOT for sequential narrative scenes.
/// <para><strong>Core Responsibilities:</strong></para>
/// <list type="number">
/// <item><description>Button container management and navigation</description></item>
/// <item><description>Input routing for keyboard/gamepad</description></item>
/// <item><description>Omega visual theme (border, CRT effects) - enabled by default</description></item>
/// </list>
/// </summary>
[GlobalClass]
public partial class MenuUi : OmegaContainer
{
    private const string _ButtonAudioMetaKey = "omega_audio_registered";

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
    private AudioManager? _AudioManager;
    private OmegaInputRouter? _InputRouter;
    private bool _InputEventsSubscribed;

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

    /// <inheritdoc/>
/// <summary>
    /// Called when the node enters the tree. Hooks up input subscriptions.
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();
        SubscribeToInputRouter();
    }

    public override void _Ready()
    {
        if (Mode == MenuMode.Disabled)
        {
            Visible = false;
            return;
        }

        // Base OmegaContainer initialization builds Omega frame if enabled
        base._Ready();

        // Cache menu-specific nodes using standard Godot GetNodeOrNull
        _ContentContainer = GetNodeOrNull<Control>("ContentContainer")
                          ?? GetNodeOrNull<Control>("CrtFrame/ContentContainer");

        _MenuTitle = GetNodeOrNull<Label>(MenuTitlePath);
        _MenuButtonContainer = GetNodeOrNull<VBoxContainer>(MenuButtonContainerPath);
        _MenuActionBar = GetNodeOrNull<HBoxContainer>(MenuActionBarPath);

        // Initialize menu-specific states
        if (_MenuButtonContainer != null) _MenuButtonContainer.Visible = true;
        if (_MenuActionBar != null) _MenuActionBar.Visible = true;
        if (_MenuTitle != null) _MenuTitle.Visible = true;

        // Register audio for existing buttons
        if (_MenuButtonContainer != null)
        {
            foreach (var button in _MenuButtonContainer.GetChildren().OfType<Button>())
            {
                RegisterButtonAudio(button);
            }
        }

        SubscribeToInputRouter();

        // Now populate menu buttons (after all caching and initialization complete)
        PopulateMenuButtons();
    }

    /// <summary>
    /// Virtual method for subclasses to override and populate their menu buttons.
    /// Called at the end of _Ready() after all initialization is complete.
    /// Override this method to add buttons specific to your menu type.
    /// </summary>
    protected virtual void PopulateMenuButtons()
    {
        // Default: no buttons. Subclasses override to add their buttons.
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

        var button = new OmegaUiButton
        {
            Name = $"{buttonText}Button",
            Text = buttonText,
            FocusMode = Control.FocusModeEnum.All,
            SizeFlagsHorizontal = Control.SizeFlags.Fill,
            SizeFlagsVertical = Control.SizeFlags.ShrinkCenter
        };
        RegisterButtonAudio(button);
        button.Pressed += () => onPressed();
        _MenuButtonContainer.AddChild(button);

        return button;
    }

    /// <summary>
    /// Adds a button to the menu action bar.
    /// </summary>
    /// <param name="buttonText">The text to display on the button.</param>
    /// <param name="onPressed">The callback when button is pressed.</param>
    /// <returns>The created Button node.</returns>
    protected Button AddActionButton(string buttonText, Action onPressed)
    {
        if (_MenuActionBar == null)
        {
            throw new InvalidOperationException("[MenuUi] Cannot add action button: MenuActionBar is not set.");
        }

        var button = new OmegaUiButton
        {
            Name = $"{buttonText}Button",
            Text = buttonText,
            FocusMode = Control.FocusModeEnum.All,
            SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter,
            SizeFlagsVertical = Control.SizeFlags.ShrinkCenter,
            CustomMinimumSize = new Vector2(120, 40)
        };
        RegisterButtonAudio(button);
        button.Pressed += () => onPressed();
        _MenuActionBar.AddChild(button);

        return button;
    }

    /// <summary>
    /// Clears all buttons from the menu using standard Godot QueueFree.
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
    /// Enables or disables the entire menu using standard Godot properties.
    /// </summary>
    /// <param name="enabled">Whether the menu should be enabled.</param>
    protected void SetMenuEnabled(bool enabled)
    {
        if (_MenuButtonContainer != null)
        {
            _MenuButtonContainer.MouseFilter = enabled ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
            _MenuButtonContainer.Modulate = enabled ? Colors.White : DesignConfigService.GetColor("disabled_gray");
        }

        if (_MenuActionBar != null)
        {
            _MenuActionBar.MouseFilter = enabled ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
        }
    }

    /// <summary>
    /// Sets focus to the first button in the menu for keyboard/gamepad navigation.
    /// Uses standard Godot GrabFocus().
    /// </summary>
    public void FocusFirstButton()
    {
        if (_MenuButtonContainer == null) return;

        var firstButton = _MenuButtonContainer.GetChildren().OfType<Button>().FirstOrDefault();
        firstButton?.GrabFocus();
    }

    /// <summary>
    /// Gets the currently focused button using standard Godot HasFocus().
    /// </summary>
    /// <returns>The focused button, or null if no button has focus.</returns>
    public Button? GetFocusedButton()
    {
        if (_MenuButtonContainer == null) return null;

        return _MenuButtonContainer.GetChildren()
            .OfType<Button>()
            .FirstOrDefault(btn => btn.HasFocus());
    }

    private void SubscribeToInputRouter()
    {
        if (_InputEventsSubscribed) return;

        // Standard Godot GetTree() and GetNodeOrNull
        var tree = GetTree();
        if (tree?.Root == null) return;

        _InputRouter = tree.Root.GetNodeOrNull<OmegaInputRouter>(OmegaInputRouter.DefaultNodeName);
        if (_InputRouter == null) return;

        _InputRouter.MenuToggleRequested += OnInputMenuToggleRequested;
        _InputRouter.NavigateUpRequested += OnInputNavigateUpRequested;
        _InputRouter.NavigateDownRequested += OnInputNavigateDownRequested;
        _InputRouter.ConfirmRequested += OnInputConfirmRequested;
        _InputRouter.BackRequested += OnInputBackRequested;

        _InputEventsSubscribed = true;
    }

    private void UnsubscribeInputRouter()
    {
        if (!_InputEventsSubscribed || _InputRouter == null) return;

        // Standard Godot IsInstanceValid check
        if (IsInstanceValid(_InputRouter))
        {
            _InputRouter.MenuToggleRequested -= OnInputMenuToggleRequested;
            _InputRouter.NavigateUpRequested -= OnInputNavigateUpRequested;
            _InputRouter.NavigateDownRequested -= OnInputNavigateDownRequested;
            _InputRouter.ConfirmRequested -= OnInputConfirmRequested;
            _InputRouter.BackRequested -= OnInputBackRequested;
        }

        _InputRouter = null;
        _InputEventsSubscribed = false;
    }

    private void RegisterButtonAudio(Button button)
    {
        // Standard Godot metadata check
        if (button.HasMeta(_ButtonAudioMetaKey)) return;

        button.MouseEntered += HandleMenuButtonHover;
        button.FocusEntered += HandleMenuButtonHover;
        button.Pressed += HandleMenuButtonPressed;
        button.SetMeta(_ButtonAudioMetaKey, true);
    }

    private void HandleMenuButtonHover()
    {
        // Lazy resolve AudioManager using standard Godot GetNodeOrNull
        if (_AudioManager == null || !IsInstanceValid(_AudioManager))
        {
            _AudioManager = GetTree()?.Root.GetNodeOrNull<AudioManager>("AudioManager");
        }
        _AudioManager?.OnUiHover(null);
    }

    private void HandleMenuButtonPressed()
    {
        // Lazy resolve AudioManager using standard Godot GetNodeOrNull
        if (_AudioManager == null || !IsInstanceValid(_AudioManager))
        {
            _AudioManager = GetTree()?.Root.GetNodeOrNull<AudioManager>("AudioManager");
        }
        _AudioManager?.OnUiConfirm(null);
    }

    private void OnInputMenuToggleRequested()
    {
        if (Mode == MenuMode.Disabled)
        {
            return;
        }

        ToggleMenuVisibility();
    }

    private void OnInputNavigateUpRequested()
    {
        if (!Visible || Mode == MenuMode.Disabled)
        {
            return;
        }

        FocusPreviousButton();
    }

    private void OnInputNavigateDownRequested()
    {
        if (!Visible || Mode == MenuMode.Disabled)
        {
            return;
        }

        FocusNextButton();
    }

    private void OnInputConfirmRequested()
    {
        if (!Visible || Mode == MenuMode.Disabled)
        {
            return;
        }

        ActivateFocusedButton();
    }

    private void OnInputBackRequested()
    {
        if (!Visible || Mode == MenuMode.Disabled)
        {
            return;
        }

        OnBackRequested();
    }

    private void FocusNextButton()
    {
        FocusRelativeButton(1);
    }

    private void FocusPreviousButton()
    {
        FocusRelativeButton(-1);
    }

    private void FocusRelativeButton(int direction)
    {
        if (_MenuButtonContainer == null)
        {
            return;
        }

        var buttons = _MenuButtonContainer.GetChildren()
            .OfType<Button>()
            .Where(btn => btn.Visible && !btn.Disabled)
            .ToList();

        if (buttons.Count == 0)
        {
            return;
        }

        var currentIndex = buttons.FindIndex(btn => btn.HasFocus());
        if (currentIndex == -1)
        {
            if (direction >= 0)
            {
                buttons[0].GrabFocus();
            }
            else
            {
                buttons[^1].GrabFocus();
            }
            return;
        }

        var nextIndex = (currentIndex + direction) % buttons.Count;
        if (nextIndex < 0)
        {
            nextIndex += buttons.Count;
        }

        buttons[nextIndex].GrabFocus();
    }

    private void ActivateFocusedButton()
    {
        var focusedButton = GetFocusedButton();
        if (focusedButton == null)
        {
            FocusFirstButton();
            focusedButton = GetFocusedButton();
        }

        // Standard Godot EmitSignal
        focusedButton?.EmitSignal(Button.SignalName.Pressed);
    }

    /// <summary>
    /// Toggles the menu visibility using standard Godot Visible property.
    /// </summary>
    protected virtual void ToggleMenuVisibility()
    {
        if (!Visible)
        {
            OpenMenu();
        }
        else
        {
            CloseMenu();
        }
    }

    /// <summary>
    /// Opens the menu using standard Godot Visible property.
    /// </summary>
    protected virtual void OpenMenu()
    {
        Visible = true;
        FocusFirstButton();
        OnMenuOpened();
    }

    /// <summary>
    /// Closes the menu using standard Godot Visible property.
    /// </summary>
    protected virtual void CloseMenu()
    {
        Visible = false;
        OnMenuClosed();
    }

    /// <summary>
    /// Called when the menu is opened via input router.
    /// </summary>
    protected virtual void OnMenuOpened()
    {
    }

    /// <summary>
    /// Called when the menu is closed via input router.
    /// </summary>
    protected virtual void OnMenuClosed()
    {
    }

    /// <summary>
    /// Called when the back action is requested.
    /// </summary>
    protected virtual void OnBackRequested()
    {
        CloseMenu();
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

    /// <inheritdoc/>
    public override void _ExitTree()
    {
        UnsubscribeInputRouter();
        base._ExitTree();
    }
}
