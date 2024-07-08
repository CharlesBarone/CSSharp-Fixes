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