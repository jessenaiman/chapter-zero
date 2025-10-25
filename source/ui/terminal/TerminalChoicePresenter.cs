using Godot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Ui.Terminal;

/// <summary>
/// Implementation of terminal choice presenter.
/// Manages choice display, user selection, and interaction handling.
/// Includes comprehensive logging for debugging orphan nodes and state issues.
/// </summary>
public class TerminalChoicePresenter : ITerminalChoicePresenter, IDisposable
{
    private readonly VBoxContainer _ChoiceContainer;
    private readonly List<Button> _ChoiceButtons = new();
    private TaskCompletionSource<List<int>>? _SelectionTaskSource;
    private bool _AllowMultipleSelection;
    private bool _Disposed;

    /// <summary>
    /// Initializes a new instance of the TerminalChoicePresenter.
    /// </summary>
    /// <param name="choiceContainer">The VBoxContainer to hold choice buttons.</param>
    /// <exception cref="ArgumentNullException">Thrown when choiceContainer is null.</exception>
    public TerminalChoicePresenter(VBoxContainer choiceContainer)
    {
        _ChoiceContainer = choiceContainer ?? throw new ArgumentNullException(nameof(choiceContainer));
        GD.PrintRich($"[color=green][TerminalChoicePresenter][/color] Initialized with container: {_ChoiceContainer.Name}");
    }

    /// <inheritdoc/>
    public async Task<List<int>> PresentChoicesAsync(IList<string> choices, bool allowMultipleSelection = false)
    {
        try
        {
            if (choices == null)
                throw new ArgumentNullException(nameof(choices));

            if (choices.Count == 0)
                throw new ArgumentException("Choices list cannot be empty", nameof(choices));

            GD.PrintRich($"[color=cyan][TerminalChoicePresenter.PresentChoicesAsync][/color] Presenting {choices.Count} string choices");

            _AllowMultipleSelection = allowMultipleSelection;
            _SelectionTaskSource = new TaskCompletionSource<List<int>>();

            // Clear existing choices
            ClearChoiceButtons();

            // Create choice buttons
            for (int i = 0; i < choices.Count; i++)
            {
                var button = CreateChoiceButton(choices[i], i);
                _ChoiceContainer.AddChild(button);
                _ChoiceButtons.Add(button);
                GD.PrintRich($"[color=yellow]  → Created button {i}: '{choices[i]}'[/color]");
            }

            GD.PrintRich($"[color=cyan]  Ready to wait for selection from {_ChoiceButtons.Count} buttons[/color]");

            // Wait for user selection
            var result = await _SelectionTaskSource.Task.ConfigureAwait(false);
            GD.PrintRich($"[color=green]  Selection received: indices={string.Join(",", result)}[/color]");
            return result;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[TerminalChoicePresenter.PresentChoicesAsync] Exception: {ex.GetType().Name} - {ex.Message}");
            throw;
        }
    }

    /// <inheritdoc/>
    /// <remarks>See <see cref="TerminalChoiceOption"/> for choice option structure.</remarks>
    public async Task<int> PresentChoicesAsync(IList<TerminalChoiceOption> choiceOptions)
    {
        try
        {
            if (choiceOptions == null)
                throw new ArgumentNullException(nameof(choiceOptions));

            if (choiceOptions.Count == 0)
                throw new ArgumentException("Choice options list cannot be empty", nameof(choiceOptions));

            GD.PrintRich($"[color=cyan][TerminalChoicePresenter.PresentChoicesAsync][/color] Presenting {choiceOptions.Count} TerminalChoiceOption choices");

            _AllowMultipleSelection = false; // Single selection for ChoiceOption
            _SelectionTaskSource = new TaskCompletionSource<List<int>>();

            // Clear existing choices
            ClearChoiceButtons();

            // Create choice buttons with options
            for (int i = 0; i < choiceOptions.Count; i++)
            {
                var button = CreateChoiceButton(choiceOptions[i], i);
                _ChoiceContainer.AddChild(button);
                _ChoiceButtons.Add(button);
                GD.PrintRich($"[color=yellow]  → Created button {i}: '{choiceOptions[i].Text}'[/color]");
            }

            GD.PrintRich($"[color=cyan]  Ready to wait for selection from {_ChoiceButtons.Count} buttons[/color]");

            // Wait for user selection and return single index
            var result = await _SelectionTaskSource.Task.ConfigureAwait(false);
            var selectedIndex = result.Count > 0 ? result[0] : -1;
            GD.PrintRich($"[color=green]  Selection received: index={selectedIndex}[/color]");
            return selectedIndex;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[TerminalChoicePresenter.PresentChoicesAsync] Exception: {ex.GetType().Name} - {ex.Message}");
            throw;
        }
    }

