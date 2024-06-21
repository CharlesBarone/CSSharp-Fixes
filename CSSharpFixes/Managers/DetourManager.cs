using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CSSharpFixes.Schemas;
using Microsoft.Extensions.Logging;

namespace CSSharpFixes.Managers;

public class DetourManager
{
    private readonly ILogger<CSSharpFixes> _logger;
    //public required IRunCommand RunCommand { get; set; }
    
    public required MemoryFunctionVoid<IntPtr, IntPtr, int, bool, float, IntPtr> ProcessUsercmds;
    
    public DetourManager(ILogger<CSSharpFixes> logger)
    {
        _logger = logger;
    }
    
    public void Start()
    {
        /* if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) RunCommand = new RunCommandWindows();
        else RunCommand = new RunCommandLinux();
        RunCommand.Hook(RunCommandHandler.Handle, HookMode.Pre);
        _logger.LogInformation("[CSSharpFixes][DetourManager][Start()][Detour=RunCommand][Signature=RunCommand}][Hook=Pre]"); */
        
        ProcessUsercmds = new(GameData.GetSignature("ProcessUsercmds"));
        ProcessUsercmds.Hook(OnProcessUsercmds, HookMode.Pre);
    }
    
    public void Stop()
    {
        /* RunCommand.Unhook(RunCommandHandler.Handle, HookMode.Pre);
        _logger.LogInformation("[CSSharpFixes][DetourManager][Stop()][Detour=RunCommand][Signature=RunCommand}][Unhook=Pre]"); */
        
        ProcessUsercmds.Unhook(OnProcessUsercmds, HookMode.Pre);
    }

