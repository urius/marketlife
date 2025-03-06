using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Src.Common;
using Src.Common_Utils;
using Src.Model;
using Src.Model.Configs;

namespace Src.Commands.LoadSave
{
    public struct LoadInternalBankConfigCommand
    {
        public async UniTask<BankConfigItem[]> ExecuteAsync()
        {
            var playerModelHolder = PlayerModelHolder.Instance;
            var getBankURL = Urls.Instance.GetBankDataURL(playerModelHolder.SocialType);
            

            var getConfigOperation = await new WebRequestsSender().GetAsync(URLHelper.AddAntiCachePostfix(getBankURL));
            if (getConfigOperation.IsSuccess)
            {
                var bankConfigDto = JsonConvert.DeserializeObject<Dictionary<string, BankItemValueDto>>(getConfigOperation.Result);
                var items = ConvertToBankItems(bankConfigDto);

                return items;
            }

            return Array.Empty<BankConfigItem>();
        }
        
        private static BankConfigItem[] ConvertToBankItems(Dictionary<string, BankItemValueDto> bankConfigDto)
        {
            var result = bankConfigDto
                .Select(kvp => ConvertItem(kvp.Key, kvp.Value))
                .ToArray();

            return result;
        }
        
        private static BankConfigItem ConvertItem(string key, BankItemValueDto value)
        {
            var splitted = key.Split('_');
            var isGold = splitted[0].IndexOf('g') >= 0;
            var amount = int.Parse(splitted[1]);
            var price = value.price;

            return new BankConfigItem { Id = key, IsGold = isGold, Value = amount, Price = price };
        }
    }
}