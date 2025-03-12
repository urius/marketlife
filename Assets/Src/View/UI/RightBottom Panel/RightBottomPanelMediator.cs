using Src.Common;
using Src.Managers;
using Src.Model;
using Src.View.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Src.View.UI.RightBottom_Panel
{
    public class RightBottomPanelMediator : MonoBehaviour
    {
        [SerializeField] private UIToggleSpriteButtonView _muteMusicButton;
        [SerializeField] private UIToggleSpriteButtonView _muteAudioButton;
        [SerializeField] private UIToggleSpriteButtonView _scaleInButton;
        [SerializeField] private UIToggleSpriteButtonView _scaleOutButton;
        [SerializeField] private Image _bgImage;

        //
        private GameStateModel _gameStateModel;
        private PlayerModelHolder _playerModelHolder;
        private Dispatcher _dispatcher;
        private UserModel _playerModel;
        private AudioManager _audioManager;

        private void Awake()
        {
            _gameStateModel = GameStateModel.Instance;
            _playerModelHolder = PlayerModelHolder.Instance;
            _dispatcher = Dispatcher.Instance;
            _audioManager = AudioManager.Instance;

            SetButtonsVisibility(false);
        }

        private void SetButtonsVisibility(bool isVisible)
        {
            _bgImage.enabled = isVisible;
            _muteMusicButton.gameObject.SetActive(isVisible && _audioManager.IsMusicAvailable());
            _muteAudioButton.gameObject.SetActive(isVisible);
            _scaleInButton.gameObject.SetActive(isVisible);
            _scaleOutButton.gameObject.SetActive(isVisible);
        }

        private async void Start()
        {
            await _gameStateModel.GameDataLoadedTask;

            _playerModel = _playerModelHolder.UserModel;
            SetButtonsVisibility(true);
            UpdateButtonsView();

            Activate();
        }

        private void Activate()
        {
            _playerModel.SettingsUpdated += OnPlayerSettingsUpdated;
            _muteMusicButton.Clicked += OnMuteMusicClicked;
            _muteAudioButton.Clicked += OnMuteAudioClicked;
            _scaleInButton.Clicked += OnScaleInClicked;
            _scaleOutButton.Clicked += OnScaleOutClicked;
        }

        private void OnScaleInClicked()
        {
            _dispatcher.UIScaleInClicked();
        }

        private void OnScaleOutClicked()
        {
            _dispatcher.UIScaleOutClicked();
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
}
