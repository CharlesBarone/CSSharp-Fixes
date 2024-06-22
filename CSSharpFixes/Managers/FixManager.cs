using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CSSharpFixes.Fixes;
using Microsoft.Extensions.Logging;

namespace CSSharpFixes.Managers;

public class FixManager(PatchManager patchManager, DetourManager detourManager, ILogger<CSSharpFixes> logger)
{
    private List<BaseFix> _fixes = new();

    public void OnConfigChanged(string propertyName, object? newValue)
    {
        int index = _fixes.FindIndex(fix => fix.ConfigurationProperty == propertyName);
        if (newValue == null)
        {
            StopFix(index);
            return;
        }

        if(newValue is not bool value) return;
        
        if(value) StartFix(index);
        else StopFix(index);
    }

    private void StartFix(int index)
    {
        if(index < 0 || index >= _fixes.Count) return;

        foreach(string patchName in _fixes[index].PatchNames) patchManager.PerformPatch(patchName);
        foreach(string detourHandlerName in _fixes[index].DetourHandlerNames) detourManager.StartHandler(detourHandlerName);
    }

    private void StopFix(int index)
    {
        if(index < 0 || index >= _fixes.Count) return;
        
        foreach(string patchName in _fixes[index].PatchNames) patchManager.UndoPatch(patchName);
        foreach(string detourHandlerName in _fixes[index].DetourHandlerNames) detourManager.StopHandler(detourHandlerName);
    }
    
    public void Start()
    {
        logger.LogInformation("[CSSharpFixes][FixManager][Start()]");
        
        _fixes.Add(new WaterFix());
        _fixes.Add(new TriggerPushFix());
        _fixes.Add(new CPhysBoxUseFix());
        _fixes.Add(new BlastDamageCrashFix());
        // _fixes.Add(new NavmeshLookupLagFix()); // Commented out since it seems to cause crashes every time I test it...
        _fixes.Add(new NoBlockFix());
        _fixes.Add(new TeamMessagesFix());
        _fixes.Add(new StopSoundFix());
        _fixes.Add(new SubTickMovementFix());
        _fixes.Add(new MovementUnlockerFix());
        _fixes.Add(new FullAllTalkFix());
        _fixes.Add(new DropMapWeaponsFix());
        _fixes.Add(new EntityStringPurgeFix());
    }
    
    public void Stop()
    {
        logger.LogInformation("[CSSharpFixes][FixManager][Stop()]");
        
        foreach (BaseFix fix in _fixes)
        {
            foreach(string patchName in fix.PatchNames) patchManager.UndoPatch(patchName);
        }
        
        _fixes.Clear();
    }
    
    
}