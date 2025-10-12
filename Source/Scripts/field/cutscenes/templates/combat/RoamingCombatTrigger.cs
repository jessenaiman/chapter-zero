using Godot;
using System;
using System.Threading.Tasks;

[Tool]
public partial class RoamingCombatTrigger : CombatTrigger
{
    // If the player has defeated this 'roaming encounter', remove the encounter.
    protected override async void _RunVictoryCutscene()
    {
        QueueFree();
    }

    // If the player has lost to this 'roaming encounter', play the game-over screen.
    protected override async void _RunLossCutscene()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
    }
}
