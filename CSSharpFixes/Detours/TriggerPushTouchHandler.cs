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
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using CSSharpFixes.Extensions;
using CSSharpFixes.Models;
using Microsoft.Extensions.Logging;

namespace CSSharpFixes.Detours;

public class TriggerPushTouchHandler : PreHandler
{
    private TriggerPushTouchHandler(string name, Detour preDetour, ILogger<CSSharpFixes> logger)
        : base(name, preDetour, logger) {}
    
    ~TriggerPushTouchHandler() { Stop(); }
    
    // Gets called in BaseHandler by public static T Build<T>(ILogger<CSSharpFixes> logger) where T : BaseHandler
    public static TriggerPushTouchHandler Build(ILogger<CSSharpFixes> logger)
    {
        Detour preDetour = new Detour("TriggerPushTouchPreDetour", "TriggerPush_Touch", HookMode.Pre, logger);
        return new TriggerPushTouchHandler("TriggerPushTouchHandler", preDetour, logger);
    }
    
    public override void Start()
    {
        PreDetour.Hook(TriggerPushTouchPre);
    }
    
    public override void Stop()
    {
        UnhookAllDetours();
    }
    
    private static readonly uint SF_TRIG_PUSH_ONCE = 0x80;

    private HookResult TriggerPushTouchPre(DynamicHook h)
    {
        // Get the trigger_push entity
        IntPtr triggerPushPtr = h.GetParam<IntPtr>(0);
        if(triggerPushPtr == IntPtr.Zero) return HookResult.Continue;
        CTriggerPush triggerPush = new(triggerPushPtr);
        
        // This trigger pushes only once (and kills itself) or pushes only on StartTouch,
        // both of which are fine already, return to the original function
        if(!triggerPush.IsValid) return HookResult.Continue;
        if((triggerPush.Spawnflags & SF_TRIG_PUSH_ONCE) > 0) return HookResult.Continue;
        if(triggerPush.TriggerOnStartTouch) return HookResult.Continue;
        
        // Get the entity being pushed
        IntPtr otherPtr = h.GetParam<IntPtr>(1);
        if(otherPtr == IntPtr.Zero) return HookResult.Continue;
        CBaseEntity other = new(otherPtr);
        if(!other.IsValid) return HookResult.Continue;
        
        // Get the entity's move type. 
        MoveType_t moveType = other.ActualMoveType;
        
        // VPhysics handling doesn't need any changes
        if(moveType == MoveType_t.MOVETYPE_VPHYSICS) return HookResult.Continue;

        // Do not push entities that have these move types
        if (moveType == MoveType_t.MOVETYPE_NONE || moveType == MoveType_t.MOVETYPE_PUSH ||
            moveType == MoveType_t.MOVETYPE_NOCLIP)
            return HookResult.Handled;
        
        // Do not push entities that are not solid
        CCollisionProperty? collision = other.Collision;
        if(collision == null) return HookResult.Handled;
        if(!Utils.IsSolid(collision)) return HookResult.Handled;

        if(!triggerPush.PassesTriggerFilters(other.Handle)) return HookResult.Handled;

        // Cancel if the entity being pushed has a parent scene node
        CBodyComponent? otherBodyComponent = other.CBodyComponent;
        if(otherBodyComponent == null) return HookResult.Handled;
        CGameSceneNode? otherSceneNode = otherBodyComponent.SceneNode;
        if(otherSceneNode == null) return HookResult.Handled;
        CGameSceneNode? parentSceneNode = otherSceneNode.PParent;
        if(parentSceneNode != null) return HookResult.Handled;
        
        // Get the push entity's scene node
        CBodyComponent? pushBodyComponent = triggerPush.CBodyComponent;
        if(pushBodyComponent == null) return HookResult.Handled;
        CGameSceneNode? pushSceneNode = pushBodyComponent.SceneNode;
        if(pushSceneNode == null) return HookResult.Handled;
        
        Vector vecAbsDir = new();
        // matrix3x4_t
        float[,]? matTransform = pushSceneNode.EntityToWorldTransform();
        if(matTransform == null) return HookResult.Handled;
        
        // _logger.LogInformation("[TriggerPushFix] moveType = {moveType}", moveType);

        Vector vecPushDir = triggerPush.PushDirEntitySpace;
        // _logger.LogInformation("[TriggerPushFix] vecPushDir = {vecPushDir}", vecPushDir);
        Utils.VectorRotate(vecPushDir, matTransform, ref vecAbsDir);
        // _logger.LogInformation("[TriggerPushFix] vecAbsDir = {vecAbsDir}", vecAbsDir);

        Vector vecPush = vecAbsDir * triggerPush.Speed;
        // _logger.LogInformation("[TriggerPushFix] triggerPush.Speed = {triggerPushSpeed}", triggerPush.Speed);
        // _logger.LogInformation("[TriggerPushFix] vecPush = {vecPush}", vecPush);

        uint flags = other.Flags;
        // _logger.LogInformation("[TriggerPushFix] flags = {flags}", flags);
        // _logger.LogInformation("[TriggerPushFix] other.BaseVelocity = {BaseVelocity}", other.BaseVelocity);
        
        // FL_BASEVELOCITY missing from CS# PlayerFlags Enum
        //https://github.com/alliedmodders/hl2sdk/blob/67ba01d05038f55448fa792c09c9aae3d0bb8263/public/const.h#L133
        const uint flBasevelocity = 1 << 23;
        if((flags & flBasevelocity) > 0) vecPush += other.BaseVelocity;
        // _logger.LogInformation("[TriggerPushFix] vecPush = {vecPush}", vecPush);
        
        if(vecPush.Z > 0.0f && (flags & (uint)PlayerFlags.FL_ONGROUND) > 0)
        {
            other.SetGroundEntity(IntPtr.Zero);
            Vector? origin = other.AbsOrigin;
            if (origin != null)
            {
                origin.Z += 1.0f;
                other.TeleportPositionOnly(origin);
            }
        }

        // Vector vecOriginalPush = vecAbsDir * triggerPush.Speed;
        // _logger.LogInformation(
        //     "[TriggerPushFix] Pushing entity {entity} | entity basevelocity {flag} = {entBaseVel} | original push velocity = {original} | final push velocity = {final}",
        //     other.Index,
        //     (flags & flBasevelocity) > 0 ? "WITH FLAG":"",
        //     other.BaseVelocity,
        //     vecOriginalPush,
        //     vecPush
        // );
        
        // _logger.LogInformation("[TriggerPushFix] PRE other.BaseVelocity = {BaseVelocity}", other.BaseVelocity);
        other.SetBaseVelocity(vecPush);
        Utilities.SetStateChanged(other, "CBaseEntity", "m_vecBaseVelocity");
        // _logger.LogInformation("[TriggerPushFix] POST other.BaseVelocity = {BaseVelocity}", other.BaseVelocity);
        
        flags |= flBasevelocity;
        other.Flags = flags;
        
        // _logger.LogInformation("TriggerPushTouch [Finished Fix]");
        return HookResult.Handled;
    }
}