using System.Text;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace CSSharpFixes;

public partial class CSSharpFixes
{
    private void RegisterHooks()
    {
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        RegisterListener<Listeners.OnMapStart>(OnMapStart);
        RegisterListener<Listeners.OnTick>(OnTick);
        RegisterListener<Listeners.OnServerPrecacheResources>(OnServerPrecacheResources);
    }

    private void UnregisterHooks()
    {
        RemoveListener<Listeners.OnMapEnd>(OnMapEnd);
        RemoveListener<Listeners.OnMapStart>(OnMapStart);
        RemoveListener<Listeners.OnTick>(OnTick);
        RemoveListener<Listeners.OnServerPrecacheResources>(OnServerPrecacheResources);
    }

    private void OnServerPrecacheResources(ResourceManifest manifest)
    {
        //manifest.AddResource("models/food/pizza/pizza_1.vmdl");
    }

    private void OnMapEnd()
    {
        
    }

    private void OnTick()
    {
        
    }

    private void OnMapStart(string mapName)
    {
        
    }
}