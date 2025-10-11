namespace OmegaSpiral.Source.Scripts.Models
{
    using Godot;

    /// <summary>
    /// Represents the visual appearance options for a character.
    /// </summary>
    public class CharacterAppearance
    {
        /// <summary>
        /// Gets or sets the name of this appearance option.
        /// </summary>
        [Export]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the preview texture for this appearance.
        /// </summary>
        public Texture2D? PreviewTexture { get; set; }

        /// <summary>
        /// Gets or sets the sprite texture for this appearance.
        /// </summary>
        public Texture2D? SpriteTexture { get; set; }

        /// <summary>
        /// Gets or sets the hair color.
        /// </summary>
        [Export]
        public Color HairColor { get; set; } = Colors.Brown;

        /// <summary>
        /// Gets or sets the skin color.
        /// </summary>
        [Export]
        public Color SkinColor { get; set; } = Colors.Beige;

        /// <summary>
        /// Gets or sets the eye color.
        /// </summary>
        [Export]
        public Color EyeColor { get; set; } = Colors.Blue;

        /// <summary>
        /// Gets or sets the body type identifier.
        /// </summary>
        [Export]
        public string BodyType { get; set; } = "Medium";

        /// <summary>
        /// Gets or sets the hair style identifier.
        /// </summary>
        [Export]
        public string HairStyle { get; set; } = "Short";

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterAppearance"/> class.
        /// </summary>
        public CharacterAppearance()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterAppearance"/> class with a name.
        /// </summary>
        /// <param name="name">The name of the appearance option.</param>
        public CharacterAppearance(string name)
        {
            Name = name;
        }
    }
}
