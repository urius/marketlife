using System;
using System.Collections.Generic;
using UnityEngine;

public class UIBankPopupMediator : UIContentPopupMediator
{
    private readonly RectTransform _parentTransform;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly BankConfig _bankConfig;
    private readonly LocalizationManager _loc;
    private readonly SpritesProvider _spritesProvider;
    private readonly Dispatcher _dispatcher;
    private readonly GameStateModel _gameStateModel;
    private readonly Dictionary<UIBankPopupItemView, BankConfigItem> _modelByViewDict = new Dictionary<UIBankPopupItemView, BankConfigItem>();

    private UITabbedContentPopupView _popupView;
    private int _currentTabIndex;
    private List<BankConfigItem> _viewModelsToDisplay;
    private BankPopupViewModel _viewModel;

    public UIBankPopupMediator(RectTransform parentTransform)
    {
        _parentTransform = parentTransform;

        _prefabsHolder = PrefabsHolder.Instance;
        _bankConfig = BankConfig.Instance;
        _loc = LocalizationManager.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
        _gameStateModel = GameStateModel.Instance;
    }

    protected override UIContentPopupView PopupView => _popupView;

    public async override void Mediate()
    {
        _viewModel = _gameStateModel.ShowingPopupModel as BankPopupViewModel;
        TopPadding = 5;
        var popupGo = GameObject.Instantiate(_prefabsHolder.UITabbedContentPopupPrefab, _parentTransform);
        _popupView = popupGo.GetComponent<UITabbedContentPopupView>();
        _popupView.SetTitleText(_loc.GetLocalization(LocalizationKeys.PopupBankTitle));
        _popupView.SetSize(1050, 715);
        _popupView.SetupTabButtons(new string[] { _loc.GetLocalization(LocalizationKeys.PopupBankGoldTab), _loc.GetLocalization(LocalizationKeys.PopupBankCashTab) });

        ShowTab(_viewModel.InitialTabIndex);
        Activate();

        await _popupView.Appear2Async();
    }

    public async override void Unmediate()
    {
        base.Unmediate();
        Deactivate();

        await _popupView.Disppear2Async();

        ClearDisplayedItems();
        GameObject.Destroy(_popupView.gameObject);
    }

    protected override void ClearDisplayedItems()
    {
        foreach (var kvp in _modelByViewDict)
        {
            DeactivateItemView(kvp.Key);
        }
        _modelByViewDict.Clear();

        base.ClearDisplayedItems();
    }

    private void Activate()
    {
        _popupView.TabButtonClicked += OnTabButtonClicked;
        _popupView.ButtonCloseClicked += OnCloseClicked;
        _bankConfig.ItemsUpdated += OnItemsUpdated;
    }

    private void Deactivate()
    {
        _popupView.TabButtonClicked -= OnTabButtonClicked;
        _popupView.ButtonCloseClicked -= OnCloseClicked;
        _bankConfig.ItemsUpdated -= OnItemsUpdated;
    }

    private void OnItemsUpdated()
    {
        ShowTab(_currentTabIndex);
    }

    private void OnCloseClicked()
    {
        _dispatcher.UIRequestRemoveCurrentPopup();
    }

    private void OnTabButtonClicked(int tabIndex)
    {
        ShowTab(tabIndex);
    }

    private void ShowTab(int tabIndex)
    {
        _popupView.SetTabButtonSelected(tabIndex);
        ClearDisplayedItems();

        _currentTabIndex = tabIndex;
        _viewModelsToDisplay = _currentTabIndex == 0 ? _bankConfig.GoldItems : _bankConfig.CashItems;
        foreach (var viewModel in _viewModelsToDisplay)
        {
            PutItem(viewModel);
        }
    }

    private void PutItem(BankConfigItem viewModel)
    {
        var rectTransform = GetOrCreateItemToDisplay(_prefabsHolder.UIBankPopupItemPrefab);
        var itemView = rectTransform.GetComponent<UIBankPopupItemView>();
        SetupItemView(itemView, viewModel);
        ActivateItemView(itemView);
        _modelByViewDict[itemView] = viewModel;
    }

    private void SetupItemView(UIBankPopupItemView itemView, BankConfigItem viewModel)
    {
        itemView.SetIconSprite(viewModel.IsGold ? _spritesProvider.GetGoldIcon() : _spritesProvider.GetCashIcon());
        itemView.SetAmountText(FormattingHelper.ToCommaSeparatedNumber(viewModel.Value));
        itemView.SetPriceText(FormattingHelper.ToCommaSeparatedNumber(viewModel.Price) + $" {_loc.GetLocalization($"{LocalizationKeys.CommonPayCurrencyNamePlural}{PluralsHelper.GetPlural(viewModel.Price)}")}");
        if (viewModel.ExtraPercent > 0)
        {
            itemView.SetExtraPercentText($"(+{viewModel.ExtraPercent}%)");
        }
        else
        {
            itemView.SetExtraPercentText(string.Empty);
        }
    }

    private void ActivateItemView(UIBankPopupItemView view)
    {
        view.Clicked += OnViewClicked;
    }

    private void DeactivateItemView(UIBankPopupItemView view)
    {
        view.Clicked -= OnViewClicked;
    }

    private void OnViewClicked(UIBankPopupItemView view)
    {
        _dispatcher.UIBankItemClicked(_modelByViewDict[view]);
    }
}