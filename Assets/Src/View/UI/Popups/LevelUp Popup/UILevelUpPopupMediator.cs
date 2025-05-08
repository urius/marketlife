using Src.Common;
using Src.Managers;
using Src.Model;
using Src.Model.Popups;
using UnityEngine;

namespace Src.View.UI.Popups.LevelUp_Popup
{
    public class UILevelUpPopupMediator : UIContentPopupMediator
    {
        private readonly RectTransform _parentTransform;

        private readonly PrefabsHolder _prefabsHolder;
        private readonly GameStateModel _gameStateModel;
        private readonly LocalizationManager _loc;
        private readonly SpritesProvider _spritesProvider;
        private readonly Dispatcher _dispatcher;
        private readonly UserProgressModel _playerProgressModel;

        private UILevelUpPopupView _popupView;
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

        private async void OnUITopPanelLevelUpAnimationFinished()
        {
            _dispatcher.UIRequestUnblockRaycasts();

            _dispatcher.UITopPanelLevelUpAnimationFinished -= OnUITopPanelLevelUpAnimationFinished;

            var popupGo = GameObject.Instantiate(_prefabsHolder.UILevelUpPopupPrefab, _parentTransform);
            _popupView = popupGo.GetComponent<UILevelUpPopupView>();

            Setup();

            await _popupView.Appear2Async();

            ActivatePopup();
        }

        private void Setup()
        {
            _popupView.SetTitleText(_loc.GetLocalization(LocalizationKeys.PopupLevelUpTitle));
            _popupView.SetDescriptionText(string.Format(_loc.GetLocalization(LocalizationKeys.PopupLevelUpMessage), _playerProgressModel.Level));
            _popupView.SetupButton(0, _spritesProvider.GetGreenButtonSprite(), _loc.GetLocalization(LocalizationKeys.CommonContinue));
            _popupView.SetupButton(1, _spritesProvider.GetBlueButtonSprite(), _loc.GetLocalization(LocalizationKeys.CommonShare));
            _popupView.SetShareRevenueButtonText($"+{CalculationHelper.GetLevelUpShareReward(_playerProgressModel.Level)}");
            _popupView.SetSize(750, 800);

            _popupView.SetShareButtonVisibility(DisabledLogicFlags.IsSharingDisabled == false);

            SetupContent();
        }

        private void Activate()
        {
            _dispatcher.UIShareSuccessCallback += OnUIShareSuccessCallback;
            _dispatcher.UITopPanelLevelUpAnimationFinished += OnUITopPanelLevelUpAnimationFinished;
        }

        private void ActivatePopup()
        {
            _popupView.ButtonClicked += OnButtonCLicked;
            _popupView.ButtonCloseClicked += OnButtonCloseClicked;
        }

        private void Deactivate()
        {
            _dispatcher.UIShareSuccessCallback -= OnUIShareSuccessCallback;
            _dispatcher.UITopPanelLevelUpAnimationFinished -= OnUITopPanelLevelUpAnimationFinished;
            _popupView.ButtonClicked -= OnButtonCLicked;
            _popupView.ButtonCloseClicked -= OnButtonCloseClicked;
        }

        private void OnButtonCloseClicked()
        {
            _dispatcher.UIRequestRemoveCurrentPopup();
        }

        private void OnUIShareSuccessCallback()
        {
            _popupView.SetShareButtonInteractable(false);
        }

        private void OnButtonCLicked(int buttonIndex)
        {
            switch (buttonIndex)
            {
                case 0:
                    _dispatcher.UIRequestRemoveCurrentPopup();
                    break;
                case 1:
                    _dispatcher.UILevelUpShareClicked(_popupView.ShareButtonTransform.position);
                    break;
            }
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
                    var item = PutItem(_spritesProvider.GetProductIcon(product.Key), $"{productName} ({groupName})");
                    item.SetTextColor(item.GreenColor);
                }
            }

            if (_viewModel.NewShelfs.Count > 0)
            {
                //PutCaption(_loc.GetLocalization(LocalizationKeys.PopupLevelUpNewShelfs));
                foreach (var shelf in _viewModel.NewShelfs)
                {
                    var item = PutItem(_spritesProvider.GetShelfIcon(shelf.NumericId), _loc.GetLocalization($"{LocalizationKeys.NameShopObjectPrefix}s_{shelf.NumericId}"));
                    item.SetTextColor(item.BlueColor);
                }
            }

            if (_viewModel.HasNewDecor)
            {
                //PutCaption(_loc.GetLocalization(LocalizationKeys.PopupLevelUpNewDecor));
                foreach (var floor in _viewModel.NewFloors)
                {
                    var item = PutItem(_spritesProvider.GetFloorIcon(floor.NumericId), string.Format(_loc.GetLocalization(LocalizationKeys.PopupLevelUpFloorFormat), floor.NumericId));
                    item.SetTextColor(item.BlueColor);
                }

                foreach (var wall in _viewModel.NewWalls)
                {
                    var item = PutItem(_spritesProvider.GetWallSprite(wall.NumericId), string.Format(_loc.GetLocalization(LocalizationKeys.PopupLevelUpWallFormat), wall.NumericId));
                    item.SetTextColor(item.BlueColor);
                }

                foreach (var window in _viewModel.NewWindows)
                {
                    var item = PutItem(_spritesProvider.GetWindowIcon(window.NumericId), string.Format(_loc.GetLocalization(LocalizationKeys.PopupLevelUpWindowFormat), window.NumericId));
                    item.SetTextColor(item.BlueColor);
                }

                foreach (var door in _viewModel.NewDoors)
                {
                    var item = PutItem(_spritesProvider.GetWindowIcon(door.NumericId), string.Format(_loc.GetLocalization(LocalizationKeys.PopupLevelUpWindowFormat), door.NumericId));
                    item.SetTextColor(item.BlueColor);
                }
            }

            if (_viewModel.NewPersonal.Count > 0)
            {
                //PutCaption(_loc.GetLocalization(LocalizationKeys.PopupLevelUpNewPersonal));
                foreach (var personal in _viewModel.NewPersonal)
                {
                    var item = PutItem(_spritesProvider.GetPersonalIcon(personal.Key), _loc.GetLocalization($"{LocalizationKeys.CommonPersonalNamePrefix}{personal.TypeIdStr}"));
                    item.SetTextColor(item.OrangeColor);
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
                    var item = PutItem(_spritesProvider.GetUpgradeIcon(upgrade.UpgradeType), upgradeName);
                    item.SetTextColor(item.OrangeColor);
                }
            }
        }

        private UILevelUpPopupItemView PutItem(Sprite sprite, string text)
        {
            var itemRect = GetOrCreateItemToDisplay(_prefabsHolder.UILevelUpPopupItemPrefab);
            var itemView = itemRect.GetComponent<UILevelUpPopupItemView>();
            itemView.SetImageSprite(sprite);
            itemView.SetText(text);
            return itemView;
        }

        private void PutCaption(string text)
        {
            var captionRect = GetOrCreateItemToDisplay(_prefabsHolder.UILevelUpPopupCaptionItemPrefab);
            var captionView = captionRect.GetComponent<UIPopupCaptionItemView>();
            captionView.SetText(text);
        }
    }
}
