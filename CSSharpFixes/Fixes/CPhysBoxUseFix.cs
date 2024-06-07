namespace CSSharpFixes.Fixes;

public class CPhysBoxUseFix: BaseFix
{
    public CPhysBoxUseFix()
    {
        Name = "CPhysBoxUseFix";
        ConfigurationProperty = "EnableCPhysBoxUseFix";
        PatchNames =
        [
            "CPhysBox_Use"
        ];
    }
}