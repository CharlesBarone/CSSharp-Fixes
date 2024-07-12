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

public class TeamMessagesFix: BaseFix
{
    public TeamMessagesFix()
    {
        Name = "TeamMessagesFix";
        ConfigurationProperty = "DisableTeamMessages";
        Events = new Dictionary<string, CSSharpFixes.GameEventHandler>
        {
            { "OnPlayerTeam", OnPlayerTeam },
        };
    }
    
    //TODO: This currently isn't working -_- ;(
    public HookResult OnPlayerTeam(GameEvent @event, GameEventInfo info, ILogger<CSSharpFixes> logger)
    {
        if(@event is not EventPlayerTeam playerTeamEvent) return HookResult.Continue;
        logger.LogInformation("[CSSharpFixes][Fix][TeamMessagesFix][OnPlayerTeam()]");
        playerTeamEvent.Silent = true;
        return HookResult.Handled;
    }
}