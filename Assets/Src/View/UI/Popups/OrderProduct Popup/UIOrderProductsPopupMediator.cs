using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIOrderProductsPopupMediator : IMediator
{
    private const int DisplayedItemsAmount = 14;

    private readonly RectTransform _parentTransform;
    private readonly LocalizationManager _loc;
    private readonly Dispatcher _dispatcher;
    private readonly ScreenCalculator _screenCalculator;
    private readonly UpdatesProvider _updatesProvider;
    private readonly SpritesProvider _spritesProvider;
    private readonly IProductsConfig _productsConfig;
    private readonly ShopModel _shopModel;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly PoolCanvasProvider _poolCanvasProvider;
    private readonly LinkedList<(UIOrderProductItemView View, ProductConfig Config)> _displayedItems = new LinkedList<(UIOrderProductItemView, ProductConfig)>();
    private readonly Queue<UIOrderProductItemView> _cachedItems = new Queue<UIOrderProductItemView>();

    private UITabbedContentPopupView _popupView;
    private Rect _itemRect;
    private IGrouping<int, ProductConfig>[] _availableProductConfigsByGroupId;
    private ProductConfig[] _currentTabProductConfigs;
    private int _currentShowFromIndex;
    private int _currentShowToIndex;
    private float _lastContentPosition;

    public UIOrderProductsPopupMediator(RectTransform contentTransform)
    {
        _parentTransform = contentTransform;

        _loc = LocalizationManager.Instance;
        _dispatcher = Dispatcher.Instance;
        _screenCalculator = ScreenCalculator.Instance;
        _updatesProvider = UpdatesProvider.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _productsConfig = GameConfigManager.Instance.ProductsConfig;
        _shopModel = GameStateModel.Instance.PlayerShopModel;
        _prefabsHolder = PrefabsHolder.Instance;
        _poolCanvasProvider = PoolCanvasProvider.Instance;
    }

    public async void Mediate()
    {
        _itemRect = (_prefabsHolder.UIOrderProductPopupItemPrefab.transform as RectTransform).rect;
        _availableProductConfigsByGroupId = _productsConfig.GetProductConfigsForLevel(_shopModel.ProgressModel.Level)
            .GroupBy(p => p.GroupId)
            .OrderBy(g => g.Key)
            .ToArray();

        var popupGo = GameObject.Instantiate(_prefabsHolder.UITabbedContentPopupPrefab, _parentTransform);
        _popupView = popupGo.GetComponent<UITabbedContentPopupView>();
        _popupView.SetupTabButtons(_availableProductConfigsByGroupId
            .Select(k => _loc.GetLocalization($"{LocalizationKeys.NameProductGroupIdPrefix}{k.Key}"))
            .ToArray());
        _popupView.SetSize(1048, 868);

        ShowTab(0);

        await _popupView.AppearAsync2();

        Activate();
    }

    public async void Unmediate()
    {
        Deactivate();
        foreach (var itemView in _cachedItems)
        {
            GameObject.Destroy(itemView.gameObject);
        }
        _cachedItems.Clear();

        await _popupView.DisppearAsync2();

        GameObject.Destroy(_popupView.gameObject);
    }

    private void Activate()
    {
        _popupView.TabButtonClicked += OnTabButtonClicked;
        _popupView.ButtonCloseClicked += OnCloseClicked;
        _updatesProvider.RealtimeUpdate += OnRealtimeUpdate;
        _dispatcher.UIRequestOrderProductAnimation += OnUIRequestOrderProductAnimation;
    }

    private void Deactivate()
    {
        _popupView.TabButtonClicked -= OnTabButtonClicked;
        _popupView.ButtonCloseClicked -= OnCloseClicked;
        _updatesProvider.RealtimeUpdate -= OnRealtimeUpdate;
        _dispatcher.UIRequestOrderProductAnimation -= OnUIRequestOrderProductAnimation;
        foreach (var displayedItem in _displayedItems)
        {
            DeactivateItem(displayedItem.View);
        }
    }

    private void OnUIRequestOrderProductAnimation(RectTransform arg1, Vector2 arg2, int arg3, ProductModel arg4)
    {
        _dispatcher.UIRequestRemoveCurrentPopup();
    }

    private void OnCloseClicked()
    {
        _dispatcher.UIRequestRemoveCurrentPopup();
    }

    private void OnRealtimeUpdate()
    {
        var pos = _popupView.ContentRectTransform.anchoredPosition.y;
        if (Math.Abs(_lastContentPosition - pos) > 0.05f)
        {
            _lastContentPosition = pos;
            var indexFrom = (int)(_lastContentPosition / _itemRect.height) * 2 - 2;
            ShowItemsFromIndex(indexFrom);
        }
    }

    private void OnTabButtonClicked(int buttonIndex)
    {
        ShowTab(buttonIndex);
    }

    private void ShowTab(int buttonIndex)
    {
        foreach (var item in _displayedItems)
        {
            HideItem(item.View);
        }
        _displayedItems.Clear();

        _currentTabProductConfigs = _availableProductConfigsByGroupId[buttonIndex].ToArray();
        _currentShowFromIndex = 0;
        _currentShowToIndex = -1;

        var rowsCount = (int)Math.Ceiling(_currentTabProductConfigs.Length * 0.5);
        _popupView.SetContentHeight(rowsCount * _itemRect.height);

        _popupView.SetTabButtonSelected(buttonIndex);
        ResetScrollPosition();
        ShowItemsFromIndex(0);
    }

    private void ResetScrollPosition()
    {
        _lastContentPosition = 0;
        _popupView.SetContentYPosition(_lastContentPosition);
    }

    private void HideItem(UIOrderProductItemView itemView)
    {
        DeactivateItem(itemView);
        itemView.transform.SetParent(_poolCanvasProvider.PoolCanvasTransform);
        _cachedItems.Enqueue(itemView);
    }

    private void ActvateItem(UIOrderProductItemView itemView)
    {
        itemView.Cliked += OnItemClicked;
    }

    private void DeactivateItem(UIOrderProductItemView itemView)
    {
        itemView.Cliked -= OnItemClicked;
    }

    private void OnItemClicked(UIOrderProductItemView itemView)
    {
        var productConfig = _displayedItems.Where(i => i.View == itemView).First().Config;
        var startAnimationScreenPoint = _screenCalculator.WorldToScreenPoint(itemView.GetIconPosition());
        _dispatcher.UIOrderProductClicked(_parentTransform, startAnimationScreenPoint, productConfig);
    }

    private void ShowItemsFromIndex(int fromIndex)
    {
        fromIndex = Math.Max(fromIndex, 0);
        var toIndex = Math.Min(fromIndex + DisplayedItemsAmount - 1, _currentTabProductConfigs.Length - 1);
        while (_currentShowFromIndex < fromIndex)
        {
            HideItem(_displayedItems.First.Value.View);
            _displayedItems.RemoveFirst();
            _currentShowFromIndex++;
        }
        while (_currentShowToIndex > toIndex)
        {
            HideItem(_displayedItems.Last.Value.View);
            _displayedItems.RemoveLast();
            _currentShowToIndex--;
        }

        while (fromIndex < _currentShowFromIndex)
        {
            _currentShowFromIndex--;
            var itemView = AddItemAt(_currentShowFromIndex);
            var config = _currentTabProductConfigs[_currentShowFromIndex];
            _displayedItems.AddFirst((itemView, config));
            ShowItem(itemView, config);
        }

        while (_currentShowToIndex < toIndex)
        {
            _currentShowToIndex++;
            var itemView = AddItemAt(_currentShowToIndex);
            var config = _currentTabProductConfigs[_currentShowToIndex];
            _displayedItems.AddLast((itemView, config));
            ShowItem(itemView, config);
        }
    }

    private void ShowItem(UIOrderProductItemView itemView, ProductConfig config)
    {
        ActvateItem(itemView);
        SetupItem(itemView, config);
    }

    private void SetupItem(UIOrderProductItemView itemView, ProductConfig config)
    {
        itemView.SetProductIcon(_spritesProvider.GetProductIcon(config.Key));
        var warehouseVolume = _shopModel.WarehouseModel.Volume;
        var price = config.GetPriceForVolume(warehouseVolume);
        itemView.SetPrice(price.IsGold, price.Value);
        itemView.SetTitleText(_loc.GetLocalization($"{LocalizationKeys.NameProductIdPrefix}{config.NumericId}"));
        var description = string.Format(
                _loc.GetLocalization(LocalizationKeys.PopupOrderProductItemDescriptionText),
                config.GetDemandForVolume(warehouseVolume),
                config.GetAmountInVolume(warehouseVolume),
                FormattingHelper.ToSeparatedTimeFormat(config.DeliverTimeSeconds),
                config.GetSellPriceForVolume(warehouseVolume));
        itemView.SetDescriptionText(description);
    }

    private UIOrderProductItemView AddItemAt(int index)
    {
        UIOrderProductItemView result;
        if (_cachedItems.Count > 0)
        {
            result = _cachedItems.Dequeue();
            result.transform.SetParent(_popupView.ContentRectTransform);
        }
        else
        {
            var go = GameObject.Instantiate(_prefabsHolder.UIOrderProductPopupItemPrefab, _popupView.ContentRectTransform);
            result = go.GetComponent<UIOrderProductItemView>();
        }
        var rectTransform = result.transform as RectTransform;
        var rect = rectTransform.rect;
        rectTransform.anchoredPosition = new Vector2(index % 2 * rect.width, -rect.height * (index / 2));
        return result;
    }
}
