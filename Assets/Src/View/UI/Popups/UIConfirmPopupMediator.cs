using Src.Common;
using Src.Managers;
using Src.Model;
using Src.Model.Popups;
using UnityEngine;

namespace Src.View.UI.Popups
{
    public class UIConfirmPopupMediator : IMediator
    {
        private readonly RectTransform _contentTransform;
        private readonly Dispatcher _dispatcher;
        private readonly ConfirmPopupViewModel _popupModel;
        private readonly PrefabsHolder _prefabsHolder;
        private readonly SpritesProvider _spritesProvider;
        private readonly LocalizationManager _loc;

        private UITextPopupView _popupView;

        public UIConfirmPopupMediator(RectTransform contentTransform)
        {
            _contentTransform = contentTransform;

            _dispatcher = Dispatcher.Instance;
            _popupModel = GameStateModel.Instance.ShowingPopupModel as ConfirmPopupViewModel;
            _prefabsHolder = PrefabsHolder.Instance;
            _spritesProvider = SpritesProvider.Instance;
            _loc = LocalizationManager.Instance;
        }

        public async void Mediate()
        {
            var popupGo = GameObject.Instantiate(_prefabsHolder.UITextPopupPrefab, _contentTransform);
            _popupView = popupGo.GetComponent<UITextPopupView>();
            _popupView.Setup(haveCloseButton: true, bottomButtonsAmount: 2);
            _popupView.SetTitleText(_popupModel.TitleText);
            _popupView.SetMessageText(_popupModel.MessageText);
            _popupView.SetupButton(0, _spritesProvider.GetGreenButtonSprite(), _loc.GetLocalization(LocalizationKeys.CommonYes));
            _popupView.SetupButton(1, _spritesProvider.GetOrangeButtonSprite(), _loc.GetLocalization(LocalizationKeys.CommonNo));

            await _popupView.AppearAsync();
            Activate();
        }

        public void Unmediate()
        {
            Deactivate();
            GameObject.Destroy(_popupView.gameObject);
        }

        private void Activate()
        {
            _popupView.Button1Clicked += OnYesClicked;
            _popupView.Button2Clicked += OnNoClicked;
            _popupView.ButtonCloseClicked += OnNoClicked;
        }

        private void Deactivate()
        {
            _popupView.Button1Clicked -= OnYesClicked;
            _popupView.Button2Clicked -= OnNoClicked;
            _popupView.ButtonCloseClicked -= OnNoClicked;
        }

        private async void OnYesClicked()
        {
            _dispatcher.UIConfirmPopupResult(true);
            await _popupView.DisppearAsync();
            _dispatcher.UIRequestRemoveCurrentPopup();
        }

        private async void OnNoClicked()
        {
            _dispatcher.UIConfirmPopupResult(false);
            await _popupView.DisppearAsync();
            _dispatcher.UIRequestRemoveCurrentPopup();
        }
    }
}
