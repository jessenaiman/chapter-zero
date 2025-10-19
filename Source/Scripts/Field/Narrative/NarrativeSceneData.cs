// <copyright file="NarrativeSceneData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace OmegaSpiral.Source.Scripts.Field.Narrative
{
    /// <summary>
    /// Contains data for narrative terminal scenes with branching story paths.
    /// Supports dynamic story progression based on player choices and Dreamweaver alignment.
    /// Used to define interactive narrative experiences with multiple endings.
    /// </summary>
    public partial class NarrativeSceneData
    {
        /// <summary>
        /// Gets or sets the type identifier for this narrative scene.
        /// Used to distinguish between different narrative scene formats.
        /// </summary>
        public string Type { get; set; } = "narrative_terminal";

        /// <summary>
        /// Gets or sets the introductory text lines displayed at the start of the scene.
        /// Sets the narrative context before player choices begin.
        /// </summary>
        public List<string> OpeningLines { get; set; } = new();

        /// <summary>
        /// Gets or sets the initial choice presented to the player.
        /// The first decision point that determines narrative branching.
        /// </summary>
        public NarrativeChoice? InitialChoice { get; set; }

        /// <summary>
        /// Gets or sets the collection of story blocks that make up the narrative.
        /// Each block represents a segment of story with choices and progression.
        /// </summary>
        public List<StoryBlock> StoryBlocks { get; set; } = new();

        /// <summary>
        /// Gets or sets the prompt asking for the player's name.
        /// Used to personalize the narrative experience.
        /// </summary>
        public string? NamePrompt { get; set; }

        /// <summary>
        /// Gets or sets the secret question used for Dreamweaver alignment detection.
        /// Helps determine which Dreamweaver persona the player resonates with.
        /// </summary>
        public SecretQuestion? SecretQuestion { get; set; }

        /// <summary>
        /// Gets or sets the final line displayed when exiting the narrative scene.
        /// Provides closure to the narrative experience.
        /// </summary>
        public string? ExitLine { get; set; }

        /// <summary>
        /// Gets or sets the cinematic configuration for Ghost Terminal sequences.
        /// When populated, Stage 1 uses this to drive multi-scene beats without hardcoded fallbacks.
        /// </summary>
        public GhostTerminalCinematicData? Cinematic { get; set; }
    }

    /// <summary>
    /// Represents a choice point in the narrative with multiple Dreamweaver-aligned options.
    /// Players select from different Dreamweaver perspectives to influence story progression.
    /// Each option affects Dreamweaver scoring and determines narrative branching.
    /// </summary>
    public partial class NarrativeChoice
    {
        /// <summary>
        /// Gets or sets the prompt text asking the player to make a choice.
        /// Provides context for the decision the player needs to make.
        /// </summary>
        public string? Prompt { get; set; }

        /// <summary>
        /// Gets or sets the collection of Dreamweaver-aligned choice options.
        /// Each option represents a different Dreamweaver perspective on the situation.
        /// </summary>
        public List<DreamweaverChoice> Options { get; set; } = new();
    }

    /// <summary>
    /// Represents a segment of narrative story with text and branching choices.
    /// Contains paragraphs of story text and the choices that lead to other blocks.
    /// Forms the building blocks of the interactive narrative experience.
    /// </summary>
    public partial class StoryBlock
    {
        /// <summary>
        /// Gets or sets the collection of text paragraphs that make up this story block.
        /// Each paragraph is displayed sequentially with typewriter effects.
        /// </summary>
        public List<string> Paragraphs { get; set; } = new();

        /// <summary>
        /// Gets or sets the question or prompt presented after the story text.
        /// Asks the player to make a decision that affects narrative progression.
        /// </summary>
        public string? Question { get; set; }

        /// <summary>
        /// Gets or sets the collection of choice options available after this story block.
        /// Each option leads to a different story path or outcome.
        /// </summary>
        public List<ChoiceOption> Choices { get; set; } = new();
    }

    /// <summary>
    /// Represents a single choice option within a story block.
    /// Players select from multiple options to determine narrative progression.
    /// Each option leads to a different story block or outcome.
    /// </summary>
    public partial class ChoiceOption
    {
        // TODO: duplicate Source/Scripts/field/narrative/DialogueChoice.cs - Consider consolidating choice option classes
        /// <summary>
        /// Gets or sets the unique identifier for this choice option.
        /// Used to track which choice was selected and determine story progression.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display text for this choice option.
        /// The text shown to the player as an available choice.
        /// </summary>
        public string? Text { get; set; }

        private string? label;

        /// <summary>
        /// Gets or sets the display label for this choice option.
        /// Falls back to <see cref="Text"/> when not explicitly provided.
        /// </summary>
        public string? Label
        {
            get => this.label ?? this.Text;
            set => this.label = value;
        }

        /// <summary>
        /// Gets or sets the detailed description for this choice option.
        /// Provides additional context or explanation for the choice.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the story block number this choice leads to.
        /// Determines which story block is displayed after this choice is selected.
        /// </summary>
        public int NextBlock { get; set; }
    }

    /// <summary>
    /// Represents a secret question used to determine Dreamweaver alignment.
    /// Players answer questions that reveal their personality and Dreamweaver affinity.
    /// Used during character creation to determine starting Dreamweaver preferences.
    /// </summary>
    public partial class SecretQuestion
    {
        /// <summary>
        /// Gets or sets the question text presented to the player.
        /// The question that helps determine Dreamweaver alignment.
        /// </summary>
        public string? Prompt { get; set; }

        /// <summary>
        /// Gets the collection of possible answers to the secret question.
        /// Each answer corresponds to a different Dreamweaver alignment.
        /// </summary>
        public List<string> Options { get; init; } = new();
    }

    /// <summary>
    /// Represents the structured cinematic configuration for the Ghost Terminal opening sequence.
    /// Parsed directly from JSON so runtime code and tests can operate without synthesized fallback text.
    /// </summary>
    public partial class GhostTerminalCinematicData
    {
        /// <summary>
        /// Gets or sets metadata used for presentation and iteration tracking.
        /// </summary>
        public GhostTerminalMetadata Metadata { get; set; } = new();

        /// <summary>
        /// Gets or sets the boot sequence configuration.
        /// </summary>
        public GhostTerminalBootSequence BootSequence { get; set; } = new();

        /// <summary>
        /// Gets or sets the opening monologue content block.
        /// </summary>
        public GhostTerminalMonologue OpeningMonologue { get; set; } = new();

        /// <summary>
        /// Gets or sets the first choice block.
        /// </summary>
        public GhostTerminalChoiceBlock FirstChoice { get; set; } = new();

        /// <summary>
        /// Gets or sets the story fragment block.
        /// </summary>
        public GhostTerminalStoryFragment StoryFragment { get; set; } = new();

        /// <summary>
        /// Gets or sets the secret question block.
        /// </summary>
        public GhostTerminalSecretQuestion SecretQuestion { get; set; } = new();

        /// <summary>
        /// Gets or sets the name question block.
        /// </summary>
        public GhostTerminalNameQuestion NameQuestion { get; set; } = new();

        /// <summary>
        /// Gets or sets the exit block used to close the stage.
        /// </summary>
        public GhostTerminalExit Exit { get; set; } = new();
    }

    /// <summary>
    /// Metadata node captured from Ghost Terminal narrative configuration.
    /// </summary>
    public partial class GhostTerminalMetadata
    {
        /// <summary>Gets or sets the iteration identifier (external counter token).</summary>
        public string Iteration { get; set; } = string.Empty;

        /// <summary>Gets or sets the fallback iteration value.</summary>
        public int IterationFallback { get; set; }

        /// <summary>Gets or sets the previous attempt descriptor.</summary>
        public string PreviousAttempt { get; set; } = string.Empty;

        /// <summary>Gets or sets the interface description string.</summary>
        public string Interface { get; set; } = string.Empty;

        /// <summary>Gets or sets the system status string.</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional metadata note.</summary>
        public string? Note { get; set; }
    }

    /// <summary>
    /// Boot sequence definition with glitch lines and transitions.
    /// </summary>
    public partial class GhostTerminalBootSequence
    {
        /// <summary>Gets the glitch lines displayed during boot.</summary>
        public List<string> GlitchLines { get; init; } = new();

        /// <summary>Gets or sets whether the sequence should fade to a stable baseline.</summary>
        public bool FadeToStable { get; set; }
    }

    /// <summary>
    /// Opening monologue configuration block.
    /// </summary>
    public partial class GhostTerminalMonologue
    {
        /// <summary>Gets the ordered lines for the opening monologue.</summary>
        public List<string> Lines { get; init; } = new();

        /// <summary>Gets or sets the cinematic timing tag.</summary>
        public string? CinematicTiming { get; set; }
    }

    /// <summary>
    /// Represents a cinematic choice block with setup lines followed by question/options.
    /// </summary>
    public partial class GhostTerminalChoiceBlock
    {
        /// <summary>Gets the setup lines displayed before presenting the choice.</summary>
        public List<string> Setup { get; init; } = new();

        /// <summary>Gets or sets the question content for this block.</summary>
        public GhostTerminalQuestion Question { get; set; } = new();
    }

    /// <summary>
    /// Structured question containing prompt, context, and options.
    /// </summary>
    public partial class GhostTerminalQuestion
    {
        /// <summary>Gets or sets the question prompt text.</summary>
        public string Prompt { get; set; } = string.Empty;

        /// <summary>Gets or sets optional contextual guidance.</summary>
        public string? Context { get; set; }

        /// <summary>Gets the available options for the question.</summary>
        public List<GhostTerminalOption> Options { get; init; } = new();
    }

    /// <summary>
    /// Represents an option from the cinematic configuration.
    /// </summary>
    public partial class GhostTerminalOption
    {
        /// <summary>Gets or sets the option identifier.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets the display text.</summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>Gets or sets the dreamweaver alignment tag when present.</summary>
        public string? Dreamweaver { get; set; }

        /// <summary>Gets or sets the philosophical tag when present.</summary>
        public string? Philosophical { get; set; }

        /// <summary>Gets or sets the optional response text.</summary>
        public string? Response { get; set; }

        /// <summary>Gets or sets the score allocation for this option.</summary>
        public GhostTerminalScoreDistribution Scores { get; set; } = new();
    }

    /// <summary>
    /// Represents the Dreamweaver score distribution for a choice.
    /// </summary>
    public partial class GhostTerminalScoreDistribution
    {
        /// <summary>Gets or sets the Light score impact.</summary>
        public int Light { get; set; }

        /// <summary>Gets or sets the Shadow score impact.</summary>
        public int Shadow { get; set; }

        /// <summary>Gets or sets the Ambition score impact.</summary>
        public int Ambition { get; set; }
    }

    /// <summary>
    /// Story fragment block containing intro paragraphs, a question, and continuation.
    /// </summary>
    public partial class GhostTerminalStoryFragment
    {
        /// <summary>Gets the introductory lines.</summary>
        public List<string> Intro { get; init; } = new();

        /// <summary>Gets or sets the question associated with the fragment.</summary>
        public GhostTerminalQuestion Question { get; set; } = new();

        /// <summary>Gets the continuation lines after the question.</summary>
        public List<string> Continuation { get; init; } = new();
    }

    /// <summary>
    /// Secret question block for the cinematic configuration.
    /// </summary>
    public partial class GhostTerminalSecretQuestion
    {
        /// <summary>Gets the setup lines before the secret prompt.</summary>
        public List<string> Setup { get; init; } = new();

        /// <summary>Gets or sets the prompt string.</summary>
        public string Prompt { get; set; } = string.Empty;

        /// <summary>Gets the available secret options.</summary>
        public List<GhostTerminalOption> Options { get; init; } = new();

        /// <summary>Gets or sets the secret reveal configuration.</summary>
        public GhostTerminalSecretReveal SecretReveal { get; set; } = new();
    }

    /// <summary>
    /// Secret reveal block containing the symbol sequence and persistence information.
    /// </summary>
    public partial class GhostTerminalSecretReveal
    {
        /// <summary>Gets or sets the visual identifier used to configure shaders/effects.</summary>
        public string? Visual { get; set; }

        /// <summary>Gets the lines displayed during the reveal.</summary>
        public List<string> Text { get; init; } = new();

        /// <summary>Gets or sets a value indicating whether the reveal persists after presentation.</summary>
        public bool Persistent { get; set; }

        /// <summary>Gets or sets the journal entry identifier logged when revealed.</summary>
        public string? JournalEntry { get; set; }
    }

    /// <summary>
    /// Name question block executed near the end of the sequence.
    /// </summary>
    public partial class GhostTerminalNameQuestion
    {
        /// <summary>Gets the setup lines displayed before the prompt.</summary>
        public List<string> Setup { get; init; } = new();

        /// <summary>Gets or sets the prompt text.</summary>
        public string Prompt { get; set; } = string.Empty;

        /// <summary>Gets the available naming options.</summary>
        public List<GhostTerminalOption> Options { get; init; } = new();
    }

    /// <summary>
    /// Exit block that concludes the cinematic.
    /// </summary>
    public partial class GhostTerminalExit
    {
        /// <summary>Gets or sets the placeholder token for the selected thread.</summary>
        public string SelectedThread { get; set; } = string.Empty;

        /// <summary>Gets the final lines displayed when closing the stage.</summary>
        public List<string> FinalLines { get; init; } = new();
    }
}
