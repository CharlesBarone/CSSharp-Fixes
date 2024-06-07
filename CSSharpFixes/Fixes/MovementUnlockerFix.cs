namespace CSSharpFixes.Fixes;

public class MovementUnlockerFix: BaseFix
{
    public MovementUnlockerFix()
    {
        Name = "MovementUnlockerFix";
        ConfigurationProperty = "EnableMovementUnlocker";
        PatchNames =
        [
            "ServerMovementUnlock"
        ];
    }
}