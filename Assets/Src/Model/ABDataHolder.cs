public class ABDataHolder
{
    public static readonly ABDataHolder Instance = new ABDataHolder();

    public string MainConfigPostfix { get; private set; }

    public void Setup(string mainConfigPostfix)
    {
        MainConfigPostfix = mainConfigPostfix ?? string.Empty;
    }
}
