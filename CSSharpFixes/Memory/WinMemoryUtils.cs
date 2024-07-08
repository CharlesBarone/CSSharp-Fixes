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
    
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(int processAccess, bool bInheritHandle, int processId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
    
    public static byte[]? ReadMemory(IntPtr address, int size)
    {
        if(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return null;
        
        byte[] buffer = new byte[size];
        int bytesRead;
        ReadProcessMemory(Process.GetCurrentProcess().Handle, address, buffer, size, out bytesRead);
        return buffer;
    }
}