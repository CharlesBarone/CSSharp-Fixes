/*
    =============================================================================
    CS#Fixes
    Copyright (C) 2023-2024 Charles Barone <CharlesBarone> / hypnos <hyps.dev>
    =============================================================================

    This program is free software; you can redistribute it and/or modify it under
    the terms of the GNU General Public License, version 3.0, as published by the
    Free Software Foundation.

    This program is distributed in the hope that it will be useful, but WITHOUT
    ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
    FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
    details.

    You should have received a copy of the GNU General Public License along with
    this program.  If not, see <http://www.gnu.org/licenses/>.
*/

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