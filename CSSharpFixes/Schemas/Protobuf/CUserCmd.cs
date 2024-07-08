using System.Drawing;
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