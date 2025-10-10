using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts;

/// <summary>
/// Presents the opening narrative terminal with a flexible prompt/choice system that content teams can extend via JSON.
/// FUTURE: Will integrate with DreamweaverSystem for LLM-powered dynamic narrative (see ADR-0003).
/// Integration points marked with // FUTURE: LLM_INTEGRATION comments.
/// </summary>
public partial class NarrativeTerminal : Control
{
    private enum PromptKind
    {
        None,
        InitialChoice,
        StoryChoice,
        Freeform,
        PlayerName,
        Secret
    }

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    private RichTextLabel _outputLabel = default!;
    private LineEdit _inputField = default!;
    private Button _submitButton = default!;
    private Label _promptLabel = default!;

    private SceneManager _sceneManager = default!;
    private GameState _gameState = default!;
    private NarratorEngine? _narratorEngine;

    // FUTURE: LLM_INTEGRATION - DreamweaverSystem connection
    // Will be set via GetNodeOrNull<DreamweaverSystem>("/root/DreamweaverSystem")
    // See ADR-0003 for complete integration architecture
    private DreamweaverSystem? _dreamweaverSystem = null;
    
    // FUTURE: LLM_INTEGRATION - Toggle for dynamic vs static narrative
    // When true and _dreamweaverSystem is available, use LLM responses
    // When false or _dreamweaverSystem is null, use static JSON (current behavior)
    [Export] 
    public bool UseDynamicNarrative { get; set; } = false;

    private NarrativeSceneData _sceneData = new();
    private PromptKind _currentPrompt = PromptKind.None;
    private IReadOnlyList<DreamweaverChoice> _threadChoices = Array.Empty<DreamweaverChoice>();
    private IReadOnlyList<ChoiceOption> _activeChoices = Array.Empty<ChoiceOption>();
    private bool _awaitingInput;
    private int _currentBlockIndex;

    // Dynamic narrative state
    private bool _useDynamicNarrative = false;
    private string _lastGeneratedNarrative = "";

    public override void _Ready()
    {
        _outputLabel = GetNode<RichTextLabel>("%OutputLabel");
        _inputField = GetNode<LineEdit>("%InputField");
        _submitButton = GetNode<Button>("%SubmitButton");
        _promptLabel = GetNode<Label>("%PromptLabel");

        _submitButton.Pressed += OnSubmitPressed;
        _inputField.TextSubmitted += _ => OnSubmitPressed();

        _sceneManager = GetNode<SceneManager>("/root/SceneManager");
        _gameState = GetNode<GameState>("/root/GameState");
        _narratorEngine = GetNodeOrNull<NarratorEngine>("/root/NarratorEngine");

        // FUTURE: LLM_INTEGRATION - Connect to DreamweaverSystem when available
        _dreamweaverSystem = GetNodeOrNull<DreamweaverSystem>("/root/DreamweaverSystem");
        if (_dreamweaverSystem != null)
        {
            // Connect to signals for dynamic narrative updates
            _dreamweaverSystem.Connect("NarrativeGenerated", new Callable(this, nameof(OnNarrativeGenerated)));
            _dreamweaverSystem.Connect("GenerationError", new Callable(this, nameof(OnGenerationError)));
            GD.Print("NarrativeTerminal: DreamweaverSystem connected for dynamic narrative");
        }

        _inputField.GrabFocus();
        CallDeferred(nameof(InitializeNarrativeAsync));
    }

    // Signal handlers for DreamweaverSystem
    private void OnNarrativeGenerated(string personaId, string generatedText)
    {
        GD.Print($"Dreamweaver narrative generated for {personaId}: {generatedText}");
        // Store the generated text for use in narrative display
        _lastGeneratedNarrative = generatedText;
    }

    private void OnGenerationError(string personaId, string errorMessage)
    {
        GD.PrintErr($"Dreamweaver generation error for {personaId}: {errorMessage}");
        // Fall back to static narrative when LLM fails
        _useDynamicNarrative = false;
    }

