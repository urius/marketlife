public class DebugDataHolder
{
    public static readonly DebugDataHolder Instance = new DebugDataHolder();

    public string DebugUid;
    public bool IsSaveDisabled = false;
    public bool UseTestConfigFile = false;
    public bool IsTutorialDisabled = false;
}
