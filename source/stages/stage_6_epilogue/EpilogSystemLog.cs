// <copyright file="Stage6SystemLog.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Stages.Stage6;

/// <summary>
/// Final system-log sequence for the Chapter Zero demo.
/// Streams JSON-driven narrative beats, drives lightweight visual cues,
/// and coordinates audio texture changes for the closing tableau.
/// </summary>
[GlobalClass]
public partial class Stage6SystemLog : Control
{
    private const string _DataPath = "res://source/stages/stage_6/stage6.json";
    private const string _SchemaPath = "res://source/data/schemas/narrative_terminal_schema.json";

    private static readonly Dictionary<DreamweaverThread, string> _DreamweaverThreadTags = new()
    {
        { DreamweaverThread.Hero, "light" },
        { DreamweaverThread.Shadow, "wrath" },
        { DreamweaverThread.Ambition, "mischief" },
    };

    private static readonly Dictionary<string, string> _VisualNodeNames = new()
    {
        { "8bit", "%Pose8Bit" },
        { "16bit", "%Pose16Bit" },
        { "32bit", "%Pose32Bit" },
        { "hd", "%PoseHD" },
        { "space", "%PoseSpace" },
    };

    private readonly List<ChoiceOption> _ActiveChoices = new();
    private readonly Dictionary<string, CanvasItem> _PoseNodes = new();
    private readonly Dictionary<AudioCategory, float> _OriginalVolumes = new();

    private RichTextLabel _OutputLabel = default!;
    private Label _PromptLabel = default!;
    private NarrativeSceneData _NarrativeData = new();
    private GameState _GameState = default!;
    private SceneManager _SceneManager = default!;
    private AudioManager? _AudioManager;
    private int _CurrentBlockIndex;
    private bool _AwaitingChoice;

    /// <summary>
    /// Event emitted when a glitch cue is encountered in the narrative data.
    /// Allows visual shaders and overlay effects to hook into the log stream.
    /// </summary>
    [Signal]
    public delegate void GlitchTriggeredEventHandler(string cue);

    [Export]
    public NodePath OutputLabelPath { get; set; } = "%LogLabel";

    [Export]
    public NodePath PromptLabelPath { get; set; } = "%PromptLabel";

    [Export]
    public float CharacterDelaySeconds { get; set; } = 0.028f;

    [Export]
    public float ChoicePauseSeconds { get; set; } = 1.2f;

    /// <inheritdoc/>
    public override async void _Ready()
    {
        this._OutputLabel = this.GetNode<RichTextLabel>(this.OutputLabelPath);
        this._PromptLabel = this.GetNode<Label>(this.PromptLabelPath);
        this._GameState = this.GetNode<GameState>("/root/GameState");
        this._SceneManager = this.GetNode<SceneManager>("/root/SceneManager");
        this._AudioManager = this.GetNodeOrNull<AudioManager>("/root/AudioManager");

        this.InitializeVisualNodes();

        if (!this.TryLoadNarrativeData())
        {
            this._OutputLabel.Text = "[color=#ff5959]System log playback failed. Missing or invalid content.[/color]";
            return;
        }

        await this.PlayOpeningAsync().ConfigureAwait(false);
        await this.PlayFromCurrentBlockAsync().ConfigureAwait(false);
    }

    private void InitializeVisualNodes()
    {
        foreach (KeyValuePair<string, string> entry in _VisualNodeNames)
        {
            if (this.GetNodeOrNull<CanvasItem>(entry.Value) is { } canvas)
            {
                canvas.Visible = entry.Key == "8bit";
                this._PoseNodes[entry.Key] = canvas;
            }
        }
    }

    /// <inheritdoc/>
    public override void _Input(InputEvent @event)
    {
        if (!this._AwaitingChoice || @event is not InputEventKey { Pressed: true } keyEvent)
        {
            return;
        }

        string? inputText = keyEvent.Unicode switch
        {
            '1' => "1",
            '2' => "2",
            '3' => "3",
            _ => null,
        };

        if (string.IsNullOrEmpty(inputText))
        {
            return;
        }

        var selection = this.ResolveChoice(inputText);
        if (selection == null)
        {
            return;
        }

        this.ProcessChoice(selection);
        GetViewport().SetInputAsHandled();
    }

    private async Task PlayOpeningAsync()
    {
        foreach (string line in this._NarrativeData.OpeningLines)
        {
            await this.DisplayLineAsync(line).ConfigureAwait(false);
        }

        if (this._NarrativeData.InitialChoice?.Options.Count > 0)
        {
            this._OutputLabel.AppendText($"\n[b]{this._NarrativeData.InitialChoice.Prompt}[/b]\n");
            var option = this._NarrativeData.InitialChoice.Options[0];
            string label = option.Label ?? option.Text ?? option.Id ?? "commit";
            this._OutputLabel.AppendText($"  1. {label}\n");
            await this.DelayAsync(this.ChoicePauseSeconds).ConfigureAwait(false);
            this._OutputLabel.AppendText("\n[italic]Playback authorised.[/italic]\n");
        }

        this._CurrentBlockIndex = 0;
    }

