using Src.Common;
using Src.Managers;
using UnityEngine;

namespace Src.View.UI.Popups.Pause_Popup
{
    public class UIPausePopupMediator : IMediator
    {
        private readonly PrefabsHolder _prefabsHolder = PrefabsHolder.Instance;
        private readonly LocalizationManager _loc = LocalizationManager.Instance;
        private readonly Dispatcher _dispatcher = Dispatcher.Instance;
        private readonly SpritesProvider _spritesProvider = SpritesProvider.Instance;
        private readonly RectTransform _contentTransform;

        private UITextPopupView _popupView;

        public UIPausePopupMediator(RectTransform contentTransform)
        {
            _contentTransform = contentTransform;
        }
        
        public async void Mediate()
        {
            var popupGo = GameObject.Instantiate(_prefabsHolder.UITextPopupPrefab, _contentTransform);
            _popupView = popupGo.GetComponent<UITextPopupView>();
            _popupView.Setup(haveCloseButton: false, bottomButtonsAmount: 1);
            _popupView.SetTitleText(_loc.GetLocalization(LocalizationKeys.PopupPauseTitle));
            _popupView.SetMessageText(_loc.GetLocalization(LocalizationKeys.PopupPauseText));
            _popupView.SetupButton(0, _spritesProvider.GetBlueButtonSprite(), _loc.GetLocalization(LocalizationKeys.CommonContinue));

            await _popupView.AppearAsync();

            Activate();
        }

        public async void Unmediate()
        {
            Deactivate();

            await _popupView.DisppearAsync();

            GameObject.Destroy(_popupView.gameObject);
        }
        
        private void Activate()
        {
            _popupView.Button1Clicked += OnCloseClicked;
        }

        private void Deactivate()
        {
            _popupView.Button1Clicked -= OnCloseClicked;
        }

        private void OnCloseClicked()
        {
            _dispatcher.UIPausePopupCloseClicked();
        }
    }
}