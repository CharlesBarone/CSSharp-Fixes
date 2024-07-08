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

using System.Diagnostics.CodeAnalysis;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace CSSharpFixes.Extensions;

public static class PlayerExtensions
{
    public static bool IsCompletelyValid(this CCSPlayerController? player,
        [MaybeNullWhen(false)] out CCSPlayerPawn playerPawn,
        [MaybeNullWhen(false)] out CBasePlayerPawn pawn
    )
    {
        playerPawn = null;
        pawn = null;
        if (player is null) return false;

        if (!player.IsCompletelyValid(out playerPawn)) return false;

        if (!player.Pawn.IsValid) return false;
        pawn = player.Pawn.Value;

        if (pawn is null) return false;
        return pawn.Handle != IntPtr.Zero;
    }

    public static bool IsCompletelyValid(this CCSPlayerController? player,
        [MaybeNullWhen(false)] out CCSPlayerPawn playerPawn
    )
    {
        playerPawn = null;
        if (player is null) return false;

        if (!player.IsCompletelyValid()) return false;
        if (!player.PlayerPawn.IsValid) return false;


        playerPawn = player.PlayerPawn.Value;
        if (playerPawn is null) return false;

        return playerPawn.Handle != IntPtr.Zero;
    }

    public static bool IsCompletelyValid(this CCSPlayerController? player)
    {
        if (player is null) return false;
        if (!player.IsValid) return false;
        if (player.Handle == IntPtr.Zero) return false;
        if (player.Connected != PlayerConnectedState.PlayerConnected) return false;
        return true;
    }

    public static bool IsAlive(this CCSPlayerController? player)
    {
        if (!player.IsCompletelyValid(out var playerPawn)) return false;
        return playerPawn.LifeState == (int)LifeState_t.LIFE_ALIVE;
    }

    public static bool IsDying(this CCSPlayerController? player)
    {
        if (!player.IsCompletelyValid(out var playerPawn)) return false;
        return playerPawn.LifeState == (int)LifeState_t.LIFE_DYING;
    }

    public static bool IsDead(this CCSPlayerController? player)
    {
        if (!player.IsCompletelyValid(out var playerPawn)) return false;
        return playerPawn.LifeState == (int)LifeState_t.LIFE_DEAD;
    }

    public static bool IsTerrorist(this CCSPlayerController? player)
    {
        return player.IsOnTeam(CsTeam.Terrorist);
    }

    public static bool IsCounterTerrorist(this CCSPlayerController? player)
    {
        return player.IsOnTeam(CsTeam.CounterTerrorist);
    }

    public static bool IsOnATeam(this CCSPlayerController? player)
    {
        return player.IsTerrorist() || player.IsCounterTerrorist();
    }

    public static bool IsOnTeam(this CCSPlayerController? player, CsTeam team)
    {
        if (!player.IsCompletelyValid(out var playerPawn)) return false;
        return playerPawn.TeamNum == (byte)team;
    }
}