    private HookResult OnProcessUsercmds(DynamicHook h)
    {
        // check if the player is a valid player
        // This is actually the CPlayerSlot not an int
        // https://github.com/gamebooster/dota2-lua-engine/blob/master/hl2sdk-dota/public/eiface.h#L93
        //var playerSlot = h.GetParam<int>(0);
        
        //convert pointer to CBasePlayerPawn
        //CCSPlayerController player = Utilities.GetPlayerFromSlot(playerSlot);
        
        //_logger.LogInformation("[OnRunCommand][{slot}]", playerSlot);
        //if (!player.IsCompletelyValid()) return HookResult.Continue;
        //_logger.LogInformation("[OnProcessUsercmds][Top]");
        //_logger.LogInformation("[OnProcessUsercmds][Param0={value}]", h.GetParam<IntPtr>(0));
        //_logger.LogInformation("[OnProcessUsercmds][Param1={value}][CUserCmd*]", h.GetParam<IntPtr>(1));
        //_logger.LogInformation("[OnProcessUsercmds][Param2={value}][cmdCount]", h.GetParam<int>(2));
        //_logger.LogInformation("[OnProcessUsercmds][Param3={value}][paused]", h.GetParam<bool>(3));
        //_logger.LogInformation("[OnProcessUsercmds][Param4={value}][margin]", h.GetParam<float>(4));
        
        // get the number of commands
        var numCommands = h.GetParam<int>(2);
        
        // pointer to commands
        // https://github.com/maecry/asphyxia-cs2/blob/master/cstrike/sdk/datatypes/usercmd.h#L262-L291
        IntPtr cmds = h.GetParam<IntPtr>(1);
        // size of CUserCmd = 0x88
        
        for (int x = 0; x < numCommands; x++)
        {
            // Calculate the pointer to the x-th CUserCmdUnix
            //CUserCmdUnix = 0x80
            //CUserCmdWindows = 0x88
            IntPtr ptr = (IntPtr)((long)cmds + 0x80 * x);
            //_logger.LogInformation("[OnProcessUsercmds][cmd={x}][CalculatedPointer={ptr}]", x, ptr);
            
            // Cast IntPtr to CUserCmdUnix
            //CUserCmdUnix? cmd = Marshal.PtrToStructure<CUserCmdUnix>(ptr);
            //if(cmd == null) continue;
            CUserCmdUnix cmd = Utils.ReinterpretCast<IntPtr, CUserCmdUnix>(ptr);
            //_logger.LogInformation(Utils.ByteToHex(Utils.ReadBytesFromAddress(ptr, 0x80)));
            
            //_logger.LogInformation("[OnProcessUsercmds][cmd={x}][CUserCmdUnix::TickCount={value}]", x, cmd.tick_count);
            //_logger.LogInformation("[OnProcessUsercmds][cmd={x}][CUserCmdUnix::unknown={value}]", x, cmd.unknown);
            //_logger.LogInformation("[OnProcessUsercmds][cmd={x}][CUserCmdUnix::BasePtr={value}]", x, cmd.@base);
            //_logger.LogInformation("[OnProcessUsercmds][cmd={x}][CUserCmdUnix::inputHistoryPtr={value}]", x, cmd.inputHistory);
            
            //Debugging Info
            //_logger.LogInformation("[OnProcessUsercmds][cmd={x}][CUserCmdUnix::TickCount={value}]", x, cmd.Value.tick_count);
            //_logger.LogInformation("[OnProcessUsercmds][cmd={x}][CUserCmdUnix::TickCount={value}]", x, cmd.tick_count);
            
            //IntPtr basePtr = cmd.Value.@base;
            IntPtr basePtr = cmd.@base;
            //_logger.LogInformation("[OnProcessUsercmds][cmd={x}][CUserCmdUnix::BasePtr={value}]", x, basePtr);
            if(basePtr == IntPtr.Zero) continue;
            
            // Unsafe block to work with pointers
            unsafe
            {
                // Dereference cmd.@base to get CBaseUserCmdPB*
                //CBaseUserCmdPB* baseCmdPtr = (CBaseUserCmdPB*)cmd.Value.@base.ToPointer();
                CBaseUserCmdPB* baseCmdPtr = (CBaseUserCmdPB*)cmd.@base.ToPointer();
                if(baseCmdPtr == null) continue;
                
                // Access subtick_moves through pointer arithmetic
                RepeatedPtrField<CSubtickMoveStep> subtick_moves = (*baseCmdPtr).subtick_moves;
                
                // Get subtick_moves count
                int subtick_moves_count = subtick_moves.current_size;
                //if(subtick_moves_count < 1) continue;
                _logger.LogInformation("[OnProcessUsercmds][cmd={x}][SubtickMovesCount={value}]", x, subtick_moves_count);
                
                for (int i = 0; i < subtick_moves_count; i++)
                {
                    //_logger.LogInformation("[OnProcessUsercmds][cmd={x}][SubtickMoveIdx={i}][SubtickMove={subTickMoveValue}]",
                    //    x, i, (*baseCmdPtr).subtick_moves.rep.elements[i].when);
                    
                    // Causes a seg fault so no bueno...
                    //(*baseCmdPtr).subtick_moves.rep.elements[i].when = 0.0f;
                    
                    if((*baseCmdPtr).subtick_moves.rep.elements[i].when == 0.0f) continue;
                    
                    // Fixed block to get the address of the 'when' field
                    fixed (float* whenPtr = &(*baseCmdPtr).subtick_moves.rep.elements[i].when)
                    {
                        IntPtr whenAddress = (IntPtr)whenPtr;
                        //_logger.LogInformation("[OnProcessUsercmds][cmd={x}][SubtickMoveIdx={i}][SubtickMoveWhenPtr={SubtickMoveWhenPtr}]",
                        //    x, i, whenAddress);
                        
                        Unsafe.Write(whenPtr, 0.0f);
                        // Convert 0.0f to byte array
                        //List<byte> zeroFloatBytes = Utils.FloatToByteArray(0.0f);

                        // Use WriteBytesToAddress to safely set 'when' to 0.0f
                        //Utils.WriteBytesToAddress(whenAddress, zeroFloatBytes);
                    }
                }
            }
        }
        return HookResult.Changed;
    }
    
    public static byte[] ReadMemory(IntPtr address, int size)
    {
        byte[] buffer = new byte[size];
        Marshal.Copy(address, buffer, 0, size);
        return buffer;
    }
}