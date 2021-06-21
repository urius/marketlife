using System;
using UnityEngine;

public class UIShelfContentPopupMediator : IMediator
{
    private readonly RectTransform _parentransform;
    private readonly GameStateModel _gameStateModel;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly Dispatcher _dispatcher;
    private readonly SpritesProvider _spritesProvider;
    private readonly LocalizationManager _loc;

    private UIContentPopupView _popupView;
    private ShelfContentPopupViewModel _viewModel;
    private UIShelfContentPopupItemView[] _itemsViews;

    public UIShelfContentPopupMediator(RectTransform parentransform)
    {
        _parentransform = parentransform;

        _gameStateModel = GameStateModel.Instance;
        _prefabsHolder = PrefabsHolder.Instance;
        _dispatcher = Dispatcher.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _loc = LocalizationManager.Instance;
    }

    public async void Mediate()
    {
        _viewModel = _gameStateModel.ShowingPopupModel as ShelfContentPopupViewModel;

        var popupGo = GameObject.Instantiate(_prefabsHolder.UIContentPopupPrefab, _parentransform);
        _popupView = popupGo.GetComponent<UIContentPopupView>();
        _popupView.SetSize(524, 600);
        _popupView.SetTitleText(_loc.GetLocalization(LocalizationKeys.PopupShelfContentTitle));

        CreateItems(_viewModel.ShelfModel.Slots);

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
        for (var i = 0; i < _viewModel.ShelfModel.Slots.Length; i++)
        {
            SubscribeToSlot(_viewModel.ShelfModel.Slots[i]);
        }
    }

    private void Deactivate()
    {
        _popupView.ButtonCloseClicked -= OnCloseClicked;
        for (var i = 0; i < _viewModel.ShelfModel.Slots.Length; i++)
        {
            DeactivateView(_itemsViews[i]);
            UnsubscribeFromSlot(_viewModel.ShelfModel.Slots[i]);
        }
    }

    private void SubscribeToSlot(ProductSlotModel slotModel)
    {
        slotModel.ProductIsSet += OnProductSet;
        slotModel.ProductRemoved += OnProductRemoved;
        slotModel.ProductAmountChanged += OnProductAmountChanged;
    }

    private void UnsubscribeFromSlot(ProductSlotModel slotModel)
    {
        slotModel.ProductIsSet -= OnProductSet;
        slotModel.ProductRemoved -= OnProductRemoved;
        slotModel.ProductAmountChanged -= OnProductAmountChanged;
    }

    private void CreateItems(ProductSlotModel[] slotModels)
    {
        _itemsViews = new UIShelfContentPopupItemView[slotModels.Length];
        for (var i = 0; i < slotModels.Length; i++)
        {
            var go = GameObject.Instantiate(_prefabsHolder.UIShelfContentPopupItemPrefab, _popupView.ContentRectTransform);
            var view = go.GetComponent<UIShelfContentPopupItemView>();
            var rectTransform = view.transform as RectTransform;
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -i * rectTransform.rect.height);
            _itemsViews[i] = view;
            ActivateView(view);
            UpdateItemView(view, slotModels[i]);
        }
    }

    private void OnProductSet(int slotIndex)
    {
        UpdateItemView(_itemsViews[slotIndex], _viewModel.ShelfModel.Slots[slotIndex]);
    }

    private void OnProductRemoved(int slotIndex, ProductModel productModel)
    {
        UpdateItemView(_itemsViews[slotIndex], _viewModel.ShelfModel.Slots[slotIndex]);
    }

    private void OnProductAmountChanged(int slotIndex, int deltaAmount)
    {
        UpdateItemView(_itemsViews[slotIndex], _viewModel.ShelfModel.Slots[slotIndex]);
    }

    private void UpdateItemView(UIShelfContentPopupItemView view, ProductSlotModel slotModel)
    {
        var productModel = slotModel.Product;
        var hasProduct = productModel != null;
        var productSprite = hasProduct ? _spritesProvider.GetProductIcon(productModel.Config.Key) : null;
        view.SetProductIcon(productSprite);

        view.SetMainButtonInteractable(!hasProduct);
        view.SetRemoveButtonVisibility(hasProduct);

        if (hasProduct)
        {
            view.SetTitleText(_loc.GetLocalization($"{LocalizationKeys.NameProductIdPrefix}{productModel.Config.NumericId}"));
            view.SetDescriptionText(string.Format(_loc.GetLocalization(LocalizationKeys.PopupShelfContentProductDescriptionFormat), productModel.Amount, slotModel.GetMaxAmount()));
        }
        else
        {
            view.SetTitleText(_loc.GetLocalization(LocalizationKeys.PopupShelfContentProductEmpty));
            view.SetDescriptionText(_loc.GetLocalization(LocalizationKeys.PopupShelfContentProductTakeFromWarehouse));
        }
    }

    private void ActivateView(UIShelfContentPopupItemView view)
    {
        view.Cliked += OnViewClicked;
        view.RemoveCliked += OnViewRemoveButtonClicked;
    }

    private void DeactivateView(UIShelfContentPopupItemView view)
    {
        view.Cliked -= OnViewClicked;
        view.RemoveCliked -= OnViewRemoveButtonClicked;
    }

    private void OnViewClicked(UIShelfContentPopupItemView view)
    {
        var index = Array.IndexOf(_itemsViews, view);
        _dispatcher.UIShelfContentAddProductClicked(_viewModel, index);
    }

    private void OnViewRemoveButtonClicked(UIShelfContentPopupItemView view)
    {
        var index = Array.IndexOf(_itemsViews, view);
        _dispatcher.UIShelfContentRemoveProductClicked(_viewModel, index);
    }

    private void OnCloseClicked()
    {
        _dispatcher.UIRequestRemoveCurrentPopup();
    }
}
