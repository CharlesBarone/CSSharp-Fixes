/*
 * SubTickMovementFix.cs
 * Author: Charles Barone <CharlesBarone> / hypnos (Discord: hyyps)
 * Email: contact@charlesbar.one
 * https://hyps.dev
 * 
 * Description: Disables subtick movement for all players on the server.
 * 
 * This fix is not "really" a fix, but rather it disables subtick movement.
 * This is a prerequisite for TriggerPushFix.
 */

namespace CSSharpFixes.Fixes;

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