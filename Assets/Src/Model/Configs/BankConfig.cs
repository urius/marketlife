using System;
using System.Collections.Generic;
using System.Linq;

public class BankConfig
{
    public static BankConfig Instance => _instance.Value;
    private static Lazy<BankConfig> _instance = new Lazy<BankConfig>();

    public event Action ItemsUpdated = delegate { };

    public readonly List<BankConfigItem> GoldItems = new List<BankConfigItem>(6);
    public readonly List<BankConfigItem> CashItems = new List<BankConfigItem>(6);

    public void SetItems(IEnumerable<BankConfigItem> items)
    {
        GoldItems.Clear();
        GoldItems.AddRange(items.Where(i => i.IsGold));
        CashItems.Clear();
        CashItems.AddRange(items.Where(i => !i.IsGold));
        ItemsUpdated();
    }
}

public class BankConfigItem
{
    public bool IsGold;
    public int Value;
    public int Price;
    public int ExtraPercent;
}