    private async void InitializeNarrativeAsync()
    {
        if (!TryLoadSceneData())
        {
            DisplayImmediate("[color=#ff5959]Unable to load terminal narrative. Please verify content files.[/color]");
            return;
        }

        await DisplayOpeningAsync();
        PresentInitialChoice();
    }

    private bool TryLoadSceneData()
    {
        string basePath = "res://Source/Data/scenes/scene1_narrative";
        string threadKey = _gameState.DreamweaverThread.ToString().ToLowerInvariant();
        var candidates = new[] { threadKey, "hero", "shadow", "ambition" };

        foreach (string candidate in candidates)
        {
            string path = $"{basePath}/{candidate}.json";
            if (!Godot.FileAccess.FileExists(path))
            {
                continue;
            }

            try
            {
                string json = Godot.FileAccess.GetFileAsString(path);
                NarrativeSceneData? data = JsonSerializer.Deserialize<NarrativeSceneData>(json, _jsonOptions);
                if (data == null)
                {
                    continue;
                }

                NormalizeNarrativeData(data);
                _sceneData = data;
                return true;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Failed to parse narrative data at {path}: {ex.Message}");
            }
        }

        GD.PrintErr("NarrativeTerminal: no valid data files found. Expected hero/shadow/ambition variants.");
        return false;
    }

    private void NormalizeNarrativeData(NarrativeSceneData data)
    {
        data.OpeningLines ??= new List<string>();
        data.StoryBlocks ??= new List<StoryBlock>();
        data.SecretQuestion ??= new SecretQuestion { Options = new List<string>() };

        if (data.InitialChoice != null)
        {
            data.InitialChoice.Options ??= new List<DreamweaverChoice>();
            foreach (DreamweaverChoice option in data.InitialChoice.Options)
            {
                if (!Enum.TryParse(option.Id, true, out DreamweaverThread parsedThread))
                {
                    parsedThread = DreamweaverThread.Hero;
                }

                option.Thread = parsedThread;
            }
            _threadChoices = data.InitialChoice.Options;
        }

        foreach (StoryBlock block in data.StoryBlocks)
        {
            block.Paragraphs ??= new List<string>();
            block.Choices ??= new List<ChoiceOption>();
        }
    }

    private async Task DisplayOpeningAsync()
    {
        if (UseDynamicNarrative && _dreamweaverSystem != null)
        {
            // Use dynamic narrative generation
            var personaId = _gameState.DreamweaverThread.ToString().ToLowerInvariant();
            var openingLine = await _dreamweaverSystem.GetOpeningLineAsync(personaId);
            await DisplayTextWithTypewriterAsync(openingLine);
        }
        else
        {
            // Use static JSON narrative (current behavior)
            foreach (string line in _sceneData.OpeningLines)
            {
                await DisplayTextWithTypewriterAsync(line);
            }
        }
    }

    private async void PresentInitialChoice()
    {
        if (_sceneData.InitialChoice == null || _threadChoices.Count == 0)
        {
            _currentBlockIndex = 0;
            PresentStoryBlock();
            return;
        }

        if (UseDynamicNarrative && _dreamweaverSystem != null)
        {
            // Use dynamic choice generation
            var personaId = _gameState.DreamweaverThread.ToString().ToLowerInvariant();
            var dynamicChoices = await _dreamweaverSystem.GenerateChoicesAsync(personaId, "initial choice");

            DisplayImmediate($"[b]{_sceneData.InitialChoice.Prompt}[/b]");

            for (int i = 0; i < dynamicChoices.Count; i++)
            {
                var choice = dynamicChoices[i];
                DisplayImmediate($"  {i + 1}. {choice.Text} — {choice.Description}");
            }

            _activeChoices = (IReadOnlyList<ChoiceOption>)dynamicChoices;
        }
        else
        {
            // Use static JSON choices (current behavior)
            DisplayImmediate($"[b]{_sceneData.InitialChoice.Prompt}[/b]");

            for (int i = 0; i < _threadChoices.Count; i++)
            {
                DreamweaverChoice option = _threadChoices[i];
                string label = option.Text ?? option.Id;
                DisplayImmediate($"  {i + 1}. {label} — {option.Description}");
            }

            _activeChoices = _threadChoices;
        }

        ConfigurePrompt(PromptKind.InitialChoice, "Choose your story thread (ex: 1 or hero)" );
    }

