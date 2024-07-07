using System.Runtime.InteropServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CSSharpFixes.Enums.Detours.ProcessUserCmds;
using CSSharpFixes.Extensions;
using CSSharpFixes.Models;
using Microsoft.Extensions.Logging;

namespace CSSharpFixes.Detours;

public class ProcessUserCmdsHandler : PreHandler
{
    private LogLevel _logLevel = LogLevel.Trace;
    private TraceLevel _traceLevel = TraceLevel.SubTickMoveSetWhenOnly;
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
        IntPtr playerPtr = h.GetParam<IntPtr>(0);
        CCSPlayerController tempPlayer = new (playerPtr);
        CCSPlayerController? player = Utilities.GetPlayerFromSlot(tempPlayer.Slot);
        if(!player.IsCompletelyValid() || !player.IsAlive() || player?.PlayerPawn.Value == null 
           || player.IsBot || !player.IsOnATeam()) return HookResult.Continue;
        
        CCSPlayerPawn playerPawn = player.PlayerPawn.Value;
        CPlayer_MovementServices? movementServices = playerPawn.MovementServices;
        if(movementServices == null) return HookResult.Continue;
        
        if (playerPawn.MoveType != MoveType_t.MOVETYPE_WALK && playerPawn.MoveType != MoveType_t.MOVETYPE_PUSH)
            return HookResult.Continue;
        
        //_logger.LogInformation("[OnProcessUsercmds][MoveType={0}]", playerPawn.MoveType);
        
        if (_logLevel == LogLevel.Trace && _traceLevel >= TraceLevel.ReadParams)
        {
            _logger.LogInformation("[OnProcessUsercmds][CUserCmd*={0}]", h.GetParam<IntPtr>(1));
            _logger.LogInformation("[OnProcessUsercmds][numCommands={1}]", h.GetParam<int>(2));
        }
        
        // get the number of commands
        int numCommands = h.GetParam<int>(2);
        
        // CUserCmd* (In my C# structs, either CUserCmdUnix or CUserCmdWindows depending on platform)
        // The separate structs are necessary because of the different sizes of the structs and
        // to allow this to be done at runtime instead of compile time like #ifdef _WIN32 in cs2fixes
        // https://github.com/maecry/asphyxia-cs2/blob/master/cstrike/sdk/datatypes/usercmd.h#L262-L291
        IntPtr cmdsPtr = h.GetParam<IntPtr>(1);
        
        for (ulong cmdIdx = 0; cmdIdx < (ulong)numCommands; cmdIdx++)
        {
            // Unsafe block to work with pointers directly
            unsafe
            {
                IntPtr cmdPtr = (IntPtr)((ulong)cmdsPtr + cmdIdx * 0x80);
                IntPtr cmdBasePtrPtr = (IntPtr)((ulong)cmdPtr + 0x30);
                if(cmdBasePtrPtr == IntPtr.Zero) continue;
                IntPtr cmdBasePtr = Marshal.ReadIntPtr(cmdBasePtrPtr);
                if(cmdBasePtr == IntPtr.Zero) continue;
                IntPtr forwardMovePtr = (IntPtr)((ulong)cmdBasePtr + 0x50);
                if(forwardMovePtr == IntPtr.Zero) continue;
                float* forwardMove = (float*)forwardMovePtr.ToPointer();
                // if(*forwardMove > 0.0f) _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][forwardMove={1}]",
                //     cmdIdx, *forwardMove);
                // 0x18 = sizeof BasePB
                // 0x8 = sizeof uint (void* pArena)
                // 0x8 = sizeof int (current_size)
                // 0x8 = sizeof int (total_size)
                // 0x8 = sizeof Rep<T>*
                IntPtr currentSizePtr = (IntPtr)((ulong)cmdBasePtr + 0x18 + 0x8);
                if(currentSizePtr == IntPtr.Zero) continue;
                int* currentSize = (int*)currentSizePtr.ToPointer();
                if(*currentSize < 1) continue;
                // if(*currentSize > 0) _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][currentSize={1}]",
                //     cmdIdx, *currentSize);
                IntPtr repPtrPtr = (IntPtr)((ulong)cmdBasePtr + 0x18 + 0x8 + 0x8 + 0x8);
                if(repPtrPtr == IntPtr.Zero) continue;
                IntPtr repPtr = Marshal.ReadIntPtr(repPtrPtr);
                if(repPtr == IntPtr.Zero) continue;
                IntPtr elementsPtr = (IntPtr)((ulong)repPtr + 0x8);
                if(elementsPtr == IntPtr.Zero) continue;

                ulong sizeOfSubTickMoveStep = 0x30;
                
                for (int subTickMoveIdx = 0; subTickMoveIdx < *currentSize; subTickMoveIdx++)
                {
                    IntPtr subTickMovePtr = (IntPtr)((ulong)elementsPtr + (ulong)subTickMoveIdx * sizeOfSubTickMoveStep);
                    if(subTickMovePtr == IntPtr.Zero) continue;
                    // 0x18 = sizeof BasePB
                    // 0x8 = sizeof ulong (button)
                    // 0x1 = sizeof bool (pressed)
                    // 0x4 = sizeof float (when)
                    // 0x4 = sizeof float (analog_forward_delta)
                    // 0x4 = sizeof float (analog_left_delta)
                    IntPtr whenPtr = (IntPtr)((ulong)subTickMovePtr + 0x18 + 0x8 + 0x1);
                    if(whenPtr == IntPtr.Zero) continue;
                    float* when = (float*)whenPtr.ToPointer();
                    if(*when > 0.0f) 
                    {
                        // _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][subTickMoveIdx={1}][whenPre={2}]",
                        // cmdIdx, subTickMoveIdx, *when);
                        *when = 0.0f;
                        // _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][subTickMoveIdx={1}][whenPost={2}]",
                        //     cmdIdx, subTickMoveIdx, *when);
                    }
                    
                }
            }
        }
        return HookResult.Changed;
    }
}