using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CSSharpFixes.Memory;

// Based on https://github.com/Source2ZE/CS2Fixes/blob/main/src/utils/plat_win.cpp
public class WinMemoryUtils
{
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesWritten);
    
    public static void PatchBytesAtAddress(IntPtr pPatchAddress, byte[] pPatch, int iPatchSize)
    {
        if(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;
        
        IntPtr bytesWritten;
        WriteProcessMemory(Process.GetCurrentProcess().Handle, pPatchAddress, pPatch, (uint)iPatchSize, out bytesWritten);
    }
}