using Godot;

namespace OmegaSpiral.Source.Scripts
{
    /// <summary>
    /// Core alignment mechanic that influences narrative and gameplay throughout the game.
    /// </summary>
    public enum DreamweaverType
    {
        Light,      // Hero thread - sacrifice and wisdom
        Mischief,   // Ambition thread - chaos and transformation
        Wrath       // Shadow thread - power and consequence
    }

    /// <summary>
    /// Player's chosen narrative alignment thread that determines story variations.
    /// </summary>
    public enum DreamweaverThread
    {
        Hero,       // Light-aligned choices
        Shadow,     // Wrath-aligned choices
        Ambition    // Mischief-aligned choices
    }

    /// <summary>
    /// Classic CRPG character classes for party creation.
    /// </summary>
    public enum CharacterClass
    {
        Fighter, Mage, Priest, Thief, Bard, Paladin, Ranger
    }

    /// <summary>
    /// Fantasy races available for character creation.
    /// </summary>
    public enum CharacterRace
    {
        Human, Elf, Dwarf, Gnome, Halfling, HalfElf
    }

    /// <summary>
    /// Types of interactive objects in ASCII dungeons.
    /// </summary>
    public enum ObjectType
    {
        Door, Monster, Chest
    }

    /// <summary>
    /// Types of tiles in the 2D tile dungeon.
    /// </summary>
    public enum TileType
    {
        Wall, Floor, Door, Key, Treasure, Exit
    }
}