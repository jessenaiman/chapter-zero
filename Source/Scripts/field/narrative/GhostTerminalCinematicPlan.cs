namespace OmegaSpiral.Source.Scripts.Field.Narrative
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Immutable description of the cinematic beats that compose the Ghost Terminal introduction.
    /// The plan enumerates which Godot scenes to instantiate and the narrative payload for each beat.
    /// </summary>
    public sealed class GhostTerminalCinematicPlan
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GhostTerminalCinematicPlan"/> class.
        /// </summary>
        /// <param name="beats">The ordered collection of cinematic beats.</param>
        public GhostTerminalCinematicPlan(List<GhostTerminalBeat> beats)
        {
            ArgumentNullException.ThrowIfNull(beats);
            this.Beats = new ReadOnlyCollection<GhostTerminalBeat>(beats);
        }

        /// <summary>
        /// Gets an empty cinematic plan.
        /// </summary>
        public static GhostTerminalCinematicPlan Empty { get; } = new(new List<GhostTerminalBeat>(0));

        /// <summary>
        /// Gets the ordered set of cinematic beats to render.
        /// </summary>
        public IReadOnlyList<GhostTerminalBeat> Beats { get; }
    }

    /// <summary>
    /// Represents a single cinematic beat in the Ghost Terminal introduction.
    /// </summary>
    public sealed class GhostTerminalBeat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GhostTerminalBeat"/> class.
        /// </summary>
        /// <param name="type">The beat type.</param>
        /// <param name="scenePath">The Godot scene path used to render the beat.</param>
        public GhostTerminalBeat(GhostTerminalBeatType type, string scenePath)
        {
            this.Type = type;
            this.ScenePath = scenePath;
        }

        /// <summary>
        /// Gets the beat type.
        /// </summary>
        public GhostTerminalBeatType Type { get; }

        /// <summary>
        /// Gets the Godot scene path that provides the visuals for this beat.
        /// </summary>
        public string ScenePath { get; }

        /// <summary>
        /// Gets or sets the primary prompt or narration associated with the beat.
        /// </summary>
        public string? Prompt { get; set; }

        /// <summary>
        /// Gets the narrative lines rendered within the beat.
        /// </summary>
        public List<string> Lines { get; } = new();

        /// <summary>
        /// Gets the choice options exposed during the beat.
        /// </summary>
        public List<GhostTerminalOption> Options { get; } = new();
    }

    /// <summary>
    /// Represents a selectable option presented within a cinematic beat.
    /// </summary>
    public sealed class GhostTerminalOption
    {
        /// <summary>
        /// Gets or sets the option identifier.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display label.
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the flavour description for the option.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Enumerates the cinematic beat categories for the Ghost Terminal sequence.
    /// </summary>
    public enum GhostTerminalBeatType
    {
        /// <summary>
        /// An opening narration line.
        /// </summary>
        OpeningLine,

        /// <summary>
        /// The Dreamweaver thread selection moment.
        /// </summary>
        ThreadChoice,

        /// <summary>
        /// A story paragraph within a block.
        /// </summary>
        StoryParagraph,

        /// <summary>
        /// A question posed at the end of a story block.
        /// </summary>
        StoryQuestion,

        /// <summary>
        /// A set of selectable answers for the current story block.
        /// </summary>
        StoryChoice,

        /// <summary>
        /// Prompt asking for the player's chosen name.
        /// </summary>
        NamePrompt,

        /// <summary>
        /// Secret alignment prompt for Dreamweaver scoring.
        /// </summary>
        SecretPrompt,

        /// <summary>
        /// Final exit line that closes the Ghost Terminal scene.
        /// </summary>
        ExitLine,
    }
}
