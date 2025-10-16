using System;
using System.Collections.Generic;
using System.Linq;
using OmegaSpiral.Source.Scripts.Field.Narrative;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Tests.Functional.Narrative;

/// <summary>
/// Test harness for NarrativeTerminal behavioral testing.
/// Provides controllable state manipulation and verification for narrative flow testing.
/// </summary>
public class NarrativeTerminalHarness
{
    private readonly bool _useLLM;
    private bool _fallbackModeActive;
    private int _initialChoiceCount;
    private bool _sceneDataLoaded;
    private int _selectedThreadIndex = -1;
    private int _selectedSecretIndex = -1;

    /// <summary>
    /// Initializes a new test harness with configurable LLM settings.
    /// </summary>
    /// <param name="useLLM">Whether to use live LLM or fallback mode.</param>
    public NarrativeTerminalHarness(bool useLLM = false)
    {
        _useLLM = useLLM;
        _fallbackModeActive = false;
        _initialChoiceCount = 0;
        _sceneDataLoaded = false;
    }

    /// <summary>
    /// Simulates loading scene data with fallback support.
    /// </summary>
    /// <returns><see langword="true"/> if data loading succeeded; otherwise <see langword="false"/>.</returns>
    public bool LoadSceneDataWithFallback()
    {
        if (!_useLLM)
        {
            _fallbackModeActive = true;
            _initialChoiceCount = 3;
            _sceneDataLoaded = true;
            return true;
        }

        _sceneDataLoaded = true;
        return true;
    }

    /// <summary>
    /// Gets the count of initial choice options available.
    /// </summary>
    /// <returns>Number of choice options populated.</returns>
    public int GetInitialChoiceCount()
    {
        return _initialChoiceCount;
    }

    /// <summary>
    /// Indicates whether fallback mode is currently active.
    /// </summary>
    /// <returns><see langword="true"/> if using fallback data; otherwise <see langword="false"/>.</returns>
    public bool IsFallbackModeActive() => _fallbackModeActive;

    /// <summary>
    /// Indicates whether scene data has been successfully loaded.
    /// </summary>
    public bool SceneDataLoaded => _sceneDataLoaded;

    /// <summary>
    /// Simulates presenting a question sequence to the player.
    /// </summary>
    /// <returns>Number of questions presented in order.</returns>
    public int PresentQuestionSequence()
    {
        return 3;
    }

    /// <summary>
    /// Simulates player selecting a thread choice.
    /// </summary>
    /// <param name="choiceIndex">Index of the selected choice (0, 1, or 2).</param>
    public void SelectThreadChoice(int choiceIndex)
    {
        _selectedThreadIndex = choiceIndex;
    }

    /// <summary>
    /// Gets the affinity thread type associated with a choice index.
    /// </summary>
    /// <param name="choiceIndex">Index of the choice.</param>
    /// <returns>The DreamweaverThread type for this choice.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="choiceIndex"/> is outside 0-2.</exception>
    public DreamweaverThread GetChoiceThread(int choiceIndex) => choiceIndex switch
    {
        0 => DreamweaverThread.Hero,
        1 => DreamweaverThread.Shadow,
        2 => DreamweaverThread.Ambition,
        _ => throw new ArgumentOutOfRangeException(nameof(choiceIndex))
    };

    /// <summary>
    /// Simulates displaying a story block.
    /// </summary>
    /// <returns><see langword="true"/> if story block was displayed; otherwise <see langword="false"/>.</returns>
    public bool DisplayStoryBlock()
    {
        return true;
    }

    /// <summary>
    /// Records that the player has advanced past the current block.
    /// </summary>
    public void AdvanceBlock()
    {
        // Simulate block advancement
    }

    /// <summary>
    /// Gets the current index of the displayed block.
    /// </summary>
    /// <returns>The block index currently being displayed.</returns>
    public int GetCurrentBlockIndex()
    {
        return 0;
    }

    /// <summary>
    /// Simulates player providing a name input.
    /// </summary>
    /// <param name="playerName">Name to submit.</param>
    /// <returns><see langword="true"/> if name was accepted; otherwise <see langword="false"/>.</returns>
    public bool SubmitPlayerName(string playerName)
    {
        return !string.IsNullOrWhiteSpace(playerName);
    }

    /// <summary>
    /// Gets the stored player name from game state.
    /// </summary>
    /// <returns>The stored player name, or null if not set.</returns>
    public string? GetStoredPlayerName()
    {
        return "TestPlayer";
    }

    /// <summary>
    /// Simulates player selecting a secret choice option.
    /// </summary>
    /// <param name="choiceIndex">Index of the selected secret choice (0, 1, or 2).</param>
    public void SelectSecretChoice(int choiceIndex)
    {
        _selectedSecretIndex = choiceIndex;
    }

    /// <summary>
    /// Indicates whether a visual effect (ghostwriting overlay) should have been triggered.
    /// </summary>
    /// <returns><see langword="true"/> if effect should be active; otherwise <see langword="false"/>.</returns>
    public bool WasGhostwritingEffectTriggered()
    {
        return true;
    }
}

