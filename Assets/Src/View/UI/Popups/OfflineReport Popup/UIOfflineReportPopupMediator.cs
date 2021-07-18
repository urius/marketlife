using System.Linq;
using TMPro;
using UnityEngine;

public class UIOfflineReportPopupMediator : UIContentPopupMediator
{
    private readonly GameStateModel _gameStateModel;
    private readonly RectTransform _parentTransform;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly LocalizationManager _loc;
    private readonly SpritesProvider _spritesProvider;
    private readonly Dispatcher _dispatcher;

    private UITabbedContentPopupView _popupView;
    private OfflineReportPopupViewModel _viewModel;

    protected override UIContentPopupView PopupView => _popupView;

    public UIOfflineReportPopupMediator(RectTransform parentTransform)
    {
        _parentTransform = parentTransform;

        _gameStateModel = GameStateModel.Instance;
        _prefabsHolder = PrefabsHolder.Instance;
        _loc = LocalizationManager.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
    }

    public override async void Mediate()
    {
        _viewModel = _gameStateModel.ShowingPopupModel as OfflineReportPopupViewModel;
        var popupGo = GameObject.Instantiate(_prefabsHolder.UITabbedContentPopupPrefab, _parentTransform);
        _popupView = popupGo.GetComponent<UITabbedContentPopupView>();
        _popupView.SetSize(670, 730);

        var titleTimePassedStr = _viewModel.ReportModel.HoursPassed >= 1
            ? string.Format(_loc.GetLocalization(LocalizationKeys.CommonHoursShortFormat), (int)_viewModel.ReportModel.HoursPassed)
            : string.Format(_loc.GetLocalization(LocalizationKeys.CommonMinutesShortFormat), _viewModel.ReportModel.MinutesPassed);
        _popupView.SetTitleText(string.Format(_loc.GetLocalization(LocalizationKeys.PopupOfflineReportTitleFormat), titleTimePassedStr));
        _popupView.SetupTabButtons(_viewModel.Tabs.Select(ToTabName).ToArray());
        ShowTab(0);

        AudioManager.Instance.PlaySound(SoundNames.PopupOpen);
        await _popupView.Appear2Async();

        Activate();
    }

    public override async void Unmediate()
    {
        base.Unmediate();

        Deactivate();

        await _popupView.Disppear2Async();

        GameObject.Destroy(_popupView.gameObject);
    }

    private void Activate()
    {
        _popupView.ButtonCloseClicked += OnCloseClicked;
    }

    private void Deactivate()
    {
        _popupView.ButtonCloseClicked -= OnCloseClicked;
    }

    private void OnCloseClicked()
    {
        _dispatcher.UIRequestRemoveCurrentPopup();
    }

    private void ShowTab(int tabIndex)
    {
        ClearDisplayedItems();

        _popupView.SetTabButtonSelected(tabIndex);
        switch (_viewModel.Tabs[tabIndex])
        {
            case OfflineReportTabType.SellProfit:
                ShowProfitTab();
                break;
        }
    }

    private void ShowProfitTab()
    {
        foreach (var itemViewMdoel in _viewModel.SoldProducts)
        {
            PutNextReportItem(_spritesProvider.GetProductIcon(itemViewMdoel.ProductKey), $"x{FormattingHelper.ToCommaSeparatedNumber(itemViewMdoel.Amount)}", $"+{FormattingHelper.ToCommaSeparatedNumber(itemViewMdoel.Profit)}$");
        }

        PutNextReportItem(null, _loc.GetLocalization(LocalizationKeys.PopupOfflineReportProfitTabOverallProfit), TextAlignmentOptions.Right, $"{FormattingHelper.ToCommaSeparatedNumber(_viewModel.TotalProfitFromSell)}$", TextAlignmentOptions.Right);
    }

    private void PutNextReportItem(Sprite iconSprite, string lextText, TextAlignmentOptions leftTextAlignment, string rightText, TextAlignmentOptions rightTextAlignment)
    {
        var itemTransform = GetOrCreateItemToDisplay(_prefabsHolder.UIOfflineReportPopupItemPrefab);
        var itemView = itemTransform.GetComponent<UIOfflineReportPopupReportItemView>();
        itemView.SetImageSprite(iconSprite);
        itemView.SetLeftText(lextText, leftTextAlignment);
        itemView.SetRightText(rightText, rightTextAlignment);
    }

    private void PutNextReportItem(Sprite iconSprite, string lextText, string rightText)
    {
        PutNextReportItem(iconSprite, lextText, TextAlignmentOptions.Left, rightText, TextAlignmentOptions.Right);
    }

    private void PutNextCaption(string captionText)
    {
        var captionTransform = GetOrCreateItemToDisplay(_prefabsHolder.UIOfflineReportPopupCaptionItemPrefab);
        var captionView = captionTransform.GetComponent<UIPopupCaptionItemView>();
        captionView.SetText(captionText);
    }

    private string ToTabName(OfflineReportTabType tabType)
    {
        string tabKey = "tab key";
        switch (tabType)
        {
            case OfflineReportTabType.SellProfit:
                tabKey = LocalizationKeys.PopupOfflineReportProfitTab;
                break;
            case OfflineReportTabType.Personal:
                tabKey = LocalizationKeys.PopupOfflineReportPersonalTab;
                break;
            case OfflineReportTabType.Activity:
                tabKey = LocalizationKeys.PopupOfflineReportActivityTab;
                break;
        }

        return _loc.GetLocalization(tabKey);
    }
}
