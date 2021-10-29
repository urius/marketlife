using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public struct LoadBankConfigCommand
{
    public async UniTask<bool> ExecuteAsync()
    {
        var playerModelHolder = PlayerModelHolder.Instance;
        var getBankURL = URLsHolder.Instance.GetBankDataURL(playerModelHolder.SocialType);
        var bankConfig = BankConfig.Instance;

        var getConfigOperation = await new WebRequestsSender().GetAsync(URLHelper.AddAntiCachePostfix(getBankURL));
        if (getConfigOperation.IsSuccess)
        {
            var bankConfigDto = JsonConvert.DeserializeObject<Dictionary<string, BankItemValueDto>>(getConfigOperation.Result);
            var items = ConvertToBankItems(bankConfigDto);
            bankConfig.SetItems(items);
        }

        return getConfigOperation.IsSuccess;
    }

    private List<BankConfigItem> ConvertToBankItems(Dictionary<string, BankItemValueDto> bankConfigDto)
    {
        var result = new List<BankConfigItem>(bankConfigDto.Count);
        foreach (var kvp in bankConfigDto)
        {
            var item = ConvertItem(kvp.Key, kvp.Value);
            result.Add(item);
        }

        var minGoldItem = result.Where(i => i.IsGold).First();
        var minCashItem = result.Where(i => !i.IsGold).First();
        foreach (var item in result)
        {
            if (item.IsGold)
            {
                if (item.Price < minGoldItem.Price) minGoldItem = item;
            }
            else if (item.Price < minCashItem.Price)
            {
                if (item.Price < minCashItem.Price) minCashItem = item;
            }
        }

        foreach (var item in result)
        {
            item.ExtraPercent = CalculateExtraPercent(item, item.IsGold ? minGoldItem : minCashItem);
        }

        return result;
    }

    private int CalculateExtraPercent(BankConfigItem item, BankConfigItem minItem)
    {
        var multiplier = (float)item.Price / minItem.Price;
        var calculatedValue = multiplier * minItem.Value;
        var extraPercent = (int)(100 * (item.Value - calculatedValue) / calculatedValue);
        if (extraPercent > 0)
        {
            return extraPercent;
        }
        else
        {
            return 0;
        }
    }

    private BankConfigItem ConvertItem(string key, BankItemValueDto value)
    {
        var splitted = key.Split('_');
        var isGold = splitted[0].IndexOf('g') >= 0;
        var amount = int.Parse(splitted[1]);
        var price = value.price;

        return new BankConfigItem { Id = key, IsGold = isGold, Value = amount, Price = price };
    }
}

public struct BankItemValueDto
{
    public int price;
}


