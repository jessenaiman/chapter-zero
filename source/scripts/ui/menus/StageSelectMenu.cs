// Copyright (c) Ωmega Spiral. All rights reserved.

using Godot;

namespace OmegaSpiral.Source.Scripts.ui.Menus
{
	/// <summary>
	/// Stage selection menu for testing and development.
	/// Allows direct access to any stage without progression requirements.
	/// </summary>
	[GlobalClass]
	public partial class StageSelectMenu : Control
	{
		private const string Stage1Scene = "res://source/stages/stage_1/opening.tscn";
		private const string Stage2Scene = "res://source/stages/stage_2/echo_hub.tscn";
		private const string Stage3Scene = "res://source/stages/stage_3/echo_vault_hub.tscn";
		private const string Stage4Scene = "res://source/stages/stage_4/tile_dungeon.tscn";
		private const string Stage5Scene = "res://source/stages/stage_5/fractured_escape.tscn";

		private Button? _stage1Button;
		private Button? _stage2Button;
		private Button? _stage3Button;
		private Button? _stage4Button;
		private Button? _stage5Button;
		private Button? _quitButton;
		private Label? _titleLabel;
		private Label? _descriptionLabel;

		/// <inheritdoc/>
		public override void _Ready()
		{
			this.InitializeUI();
			this.ConnectSignals();
			this.UpdateStageAvailability();
		}

		/// <summary>
		/// Initializes UI element references.
		/// </summary>
		private void InitializeUI()
		{
			this._titleLabel = this.GetNode<Label>("CenterContainer/VBoxContainer/TitleLabel");
			this._descriptionLabel = this.GetNode<Label>("CenterContainer/VBoxContainer/DescriptionLabel");

			var buttonContainer = this.GetNode<VBoxContainer>("CenterContainer/VBoxContainer/ButtonContainer");
			this._stage1Button = buttonContainer.GetNode<Button>("Stage1Button");
			this._stage2Button = buttonContainer.GetNode<Button>("Stage2Button");
			this._stage3Button = buttonContainer.GetNode<Button>("Stage3Button");
			this._stage4Button = buttonContainer.GetNode<Button>("Stage4Button");
			this._stage5Button = buttonContainer.GetNode<Button>("Stage5Button");
			this._quitButton = buttonContainer.GetNode<Button>("QuitButton");

			// Set button texts
			this._stage1Button.Text = "Stage 1: Ghost Terminal";
			this._stage2Button.Text = "Stage 2: Echo Hub";
			this._stage3Button.Text = "Stage 3: Echo Vault";
			this._stage4Button.Text = "Stage 4: Town Exploration";
			this._stage5Button.Text = "Stage 5: Fractured Escape";
			this._quitButton.Text = "Quit";
		}

		/// <summary>
		/// Connects button signals to handler methods.
		/// </summary>
		private void ConnectSignals()
		{
			if (this._stage1Button != null)
			{
				this._stage1Button.Pressed += this.OnStage1Pressed;
			}

			if (this._stage2Button != null)
			{
				this._stage2Button.Pressed += this.OnStage2Pressed;
			}

			if (this._stage3Button != null)
			{
				this._stage3Button.Pressed += this.OnStage3Pressed;
			}

			if (this._stage4Button != null)
			{
				this._stage4Button.Pressed += this.OnStage4Pressed;
			}

			if (this._stage5Button != null)
			{
				this._stage5Button.Pressed += this.OnStage5Pressed;
			}

			if (this._quitButton != null)
			{
				this._quitButton.Pressed += this.OnQuitPressed;
			}
		}

		/// <summary>
		/// Updates which stages are available based on scene file existence.
		/// </summary>
		private void UpdateStageAvailability()
		{
			if (this._stage1Button != null)
			{
				this._stage1Button.Disabled = !ResourceLoader.Exists(Stage1Scene);
			}

			if (this._stage2Button != null)
			{
				this._stage2Button.Disabled = !ResourceLoader.Exists(Stage2Scene);
			}

			if (this._stage3Button != null)
			{
				this._stage3Button.Disabled = !ResourceLoader.Exists(Stage3Scene);
			}

			if (this._stage4Button != null)
			{
				this._stage4Button.Disabled = !ResourceLoader.Exists(Stage4Scene);
			}

			if (this._stage5Button != null)
			{
				this._stage5Button.Disabled = !ResourceLoader.Exists(Stage5Scene);
			}
		}

		/// <summary>
		/// Loads Stage 1: Ghost Terminal.
		/// </summary>
		private void OnStage1Pressed()
		{
			GD.Print("[StageSelectMenu] Loading Stage 1: Ghost Terminal");
			this.LoadStage(Stage1Scene);
		}

		/// <summary>
		/// Loads Stage 2: NetHack Dungeon.
		/// </summary>
		private void OnStage2Pressed()
		{
			GD.Print("[StageSelectMenu] Loading Stage 2: NetHack Dungeon");
			this.LoadStage(Stage2Scene);
		}

		/// <summary>
		/// Loads Stage 3: Wizardry Party Creation.
		/// </summary>
		private void OnStage3Pressed()
		{
			GD.Print("[StageSelectMenu] Loading Stage 3: Wizardry Party");
			this.LoadStage(Stage3Scene);
		}

		/// <summary>
		/// Loads Stage 4: Town Exploration.
		/// </summary>
		private void OnStage4Pressed()
		{
			GD.Print("[StageSelectMenu] Loading Stage 4: Town Exploration");
			this.LoadStage(Stage4Scene);
		}

		/// <summary>
		/// Loads Stage 5: Fractured Escape.
		/// </summary>
		private void OnStage5Pressed()
		{
			GD.Print("[StageSelectMenu] Loading Stage 5: Fractured Escape");
			this.LoadStage(Stage5Scene);
		}

		/// <summary>
		/// Quits the application.
		/// </summary>
		private void OnQuitPressed()
		{
			GD.Print("[StageSelectMenu] Quitting application");
			this.GetTree().Quit();
		}

		/// <summary>
		/// Loads the specified stage scene.
		/// </summary>
		/// <param name="scenePath">Path to the stage scene file.</param>
		private void LoadStage(string scenePath)
		{
			if (!ResourceLoader.Exists(scenePath))
			{
				GD.PrintErr($"[StageSelectMenu] Scene not found: {scenePath}");
				return;
			}

			// Reset game state for fresh stage start
			if (this.HasNode("/root/GameState"))
			{
				var gameState = this.GetNode("/root/GameState");
				if (gameState.HasMethod("ResetForNewStage"))
				{
					gameState.Call("ResetForNewStage");
				}
			}

			var error = this.GetTree().ChangeSceneToFile(scenePath);
			if (error != Error.Ok)
			{
				GD.PrintErr($"[StageSelectMenu] Failed to load scene: {scenePath}, Error: {error}");
			}
		}
	}
}
