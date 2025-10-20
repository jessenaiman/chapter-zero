// Copyright (c) Ωmega Spiral. All rights reserved.

using Godot;

namespace OmegaSpiral.Source.Scripts.ui.Menus;

/// <summary>
/// Stage selection menu for testing and development.
/// Allows direct access to any stage without progression requirements.
/// </summary>
[GlobalClass]
public partial class StageSelectMenu : Control
{
	/// <summary>
	/// NOTE: Stage 1 resources were moved into the <c>ghost</c> subtree during refactor.
	/// </summary>
	private const string Stage1Scene = "res://source/stages/ghost/scenes/opening.tscn";
	private const string Stage2Scene = "res://source/stages/stage_2/echo_hub.tscn";
	private const string Stage3Scene = "res://source/stages/stage_3/echo_vault_hub.tscn";
	private const string Stage4Scene = "res://source/stages/stage_4/tile_dungeon.tscn";
	private const string Stage5Scene = "res://source/stages/stage_5/fractured_escape.tscn";

		private Button? stage1Button;
		private Button? stage2Button;
		private Button? stage3Button;
		private Button? stage4Button;
		private Button? stage5Button;
		private Button? quitButton;
		private Label? titleLabel;
		private Label? descriptionLabel;

		/// <inheritdoc/>
		public override void _Ready()
		{
			this.InitializeUi();
			this.ConnectSignals();
			this.UpdateStageAvailability();
		}

		/// <summary>
		/// Initializes UI element references.
		/// </summary>
		private void InitializeUi()
		{
			this.titleLabel = this.GetNode<Label>("CenterContainer/VBoxContainer/TitleLabel");
			this.descriptionLabel = this.GetNode<Label>("CenterContainer/VBoxContainer/DescriptionLabel");

			var buttonContainer = this.GetNode<VBoxContainer>("CenterContainer/VBoxContainer/ButtonContainer");
			this.stage1Button = buttonContainer.GetNode<Button>("Stage1Button");
			this.stage2Button = buttonContainer.GetNode<Button>("Stage2Button");
			this.stage3Button = buttonContainer.GetNode<Button>("Stage3Button");
			this.stage4Button = buttonContainer.GetNode<Button>("Stage4Button");
			this.stage5Button = buttonContainer.GetNode<Button>("Stage5Button");
			this.quitButton = buttonContainer.GetNode<Button>("QuitButton");

			// Set button texts
			if (this.stage1Button != null) this.stage1Button.Text = "Stage 1: Ghost Terminal";
			if (this.stage2Button != null) this.stage2Button.Text = "Stage 2: Echo Hub";
			if (this.stage3Button != null) this.stage3Button.Text = "Stage 3: Echo Vault";
			if (this.stage4Button != null) this.stage4Button.Text = "Stage 4: Town Exploration";
			if (this.stage5Button != null) this.stage5Button.Text = "Stage 5: Fractured Escape";
			if (this.quitButton != null) this.quitButton.Text = "Quit";
		}

		/// <summary>
		/// Connects button signals to handler methods.
		/// </summary>
		private void ConnectSignals()
		{
			if (this.stage1Button != null)
			{
				this.stage1Button.Pressed += this.OnStage1Pressed;
			}

			if (this.stage2Button != null)
			{
				this.stage2Button.Pressed += this.OnStage2Pressed;
			}

			if (this.stage3Button != null)
			{
				this.stage3Button.Pressed += this.OnStage3Pressed;
			}

			if (this.stage4Button != null)
			{
				this.stage4Button.Pressed += this.OnStage4Pressed;
			}

			if (this.stage5Button != null)
			{
				this.stage5Button.Pressed += this.OnStage5Pressed;
			}

			if (this.quitButton != null)
			{
				this.quitButton.Pressed += this.OnQuitPressed;
			}
		}

		/// <summary>
		/// Updates which stages are available based on scene file existence.
		/// </summary>
		private void UpdateStageAvailability()
		{
			if (this.stage1Button != null)
			{
				this.stage1Button.Disabled = !ResourceLoader.Exists(Stage1Scene);
			}

			if (this.stage2Button != null)
			{
				this.stage2Button.Disabled = !ResourceLoader.Exists(Stage2Scene);
			}

			if (this.stage3Button != null)
			{
				this.stage3Button.Disabled = !ResourceLoader.Exists(Stage3Scene);
			}

			if (this.stage4Button != null)
			{
				this.stage4Button.Disabled = !ResourceLoader.Exists(Stage4Scene);
			}

			if (this.stage5Button != null)
			{
				this.stage5Button.Disabled = !ResourceLoader.Exists(Stage5Scene);
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