    private void PresentStoryBlock()
    {
        if (_currentBlockIndex < 0 || _currentBlockIndex >= _sceneData.StoryBlocks.Count)
        {
            PromptForName();
            return;
        }

        var block = _sceneData.StoryBlocks[_currentBlockIndex];

        _ = DisplayStoryBlockAsync(block);
    }

    private async Task DisplayStoryBlockAsync(StoryBlock block)
    {
        foreach (string paragraph in block.Paragraphs)
        {
            await DisplayTextWithTypewriterAsync(paragraph);
        }

        if (!string.IsNullOrEmpty(block.Question))
        {
            DisplayImmediate($"[b]{block.Question}[/b]");

            if (block.Choices.Count > 0)
            {
                _activeChoices = block.Choices;
                for (int i = 0; i < block.Choices.Count; i++)
                {
                    DisplayImmediate($"  {i + 1}. {block.Choices[i].Text}");
                }

                ConfigurePrompt(PromptKind.StoryChoice, "Select an option (number or text)" );
            }
            else
            {
                _activeChoices = Array.Empty<ChoiceOption>();
                ConfigurePrompt(PromptKind.Freeform, "Enter your response" );
            }
        }
        else
        {
            _currentBlockIndex++;
            PresentStoryBlock();
        }
    }

    private void PromptForName()
    {
        string prompt = string.IsNullOrWhiteSpace(_sceneData.NamePrompt)
            ? "What name should the terminal record?"
            : _sceneData.NamePrompt;

        DisplayImmediate($"[b]{prompt}[/b]");
        ConfigurePrompt(PromptKind.PlayerName, "Enter your name" );
    }

    private void PromptForSecret()
    {
        if (_sceneData.SecretQuestion == null)
        {
            CompleteNarrativeSceneAsync();
            return;
        }

        DisplayImmediate($"[b]{_sceneData.SecretQuestion.Prompt}[/b]");

        if (_sceneData.SecretQuestion.Options.Count > 0)
        {
            for (int i = 0; i < _sceneData.SecretQuestion.Options.Count; i++)
            {
                DisplayImmediate($"  {i + 1}. {_sceneData.SecretQuestion.Options[i]}");
            }
        }

        ConfigurePrompt(PromptKind.Secret, "Share your secret (number or text)" );
    }

    private void ConfigurePrompt(PromptKind kind, string placeholder)
    {
        _currentPrompt = kind;
        _awaitingInput = true;
        _inputField.PlaceholderText = placeholder;
        _promptLabel.Text = placeholder;
        _inputField.Text = string.Empty;
        _inputField.GrabFocus();
    }

    private void OnSubmitPressed()
    {
        if (!_awaitingInput)
        {
            return;
        }

        string rawInput = _inputField.Text.Trim();
        _inputField.Text = string.Empty;

        if (string.IsNullOrEmpty(rawInput))
        {
            return;
        }

        switch (_currentPrompt)
        {
            case PromptKind.InitialChoice:
                HandleThreadSelection(rawInput);
                break;
            case PromptKind.StoryChoice:
                HandleStoryChoice(rawInput);
                break;
            case PromptKind.Freeform:
                HandleFreeformResponse(rawInput);
                break;
            case PromptKind.PlayerName:
                HandlePlayerName(rawInput);
                break;
            case PromptKind.Secret:
                HandleSecret(rawInput);
                break;
        }
    }

