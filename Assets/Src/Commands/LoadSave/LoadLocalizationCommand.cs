using Cysharp.Threading.Tasks;
using Src.Managers;

public struct LoadLocalizationCommand : IAsyncGameLoadCommand
{
    public UniTask<bool> ExecuteAsync()
    {
        return LocalizationManager.Instance.LoadLocalizationAsync();
    }
}
