using Src.Common;
using Src.Managers;
using Src.Model;
using Src.Model.Popups;
using UnityEngine;

namespace Src.View.UI.Popups.CashDeskPopup
{
    public class UICashDeskPopupMediator : IMediator
    {
        private readonly RectTransform _contentTransform;
        private readonly GameStateModel _gameSatteModel;
        private readonly PrefabsHolder _prefabsHolder;
        private readonly LocalizationManager _loc;
        private readonly Dispatcher _dispatcher;
        private readonly SpritesProvider _spritesProvider;

        //
        private CashDeskPopupViewModel _viewModel;
        private UICashDeskPopupView _popupView;

        public UICashDeskPopupMediator(RectTransform contentTransform)
        {
            _contentTransform = contentTransform;

            _gameSatteModel = GameStateModel.Instance;
            _prefabsHolder = PrefabsHolder.Instance;
            _loc = LocalizationManager.Instance;
            _dispatcher = Dispatcher.Instance;
            _spritesProvider = SpritesProvider.Instance;
        }

        public async void Mediate()
        {
            _viewModel = _gameSatteModel.ShowingPopupModel as CashDeskPopupViewModel;
            var go = GameObject.Instantiate(_prefabsHolder.UICashDeskPopupPrefab, _contentTransform);
            _popupView = go.GetComponent<UICashDeskPopupView>();
            _popupView.SetTitleText(_loc.GetLocalization(LocalizationKeys.PopupCashDeskTitle));
            UpdateCashManView();

            Activate();

            await _popupView.Appear2Async();
        }

        public async void Unmediate()
        {
            Deactivate();

            await _popupView.Disppear2Async();

            GameObject.Destroy(_popupView.gameObject);
            _popupView = null;
        }

        private void Activate()
        {
            _viewModel.CashDeskModel.DisplayItemChanged += OnCashDeskDisplayItemChanged;
            _popupView.ChangeHairClicked += OnChangeHairClicked;
            _popupView.ChangeGlassesClicked += OnChangeGlassesClicked;
            _popupView.ChangeDressClicked += OnChangeDressClicked;
            _popupView.ButtonCloseClicked += OnCloseClicked;
            _popupView.ApplyClicked += OnCloseClicked;
        }

        private void Deactivate()
        {
            _viewModel.CashDeskModel.DisplayItemChanged -= OnCashDeskDisplayItemChanged;
            _popupView.ChangeHairClicked -= OnChangeHairClicked;
            _popupView.ChangeGlassesClicked -= OnChangeGlassesClicked;
            _popupView.ChangeDressClicked -= OnChangeDressClicked;
            _popupView.ButtonCloseClicked -= OnCloseClicked;
            _popupView.ApplyClicked -= OnCloseClicked;
        }

        private void OnCashDeskDisplayItemChanged()
        {
            UpdateCashManView();
        }

        private void UpdateCashManView()
        {
            var cashDeskModel = _viewModel.CashDeskModel;
            _popupView.SetHairSprite(_spritesProvider.GetHumanHairSprite(cashDeskModel.HairId));
            _popupView.SetGlassesSprite(_spritesProvider.GetHumanGlassesSprite(cashDeskModel.GlassesId));
            _popupView.SetTorsoSprite(_spritesProvider.GetTopDressSprite(cashDeskModel.DressId));
            var handSprite = _spritesProvider.GetHandDressSprite(cashDeskModel.DressId);
            _popupView.SetLeftHandSprite(handSprite);
            _popupView.SetRightHandSprite(handSprite);
        }

        private void OnCloseClicked()
        {
            _dispatcher.UIRequestRemoveCurrentPopup();
        }

        private void OnChangeDressClicked(int direction)
        {
            _dispatcher.UIChangeCashDeskManDressClicked(_viewModel.CashDeskModel.DressId + direction);
        }

        private void OnChangeGlassesClicked(int direction)
        {
            _dispatcher.UIChangeCashDeskManGlassesClicked(_viewModel.CashDeskModel.GlassesId + direction);
        }

        private void OnChangeHairClicked(int direction)
        {
            _dispatcher.UIChangeCashDeskManHairClicked(_viewModel.CashDeskModel.HairId + direction);
        }
    }
}
