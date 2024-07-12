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

using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CSSharpFixes.Extensions;
using CSSharpFixes.Models;
using CSSharpFixes.Schemas.Protobuf;
using Microsoft.Extensions.Logging;
using CBaseUserCmdPB = CSSharpFixes.Schemas.Protobuf.CBaseUserCmdPB;

namespace CSSharpFixes.Detours;

public class ProcessUserCmdsHandler : PreHandler
{
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
        
        // get the number of commands
        int numCommands = h.GetParam<int>(2);
        
        // https://github.com/maecry/asphyxia-cs2/blob/master/cstrike/sdk/datatypes/usercmd.h#L262-L291
        IntPtr cmdsPtr = h.GetParam<IntPtr>(1);
        
        for (ulong cmdIdx = 0; cmdIdx < (ulong)numCommands; cmdIdx++)
        {
            IntPtr cmdPtr = (IntPtr)((ulong)cmdsPtr + cmdIdx * CUserCmd.Size());
            CUserCmd cUserCmd = new(cmdPtr);
            
            // if(cUserCmd.LeftHandDesired != null)
            //     _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][LeftHandDesired={1}]",
            //         cmdIdx, cUserCmd.LeftHandDesired);
            
            CBaseUserCmdPB? cBaseUserCmdPb = cUserCmd.Base;
            if(cBaseUserCmdPb == null) continue;

            // if(cBaseUserCmdPb.CommandNumber != null && cBaseUserCmdPb.CommandNumber != 0)
            //     _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][CommandNumber={1}]",
            //         cmdIdx, cBaseUserCmdPb.CommandNumber.Value.ToString() );
            // if(cBaseUserCmdPb.ClientTick != null && cBaseUserCmdPb.ClientTick != 0)
            //     _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][ClientTick={1}]",
            //         cmdIdx, cBaseUserCmdPb.ClientTick.Value.ToString() );
            // if (cBaseUserCmdPb.ForwardMove != null && cBaseUserCmdPb.ForwardMove != 0.0f)
            //     _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][ForwardMove={1}]",
            //         cmdIdx, cBaseUserCmdPb.ForwardMove.Value);
            // if(cBaseUserCmdPb.SideMove != null && cBaseUserCmdPb.SideMove != 0.0f)
            //     _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][SideMove={1}]",
            //         cmdIdx, cBaseUserCmdPb.SideMove.Value );
            // if(cBaseUserCmdPb.UpMove != null && cBaseUserCmdPb.UpMove != 0.0f)
            //     _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][UpMove={1}]",
            //         cmdIdx, cBaseUserCmdPb.UpMove.Value );
            // if(cBaseUserCmdPb.Impulse != null && cBaseUserCmdPb.Impulse != 0)
            //     _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][Impulse={1}]",
            //         cmdIdx, cBaseUserCmdPb.Impulse.Value.ToString() );
            // if(cBaseUserCmdPb.WeaponSelect != null && cBaseUserCmdPb.WeaponSelect != 0)
            //     _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][WeaponSelect={1}]",
            //         cmdIdx, cBaseUserCmdPb.WeaponSelect.Value.ToString() );
            
            // if(*currentSize > 0) _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][currentSize={1}]",
            //     cmdIdx, *currentSize);
            
            Schemas.Protobuf.Interop.RepeatedPtrField<CSubTickMoveStep>? subTickMoves = cBaseUserCmdPb.SubtickMoves;
            if(subTickMoves == null) continue;
            if(subTickMoves.CurrentSize == null || subTickMoves.CurrentSize == 0) continue;
            
            // if(subTickMoves.CurrentSize != null && subTickMoves.CurrentSize != 0)
            //     _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][CurrentSize={1}]",
            //         cmdIdx, subTickMoves.CurrentSize.Value );
            // if(subTickMoves.TotalSize != null && subTickMoves.TotalSize != 0)
            //     _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][TotalSize={1}]",
            //         cmdIdx, subTickMoves.TotalSize.Value );

            Schemas.Protobuf.Interop.Rep<CSubTickMoveStep>? rep = subTickMoves.Rep;
            if(rep == null) continue;
            
            // if(rep.AllocatedSize != null && rep.AllocatedSize != 0)
            //     _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][AllocatedSize={1}]",
            //         cmdIdx, rep.AllocatedSize.Value );
            
            for (int subTickMoveIdx = 0; subTickMoveIdx < subTickMoves.CurrentSize; subTickMoveIdx++)
            {
                IntPtr? subTickMoveAddress = subTickMoves[subTickMoveIdx];
                if(subTickMoveAddress == null) continue;
                CSubTickMoveStep subTickMove = new((IntPtr)subTickMoveAddress);
                if(subTickMove.Pressed == null) continue;
                if(subTickMove.When == null) continue;
                
                if(subTickMove.When > 0.0f)
                {
                    // There's some weird case where this can happen if you constantly walk against a trigger push.
                    // This should never be a huge number if it is, skip it...
                    if(subTickMove.When > 1000.0f) continue; 
                    if(subTickMove.When < 0.0001) continue;
                    
                    // if(subTickMove.Pressed != null && subTickMove.Pressed == true)
                    //     _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][subTickMoveIdx={1}][Pressed={2}]",
                    //         cmdIdx, subTickMoveIdx, subTickMove.Pressed.Value);
                    
                    // _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][subTickMoveIdx={1}][whenPre={2}]",
                    //     cmdIdx, subTickMoveIdx, subTickMove.When);

                    subTickMove.When = 0.0f;
                    
                    // _logger.LogInformation("[OnProcessUsercmds][cmdIdx={0}][subTickMoveIdx={1}][whenPost={2}]",
                    //     cmdIdx, subTickMoveIdx, subTickMove.When);
                }
            }
        }
        return HookResult.Changed;
    }
}