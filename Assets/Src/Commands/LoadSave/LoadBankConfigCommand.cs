using System.Linq;
using Cysharp.Threading.Tasks;
using Src.Common;
using Src.Model.Configs;

namespace Src.Commands.LoadSave
{
    public struct LoadBankConfigCommand
    {
        public async UniTask<bool> ExecuteAsync()
        {
            var bankConfig = BankConfig.Instance;
            
            var loadedItems = await LoadBankData();
            
            if (loadedItems.Any())
            {
                var items = SetItemsExtraPercentValue(loadedItems);
                
                bankConfig.SetItems(items);
                
                return true;
            }

            return false;
        }

        private static UniTask<BankConfigItem[]> LoadBankData()
        {
            var loadTask = MirraSdkWrapper.IsMirraSdkUsed
                ? new LoadMirraPlatformBankConfigCommand().ExecuteAsync()
                : new LoadInternalBankConfigCommand().ExecuteAsync();
            
            return loadTask;
        }

        private static BankConfigItem[] SetItemsExtraPercentValue(BankConfigItem[] bankConfigItems)
        {
            var minGoldItem = bankConfigItems.First(i => i.IsGold);
            var minCashItem = bankConfigItems.First(i => !i.IsGold);
            foreach (var item in bankConfigItems)
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

            foreach (var item in bankConfigItems)
            {
                item.ExtraPercent = CalculateExtraPercent(item, item.IsGold ? minGoldItem : minCashItem);
            }

            return bankConfigItems;
        }

        private static int CalculateExtraPercent(BankConfigItem item, BankConfigItem minItem)
        {
            var multiplier = (float)item.Price / minItem.Price;
            var calculatedValue = multiplier * minItem.Value;
            var extraPercent = (int)(100 * (item.Value - calculatedValue) / calculatedValue);
            
            return extraPercent > 0 ? extraPercent : 0;
        }
    }

    public struct BankItemValueDto
    {
        public int price;
    }
}