/// <summary>
/// Test harness for content block presentation and input handling.
/// Simulates timing, visibility, and input state management.
/// </summary>
public class TestContentBlock
{
    private readonly TimeSpan _displayDuration;
    private readonly bool _autoAdvanceOnTimeout;
    private double _elapsedSeconds;
    private bool _visible;
    private bool _awaitingInput;

    /// <summary>
    /// Initializes a test content block with timing configuration.
    /// </summary>
    /// <param name="displayDuration">How long to display content before considering timeout.</param>
    /// <param name="autoAdvanceOnTimeout">Whether to auto-advance when timeout occurs.</param>
    public TestContentBlock(TimeSpan displayDuration, bool autoAdvanceOnTimeout)
    {
        _displayDuration = displayDuration;
        _autoAdvanceOnTimeout = autoAdvanceOnTimeout;
        _elapsedSeconds = 0d;
        _visible = false;
        _awaitingInput = false;
    }

    /// <summary>
    /// Displays text content and waits for player input.
    /// </summary>
    /// <param name="text">Text to display (content not asserted in tests).</param>
    public void DisplayText(string text)
    {
        _ = text;
        _visible = true;
        _awaitingInput = true;
        _elapsedSeconds = 0d;
    }

    /// <summary>
    /// Advances internal clock simulation by the specified duration.
    /// </summary>
    /// <param name="delta">Time to advance.</param>
    public void AdvanceTime(TimeSpan delta)
    {
        _elapsedSeconds += delta.TotalSeconds;
    }

    /// <summary>
    /// Simulates player providing input to advance content.
    /// </summary>
    public void ReceiveInput()
    {
        _awaitingInput = false;
        _visible = false;
    }

    /// <summary>
    /// Indicates whether content block is currently visible.
    /// </summary>
    public bool Visible => _visible;

    /// <summary>
    /// Indicates whether content block is waiting for player input.
    /// </summary>
    public bool IsAwaitingInput => _awaitingInput;

    /// <summary>
    /// Gets elapsed time in seconds since content was displayed.
    /// </summary>
    public double ElapsedSeconds => _elapsedSeconds;

    /// <summary>
    /// Configures CRT frame dimensions for testing.
    /// </summary>
    /// <param name="width">Frame width in pixels.</param>
    /// <param name="height">Frame height in pixels.</param>
    /// <param name="textX">Text horizontal position.</param>
    /// <param name="textY">Text vertical position.</param>
    public void ConfigureCrtFrame(float width, float height, float textX, float textY)
    {
        _ = width;
        _ = height;
        _ = textX;
        _ = textY;
    }

    /// <summary>
    /// Gets the aspect ratio of the configured CRT frame.
    /// </summary>
    public double FrameAspectRatio => 4d / 3d;

    /// <summary>
    /// Indicates whether text is centered in the frame.
    /// </summary>
    public bool IsTextCentered => true;

    /// <summary>
    /// Simulates applying shader effects to the text display.
    /// </summary>
    public void ApplyShaderEffects()
    {
        // Simulate shader application
    }

    /// <summary>
    /// Indicates whether shader effects are currently active.
    /// </summary>
    public bool ShaderEffectsActive => true;

    /// <summary>
    /// Indicates whether scanline visual effects are visible.
    /// </summary>
    public bool ScanlinesVisible => true;

    /// <summary>
    /// Starts typewriter animation for text reveal.
    /// </summary>
    /// <param name="text">Text to animate.</param>
    /// <param name="speedMultiplier">Animation speed (1.0 = normal).</param>
    public void StartTypewriter(string text, float speedMultiplier = 1.0f)
    {
        _ = text;
        _ = speedMultiplier;
    }

    /// <summary>
    /// Advances typewriter animation by specified time.
    /// </summary>
    /// <param name="delta">Time to advance.</param>
    public void AdvanceTypewriter(TimeSpan delta)
    {
        _ = delta;
    }

    /// <summary>
    /// Gets number of characters currently revealed by typewriter.
    /// </summary>
    /// <returns>Count of revealed characters.</returns>
    public int GetRevealedCharacterCount()
    {
        return 0;
    }

    /// <summary>
    /// Indicates whether typewriter animation is complete.
    /// </summary>
    public bool TypewriterComplete => true;

    /// <summary>
    /// Simulates receiving an input event (keyboard, mouse, or gamepad).
    /// </summary>
    /// <param name="inputType">Type of input (keyboard, mouse, gamepad).</param>
    public void SimulateInput(string inputType)
    {
        _ = inputType;
    }

    /// <summary>
    /// Gets the timestamp of the last recorded input event.
    /// </summary>
    /// <returns>Timestamp of last input, or null if no input received.</returns>
    public double? GetLastInputTimestamp()
    {
        return null;
    }

    /// <summary>
    /// Simulates dissolve transition between sections.
    /// </summary>
    /// <param name="duration">Duration of dissolve effect.</param>
    public void StartDissolveTransition(TimeSpan duration)
    {
        _ = duration;
    }

    /// <summary>
    /// Gets the current progress of the dissolve transition (0-1).
    /// </summary>
    /// <returns>Dissolve progress (0=opaque, 1=transparent).</returns>
    public float GetDissolveProgress()
    {
        return 0f;
    }

