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
using CounterStrikeSharp.API.Modules.Utils;

namespace CSSharpFixes;

public partial class CSSharpFixes
{
    private void RegisterHooks()
    {
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        RegisterListener<Listeners.OnMapStart>(OnMapStart);
        RegisterListener<Listeners.OnTick>(OnTick);
        RegisterListener<Listeners.OnServerPrecacheResources>(OnServerPrecacheResources);
    }

    private void UnregisterHooks()
    {
        RemoveListener<Listeners.OnMapEnd>(OnMapEnd);
        RemoveListener<Listeners.OnMapStart>(OnMapStart);
        RemoveListener<Listeners.OnTick>(OnTick);
        RemoveListener<Listeners.OnServerPrecacheResources>(OnServerPrecacheResources);
    }

    private void OnServerPrecacheResources(ResourceManifest manifest)
    {
        //manifest.AddResource("models/food/pizza/pizza_1.vmdl");
    }

    private void OnMapEnd()
    {
        
    }

    private void OnTick()
    {
        _fixManager.OnTick();
    }

    private void OnMapStart(string mapName)
    {
        
    }
}