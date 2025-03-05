using Src.Common;
using Src.Managers;
using Src.Model;
using UnityEngine;

namespace Src.View.UI.Popups.OldGameCompensationPopup
{
    public class UIOldGameCompensationPopupMediator : IMediator
    {
        private readonly RectTransform _contentTransform;
        private readonly OldGameCompensationHolder _compensationHolder;
        private readonly PrefabsHolder _prefabsHolder;
        private readonly Dispatcher _dispatcher;
        private readonly LocalizationManager _loc;

        //
        private UIOldGameCompensationPopupView _popupView;

        public UIOldGameCompensationPopupMediator(RectTransform contentTransform)
        {
            _contentTransform = contentTransform;

            _compensationHolder = OldGameCompensationHolder.Instance;
            _prefabsHolder = PrefabsHolder.Instance;
            _dispatcher = Dispatcher.Instance;
            _loc = LocalizationManager.Instance;
        }

        public async void Mediate()
        {
            var popupGo = GameObject.Instantiate(_prefabsHolder.UIOldGameCompensationPopupPrefab, _contentTransform);
            _popupView = popupGo.GetComponent<UIOldGameCompensationPopupView>();

            SetupPopup();

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
            _popupView.ButtonCloseClicked += OnButtonCloseClicked;
        }

        private void Deactivate()
        {
            _popupView.ButtonCloseClicked -= OnButtonCloseClicked;
        }

        private void OnButtonCloseClicked()
        {
            _dispatcher.UIRequestRemoveCurrentPopup();
        }

        private void SetupPopup()
        {
            _popupView.SetTitleText(_loc.GetLocalization(LocalizationKeys.PopupCompensationTitle));
            _popupView.SetMessageText(_loc.GetLocalization(LocalizationKeys.PopupCompensationMessage));

            var compensationData = _compensationHolder.Compensation;
            _popupView.SetGoldText($"+{FormattingHelper.ToCommaSeparatedNumber(compensationData.AmountGold)}");
            _popupView.SetCashText($"+{FormattingHelper.ToCommaSeparatedNumber(compensationData.AmountCash)}");
        }
    }
}
