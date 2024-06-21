using System.Runtime.InteropServices;

namespace CSSharpFixes.Schemas;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct BasePB
{
    public IntPtr vftable;
    public uint has_bits;
    public ulong cached_size;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Rep<T>
{
    public int allocated_size;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int.MaxValue - 2 * sizeof(int)) / 8)] // Assuming 8 bytes per IntPtr
    public T[] elements;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RepeatedPtrField<T>
{
    public IntPtr arena;
    public int current_size;
    public int total_size;
    public Rep<T> rep;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CInButtonStateReal
{
    public IntPtr table;
    public ulong buttonstate1;
    public ulong buttonstate2;
    public ulong buttonstate3;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x30)]
    public byte[] pad;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CSubtickMoveStep
{
    public BasePB basePB; // Include BasePB as a field
    public ulong button;
    [MarshalAs(UnmanagedType.I1)]
    public bool pressed;
    public float when;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CBaseUserCmdPB
{
    public BasePB basePB; // Include BasePB as a field
    public RepeatedPtrField<CSubtickMoveStep> subtick_moves;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CUserCmdWindows
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
    public byte[] pad;
    public uint tick_count;
    public uint unknown;
    public IntPtr inputHistory;
    public IntPtr @base; // CBaseUserCmdPB *base;
    public CInButtonStateReal buttonState;
    public IntPtr windowsMoment;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CUserCmdUnix
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
    public byte[] pad;
    public uint tick_count;
    public uint unknown;
    public IntPtr inputHistory;
    public IntPtr @base; // CBaseUserCmdPB *base;
    public CInButtonStateReal buttonState;
}

