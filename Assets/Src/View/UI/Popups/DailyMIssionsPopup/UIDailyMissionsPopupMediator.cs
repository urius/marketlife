using System;
using System.Collections.Generic;
using UnityEngine;

public class UIDailyMissionsPopupMediator : IMediator
{
    private readonly RectTransform _contentTransform;
    private readonly GameStateModel _gameStateModel;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly LocalizationManager _loc;
    private readonly SpritesProvider _spritesProvider;
    private readonly Dispatcher _dispatcher;

    //
    private DailyMissionsPopupViewModel _viewModel;
    private UIDailyMissionsPopupView _popupView;
    private List<UIDailyMissionsPopupMissionItemView> _displayedItems = new List<UIDailyMissionsPopupMissionItemView>();

    public UIDailyMissionsPopupMediator(RectTransform contentTransform)
    {
        _contentTransform = contentTransform;

        _gameStateModel = GameStateModel.Instance;
        _prefabsHolder = PrefabsHolder.Instance;
        _loc = LocalizationManager.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
    }

    public async void Mediate()
    {
        _viewModel = _gameStateModel.ShowingPopupModel as DailyMissionsPopupViewModel;

        var popupGo = GameObject.Instantiate(_prefabsHolder.UIDailyMissionsPopupPrefab, _contentTransform);
        _popupView = popupGo.GetComponent<UIDailyMissionsPopupView>();
        _popupView.SetTitleText(_loc.GetLocalization(LocalizationKeys.PopupDailyMissionsTitle));

        UpdateItems();
        UpdateSecondsLeftText(-1);

        await _popupView.Appear2Async();

        Activate();
    }

    public async void Unmediate()
    {
        Deactivate();

        await _popupView.Disppear2Async();

        GameObject.Destroy(_popupView.gameObject);
        _popupView = null;
    }

    private void UpdateSecondsLeftText(int secondsLeft)
    {
        if (secondsLeft < 0)
        {
            _popupView.SetStatusText(string.Empty);
        }
        else
        {
            var timeLeftStr = FormattingHelper.ToSeparatedTimeFormat(secondsLeft);
            _popupView.SetStatusText(string.Format(_loc.GetLocalization(LocalizationKeys.PopupDailyMissionsStatusFormat), timeLeftStr));
        }
    }

    private void Activate()
    {
        _popupView.ButtonCloseClicked += OnButtonCloseClicked;
        _dispatcher.DailyMissionsSecondsLeftAmountUpdated += OnDailyMissionsSecondsLeftAmountUpdated;
        foreach (var missionModel in _viewModel.DailyMissionsModel.MissionsList)
        {
            missionModel.RewardTaken += OnMissionRewardTaken;
        }
    }

    private void Deactivate()
    {
        _popupView.ButtonCloseClicked -= OnButtonCloseClicked;
        _dispatcher.DailyMissionsSecondsLeftAmountUpdated -= OnDailyMissionsSecondsLeftAmountUpdated;
        _displayedItems.ForEach(DeactivateItem);
        foreach (var missionModel in _viewModel.DailyMissionsModel.MissionsList)
        {
            missionModel.RewardTaken -= OnMissionRewardTaken;
        }
    }

    private void OnDailyMissionsSecondsLeftAmountUpdated(int secondsLeft)
    {
        UpdateSecondsLeftText(secondsLeft);
    }

    private void OnButtonCloseClicked()
    {
        _dispatcher.UIRequestRemoveCurrentPopup();
    }

    private void UpdateItems()
    {
        var missionsCount = _viewModel.DailyMissionsModel.MissionsList.Count;
        for (var i = 0; i < missionsCount; i++)
        {
            if (_displayedItems.Count < i + 1)
            {
                var itemGo = GameObject.Instantiate(_prefabsHolder.UIDailyMissionsPopupItemPrefab, _popupView.ContentRectTransform);
                var view = itemGo.GetComponent<UIDailyMissionsPopupMissionItemView>();
                var rectTransform = (view.transform as RectTransform);
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -i * rectTransform.sizeDelta.y);
                _displayedItems.Add(view);
                ActivateView(view);
            }
            UpdateItem(_displayedItems[i]);
        }

