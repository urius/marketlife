using Cysharp.Threading.Tasks;
using Src.Managers;
using Src.Model;

namespace Src.Commands.LoadSave
{
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
}
