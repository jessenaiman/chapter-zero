// <copyright file="MainMenuWithAnimations.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Ui.Menus;
using OmegaSpiral.Source.Stages.Stage0Start;

namespace OmegaSpiral.Source.Ui.Menus
{
    /// <summary>
    /// Main menu with Omega styling and intro animations.
    /// Extends the base MainMenu with fade-in animations for title and menu buttons.
    /// The animation can be skipped by player input.
    /// </summary>
    [GlobalClass]
    public partial class MainMenuWithAnimations : MainMenu
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenuWithAnimations"/> class.
        /// Sets up correct node paths for the animated menu structure.
        /// </summary>
        public MainMenuWithAnimations()
        {
            // Node paths will be set in _Ready() after base initialization
        }

        /// <summary>
        /// Animation state machine for managing menu transitions.
        /// </summary>
        private AnimationNodeStateMachinePlayback? _AnimationStateMachine;        /// <summary>
                                                                                  /// Called when the node enters the scene tree.
                                                                                  /// Sets up animation state machine and initializes intro sequence.
                                                                                  /// Does not populate buttons since they are already in the scene.
                                                                                  /// </summary>
        public override void _Ready()
        {
            // Set node paths before calling base._Ready()
            MenuTitlePath = "MenuContainer/TitleMargin/TitleContainer/TitleLabel";
            MenuButtonContainerPath = "MenuContainer/MenuButtonsMargin/MenuButtonsContainer/MenuButtonsBoxContainer";
            MenuActionBarPath = null; // No action bar in this layout

            // Call base._Ready() for OmegaContainer setup, but we'll override PopulateMenuButtons
            base._Ready();

            // Set up animation
            var animationTree = GetNodeOrNull("MenuAnimationTree") as AnimationTree;
            if (animationTree != null)
            {
                _AnimationStateMachine = (AnimationNodeStateMachinePlayback) animationTree.Get("parameters/playback");

                // Start with intro animation
                _AnimationStateMachine?.Travel("Intro");
            }
        }

        /// <summary>
        /// Override to prevent dynamic button population since buttons are already in the scene.
        /// </summary>
        protected override void PopulateMenuButtons()
        {
            // Do nothing - buttons are already created in the scene
        }

        /// <summary>
        /// Called when intro animation completes.
        /// Transitions to main menu state.
        /// </summary>
        private void OnIntroDone()
        {
            _AnimationStateMachine?.Travel("OpenMainMenu");
        }

        /// <summary>
        /// Checks if currently in intro animation state.
        /// </summary>
        private bool IsInIntro()
        {
            return _AnimationStateMachine?.GetCurrentNode() == "Intro";
        }

        /// <summary>
        /// Determines if the given input event should skip the intro.
        /// </summary>
        private static bool EventSkipsIntro(InputEvent @event)
        {
            return @event.IsActionReleased("ui_accept") ||
                   @event.IsActionReleased("ui_select") ||
                   @event.IsActionReleased("ui_cancel") ||
                   IsMouseButtonReleased(@event);
        }

        /// <summary>
        /// Checks if the event is a released mouse button.
        /// </summary>
        private static bool IsMouseButtonReleased(InputEvent @event)
        {
            return @event is InputEventMouseButton mouseEvent && !mouseEvent.Pressed;
        }

        /// <summary>
        /// Handles input events, allowing intro skip.
        /// </summary>
        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            if (IsInIntro() && EventSkipsIntro(@event))
            {
                OnIntroDone();
            }
        }
    }
}
