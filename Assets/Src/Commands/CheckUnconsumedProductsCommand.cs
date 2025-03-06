using Cysharp.Threading.Tasks;
using Src.Common;
using Src.Model;
using UnityEngine;

namespace Src.Commands
{
    public struct CheckUnconsumedProductsCommand : IAsyncGameLoadCommand
    {
        public async UniTask<bool> ExecuteAsync()
        {
            MirraSdkWrapper.FetchAndConsume(ConsumeProductCallback);

            await UniTask.Delay(100);
            
            return true;
        }

        private static void ConsumeProductCallback(string productId)
        {
            Debug.Log($"CheckUnconsumedProductsCommand: Consumed product: {productId}");
            
            var playerModelHolder = PlayerModelHolder.Instance;
            
            var splittedName = productId.Split("_");
            var isGold = splittedName[0].IndexOf('g') >= 0;
            var value = int.Parse(splittedName[1]);
            
            if (isGold)
            {
                playerModelHolder.UserModel.AddGold(value);
            }
            else
            {
                playerModelHolder.UserModel.AddCash(value);
            }
        }
    }
}