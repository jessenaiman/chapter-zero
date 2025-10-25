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
    public partial class OmegaWindow : Control
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

        private RichTextLabel? _OutputLabel;
        private Button? _SubmitButton;
        private LineEdit? _InputField;
        private Label? _PromptLabel;
        private Label? _TitleLabel;
#pragma warning disable CA1816 // Godot nodes don't need explicit disposal
        private Panel? _Bezel;
#pragma warning restore CA1816

        public override void _Ready()
        {
            base._Ready();

            // Get references to Ui elements with null-safe checks
            _OutputLabel = GetNodeOrNull<RichTextLabel>("%OutputLabel");
            _InputField = GetNodeOrNull<LineEdit>("%InputField");
            _SubmitButton = GetNodeOrNull<Button>("%SubmitButton");
            _PromptLabel = GetNodeOrNull<Label>("%PromptLabel");
            _TitleLabel = GetNodeOrNull<Label>("Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/Header/Title");
            _Bezel = GetNodeOrNull<Panel>("Bezel");

            if (_TitleLabel == null || _Bezel == null)
            {
                GD.PrintErr("[OmegaWindow] CRITICAL: Required nodes missing. Cannot initialize.");
                return;
            }

            // Set title
            _TitleLabel.Text = Title;

            // Configure visibility based on context (only if nodes exist)
            if (_InputField != null) _InputField.Visible = ShowInputField;
            if (_SubmitButton != null) _SubmitButton.Visible = ShowInputField;
            if (_PromptLabel != null) _PromptLabel.Visible = ShowInputField;

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

                    GD.Print($"[OmegaWindow._Ready] Applied border: Color={styleBox.BorderColor}, Width={styleBox.BorderWidthTop}");
                }
                else
                {
                    // Fallback: create a new StyleBoxFlat if none exists
                    GD.PrintErr("[OmegaWindow._Ready] No StyleBoxFlat found in scene - creating new one");
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
            if (_Bezel == null)
            {
                GD.PrintErr("[OmegaWindow] Cannot apply bezel constraints - _Bezel is null!");
                return;
            }

            var viewport = GetViewport();
            if (viewport == null)
            {
                GD.PrintErr("[OmegaWindow] Cannot apply bezel constraints - viewport is null!");
                return;
            }

            var viewportSize = viewport.GetVisibleRect().Size;

            // Calculate half margins for symmetric border
            float horizontalMarginHalf = BezelHorizontalMargin / 2f;
            float verticalMarginHalf = BezelVerticalMargin / 2f;

            GD.Print($"[OmegaWindow] BEFORE: Bezel offsets = L:{_Bezel.OffsetLeft}, T:{_Bezel.OffsetTop}, R:{_Bezel.OffsetRight}, B:{_Bezel.OffsetBottom}");

            // Set bezel offsets to create visible border
            // Left offset
            _Bezel.OffsetLeft = horizontalMarginHalf;
            // Top offset
            _Bezel.OffsetTop = verticalMarginHalf;
            // Right offset (negative from right edge)
            _Bezel.OffsetRight = -horizontalMarginHalf;
            // Bottom offset (negative from bottom edge)
            _Bezel.OffsetBottom = -verticalMarginHalf;

            GD.Print($"[OmegaWindow] AFTER: Applied bezel constraints: viewport={viewportSize}, margins=({horizontalMarginHalf}, {verticalMarginHalf})");
            GD.Print($"[OmegaWindow] AFTER: Bezel offsets = L:{_Bezel.OffsetLeft}, T:{_Bezel.OffsetTop}, R:{_Bezel.OffsetRight}, B:{_Bezel.OffsetBottom}");
        }

        /// <summary>
        /// Appends text to the terminal output asynchronously with typing effect.
        /// </summary>
        /// <param name="text">The text to append.</param>
        /// <param name="typingSpeed">Characters per second (0 for instant).</param>
        public async Task AppendTextAsync(string text, float typingSpeed = 50f)
        {
            if (_OutputLabel == null) return;

            if (typingSpeed <= 0)
            {
                _OutputLabel.Text += text;
                return;
            }

            foreach (char c in text)
            {
                _OutputLabel.Text += c;
                await Task.Delay((int)(1000f / typingSpeed)).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Appends text to the terminal output instantly.
        /// </summary>
        /// <param name="text">The text to append.</param>
        public void AppendText(string text)
        {
            _OutputLabel?.AppendText(text);
        }

        /// <summary>
        /// Clears the terminal output.
        /// </summary>
        public void ClearOutput()
        {
            if (_OutputLabel != null)
                _OutputLabel.Text = "";
        }

        /// <summary>
        /// Gets the current input text.
        /// </summary>
        /// <returns>The input text.</returns>
        public string GetInputText() => _InputField?.Text ?? "";

        /// <summary>
        /// Sets the input field text.
        /// </summary>
        /// <param name="text">The text to set.</param>
        /// <exception cref="ArgumentNullException">Thrown when text is null.</exception>
        public void SetInputText(string text)
        {
            if (_InputField != null)
                _InputField.Text = text;
        }

        /// <summary>
        /// Clears the input field.
        /// </summary>
        public void ClearInput()
        {
            if (_InputField != null)
                _InputField.Text = "";
        }

        /// <summary>
        /// Connects the submit button to a callback.
        /// </summary>
        /// <param name="callback">The callback to invoke on submit.</param>
        /// <exception cref="ArgumentNullException">Thrown when callback is null.</exception>
        public void ConnectSubmit(Action callback)
        {
            if (_SubmitButton != null)
                _SubmitButton.Pressed += callback;
        }

        /// <summary>
        /// Disconnects a specific submit button callback.
        /// </summary>
        /// <param name="callback">The callback to disconnect.</param>
        /// <exception cref="ArgumentNullException">Thrown when callback is null.</exception>
        public void DisconnectSubmit(Action callback)
        {
            if (_SubmitButton != null)
                _SubmitButton.Pressed -= callback;
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
                GD.PushError("OmegaWindow: Content container not found. Cannot add content node.");
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
                GD.PushError("OmegaWindow: Content container not found. Cannot remove content node.");
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
