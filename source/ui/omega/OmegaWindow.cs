using Godot;
using System;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Ui.Omega
{
    /// <summary>
    /// Reusable terminal window component for all game contexts (Stage Select, NPC Dialogs, Settings, etc.).
    /// Provides a cyberpunk terminal Ui with dynamic content adaptation.
    /// Enforces frame-constrained layout architecture: visible bezel border around all content.
    /// </summary>
    [GlobalClass]
    public partial class TerminalWindow : Control
    {
        [Export] public string Title { get; set; } = "Ωmega Spiral / Terminal";
        [Export] public bool ShowInputField { get; set; } = true;
        [Export] public bool ShowIndicators { get; set; } = true;

        /// <summary>
        /// Frame border color (RGB + alpha).
        /// </summary>
        [Export] public Color FrameBorderColor { get; set; } = new Color(1, 0, 0, 1.0f); // Red

        /// <summary>
        /// Frame border width in pixels (applied to all four sides).
        /// </summary>
        [Export] public int FrameBorderWidth { get; set; } = 4;

        /// <summary>
        /// Horizontal margin (left + right) for the bezel border in pixels.
        /// Creates the "CRT monitor bezel" effect.
        /// </summary>
        [Export] public float BezelHorizontalMargin { get; set; } = 128f; // 64px left + 64px right

        /// <summary>
        /// Vertical margin (top + bottom) for the bezel border in pixels.
        /// </summary>
        [Export] public float BezelVerticalMargin { get; set; } = 96f; // 48px top + 48px bottom

        private RichTextLabel? _outputLabel;
        private Button? _submitButton;
        private LineEdit? _inputField;
        private Label? _promptLabel;
        private Label? _titleLabel;
#pragma warning disable CA1816 // Godot nodes don't need explicit disposal
        private Panel? _bezel;
#pragma warning restore CA1816

        public override void _Ready()
        {
            base._Ready();

            // Get references to Ui elements with null-safe checks
            _outputLabel = GetNodeOrNull<RichTextLabel>("%OutputLabel");
            _inputField = GetNodeOrNull<LineEdit>("%InputField");
            _submitButton = GetNodeOrNull<Button>("%SubmitButton");
            _promptLabel = GetNodeOrNull<Label>("%PromptLabel");
            _titleLabel = GetNodeOrNull<Label>("Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/Header/Title");
            _bezel = GetNodeOrNull<Panel>("Bezel");

            if (_titleLabel == null || _bezel == null)
            {
                GD.PrintErr("[TerminalWindow] CRITICAL: Required nodes missing. Cannot initialize.");
                return;
            }

            // Set title
            _titleLabel.Text = Title;

            // Configure visibility based on context (only if nodes exist)
            if (_inputField != null) _inputField.Visible = ShowInputField;
            if (_submitButton != null) _submitButton.Visible = ShowInputField;
            if (_promptLabel != null) _promptLabel.Visible = ShowInputField;

            var indicators = GetNodeOrNull<Control>("Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/Header/Indicators");
            if (indicators != null)
            {
                indicators.Visible = ShowIndicators;
            }

            // Set border color and thickness for TerminalFrame
            var terminalFrame = GetNodeOrNull<Panel>("Bezel/MainMargin/MainLayout/TerminalFrame");

            if (terminalFrame != null)
            {
                // Get the existing StyleBoxFlat from the scene (or create if it doesn't exist)
                var styleBox = terminalFrame.GetThemeStylebox("panel") as StyleBoxFlat;

                if (styleBox != null)
                {
                    // Modify the existing StyleBoxFlat to ensure border properties are set
                    // This preserves any scene-defined properties while applying exported values
                    styleBox.BorderColor = FrameBorderColor;
                    styleBox.BorderWidthTop = FrameBorderWidth;
                    styleBox.BorderWidthBottom = FrameBorderWidth;
                    styleBox.BorderWidthLeft = FrameBorderWidth;
                    styleBox.BorderWidthRight = FrameBorderWidth;

                    GD.Print($"[TerminalWindow._Ready] Applied border: Color={styleBox.BorderColor}, Width={styleBox.BorderWidthTop}");
                }
                else
                {
                    // Fallback: create a new StyleBoxFlat if none exists
                    GD.PrintErr("[TerminalWindow._Ready] No StyleBoxFlat found in scene - creating new one");
                    var newStyleBox = new StyleBoxFlat();
                    newStyleBox.BgColor = new Color(0, 0, 0, 0); // Transparent background
                    newStyleBox.BorderColor = FrameBorderColor;
                    newStyleBox.BorderWidthTop = FrameBorderWidth;
                    newStyleBox.BorderWidthBottom = FrameBorderWidth;
                    newStyleBox.BorderWidthLeft = FrameBorderWidth;
                    newStyleBox.BorderWidthRight = FrameBorderWidth;

                    terminalFrame.AddThemeStyleboxOverride("panel", newStyleBox);
                }

                terminalFrame.QueueRedraw();
            }

            // Apply bezel constraints to ensure visible border
            CallDeferred(nameof(ApplyBezelConstraints));

            // Listen for viewport size changes to maintain bezel
            GetTree().Root.SizeChanged += OnViewportSizeChanged;
        }
        public override void _ExitTree()
        {
            base._ExitTree();
            GetTree().Root.SizeChanged -= OnViewportSizeChanged;
        }

        /// <summary>
        /// Called when viewport size changes to maintain bezel constraints.
        /// </summary>
        private void OnViewportSizeChanged()
        {
            ApplyBezelConstraints();
        }

        /// <summary>
        /// Enforces bezel border margins to create visible frame around content.
        /// Called during _Ready and can be called when viewport size changes.
        /// </summary>
        private void ApplyBezelConstraints()
        {
            if (_bezel == null)
            {
                GD.PrintErr("[TerminalWindow] Cannot apply bezel constraints - _bezel is null!");
                return;
            }

            var viewport = GetViewport();
            if (viewport == null)
            {
                GD.PrintErr("[TerminalWindow] Cannot apply bezel constraints - viewport is null!");
                return;
            }

            var viewportSize = viewport.GetVisibleRect().Size;

            // Calculate half margins for symmetric border
            float horizontalMarginHalf = BezelHorizontalMargin / 2f;
            float verticalMarginHalf = BezelVerticalMargin / 2f;

            GD.Print($"[TerminalWindow] BEFORE: Bezel offsets = L:{_bezel.OffsetLeft}, T:{_bezel.OffsetTop}, R:{_bezel.OffsetRight}, B:{_bezel.OffsetBottom}");

            // Set bezel offsets to create visible border
            // Left offset
            _bezel.OffsetLeft = horizontalMarginHalf;
            // Top offset
            _bezel.OffsetTop = verticalMarginHalf;
            // Right offset (negative from right edge)
            _bezel.OffsetRight = -horizontalMarginHalf;
            // Bottom offset (negative from bottom edge)
            _bezel.OffsetBottom = -verticalMarginHalf;

            GD.Print($"[TerminalWindow] AFTER: Applied bezel constraints: viewport={viewportSize}, margins=({horizontalMarginHalf}, {verticalMarginHalf})");
            GD.Print($"[TerminalWindow] AFTER: Bezel offsets = L:{_bezel.OffsetLeft}, T:{_bezel.OffsetTop}, R:{_bezel.OffsetRight}, B:{_bezel.OffsetBottom}");
        }

        /// <summary>
        /// Appends text to the terminal output asynchronously with typing effect.
        /// </summary>
        /// <param name="text">The text to append.</param>
        /// <param name="typingSpeed">Characters per second (0 for instant).</param>
        public async Task AppendTextAsync(string text, float typingSpeed = 50f)
        {
            if (_outputLabel == null) return;

            if (typingSpeed <= 0)
            {
                _outputLabel.Text += text;
                return;
            }

            foreach (char c in text)
            {
                _outputLabel.Text += c;
                await Task.Delay((int)(1000f / typingSpeed)).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Appends text to the terminal output instantly.
        /// </summary>
        /// <param name="text">The text to append.</param>
        public void AppendText(string text)
        {
            _outputLabel?.AppendText(text);
        }

        /// <summary>
        /// Clears the terminal output.
        /// </summary>
        public void ClearOutput()
        {
            if (_outputLabel != null)
                _outputLabel.Text = "";
        }

        /// <summary>
        /// Gets the current input text.
        /// </summary>
        /// <returns>The input text.</returns>
        public string GetInputText() => _inputField?.Text ?? "";

        /// <summary>
        /// Sets the input field text.
        /// </summary>
        /// <param name="text">The text to set.</param>
        public void SetInputText(string text)
        {
            if (_inputField != null)
                _inputField.Text = text;
        }

        /// <summary>
        /// Clears the input field.
        /// </summary>
        public void ClearInput()
        {
            if (_inputField != null)
                _inputField.Text = "";
        }

        /// <summary>
        /// Connects the submit button to a callback.
        /// </summary>
        /// <param name="callback">The callback to invoke on submit.</param>
        public void ConnectSubmit(Action callback)
        {
            if (_submitButton != null)
                _submitButton.Pressed += callback;
        }

        /// <summary>
        /// Disconnects a specific submit button callback.
        /// </summary>
        /// <param name="callback">The callback to disconnect.</param>
        public void DisconnectSubmit(Action callback)
        {
            if (_submitButton != null)
                _submitButton.Pressed -= callback;
        }

        /// <summary>
        /// Adds a custom content node to the terminal body.
        /// Automatically applies layout constraints after adding content.
        /// </summary>
        /// <param name="node">The node to add.</param>
        public void AddContentNode(Node node)
        {
            var screenLayout = GetNodeOrNull<VBoxContainer>("Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/TerminalBody/ScreenPadding/ScreenLayout");
            if (screenLayout == null)
            {
                GD.PushError("TerminalWindow: Content container not found. Cannot add content node.");
                return;
            }

            screenLayout.AddChild(node);

            // Ensure bezel constraints are maintained after adding content
            CallDeferred(nameof(ApplyBezelConstraints));
        }

        /// <summary>
        /// Removes a content node from the terminal body.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        public void RemoveContentNode(Node node)
        {
            var screenLayout = GetNodeOrNull<VBoxContainer>("Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/TerminalBody/ScreenPadding/ScreenLayout");
            if (screenLayout == null)
            {
                GD.PushError("TerminalWindow: Content container not found. Cannot remove content node.");
                return;
            }

            screenLayout.RemoveChild(node);
        }

        /// <summary>
        /// Gets the terminal's content container for adding custom Ui elements.
        /// </summary>
        /// <returns>The content container VBoxContainer, or null if not found.</returns>
        public VBoxContainer? GetContentContainer()
        {
            return GetNodeOrNull<VBoxContainer>("Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/TerminalBody/ScreenPadding/ScreenLayout");
        }
    }
}
