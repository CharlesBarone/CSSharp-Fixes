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
using CSSharpFixes.Extensions;
using CSSharpFixes.Fixes.Interfaces;

namespace CSSharpFixes.Fixes;

public class NoBlockFix: BaseFix, ITickable
{
    public NoBlockFix()
    {
        Name = "NoBlockFix";
        ConfigurationProperty = "EnableNoBlock";
    }

    public void OnTick(List<CCSPlayerController> players)
    {
        if(!Enabled) return;
        
        foreach (CCSPlayerController player in players)
        {
            if(!player.IsCompletelyValid(out var playerPawn)) continue;
            CollisionGroup collisionGroup = (CollisionGroup)playerPawn.Collision.CollisionAttribute.CollisionGroup;
            if(collisionGroup == CollisionGroup.COLLISION_GROUP_DEBRIS) continue;
            playerPawn.SetCollisionGroup(CollisionGroup.COLLISION_GROUP_DEBRIS);
        }
    }
}