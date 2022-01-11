using System;
using System.Linq;

public class BankPopupViewModel : PopupViewModelBase
{
    public event Action ItemsUpdated = delegate { };

    public readonly int InitialTabIndex;

    private readonly BankConfig _bankConfig;
    private readonly MainConfig _mainConfig;
    private readonly bool _isBuyInBankAllowed;

    public BankPopupViewModel(int initialTabIndex, BankConfig bankConfig, MainConfig mainConfig, bool isBuyInBankAllowed)
    {
        InitialTabIndex = initialTabIndex;

        _bankConfig = bankConfig;
        _mainConfig = mainConfig;
        _isBuyInBankAllowed = isBuyInBankAllowed;
        UpdateItems();

        _bankConfig.ItemsUpdated += OnConfigItemsUpdated;
    }

    public IBankItemViewModel[] GoldItems { get; private set; }
    public IBankItemViewModel[] CashItems { get; private set; }

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
        GoldItems = new IBankItemViewModel[] { new BankAdvertItemViewModel(isGold: true, value: _mainConfig.BankAdvertRewardGold) }
            .Concat(_bankConfig.GoldItems
            .Select(c => new BankBuyableItemViewModel(c, _isBuyInBankAllowed)))
            .ToArray();
        CashItems = new IBankItemViewModel[] { new BankAdvertItemViewModel(isGold: false, value: _mainConfig.BankAdvertRewardGold * CalculationHelper.GetGoldToCashConversionRate()) }
            .Concat(_bankConfig.CashItems
            .Select(c => new BankBuyableItemViewModel(c, _isBuyInBankAllowed)))
            .ToArray();

        ItemsUpdated();
    }
}

public enum BankItemRetrieveMethodType
{
    None,
    Advert,
    RealBuy,
}

public interface IBankItemViewModel
{
    BankItemRetrieveMethodType RetrieveMethodType { get; }
    bool IsGold { get; }
    int Value { get; }
}

public class BankAdvertItemViewModel : IBankItemViewModel
{
    private bool _isGold;
    private int _value;

    public BankAdvertItemViewModel(bool isGold, int value)
    {
        _isGold = isGold;
        _value = value;
    }

    public BankItemRetrieveMethodType RetrieveMethodType => BankItemRetrieveMethodType.Advert;
    public bool IsGold => _isGold;
    public int Value => _value;
}

public class BankBuyableItemViewModel : IBankItemViewModel
{
    public readonly int Price;
    public readonly int ExtraPercent;
    public readonly bool IsAvailable = true;

    private readonly BankConfigItem _bankConfigItem;

    public BankBuyableItemViewModel(BankConfigItem bankConfigItem, bool isAvailable)
    {
        _bankConfigItem = bankConfigItem;

        Price = bankConfigItem.Price;
        ExtraPercent = bankConfigItem.ExtraPercent;
        IsAvailable = isAvailable;
    }

    public BankItemRetrieveMethodType RetrieveMethodType => BankItemRetrieveMethodType.RealBuy;
    bool IBankItemViewModel.IsGold => _bankConfigItem.IsGold;
    int IBankItemViewModel.Value => _bankConfigItem.Value;

    public BankConfigItem GetBankconfigItem()
    {
        return _bankConfigItem;
    }
}
