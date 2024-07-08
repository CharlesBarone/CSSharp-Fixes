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
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CSSharpFixes.Memory;
using Microsoft.Extensions.Logging;

namespace CSSharpFixes.Models;

public class Detour
{
    private string _name;
    private string _signatureName;
    private HookMode _hookMode;
    private bool _isHooked;
    public BaseMemoryFunction MemoryFunction;
    private List<Func<DynamicHook, HookResult>> _handlers;
    
    private readonly ILogger<CSSharpFixes> _logger;
    
    public Detour(string name, string signatureName, HookMode hookMode, ILogger<CSSharpFixes> logger)
    {
        _name = name;
        _signatureName = signatureName;
        _hookMode = hookMode;
        _logger = logger;
        _isHooked = false;
        _handlers = new List<Func<DynamicHook, HookResult>>();
        MemoryFunction = DetourMemoryFunctions.Get(_signatureName);
    }
    
    ~Detour()
    {
        if(_isHooked)
        {
            Unhook();
        }
    }
    
    public bool IsHooked() { return _isHooked; }
    public string GetName() { return _name; }
    public string GetSignatureName() { return _signatureName; }
    public string GetSignature() { return GameData.GetSignature(_signatureName); }
    public HookMode GetHookMode() { return _hookMode; }
    
    public void Hook(Func<DynamicHook, HookResult> handler, bool hookOnce = false)
    {
        if(hookOnce && _isHooked) return;
        MemoryFunction.Hook(handler, _hookMode);
        _isHooked = true;
        _handlers.Add(handler);
    }
    
    public void Unhook()
    {
        if(!_isHooked) return;

        foreach(Func<DynamicHook, HookResult> handler in _handlers)
        {
            MemoryFunction.Unhook(handler, _hookMode);
        }
        
        _handlers.Clear();
        _isHooked = false;
    }
}