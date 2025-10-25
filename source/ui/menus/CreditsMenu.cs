// <copyright file="CreditsMenu.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Ui.Menus;

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

            // Try to cache scroll container from scene first
            _CreditsScroll = GetNodeOrNull<ScrollContainer>("ContentContainer/CreditsScrollContainer");
            _CreditsContent = GetNodeOrNull<VBoxContainer>("ContentContainer/CreditsScrollContainer/CreditsContent");

            // If not found in scene, create programmatically
            var contentContainer = GetNodeOrNull<Control>("ContentContainer");
            if (contentContainer != null && _CreditsScroll == null)
            {
                _CreditsScroll = new ScrollContainer
                {
                    Name = "CreditsScrollContainer",
                    SizeFlagsHorizontal = SizeFlags.ExpandFill,
                    SizeFlagsVertical = SizeFlags.ExpandFill,
                    FollowFocus = true
                };

                // Insert before MenuButtonContainer to maintain proper order
                var menuButtonContainer = contentContainer.GetNodeOrNull<VBoxContainer>("MenuButtonContainer");
                if (menuButtonContainer != null)
                {
                    var index = menuButtonContainer.GetIndex();
                    contentContainer.AddChild(_CreditsScroll);
                    contentContainer.MoveChild(_CreditsScroll, index);
                }
                else
                {
                    contentContainer.AddChild(_CreditsScroll);
                }

                _CreditsContent = new VBoxContainer
                {
                    Name = "CreditsContent",
                    SizeFlagsHorizontal = SizeFlags.ExpandFill,
                    Alignment = BoxContainer.AlignmentMode.Center
                };
                _CreditsScroll.AddChild(_CreditsContent);
            }

            // Populate credits content
            PopulateCredits();
        }

        /// <summary>
        /// Populates the credits menu with a back button.
        /// Called by MenuBase after initialization completes.
        /// </summary>
        protected override void PopulateMenuButtons()
        {
            // Create back button dynamically
            _BackButton = CreateMenuButton("BackButton", "Back");

            // Connect button signal
            if (_BackButton != null)
                _BackButton.Pressed += OnBackPressed;
        }

        /// <summary>
        /// Called every frame to handle scrolling credits.
        /// </summary>
        /// <param name="delta">Time elapsed since the previous frame.</param>
        public override void _Process(double delta)
        {
            base._Process(delta);

            if (_IsScrolling && _CreditsScroll != null)
            {
                // Scroll the credits
                float newScroll = (float)(_CreditsScroll.ScrollVertical + ScrollSpeed * delta);
                _CreditsScroll.ScrollVertical = (int)newScroll;

                // Check if we've reached the end
                if (_CreditsContent != null)
                {
                    float contentHeight = _CreditsContent.GetCombinedMinimumSize().Y;
                    float visibleHeight = _CreditsScroll.GetViewportRect().Size.Y;

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
                CustomMinimumSize = new Vector2(0, fontSize + 10)
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
