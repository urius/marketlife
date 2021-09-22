using System.Linq;
using UnityEngine;

public class UIDailyBonusPopupMediator : IMediator
{
    private readonly RectTransform _contentTransform;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly LocalizationManager _loc;
    private readonly SpritesProvider _spritesProvider;
    private readonly Dispatcher _dispatcher;
    private readonly UserModel _playerModel;

    //
    private DailyBonusPopupViewModel _viewModel;
    private UIDailyBonusPopupView _popupView;

    public UIDailyBonusPopupMediator(RectTransform contentTransform)
    {
        _contentTransform = contentTransform;

        _prefabsHolder = PrefabsHolder.Instance;
        _loc = LocalizationManager.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
        _playerModel = PlayerModelHolder.Instance.UserModel;
    }

    public async void Mediate()
    {
        _viewModel = GameStateModel.Instance.ShowingPopupModel as DailyBonusPopupViewModel;

        var popupGo = GameObject.Instantiate(_prefabsHolder.UIDailyBonusPopupPrefab, _contentTransform);
        _popupView = popupGo.GetComponent<UIDailyBonusPopupView>();

        SetupView();

        await _popupView.Appear2Async();

        Acttivate();
    }

    public async void Unmediate()
    {
        Deactivate();

        await _popupView.Disppear2Async();

        GameObject.Destroy(_popupView.gameObject);
    }

    private void Acttivate()
    {
        _popupView.ButtonCloseClicked += OnButtonCloseClicked;
        _popupView.TakeButtonClicked += OnTakeButtonClicked;
        _playerModel.BonusStateUpdated += OnBonusStateUpdated;
    }

    private void Deactivate()
    {
        _popupView.ButtonCloseClicked -= OnButtonCloseClicked;
        _popupView.TakeButtonClicked -= OnTakeButtonClicked;
        _playerModel.BonusStateUpdated -= OnBonusStateUpdated;
    }

    private void OnBonusStateUpdated()
    {
        if (DateTimeHelper.IsSameDays(_playerModel.BonusState.LastBonusTakeTimestamp, _viewModel.OpenTimestamp))
        {
            _popupView.SetTakeButtonInteractable(false);
        }
    }

    private void OnButtonCloseClicked()
    {
        _dispatcher.UIRequestRemoveCurrentPopup();
    }

    private void OnTakeButtonClicked()
    {
        var itemViewsPositions = _popupView.ItemViews.Select(v => v.transform.position).ToArray();
        _dispatcher.UIDailyBonusTakeClicked(itemViewsPositions);
    }

    private void SetupView()
    {
        _popupView.SetTitleText(_loc.GetLocalization(LocalizationKeys.PopupDailyBonusTitle));

        var configItems = _viewModel.BonusConfig.DailyBonusConfig;
        for (var i = 0; i < configItems.Length; i++)
        {
            SetupItemView(_popupView.ItemViews[i], configItems[i]);
        }
    }

    private void SetupItemView(UIDailyBonusPopupPrizeItemView itemView, DailyBonusConfig itemConfig)
    {
        itemView.SetDayText(string.Format(_loc.GetLocalization(LocalizationKeys.PopupDailyBonusDayFormat), itemConfig.DayNum));
        itemView.SetIconSprite(itemConfig.Reward.IsGold ? _spritesProvider.GetGoldIcon() : _spritesProvider.GetCashIcon());
        itemView.SetValueText($"+{FormattingHelper.ToCommaSeparatedNumber(itemConfig.Reward.Value)}");
        itemView.SetAlpha(itemConfig.DayNum <= _viewModel.CurrentBonusDay ? 1 : 0.4f);
    }
}
