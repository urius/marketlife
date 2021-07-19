using System;
using UnityEngine;

public class UILevelUpPopupMediator : UIContentPopupMediator
{
    private readonly RectTransform _parentTransform;

    private readonly PrefabsHolder _prefabsHolder;
    private readonly GameStateModel _gameStateModel;
    private readonly LocalizationManager _loc;
    private readonly SpritesProvider _spritesProvider;

    private UIContentPopupPlusView _popupView;
    private LevelUpPopupViewModel _viewModel;

    public UILevelUpPopupMediator(RectTransform parentTransform)
    {
        _parentTransform = parentTransform;

        _prefabsHolder = PrefabsHolder.Instance;
        _gameStateModel = GameStateModel.Instance;
        _loc = LocalizationManager.Instance;
        _spritesProvider = SpritesProvider.Instance;
    }

    protected override UIContentPopupView PopupView => _popupView;

    public override async void Mediate()
    {
        _viewModel = _gameStateModel.ShowingPopupModel as LevelUpPopupViewModel;
        var popupGo = GameObject.Instantiate(_prefabsHolder.UIContentPopupPlusPrefab, _parentTransform);
        _popupView = popupGo.GetComponent<UIContentPopupPlusView>();

        SetupContent();

        await _popupView.Appear2Async();
    }

    private void SetupContent()
    {
        if (_viewModel.NewProducts.Count > 0)
        {
            PutCaption(_loc.GetLocalization(LocalizationKeys.PopupLevelUpNewProducts));
            foreach (var product in _viewModel.NewProducts)
            {
                var productName = _loc.GetLocalization($"{LocalizationKeys.NameProductIdPrefix}{product.NumericId}");
                var groupName = _loc.GetLocalization($"{LocalizationKeys.NameProductGroupIdPrefix}{product.GroupId}");
                PutItem(_spritesProvider.GetProductIcon(product.Key), $"{productName} ({groupName})");
            }
        }

        if (_viewModel.NewShelfs.Count > 0)
        {
            PutCaption(_loc.GetLocalization(LocalizationKeys.PopupLevelUpNewShelfs));
            foreach (var shelf in _viewModel.NewShelfs)
            {
                PutItem(_spritesProvider.GetShelfIcon(shelf.NumericId), _loc.GetLocalization($"{LocalizationKeys.NameShopObjectPrefix}s_{shelf.NumericId}"));
            }
        }

        if (_viewModel.HasNewDecor)
        {
            PutCaption(_loc.GetLocalization(LocalizationKeys.PopupLevelUpNewDecor));
            foreach (var floor in _viewModel.NewFloors)
            {
                PutItem(_spritesProvider.GetFloorIcon(floor.NumericId), string.Format(_loc.GetLocalization(LocalizationKeys.PopupLevelUpFloorFormat), floor.NumericId));
            }

            foreach (var wall in _viewModel.NewWalls)
            {
                PutItem(_spritesProvider.GetWallIcon(wall.NumericId), string.Format(_loc.GetLocalization(LocalizationKeys.PopupLevelUpWallFormat), wall.NumericId));
            }

            foreach (var window in _viewModel.NewWindows)
            {
                PutItem(_spritesProvider.GetWindowIcon(window.NumericId), string.Format(_loc.GetLocalization(LocalizationKeys.PopupLevelUpWindowFormat), window.NumericId));
            }

            foreach (var door in _viewModel.NewDoors)
            {
                PutItem(_spritesProvider.GetWindowIcon(door.NumericId), string.Format(_loc.GetLocalization(LocalizationKeys.PopupLevelUpWindowFormat), door.NumericId));
            }
        }

        if (_viewModel.NewPersonal.Count > 0)
        {
            PutCaption(_loc.GetLocalization(LocalizationKeys.PopupLevelUpNewPersonal));
            foreach (var personal in _viewModel.NewPersonal)
            {
                PutItem(_spritesProvider.GetPersonalIcon(personal.Key), _loc.GetLocalization($"{LocalizationKeys.CommonPersonalNamePrefix}{personal.TypeIdStr}"));
            }
        }

        if (_viewModel.NewUpgrades.Count > 0)
        {
            PutCaption(_loc.GetLocalization(LocalizationKeys.PopupLevelUpNewUpgrades));
            foreach (var upgrade in _viewModel.NewUpgrades)
            {
                var upgradeName = string.Format(
                    _loc.GetLocalization($"{LocalizationKeys.CommonUpgradeNameFormat}{upgrade.UpgradeTypeStr}"),
                    upgrade.Value);
                PutItem(_spritesProvider.GetUpgradeIcon(upgrade.UpgradeType), upgradeName);
            }
        }
    }

    private void PutItem(Sprite sprite, string text)
    {
        var itemRect = GetOrCreateItemToDisplay(_prefabsHolder.UILevelUpPopupCaptionItemPrefab);
        var itemView = itemRect.GetComponent<UILevelUpPopupItemView>();
        itemView.SetImageSprite(sprite);
        itemView.SetText(text);
    }

    private void PutCaption(string text)
    {
        var captionRect = GetOrCreateItemToDisplay(_prefabsHolder.UILevelUpPopupCaptionItemPrefab);
        var captionView = captionRect.GetComponent<UIPopupCaptionItemView>();
        captionView.SetText(text);
    }

    public override async void Unmediate()
    {
        base.Unmediate();

        await _popupView.Disppear2Async();

        GameObject.Destroy(_popupView);
    }
}
