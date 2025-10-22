using Godot;
using System;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.UI.Terminal
{
    /// <summary>
    /// Reusable terminal window component for all game contexts (Stage Select, NPC Dialogs, Settings, etc.).
    /// Provides a cyberpunk terminal UI with dynamic content adaptation.
    /// </summary>
    [GlobalClass]
    public partial class TerminalWindow : Control
    {
        [Export] public string Title { get; set; } = "Ωmega Spiral / Terminal";
        [Export] public bool ShowInputField { get; set; } = true;
        [Export] public bool ShowIndicators { get; set; } = true;

        private RichTextLabel? _outputLabel;
        private LineEdit? _inputField;
        private Button? _submitButton;
        private Label? _promptLabel;
        private Label? _titleLabel;

        public override void _Ready()
        {
            base._Ready();

            // Get references to UI elements
            _outputLabel = GetNode<RichTextLabel>("%OutputLabel");
            _inputField = GetNode<LineEdit>("%InputField");
            _submitButton = GetNode<Button>("%SubmitButton");
            _promptLabel = GetNode<Label>("%PromptLabel");
            _titleLabel = GetNode<Label>("Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/Header/Title");

            // Set title
            _titleLabel.Text = Title;

            // Configure visibility based on context
            _inputField.Visible = ShowInputField;
            _submitButton.Visible = ShowInputField;
            _promptLabel.Visible = ShowInputField;

            var indicators = GetNode<Control>("Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/Header/Indicators");
            indicators.Visible = ShowIndicators;
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
                await Task.Delay((int)(1000f / typingSpeed));
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
        /// </summary>
        /// <param name="node">The node to add.</param>
        public void AddContentNode(Node node)
        {
            var screenLayout = GetNode<VBoxContainer>("Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/TerminalBody/ScreenPadding/ScreenLayout");
            screenLayout.AddChild(node);
        }

        /// <summary>
        /// Removes a content node from the terminal body.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        public void RemoveContentNode(Node node)
        {
            var screenLayout = GetNode<VBoxContainer>("Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/TerminalBody/ScreenPadding/ScreenLayout");
            screenLayout.RemoveChild(node);
        }

        /// <summary>
        /// Gets the terminal's content container for adding custom UI elements.
        /// </summary>
        /// <returns>The content container VBoxContainer.</returns>
        public VBoxContainer GetContentContainer()
        {
            return GetNode<VBoxContainer>("Bezel/MainMargin/MainLayout/TerminalFrame/TerminalContent/TerminalBody/ScreenPadding/ScreenLayout");
        }
    }
}