using System.Runtime.InteropServices;
using System.Text;

namespace CSSharpFixes;

public class Utils
{
    
    // Mimic the behavior of the C++ function std::vector<int16_t> CGameConfig::HexToByte(std::string_view src)
    public static List<byte> HexToByte(string src)
    {
        if (string.IsNullOrEmpty(src))
        {
            return new List<byte>();
        }

        Func<char, byte> hexCharToByte = c =>
        {
            if (c >= '0' && c <= '9') return (byte)(c - '0');
            if (c >= 'A' && c <= 'F') return (byte)(c - 'A' + 10);
            if (c >= 'a' && c <= 'f') return (byte)(c - 'a' + 10);
            return 0xFF; // Invalid hex character
        };

        List<byte> result = new List<byte>();
        bool isCodeStyle = src[0] == '\\';
        string pattern = isCodeStyle ? "\\x" : " ";
        string wildcard = isCodeStyle ? "2A" : "?";
        int pos = 0;

        while (pos < src.Length)
        {
            int found = src.IndexOf(pattern, pos);
            if (found == -1)
            {
                found = src.Length;
            }

            string str = src.Substring(pos, found - pos);
            pos = found + pattern.Length;

            if (string.IsNullOrEmpty(str)) continue;

            string byteStr = str;

            if (byteStr.Substring(0, wildcard.Length) == wildcard)
            {
                result.Add(0xFF); // Representing wildcard as 0xFF
                continue;
            }

            if (byteStr.Length < 2)
            {
                return new List<byte>(); // Invalid byte length
            }

            byte high = hexCharToByte(byteStr[0]);
            byte low = hexCharToByte(byteStr[1]);

            if (high == 0xFF || low == 0xFF)
            {
                return new List<byte>(); // Invalid hex character
            }

            result.Add((byte)((high << 4) | low));
        }

        return result;
    }
    
    public static string ByteToHex(List<byte> bytes, bool useCodeStyle = false)
    {
        if (bytes == null || bytes.Count == 0)
        {
            return string.Empty;
        }

        Func<byte, char> byteToHexChar = b =>
        {
            if (b < 10) return (char)(b + '0');
            return (char)(b - 10 + 'A');
        };

        StringBuilder result = new StringBuilder();
        string pattern = useCodeStyle ? "\\x" : " ";

        foreach (byte b in bytes)
        {
            if (b == 0xFF) // Wildcard representation
            {
                result.Append(useCodeStyle ? "\\x2A" : "??");
            }
            else
            {
                result.Append(byteToHexChar((byte)(b >> 4))); // High nibble
                result.Append(byteToHexChar((byte)(b & 0x0F))); // Low nibble
                if (useCodeStyle)
                {
                    result.Append(pattern); // Separator
                }
            }
        }

        // Remove the trailing separator if using code style
        if (useCodeStyle && result.Length > 0)
        {
            result.Remove(result.Length - pattern.Length, pattern.Length);
        }

        return result.ToString();
    }
    
    public static List<byte> ReadBytesFromAddress(IntPtr address, int length)
    {
        List<byte> result = new List<byte>();
        for (int i = 0; i < length; i++)
        {
            result.Add(ReadByteFromAddress(address + i));
        }

        return result;
    }

    public static unsafe byte ReadByteFromAddress(IntPtr address)
    {
        return *(byte*)address.ToPointer();
    }
    
    public static void WriteBytesToAddress(IntPtr address, List<byte> bytes)
    {
        int patchSize = bytes.Count;
        if(patchSize == 0) throw new ArgumentException("Patch bytes list cannot be empty.");
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Memory.UnixMemoryUtils.PatchBytesAtAddress(address, bytes.ToArray(), patchSize);
        }
        else
        {
            Memory.WinMemoryUtils.PatchBytesAtAddress(address, bytes.ToArray(), patchSize);
        }
    }
    
    public static List<byte> FloatToByteArray(float value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        return bytes.ToList();
    }
    
    public static unsafe TDest ReinterpretCast<TSource, TDest>(TSource source)
    {
        var sourceRef = __makeref(source);
        var dest = default(TDest);
        var destRef = __makeref(dest);
        *(IntPtr*)&destRef = *(IntPtr*)&sourceRef;
        return __refvalue(destRef, TDest);
    }
}