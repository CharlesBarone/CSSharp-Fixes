using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CSSharpFixes.Config;
using CSSharpFixes.Managers;

namespace CSSharpFixes;

public partial class CSSharpFixes: BasePlugin
{
    public override string ModuleName => _moduleInformation.ModuleName;
    public override string ModuleVersion => _moduleInformation.ModuleVersion;
    public override string ModuleAuthor => _moduleInformation.ModuleAuthor;
    public override string ModuleDescription => _moduleInformation.ModuleDescription;
    
    private readonly ModuleInformation _moduleInformation;
    private readonly Configuration _configuration;
    
    private readonly GameDataManager _gameDataManager;
    private readonly DetourManager _detourManager;
    private readonly PatchManager _patchManager;
    private readonly FixManager _fixManager;
    
    public CSSharpFixes(ModuleInformation moduleInformation, GameDataManager gameDataManager, DetourManager detourManager,
        PatchManager patchManager, FixManager fixManager, Configuration configuration)
    {
        _moduleInformation = moduleInformation;
        
        _gameDataManager = gameDataManager;
        _detourManager = detourManager;
        _patchManager = patchManager;
        _fixManager = fixManager;
        
        _configuration = configuration;
    }
    
    public override void Load(bool hotReload)
    {
        RegisterHooks();
        RegisterConVars();
        _gameDataManager.Start();
        _detourManager.Start();
        _patchManager.Start();
        _fixManager.Start();
        _configuration.Start();
        
        if (hotReload)
        {
            OnMapStart(Server.MapName);
        }
    }

    public override void Unload(bool hotReload)
    {
        UnregisterHooks();
        _fixManager.Stop();
        _patchManager.Stop();
        _detourManager.Stop();
        _gameDataManager.Stop();
    }
}