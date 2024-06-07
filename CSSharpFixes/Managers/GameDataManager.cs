using CounterStrikeSharp.API.Core;

namespace CSSharpFixes.Managers;

public class GameDataManager
{
    private Dictionary<string /* modulePath */, Dictionary<string /* Signature */, IntPtr /* Address */>> _signatures = new();
    
    public IntPtr GetAddress(string modulePath, string signature)
    {
        if (!_signatures.TryGetValue(modulePath, out var signatures))
        {
            signatures = new Dictionary<string, IntPtr>();
            _signatures[modulePath] = signatures;
        }
        
        if (!signatures.TryGetValue(signature, out var address))
        {
            string byteString = GameData.GetSignature(signature);
            // Returns address if found, otherwise a C++ nullptr which is a IntPtr.Zero in C#
            address = NativeAPI.FindSignature(modulePath, byteString);
            signatures[signature] = address;
        }
        
        return address;
    }
    
    public void Start()
    {
        
    }
    
    public void Stop()
    {
        _signatures.Clear();
    }
}