using System.Runtime.InteropServices;
using CSSharpFixes.Detours;
using Microsoft.Extensions.Logging;

namespace CSSharpFixes.Managers;

public class DetourManager
{
    private readonly ILogger<CSSharpFixes> _logger;
    
    private Dictionary<string, BaseHandler> _handlers = new();
    
    public DetourManager(ILogger<CSSharpFixes> logger)
    {
        _logger = logger;
    }
    
    public void Start()
    {
        _handlers.Add("ProcessUserCmdsHandler", BaseHandler.Build<ProcessUserCmdsHandler>(_logger));
    }
    
    public void Stop()
    {
        StopAllHandlers();
        _handlers.Clear();
    }

    private void StartAllHandlers() { foreach(BaseHandler handler in _handlers.Values) handler.Start(); }
    private void StopAllHandlers() { foreach(BaseHandler handler in _handlers.Values) handler.Stop(); }
    
    public void StartHandler(string name)
    {
        if(_handlers.TryGetValue(name, out BaseHandler? handler)) handler.Start();
    }
    
    public void StopHandler(string name)
    {
        if(_handlers.TryGetValue(name, out BaseHandler? handler)) handler.Stop();
    }
    
    
}