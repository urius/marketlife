using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Src.Common;
using Src.Model.Configs;
using UnityEngine;

namespace Src.Commands.LoadSave
{
    public struct LoadMirraPlatformBankConfigCommand
    {
        public async UniTask<BankConfigItem[]> ExecuteAsync()
        {
            Debug.Log("LoadMirraPlatformBankConfigCommand");
            
            var products = await MirraSdkWrapper.FetchProducts();

            Debug.Log("LoadMirraPlatformBankConfigCommand, products len: " + products.Length);

            var result = products.Select(ConvertToBankConfigItem).ToArray();
            
            return result;
        }

        private static BankConfigItem ConvertToBankConfigItem(BankProductData bankProductData)
        {
            var str = JsonConvert.SerializeObject(bankProductData);
            Debug.Log("LoadMirraPlatformBankConfigCommand, product = " + str);
            
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