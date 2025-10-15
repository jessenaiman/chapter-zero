// <copyright file="Trigger.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace OmegaSpiral.Field.Cutscenes
{
    /// <summary>
    /// A <see cref="Cutscene"/> that triggers on collision with a <see cref="Gamepiece"/>'s collision shapes.
    ///
    /// A Gamepiece with collision shapes on a layer monitored by the Trigger may activate the Trigger.
    /// Triggers typically wait for <see cref="Gamepiece.MovementStopped"/> before being run, but that behaviour may
    /// be overridden in derived Triggers by modifying <see cref="OnAreaEntered(Area2D)"/>.
    /// </summary>
    [Tool]
    public partial class Trigger : Cutscene
    {
        private bool isActive = true;

        /// <summary>
        /// Emitted when a <see cref="Gamepiece"/> begins moving to the cell occupied by the Trigger.
        /// </summary>
        /// <param name="gamepiece">The gamepiece that is entering.</param>
        [Signal]
        public delegate void GamepieceEnteredEventHandler(Gamepiece gamepiece);

        /// <summary>
        /// Emitted when a <see cref="Gamepiece"/> begins moving away from the cell occupied by the Trigger.
        /// </summary>
        /// <param name="gamepiece">The gamepiece that is exiting.</param>
        [Signal]
        public delegate void GamepieceExitedEventHandler(Gamepiece gamepiece);

        /// <summary>
        /// Emitted when a <see cref="Gamepiece"/> is finishing moving to the cell occupied by the Trigger.
        /// </summary>
        /// <param name="gamepiece">The gamepiece that triggered.</param>
        [Signal]
        public delegate void TriggeredEventHandler(Gamepiece gamepiece);

        /// <summary>
        /// Gets or sets a value indicating whether an active Trigger may be run, whereas one that is inactive may only be run
        /// directly through code via the <see cref="Cutscene.Run"/> method.
        /// </summary>
        [Export]
        public bool IsActive
        {
            get => this.isActive;
            set
            {
                this.isActive = value;

                if (!Engine.IsEditorHint())
                {
                    if (!this.IsInsideTree())
                    {
                        // We'll set the value and wait for the node to be ready
                        this.isActive = value;
                        return;
                    }

                    // We use "Visible Collision Shapes" to debug positions on the gameboard, so we'll want
                    // to change the state of child collision shapes. These could be either CollisionShape2Ds
                    // or CollisionPolygon2Ds.
                    // Note that we only want to disable the collision shapes of objects that are actually
                    // connected to this Interaction.
                    var connections = this.GetIncomingConnections();
                    foreach (var connection in connections)
                    {
                        var callable = (Callable) connection["callable"];
                        if (callable.Method == "_on_area_entered")
                        {
                            var callableDict = (Godot.Collections.Dictionary) connection["callable"];
                            if (callableDict.ContainsKey("object"))
                            {
                                var objectVariant = callableDict["object"];
                                var connectedNode = objectVariant.As<Node>();
                                if (connectedNode is Area2D connectedArea)
                                {
                                    foreach (var node in connectedArea.FindChildren("*", "CollisionShape2D"))
                                    {
                                        if (node is CollisionShape2D shape2D)
                                        {
                                            shape2D.SetDisabled(!this.isActive);
                                        }
                                    }

                                    foreach (var node in connectedArea.FindChildren("*", "CollisionPolygon2D"))
                                    {
                                        if (node is CollisionPolygon2D polygon2D)
                                        {
                                            polygon2D.SetDisabled(!this.isActive);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets configuration warnings for this trigger.
        /// </summary>
        /// <returns>An array of configuration warning strings.</returns>
        public string[] GetConfigurationWarnings()
        {
            var warnings = new List<string>();
            var hasAreaEnteredBindings = false;

            var connections = this.GetIncomingConnections();
            foreach (var connection in connections)
            {
                var callable = (Callable) connection["callable"];
                if (callable.Method == "_on_area_entered")
                {
                    hasAreaEnteredBindings = true;
                }
            }

            if (!hasAreaEnteredBindings)
            {
                warnings.Add("This object does not have a CollisionObject2D's signals connected to " +
                    "this Trigger's _on_area_entered method. The Trigger will never be triggered!");
            }

            return warnings.ToArray();
        }

        /// <summary>
        /// Called when an area exits the trigger.
        /// </summary>
        /// <param name="area">The area that exited.</param>
        protected void OnAreaExited(Area2D area)
        {
            ArgumentNullException.ThrowIfNull(area);

            var gamepiece = area.Owner as Gamepiece;
            if (gamepiece != null)
            {
                this.EmitSignal(SignalName.GamepieceExited, gamepiece);
            }
        }

        /// <inheritdoc/>
        public override void _Ready()
        {
            if (!Engine.IsEditorHint())
            {
                // FieldEvents.input_paused.connect(_on_input_paused) - we'll need to implement this when FieldEvents is available
            }
        }

        /// <summary>
        /// Called when a gamepiece arrives at the trigger.
        /// </summary>
        /// <param name="gamepiece">The gamepiece that arrived.</param>
        protected void OnGamepieceArrived(Gamepiece gamepiece)
        {
            this.EmitSignal(SignalName.Triggered, gamepiece);
            this.Run();
        }

        /// <summary>
        /// Called when an area enters the trigger.
        /// </summary>
        /// <param name="area">The area that entered.</param>
        protected void OnAreaEntered(Area2D area)
        {
            ArgumentNullException.ThrowIfNull(area);

            var gamepiece = area.Owner as Gamepiece;

            // Check to make sure that the gamepiece is moving before connecting to its 'arriving'
            // signal. This catches edge cases where the Trigger is unpaused while a colliding object
            // is standing on top of it (which would mean that _on_gamepiece_arrived would trigger once
            // the gamepiece moves OFF of it. Which is bad.).
            if (gamepiece != null && gamepiece.IsMoving)
            {
                this.EmitSignal(SignalName.GamepieceEntered, gamepiece);
                gamepiece.MovementStopped += () => this.OnGamepieceArrived(gamepiece);

                // Triggers need to block input early. Otherwise, if waiting until the gamepiece has arrived,
                // there's a chance that the player's controller may have received the gamepiece.arrived
                // signal first and continue moving the gamepiece.
                // await GetTree().ProcessFrame;
                CutsceneInProgress = true;
            }
        }
    }
}