        while (_displayedItems.Count > missionsCount)
        {
            var itemToRemove = _displayedItems[_displayedItems.Count - 1];
            RemoveItem(itemToRemove);
        }
    }

    private void OnMissionRewardTaken()
    {
        UpdateItems();
    }

    private void RemoveItem(UIDailyMissionsPopupMissionItemView itemToRemove)
    {
        DeactivateItem(itemToRemove);
        _displayedItems.Remove(itemToRemove);
        GameObject.Destroy(itemToRemove.gameObject);
    }

    private void ActivateView(UIDailyMissionsPopupMissionItemView itemView)
    {
        itemView.TakeRewardClicked += OnTakeRewardClicked;
    }

    private void DeactivateItem(UIDailyMissionsPopupMissionItemView itemView)
    {
        itemView.TakeRewardClicked -= OnTakeRewardClicked;
    }

    private void OnTakeRewardClicked(UIDailyMissionsPopupMissionItemView itemView)
    {
        var itemViewModel = GetViewModelByView(itemView);
        _dispatcher.UITakeDailyMissionRewardClicked(itemViewModel, itemView.RewardButtonTransform.position);
    }

    private DailyMissionModel GetViewModelByView(UIDailyMissionsPopupMissionItemView itemView)
    {
        var index = _displayedItems.IndexOf(itemView);
        if (index >= 0)
        {
            return _viewModel.DailyMissionsModel.MissionsList[index];
        }
        else
        {
            return null;
        }
    }

    private void UpdateItem(UIDailyMissionsPopupMissionItemView itemView)
    {
        var missionModel = GetViewModelByView(itemView);

        var missionIcon = GetMissionIcon(missionModel);
        itemView.SetMissionIconSprite(missionIcon);
        var missionDescription = GetMissionDescription(missionModel);
        itemView.SetMissionDescription(missionDescription);
        var rewardIconSprite = GetRewardIconSprite(missionModel.Reward);
        itemView.SetRewardIconSprite(rewardIconSprite);
        var rewardTextColor = GetRewardTextColor(missionModel.Reward);
        itemView.SetRewardTextColor(rewardTextColor);
        itemView.SetRewardAmount(missionModel.Reward.Amount);

        UpdateItemProgressView(itemView, missionModel);
    }

    private void UpdateItemProgressView(UIDailyMissionsPopupMissionItemView itemView, DailyMissionModel missionModel)
    {
        itemView.SetProgressVisibility(missionModel.IsCompleted == false);
        if (missionModel.IsCompleted == false)
        {
            itemView.SetProgressText(missionModel.Value, missionModel.TargetValue);
            itemView.SetProgress(missionModel.Progress);
        }

        itemView.SetTakeButtonVisibility(missionModel.IsCompleted);
        itemView.SetTakeButtonInteractable(missionModel.IsCompleted && missionModel.IsRewardTaken == false);
        var takeButtonText = _loc.GetLocalization(missionModel.IsRewardTaken ? LocalizationKeys.PopupDailyMissionsMissionCompleted : LocalizationKeys.PopupDailyMissionsRetreiveReward);
        itemView.SetTakeButtonText(takeButtonText);
    }

    private Sprite GetRewardIconSprite(Reward reward)
    {
        switch (reward.Type)
        {
            case RewardType.Cash:
                return _spritesProvider.GetCashIcon();
            case RewardType.Gold:
                return _spritesProvider.GetGoldIcon();
            case RewardType.Exp:
                return _spritesProvider.GetStarIcon(isBig: true);
        }

        return null;
    }
    
    private Color GetRewardTextColor(Reward reward)
    {
        var result = Color.white;
        
        switch (reward.Type)
        {
            case RewardType.Cash:
                 ColorUtility.TryParseHtmlString(Constants.CashAmountTextGreenColor, out result);
                 break;
            case RewardType.Gold:
                ColorUtility.TryParseHtmlString(Constants.GoldAmountTextRedColor, out result);
                break;
        }

        return result;
    }

    private string GetMissionDescription(DailyMissionModel missionModel)
    {
        var result = _loc.GetLocalization($"{LocalizationKeys.MissionDescriptionFormat}{missionModel.Key}");

        switch (missionModel.Key)
        {
            case MissionKeys.AddShelfs:
                var shelfId = (missionModel as DailyMissionAddShelfsModel).ShelfNumericId;
                var shelfName = _loc.GetLocalization($"{LocalizationKeys.NameShopObjectPrefix}s_{shelfId}");
                result = string.Format(result, missionModel.TargetValue - missionModel.StartValue, shelfName);
                break;
            case MissionKeys.SellProduct:
                var productId = (missionModel as DailyMissionSellProductModel).ProductConfig.NumericId;
                var productName = _loc.GetLocalization($"{LocalizationKeys.NameProductIdPrefix}{productId}");
                result = string.Format(result, missionModel.TargetValue, productName);
                break;
            case MissionKeys.AddFriends:
                result = string.Format(result, missionModel.TargetValue - missionModel.StartValue);
                break;
            default:
                result = string.Format(result, missionModel.TargetValue);
                break;
        }

        return result;
    }

    private Sprite GetMissionIcon(DailyMissionModel missionModel)
    {
        switch (missionModel.Key)
        {
            case MissionKeys.AddCash:
                return _spritesProvider.GetCashIcon();
            case MissionKeys.AddFriends:
            case MissionKeys.VisitFriends:
                return _spritesProvider.GetFriendsIcon();
            case MissionKeys.AddGold:
                return _spritesProvider.GetGoldIcon();
            case MissionKeys.AddShelfs:
                var shelfId = (missionModel as DailyMissionAddShelfsModel).ShelfNumericId;
                return _spritesProvider.GetShelfIcon(shelfId);
            case MissionKeys.AddWarehouseCells:
                return _spritesProvider.GetUpgradeIcon(UpgradeType.WarehouseSlots);
            case MissionKeys.UpgradeWarehouseVolume:
                return _spritesProvider.GetUpgradeIcon(UpgradeType.WarehouseVolume);
            case MissionKeys.CleanUnwashes:
                return _spritesProvider.GetUnwashIcon(1);
            case MissionKeys.ExpandShop:
                return _spritesProvider.GetUpgradeIcon(UpgradeType.ExpandX);
            case MissionKeys.GiftToFriend:
                return _spritesProvider.GetGiftboxBlueIcon();
            case MissionKeys.RepaintFloors:
                return _spritesProvider.GetFloorIcon(5);
            case MissionKeys.RepaintWalls:
                return _spritesProvider.GetWallSprite(4);
            case MissionKeys.SellProduct:
                var productKey = (missionModel as DailyMissionSellProductModel).ProductConfig.Key;
                return _spritesProvider.GetProductIcon(productKey);
        }

        return _spritesProvider.GetMissionsIcon();
    }
}
