using Src.Common;
using Src.Managers;
using UnityEngine;

public class UIErrorPopupMediator : IMediator
{
    private readonly RectTransform _contentTransform;
    private readonly ErrorPopupViewModel _popupModel;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly LocalizationManager _loc;
    private readonly Dispatcher _dispatcher;
    private readonly SpritesProvider _spritesProvider;

    //
    private UITextPopupView _popupView;

    public UIErrorPopupMediator(RectTransform contentTransform)
    {
        _contentTransform = contentTransform;

        _popupModel = GameStateModel.Instance.ShowingPopupModel as ErrorPopupViewModel;
        _prefabsHolder = PrefabsHolder.Instance;
        _loc = LocalizationManager.Instance;
        _dispatcher = Dispatcher.Instance;
        _spritesProvider = SpritesProvider.Instance;
    }

    public async void Mediate()
    {
        var popupGo = GameObject.Instantiate(_prefabsHolder.UITextPopupPrefab, _contentTransform);
        _popupView = popupGo.GetComponent<UITextPopupView>();
        _popupView.Setup(haveCloseButton: false, bottomButtonsAmount: 1);
        _popupView.SetTitleText(_loc.GetLocalization(LocalizationKeys.PopupErrorTitle));
        _popupView.SetMessageText(_popupModel.ErrorText);
        _popupView.SetupButton(0, _spritesProvider.GetBlueButtonSprite(), _loc.GetLocalization(LocalizationKeys.PopupErrorOk));

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
        _popupView.ButtonCloseClicked += OnCloseClicked;
    }

    private void Deactivate()
    {
        _popupView.Button1Clicked -= OnCloseClicked;
        _popupView.ButtonCloseClicked -= OnCloseClicked;
    }

    private void OnCloseClicked()
    {
        _dispatcher.UIRequestRemoveCurrentPopup();
    }
}
