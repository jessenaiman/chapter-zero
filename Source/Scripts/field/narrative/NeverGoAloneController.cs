namespace OmegaSpiral.Source.Scripts.Field.Narrative
{
    using System;
    using System.Text;
    using Godot;
    using OmegaSpiral.Source.Scripts.Infrastructure;
    using ConfigDictionary = Godot.Collections.Dictionary<string, Godot.Variant>;
    using GodotArray = Godot.Collections.Array;

    /// <summary>
    /// Lightweight controller that renders the Never Go Alone stage summary and
    /// transitions to the next gameplay scene when the player is ready.
    /// </summary>
    [GlobalClass]
    public partial class NeverGoAloneController : Node2D
    {
        private const string StageConfigPath = "res://Source/Data/stages/character-selection/never_go_alone.json";

        private Label? titleLabel;
        private RichTextLabel? summaryLabel;
        private Button? continueButton;
        private SceneManager? sceneManager;

        /// <inheritdoc/>
        public override void _Ready()
        {
            this.titleLabel = this.GetNode<Label>("Title");
            this.summaryLabel = this.GetNode<RichTextLabel>("Summary");
            this.continueButton = this.GetNode<Button>("ContinueButton");
            this.sceneManager = this.GetNode<SceneManager>("/root/SceneManager");

            if (this.titleLabel == null || this.summaryLabel == null || this.continueButton == null)
            {
                GD.PrintErr("NeverGoAloneController: required UI nodes were not found.");
                return;
            }

            try
            {
                var config = ConfigurationService.LoadConfiguration(StageConfigPath);
                this.PopulateView(config);
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Failed to load Never Go Alone configuration: {ex.Message}");
                this.summaryLabel.Text = "The Echo corridors have not yet been charted.";
            }

            this.continueButton.Pressed += this.OnContinuePressed;
        }

        private void PopulateView(ConfigDictionary config)
        {
            if (this.titleLabel == null || this.summaryLabel == null)
            {
                return;
            }

            if (config.TryGetValue("title", out var titleVariant) && titleVariant.VariantType == Variant.Type.String)
            {
                this.titleLabel.Text = titleVariant.As<string>();
            }

            var builder = new StringBuilder();

            if (config.TryGetValue("description", out var descriptionVariant) &&
                descriptionVariant.VariantType == Variant.Type.Array)
            {
                var descriptionArray = (GodotArray) descriptionVariant;
                foreach (var line in descriptionArray)
                {
                    builder.AppendLine(line.As<string>());
                }
                builder.AppendLine();
            }

            if (config.TryGetValue("companions", out var companionsVariant) &&
                companionsVariant.VariantType == Variant.Type.Array)
            {
                builder.AppendLine("[b]Companions[/b]");
                var companionsArray = (GodotArray) companionsVariant;
                foreach (ConfigDictionary companion in companionsArray)
                {
                    string name = companion.TryGetValue("name", out var nameVariant) ? nameVariant.As<string>() : "Unknown";
                    int affinity = companion.TryGetValue("affinityRequirement", out var affinityVariant)
                        ? (int) affinityVariant
                        : 0;
                    builder.AppendLine(FormattableString.Invariant($"• {name} (Affinity ≥ {affinity})"));

                    if (companion.TryGetValue("abilities", out var abilitiesVariant) &&
                        abilitiesVariant.VariantType == Variant.Type.Array)
                    {
                        var abilitiesArray = (GodotArray) abilitiesVariant;
                        foreach (ConfigDictionary ability in abilitiesArray)
                        {
                            string abilityName = ability.TryGetValue("name", out var abilityNameVariant)
                                ? abilityNameVariant.As<string>()
                                : "Ability";
                            string effect = ability.TryGetValue("effect", out var effectVariant)
                                ? effectVariant.As<string>()
                                : string.Empty;
                            builder.AppendLine(FormattableString.Invariant($"    - [i]{abilityName}[/i]: {effect}"));
                        }
                    }
                }

                builder.AppendLine();
            }

            if (config.TryGetValue("escapeSequence", out var escapeVariant) &&
                escapeVariant.VariantType == Variant.Type.Dictionary)
            {
                var escapeDict = (ConfigDictionary) escapeVariant;
                builder.AppendLine("[b]Escape Tiers[/b]");

                if (escapeDict.TryGetValue("tiers", out var tiersVariant) &&
                    tiersVariant.VariantType == Variant.Type.Array)
                {
                    var tiersArray = (GodotArray) tiersVariant;
                    foreach (ConfigDictionary tier in tiersArray)
                    {
                        string id = tier.TryGetValue("id", out var idVariant) ? idVariant.As<string>() : "tier";
                        string objective = tier.TryGetValue("objective", out var objectiveVariant)
                            ? objectiveVariant.As<string>()
                            : string.Empty;
                        builder.AppendLine(FormattableString.Invariant($"• {id}: {objective}"));
                    }

                    builder.AppendLine();
                }

                if (escapeDict.TryGetValue("finale", out var finaleVariant) &&
                    finaleVariant.VariantType == Variant.Type.Dictionary)
                {
                    var finaleDict = (ConfigDictionary) finaleVariant;
                    string success = finaleDict.TryGetValue("successNarration", out var successVariant)
                        ? successVariant.As<string>()
                        : string.Empty;
                    string fallback = finaleDict.TryGetValue("fallbackNarration", out var fallbackVariant)
                        ? fallbackVariant.As<string>()
                        : string.Empty;

                    builder.AppendLine("[b]Finale[/b]");
                    builder.AppendLine(success);
                    builder.AppendLine();
                    builder.AppendLine(fallback);
                }
            }

            this.summaryLabel.Text = builder.ToString();
        }

        private void OnContinuePressed()
        {
            if (this.sceneManager == null)
            {
                return;
            }

            // Check if the external Open RPG scene is available
            string externalScenePath = "res://Source/ExternalScenes/OpenRPGMain.tscn";
            if (Godot.FileAccess.FileExists(externalScenePath))
            {
                // Transition to the external Open RPG scene using SceneManager
                GD.Print("Transitioning to external Open RPG scene...");
                this.sceneManager.TransitionToScene("OpenRPGMain");
            }
            else
            {
                // Fallback to internal TileDungeon scene if external scene is not available
                GD.Print("External Open RPG scene not found, falling back to internal TileDungeon scene.");
                this.sceneManager.TransitionToScene("Scene4TileDungeon");
            }
        }
    }
}
