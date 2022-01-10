using System.Linq;

public class UIDailyMissionsButtonMediator : IMediator
{
    private readonly UIDailyMIssionsButtonView _buttonView;
    private readonly GameStateModel _gameStateModel;
    private readonly PlayerModelHolder _playerModelHodler;
    private readonly Dispatcher _dispatcher;
    private readonly ColorsHolder _colorsHolder;

    //
    private DailyMissionsModel _playerMissionsModel;
    private bool _newMissionsNotificationFlag = false;

    public UIDailyMissionsButtonMediator(UIDailyMIssionsButtonView buttonView)
    {
        _buttonView = buttonView;

        _gameStateModel = GameStateModel.Instance;
        _playerModelHodler = PlayerModelHolder.Instance;
        _dispatcher = Dispatcher.Instance;
        _colorsHolder = ColorsHolder.Instance;
    }

    public void Mediate()
    {
        _playerMissionsModel = _playerModelHodler.UserModel.DailyMissionsModel;
        UpdateButtonView();
        Activate();
    }

    public void Unmediate()
    {
        Deactivate();
    }

    private void UpdateButtonView()
    {
        _buttonView.gameObject.SetActive(_playerMissionsModel.MissionsList.Count > 0);

        if (HaveCompletedMissions())
        {
            _buttonView.SetNotificationColor(_colorsHolder.DailyMissionCompletedMissionsNotificationColor);
            _buttonView.SetNotificationVisibility(true);
        }
        else if (_newMissionsNotificationFlag == true)
        {
            _buttonView.SetNotificationColor(_colorsHolder.DailyMissionNewMissionsNotificationColor);
            _buttonView.SetNotificationVisibility(true);
        }
        else
        {
            _buttonView.SetNotificationVisibility(false);
        }
    }

    private void Activate()
    {
        _buttonView.OnClick += OnButtonClick;
        _playerMissionsModel.MissionAdded += OnMissionAdded;
        _playerMissionsModel.MissionRemoved += OnMissionRemoved;
        _gameStateModel.PopupShown += OnPopupShown;
        _gameStateModel.PopupRemoved += OnPopupRemoved;
        foreach (var mission in _playerMissionsModel.MissionsList)
        {
            SubscribeOnMission(mission);
        }
    }

    private void Deactivate()
    {
        _buttonView.OnClick -= OnButtonClick;
        _playerMissionsModel.MissionAdded -= OnMissionAdded;
        _playerMissionsModel.MissionRemoved -= OnMissionRemoved;
        _gameStateModel.PopupShown -= OnPopupShown;
        _gameStateModel.PopupRemoved -= OnPopupRemoved;
        foreach (var mission in _playerMissionsModel.MissionsList)
        {
            UnsubscribeFromMission(mission);
        }
    }

    private void OnPopupShown()
    {
        if (_gameStateModel.ShowingPopupModel.PopupType == PopupType.DailyMissions)
        {
            _newMissionsNotificationFlag = false;
            UpdateButtonView();
        }
    }

    private void OnPopupRemoved(PopupViewModelBase popupModel)
    {
        if (popupModel.PopupType == PopupType.DailyMissions)
        {
            UpdateButtonView();
        }
    }

    private void OnMissionAdded(DailyMissionModel missionModel)
    {
        _newMissionsNotificationFlag = true;
        UpdateButtonView();
        SubscribeOnMission(missionModel);
    }

    private void SubscribeOnMission(DailyMissionModel missionModel)
    {
        UnsubscribeFromMission(missionModel);
        missionModel.ValueChanged += OnMissionStateChanged;
        missionModel.RewardTaken += OnMissionStateChanged;
    }

    private void UnsubscribeFromMission(DailyMissionModel missionModel)
    {
        missionModel.ValueChanged -= OnMissionStateChanged;
        missionModel.RewardTaken -= OnMissionStateChanged;
    }

    private void OnMissionStateChanged()
    {
        UpdateButtonView();
    }

    private void OnMissionRemoved(DailyMissionModel missionModel)
    {
        UnsubscribeFromMission(missionModel);
        UpdateButtonView();
    }

    private void OnButtonClick()
    {
        _dispatcher.UIDailyMissionsButtonClicked();
    }

    private bool HaveCompletedMissions()
    {
        return _playerMissionsModel.MissionsList.Any(m => m.IsCompleted && m.IsRewardTaken == false);
    }
}
