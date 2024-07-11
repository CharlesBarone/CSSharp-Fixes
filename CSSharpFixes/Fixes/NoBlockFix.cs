/*
    =============================================================================
    CS#Fixes
    Copyright (C) 2023-2024 Charles Barone <CharlesBarone> / hypnos <hyps.dev>
    =============================================================================

    This program is free software; you can redistribute it and/or modify it under
    the terms of the GNU General Public License, version 3.0, as published by the
    Free Software Foundation.

    This program is distributed in the hope that it will be useful, but WITHOUT
    ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
    FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
    details.

    You should have received a copy of the GNU General Public License along with
    this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Events;
using CSSharpFixes.Extensions;
using CSSharpFixes.Fixes.Interfaces;
using Microsoft.Extensions.Logging;

namespace CSSharpFixes.Fixes;

public class NoBlockFix: BaseFix, ITickable
{
    public NoBlockFix()
    {
        Name = "NoBlockFix";
        ConfigurationProperty = "EnableNoBlock";
        // Events = new Dictionary<string, CSSharpFixes.GameEventHandler>
        // {
        //     { "OnPlayerSpawn", OnPlayerSpawn },
        // };
    }
    
    public void ApplyNoBlock(CCSPlayerController? player)
    {
        if(!player.IsCompletelyValid(out var playerPawn)) return;
        CollisionGroup collisionGroup = (CollisionGroup)playerPawn.Collision.CollisionAttribute.CollisionGroup;
        if(collisionGroup == CollisionGroup.COLLISION_GROUP_DEBRIS) return;
        playerPawn.SetCollisionGroup(CollisionGroup.COLLISION_GROUP_DEBRIS);
    }
    
    public void OnTick(List<CCSPlayerController> players)
    {
        foreach(CCSPlayerController player in players)
        {
            ApplyNoBlock(player);
        }
    }

    // public HookResult OnPlayerSpawn(GameEvent @event, GameEventInfo info, ILogger<CSSharpFixes> logger)
    // {
    //     if(@event is not EventPlayerSpawn playerSpawnEvent) return HookResult.Continue;
    //     // logger.LogInformation("[CSSharpFixes][Fix][NoBlockFix][OnPlayerSpawn()]");
    //     ApplyNoBlock(playerSpawnEvent.Userid);
    //     return HookResult.Continue;
    // }
}