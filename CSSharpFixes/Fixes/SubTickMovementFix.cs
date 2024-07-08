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

namespace CSSharpFixes.Fixes;

/*
 * Description: Disables subtick movement for all players on the server.
 *
 * This fix is not "really" a fix, but rather it disables subtick movement.
 * This is a prerequisite for TriggerPushFix.
 */
public class SubTickMovementFix: BaseFix
{
    public SubTickMovementFix()
    {
        Name = "SubTickMovementFix";
        ConfigurationProperty = "DisableSubTickMovement";
        DetourHandlerNames =
        [
            "ProcessUserCmdsHandler"
        ];
    }
}