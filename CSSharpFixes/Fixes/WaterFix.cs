namespace CSSharpFixes.Fixes;

public class WaterFix: BaseFix
{
    public WaterFix()
    {
        Name = "WaterFix";
        ConfigurationProperty = "EnableWaterFix";
        PatchNames =
        [
            "FixWaterFloorJump",
            "WaterLevelGravity",
            "CategorizeUnderwater"
        ];
    }
}