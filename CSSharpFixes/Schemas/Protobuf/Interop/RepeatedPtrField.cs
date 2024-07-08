using System.Runtime.InteropServices;

namespace CSSharpFixes.Schemas.Protobuf.Interop;

public class RepeatedPtrField<T> where T : class, Interfaces.ISizeable
{
    public static ulong Size() => 0x18;
    
    private IntPtr _address;
    private Dictionary<string, ulong> _offsets = new();
    
    public RepeatedPtrField(IntPtr address)
    {
        if (address == IntPtr.Zero) throw new ArgumentException("Address cannot be zero.");
        _address = address;
        
        _offsets.Add("ArenaAddress", 0x0);
        _offsets.Add("CurrentSize", 0x8);
        _offsets.Add("TotalSize", 0xC);
        _offsets.Add("Rep", 0x10);
    }
    
    ~RepeatedPtrField()
    {
        _address = IntPtr.Zero;
        _offsets.Clear();
    }
    
    public IntPtr? ArenaAddress
    {
        get
        {
            IntPtr arenaAddressPtr = (IntPtr)((ulong)_address + _offsets["Arena"]);
            if(arenaAddressPtr == IntPtr.Zero) return null;
            IntPtr arenaAddress = Marshal.ReadIntPtr(arenaAddressPtr);
            if(arenaAddress == IntPtr.Zero) return null;
            return arenaAddress;
        }
    }
    
    public int? CurrentSize
    {
        get
        {
            IntPtr currentSizePtr = (IntPtr)((ulong)_address + _offsets["CurrentSize"]);
            if(currentSizePtr == IntPtr.Zero) return null;
            
            unsafe
            {
                int* currentSize = (int*)currentSizePtr.ToPointer();
                return *currentSize;
            }
        }
    }
    
    public int? TotalSize
    {
        get
        {
            IntPtr totalSizePtr = (IntPtr)((ulong)_address + _offsets["TotalSize"]);
            if(totalSizePtr == IntPtr.Zero) return null;
            
            unsafe
            {
                int* totalSize = (int*)totalSizePtr.ToPointer();
                return *totalSize;
            }
        }
    }
    
    public Rep<T>? Rep
    {
        get
        {
            IntPtr repAddressPtr = (IntPtr)((ulong)_address + _offsets["Rep"]);
            if(repAddressPtr == IntPtr.Zero) return null;
            IntPtr repAddress = Marshal.ReadIntPtr(repAddressPtr);
            if(repAddress == IntPtr.Zero) return null;
            return new Rep<T>(repAddress);
        }
    }

    public int Length
    {
        get
        {
            if(CurrentSize == null) return 0;
            return (int)CurrentSize;
        }
    }
    
    public IntPtr? this[int index]
    {
        get
        {
            if(index < 0 || index >= Length) return null;
            
            Rep<T>? rep = Rep;
            if(rep == null) return null;
            IntPtr? elementsPtr = rep.ElementsAddress;
            if(elementsPtr == null) return null;
            
            IntPtr tPtr = (IntPtr)((ulong)elementsPtr + (ulong)index * T.Size());
            if(tPtr == IntPtr.Zero) return null;

            return tPtr;
        }
    }
    
    
}