    private async Task PlayFromCurrentBlockAsync()
    {
        while (this._CurrentBlockIndex < this._NarrativeData.StoryBlocks.Count)
        {
            var block = this._NarrativeData.StoryBlocks[this._CurrentBlockIndex];
            bool requiresChoice = await this.PlayBlockAsync(block).ConfigureAwait(false);
            if (requiresChoice)
            {
                return;
            }

            this._CurrentBlockIndex++;
        }

        await this.HandleExitAsync().ConfigureAwait(false);
    }

    private async Task<bool> PlayBlockAsync(StoryBlock block)
    {
        await this.ProcessParagraphsAsync(block.Paragraphs).ConfigureAwait(false);
        return await this.HandleChoicePresentationAsync(block).ConfigureAwait(false);
    }

    private async Task ProcessParagraphsAsync(IList<string> paragraphs)
    {
        foreach (string paragraph in paragraphs)
        {
            if (this.TryHandleCue(paragraph))
            {
                continue;
            }

            if (paragraph.StartsWith("[DW:", StringComparison.OrdinalIgnoreCase))
            {
                string? spokenLine = this.FilterDreamweaverLine(paragraph);
                if (!string.IsNullOrEmpty(spokenLine))
                {
                    await this.DisplayLineAsync(spokenLine).ConfigureAwait(false);
                }

                continue;
            }

            await this.DisplayLineAsync(paragraph).ConfigureAwait(false);
        }
    }

    private async Task<bool> HandleChoicePresentationAsync(StoryBlock block)
    {
        if (string.IsNullOrWhiteSpace(block.Question) || block.Choices.Count == 0)
        {
            return false;
        }

        this._OutputLabel.AppendText($"\n[center][b]{block.Question}[/b][/center]\n");
        this._ActiveChoices.Clear();
        foreach (var choice in block.Choices)
        {
            this._ActiveChoices.Add(choice);
        }

        for (int i = 0; i < block.Choices.Count; i++)
        {
            string label = block.Choices[i].Text ?? $"Option {i + 1}";
            this._OutputLabel.AppendText($"  {i + 1}. {label}\n");
        }

        this._PromptLabel.Text = "Select an answer (1 or 2)";
        this._AwaitingChoice = true;
        return true;
    }

    private bool TryHandleCue(string paragraph)
    {
        if (!paragraph.StartsWith("[", StringComparison.Ordinal))
        {
            return false;
        }

        if (paragraph.StartsWith("[VISUAL", StringComparison.OrdinalIgnoreCase))
        {
            this.ApplyVisualCue(paragraph);
            return true;
        }

        if (paragraph.StartsWith("[AUDIO", StringComparison.OrdinalIgnoreCase))
        {
            this.ApplyAudioCue(paragraph);
            return true;
        }

        if (paragraph.StartsWith("[GLITCH]", StringComparison.OrdinalIgnoreCase))
        {
            EmitSignal(SignalName.GlitchTriggered, paragraph);
            return true;
        }

        return false;
    }

    private string? FilterDreamweaverLine(string paragraph)
    {
        int closingBracketIndex = paragraph.IndexOf(']');
        if (closingBracketIndex <= 0)
        {
            return paragraph;
        }

        string tagValue = paragraph.Substring(4, closingBracketIndex - 4);
        string threadTag = _DreamweaverThreadTags.GetValueOrDefault(this._GameState.DreamweaverThread, "light");
        return string.Equals(tagValue, threadTag, StringComparison.OrdinalIgnoreCase)
            ? paragraph[(closingBracketIndex + 1)..].TrimStart()
            : null;
    }

    private void ApplyVisualCue(string paragraph)
    {
        string era = ExtractToken(paragraph, "era");
        if (string.IsNullOrWhiteSpace(era))
        {
            return;
        }

        foreach (KeyValuePair<string, CanvasItem> entry in this._PoseNodes)
        {
            entry.Value.Visible = string.Equals(entry.Key, era, StringComparison.OrdinalIgnoreCase);
        }
    }

    private void ApplyAudioCue(string paragraph)
    {
        string command = ExtractToken(paragraph, null);
        this.ProcessAudioCommand(command);
    }

    private void ProcessAudioCommand(string command)
    {
        if (this._AudioManager == null)
        {
            return;
        }

        switch (command.ToLowerInvariant())
        {
            case "bitcrush:on":
                this.CacheVolume(AudioCategory.Music);
                this.CacheVolume(AudioCategory.Ambient);
                this._AudioManager.SetCategoryVolume(AudioCategory.Music, -12f);
                this._AudioManager.SetCategoryVolume(AudioCategory.Ambient, -18f);
                break;
            case "bitcrush:off":
                this.RestoreVolume(AudioCategory.Music);
                this.RestoreVolume(AudioCategory.Ambient);
                break;
            case "mute":
                this.CacheVolume(AudioCategory.Music);
                this.CacheVolume(AudioCategory.Ambient);
                this._AudioManager.SetCategoryVolume(AudioCategory.Music, -80f);
                this._AudioManager.SetCategoryVolume(AudioCategory.Ambient, -80f);
                break;
            case "unmute":
                this.RestoreVolume(AudioCategory.Music);
                this.RestoreVolume(AudioCategory.Ambient);
                break;
        }
    }

