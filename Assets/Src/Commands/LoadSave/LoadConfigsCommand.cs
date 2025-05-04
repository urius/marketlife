using Cysharp.Threading.Tasks;
using Src.Common;
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
            
            result &= await GameConfigManager.Instance.LoadMainConfigAsync();

            if (MirraSdkWrapper.IsCrazyGames) return result;
            
            result &= await new LoadBankConfigCommand().ExecuteAsync();
            
            return result;
        }
    }
}
