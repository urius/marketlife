using System.Linq;
using Cysharp.Threading.Tasks;
using Src.Common;
using Src.Model.Configs;

namespace Src.Commands.LoadSave
{
    public struct LoadMirraPlatformBankConfigCommand
    {
        public async UniTask<BankConfigItem[]> ExecuteAsync()
        {
            var products = await MirraSdkWrapper.FetchProducts();

            var result = products.Select(ConvertToBankConfigItem).ToArray();
            
            return result;
        }

        private static BankConfigItem ConvertToBankConfigItem(BankProductData bankProductData)
        {
            var splittedName = bankProductData.ProductId.Split('_');
            var isGold = splittedName[0].IndexOf('g') >= 0;
            var value = int.Parse(splittedName[1]);
            
            return new BankConfigItem()
            {
                Id = bankProductData.ProductId,
                IsGold = isGold,
                Value = value,
                Price = bankProductData.Price,
                LocalizedCurrencyName = bankProductData.CurrencyNameLocalized,
            };
        }
    }
}