    private void CacheVolume(AudioCategory category)
    {
        if (this._AudioManager == null || this._OriginalVolumes.ContainsKey(category))
        {
            return;
        }

        this._OriginalVolumes[category] = this._AudioManager.GetCategoryVolume(category);
    }

    private void RestoreVolume(AudioCategory category)
    {
        if (this._AudioManager == null || !this._OriginalVolumes.TryGetValue(category, out float volume))
        {
            return;
        }

        this._AudioManager.SetCategoryVolume(category, volume);
        this._OriginalVolumes.Remove(category);
    }

    private void RestoreAllVolumes()
    {
        if (this._AudioManager == null || this._OriginalVolumes.Count == 0)
        {
            return;
        }

        var categories = new List<AudioCategory>(this._OriginalVolumes.Keys);
        foreach (AudioCategory category in categories)
        {
            this.RestoreVolume(category);
        }
    }

    private async void ProcessChoice(ChoiceOption selection)
    {
        this._AwaitingChoice = false;
        this._PromptLabel.Text = string.Empty;

        this._GameState.SceneData["stage6.choice"] = selection.Id;
        this._GameState.SceneData["stage6.choice_text"] = selection.Text ?? selection.Label ?? selection.Id;

        int nextIndex = selection.NextBlock;
        if (nextIndex < 0 || nextIndex > this._NarrativeData.StoryBlocks.Count)
        {
            this._CurrentBlockIndex = this._NarrativeData.StoryBlocks.Count;
        }
        else
        {
            this._CurrentBlockIndex = nextIndex;
        }

        await this.PlayFromCurrentBlockAsync().ConfigureAwait(false);
    }

    private ChoiceOption? ResolveChoice(string input)
    {
        if (!int.TryParse(input, out int index))
        {
            return null;
        }

        index -= 1;
        if (index < 0 || index >= this._ActiveChoices.Count)
        {
            return null;
        }

        return this._ActiveChoices[index];
    }

    private async Task HandleExitAsync()
    {
        string threadTag = _DreamweaverThreadTags.GetValueOrDefault(this._GameState.DreamweaverThread, "light");
        this._GameState.SceneData["stage6.alignment"] = threadTag;
        this._GameState.SceneData["stage6.completed"] = true;

        if (!string.IsNullOrWhiteSpace(this._NarrativeData.ExitLine))
        {
            await this.DisplayLineAsync(this._NarrativeData.ExitLine).ConfigureAwait(false);
        }

        await this.DisplayLineAsync("[center][i]Coming soon[/i][/center]").ConfigureAwait(false);
        await this.DelayAsync(3.5f).ConfigureAwait(false);
        this.RestoreAllVolumes();
        this._SceneManager.TransitionToScene("res://source/ui/menus/main_menu.tscn", false);
    }

    private bool TryLoadNarrativeData()
    {
        if (!Godot.FileAccess.FileExists(_DataPath))
        {
            GD.PrintErr($"[Stage6SystemLog] Narrative file missing at {_DataPath}");
            return false;
        }

        try
        {
            var payload = ConfigurationService.LoadConfiguration(_DataPath);
            if (!ConfigurationService.ValidateConfiguration(payload, _SchemaPath))
            {
                GD.PrintErr("[Stage6SystemLog] Schema validation failed for Stage 6 log.");
                return false;
            }

            // Initialize narrative data from payload
            this._NarrativeData = new NarrativeSceneData();
            return true;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[Stage6SystemLog] Failed to load Stage 6 log: {ex.Message}");
            return false;
        }
    }

    private async Task DisplayLineAsync(string text)
    {
        this._OutputLabel.AppendText("\n");
        foreach (char character in text)
        {
            this._OutputLabel.AppendText(character.ToString());
            await this.DelayAsync(this.CharacterDelaySeconds).ConfigureAwait(false);
        }
    }

    private async Task DelayAsync(float seconds)
    {
        if (seconds <= 0f)
        {
            return;
        }

        var timer = this.GetTree().CreateTimer(seconds);
        await this.ToSignal(timer, Godot.Timer.SignalName.Timeout);
    }

    private static string ExtractToken(string paragraph, string? key)
    {
        int colonIndex = paragraph.IndexOf(':');
        if (colonIndex < 0)
        {
            return string.Empty;
        }

        string content = paragraph[(colonIndex + 1)..].Trim().TrimEnd(']');
        if (string.IsNullOrEmpty(key))
        {
            return content;
        }

        int equalsIndex = content.IndexOf('=');
        if (equalsIndex < 0)
        {
            return content;
        }

        string tokenKey = content[..equalsIndex].Trim();
        string tokenValue = content[(equalsIndex + 1)..].Trim();
        return string.Equals(tokenKey, key, StringComparison.OrdinalIgnoreCase) ? tokenValue : string.Empty;
    }
}
