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

namespace CSSharpFixes.Managers;

public class GameDataManager
{
    private Dictionary<string /* modulePath */, Dictionary<string /* Signature */, IntPtr /* Address */>> _signatures = new();
    
    public IntPtr GetAddress(string modulePath, string signature)
    {
        if (!_signatures.TryGetValue(modulePath, out var signatures))
        {
            signatures = new Dictionary<string, IntPtr>();
            _signatures[modulePath] = signatures;
        }
        
        if (!signatures.TryGetValue(signature, out var address))
        {
            string byteString = GameData.GetSignature(signature);
            // Returns address if found, otherwise a C++ nullptr which is a IntPtr.Zero in C#
            address = NativeAPI.FindSignature(modulePath, byteString);
            signatures[signature] = address;
        }
        
        return address;
    }
    
    public void Start()
    {
        
    }
    
    public void Stop()
    {
        _signatures.Clear();
    }
}