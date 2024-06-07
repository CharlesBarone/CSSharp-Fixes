using CSSharpFixes.Fixes;
using Microsoft.Extensions.Logging;

namespace CSSharpFixes.Managers;

public class FixManager(PatchManager patchManager, ILogger<CSSharpFixes> logger)
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
    }

    private void StopFix(int index)
    {
        if(index < 0 || index >= _fixes.Count) return;
        
        foreach(string patchName in _fixes[index].PatchNames) patchManager.UndoPatch(patchName);
    }
    
    public void Start()
    {
        logger.LogInformation("[CSSharpFixes][FixManager][Start()]");
        
        _fixes.Add(new WaterFix());
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