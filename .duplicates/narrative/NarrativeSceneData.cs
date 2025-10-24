// <copyright file="NarrativeSceneData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Narrative
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
        public IList<string> OpeningLines { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the initial choice presented to the player.
        /// The first decision point that determines narrative branching.
        /// </summary>
        public NarrativNethackice? InitialChoice { get; set; }

        /// <summary>
        /// Gets or sets the collection of story blocks that make up the narrative.
        /// Each block represents a segment of story with choices and progression.
        /// </summary>
        public IList<StoryBlock> StoryBlocks { get; set; } = new List<StoryBlock>();

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

        /// <summary>
        /// Gets or sets the configuration data for the Nethack Chamber stage (Stage 2).
        /// </summary>
        public NethackChamberData? NethackChamber { get; set; }

        /// <summary>
        /// Gets or sets the configuration data for the Nethack Vault stage (Stage 3).
        /// </summary>
        public NethackVaultData? NethackVault { get; set; }
    }

    /// <summary>
    /// Represents a choice point in the narrative with multiple options.
    /// Players select from available options to influence story progression.
    /// Each option can be interpreted differently based on context or stage-specific implementations.
    /// </summary>
    public partial class NarrativeChoice
    {
        /// <summary>
        /// Gets or sets the prompt text asking the player to make a choice.
        /// Provides context for the decision the player needs to make.
        /// </summary>
        public string? Prompt { get; set; }

        /// <summary>
        /// Gets or sets the collection of choice options available to the player.
        /// Each option represents a possible decision path in the narrative.
        /// See <see cref="ChoiceOption"/> for choice structure.
        /// </summary>
        public IList<ChoiceOption> Options { get; set; } = new List<ChoiceOption>();
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
        public IList<string> Paragraphs { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the question or prompt presented after the story text.
        /// Asks the player to make a decision that affects narrative progression.
        /// </summary>
        public string? Question { get; set; }

        /// <summary>
        /// Gets or sets the collection of choice options available after this story block.
        /// Each option leads to a different story path or outcome.
        /// See <see cref="ChoiceOption"/> for choice structure.
        /// </summary>
        public IList<ChoiceOption> Choices { get; set; } = new List<ChoiceOption>();
    }

    /// <summary>
    /// Represents a single choice option within a story block.
    /// Players select from multiple options to determine narrative progression.
    /// Each option leads to a different story block or outcome.
    /// See <see cref="ChoiceOption"/> for choice structure.
    /// </summary>
    public partial class ChoiceOption
    {
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

        private string? _Label;

        /// <summary>
        /// Gets or sets the display label for this choice option.
        /// Falls back to <see cref="Text"/> when not explicitly provided.
        /// </summary>
        public string? Label
        {
            get => this._Label ?? this.Text;
            set => this._Label = value;
        }

        /// <summary>
        /// Gets or sets the response or consequence of selecting this choice.
        /// </summary>
        public string? Response { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this choice is available.
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Gets or sets the next dialogue node to go to when this choice is selected.
        /// </summary>
        public string? NextNodeId { get; set; }

        /// <summary>
        /// Gets or sets the story block number this choice leads to (for narrative contexts).
        /// </summary>
        public int NextBlock { get; set; }

        /// <summary>
        /// Gets or sets the detailed description for this choice option.
        /// </summary>
        public string Description { get; set; } = string.Empty;
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
        public IList<string> Options { get; init; } = new List<string>();
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
        public IList<string> GlitchLines { get; init; } = new List<string>();

        /// <summary>Gets or sets whether the sequence should fade to a stable baseline.</summary>
        public bool FadeToStable { get; set; }
    }

    /// <summary>
    /// Opening monologue configuration block.
    /// </summary>
    public partial class GhostTerminalMonologue
    {
        /// <summary>Gets the ordered lines for the opening monologue.</summary>
        public IList<string> Lines { get; init; } = new List<string>();

        /// <summary>Gets or sets the cinematic timing tag.</summary>
        public string? CinematicTiming { get; set; }
    }

    /// <summary>
    /// Represents a cinematic choice block with setup lines followed by question/options.
    /// </summary>
    public partial class GhostTerminalChoiceBlock
    {
        /// <summary>Gets the setup lines displayed before presenting the choice.</summary>
        public IList<string> Setup { get; init; } = new List<string>();

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
        public IList<GhostTerminalOption> Options { get; init; } = new List<GhostTerminalOption>();
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
        public IList<string> Intro { get; init; } = new List<string>();

        /// <summary>Gets or sets the question associated with the fragment.</summary>
        public GhostTerminalQuestion Question { get; set; } = new();

        /// <summary>Gets the continuation lines after the question.</summary>
        public IList<string> Continuation { get; init; } = new List<string>();
    }

    /// <summary>
    /// Secret question block for the cinematic configuration.
    /// </summary>
    public partial class GhostTerminalSecretQuestion
    {
        /// <summary>Gets the setup lines before the secret prompt.</summary>
        public IList<string> Setup { get; init; } = new List<string>();

        /// <summary>Gets or sets the prompt string.</summary>
        public string Prompt { get; set; } = string.Empty;

        /// <summary>Gets the available secret options.</summary>
        public IList<GhostTerminalOption> Options { get; init; } = new List<GhostTerminalOption>();

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
        public IList<string> Text { get; init; } = new List<string>();

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
        public IList<string> Setup { get; init; } = new List<string>();

        /// <summary>Gets or sets the prompt text.</summary>
        public string Prompt { get; set; } = string.Empty;

        /// <summary>Gets the available naming options.</summary>
        public IList<GhostTerminalOption> Options { get; init; } = new List<GhostTerminalOption>();
    }

    /// <summary>
    /// Exit block that concludes the cinematic.
    /// </summary>
    public partial class GhostTerminalExit
    {
        /// <summary>Gets or sets the placeholder token for the selected thread.</summary>
        public string SelectedThread { get; set; } = string.Empty;

        /// <summary>Gets the final lines displayed when closing the stage.</summary>
        public IList<string> FinalLines { get; init; } = new List<string>();
    }

    /// <summary>
    /// Root data payload for the Nethack Chamber stage (Stage 2).
    /// </summary>
    public partial class NethackChamberData
    {
        /// <summary>Gets or sets metadata for the stage.</summary>
        public NethackChamberMetadata Metadata { get; set; } = new();

        /// <summary>Gets the Dreamweaver definitions referenced by interludes and chambers.</summary>
        public IList<NethackChamberDreamweaver> Dreamweavers { get; init; } = new List<NethackChamberDreamweaver>();

        /// <summary>Gets the ordered interludes preceding each chamber.</summary>
        public IList<NethackChamberInterlude> Interludes { get; init; } = new List<NethackChamberInterlude>();

        /// <summary>Gets the chamber definitions the player traverses.</summary>
        public IList<NethackChamberChamber> Chambers { get; init; } = new List<NethackChamberChamber>();

        /// <summary>Gets or sets the finale configuration.</summary>
        public NethackChamberFinale Finale { get; set; } = new();
    }

    /// <summary>
    /// Metadata surfaced by the Nethack Chamber stage.
    /// </summary>
    public partial class NethackChamberMetadata
    {
        /// <summary>Gets or sets the external iteration identifier.</summary>
        public string Iteration { get; set; } = string.Empty;

        /// <summary>Gets or sets the fallback iteration integer.</summary>
        public int IterationFallback { get; set; }

        /// <summary>Gets or sets the system status string.</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>Gets the system intro lines displayed at stage start.</summary>
        public IList<string> SystemIntro { get; init; } = new List<string>();
    }

    /// <summary>
    /// Dreamweaver styling information used for Ui theming during Stage 2.
    /// </summary>
    public partial class NethackChamberDreamweaver
    {
        /// <summary>Gets or sets the unique identifier (light/shadow/ambition).</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets the display name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets an accent color string (hex).</summary>
        public string AccentColor { get; set; } = string.Empty;

        /// <summary>Gets or sets the text theme identifier for Ui skinning.</summary>
        public string TextTheme { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a pre-chamber interlude where a Dreamweaver poses a question.
    /// </summary>
    public partial class NethackChamberInterlude
    {
        /// <summary>Gets or sets the interlude identifier.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets the Dreamweaver that owns the upcoming chamber.</summary>
        public string Owner { get; set; } = string.Empty;

        /// <summary>Gets or sets the question prompt.</summary>
        public string Prompt { get; set; } = string.Empty;

        /// <summary>Gets the selectable options (always three).</summary>
        public IList<NethackChamberOption> Options { get; init; } = new List<NethackChamberOption>();
    }

    /// <summary>
    /// Represents a selectable option used in interludes or chamber interactions.
    /// </summary>
    public partial class NethackChamberOption
    {
        /// <summary>Gets or sets the option identifier.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets the text presented to the player.</summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>Gets or sets the Dreamweaver alignment associated with this option.</summary>
        public string Alignment { get; set; } = string.Empty;

        /// <summary>Gets or sets optional interaction prompt for chamber objects.</summary>
        public string Prompt { get; set; } = string.Empty;

        /// <summary>Gets log lines displayed during the interaction.</summary>
        public IList<string> InteractionLog { get; init; } = new List<string>();

        /// <summary>Gets or sets the banter payload for approval/dissent lines.</summary>
        public NethackChamberBanter Banter { get; set; } = new();
    }

    /// <summary>
    /// Approval/dissent line bundle for Dreamweaver reactions.
    /// </summary>
    public partial class NethackChamberBanter
    {
        /// <summary>Gets or sets the approving Dreamweaver line.</summary>
        public NethackChamberLine Approve { get; set; } = new();

        /// <summary>Gets dissenting lines.</summary>
        public IList<NethackChamberLine> Dissent { get; init; } = new List<NethackChamberLine>();
    }

    /// <summary>
    /// Represents a single speaker line.
    /// </summary>
    public partial class NethackChamberLine
    {
        /// <summary>Gets or sets the speaker identifier.</summary>
        public string Speaker { get; set; } = string.Empty;

        /// <summary>Gets or sets the dialogue text.</summary>
        public string Line { get; set; } = string.Empty;
    }

    /// <summary>
    /// Chamber definition containing layout info and interactive objects.
    /// </summary>
    public partial class NethackChamberChamber
    {
        /// <summary>Gets or sets the chamber identifier.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets the owner Dreamweaver id.</summary>
        public string Owner { get; set; } = string.Empty;

        /// <summary>Gets or sets layout style information.</summary>
        public NethackChamberStyle Style { get; set; } = new();

        /// <summary>Gets the interactive objects (door, monster, chest).</summary>
        public IList<NethackChamberObject> Objects { get; init; } = new List<NethackChamberObject>();

        /// <summary>Gets decoy Nethackes that collapse to static.</summary>
        public IList<NethackChamberDecoy> Decoys { get; init; } = new List<NethackChamberDecoy>();
    }

    /// <summary>
    /// Layout style for a chamber.
    /// </summary>
    public partial class NethackChamberStyle
    {
        /// <summary>Gets or sets the template identifier.</summary>
        public string Template { get; set; } = string.Empty;

        /// <summary>Gets or sets ambient audio resource path.</summary>
        public string Ambient { get; set; } = string.Empty;

        /// <summary>Gets or sets the number of decoy tiles to spawn.</summary>
        public int DecoyCount { get; set; }

        /// <summary>Gets or sets the glitch profile identifier.</summary>
        public string GlitchProfile { get; set; } = string.Empty;
    }

    /// <summary>
    /// Interactive object inside a chamber.
    /// </summary>
    public partial class NethackChamberObject
    {
        /// <summary>Gets or sets the slot type (door/monster/chest).</summary>
        public string Slot { get; set; } = string.Empty;

        /// <summary>Gets or sets the Dreamweaver alignment.</summary>
        public string Alignment { get; set; } = string.Empty;

        /// <summary>Gets or sets the textual prompt shown when revealed.</summary>
        public string Prompt { get; set; } = string.Empty;

        /// <summary>Gets log lines describing the interaction.</summary>
        public IList<string> InteractionLog { get; init; } = new List<string>();

        /// <summary>Gets or sets banter lines triggered after interaction.</summary>
        public NethackChamberBanter Banter { get; set; } = new();
    }

    /// <summary>
    /// Represents a decoy object that collapses when examined.
    /// </summary>
    public partial class NethackChamberDecoy
    {
        /// <summary>Gets or sets the decoy identifier.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets the reveal text displayed when examined.</summary>
        public string RevealText { get; set; } = string.Empty;
    }

    /// <summary>
    /// Finale dialogue configuration mapping each dreamweaver to claim lines.
    /// </summary>
    public partial class NethackChamberFinale
    {
        /// <summary>Gets or sets the claimants dictionary.</summary>
        public Dictionary<string, NethackChamberFinaleClaimant> Claimants { get; init; } = new();

        /// <summary>Gets or sets the system outro string.</summary>
        public string SystemOutro { get; set; } = string.Empty;
    }

    /// <summary>
    /// Finale lines for a specific Dreamweaver.
    /// </summary>
    public partial class NethackChamberFinaleClaimant
    {
        /// <summary>Gets or sets the claim line delivered by the winner.</summary>
        public string Claim { get; set; } = string.Empty;

        /// <summary>Gets the responses from the remaining dreamweavers.</summary>
        public IList<NethackChamberLine> Responses { get; init; } = new List<NethackChamberLine>();
    }

    /// <summary>
    /// Root data payload for Stage 3 - Nethack Vault.
    /// </summary>
    public partial class NethackVaultData
    {
        /// <summary>Gets or sets metadata describing presentation tiers and intro text.</summary>
        public NethackVaultMetadata Metadata { get; set; } = new();

        /// <summary>Gets the ledger defining which Dreamweaver owns each decision beat.</summary>
        public IList<NethackVaultPointsLedgerEntry> PointsLedger { get; init; } = new List<NethackVaultPointsLedgerEntry>();

        /// <summary>Gets the ordered beat list (selection/combat/finale).</summary>
        public IList<NethackVaultBeat> Beats { get; init; } = new List<NethackVaultBeat>();

        /// <summary>Gets the Nethack definition catalogue.</summary>
        public IList<NethackVaultNethackDefinition> NethackDefinitions { get; init; } = new List<NethackVaultNethackDefinition>();

        /// <summary>Gets special non-Nethack options (rerolls, duplicates).</summary>
        public IList<NethackVaultSpecialOption> SpecialOptions { get; init; } = new List<NethackVaultSpecialOption>();

        /// <summary>Gets the combat encounter table.</summary>
        public IList<NethackVaultCombat> Combats { get; init; } = new List<NethackVaultCombat>();

        /// <summary>Gets global Omega log strings.</summary>
        public IList<string> OmegaLogs { get; init; } = new List<string>();

        /// <summary>Gets or sets the party persistence configuration.</summary>
        public NethackVaultPartyPersistence PartyPersistence { get; set; } = new();
    }

    /// <summary>Metadata for Nethack Vault presentation.</summary>
    public partial class NethackVaultMetadata
    {
        /// <summary>Gets or sets palette descriptor.</summary>
        public string Palette { get; set; } = string.Empty;

        /// <summary>Gets intro lines shown before decisions.</summary>
        public IList<string> SystemIntro { get; init; } = new List<string>();

        /// <summary>Gets presentation tier descriptors.</summary>
        public IList<NethackVaultPresentationTier> PresentationTiers { get; init; } = new List<NethackVaultPresentationTier>();
    }

    /// <summary>Represents a visual tier change.</summary>
    public partial class NethackVaultPresentationTier
    {
        /// <summary>Gets or sets tier index.</summary>
        public int Tier { get; set; }

        /// <summary>Gets or sets theme identifier.</summary>
        public string Theme { get; set; } = string.Empty;

        /// <summary>Gets or sets additional notes.</summary>
        public string? Notes { get; set; }
    }

    /// <summary>Ledger entry tying beats to Dreamweaver owners.</summary>
    public partial class NethackVaultPointsLedgerEntry
    {
        /// <summary>Gets or sets beat identifier.</summary>
        public string BeatId { get; set; } = string.Empty;

        /// <summary>Gets or sets the owner Dreamweaver id.</summary>
        public string Owner { get; set; } = string.Empty;
    }

    /// <summary>Represents a beat in the Nethack Vault sequence.</summary>
    public partial class NethackVaultBeat
    {
        /// <summary>Gets or sets beat identifier.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets beat type (selection/combat/finale).</summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>Gets or sets tier this beat belongs to.</summary>
        public int Tier { get; set; }

        /// <summary>Gets or sets owning Dreamweaver id (for selection beats).</summary>
        public string? Owner { get; set; }

        /// <summary>Gets or sets the prompt text for selection beats.</summary>
        public string? Prompt { get; set; }

        /// <summary>Gets Dreamweaver intro strings keyed by id.</summary>
        public Dictionary<string, string> DreamweaverIntro { get; init; } = new();

        /// <summary>Gets champion-specific whisper lines keyed by Dreamweaver id.</summary>
        public Dictionary<string, string> ChampionWhispers { get; init; } = new();

        /// <summary>Gets option identifiers used for this beat.</summary>
        public IList<string> Options { get; init; } = new List<string>();

        /// <summary>Gets or sets encounter identifier for combat beats.</summary>
        public string? EncounterId { get; set; }

        /// <summary>Gets or sets encounter intro text.</summary>
        public string? EncounterIntro { get; set; }

        /// <summary>Gets or sets tutorial hint string.</summary>
        public string? TutorialHint { get; set; }

        /// <summary>Gets Omega interrupt strings for combat beats.</summary>
        public IList<string> OmegaInterrupts { get; init; } = new List<string>();

        /// <summary>Gets finale summary dialogue keyed by Dreamweaver / system.</summary>
        public Dictionary<string, List<string>> Summary { get; init; } = new();
    }

    /// <summary>Definition for a selectable Nethack (party member).</summary>
    public partial class NethackVaultNethackDefinition
    {
        /// <summary>Gets or sets Nethack identifier.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets display name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets owning Dreamweaver id.</summary>
        public string Owner { get; set; } = string.Empty;

        /// <summary>Gets or sets archetype descriptor.</summary>
        public string Archetype { get; set; } = string.Empty;

        /// <summary>Gets or sets descriptive copy.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets mechanics block.</summary>
        public NethackVaultMechanics Mechanics { get; set; } = new();

        /// <summary>Gets or sets memory cost copy.</summary>
        public string? MemoryCost { get; set; }

        /// <summary>Gets Dreamweaver reactions keyed by id.</summary>
        public Dictionary<string, NethackVaultResponsePair> DreamweaverResponses { get; init; } = new();
    }

    /// <summary>Describes mechanics of an Nethack.</summary>
    public partial class NethackVaultMechanics
    {
        /// <summary>Gets or sets HP value.</summary>
        public int Hp { get; set; }

        /// <summary>Gets or sets Attack value.</summary>
        public int Attack { get; set; }

        /// <summary>Gets or sets signature ability text.</summary>
        public string Signature { get; set; } = string.Empty;
    }

    /// <summary>Supportive/caution line pair.</summary>
    public partial class NethackVaultResponsePair
    {
        /// <summary>Gets or sets supportive line.</summary>
        public string? Supportive { get; set; }

        /// <summary>Gets or sets caution line.</summary>
        public string? Caution { get; set; }
    }

    /// <summary>Represents a special non-Nethack option (reroll, duplicate).</summary>
    public partial class NethackVaultSpecialOption
    {
        /// <summary>Gets or sets option identifier.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets beat id stage this option belongs to.</summary>
        public string Stage { get; set; } = string.Empty;

        /// <summary>Gets or sets description.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Gets or sets effect summary.</summary>
        public string? Effect { get; set; }

        /// <summary>Gets or sets owner Dreamweaver id.</summary>
        public string? Owner { get; set; }

        /// <summary>Gets reactions keyed by Dreamweaver id.</summary>
        public Dictionary<string, string> DreamweaverReactions { get; init; } = new();
    }

    /// <summary>Combat encounter definition.</summary>
    public partial class NethackVaultCombat
    {
        /// <summary>Gets or sets encounter identifier.</summary>
        public string EncounterId { get; set; } = string.Empty;

        /// <summary>Gets enemy composition.</summary>
        public IList<NethackVaultEnemyEntry> EnemyList { get; init; } = new List<NethackVaultEnemyEntry>();

        /// <summary>Gets sample log lines for presentation.</summary>
        public IList<string> LogStyle { get; init; } = new List<string>();
    }

    /// <summary>Enemy entry inside combat definition.</summary>
    public partial class NethackVaultEnemyEntry
    {
        /// <summary>Gets or sets enemy identifier.</summary>
        public string EnemyId { get; set; } = string.Empty;

        /// <summary>Gets or sets count.</summary>
        public int Count { get; set; }

        /// <summary>Gets or sets level.</summary>
        public int Level { get; set; }

        /// <summary>Gets or sets conditional expression, when provided.</summary>
        public string? Conditional { get; set; }
    }

    /// <summary>Party persistence configuration for Stage 3.</summary>
    public partial class NethackVaultPartyPersistence
    {
        /// <summary>Gets or sets beat id when save occurs.</summary>
        public string TriggerBeat { get; set; } = string.Empty;

        /// <summary>Gets fields to persist.</summary>
        public IList<string> FieldsSaved { get; init; } = new List<string>();

        /// <summary>Gets or sets save method path.</summary>
        public string? SaveMethod { get; set; }

        /// <summary>Gets or sets additional notes.</summary>
        public string? Notes { get; set; }
    }
}
