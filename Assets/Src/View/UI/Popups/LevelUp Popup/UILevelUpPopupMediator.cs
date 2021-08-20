using UnityEngine;

public class UILevelUpPopupMediator : UIContentPopupMediator
{
    private readonly RectTransform _parentTransform;

    private readonly PrefabsHolder _prefabsHolder;
    private readonly GameStateModel _gameStateModel;
    private readonly LocalizationManager _loc;
    private readonly SpritesProvider _spritesProvider;
    private readonly Dispatcher _dispatcher;
    private readonly UserProgressModel _playerProgressModel;

    private UIContentPopupPlusView _popupView;
    private LevelUpPopupViewModel _viewModel;

    public UILevelUpPopupMediator(RectTransform parentTransform)
    {
        _parentTransform = parentTransform;

        _prefabsHolder = PrefabsHolder.Instance;
        _gameStateModel = GameStateModel.Instance;
        _loc = LocalizationManager.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
        _playerProgressModel = PlayerModelHolder.Instance.UserModel.ProgressModel;
    }

    protected override UIContentPopupView PopupView => _popupView;

    public override void Mediate()
    {
        _dispatcher.UIRequestBlockRaycasts();

        _viewModel = _gameStateModel.ShowingPopupModel as LevelUpPopupViewModel;

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
        _dispatcher.UITopPanelLevelUpAnimationFinished += OnUITopPanelLevelUpAnimationFinished;
    }

    private async void OnUITopPanelLevelUpAnimationFinished()
    {
        _dispatcher.UIRequestUnblockRaycasts();

        _dispatcher.UITopPanelLevelUpAnimationFinished -= OnUITopPanelLevelUpAnimationFinished;

        var popupGo = GameObject.Instantiate(_prefabsHolder.UIContentPopupPlusPrefab, _parentTransform);
        _popupView = popupGo.GetComponent<UIContentPopupPlusView>();

        Setup();

        await _popupView.Appear2Async();

        ActivatePopup();
    }

    private void Setup()
    {
        _popupView.SetTitleText(_loc.GetLocalization(LocalizationKeys.PopupLevelUpTitle));
        _popupView.SetCaptionText(string.Format(_loc.GetLocalization(LocalizationKeys.PopupLevelUpMessage), _playerProgressModel.Level));
        _popupView.SetupButtonsAmount(haveCloseButton: false, 1);
        _popupView.SetupButton(0, _spritesProvider.GetGreenButtonSprite(), _loc.GetLocalization(LocalizationKeys.CommonContinue));
        _popupView.SetSize(680, 740);

        SetupContent();
    }

    private void ActivatePopup()
    {
        _popupView.Button1Clicked += OnContinueButtonCLicked;
    }

    private void Deactivate()
    {
        _popupView.Button1Clicked -= OnContinueButtonCLicked;
        _dispatcher.UITopPanelLevelUpAnimationFinished -= OnUITopPanelLevelUpAnimationFinished;
    }

    private void OnContinueButtonCLicked()
    {
        _dispatcher.UIRequestRemoveCurrentPopup();
    }

    private void SetupContent()
    {
        PutCaption(_loc.GetLocalization(LocalizationKeys.PopupLevelUpNewElements));
        if (_viewModel.NewProducts.Count > 0)
        {
            //PutCaption(_loc.GetLocalization(LocalizationKeys.PopupLevelUpNewProducts));
            foreach (var product in _viewModel.NewProducts)
            {
                var productName = _loc.GetLocalization($"{LocalizationKeys.NameProductIdPrefix}{product.NumericId}");
                var groupName = _loc.GetLocalization($"{LocalizationKeys.NameProductGroupIdPrefix}{product.GroupId}");
                PutItem(_spritesProvider.GetProductIcon(product.Key), $"{productName} ({groupName})");
            }
        }

        if (_viewModel.NewShelfs.Count > 0)
        {
            //PutCaption(_loc.GetLocalization(LocalizationKeys.PopupLevelUpNewShelfs));
            foreach (var shelf in _viewModel.NewShelfs)
            {
                PutItem(_spritesProvider.GetShelfIcon(shelf.NumericId), _loc.GetLocalization($"{LocalizationKeys.NameShopObjectPrefix}s_{shelf.NumericId}"));
            }
        }

        if (_viewModel.HasNewDecor)
        {
            //PutCaption(_loc.GetLocalization(LocalizationKeys.PopupLevelUpNewDecor));
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
            //PutCaption(_loc.GetLocalization(LocalizationKeys.PopupLevelUpNewPersonal));
            foreach (var personal in _viewModel.NewPersonal)
            {
                PutItem(_spritesProvider.GetPersonalIcon(personal.Key), _loc.GetLocalization($"{LocalizationKeys.CommonPersonalNamePrefix}{personal.TypeIdStr}"));
            }
        }

        if (_viewModel.NewUpgrades.Count > 0)
        {
            //PutCaption(_loc.GetLocalization(LocalizationKeys.PopupLevelUpNewUpgrades));
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
        var itemRect = GetOrCreateItemToDisplay(_prefabsHolder.UILevelUpPopupItemPrefab);
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
}
