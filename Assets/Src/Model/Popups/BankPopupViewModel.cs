using System;
using System.Linq;

public class BankPopupViewModel : PopupViewModelBase
{
    public event Action ItemsUpdated = delegate { };

    public readonly int InitialTabIndex;

    private readonly BankConfig _bankConfig;
    private readonly MainConfig _mainConfig;

    public BankPopupViewModel(int initialTabIndex, BankConfig bankConfig, MainConfig mainConfig)
    {
        InitialTabIndex = initialTabIndex;

        _bankConfig = bankConfig;
        _mainConfig = mainConfig;
        UpdateItems();

        _bankConfig.ItemsUpdated += OnConfigItemsUpdated;
    }

    public BankItemViewModel[] GoldItems { get; private set; }
    public BankItemViewModel[] CashItems { get; private set; }

    public override PopupType PopupType => PopupType.Bank;

    public override void Dispose()
    {
        _bankConfig.ItemsUpdated -= OnConfigItemsUpdated;
        base.Dispose();
    }

    private void OnConfigItemsUpdated()
    {
        UpdateItems();
    }

    private void UpdateItems()
    {
        GoldItems = new[] { new BankItemViewModel(isGold: true, goldReward: _mainConfig.BankAdvertRewardGold) }
            .Concat(_bankConfig.GoldItems
            .Select(c => new BankItemViewModel(c)))
            .ToArray();
        CashItems = new[] { new BankItemViewModel(isGold: false, goldReward: _mainConfig.BankAdvertRewardGold) }
            .Concat(_bankConfig.CashItems
            .Select(c => new BankItemViewModel(c)))
            .ToArray();

        ItemsUpdated();
    }
}

public class BankItemViewModel
{
    public readonly bool IsAds;
    public readonly bool IsGold;
    public readonly int Value;
    public readonly int Price;
    public readonly int ExtraPercent;

    private readonly BankConfigItem _bankConfigItem;

    public BankItemViewModel(bool isGold, int goldReward)
    {
        IsAds = true;
        IsGold = isGold;
        Value = isGold ? goldReward : goldReward * CalculationHelper.GetGoldToCashConversionRate();
        Price = 0;
        ExtraPercent = 0;
    }

    public BankItemViewModel(BankConfigItem bankConfigItem)
    {
        IsAds = false;
        IsGold = bankConfigItem.IsGold;
        Value = bankConfigItem.Value;
        Price = bankConfigItem.Price;
        ExtraPercent = bankConfigItem.ExtraPercent;

        _bankConfigItem = bankConfigItem;
    }

    public BankConfigItem GetBankconfigItem()
    {
        return _bankConfigItem;
    }
}
