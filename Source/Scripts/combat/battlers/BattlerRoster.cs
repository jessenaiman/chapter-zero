// <copyright file="BattlerRoster.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Linq;
using Godot;

public partial class BattlerRoster : RefCounted
{
    private Callable getNodesInGroup;

    public BattlerRoster(SceneTree treeRef)
    {
        this.getNodesInGroup = new Callable(treeRef, "get_nodes_in_group");
    }

    public Battler[] GetBattlers()
    {
        var rawBattlerList = (Godot.Collections.Array)this.getNodesInGroup.Call(Battler.Group);
        var battlerList = new Battler[rawBattlerList.Count];
        for (int i = 0; i < rawBattlerList.Count; i++)
        {
            battlerList[i] = (Battler)rawBattlerList[i];
        }

        return battlerList;
    }

    public Battler[] GetPlayerBattlers()
    {
        return this.GetBattlers().Where(battler => battler.Actor != null && battler.Actor.IsPlayer).ToArray();
    }

    public Battler[] GetEnemyBattlers()
    {
        return this.GetBattlers().Where(battler => battler.Actor != null && !battler.Actor.IsPlayer).ToArray();
    }

    public static bool AreBattlersDefeated(Battler[] battlers)
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
