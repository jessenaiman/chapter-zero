using Godot;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// Interface for presenting and handling player choices in the Omega UI.
/// Manages choice display, selection, and input handling.
/// </summary>
public interface IOmegaChoicePresenter
{
    /// <summary>
    /// Presents a list of choices to the player.
    /// </summary>
    /// <param name="choices">The collection of choice options to present.</param>
    /// <param name="allowMultipleSelection">Whether multiple choices can be selected.</param>
    /// <returns>A task that completes with the selected choice(s) index(es).</returns>
    Task<List<int>> PresentChoicesAsync(IList<string> choices, bool allowMultipleSelection = false);

    /// <summary>
    /// Presents choices with custom display options.
    /// See <see cref="OmegaChoiceOption"/> for choice structure.
    /// </summary>
    /// <param name="choiceOptions">The choice options with additional display properties.</param>
    /// <returns>A task that completes with the selected choice index.</returns>
    Task<int> PresentChoicesAsync(IList<OmegaChoiceOption> choiceOptions);

    /// <summary>
    /// Hides the choice presentation interface.
    /// </summary>
    void HideChoices();

    /// <summary>
    /// Gets the currently selected choice index, or -1 if none selected.
    /// </summary>
    /// <returns>The selected choice index.</returns>
    int GetSelectedChoiceIndex();

    /// <summary>
    /// Enables or disables choice navigation.
    /// </summary>
    /// <param name="enabled">Whether choice navigation should be enabled.</param>
    void SetChoiceNavigationEnabled(bool enabled);

    /// <summary>
    /// Checks if choices are currently being presented.
    /// </summary>
    /// <returns>True if choices are visible, false otherwise.</returns>
    bool AreChoicesVisible();
}

/// <summary>
/// Represents an Omega UI choice option with display properties.
/// </summary>
public class OmegaChoiceOption
{
    /// <summary>
    /// The text to display for this choice.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// The color to use for this choice text.
    /// </summary>
    public Color? TextColor { get; set; }

    /// <summary>
    /// Whether this choice is initially selected.
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// Additional metadata associated with this choice.
    /// </summary>
    public object? Metadata { get; set; }
}
