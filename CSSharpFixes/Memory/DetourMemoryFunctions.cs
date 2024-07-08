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

using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace CSSharpFixes.Memory;

public static class DetourMemoryFunctions
{
    static DetourMemoryFunctions()
    {
        Add<MemoryFunctionVoid<IntPtr, IntPtr, int, bool, float, IntPtr>>("ProcessUsercmds");
    }

    private static Dictionary<string, BaseMemoryFunction> _memoryFunctions = new();
    
    private static void Add<T>(string signatureName) where T: BaseMemoryFunction
    {
        _memoryFunctions.Add(signatureName, Utils.BuildMemoryFunction<T>(signatureName));
    }
    
    public static BaseMemoryFunction Get(string signatureName)
    {
        if(!_memoryFunctions.TryGetValue(signatureName, out BaseMemoryFunction? memoryFunction))
            throw new InvalidOperationException($"Memory function for signature {signatureName} not found.");

        return memoryFunction;
    }
}