using Cysharp.Threading.Tasks;

public struct LoadLocalizationCommand : IAsyncGameLoadCommand
{
    public UniTask<bool> ExecuteAsync()
    {
        return LocalizationManager.Instance.LoadLocalizationAsync();
    }
}
