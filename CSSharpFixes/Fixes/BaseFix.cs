namespace CSSharpFixes.Fixes;

public abstract class BaseFix
{
    public string Name = String.Empty;
    public string ConfigurationProperty = String.Empty;
    public List<string> PatchNames = new();
    public List<string> DetourHandlerNames = new();
}