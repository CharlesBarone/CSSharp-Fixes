using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CSSharpFixes.Models;
using CSSharpFixes.Schemas;
using Microsoft.Extensions.Logging;

namespace CSSharpFixes.Detours;

public class ProcessUserCmdsHandler : PreHandler
{
    private LogLevel _logLevel = LogLevel.Trace;
    private ProcessUserCmdsHandler(string name, Detour preDetour, ILogger<CSSharpFixes> logger)
        : base(name, preDetour, logger) {}
    
    ~ProcessUserCmdsHandler() { Stop(); }
    
    // Gets called in BaseHandler by public static T Build<T>(ILogger<CSSharpFixes> logger) where T : BaseHandler
    public static ProcessUserCmdsHandler Build(ILogger<CSSharpFixes> logger)
    {
        Detour preDetour = new Detour("ProcessUserCmdsPreDetour", "ProcessUsercmds", HookMode.Pre, logger);
        return new ProcessUserCmdsHandler("ProcessUserCmdsHandler", preDetour, logger);
    }
    
    public override void Start()
    {
        PreDetour.Hook(ProcessUserCmdsPre);
    }
    
    public override void Stop()
    {
        UnhookAllDetours();
    }

    private HookResult ProcessUserCmdsPre(DynamicHook h)
    {
        if (_logLevel == LogLevel.Trace)
        {
            _logger.LogInformation("[OnProcessUsercmds][CUserCmd*={0}]",
                h.GetParam<IntPtr>(1));
            _logger.LogInformation("[OnProcessUsercmds][numCommands={1}]",
                h.GetParam<int>(2));
        }
        
        // get the number of commands
        var numCommands = h.GetParam<int>(2);
        
        // CUserCmd* (In my C# structs, either CUserCmdUnix or CUserCmdWindows depending on platform)
        // The separate structs are necessary because of the different sizes of the structs and
        // to allow this to be done at runtime instead of compile time like #ifdef _WIN32 in cs2fixes
        // https://github.com/maecry/asphyxia-cs2/blob/master/cstrike/sdk/datatypes/usercmd.h#L262-L291
        IntPtr cmds = h.GetParam<IntPtr>(1);
        
        for (int cmdIdx = 0; cmdIdx < numCommands; cmdIdx++)
        {
            // Do pointer arithmetic to get the address of the CUserCmd
            //CUserCmdUnix = 0x80
            //CUserCmdWindows = 0x88
            IntPtr cmdPtr = (IntPtr)((long)cmds + 0x80 * cmdIdx);
            
            if(cmds == IntPtr.Zero && _logLevel == LogLevel.Trace)
                _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][CalculatedPointer=nullptr][Skipping!]",
                    cmdIdx);
            
            if(cmds == IntPtr.Zero) continue;
            
            if(_logLevel == LogLevel.Trace)
                _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][CalculatedPointer={1}]",
                    cmdIdx, cmdPtr);
            
            // Cast IntPtr to CUserCmdUnix
            CUserCmdUnix? cmd = Marshal.PtrToStructure<CUserCmdUnix>(cmdPtr);
            if(cmd == null) continue;
            //CUserCmdUnix cmd = Utils.ReinterpretCast<IntPtr, CUserCmdUnix>(ptr);
            
            //_logger.LogInformation("[OnProcessUsercmds][cmdIdx={x}][CUserCmdUnix::TickCount={value}]", cmdIdx, cmd.Value.tick_count);
            
            //IntPtr basePtr = cmd.@base;
            IntPtr basePtr = cmd.Value.@base;
            if(basePtr == IntPtr.Zero) continue;
            
            if(_logLevel == LogLevel.Trace)
                _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][CUserCmdUnix::BasePtr={1}]",
                    cmdIdx, basePtr);
            
            // Unsafe block to work with pointers directly
            unsafe
            {
                //CBaseUserCmdPB* baseCmdPtr = (CBaseUserCmdPB*)cmd.@base.ToPointer();
                
                // Dereference cmd.@base to get CBaseUserCmdPB*
                CBaseUserCmdPB* baseCmdPtr = (CBaseUserCmdPB*)cmd.Value.@base.ToPointer();
                if(baseCmdPtr == null) continue;
                
                // Access subtick_moves through pointer arithmetic
                //RepeatedPtrField<CSubtickMoveStep> subtick_moves = (*baseCmdPtr).subtick_moves;
                
                int subtick_moves_count = baseCmdPtr->subtick_moves.current_size;
                if(subtick_moves_count < 1) continue; // No need to continue if there are no subtick moves
                
                if(_logLevel == LogLevel.Trace)
                    _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][SubtickMovesCount={1}]",
                        cmdIdx, subtick_moves_count);
                
                for (int SubtickMoveIdx = 0; SubtickMoveIdx < subtick_moves_count; SubtickMoveIdx++)
                {
                    if(_logLevel == LogLevel.Trace)
                        _logger.LogInformation(
                            "[OnProcessUsercmds][cmdIdx={0}][SubtickMoveIdx={1}][SubtickMovePre={2}]",
                            cmdIdx, SubtickMoveIdx,
                            (*baseCmdPtr).subtick_moves.rep.elements[SubtickMoveIdx].when);
                    
                    // Causes a seg fault so no bueno... thus do the fixed block below.
                    // Leaving this here as a note to future self as to why i don't just do this...
                    // (*baseCmdPtr).subtick_moves.rep.elements[i].when = 0.0f;
                    
                    // Skip if when is already 0.0f
                    if((*baseCmdPtr).subtick_moves.rep.elements[SubtickMoveIdx].when == 0.0f) continue;
                    
                    // Get pointer to when field
                    fixed (float* whenPtr = &(*baseCmdPtr).subtick_moves.rep.elements[SubtickMoveIdx].when)
                    {
                        IntPtr whenAddress = (IntPtr)whenPtr;
                        
                        if(_logLevel == LogLevel.Trace)
                            _logger.LogInformation(
                                "[OnProcessUsercmds][cmdIdx={0}][SubtickMoveIdx={1}][SubtickMoveWhenPtr={2}]",
                                cmdIdx, SubtickMoveIdx, whenAddress);
                        
                        //Unsafe.Write(whenPtr, 0.0f);
                        Utils.WriteBytesToAddress(whenAddress, Utils.FloatToByteArray(0.0f));
                    }
                    
                    if(_logLevel == LogLevel.Trace)
                        _logger.LogInformation(
                            "[OnProcessUsercmds][cmdIdx={0}][SubtickMoveIdx={1}][SubtickMovePost={2}]",
                            cmdIdx, SubtickMoveIdx,
                            (*baseCmdPtr).subtick_moves.rep.elements[SubtickMoveIdx].when);
                }
            }
        }
        return HookResult.Changed;
    }
}