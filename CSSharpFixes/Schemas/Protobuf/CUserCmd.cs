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

using System.Runtime.InteropServices;

namespace CSSharpFixes.Schemas.Protobuf;

public class CUserCmd: Interfaces.ISizeable
{
    public static ulong Size() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? (ulong)0x88 : (ulong)0x80;
    
    private IntPtr _address;
    private Dictionary<string, ulong> _offsets = new();
    
    public CUserCmd(IntPtr address)
    {
        if (address == IntPtr.Zero) throw new ArgumentException("Address cannot be zero.");
        _address = address;
        
        // CBasePB + RepeatedPtrField_t<CCSGOInputHistoryEntryPB>
        // 0x18 + 0x18 = 0x30
        _offsets.Add("Base", 0x30); //CBasePB + RepeatedPtrField_t<CCSGOInputHistoryEntryPB>
        _offsets.Add("LeftHandDesired", 0x38);
    }
    
    ~CUserCmd()
    {
        _address = IntPtr.Zero;
        _offsets.Clear();
    }
    
    public CBaseUserCmdPB? Base
    {
        get
        {
            IntPtr baseAddressPtr = (IntPtr)((ulong)_address + _offsets["Base"]);
            if(baseAddressPtr == IntPtr.Zero) return null;
            IntPtr baseAddress = Marshal.ReadIntPtr(baseAddressPtr);
            if(baseAddress == IntPtr.Zero) return null;
            return new CBaseUserCmdPB(baseAddress);
        }
    }
    
    public Boolean? LeftHandDesired
    {
        get
        {
            IntPtr leftHandDesiredPtr = (IntPtr)((ulong)_address + _offsets["LeftHandDesired"]);
            if(leftHandDesiredPtr == IntPtr.Zero) return null;
            
            unsafe
            {
                Boolean* leftHandDesired = (Boolean*)leftHandDesiredPtr.ToPointer();
                return *leftHandDesired;
            }
        }
    }
}