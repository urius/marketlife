using Cysharp.Threading.Tasks;

public struct LoadConfigsCommand
{
    public UniTask<bool> ExecuteAsync()
    {
        return GameConfigManager.Instance.LoadConfigAsync();
    }
}
