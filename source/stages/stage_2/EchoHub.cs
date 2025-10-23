// <copyright file="EchoHub.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Stages.Stage2;

/// <summary>
/// Orchestrator for the Echo Chamber stage (Stage 2).
/// Manages stage initialization and loads the first beat from the stage manifest.
/// Individual beats handle their own progression via BeatSceneBase pattern.
/// This simplified design mirrors Stage 1's MainMenu approach.
/// </summary>
[GlobalClass]
public partial class EchoHub : Node
{
    private const string Stage2ManifestPath = "res://source/stages/stage_2/stage_2_manifest.json";
    private SceneManager? _sceneManager;

    /// <summary>
    /// Emitted when the entire Echo Chamber stage completes.
    /// </summary>
    /// <param name="claimedDreamweaver">The Dreamweaver that claimed the player (based on affinity).</param>
    [Signal]
    public delegate void StageCompleteEventHandler(string claimedDreamweaver);

    /// <inheritdoc/>
    public override void _Ready()
    {
        GD.Print("[EchoHub] Echo Chamber stage starting");

        // Get scene manager
        _sceneManager = GetNode<SceneManager>("/root/SceneManager");

        if (_sceneManager == null)
        {
            GD.PrintErr("[EchoHub] SceneManager not found in scene tree");
            return;
        }

        // Load manifest to get first beat
        var manifestLoader = new StageManifestLoader();
        var manifest = manifestLoader.LoadManifest(Stage2ManifestPath);

        if (manifest == null)
        {
            GD.PrintErr("[EchoHub] Failed to load stage manifest");
            return;
        }

        GD.Print($"[EchoHub] Loaded manifest with {manifest.Scenes.Count} beats");

        // Get first beat and transition to it
        var firstBeat = manifest.GetFirstScene();
        if (firstBeat == null)
        {
            GD.PrintErr("[EchoHub] No first beat found in manifest");
            return;
        }

        GD.Print($"[EchoHub] Starting first beat: {firstBeat.Id} ({firstBeat.SceneFile})");
        _sceneManager.TransitionToScene(firstBeat.SceneFile);
    }
}
