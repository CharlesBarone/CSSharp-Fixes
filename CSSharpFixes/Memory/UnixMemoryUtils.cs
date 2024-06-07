using System.Runtime.InteropServices;

namespace CSSharpFixes.Memory;

// Based on https://github.com/Source2ZE/CS2Fixes/blob/main/src/utils/plat_unix.cpp
public class UnixMemoryUtils
{
    static int ParseProt(string s)
    {
        int prot = 0;

        foreach (var c in s)
        {
            switch (c)
            {
                case '-':
                    break;
                case 'r':
                    prot |= NativeMethods.PROT_READ;
                    break;
                case 'w':
                    prot |= NativeMethods.PROT_WRITE;
                    break;
                case 'x':
                    prot |= NativeMethods.PROT_EXEC;
                    break;
                case 's':
                    break;
                case 'p':
                    break;
                default:
                    break;
            }
        }

        return prot;
    }

    static int GetProt(IntPtr pAddr, uint nSize)
    {
        using (var f = File.OpenRead("/proc/self/maps"))
        using (var reader = new StreamReader(f))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line == null)
                    continue;

                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 5)
                    continue;

                var range = parts[0];
                var prot = parts[1];

                var startEnd = range.Split('-');
                if (startEnd.Length != 2)
                    continue;

                var start = Convert.ToUInt64(startEnd[0], 16);
                var end = Convert.ToUInt64(startEnd[1], 16);

                if (start < (ulong)pAddr && end > (ulong)pAddr + nSize)
                {
                    return ParseProt(prot);
                }
            }
        }

        return 0;
    }

    public static void PatchBytesAtAddress(IntPtr pPatchAddress, byte[] pPatch, int iPatchSize)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return;
        
        var oldProt = GetProt(pPatchAddress, (uint)iPatchSize);

        var pageSize = (ulong)NativeMethods.sysconf(NativeMethods._SC_PAGESIZE);
        var alignAddr = (IntPtr)((long)pPatchAddress & ~(long)(pageSize - 1));

        var end = (IntPtr)((long)pPatchAddress + iPatchSize);
        var alignSize = (ulong)((long)end - (long)alignAddr);

        var result = NativeMethods.mprotect(alignAddr, alignSize, NativeMethods.PROT_READ | NativeMethods.PROT_WRITE);

        Marshal.Copy(pPatch, 0, pPatchAddress, iPatchSize);

        result = NativeMethods.mprotect(alignAddr, alignSize, oldProt);
    }

    #pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
    #pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    static class NativeMethods
    {
        public const int O_RDONLY = 0;
        public const int PROT_READ = 0x1;
        public const int PROT_WRITE = 0x2;
        public const int PROT_EXEC = 0x4;
        public const int MAP_PRIVATE = 0x2;
        public const int PT_LOAD = 1;
        public const int PF_X = 0x1;
        public const int _SC_PAGESIZE = 30;
        public const int RTLD_DI_LINKMAP = 2;

        [DllImport("libc")]
        public static extern int dlinfo(IntPtr handle, int request, out link_map lmap);

        [DllImport("libc")]
        public static extern int dlclose(IntPtr handle);

        [DllImport("libc")]
        public static extern int open(string pathname, int flags);

        [DllImport("libc")]
        public static extern int fstat(int fd, out stat buf);

        [DllImport("libc")]
        public static extern IntPtr mmap(IntPtr addr, ulong length, int prot, int flags, int fd, ulong offset);

        [DllImport("libc")]
        public static extern int munmap(IntPtr addr, ulong length);

        [DllImport("libc")]
        public static extern int mprotect(IntPtr addr, ulong len, int prot);

        [DllImport("libc")]
        public static extern long sysconf(int name);

        [StructLayout(LayoutKind.Sequential)]
        public struct link_map
        {
            public IntPtr l_addr;
            public IntPtr l_name;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ElfW
        {
            public struct Ehdr
            {
                public byte e_shnum;
                public uint e_shoff;
                public ushort e_phnum;
                public uint e_phoff;
            }

            public struct Phdr
            {
                public int p_type;
                public int p_flags;

                public ulong p_vaddr;
                public ulong p_filesz;
            }

            public struct Shdr
            {
                public uint sh_name;
                public uint sh_offset;
                public uint sh_size;
                public ulong sh_addr;
            }
        }

        [StructLayout(LayoutKind.Sequential)]

        public struct stat
        {
            public ulong st_size;
        }
    }
    #pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    #pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
}