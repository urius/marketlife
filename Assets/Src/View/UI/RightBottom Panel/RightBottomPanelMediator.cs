using UnityEngine;

public class RightBottomPanelMediator : MonoBehaviour
{
    [SerializeField] private UIToggleSpriteButtonView _muteMusicButton;
    [SerializeField] private UIToggleSpriteButtonView _muteAudioButton;

    //
    private GameStateModel _gameStateModel;
    private PlayerModelHolder _playerModelHolder;
    private Dispatcher _dispatcher;
    private UserModel _playerModel;

    private void Awake()
    {
        _gameStateModel = GameStateModel.Instance;
        _playerModelHolder = PlayerModelHolder.Instance;

        _dispatcher = Dispatcher.Instance;
    }

    private async void Start()
    {
        await _gameStateModel.GameDataLoadedTask;

        _playerModel = _playerModelHolder.UserModel;
        UpdateButtonsView();

        Activate();
    }

    private void Activate()
    {
        _playerModel.SettingsUpdated += OnPlayerSettingsUpdated;
        _muteMusicButton.Clicked += OnMuteMusicClicked;
        _muteAudioButton.Clicked += OnMuteAudioClicked;
    }

    private void OnMuteAudioClicked()
    {
        _dispatcher.UIMuteAudioClicked();
    }

    private void OnMuteMusicClicked()
    {
        _dispatcher.UIMuteMusicClicked();
    }

    private void OnPlayerSettingsUpdated()
    {
        UpdateButtonsView();
    }

    private void UpdateButtonsView()
    {
        var settingsModel = _playerModel.UserSettingsModel;
        //_muteMusicButton.SetInteractable(settingsModel.IsAudioMuted);
        if (settingsModel.IsAudioMuted)
        {
            _muteAudioButton.ToggleOff();
        }
        else
        {
            _muteAudioButton.ToggleOn();
        }

        if (settingsModel.IsMusicMuted)
        {
            _muteMusicButton.ToggleOff();
        }
        else
        {
            _muteMusicButton.ToggleOn();
        }
    }
}