    private void HandleThreadSelection(string input)
    {
        DreamweaverChoice? choice = ResolveThreadChoice(input);
        if (choice == null)
        {
            DisplayImmediate("[color=#ffae42]Please choose a valid thread (number or hero/shadow/ambition).[/color]");
            return;
        }

        _sceneManager.SetDreamweaverThread(choice.Id);
    _narratorEngine?.AddDialogue($"Thread locked: {choice.Text}");
        DisplayImmediate($"You lean toward the {choice.Text} path: {choice.Description}");

        _gameState.DreamweaverThread = choice.Thread;

        _awaitingInput = false;
        _currentPrompt = PromptKind.None;
        _currentBlockIndex = 0;

        PresentStoryBlock();
    }

    private DreamweaverChoice? ResolveThreadChoice(string input)
    {
        if (_threadChoices.Count == 0)
        {
            return null;
        }

        if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int index))
        {
            index -= 1;
            if (index >= 0 && index < _threadChoices.Count)
            {
                return _threadChoices[index];
            }
        }

        foreach (DreamweaverChoice option in _threadChoices)
        {
            if (option.Id.Equals(input, StringComparison.OrdinalIgnoreCase) ||
                (option.Text != null && option.Text.Equals(input, StringComparison.OrdinalIgnoreCase)))
            {
                return option;
            }
        }

        return null;
    }

    private void HandleStoryChoice(string input)
    {
        ChoiceOption? selection = ResolveChoiceOption(input);
        if (selection == null)
        {
            DisplayImmediate("[color=#ffae42]That option is unavailable. Try the listed number or text.[/color]");
            return;
        }

        _awaitingInput = false;
        _currentPrompt = PromptKind.None;

        int nextBlock = Mathf.Clamp(selection.NextBlock, 0, _sceneData.StoryBlocks.Count);
        if (nextBlock == _currentBlockIndex)
        {
            _currentBlockIndex++;
        }
        else
        {
            _currentBlockIndex = nextBlock;
        }

        PresentStoryBlock();
    }

    private ChoiceOption? ResolveChoiceOption(string input)
    {
        if (_activeChoices.Count == 0)
        {
            return null;
        }

        if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int index))
        {
            index -= 1;
            if (index >= 0 && index < _activeChoices.Count)
            {
                return _activeChoices[index];
            }
        }

        foreach (ChoiceOption option in _activeChoices)
        {
            if (option.Text.Equals(input, StringComparison.OrdinalIgnoreCase))
            {
                return option;
            }
        }

        return null;
    }

    private void HandleFreeformResponse(string input)
    {
        RecordSceneResponse($"block-{_currentBlockIndex}-response", input);
        _awaitingInput = false;
        _currentPrompt = PromptKind.None;
        _currentBlockIndex++;
        PresentStoryBlock();
    }

    private void HandlePlayerName(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            DisplayImmediate("[color=#ff5959]Please provide a name for the archives.[/color]");
            return;
        }

        _sceneManager.SetPlayerName(input);
        DisplayImmediate($"Identity confirmed: [b]{input}[/b].");

        _awaitingInput = false;
        _currentPrompt = PromptKind.None;

        PromptForSecret();
    }

    private void HandleSecret(string input)
    {
        if (_sceneData.SecretQuestion != null && _sceneData.SecretQuestion.Options.Count > 0)
        {
            if (int.TryParse(input, out int index))
            {
                index -= 1;
                if (index >= 0 && index < _sceneData.SecretQuestion.Options.Count)
                {
                    input = _sceneData.SecretQuestion.Options[index];
                }
            }
        }

        RecordSceneResponse("secret", input);
        DisplayImmediate("A fragment has been secured in the archive.");

        _awaitingInput = false;
        _currentPrompt = PromptKind.None;

        CompleteNarrativeSceneAsync();
    }

    private void RecordSceneResponse(string key, string value)
    {
        string compositeKey = $"scene1_narrative.{key}";
        if (_gameState.SceneData.ContainsKey(compositeKey))
        {
            _gameState.SceneData[compositeKey] = value;
        }
        else
        {
            _gameState.SceneData.Add(compositeKey, value);
        }
    }

    private async void CompleteNarrativeSceneAsync()
    {
        if (!string.IsNullOrWhiteSpace(_sceneData.ExitLine))
        {
            await DisplayTextWithTypewriterAsync(_sceneData.ExitLine);
        }

        await DisplayTextWithTypewriterAsync("Moving to the next part of your journey...");

        _sceneManager.UpdateCurrentScene(2);
    var timer = new Godot.Timer { WaitTime = 2.5f, OneShot = true };
    timer.Timeout += () => _sceneManager.TransitionToScene("Scene2NethackSequence");
    AddChild(timer);
    timer.Start();
    }

    private void DisplayImmediate(string text)
    {
        if (_outputLabel != null)
        {
            _outputLabel.AppendText(text + "\n");
        }
        else
        {
            GD.Print(text);
        }
    }

    private async Task DisplayTextWithTypewriterAsync(string text)
    {
        _outputLabel.AppendText("\n");
        foreach (char character in text)
        {
            _outputLabel.AppendText(character.ToString());
            await ToSignal(GetTree().CreateTimer(0.025f), Godot.Timer.SignalName.Timeout);
        }

        _outputLabel.AppendText("\n");
    }

    // ============================================================================
    // FUTURE: LLM_INTEGRATION - Dreamweaver Consultation Methods
    // ============================================================================
    // These methods will be implemented when DreamweaverSystem is integrated
    // See ADR-0003: docs/adr/adr-0003-nobodywho-llm-integration.md
    // ============================================================================

    /// <summary>
    /// FUTURE: Consults all three Dreamweavers (Hero, Shadow, Ambition) + Omega narrator
    /// based on player situation/choice. Will replace or augment static JSON responses.
    /// </summary>
    /// <param name="situation">Player's current situation or choice context</param>
    // private void ConsultDreamweavers(string situation)
    // {
    //     if (_dreamweaverSystem != null && UseDynamicNarrative)
    //     {
    //         // Use LLM-powered Dreamweavers for dynamic narrative
    //         _dreamweaverSystem.Call("ConsultAllDreamweavers", situation);
    //     }
    //     else
    //     {
    //         // Fallback to static JSON narrative (current behavior)
    //         // Continue with existing story block display logic
    //     }
    // }

    /// <summary>
    /// FUTURE: Handles response from DreamweaverSystem after all personas have responded.
    /// Receives Hero, Shadow, Ambition, and Omega responses in JSON format.
    /// </summary>
    /// <param name="consultations">Dictionary with hero/shadow/ambition/omega JSON responses</param>
    // private void OnDreamweaversConsultation(Godot.Collections.Dictionary consultations)
    // {
    //     // Parse JSON responses from each Dreamweaver
    //     // string heroJson = consultations["hero"].AsString();
    //     // string shadowJson = consultations["shadow"].AsString();
    //     // string ambitionJson = consultations["ambition"].AsString();
    //     // string omegaJson = consultations["omega"].AsString();
    //     
    //     // Display formatted Dreamweaver guidance in terminal
    //     // DisplayDreamweaverConsultation(heroJson, shadowJson, ambitionJson, omegaJson);
    //     
    //     // Update game state based on player's alignment with each Dreamweaver
    //     // Continue narrative flow
    // }

    /// <summary>
    /// FUTURE: Formats and displays LLM-generated Dreamweaver consultations.
    /// Parses JSON and presents in terminal-style format.
    /// </summary>
    // private void DisplayDreamweaverConsultation(string heroJson, string shadowJson, string ambitionJson, string omegaJson)
    // {
    //     // Parse each JSON response according to schema defined in ADR-0003:
    //     // Hero: {advice, challenge, moral}
    //     // Shadow: {whisper, secret, cost}
    //     // Ambition: {strategy, goal, reward}
    //     // Omega: {narration, choice_context, consequence}
    //     
    //     // Display formatted output in terminal
    // }
}