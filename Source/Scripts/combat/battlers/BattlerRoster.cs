using Godot;
using System;
using System.Linq;

public partial class BattlerRoster : RefCounted
{
    private Callable _getNodesInGroup;

    public BattlerRoster(SceneTree treeRef)
    {
        _getNodesInGroup = new Callable(treeRef, "get_nodes_in_group");
    }

    public Battler[] GetBattlers()
    {
        var rawBattlerList = (Godot.Collections.Array)_getNodesInGroup.Call(Battler.Group);
        var battlerList = new Battler[rawBattlerList.Count];
        for (int i = 0; i < rawBattlerList.Count; i++)
        {
            battlerList[i] = (Battler)rawBattlerList[i];
        }
        return battlerList;
    }

    public Battler[] GetPlayerBattlers()
    {
        return GetBattlers().Where(battler => battler.Actor != null && battler.Actor.IsPlayer).ToArray();
    }

    public Battler[] GetEnemyBattlers()
    {
        return GetBattlers().Where(battler => battler.Actor != null && !battler.Actor.IsPlayer).ToArray();
    }

    public bool AreBattlersDefeated(Battler[] battlers)
    {
        foreach (Battler battler in battlers)
        {
            if (battler.Actor.IsActive)
            {
                return false;
            }
        }

        return true;
    }
}
