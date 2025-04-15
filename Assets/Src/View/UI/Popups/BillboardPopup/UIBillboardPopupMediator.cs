using Src.Common;
using Src.Managers;
using UnityEngine;

public class UIBillboardPopupMediator : IMediator
{
    private readonly RectTransform _contentTransform;
    private readonly GameStateModel _gameStateModel;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly LocalizationManager _loc;
    private readonly Dispatcher _dispatcher;

    //
    private UIBillboardPopupView _popupView;
    private BillboardPopupViewModel _viewModel;

    public UIBillboardPopupMediator(RectTransform contentTransform)
    {
        _contentTransform = contentTransform;

        _gameStateModel = GameStateModel.Instance;
        _prefabsHolder = PrefabsHolder.Instance;
        _loc = LocalizationManager.Instance;
        _dispatcher = Dispatcher.Instance;
    }

    public async void Mediate()
    {
        _viewModel = _gameStateModel.ShowingPopupModel as BillboardPopupViewModel;
        var go = GameObject.Instantiate(_prefabsHolder.UIBillboardPopupPrefab, _contentTransform);
        _popupView = go.GetComponent<UIBillboardPopupView>();
        _popupView.SetTitleText(_loc.GetLocalization(LocalizationKeys.PopupBillboardTitle));
        _popupView.SetMessageText(_loc.GetLocalization(LocalizationKeys.PopupBillboardMessage));

        _popupView.SetInputFieldText(_viewModel.InitialText);

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
        _popupView.ApplyClicked += OnApplyClicked;
    }

    private void Deactivate()
    {
        _popupView.ButtonCloseClicked -= OnButtonCloseClicked;
        _popupView.ApplyClicked -= OnApplyClicked;
    }

    private void OnButtonCloseClicked()
    {
        _dispatcher.UIRequestRemoveCurrentPopup();
    }

    private void OnApplyClicked()
    {
        _dispatcher.UIBillboardPopupApplyTextClicked(_popupView.InputFieldText);
        _dispatcher.UIRequestRemoveCurrentPopup();
    }
}