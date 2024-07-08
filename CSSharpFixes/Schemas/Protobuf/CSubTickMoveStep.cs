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

namespace CSSharpFixes.Schemas.Protobuf;

public class CSubTickMoveStep: Interfaces.ISizeable
{
    
    public static ulong Size() => 0x30;
    
    private IntPtr _address;
    private Dictionary<string, ulong> _offsets = new();
    
    public CSubTickMoveStep(IntPtr address)
    {
        if (address == IntPtr.Zero) throw new ArgumentException("Address cannot be zero.");
        _address = address;
        
        // CBasePB size = 0x18
        _offsets.Add("Button", 0x18); // ulong
        _offsets.Add("Pressed", 0x20); // bool
        // 0x3 bytes padding (to align bool)
        _offsets.Add("When", 0x24); // float
        _offsets.Add("AnalogForwardDelta", 0x28); // float
        _offsets.Add("AnalogLeftDelta", 0x2C); // float
    }
    
    ~CSubTickMoveStep()
    {
        _address = IntPtr.Zero;
        _offsets.Clear();
    }
    
    public ulong? Button
    {
        get
        {
            IntPtr buttonPtr = (IntPtr)((ulong)_address + _offsets["Button"]);
            if(buttonPtr == IntPtr.Zero) return null;
            
            unsafe
            {
                ulong* button = (ulong*)buttonPtr.ToPointer();
                return *button;
            }
        }
    }
    
    public bool? Pressed
    {
        get
        {
            IntPtr pressedPtr = (IntPtr)((ulong)_address + _offsets["Pressed"]);
            if(pressedPtr == IntPtr.Zero) return null;
            
            unsafe
            {
                bool* pressed = (bool*)pressedPtr.ToPointer();
                return *pressed;
            }
        }
    }
    
    public float? When
    {
        get
        {
            IntPtr whenPtr = (IntPtr)((ulong)_address + _offsets["When"]);
            if(whenPtr == IntPtr.Zero) return null;
            
            unsafe
            {
                float* when = (float*)whenPtr.ToPointer();
                return *when;
            }
        }
        set
        {
            IntPtr whenPtr = (IntPtr)((ulong)_address + _offsets["When"]);
            if(whenPtr == IntPtr.Zero) return;
            if (value == null) return;
            
            unsafe
            {
                float* when = (float*)whenPtr.ToPointer();
                *when = (float)value;
            }
        }
    }
    
    public float? AnalogForwardDelta
    {
        get
        {
            IntPtr analogForwardDeltaPtr = (IntPtr)((ulong)_address + _offsets["AnalogForwardDelta"]);
            if(analogForwardDeltaPtr == IntPtr.Zero) return null;
            
            unsafe
            {
                float* analogForwardDelta = (float*)analogForwardDeltaPtr.ToPointer();
                return *analogForwardDelta;
            }
        }
    }
    
    public float? AnalogLeftDelta
    {
        get
        {
            IntPtr analogLeftDeltaPtr = (IntPtr)((ulong)_address + _offsets["AnalogLeftDelta"]);
            if(analogLeftDeltaPtr == IntPtr.Zero) return null;
            
            unsafe
            {
                float* analogLeftDelta = (float*)analogLeftDeltaPtr.ToPointer();
                return *analogLeftDelta;
            }
        }
    }
}