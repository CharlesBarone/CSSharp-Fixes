using CounterStrikeSharp.API.Core;
using CSSharpFixes.Config;
using CSSharpFixes.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace CSSharpFixes;

public class Injection : IPluginServiceCollection<CSSharpFixes>
{
    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ModuleInformation>();
        serviceCollection.AddSingleton<GameDataManager>();
        serviceCollection.AddSingleton<PatchManager>();
        serviceCollection.AddSingleton<FixManager>();
        serviceCollection.AddSingleton<Configuration>();
    }
}