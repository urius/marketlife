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

            new LoadLeaderboardsDataCommand().ExecuteAsync().Forget();
            
            result &= await GameConfigManager.Instance.LoadMainConfigAsync(abDataHolder.MainConfigPostfix);
            result &= await new LoadBankConfigCommand().ExecuteAsync();
            
            
            return result;
        }
    }
}
