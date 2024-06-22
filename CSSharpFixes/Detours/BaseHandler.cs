using System.Reflection;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using Microsoft.Extensions.Logging;

namespace CSSharpFixes.Detours;

public abstract class BaseHandler
{
    public string Name { get; set; }

    public abstract Enums.Detours.Mode Mode { get; }
    public abstract Models.Detour PreDetour { get; }
    public abstract Models.Detour PostDetour { get; }

    protected readonly ILogger<CSSharpFixes> _logger;
    
    public abstract void Start();
    public abstract void Stop();
    protected abstract void UnhookAllDetours();
    
    protected BaseHandler(string name, ILogger<CSSharpFixes> logger)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));;
    }
    
    private static BaseMemoryFunction GetMemoryFunction<T>() where T : BaseHandler
    {
        MethodInfo? getMemoryFunctionMethod = typeof(T).GetMethod("GetMemoryFunction");
        if(getMemoryFunctionMethod == null)
            throw new InvalidOperationException($"Class {typeof(T).Name} must define a static GetMemoryFunction method.");
        
        return getMemoryFunctionMethod.Invoke(null, null) as BaseMemoryFunction
               ?? throw new InvalidOperationException();
    }
    
    public static T Build<T>(ILogger<CSSharpFixes> logger) where T : BaseHandler
    {
        MethodInfo? buildMethod = typeof(T).GetMethod("Build", new Type[] { typeof(ILogger<CSSharpFixes>) });
        if (buildMethod == null || !buildMethod.IsStatic)
            throw new InvalidOperationException($"Class {typeof(T).Name} must define a static Build method.");
        
        return buildMethod.Invoke(null, [logger]) as T ?? throw new InvalidOperationException();
    }
}

public abstract class PreHandler : BaseHandler
{
    public override Enums.Detours.Mode Mode => Enums.Detours.Mode.Pre;
    
    public override Models.Detour PreDetour { get; }
    public override Models.Detour PostDetour 
        => throw new NotSupportedException("PostDetour is only available if Mode is set to Post or Both.");
    
    protected override void UnhookAllDetours()
    {
        if(PreDetour.IsHooked()) PreDetour.Unhook();
    }
    
    protected PreHandler(string name, Models.Detour preDetour, ILogger<CSSharpFixes> logger) : base(name, logger)
    {
        PreDetour = preDetour ?? throw new ArgumentNullException(nameof(preDetour));
    }
}

public abstract class PostHandler : BaseHandler
{
    public override Enums.Detours.Mode Mode => Enums.Detours.Mode.Post;
    
    public override Models.Detour PreDetour 
        => throw new NotSupportedException("PreDetour is only available if Mode is set to Pre or Both.");
    public override Models.Detour PostDetour { get; }
    
    protected override void UnhookAllDetours()
    {
        if(PostDetour.IsHooked()) PostDetour.Unhook();
    }
    
    protected PostHandler(string name, Models.Detour postDetour, ILogger<CSSharpFixes> logger) : base(name, logger)
    {
        PostDetour = postDetour ?? throw new ArgumentNullException(nameof(postDetour));
    }
}

public abstract class PrePostHandler : BaseHandler
{
    public override Enums.Detours.Mode Mode => Enums.Detours.Mode.Both;
    
    public override Models.Detour PreDetour { get; }
    public override Models.Detour PostDetour { get; }

    protected override void UnhookAllDetours()
    {
        if(PreDetour.IsHooked()) PreDetour.Unhook();
        if(PostDetour.IsHooked()) PostDetour.Unhook();
    }

    protected PrePostHandler(string name, Models.Detour preDetour, Models.Detour postDetour, ILogger<CSSharpFixes> logger) 
        : base(name, logger)
    {
        PreDetour = preDetour ?? throw new ArgumentNullException(nameof(preDetour));
        PostDetour = postDetour ?? throw new ArgumentNullException(nameof(postDetour));
    }
}