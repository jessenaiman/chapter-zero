using Godot;
using System;

namespace OmegaSpiral.Ui.Menus
{
    [GlobalClass]
    public partial class StageSelectMenu : Control
    {
        private Button? stage1Button;
        private Button? stage2Button;
        private Button? quitButton;

        public override void _Ready()
        {
            this.stage1Button = this.GetNodeOrNull<Button>("Panel/VBoxContainer/Stage1Button");
            this.stage2Button = this.GetNodeOrNull<Button>("Panel/VBoxContainer/Stage2Button");
            this.quitButton = this.GetNodeOrNull<Button>("Panel/VBoxContainer/QuitButton");

            if (this.stage1Button != null)
            {
                this.stage1Button.Pressed += this.OnStage1Pressed;
            }

            if (this.stage2Button != null)
            {
                this.stage2Button.Pressed += this.OnStage2Pressed;
                this.UpdateStage2Status();
            }

            if (this.quitButton != null)
            {
                this.quitButton.Pressed += this.OnQuitPressed;
            }
        }

        private void UpdateStage2Status()
        {
            if (this.stage2Button == null) return;

            bool contentExists = Godot.FileAccess.FileExists("res://source/data/stages/ghost-terminal/hero.json") &&
                                 Godot.FileAccess.FileExists("res://source/data/stages/ghost-terminal/shadow.json") &&
                                 Godot.FileAccess.FileExists("res://source/data/stages/ghost-terminal/ambition.json");

            if (contentExists)
            {
                this.stage2Button.Text = "Ready (LLM Generated)";
            }
            else
            {
                this.stage2Button.Text = "Generate Content";
            }
        }

        private void OnStage1Pressed()
        {
            // Transition to stage 1
            GD.Print("Stage 1 pressed");
        }

        private void OnStage2Pressed()
        {
            // Transition to stage 2
            GD.Print("Stage 2 pressed");
        }

        private void OnQuitPressed()
        {
            GetTree().Quit();
        }
    }
}
