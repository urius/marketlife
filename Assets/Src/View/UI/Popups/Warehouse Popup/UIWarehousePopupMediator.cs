using System.Collections.Generic;
using Src.Common;
using Src.Managers;
using Src.Model;
using UnityEngine;

namespace Src.View.UI.Popups.Warehouse_Popup
{
    public class UIWarehousePopupMediator : IMediator
    {
        private readonly RectTransform _parentTransfoem;
        private readonly GameStateModel _gameStateModel;
        private readonly PrefabsHolder _prefabsHolder;
        private readonly LocalizationManager _loc;
        private readonly Dispatcher _dispatcher;
        private readonly SpritesProvider _spritesProvider;

        private UIContentPopupView _popupView;
        private Vector2Int _popupSize;
        private List<(UIWarehousePopupItemView View, ProductSlotModel ViewModel)> _displayedItems = new List<(UIWarehousePopupItemView View, ProductSlotModel ViewModel)>();

        public UIWarehousePopupMediator(RectTransform parentTransfoem)
        {
            _parentTransfoem = parentTransfoem;

            _gameStateModel = GameStateModel.Instance;
            _prefabsHolder = PrefabsHolder.Instance;
            _loc = LocalizationManager.Instance;
            _dispatcher = Dispatcher.Instance;
            _spritesProvider = SpritesProvider.Instance;
        }

        public async void Mediate()
        {
            var popupGo = GameObject.Instantiate(_prefabsHolder.UIContentPopupPrefab, _parentTransfoem);
            _popupView = popupGo.GetComponent<UIContentPopupView>();
            _popupSize = new Vector2Int(940, 750);
            _popupView.SetSize(_popupSize.x, _popupSize.y);
            _popupView.SetTitleText(_loc.GetLocalization(LocalizationKeys.PopupWarehouseTitle));

            CreateItems();

            await _popupView.Appear2Async();

            Activate();
        }

        public async void Unmediate()
        {
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
            foreach (var item in _displayedItems)
            {
                DeactivateItem(item.View);
            }
        }

        private void AactivateItem(UIWarehousePopupItemView view)
        {
            view.Clicked += OnItemClicked;
        }

        private void DeactivateItem(UIWarehousePopupItemView view)
        {
            view.Clicked -= OnItemClicked;
        }

        private void OnCloseClicked()
        {
            _dispatcher.UIRequestRemoveCurrentPopup();
        }

        private void OnItemClicked(UIWarehousePopupItemView itemView)
        {
            foreach (var (View, ViewModel) in _displayedItems)
            {
                if (View == itemView)
                {
                    _dispatcher.UIWarehousePopupSlotClicked(ViewModel.Index);
                    return;
                }
            }
        }

        private void CreateItems()
        {
            var itemSize = (_prefabsHolder.UIWarehousePopupItemPrefab.transform as RectTransform).sizeDelta;
            var maxColumns = _popupSize.x / (int)itemSize.x;

            var playerModelHolder = PlayerModelHolder.Instance;
            var warehouseSlots = playerModelHolder.ShopModel.WarehouseModel.Slots;
            var i = 0;
            var serverTime = _gameStateModel.ServerTime;
            var contentSizeY = 0f;
            foreach (var slot in warehouseSlots)
            {
                if (slot.HasProduct && slot.Product.DeliverTime <= serverTime)
                {
                    var slotGo = GameObject.Instantiate(_prefabsHolder.UIWarehousePopupItemPrefab, _popupView.ContentRectTransform);
                    var itemView = slotGo.GetComponent<UIWarehousePopupItemView>();
                    var rectTransform = itemView.transform as RectTransform;
                    var position = new Vector2((i % maxColumns) * itemSize.x, -(i / maxColumns) * itemSize.y);
                    rectTransform.anchoredPosition = position;
                    contentSizeY = Mathf.Max(contentSizeY, Mathf.Abs(position.y) + itemSize.y);
                    _displayedItems.Add((itemView, slot));
                    SetupItemView(itemView, slot);
                    AactivateItem(itemView);
                    i++;
                }
            }

            _popupView.SetContentHeight(contentSizeY);
        }

        private void SetupItemView(UIWarehousePopupItemView itemView, ProductSlotModel slot)
        {
            var product = slot.Product;
            itemView.SetIconSprite(_spritesProvider.GetProductIcon(product.Config.Key));
            itemView.SetAmountText(string.Format(_loc.GetLocalization(LocalizationKeys.PopupShelfContentProductDescriptionFormat), product.Amount, slot.GetMaxAmount()));
            var productName = _loc.GetLocalization($"{LocalizationKeys.NameProductIdPrefix}{product.NumericId}");
            itemView.SetupHint(string.Format(_loc.GetLocalization(LocalizationKeys.PopupWarehouseItemHintFormat), productName));
        }
    }
}
