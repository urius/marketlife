using Cysharp.Threading.Tasks;

public struct LoadConfigsCommand
{
    public async UniTask<bool> ExecuteAsync()
    {
        var result = true;
        result &= await GameConfigManager.Instance.LoadConfigAsync();
        result &= await new LoadBankConfigCommand().ExecuteAsync();
        return result;
    }
}
