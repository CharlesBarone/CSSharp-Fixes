using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CSSharpFixes.Memory;
using Microsoft.Extensions.Logging;

namespace CSSharpFixes.Models;

public class Detour
{
    private string _name;
    private string _signatureName;
    private HookMode _hookMode;
    private bool _isHooked;
    public BaseMemoryFunction MemoryFunction;
    private List<Func<DynamicHook, HookResult>> _handlers;
    
    private readonly ILogger<CSSharpFixes> _logger;
    
    public Detour(string name, string signatureName, HookMode hookMode, ILogger<CSSharpFixes> logger)
    {
        _name = name;
        _signatureName = signatureName;
        _hookMode = hookMode;
        _logger = logger;
        _isHooked = false;
        _handlers = new List<Func<DynamicHook, HookResult>>();
        MemoryFunction = DetourMemoryFunctions.Get(_signatureName);
    }
    
    ~Detour()
    {
        if(_isHooked)
        {
            Unhook();
        }
    }
    
    public bool IsHooked() { return _isHooked; }
    public string GetName() { return _name; }
    public string GetSignatureName() { return _signatureName; }
    public string GetSignature() { return GameData.GetSignature(_signatureName); }
    public HookMode GetHookMode() { return _hookMode; }
    
    public void Hook(Func<DynamicHook, HookResult> handler, bool hookOnce = false)
    {
        if(hookOnce && _isHooked) return;
        MemoryFunction.Hook(handler, _hookMode);
        _isHooked = true;
        _handlers.Add(handler);
    }
    
    public void Unhook()
    {
        if(!_isHooked) return;

        foreach(Func<DynamicHook, HookResult> handler in _handlers)
        {
            MemoryFunction.Unhook(handler, _hookMode);
        }
        
        _handlers.Clear();
        _isHooked = false;
    }
}