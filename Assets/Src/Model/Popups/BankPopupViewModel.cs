using System;
using System.Linq;

public class BankPopupViewModel : PopupViewModelBase
{
    public event Action ItemsUpdated = delegate { };
    public event Action RestWatchesCountChanged = delegate { };
    public event Action AdvertWatchTimeChanged = delegate { };

    public readonly int InitialTabIndex;

    private readonly BankConfig _bankConfig;
    private readonly MainConfig _mainConfig;
    private readonly bool _isBuyInBankAllowed;
    private readonly IBankAdvertWatchStateProvider _advertWatchStateProvider;
    private readonly int _advertDefaultWatchesCount;
    private readonly int _advertCooldownMinutes;

    public BankPopupViewModel(
        int initialTabIndex,
        BankConfig bankConfig,
        MainConfig mainConfig,
        bool isBuyInBankAllowed,
        IBankAdvertWatchStateProvider advertWatchStateProvider)
    {
        InitialTabIndex = initialTabIndex;

        _bankConfig = bankConfig;
        _mainConfig = mainConfig;
        _isBuyInBankAllowed = isBuyInBankAllowed;
        _advertWatchStateProvider = advertWatchStateProvider;
        _advertDefaultWatchesCount = mainConfig.AdvertDefaultWatchesCount;
        _advertCooldownMinutes = mainConfig.AdvertWatchCooldownMinutes;
        UpdateItems();

        _bankConfig.ItemsUpdated += OnConfigItemsUpdated;
        _advertWatchStateProvider.BankAdvertWatchesCountChanged += OnBankAdvertWatchesCountChanged;
        _advertWatchStateProvider.BankAdvertWatchTimeChanged += OnBankAdvertWatchTimeChanged;
    }

    private int ResetWatchesCountTime => _advertWatchStateProvider.BankAdvertWatchTime + _advertCooldownMinutes * 60;

    public IBankItemViewModel[] GoldItems { get; private set; }
    public IBankItemViewModel[] CashItems { get; private set; }

    public override PopupType PopupType => PopupType.Bank;

    public override void Dispose()
    {
        _bankConfig.ItemsUpdated -= OnConfigItemsUpdated;
        _advertWatchStateProvider.BankAdvertWatchesCountChanged -= OnBankAdvertWatchesCountChanged;
        _advertWatchStateProvider.BankAdvertWatchTimeChanged -= OnBankAdvertWatchTimeChanged;
        base.Dispose();
    }

    private void OnConfigItemsUpdated()
    {
        UpdateItems();
    }

    private void OnBankAdvertWatchesCountChanged()
    {
        var advertItems = GoldItems.Concat(CashItems)
            .Where(i => i.RetrieveMethodType == BankItemRetrieveMethodType.Advert)
            .Select(i => i as BankAdvertItemViewModel);

        var restWatchesCount = _advertDefaultWatchesCount - _advertWatchStateProvider.BankAdvertWatchesCount;
        foreach (var item in advertItems)
        {
            item.UpdateWatchesCount(restWatchesCount);
        }

        RestWatchesCountChanged();
    }

    private void OnBankAdvertWatchTimeChanged()
    {
        var advertItems = GoldItems.Concat(CashItems)
               .Where(i => i.RetrieveMethodType == BankItemRetrieveMethodType.Advert)
               .Select(i => i as BankAdvertItemViewModel);

        foreach (var item in advertItems)
        {
            item.UpdateResetWatchesCountTime(ResetWatchesCountTime);
        }

        AdvertWatchTimeChanged();
    }

    private void UpdateItems()
    {
        var restWatchesCount = _advertDefaultWatchesCount - _advertWatchStateProvider.BankAdvertWatchesCount;
        var bankAdvertRewardGold = _mainConfig.BankAdvertRewardGold;
        var bankAdvertRewardCash = bankAdvertRewardGold * CalculationHelper.GetGoldToCashConversionRate();
        GoldItems = new IBankItemViewModel[] { new BankAdvertItemViewModel(isGold: true, bankAdvertRewardGold, restWatchesCount, ResetWatchesCountTime) }
            .Concat(_bankConfig.GoldItems
            .Select(c => new BankBuyableItemViewModel(c, _isBuyInBankAllowed)))
            .ToArray();
        CashItems = new IBankItemViewModel[] { new BankAdvertItemViewModel(isGold: false, bankAdvertRewardCash, restWatchesCount, ResetWatchesCountTime) }
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
    private int _restWatchesCount;
    private int _resetWatchesCountTime;

    public BankAdvertItemViewModel(bool isGold, int value, int restWatchesCount, int resetWatchesCountTime)
    {
        _isGold = isGold;
        _value = value;
        _restWatchesCount = restWatchesCount;
        _resetWatchesCountTime = resetWatchesCountTime;
    }

    public BankItemRetrieveMethodType RetrieveMethodType => BankItemRetrieveMethodType.Advert;
    public bool IsGold => _isGold;
    public int Value => _value;
    public int RestWatchesCount => _restWatchesCount;
    public int ResetWatchesCountTime => _resetWatchesCountTime;

    public void UpdateWatchesCount(int newValue)
    {
        _restWatchesCount = newValue;
    }

    public void UpdateResetWatchesCountTime(int time)
    {
        _resetWatchesCountTime = time;
    }
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
