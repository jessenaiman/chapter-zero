using Godot;

namespace OmegaSpiral.Domain.Models
{
    /// <summary>
    /// Represents the visual appearance properties of a character.
    /// </summary>
    public class CharacterAppearance
    {
        /// <summary>
        /// Gets or sets the character's skin color in RGBA format.
        /// </summary>
        public Color SkinColor { get; set; } = Colors.White;

        /// <summary>
        /// Gets or sets the character's hair color in RGBA format.
        /// </summary>
        public Color HairColor { get; set; } = Colors.Black;

        /// <summary>
        /// Gets or sets the character's eye color in RGBA format.
        /// </summary>
        public Color EyeColor { get; set; } = Colors.Brown;

        /// <summary>
        /// Gets or sets the character's hair style identifier.
        /// </summary>
        public string HairStyle { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the character's eye style identifier.
        /// </summary>
        public string EyeStyle { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the character's clothing style identifier.
        /// </summary>
        public string ClothingStyle { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the character's accessory identifier.
        /// </summary>
        public string Accessory { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the path to the character's sprite texture.
        /// </summary>
        public string SpritePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the path to the character's portrait texture.
        /// </summary>
        public string PortraitPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the character's height scale factor.
        /// </summary>
        public float HeightScale { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the character's width scale factor.
        /// </summary>
        public float WidthScale { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the character's body type identifier.
        /// </summary>
        public string BodyType { get; set; } = "Normal";

        /// <summary>
        /// Gets or sets the character's facial expression identifier.
        /// </summary>
        public string FacialExpression { get; set; } = "Neutral";

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterAppearance"/> class.
        /// </summary>
        public CharacterAppearance()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterAppearance"/> class with specified parameters.
        /// </summary>
        /// <param name="skinColor">The character's skin color.</param>
        /// <param name="hairColor">The character's hair color.</param>
        /// <param name="eyeColor">The character's eye color.</param>
        /// <param name="hairStyle">The character's hair style identifier.</param>
        /// <param name="eyeStyle">The character's eye style identifier.</param>
        /// <param name="clothingStyle">The character's clothing style identifier.</param>
        /// <param name="accessory">The character's accessory identifier.</param>
        /// <param name="spritePath">The path to the character's sprite texture.</param>
        /// <param name="portraitPath">The path to the character's portrait texture.</param>
        /// <param name="heightScale">The character's height scale factor.</param>
        /// <param name="widthScale">The character's width scale factor.</param>
        /// <param name="bodyType">The character's body type identifier.</param>
        /// <param name="facialExpression">The character's facial expression identifier.</param>
        public CharacterAppearance(
            Color skinColor,
            Color hairColor,
            Color eyeColor,
            string hairStyle,
            string eyeStyle,
            string clothingStyle,
            string accessory,
            string spritePath,
            string portraitPath,
            float heightScale,
            float widthScale,
            string bodyType,
            string facialExpression)
        {
            this.SkinColor = skinColor;
            this.HairColor = hairColor;
            this.EyeColor = eyeColor;
            this.HairStyle = hairStyle;
            this.EyeStyle = eyeStyle;
            this.ClothingStyle = clothingStyle;
            this.Accessory = accessory;
            this.SpritePath = spritePath;
            this.PortraitPath = portraitPath;
            this.HeightScale = heightScale;
            this.WidthScale = widthScale;
            this.BodyType = bodyType;
            this.FacialExpression = facialExpression;
        }

        /// <summary>
        /// Creates a copy of this character appearance.
        /// </summary>
        /// <returns>A new instance of <see cref="CharacterAppearance"/> with the same values.</returns>
        public CharacterAppearance Clone()
        {
            return new CharacterAppearance
            {
                SkinColor = this.SkinColor,
                HairColor = this.HairColor,
                EyeColor = this.EyeColor,
                HairStyle = this.HairStyle,
                EyeStyle = this.EyeStyle,
                ClothingStyle = this.ClothingStyle,
                Accessory = this.Accessory,
                SpritePath = this.SpritePath,
                PortraitPath = this.PortraitPath,
                HeightScale = this.HeightScale,
                WidthScale = this.WidthScale,
                BodyType = this.BodyType,
                FacialExpression = this.FacialExpression,
            };
        }

        /// <summary>
        /// Resets the appearance to default values.
        /// </summary>
        public void ResetToDefault()
        {
            this.SkinColor = Colors.White;
            this.HairColor = Colors.Black;
            this.EyeColor = Colors.Brown;
            this.HairStyle = string.Empty;
            this.EyeStyle = string.Empty;
            this.ClothingStyle = string.Empty;
            this.Accessory = string.Empty;
            this.SpritePath = string.Empty;
            this.PortraitPath = string.Empty;
            this.HeightScale = 1.0f;
            this.WidthScale = 1.0f;
            this.BodyType = "Normal";
            this.FacialExpression = "Neutral";
        }
    }
}
