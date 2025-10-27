using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using GdUnit4;
using Godot;
using OmegaSpiral.Source.Audio;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public partial class AudioManagerTests
{
    private const string _HoverCuePath = "res://assets/audio/ui/hover.ogg";
    private const string _ConfirmCuePath = "res://assets/audio/ui/confirm.ogg";

    [TestCase]
    public async Task OnUiHover_EmitsCueStarted()
    {
        var audioManager = await CreateAudioManagerAsync().ConfigureAwait(false);
        SeedCueStream(audioManager, _HoverCuePath);

        bool cueSignalReceived = false;
        int capturedCategory = -1;
        audioManager.Connect(AudioManager.SignalName.CueStarted, Callable.From<string, int>((cueId, category) =>
        {
            if (cueId == _HoverCuePath)
            {
                cueSignalReceived = true;
                capturedCategory = category;
            }
        }));

        audioManager.OnUiHover(null);

        AssertThat(cueSignalReceived).IsTrue();
        AssertThat(capturedCategory).IsEqual((int)AudioCategory.Sfx);
    }

    [TestCase]
    public async Task OnUiConfirm_DucksAmbientAndRestores()
    {
        var audioManager = await CreateAudioManagerAsync().ConfigureAwait(false);
        SeedCueStream(audioManager, _ConfirmCuePath);

        bool duckSignalReceived = false;
        audioManager.Connect(AudioManager.SignalName.DuckApplied, Callable.From<int, int, float, int>((_, target, amount, duration) =>
        {
            if (target == (int)AudioCategory.Ambient && amount == -6f && duration == 250)
            {
                duckSignalReceived = true;
            }
        }));

        var originalVolume = audioManager.GetCategoryVolume(AudioCategory.Ambient);
        audioManager.OnUiConfirm(null);
        var duckedVolume = audioManager.GetCategoryVolume(AudioCategory.Ambient);

        AssertThat(duckSignalReceived).IsTrue();
        AssertThat(duckedVolume).IsEqual(originalVolume - 6f);

        var tree = (SceneTree)Engine.GetMainLoop();
        await tree.ToSignal(tree.CreateTimer(0.35f), SceneTreeTimer.SignalName.Timeout);

        AssertThat(audioManager.GetCategoryVolume(AudioCategory.Ambient)).IsEqual(originalVolume);
    }

    private static async Task<AudioManager> CreateAudioManagerAsync()
    {
        var audioManager = AutoFree(new AudioManager())!;
        var tree = (SceneTree)Engine.GetMainLoop();
        var readyAwaitable = audioManager.ToSignal(audioManager, Node.SignalName.Ready);
        tree.Root.AddChild(audioManager);
        await readyAwaitable;
        return audioManager;
    }

    private static void SeedCueStream(AudioManager audioManager, string path)
    {
        var cacheField = typeof(AudioManager).GetField("audioCache", BindingFlags.Instance | BindingFlags.NonPublic);
        var cache = cacheField?.GetValue(audioManager) as Dictionary<string, AudioStream>;
        if (cache == null)
        {
            return;
        }

        if (!cache.ContainsKey(path))
        {
            var stream = new AudioStreamWav
            {
                Format = AudioStreamWav.FormatEnum.Format16Bits,
                MixRate = 44100,
                Stereo = false,
                Data = new byte[882] // 0.01s of silence
            };
            cache[path] = stream;
        }
    }
}
