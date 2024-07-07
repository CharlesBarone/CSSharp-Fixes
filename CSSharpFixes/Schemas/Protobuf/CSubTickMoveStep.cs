using System.Drawing;

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
        
        _offsets.Add("Button", 0x18);
        _offsets.Add("Pressed", 0x20);
        _offsets.Add("When", 0x21);
        _offsets.Add("AnalogFowardDelta", 0x25);
        _offsets.Add("AnalogLeftDelta", 0x29);
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
    
    public float? AnalogFowardDelta
    {
        get
        {
            IntPtr analogFowardDeltaPtr = (IntPtr)((ulong)_address + _offsets["AnalogFowardDelta"]);
            if(analogFowardDeltaPtr == IntPtr.Zero) return null;
            
            unsafe
            {
                float* analogFowardDelta = (float*)analogFowardDeltaPtr.ToPointer();
                return *analogFowardDelta;
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