// <copyright file="NarrativeChoicePresenter.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Narrative-driven choice presenter that IS a VBoxContainer node.
/// Manages choice display, user selection, and async completion handling.
/// Extends VBoxContainer to be a proper Godot node following standard architecture.
/// </summary>
/// <remarks>
/// This presenter handles narrative-specific choice logic (TaskCompletionSource, selection tracking).
/// The actual presentation is delegated to child button nodes.
/// Relies on Godot's node lifecycle for cleanup - no manual disposal needed.
/// </remarks>
[GlobalClass]
public partial class NarrativeChoicePresenter : VBoxContainer
{
    private readonly List<Button> _choiceButtons = new();
    private TaskCompletionSource<List<int>>? _selectionTaskSource;
    private bool _allowMultipleSelection;

    /// <summary>
    /// Initializes a new instance of the <see cref="NarrativeChoicePresenter"/> class.
    /// This VBoxContainer manages its own choice buttons as children.
    /// </summary>
    public NarrativeChoicePresenter()
    {
        // Configure container defaults
        SizeFlagsVertical = SizeFlags.Fill;
        SizeFlagsHorizontal = SizeFlags.Fill;
    }

    /// <summary>
    /// Presents a list of choices and waits for the player to select one or more.
    /// </summary>
    /// <param name="choices">The list of choice texts to present.</param>
    /// <param name="allowMultipleSelection">Whether multiple choices can be selected (default: single selection).</param>
    /// <returns>A list of selected choice indices.</returns>
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
            this.AddChild(button);
            _choiceButtons.Add(button);
        }

        // Wait for user selection
        return await _selectionTaskSource.Task.ConfigureAwait(false);
    }

    /// <summary>
    /// Hides all choices and cancels any pending selection.
    /// </summary>
    public void HideChoices()
    {
        ClearChoiceButtons();
        _selectionTaskSource?.TrySetCanceled();
        _selectionTaskSource = null;
    }

    /// <summary>
    /// Gets the index of the currently selected choice (if any).
    /// </summary>
    /// <returns>The selected choice index, or -1 if none selected.</returns>
    public int GetSelectedChoiceIndex()
    {
        // This is a simplified implementation
        // In a real scenario, this would track the currently highlighted choice
        return -1;
    }

    /// <summary>
    /// Enables or disables choice navigation.
    /// </summary>
    /// <param name="enabled">Whether choices can be selected.</param>
    public void SetChoiceNavigationEnabled(bool enabled)
    {
        foreach (var button in _choiceButtons)
        {
            button.Disabled = !enabled;
        }
    }

    /// <summary>
    /// Checks if any choices are currently visible.
    /// </summary>
    /// <returns><see langword="true"/> if choices are displayed, <see langword="false"/> otherwise.</returns>
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
            this.RemoveChild(button);
            button.QueueFree();
        }
        _choiceButtons.Clear();
    }

    /// <summary>
    /// Called when node exits the scene tree.
    /// Cleanup is handled by Godot's lifecycle.
    /// </summary>
    public override void _ExitTree()
    {
        base._ExitTree();
        HideChoices();
    }
}