    /// <inheritdoc/>
    public void HideChoices()
    {
        try
        {
            GD.PrintRich($"[color=orange][TerminalChoicePresenter.HideChoices][/color] Clearing {_ChoiceButtons.Count} buttons");
            ClearChoiceButtons();
            _SelectionTaskSource?.TrySetCanceled();
            _SelectionTaskSource = null;
            GD.PrintRich($"[color=green]  ✓ Choices hidden successfully[/color]");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[TerminalChoicePresenter.HideChoices] Exception: {ex.GetType().Name} - {ex.Message}");
            throw;
        }
    }

    /// <inheritdoc/>
    public int GetSelectedChoiceIndex()
    {
        // This is a simplified implementation
        // In a real scenario, this would track the currently highlighted choice
        return -1;
    }

    /// <inheritdoc/>
    public void SetChoiceNavigationEnabled(bool enabled)
    {
        try
        {
            GD.PrintRich($"[color=cyan][TerminalChoicePresenter.SetChoiceNavigationEnabled][/color] Setting navigation {(enabled ? "enabled" : "disabled")} for {_ChoiceButtons.Count} buttons");
            foreach (var button in _ChoiceButtons)
            {
                button.Disabled = !enabled;
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[TerminalChoicePresenter.SetChoiceNavigationEnabled] Exception: {ex.GetType().Name} - {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public bool AreChoicesVisible()
    {
        var visible = _ChoiceButtons.Count > 0;
        GD.PrintRich($"[color=blue][TerminalChoicePresenter.AreChoicesVisible][/color] → {visible} ({_ChoiceButtons.Count} buttons)");
        return visible;
    }

    /// <summary>
    /// Simulates choice selection for testing purposes.
    /// Completes the pending choice task with the specified index.
    /// </summary>
    /// <param name="index">The choice index to select.</param>
    public void SimulateChoiceSelection(int index)
    {
        GD.PrintRich($"[color=magenta][TerminalChoicePresenter.SimulateChoiceSelection][/color] Simulating selection of index {index}");
        OnChoiceSelected(index);
    }

    /// <summary>
    /// Creates a choice button with the specified text and index.
    /// </summary>
    /// <param name="text">The button text.</param>
    /// <param name="index">The choice index.</param>
    /// <returns>The created button.</returns>
    private Button CreateChoiceButton(string text, int index)
    {
        try
        {
            var button = new Button();
            button.Text = text;
            button.Connect(Button.SignalName.Pressed, Callable.From(() => OnChoiceSelected(index)));
            return button;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[TerminalChoicePresenter.CreateChoiceButton] Failed to create button for '{text}': {ex.GetType().Name} - {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Creates a choice button with the specified options.
    /// </summary>
    /// <param name="option">The choice option configuration.</param>
    /// <param name="index">The choice index.</param>
    /// <returns>The created button.</returns>
    private Button CreateChoiceButton(TerminalChoiceOption option, int index)
    {
        var button = new Button();
        button.Text = option.Text;

        // Apply color if specified
        if (option.TextColor.HasValue)
        {
            button.AddThemeColorOverride("font_color", option.TextColor.Value);
        }

        button.Connect(Button.SignalName.Pressed, Callable.From(() => OnChoiceSelected(index)));
        return button;
    }

    /// <summary>
    /// Handles choice selection by the user.
    /// </summary>
    /// <param name="index">The selected choice index.</param>
    private void OnChoiceSelected(int index)
    {
        try
        {
            GD.PrintRich($"[color=magenta][TerminalChoicePresenter.OnChoiceSelected][/color] Choice {index} selected");

            if (_SelectionTaskSource != null && !_SelectionTaskSource.Task.IsCompleted)
            {
                var selectedIndices = _AllowMultipleSelection
                    ? new List<int> { index } // For now, single selection even in multi-mode
                    : new List<int> { index };

                var result = _SelectionTaskSource.TrySetResult(selectedIndices);
                GD.PrintRich($"[color=green]  ✓ Selection processed, TrySetResult={result}[/color]");
            }
            else
            {
                GD.Print($"[TerminalChoicePresenter.OnChoiceSelected] No active selection task or task already completed");
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[TerminalChoicePresenter.OnChoiceSelected] Exception: {ex.GetType().Name} - {ex.Message}");
        }
    }

    /// <summary>
    /// Clears all choice buttons from the container.
    /// Logs orphan node detection and cleanup information.
    /// Uses a comprehensive cleanup strategy to prevent orphan nodes.
    /// </summary>
    private void ClearChoiceButtons()
    {
        try
        {
            GD.PrintRich($"[color=orange][TerminalChoicePresenter.ClearChoiceButtons][/color] Clearing {_ChoiceButtons.Count} buttons from container");

            // First pass: disable buttons and clear any pending interactions
            foreach (var button in _ChoiceButtons)
            {
                try
                {
                    if (button.IsNodeReady())
                    {
                        button.Disabled = true;
                        button.ReleaseFocus();
                        GD.PrintRich($"[color=yellow]  → Disabled and cleared button focus[/color]");
                    }
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"[TerminalChoicePresenter.ClearChoiceButtons] Failed to disable button: {ex.GetType().Name}");
                }
            }

            // Second pass: remove all children from container
            foreach (var button in _ChoiceButtons)
            {
                try
                {
                    if (button.IsNodeReady() && button.GetParent() == _ChoiceContainer)
                    {
                        _ChoiceContainer.RemoveChild(button);
                        GD.PrintRich($"[color=yellow]  → Removed button from container[/color]");
                    }
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"[TerminalChoicePresenter.ClearChoiceButtons] Failed to remove button: {ex.GetType().Name} - {ex.Message}");
                }
            }

            // Third pass: free all buttons
            foreach (var button in _ChoiceButtons)
            {
                try
                {
                    if (button.IsNodeReady())
                    {
                        button.QueueFree();
                        GD.PrintRich($"[color=yellow]  → Queued button for deletion[/color]");
                    }
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"[TerminalChoicePresenter.ClearChoiceButtons] Failed to free button: {ex.GetType().Name} - {ex.Message}");
                }
            }

            _ChoiceButtons.Clear();
            GD.PrintRich($"[color=green]  ✓ All {_ChoiceButtons.Count} buttons cleared[/color]");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[TerminalChoicePresenter.ClearChoiceButtons] Exception: {ex.GetType().Name} - {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        GD.PrintRich($"[color=orange][TerminalChoicePresenter.Dispose][/color] Called");
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the presenter and cleans up resources.
    /// </summary>
    /// <param name="disposing">Whether this is being called from Dispose() or finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_Disposed)
        {
            if (disposing)
            {
                GD.PrintRich($"[color=cyan]  → Disposing managed resources[/color]");
                HideChoices();
            }
            GD.PrintRich($"[color=green]  ✓ Dispose completed[/color]");
            _Disposed = true;
        }
    }
}
