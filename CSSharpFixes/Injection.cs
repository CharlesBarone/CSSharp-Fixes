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
using CSSharpFixes.Config;
using CSSharpFixes.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace CSSharpFixes;

public class Injection : IPluginServiceCollection<CSSharpFixes>
{
    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ModuleInformation>();
        serviceCollection.AddSingleton<GameDataManager>();
        serviceCollection.AddSingleton<DetourManager>();
        serviceCollection.AddSingleton<PatchManager>();
        serviceCollection.AddSingleton<EventManager>();
        serviceCollection.AddSingleton<FixManager>();
        serviceCollection.AddSingleton<Configuration>();
    }
}