using Cysharp.Threading.Tasks;

public struct LoadConfigsCommand : IAsyncGameLoadCommand
{
    public async UniTask<bool> ExecuteAsync()
    {
        var result = true;
        var abDataHolder = ABDataHolder.Instance;

        result &= await GameConfigManager.Instance.LoadMainConfigAsync(abDataHolder.MainConfigPostfix);
        result &= await new LoadBankConfigCommand().ExecuteAsync();
        return result;
    }
}
