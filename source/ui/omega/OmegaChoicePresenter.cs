using Godot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// Implementation of Omega UI choice presenter.
/// Manages choice display, user selection, and interaction handling.
/// </summary>
public class OmegaChoicePresenter : IOmegaChoicePresenter, IDisposable
{
    private readonly VBoxContainer _choiceContainer;
    private readonly List<Button> _choiceButtons = new();
    private TaskCompletionSource<List<int>>? _selectionTaskSource;
    private bool _allowMultipleSelection;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="OmegaChoicePresenter"/> class.
    /// </summary>
    /// <param name="choiceContainer">The VBoxContainer to hold choice buttons.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="choiceContainer"/> is <see langword="null"/>.</exception>
    public OmegaChoicePresenter(VBoxContainer choiceContainer)
    {
        _choiceContainer = choiceContainer ?? throw new ArgumentNullException(nameof(choiceContainer));
    }

    /// <inheritdoc/>
    public async Task<List<int>> PresentChoicesAsync(IList<string> choices, bool allowMultipleSelection = false)
    {
        if (choices == null)
            throw new ArgumentNullException(nameof(choices));

        if (choices.Count == 0)
            throw new ArgumentException("Choices list cannot be empty", nameof(choices));

        _allowMultipleSelection = allowMultipleSelection;
        _selectionTaskSource = new TaskCompletionSource<List<int>>();

        // Clear existing choices
        ClearChoiceButtons();

        // Create choice buttons
        for (int i = 0; i < choices.Count; i++)
        {
            var button = CreateChoiceButton(choices[i], i);
            _choiceContainer.AddChild(button);
            _choiceButtons.Add(button);
        }

        // Wait for user selection
        return await _selectionTaskSource.Task.ConfigureAwait(false);
    }

    /// <inheritdoc/>
    /// <remarks>See <see cref="OmegaChoiceOption"/> for choice option structure.</remarks>
    public async Task<int> PresentChoicesAsync(IList<OmegaChoiceOption> choiceOptions)
    {
        if (choiceOptions == null)
            throw new ArgumentNullException(nameof(choiceOptions));

        if (choiceOptions.Count == 0)
            throw new ArgumentException("Choice options list cannot be empty", nameof(choiceOptions));

        _allowMultipleSelection = false; // Single selection for ChoiceOption
        _selectionTaskSource = new TaskCompletionSource<List<int>>();

        // Clear existing choices
        ClearChoiceButtons();

        // Create choice buttons with options
        for (int i = 0; i < choiceOptions.Count; i++)
        {
            var button = CreateChoiceButton(choiceOptions[i], i);
            _choiceContainer.AddChild(button);
            _choiceButtons.Add(button);
        }

        // Wait for user selection and return single index
        var result = await _selectionTaskSource.Task.ConfigureAwait(false);
        return result.Count > 0 ? result[0] : -1;
    }

    /// <inheritdoc/>
    public void HideChoices()
    {
        ClearChoiceButtons();
        _selectionTaskSource?.TrySetCanceled();
        _selectionTaskSource = null;
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
        foreach (var button in _choiceButtons)
        {
            button.Disabled = !enabled;
        }
    }

    /// <inheritdoc/>
    public bool AreChoicesVisible()
    {
        return _choiceButtons.Count > 0;
    }

    /// <summary>
    /// Simulates choice selection for testing purposes.
    /// Completes the pending choice task with the specified index.
    /// </summary>
    /// <param name="index">The choice index to select.</param>
    public void SimulateChoiceSelection(int index)
    {
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
        var button = new Button();
        button.Text = text;
        button.Connect(Button.SignalName.Pressed, Callable.From(() => OnChoiceSelected(index)));
        return button;
    }

    /// <summary>
    /// Creates a choice button with the specified options.
    /// </summary>
    /// <param name="option">The choice option configuration.</param>
    /// <param name="index">The choice index.</param>
    /// <returns>The created button.</returns>
    private Button CreateChoiceButton(OmegaChoiceOption option, int index)
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
        if (_selectionTaskSource != null && !_selectionTaskSource.Task.IsCompleted)
        {
            var selectedIndices = _allowMultipleSelection
                ? new List<int> { index } // For now, single selection even in multi-mode
                : new List<int> { index };

            _selectionTaskSource.TrySetResult(selectedIndices);
        }
    }

    /// <summary>
    /// Clears all choice buttons from the container.
    /// </summary>
    private void ClearChoiceButtons()
    {
        foreach (var button in _choiceButtons)
        {
            _choiceContainer.RemoveChild(button);
            button.QueueFree();
        }
        _choiceButtons.Clear();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the presenter and cleans up resources.
    /// </summary>
    /// <param name="disposing">Whether this is being called from Dispose() or finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                HideChoices();
            }
            _disposed = true;
        }
    }
}
