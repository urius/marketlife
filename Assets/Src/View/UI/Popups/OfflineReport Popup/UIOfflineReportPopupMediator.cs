using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIOfflineReportPopupMediator : IMediator
{
    private readonly GameStateModel _gameStateModel;
    private readonly RectTransform _parentTransform;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly LocalizationManager _loc;
    private readonly SpritesProvider _spritesProvider;
    private readonly PoolCanvasProvider _poolCanvasProvider;
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
    }

    public async void Mediate()
    {
        _viewModel = _gameStateModel.ShowingPopupModel as OfflineReportPopupViewModel;
        var popupGo = GameObject.Instantiate(_prefabsHolder.UITabbedContentPopupPrefab, _parentTransform);
        _popupView = popupGo.GetComponent<UITabbedContentPopupView>();
        _popupView.SetSize(560, 730);

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
    }

    private void Activate()
    {
    }

    private void Deactivate()
    {
    }

    private void ShowTab(int tabIndex)
    {
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
        ClearDisplayedItems();

        var captionTransform = GetOrCreateItemToDisplay(_prefabsHolder.UIOfflineReportPopupCaptionPrefab);
        var captionView = captionTransform.GetComponent<UIOfflineReportPopupCaptionItemView>();
        captionView.SetText(string.Format(_loc.GetLocalization(LocalizationKeys.PopupOfflineReportProfitTabProfitTitleFormat), _viewModel.ReportModel.HoursPassed));
        PutNext(captionTransform);

        foreach (var itemViewMdoel in _viewModel.SoldProducts)
        {
            var sellItemTransform = GetOrCreateItemToDisplay(_prefabsHolder.UIOfflineReportPopupItemPrefab);
            var itemView = sellItemTransform.GetComponent<UIOfflineReportPopupReportItemView>();
            itemView.SetImageSprite(_spritesProvider.GetProductIcon(itemViewMdoel.ProductKey));
            itemView.SetLeftText($"x{itemViewMdoel.Amount}");
            itemView.SetRightText($"+{itemViewMdoel.Profit}");
            PutNext(sellItemTransform);
        }
    }

    private void ClearDisplayedItems()
    {
        foreach (var displayedItem in _displayedItems)
        {
            displayedItem.Transform.SetParent(_poolCanvasProvider.PoolCanvasTransform);
            _cachedItemsByPrefab[displayedItem.Prefab].Enqueue(displayedItem.Transform);
        }

        _displayedItems.Clear();
    }

    private void PutNext(RectTransform itemTransform)
    {
        itemTransform.anchoredPosition = new Vector2(0, _putPointer);
        _putPointer -= (int)itemTransform.rect.height;
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
