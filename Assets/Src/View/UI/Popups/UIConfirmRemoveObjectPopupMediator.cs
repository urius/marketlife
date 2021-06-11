using UnityEngine;

public class UIConfirmRemoveObjectPopupMediator : IMediator
{
    private readonly RectTransform _contentTransform;
    private readonly Dispatcher _dispatcher;
    private readonly IConfirmRemoveObjectPopupViewModel _popupModel;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly SpritesProvider _spritesProvider;
    private readonly LocalizationManager _loc;

    private UITextPopupView _popupView;

    public UIConfirmRemoveObjectPopupMediator(RectTransform contentTransform)
    {
        _contentTransform = contentTransform;

        _dispatcher = Dispatcher.Instance;
        _popupModel = GameStateModel.Instance.ShowingPopupModel as IConfirmRemoveObjectPopupViewModel;
        _prefabsHolder = PrefabsHolder.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _loc = LocalizationManager.Instance;
    }

    public async void Mediate()
    {
        var popupGo = GameObject.Instantiate(_prefabsHolder.UITextPopupPrefab, _contentTransform);
        _popupView = popupGo.GetComponent<UITextPopupView>();
        _popupView.Setup(haveCloseButton: true, bottomButtonsAmount: 2);
        _popupView.SetupButton(0, _spritesProvider.GetGreenButtonSprite(), _loc.GetLocalization(LocalizationKeys.CommonYes));
        _popupView.SetupButton(1, _spritesProvider.GetOrangeButtonSprite(), _loc.GetLocalization(LocalizationKeys.CommonNo));
        _popupView.SetTitleText(_loc.GetLocalization(LocalizationKeys.PopupRemoveObjectTitle));
        _popupView.SetMessageText(string.Format(_loc.GetLocalization(LocalizationKeys.PopupRemoveObjectText), _popupModel.SellPrice));

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
        _dispatcher.UIRemovePopupResult(true);
        await _popupView.DisppearAsync();
        _dispatcher.UIRequestRemoveCurrentPopup();
    }

    private async void OnNoClicked()
    {
        _dispatcher.UIRemovePopupResult(false);
        await _popupView.DisppearAsync();
        _dispatcher.UIRequestRemoveCurrentPopup();
    }
}
