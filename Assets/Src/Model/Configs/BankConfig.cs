using System;
using System.Collections.Generic;
using System.Linq;

namespace Src.Model.Configs
{
    public class BankConfig
    {
        public static BankConfig Instance => _instance.Value;
        
        private static readonly Lazy<BankConfig> _instance = new();

        public event Action ItemsUpdated = delegate { };

        public readonly List<BankConfigItem> GoldItems = new(6);
        public readonly List<BankConfigItem> CashItems = new(6);

        public void SetItems(BankConfigItem[] bankConfigItems)
        {
            GoldItems.Clear();
            GoldItems.AddRange(bankConfigItems.Where(i => i.IsGold));
            CashItems.Clear();
            CashItems.AddRange(bankConfigItems.Where(i => !i.IsGold));
            ItemsUpdated();
        }
    }

    public class BankConfigItem
    {
        public string Id;
        public bool IsGold;
        public int Value;
        public int Price;
        public int ExtraPercent;
        public string LocalizedCurrencyName;
    }
}