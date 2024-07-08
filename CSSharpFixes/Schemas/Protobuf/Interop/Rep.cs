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

namespace CSSharpFixes.Schemas.Protobuf.Interop;

public class Rep<T> where T : class
{
    private IntPtr _address;
    private Dictionary<string, ulong> _offsets = new();
    
    public Rep(IntPtr address)
    {
        if (address == IntPtr.Zero) throw new ArgumentException("Address cannot be zero.");
        _address = address;
        
        _offsets.Add("AllocatedSize", 0x0);
        // 0x4 of padding
        _offsets.Add("ElementsAddress", 0x8);
    }
    
    ~Rep()
    {
        _address = IntPtr.Zero;
        _offsets.Clear();
    }
    
    public int? AllocatedSize
    {
        get
        {
            IntPtr allocatedSizePtr = (IntPtr)((ulong)_address + _offsets["AllocatedSize"]);
            if(allocatedSizePtr == IntPtr.Zero) return null;
            
            unsafe
            {
                int* allocatedSize = (int*)allocatedSizePtr.ToPointer();
                return *allocatedSize;
            }
        }
    }
    
    public IntPtr? ElementsAddress
    {
        get
        {
            IntPtr elementsAddressPtr = (IntPtr)((ulong)_address + _offsets["ElementsAddress"]);
            if(elementsAddressPtr == IntPtr.Zero) return null;
            IntPtr elementsAddress = Marshal.ReadIntPtr(elementsAddressPtr);
            return elementsAddress;
        }
    }
}