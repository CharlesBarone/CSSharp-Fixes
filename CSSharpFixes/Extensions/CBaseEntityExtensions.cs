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

using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;

namespace CSSharpFixes.Extensions;

public static class CBaseEntityExtensions
{
    public static void SetGroundEntity(this CBaseEntity? baseEntity, IntPtr groundEntityHandle)
    {
        if (baseEntity is null) return;
        
        MemoryFunctionVoid<IntPtr, IntPtr, IntPtr> setGroundEntityFunc = new(GameData.GetSignature("SetGroundEntity"));
        Action<IntPtr, IntPtr, IntPtr> setGroundEntity = setGroundEntityFunc.Invoke;
        setGroundEntity(baseEntity.Handle, groundEntityHandle, IntPtr.Zero);
    }
    
    // Add a setter for BaseVelocity to CBaseEntity so it can be set with a Vector in 1 line
    public static void SetBaseVelocity(this CBaseEntity? baseEntity, Vector baseVelocity)
    {
        if(baseEntity is null) return;
        if(!baseEntity.IsValid) return;
        baseEntity.BaseVelocity.X = baseVelocity.X;
        baseEntity.BaseVelocity.Y = baseVelocity.Y;
        baseEntity.BaseVelocity.Z = baseVelocity.Z;
    }
    
    public static void TeleportPositionOnly(this CBaseEntity? baseEntity, Vector position)
    {
        if(baseEntity is null) return;
        if(!baseEntity.IsValid) return;
        VirtualFunction.CreateVoid<IntPtr, IntPtr, IntPtr, IntPtr>(baseEntity.Handle, GameData.GetOffset("CBaseEntity_Teleport"))(
            baseEntity.Handle, position.Handle, IntPtr.Zero, IntPtr.Zero);
    }
}