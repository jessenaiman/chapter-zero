
// <copyright file="BattlerRoster.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts.Combat.Battlers;
/// <summary>
/// Provides methods to query and manage battlers within the current scene.
/// </summary>
/// <remarks>
/// This class uses Godot's scene tree to retrieve battler nodes grouped by <see cref="Battler.Group"/>.
/// </remarks>
[GlobalClass]
public partial class BattlerRoster : RefCounted
{
    private Callable getNodesInGroup;

    /// <summary>
    /// Initializes a new instance of the <see cref="BattlerRoster"/> class.
    /// </summary>
    /// <param name="treeRef">The <see cref="SceneTree"/> reference used to query battler nodes.</param>
    public BattlerRoster(SceneTree treeRef)
    {
        this.getNodesInGroup = new Callable(treeRef, "get_nodes_in_group");
    }

    /// <summary>
    /// Retrieves all battlers currently present in the scene.
    /// </summary>
    /// <returns>
    /// An array of <see cref="Battler"/> instances found in the battler group.
    /// </returns>
    public Battler[] GetBattlers()
    {
        var rawBattlerList = (Godot.Collections.Array) this.getNodesInGroup.Call(Battler.Group);
        var battlerList = new Battler[rawBattlerList.Count];
        for (int i = 0; i < rawBattlerList.Count; i++)
        {
            battlerList[i] = (Battler) rawBattlerList[i];
        }

        return battlerList;
    }

    /// <summary>
    /// Retrieves all player-controlled battlers.
    /// </summary>
    /// <returns>
    /// An array of <see cref="Battler"/> instances whose <see cref="Battler.Actor"/> is a player.
    /// </returns>
    public Battler[] GetPlayerBattlers()
    {
        return this.GetBattlers().Where(battler => battler.Actor != null && battler.Actor.IsPlayer).ToArray();
    }

    /// <summary>
    /// Retrieves all enemy battlers.
    /// </summary>
    /// <returns>
    /// An array of <see cref="Battler"/> instances whose <see cref="Battler.Actor"/> is not a player.
    /// </returns>
    public Battler[] GetEnemyBattlers()
    {
        return this.GetBattlers().Where(battler => battler.Actor != null && !battler.Actor.IsPlayer).ToArray();
    }

    /// <summary>
    /// Determines whether all specified battlers are defeated.
    /// </summary>
    /// <param name="battlers">The array of <see cref="Battler"/> instances to check.</param>
    /// <returns>
    /// <see langword="true"/> if all battlers are defeated; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool AreBattlersDefeated(Battler[] battlers)
    {
        foreach (Battler battler in battlers)
        {
            if (battler.Actor != null && battler.Actor.IsActive)
            {
                return false;
            }
        }

        return true;
    }
}
