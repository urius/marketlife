using Cysharp.Threading.Tasks;

public struct LoadLocalizationCommand
{
    public UniTask<bool> ExecuteAsync()
    {
        return LocalizationManager.Instance.LoadLocalizationAsync();
    }
}
