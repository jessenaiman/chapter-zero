using Godot;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.Common;

/// <summary>
/// Base class for all narrative scenes across stages 1, 2, and 3.
/// Provides common functionality like audio management, and scene transitions.
/// </summary>
[GlobalClass]
public abstract partial class BaseNarrativeScene : Control
{
    /// <summary>
    /// Plays audio using the centralized AudioManager.
    /// </summary>
    protected void PlayAudio(AudioCategory category, string resourcePath)
    {
        var stream = ResourceLoader.Load<AudioStream>(resourcePath);
        if (stream == null)
        {
            GD.PushWarning($"[BaseNarrativeScene] Unable to load audio stream: {resourcePath}");
            return;
        }

        var audioManager = GetNodeOrNull<AudioManager>("/root/AudioManager");
        if (audioManager != null)
        {
            audioManager.PlayOneShot(stream, category);
        }
        else
        {
            GD.PrintErr("[BaseNarrativeScene] AudioManager not found in scene tree");
        }
    }

    /// <summary>
    /// Transitions to the next scene in the narrative sequence.
    /// </summary>
    /// <param name="nextScenePath">Path to the next scene file.</param>
    protected async Task TransitionToScene(string nextScenePath)
    {
        if (ResourceLoader.Load<PackedScene>(nextScenePath) is not { } nextScene)
        {
            GD.PrintErr($"[BaseNarrativeScene] Failed to load scene: {nextScenePath}");
            return;
        }

        // Optional: Play transition effect here
        await ToSignal(GetTree().CreateTimer(0.5), SceneTreeTimer.SignalName.Timeout);
        GetTree().ChangeSceneToPacked(nextScene);
    }
}