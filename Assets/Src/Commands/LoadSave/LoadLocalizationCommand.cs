using Cysharp.Threading.Tasks;
using Src.Managers;

namespace Src.Commands.LoadSave
{
    public struct LoadLocalizationCommand : IAsyncGameLoadCommand
    {
        public UniTask<bool> ExecuteAsync()
        {
            return LocalizationManager.Instance.LoadLocalizationAsync();
        }
    }
}
