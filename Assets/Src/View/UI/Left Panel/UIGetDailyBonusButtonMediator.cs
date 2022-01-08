public class UIGetDailyBonusButtonMediator : IMediator
{
    private readonly UIGetDailyBonusButtonView _buttonView;
    private readonly GameStateModel _gameStateModel;
    private readonly PlayerModelHolder _playerModelHolder;
    private readonly LocalizationManager _loc;
    private readonly Dispatcher _dispatcher;

    public UIGetDailyBonusButtonMediator(UIGetDailyBonusButtonView buttonView)
    {
        _buttonView = buttonView;

        _gameStateModel = GameStateModel.Instance;
        _playerModelHolder = PlayerModelHolder.Instance;
        _loc = LocalizationManager.Instance;
        _dispatcher = Dispatcher.Instance;
    }

    public async void Mediate()
    {
        _buttonView.SetVisibility(false);

        await _gameStateModel.GameDataLoadedTask;

        SetupView();

        Activate();
    }

    public void Unmediate()
    {
        Deactivate();
    }

    private void Activate()
    {
        _buttonView.Clicked += OnButtonClicked;
        _playerModelHolder.UserModel.BonusStateUpdated += OnBonusStateUpdated;
    }

    private void Deactivate()
    {
        _buttonView.Clicked -= OnButtonClicked;
        _playerModelHolder.UserModel.BonusStateUpdated -= OnBonusStateUpdated;
    }

    private void SetupView()
    {
        var bonusState = _playerModelHolder.UserModel.BonusState;
        if (bonusState.IsOldGameBonusProcessed == false)
        {
            _buttonView.ShowBlue();
            _buttonView.SetVisibility(true);
            _buttonView.SetupHintText(_loc.GetLocalization(LocalizationKeys.HintOldGameBonus));
        }
        else
        {
            var bonusDateTime = DateTimeHelper.GetDateTimeByUnixTimestamp(bonusState.LastBonusTakeTimestamp);
            var serverDateTime = DateTimeHelper.GetDateTimeByUnixTimestamp(_gameStateModel.ServerTime);
            if (DateTimeHelper.IsSameDays(bonusDateTime, serverDateTime))
            {
                _buttonView.SetVisibility(false);
            }
            else if (DateTimeHelper.IsNextDay(bonusDateTime, serverDateTime))
            {
                switch (bonusState.LastTakenBonusRank)
                {
                    case 0:
                        _buttonView.ShowYellow();
                        break;
                    case 1:
                    case 2:
                        _buttonView.ShowGreen();
                        break;
                    default:
                        _buttonView.ShowRed();
                        break;

                }
                _buttonView.SetVisibility(true);
                _buttonView.SetupHintText(_loc.GetLocalization(LocalizationKeys.HintDailyBonus));
            }
            else
            {
                _buttonView.ShowYellow();
                _buttonView.SetVisibility(true);
                _buttonView.SetupHintText(_loc.GetLocalization(LocalizationKeys.HintDailyBonus));
            }
        }
    }

    private void OnBonusStateUpdated()
    {
        SetupView();
    }

    private void OnButtonClicked()
    {
        _dispatcher.UIGetBonusButtonClicked();
    }
}
