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
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;

namespace CSSharpFixes;

public partial class CSSharpFixes
{
    public delegate HookResult GameEventHandler(GameEvent gameEvent, GameEventInfo gameEventInfo, ILogger<CSSharpFixes> logger);
    
    private void RegisterHooks()
    {
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        RegisterListener<Listeners.OnMapStart>(OnMapStart);
        RegisterListener<Listeners.OnServerPrecacheResources>(OnServerPrecacheResources);
        RegisterListener<Listeners.OnTick>(OnTick);
    }

    private void UnregisterHooks()
    {
        RemoveListener<Listeners.OnMapEnd>(OnMapEnd);
        RemoveListener<Listeners.OnMapStart>(OnMapStart);
        RemoveListener<Listeners.OnServerPrecacheResources>(OnServerPrecacheResources);
    }

    private void OnServerPrecacheResources(ResourceManifest manifest)
    {
        //manifest.AddResource("models/food/pizza/pizza_1.vmdl");
    }

    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info) => 
        _eventManager.OnRoundStart(@event, info);
    
    [GameEventHandler]
    public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info) => 
        _eventManager.OnPlayerSpawn(@event, info);
    
    [GameEventHandler]
    public HookResult OnPlayerTeam(EventPlayerTeam @event, GameEventInfo info) => 
        _eventManager.OnPlayerTeam(@event, info);

    private void OnTick() => _fixManager.OnTick();
    
    private void OnMapEnd()
    {
        
    }

    private void OnMapStart(string mapName)
    {
        
    }
}