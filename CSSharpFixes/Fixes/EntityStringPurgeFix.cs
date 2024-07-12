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
using CounterStrikeSharp.API.Modules.Events;
using Microsoft.Extensions.Logging;

namespace CSSharpFixes.Fixes;

public class EntityStringPurgeFix: BaseFix
{
    public EntityStringPurgeFix()
    {
        Name = "EntityStringPurgeFix";
        ConfigurationProperty = "EnableEntityStringPurge";
        Events = new Dictionary<string, CSSharpFixes.GameEventHandler>
        {
            { "OnRoundStartPre", OnRoundStartPre }
        };
    }

    public HookResult OnRoundStartPre(GameEvent @event, GameEventInfo info, ILogger<CSSharpFixes> logger)
    {
        if(@event is not EventRoundPrestart roundPrestartEvent) return HookResult.Continue;
        logger.LogInformation("[CSSharpFixes][Fix][EntityStringPurgeFix][OnRoundStartPre()][NOT IMPLEMENTED]");
        
        //TODO: Finish implementing this. requires adding to C++ side of CounterStrikeSharp to finish.
        
        return HookResult.Continue;
    }
}