using System;
using System.Collections.Generic;
using System.Linq;
using Src.Common;
using Src.Managers;
using Src.Model;
using Src.Model.Configs;
using Src.Model.Popups;
using Src.View.UI.Tutorial;
using UnityEngine;

namespace Src.View.UI.Popups.OrderProduct_Popup
{
    public class UIOrderProductsPopupMediator : IMediator
    {
        private const int DisplayedItemsAmount = 14;

        private readonly RectTransform _parentTransform;
        private readonly LocalizationManager _loc;
        private readonly Dispatcher _dispatcher;
        private readonly ScreenCalculator _screenCalculator;
        private readonly UpdatesProvider _updatesProvider;
        private readonly SpritesProvider _spritesProvider;
        private readonly ShopModel _shopModel;
        private readonly UserModel _playerModel;
        private readonly PrefabsHolder _prefabsHolder;
        private readonly PoolCanvasProvider _poolCanvasProvider;
        private readonly TutorialUIElementsProvider _tutorialUIElementsProvider;
        private readonly LinkedList<(UIOrderProductItemView View, ProductConfig Config)> _displayedItems = new();
        private readonly Queue<UIOrderProductItemView> _cachedItems = new();

        private UITabbedContentPopupView _popupView;
        private OrderProductPopupViewModel _viewModel;
        private Rect _itemRect;
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
            _playerModel = PlayerModelHolder.Instance.UserModel;
            _shopModel = _playerModel.ShopModel;
            _prefabsHolder = PrefabsHolder.Instance;
            _poolCanvasProvider = PoolCanvasProvider.Instance;
            _tutorialUIElementsProvider = TutorialUIElementsProvider.Instance;
        }

        public async void Mediate()
        {
            _viewModel = GameStateModel.Instance.ShowingPopupModel as OrderProductPopupViewModel;

            _itemRect = (_prefabsHolder.UIOrderProductPopupItemPrefab.transform as RectTransform).rect;
            var popupGo = GameObject.Instantiate(_prefabsHolder.UITabbedContentPopupPrefab, _parentTransform);
            _popupView = popupGo.GetComponent<UITabbedContentPopupView>();
            _popupView.SetupTabButtons(_viewModel.TabIds
                .Select(id => _loc.GetLocalization($"{LocalizationKeys.NameProductGroupIdPrefix}{id}"))
                .ToArray());
            _popupView.SetSize(1048, 868);

            ShowTab(_viewModel.SelectedTabIndex);

            await _popupView.Appear2Async();

            _dispatcher.UIOrderPopupAppeared();

            Activate();
        }

        public async void Unmediate()
        {
            _tutorialUIElementsProvider.ClearElement(TutorialUIElement.OrderProductsPopupFirstItem);

            Deactivate();
            foreach (var itemView in _cachedItems)
            {
                GameObject.Destroy(itemView.gameObject);
            }
            _cachedItems.Clear();

            await _popupView.Disppear2Async();

            GameObject.Destroy(_popupView.gameObject);
        }

        private void Activate()
        {
            _popupView.TabButtonClicked += OnTabButtonClicked;
            _popupView.ButtonCloseClicked += OnCloseClicked;
            _viewModel.TabSelected += OnTabSelected;
            _updatesProvider.RealtimeUpdate += OnRealtimeUpdate;
        }

        private void Deactivate()
        {
            _popupView.TabButtonClicked -= OnTabButtonClicked;
            _popupView.ButtonCloseClicked -= OnCloseClicked;
            _viewModel.TabSelected -= OnTabSelected;
            _updatesProvider.RealtimeUpdate -= OnRealtimeUpdate;
            foreach (var displayedItem in _displayedItems)
            {
                DeactivateItem(displayedItem.View);
            }
        }

        private void OnTabSelected(int tabIndex)
        {
            ShowTab(tabIndex);
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
            _viewModel.OnViewTabClicked(buttonIndex);
        }

        private void ShowTab(int buttonIndex)
        {
            foreach (var item in _displayedItems)
            {
                HideItem(item.View);
            }
            _displayedItems.Clear();

            _currentTabProductConfigs = _viewModel.GetProductsByTabIndex(buttonIndex);
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
            if (_tutorialUIElementsProvider.HasElement(TutorialUIElement.OrderProductsPopupFirstItem) == false)
            {
                _tutorialUIElementsProvider.SetElement(TutorialUIElement.OrderProductsPopupFirstItem, itemView.transform as RectTransform);
            }

            itemView.SetProductIcon(_spritesProvider.GetProductIcon(config.Key));
            var warehouseVolume = _shopModel.WarehouseModel.Volume;
            var price = config.GetPriceForVolume(warehouseVolume);
            itemView.SetPrice(price.IsGold, price.Value);
            itemView.SetTitleText(_loc.GetLocalization($"{LocalizationKeys.NameProductIdPrefix}{config.NumericId}"));
            itemView.SetDemandText(_loc.GetLocalization(LocalizationKeys.PopupOrderProductDemandText));
            itemView.SetDemandPercent(config.DemandPer1000v/config.DemandPer1000vMax);

            var profitText = RichTextHelper.FormatColor($"+{config.GetProfitForVolume(warehouseVolume)}$", "#068220");
            var deliverTimeText = RichTextHelper.FormatBlue(FormattingHelper.ToSeparatedTimeFormat(config.DeliverTimeSeconds));
            var description =
                $"{_loc.GetLocalization(LocalizationKeys.PopupOrderProductDeliverText)} {deliverTimeText}" +
                $"\n\n{_loc.GetLocalization(LocalizationKeys.PopupOrderProductProfitText)} {profitText}";
                
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
}
