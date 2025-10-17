// <copyright file="DefaultPartyFactory.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using OmegaSpiral.Source.Scripts;

namespace OmegaSpiral.Source.Scripts.Common;

/// <summary>
/// Factory class for creating default party configurations for the Scene5 demo.
/// This provides a pre-configured 4-member party matching the original godot-open-rpg demo.
/// </summary>
public static class DefaultPartyFactory
{
    /// <summary>
    /// Creates a default party of 4 heroes with classic RPG roles.
    /// This party composition matches the demo requirements:
    /// Fighter (tank), Rogue (DPS), Black Mage (offensive magic), White Mage (healing).
    /// </summary>
    /// <returns>A <see cref="PartyData"/> instance containing 4 pre-configured characters.</returns>
    public static PartyData CreateDefaultDemoParty()
    {
        var party = new PartyData();

        // Fighter - Tank/Melee DPS
        var fighter = new Character("Garrett", CharacterClass.Fighter, CharacterRace.Human);
        party.AddMember(fighter);

        // Thief/Rogue - DPS/Utility
        var rogue = new Character("Shadow", CharacterClass.Thief, CharacterRace.Elf);
        party.AddMember(rogue);

        // Mage - Black Mage/Offensive Caster
        var blackMage = new Character("Merlin", CharacterClass.Mage, CharacterRace.Human);
        party.AddMember(blackMage);

        // Priest - White Mage/Healer
        var whiteMage = new Character("Celeste", CharacterClass.Priest, CharacterRace.Elf);
        party.AddMember(whiteMage);

        // Set starting gold for demo
        party.AddGold(100);

        return party;
    }

    /// <summary>
    /// Creates a minimal 3-member party for testing.
    /// Used when the full 4-member party is not required.
    /// </summary>
    /// <returns>A <see cref="PartyData"/> instance containing 3 pre-configured characters.</returns>
    public static PartyData CreateMinimalParty()
    {
        var party = new PartyData();

        party.AddMember(new Character("Hero", CharacterClass.Fighter, CharacterRace.Human));
        party.AddMember(new Character("Wizard", CharacterClass.Mage, CharacterRace.Elf));
        party.AddMember(new Character("Cleric", CharacterClass.Priest, CharacterRace.Dwarf));

        party.AddGold(50);

        return party;
    }
}
