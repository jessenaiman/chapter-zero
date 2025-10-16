namespace OmegaSpiral.Domain.Models
{
    /// <summary>
    /// Represents a character class in the game with associated attributes and abilities.
    /// </summary>
    public class CharacterClass
    {
        /// <summary>
        /// Gets or sets the unique identifier for the character class.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the character class.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the character class.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the base health points for this class.
        /// </summary>
        public int BaseHealth { get; set; }

        /// <summary>
        /// Gets or sets the base mana points for this class.
        /// </summary>
        public int BaseMana { get; set; }

        /// <summary>
        /// Gets or sets the base attack power for this class.
        /// </summary>
        public int BaseAttack { get; set; }

        /// <summary>
        /// Gets or sets the base defense power for this class.
        /// </summary>
        public int BaseDefense { get; set; }

        /// <summary>
        /// Gets or sets the base magic power for this class.
        /// </summary>
        public int BaseMagic { get; set; }

        /// <summary>
        /// Gets or sets the base magic defense for this class.
        /// </summary>
        public int BaseMagicDefense { get; set; }

        /// <summary>
        /// Gets or sets the base speed for this class.
        /// </summary>
        public int BaseSpeed { get; set; }

        /// <summary>
        /// Gets or sets the icon resource path for this class.
        /// </summary>
        public string IconPath { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterClass"/> class.
        /// </summary>
        public CharacterClass()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterClass"/> class with specified parameters.
        /// </summary>
        /// <param name="id">The unique identifier for the character class.</param>
        /// <param name="name">The display name of the character class.</param>
        /// <param name="description">The description of the character class.</param>
        /// <param name="baseHealth">The base health points for this class.</param>
        /// <param name="baseMana">The base mana points for this class.</param>
        /// <param name="baseAttack">The base attack power for this class.</param>
        /// <param name="baseDefense">The base defense power for this class.</param>
        /// <param name="baseMagic">The base magic power for this class.</param>
        /// <param name="baseMagicDefense">The base magic defense for this class.</param>
        /// <param name="baseSpeed">The base speed for this class.</param>
        /// <param name="iconPath">The icon resource path for this class.</param>
        public CharacterClass(
            string id,
            string name,
            string description,
            int baseHealth,
            int baseMana,
            int baseAttack,
            int baseDefense,
            int baseMagic,
            int baseMagicDefense,
            int baseSpeed,
            string iconPath)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.BaseHealth = baseHealth;
            this.BaseMana = baseMana;
            this.BaseAttack = baseAttack;
            this.BaseDefense = baseDefense;
            this.BaseMagic = baseMagic;
            this.BaseMagicDefense = baseMagicDefense;
            this.BaseSpeed = baseSpeed;
            this.IconPath = iconPath;
        }

        /// <summary>
        /// Creates a copy of this character class.
        /// </summary>
        /// <returns>A new instance of <see cref="CharacterClass"/> with the same values.</returns>
        public CharacterClass Clone()
        {
            return new CharacterClass
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                BaseHealth = this.BaseHealth,
                BaseMana = this.BaseMana,
                BaseAttack = this.BaseAttack,
                BaseDefense = this.BaseDefense,
                BaseMagic = this.BaseMagic,
                BaseMagicDefense = this.BaseMagicDefense,
                BaseSpeed = this.BaseSpeed,
                IconPath = this.IconPath,
            };
        }
    }
}
