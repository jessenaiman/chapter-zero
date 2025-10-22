using Godot;
using System;
using System.Collections.Generic;
using OmegaSpiral.Source.Scripts.Common.Dialogue;
using OmegaSpiral.Source.UI.Terminal;

namespace OmegaSpiral.Source.Scripts.UI.Terminal;

/// <summary>
/// Terminal window system with outer frame and inner content.
/// Implements frame-constrained content that adapts to any frame size.
/// </summary>
[GlobalClass]
public partial class TerminalWindow : TerminalBase
{
    /// <summary>
    /// Gets or sets the frame size for the terminal window.
    /// Content adapts to fit within these bounds.
    /// </summary>
    public Vector2I FrameSize
    {
        get => _frameSize;
        set
        {
            _frameSize = value;
            OnFrameSizeChanged();
        }
    }

    /// <summary>
    /// Gets or sets the content to display in the terminal window.
    /// </summary>
    public string Content
    {
        get => _content;
        set
        {
            _content = value;
            UpdateContentDisplay();
        }
    }

    /// <summary>
    /// Gets or sets whether the terminal window is visible.
    /// </summary>
    public new bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            UpdateVisibility();
        }
    }

    /// <summary>
    /// Gets or sets the terminal window title.
    /// </summary>
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            UpdateTitleDisplay();
        }
    }

    // Private fields
    private Vector2I _frameSize = new(800, 600);
    private string _content = string.Empty;
    private bool _isVisible = true;
    private string _title = "Terminal";

    // Child nodes
    private Control? _frameContainer;
    private Label? _titleLabel;
    private RichTextLabel? _contentLabel;
    private VBoxContainer? _contentContainer;

    /// <summary>
    /// Called when the node enters the scene tree.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        // Set terminal mode to full for complete terminal functionality
        terminalMode = TerminalMode.Full;

        // Initialize child nodes
        InitializeChildNodes();

        // Apply initial state
        ApplyInitialState();
    }

    /// <summary>
    /// Initializes references to child nodes.
    /// </summary>
    private void InitializeChildNodes()
    {
        // In a real implementation, these would be retrieved from the scene
        // For now, we'll create them programmatically
        _frameContainer = new Control();
        _titleLabel = new Label();
        _contentLabel = new RichTextLabel();
        _contentContainer = new VBoxContainer();

        // Add to scene tree
        AddChild(_frameContainer);
        _frameContainer.AddChild(_titleLabel);
        _frameContainer.AddChild(_contentContainer);
        _contentContainer.AddChild(_contentLabel);
    }

    /// <summary>
    /// Applies the initial state to the terminal window.
    /// </summary>
    private void ApplyInitialState()
    {
        OnFrameSizeChanged();
        UpdateContentDisplay();
        UpdateVisibility();
        UpdateTitleDisplay();
    }

    /// <summary>
    /// Called when the frame size changes.
    /// </summary>
    private void OnFrameSizeChanged()
    {
        if (_frameContainer == null) return;

        // Resize the frame container to match the frame size
        _frameContainer.Size = new Vector2(_frameSize.X, _frameSize.Y);

        // Position the frame container in the center of the viewport
        var viewportSize = GetViewportRect().Size;
        _frameContainer.Position = new Vector2(
            (viewportSize.X - _frameSize.X) / 2,
            (viewportSize.Y - _frameSize.Y) / 2
        );

        // Update content layout to fit within frame
        UpdateContentLayout();
    }

    /// <summary>
    /// Updates the content layout to fit within the frame.
    /// </summary>
    private void UpdateContentLayout()
    {
        if (_contentContainer == null || _titleLabel == null || _contentLabel == null) return;

        // Calculate margins based on frame size (8% of frame dimensions)
        var horizontalMargin = _frameSize.X * 0.08f;
        var verticalMargin = _frameSize.Y * 0.08f;

        // Position title label at top with margin
        _titleLabel.Position = new Vector2(horizontalMargin, verticalMargin);
        _titleLabel.Size = new Vector2(_frameSize.X - (horizontalMargin * 2), 0);

        // Position content container below title with margin
        var titleHeight = _titleLabel.Size.Y > 0 ? _titleLabel.Size.Y : 30;
        _contentContainer.Position = new Vector2(horizontalMargin, verticalMargin + titleHeight + 10);
        _contentContainer.Size = new Vector2(
            _frameSize.X - (horizontalMargin * 2),
            _frameSize.Y - (verticalMargin * 2) - titleHeight - 20
        );

        // Ensure content label fills the content container
        _contentLabel.Size = _contentContainer.Size;
    }

    /// <summary>
    /// Updates the content display with the current content.
    /// </summary>
    private void UpdateContentDisplay()
    {
        if (_contentLabel == null) return;

        _contentLabel.Text = _content;
    }

    /// <summary>
    /// Updates the visibility of the terminal window.
    /// </summary>
    private void UpdateVisibility()
    {
        if (_frameContainer == null) return;

        _frameContainer.Visible = _isVisible;
    }

    /// <summary>
    /// Updates the title display with the current title.
    /// </summary>
    private void UpdateTitleDisplay()
    {
        if (_titleLabel == null) return;

        _titleLabel.Text = _title;
    }

    /// <summary>
    /// Sets the frame size with animation.
    /// </summary>
    /// <param name="newSize">The new frame size.</param>
    /// <param name="duration">The animation duration in seconds.</param>
    public async void AnimateFrameResize(Vector2I newSize, float duration = 0.5f)
    {
        if (_frameContainer == null) return;

        // Store original values
        var originalSize = _frameSize;
        var originalPosition = _frameContainer.Position;

        // Update frame size
        _frameSize = newSize;

        // Calculate new position
        var viewportSize = GetViewportRect().Size;
        var newPosition = new Vector2(
            (viewportSize.X - _frameSize.X) / 2,
            (viewportSize.Y - _frameSize.Y) / 2
        );

        // Animate the resize
        var elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            var ratio = elapsedTime / duration;

            // Interpolate size and position
            var currentSize = ((Vector2)originalSize).Lerp(_frameSize, ratio);
            var currentPosition = ((Vector2)originalPosition).Lerp(newPosition, ratio);

            // Apply interpolated values
            _frameContainer.Size = currentSize;
            _frameContainer.Position = currentPosition;

            // Update content layout
            UpdateContentLayout();

            // Wait for next frame
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            elapsedTime += (float)GetProcessDeltaTime();
        }

        // Ensure final values are applied
        _frameContainer.Size = new Vector2(_frameSize.X, _frameSize.Y);
        _frameContainer.Position = newPosition;
        UpdateContentLayout();
    }

    /// <summary>
    /// Shows the terminal window with fade-in animation.
    /// </summary>
    /// <param name="duration">The fade duration in seconds.</param>
    public async void ShowWithFade(float duration = 0.3f)
    {
        if (_frameContainer == null) return;

        // Set initial state
        _frameContainer.Modulate = new Color(1, 1, 1, 0);
        _isVisible = true;
        UpdateVisibility();

        // Animate fade-in
        var elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            var alpha = elapsedTime / duration;
            _frameContainer.Modulate = new Color(1, 1, 1, alpha);

            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            elapsedTime += (float)GetProcessDeltaTime();
        }

        // Ensure fully opaque
        _frameContainer.Modulate = Colors.White;
    }

    /// <summary>
    /// Hides the terminal window with fade-out animation.
    /// </summary>
    /// <param name="duration">The fade duration in seconds.</param>
    public async void HideWithFade(float duration = 0.3f)
    {
        if (_frameContainer == null) return;

        // Animate fade-out
        var elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            var alpha = 1.0f - (elapsedTime / duration);
            _frameContainer.Modulate = new Color(1, 1, 1, alpha);

            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            elapsedTime += (float)GetProcessDeltaTime();
        }

        // Hide completely
        _isVisible = false;
        UpdateVisibility();
        _frameContainer.Modulate = Colors.White;
    }
}
