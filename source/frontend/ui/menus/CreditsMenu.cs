// <copyright file="CreditsMenu.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Ui.Menus
{
    /// <summary>
    /// Credits menu showing game contributors and acknowledgments.
    /// Displays scrolling credits with team information.
    /// Extends MenuUi for consistent menu behavior and styling.
    /// </summary>
    [GlobalClass]
    public partial class CreditsMenu : MenuUi
    {
        // --- EXPORTED PROPERTIES ---

        /// <summary>
        /// Speed at which credits scroll (pixels per second).
        /// </summary>
        [Export] public float ScrollSpeed { get; set; } = 50.0f;

        /// <summary>
        /// Whether credits should loop when they reach the end.
        /// </summary>
        [Export] public bool LoopCredits { get; set; } = true;

        // --- PRIVATE FIELDS ---

        private Button? _BackButton;
        private ScrollContainer? _CreditsScroll;
        private VBoxContainer? _CreditsContent;
        private bool _IsScrolling;

        // --- GODOT LIFECYCLE ---

        /// <summary>
        /// Called when the node enters the scene tree.
        /// Sets up the credits menu with scrolling text.
        /// Button population is handled by PopulateMenuButtons().
        /// </summary>
        public override void _Ready()
        {
            base._Ready();

            // Set the menu title
            SetMenuTitle("CREDITS");

            // Defer node caching until after reparenting completes
            CallDeferred(nameof(CacheCreditsNodes));
        }

        /// <summary>
        /// Caches CreditsMenu-specific nodes after reparenting completes.
        /// Uses exported paths first, then falls back to FindChild for robustness.
        /// Idempotent: safe to call multiple times without creating duplicates.
        /// </summary>
        private void CacheCreditsNodes()
        {
            // Try to resolve ContentContainer using exported path or recursive search
            var contentContainer = ContentContainer;
            if (contentContainer == null)
            {
                contentContainer = FindChild("ContentContainer", true, false) as Control;
                if (contentContainer == null)
                {
                    GD.PushWarning("[CreditsMenu] ContentContainer not found after reparenting");
                    return;
                }
            }

            // Cache CreditsScrollContainer with fallback to FindChild
            _CreditsScroll ??= GetNodeOrNull<ScrollContainer>("CreditsScrollContainer")
                            ?? contentContainer.GetNodeOrNull<ScrollContainer>("CreditsScrollContainer")
                            ?? FindChild("CreditsScrollContainer", true, false) as ScrollContainer;

            if (_CreditsScroll == null)
                GD.PushWarning("[CreditsMenu] CreditsScrollContainer not found");

            // Cache CreditsContent with fallback to FindChild
            _CreditsContent ??= GetNodeOrNull<VBoxContainer>("CreditsContent")
                             ?? contentContainer.GetNodeOrNull<VBoxContainer>("CreditsContent")
                             ?? FindChild("CreditsContent", true, false) as VBoxContainer;

            if (_CreditsContent == null)
                GD.PushWarning("[CreditsMenu] CreditsContent not found");

            // Cache MenuButtonContainer with fallback to FindChild
            var menuButtonContainer = MenuButtonContainer
                                   ?? FindChild("MenuButtonContainer", true, false) as VBoxContainer;

            if (menuButtonContainer == null)
                GD.PushWarning("[CreditsMenu] MenuButtonContainer not found");

            // Cache MenuTitle with fallback to FindChild
            var menuTitle = MenuTitle
                          ?? FindChild("MenuTitle", true, false) as Label;

            if (menuTitle == null)
                GD.PushWarning("[CreditsMenu] MenuTitle not found");

            // Cache BackButton with fallback to FindChild
            _BackButton ??= GetNodeOrNull<Button>("BackButton")
                         ?? contentContainer.GetNodeOrNull<Button>("BackButton")
                         ?? FindChild("BackButton", true, false) as Button;

            if (_BackButton == null)
                GD.PushWarning("[CreditsMenu] BackButton not found");

            GD.Print("[CreditsMenu] Node caching completed after reparenting");

            // Populate credits after caching nodes
            PopulateCredits();
        }

        /// <summary>
        /// Populates the credits menu with a back button in the action bar.
        /// Called by MenuBase after initialization completes.
        /// </summary>
        protected override void PopulateMenuButtons()
        {
            // Create back button via AddMenuButton to attach to MenuButtonContainer after reparenting
            _BackButton = AddMenuButton("Back", OnBackPressed);
            if (_BackButton != null)
            {
                _BackButton.Name = "BackButton";
            }

            // Ensure focus on the back button when showing the menu
            FocusFirstButton();
        }

        /// <summary>
        /// Called every frame to handle scrolling credits.
        /// </summary>
        /// <param name="delta">Time elapsed since the previous frame.</param>
        public override void _Process(double delta)
        {
            base._Process(delta);

            if (_IsScrolling && _CreditsScroll != null && _CreditsContent != null)
            {
                // Scroll the credits
                float newScroll = (float)(_CreditsScroll.ScrollVertical + ScrollSpeed * delta);
                _CreditsScroll.ScrollVertical = (int)newScroll;

                // Check if we've reached the end, guarding against zero height
                float contentHeight = _CreditsContent.GetCombinedMinimumSize().Y;
                float visibleHeight = _CreditsScroll.GetViewportRect().Size.Y;

                // Only proceed if we have valid dimensions
                if (visibleHeight > 0 && contentHeight > 0)
                {
                    if (newScroll >= contentHeight - visibleHeight)
                    {
                        if (LoopCredits)
                        {
                            // Reset to top
                            _CreditsScroll.ScrollVertical = 0;
                        }
                        else
                        {
                            // Stop scrolling
                            _IsScrolling = false;
                        }
                    }
                }
            }
        }

        // --- PUBLIC API ---

        /// <summary>
        /// Shows the credits menu and starts scrolling.
        /// </summary>
        public void ShowCredits()
        {
            Visible = true;
            _IsScrolling = true;
            if (_CreditsScroll != null)
                _CreditsScroll.ScrollVertical = 0;
            FocusFirstButton();
        }

        /// <summary>
        /// Hides the credits menu and stops scrolling.
        /// </summary>
        public void HideCredits()
        {
            _IsScrolling = false;
            Visible = false;
        }

        // --- PRIVATE METHODS ---

        private void PopulateCredits()
        {
            if (_CreditsContent == null) return;

            // Clear existing content
            foreach (Node child in _CreditsContent.GetChildren())
            {
                child.QueueFree();
            }

            // Add credits sections
            AddCreditsSection("ΩMEGA SPIRAL - CHAPTER ZERO", 48);
            AddCreditsSection("", 24); // Spacer

            AddCreditsSection("DEVELOPMENT TEAM", 36);
            AddCreditsSection("Lead Developer", 24);
            AddCreditsSection("Jesse Naiman", 18);
            AddCreditsSection("", 12); // Spacer

            AddCreditsSection("Game Design", 24);
            AddCreditsSection("Jesse Naiman", 18);
            AddCreditsSection("", 12);

            AddCreditsSection("Programming", 24);
            AddCreditsSection("Jesse Naiman", 18);
            AddCreditsSection("", 12);

            AddCreditsSection("UI/UX Design", 24);
            AddCreditsSection("Jesse Naiman", 18);
            AddCreditsSection("", 24);

            AddCreditsSection("SPECIAL THANKS", 36);
            AddCreditsSection("Godot Engine Community", 24);
            AddCreditsSection("Open Source Contributors", 18);
            AddCreditsSection("Beta Testers", 18);
            AddCreditsSection("", 24);

            AddCreditsSection("TOOLS & LIBRARIES", 36);
            AddCreditsSection("Godot Engine", 24);
            AddCreditsSection("GdUnit4 Testing Framework", 18);
            AddCreditsSection("C# 14.0", 18);
            AddCreditsSection(".NET 10.0", 18);
            AddCreditsSection("", 24);

            AddCreditsSection("© 2025 Ωmega Spiral", 24);
            AddCreditsSection("All rights reserved", 18);
        }

        private void AddCreditsSection(string text, int fontSize)
        {
            if (_CreditsContent == null) return;

            var label = new Label
            {
                Text = text,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                CustomMinimumSize = new Vector2(400, fontSize + 10)
            };

            // Set font size (simplified - would need proper theme setup)
            var font = label.GetThemeFont("font");
            if (font != null)
            {
                label.AddThemeFontSizeOverride("font_size", fontSize);
            }

            _CreditsContent.AddChild(label);
        }

        // --- EVENT HANDLERS ---

        private void OnBackPressed()
        {
            HideCredits();
        }
    }
}
