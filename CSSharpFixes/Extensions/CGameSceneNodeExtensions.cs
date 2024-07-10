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
using CounterStrikeSharp.API.Modules.Utils;

namespace CSSharpFixes.Extensions;

public static class CGameSceneNodeExtensions
{
    public static float[,]? EntityToWorldTransform(this CGameSceneNode? sceneNode)
    {
        if (sceneNode is null) return null;
        
        // matrix3x4_t
        float[,] mat = new float[3, 4];

        QAngle angles = sceneNode.AbsRotation;
        float sr, sp, sy, cr, cp, cy;
        
        Utils.SinCos(Utils.DegToRad(angles.Y), out sy, out cy); // YAW
        Utils.SinCos(Utils.DegToRad(angles.X), out sp, out cp); // PITCH
        Utils.SinCos(Utils.DegToRad(angles.Z), out sr, out cr); // ROLL

        mat[0, 0] = cp * cy;
        mat[1, 0] = cp * sy;
        mat[2, 0] = -sp;

        float crcy = cr * cy;
        float crsy = cr * sy;
        float srcy = sr * cy;
        float srsy = sr * sy;
        
        mat[0, 1] = sp * srcy - crsy;
        mat[1, 1] = sp * srsy + crcy;
        mat[2, 1] = sr * cp;
        
        mat[0, 2] = sp * crcy + srsy;
        mat[1, 2] = sp * crsy - srcy;
        mat[2, 2] = cr * cp;

        Vector pos = sceneNode.AbsOrigin;
        mat[0, 3] = pos.X;
        mat[1, 3] = pos.Y;
        mat[2, 3] = pos.Z;

        return mat;
    }
}