using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIOfflineReportPopupMediator : IMediator
{
    private const int TopPadding = 20;

    private readonly GameStateModel _gameStateModel;
    private readonly RectTransform _parentTransform;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly LocalizationManager _loc;
    private readonly SpritesProvider _spritesProvider;
    private readonly PoolCanvasProvider _poolCanvasProvider;
    private readonly Dispatcher _dispatcher;

    private UITabbedContentPopupView _popupView;
    private OfflineReportPopupViewModel _viewModel;
    private Queue<(RectTransform Transform, GameObject Prefab)> _displayedItems = new Queue<(RectTransform Transform, GameObject Prefab)>();
    private Dictionary<GameObject, Queue<RectTransform>> _cachedItemsByPrefab = new Dictionary<GameObject, Queue<RectTransform>>(2);
    private int _putPointer;

    public UIOfflineReportPopupMediator(RectTransform parentTransform)
    {
        _parentTransform = parentTransform;

        _gameStateModel = GameStateModel.Instance;
        _prefabsHolder = PrefabsHolder.Instance;
        _loc = LocalizationManager.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _poolCanvasProvider = PoolCanvasProvider.Instance;
        _dispatcher = Dispatcher.Instance;
    }

    public async void Mediate()
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

        await _popupView.Appear2Async();

        Activate();
    }

    public async void Unmediate()
    {
        Deactivate();

        await _popupView.Disppear2Async();

        foreach (var kvp in _cachedItemsByPrefab)
        {
            var queue = kvp.Value;
            while (queue.Count > 0)
            {
                var rectTransform = queue.Dequeue();
                GameObject.Destroy(rectTransform.gameObject);
            }
        }
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
        PutNext(itemTransform);
    }

    private void PutNextReportItem(Sprite iconSprite, string lextText, string rightText)
    {
        PutNextReportItem(iconSprite, lextText, TextAlignmentOptions.Left, rightText, TextAlignmentOptions.Right);
    }

    private void PutNextCaption(string captionText)
    {
        var captionTransform = GetOrCreateItemToDisplay(_prefabsHolder.UIOfflineReportPopupCaptionPrefab);
        var captionView = captionTransform.GetComponent<UIPopupCaptionItemView>();
        captionView.SetText(captionText);
        PutNext(captionTransform);
    }

    private void ClearDisplayedItems()
    {
        foreach (var displayedItem in _displayedItems)
        {
            displayedItem.Transform.SetParent(_poolCanvasProvider.PoolCanvasTransform);
            _cachedItemsByPrefab[displayedItem.Prefab].Enqueue(displayedItem.Transform);
        }

        _putPointer = TopPadding;
        _popupView.SetContentHeight(0);
        _displayedItems.Clear();
    }

    private void PutNext(RectTransform itemTransform)
    {
        itemTransform.anchoredPosition = new Vector2(0, -_putPointer);
        _putPointer += (int)itemTransform.rect.height;

        _popupView.SetContentHeight(_putPointer);
    }

    private RectTransform GetOrCreateItemToDisplay(GameObject prefab)
    {
        RectTransform result = null;
        if (_cachedItemsByPrefab.ContainsKey(prefab))
        {
            var queue = _cachedItemsByPrefab[prefab];
            if (queue.Count > 0)
            {
                var item = queue.Dequeue();
                item.SetParent(_popupView.ContentRectTransform);
                result = item;
            }
        }
        else
        {
            _cachedItemsByPrefab[prefab] = new Queue<RectTransform>();
        }

        if (result == null)
        {
            var go = GameObject.Instantiate(prefab, _popupView.ContentRectTransform);
            result = go.GetComponent<RectTransform>();
        }
        _displayedItems.Enqueue((result, prefab));
        return result;
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