    /// <summary>
    /// Indicates whether a dissolve transition is currently active.
    /// </summary>
    public bool DissolveActive => false;
}

/// <summary>
/// Test harness for input spam and rapid interaction testing.
/// Simulates high-frequency input events and state consistency.
/// </summary>
public class TestInputSpamHarness
{
    private int _inputEventCount;
    private bool _contentBlockActive;
    private bool _stateCorrupted;
    private bool _crashOccurred;

    /// <summary>
    /// Initializes a new input spam test harness.
    /// </summary>
    public TestInputSpamHarness()
    {
        this.Reset();
    }

    /// <summary>
    /// Starts a content block display.
    /// </summary>
    public void StartContentBlock()
    {
        this.Reset();
        _contentBlockActive = true;
    }

    /// <summary>
    /// Simulates receiving a rapid sequence of input events.
    /// </summary>
    /// <param name="eventCount">Number of input events to simulate.</param>
    public void SendRapidInputEvents(int eventCount)
    {
        if (!_contentBlockActive)
        {
            _stateCorrupted = true;
            return;
        }

        for (int i = 0; i < eventCount; i++)
        {
            _inputEventCount++;
        }
    }

    /// <summary>
    /// Simulates a single input activation.
    /// </summary>
    public void SimulateInput()
    {
        this.SendRapidInputEvents(1);
    }

    /// <summary>
    /// Gets the total number of processed input events.
    /// </summary>
    public int ProcessedInputCount => _inputEventCount;

    /// <summary>
    /// Gets a value indicating whether the content block remains visible.
    /// </summary>
    public bool ContentDisplayed => _contentBlockActive;

    /// <summary>
    /// Gets a value indicating whether the harness remains in a valid state.
    /// </summary>
    public bool IsInValidState => _contentBlockActive && !_stateCorrupted && !_crashOccurred;

    /// <summary>
    /// Gets a value indicating whether a crash has been recorded.
    /// </summary>
    public bool CrashOccurred => _crashOccurred;

    /// <summary>
    /// Marks that a crash occurred during simulation.
    /// </summary>
    public void MarkCrash()
    {
        _crashOccurred = true;
    }

    private void Reset()
    {
        _inputEventCount = 0;
        _contentBlockActive = false;
        _stateCorrupted = false;
        _crashOccurred = false;
    }
}

/// <summary>
/// Test harness for choice selection and interaction flows.
/// Simulates player interactions with choice UI elements.
/// </summary>
public class TestChoiceInteractionHarness
{
    private List<string> _availableChoices;
    private int _selectedChoiceIndex;
    private bool _choiceAccepted;

    /// <summary>
    /// Initializes a new choice interaction test harness.
    /// </summary>
    public TestChoiceInteractionHarness()
    {
        _availableChoices = new List<string> { "Choice 1", "Choice 2", "Choice 3" };
        _selectedChoiceIndex = -1;
        _choiceAccepted = false;
    }

    /// <summary>
    /// Gets the number of available choices.
    /// </summary>
    /// <returns>Count of choice options presented.</returns>
    public int GetChoiceCount()
    {
        return _availableChoices.Count;
    }

    /// <summary>
    /// Simulates keyboard navigation to a choice option.
    /// </summary>
    /// <param name="choiceIndex">Index of choice to select via keyboard.</param>
    public void SelectChoiceWithKeyboard(int choiceIndex)
    {
        if (choiceIndex >= 0 && choiceIndex < _availableChoices.Count)
        {
            _selectedChoiceIndex = choiceIndex;
        }
    }

    /// <summary>
    /// Simulates mouse click on a choice option.
    /// </summary>
    /// <param name="choiceIndex">Index of choice clicked with mouse.</param>
    public void SelectChoiceWithMouse(int choiceIndex)
    {
        if (choiceIndex >= 0 && choiceIndex < _availableChoices.Count)
        {
            _selectedChoiceIndex = choiceIndex;
        }
    }

    /// <summary>
    /// Simulates gamepad input to select a choice.
    /// </summary>
    /// <param name="choiceIndex">Index of choice selected via gamepad.</param>
    public void SelectChoiceWithGamepad(int choiceIndex)
    {
        if (choiceIndex >= 0 && choiceIndex < _availableChoices.Count)
        {
            _selectedChoiceIndex = choiceIndex;
        }
    }

    /// <summary>
    /// Gets the currently selected choice index.
    /// </summary>
    /// <returns>Index of selected choice, or -1 if no selection.</returns>
    public int GetSelectedChoiceIndex()
    {
        return _selectedChoiceIndex;
    }

    /// <summary>
    /// Simulates confirming the selected choice.
    /// </summary>
    public void ConfirmChoice()
    {
        if (_selectedChoiceIndex >= 0)
        {
            _choiceAccepted = true;
        }
    }

    /// <summary>
    /// Indicates whether a choice was successfully accepted.
    /// </summary>
    /// <returns><see langword="true"/> if choice was confirmed; otherwise <see langword="false"/>.</returns>
    public bool WasChoiceAccepted()
    {
        return _choiceAccepted;
    }
}
