namespace CSSharpFixes.Fixes;

public class NavmeshLookupLagFix: BaseFix
{
    public NavmeshLookupLagFix()
    {
        Name = "NavmeshLookupLagFix";
        ConfigurationProperty = "EnableNavmeshLookupLagFix";
        PatchNames =
        [
            "BotNavIgnore"
        ];
    }
}