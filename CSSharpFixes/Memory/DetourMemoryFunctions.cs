using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace CSSharpFixes.Memory;

public static class DetourMemoryFunctions
{
    static DetourMemoryFunctions()
    {
        Add<MemoryFunctionVoid<IntPtr, IntPtr, int, bool, float, IntPtr>>("ProcessUsercmds");
    }

    private static Dictionary<string, BaseMemoryFunction> _memoryFunctions = new();
    
    private static void Add<T>(string signatureName) where T: BaseMemoryFunction
    {
        _memoryFunctions.Add(signatureName, Utils.BuildMemoryFunction<T>(signatureName));
    }
    
    public static BaseMemoryFunction Get(string signatureName)
    {
        if(!_memoryFunctions.TryGetValue(signatureName, out BaseMemoryFunction? memoryFunction))
            throw new InvalidOperationException($"Memory function for signature {signatureName} not found.");

        return memoryFunction;
    }
}