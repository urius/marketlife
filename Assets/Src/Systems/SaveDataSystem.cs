using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SaveDataSystem
{
    private const int DefaultSavePrewarmSeconds = 2;
    private const int DefaultSaveCooldownSeconds = 6;

    private readonly GameStateModel _gameStateModel = GameStateModel.Instance;
    private readonly Dispatcher _dispatcher = Dispatcher.Instance;
    private readonly UpdatesProvider _updatesProvider = UpdatesProvider.Instance;

    private bool _saveProcessIsTriggered = false;
    private SaveField _saveFieldsData = SaveField.None;
    private bool _isSaveRequestSending = false;
    private ShopModel _shopModel;
    private bool _saveInProgress = false;
    private UserModel _playerModel;
    private int _savePlayerDataPrewarmSeconds = DefaultSavePrewarmSeconds;
    private int _saveExternalDataCooldownSeconds = 0;
    private Action _unsubscribeFromFriendActionsDelegate;
    private Queue<UserModel> _saveExternalDataQueue = new Queue<UserModel>();

    public bool NeedToSavePlayerData => _saveFieldsData != SaveField.None;
    public bool NeedToSaveExternalData => _saveExternalDataQueue.Count > 0;
    public bool NeedToSave => NeedToSavePlayerData || NeedToSaveExternalData;

    public async void Start()
    {
        await _gameStateModel.GameDataLoadedTask;

        _playerModel = PlayerModelHolder.Instance.UserModel;
        _shopModel = _playerModel.ShopModel;

        Activate();
    }

    public void MarkToSaveField(SaveField field)
    {
        _saveFieldsData |= field;
        if (_savePlayerDataPrewarmSeconds > 0)
        {
            UpdateSaveCooldownIfNeeded();
        }
    }

    private void Activate()
    {
        var progressModel = _playerModel.ProgressModel;
        var actionsDataModel = _playerModel.FriendsActionsDataModels;
        var designModel = _shopModel.ShopDesign;
        var warehouseModel = _shopModel.WarehouseModel;
        var personalModel = _shopModel.PersonalModel;
        var playerExternalActionsModel = _playerModel.ExternalActionsModel;
        var playerDailyMissionsModel = _playerModel.DailyMissionsModel;

        foreach (var kvp in _shopModel.ShopObjects)
        {
            if (kvp.Value.Type == ShopObjectType.Shelf)
            {
                SubscribeOnShelfModel(kvp.Value as ShelfModel);
            }
            else if (kvp.Value.Type == ShopObjectType.CashDesk)
            {
                SubscribeOnCashDeskModel(kvp.Value as CashDeskModel);
            }
        }
        _shopModel.ShopObjectsChanged += OnShopObjectsChanged;
        _shopModel.ShopObjectPlaced += OnShopObjectPlaced;
        _shopModel.ShopObjectRemoved += OnShopObjectRemoved;
        _shopModel.UnwashAdded += OnUnwashAdded;
        _shopModel.UnwashRemoved += OnUnwashRemoved;
        _shopModel.BillboardModel.AvailabilityChanged += OnBillboardStateChanged;
        _shopModel.BillboardModel.TextChanged += OnBillboardStateChanged;

        _playerModel.TutorialStepPassed += OnTutorialStepPassed;
        _playerModel.BonusStateUpdated += OnBonusStateUpdated;
        _playerModel.SettingsUpdated += OnPlayerSettingsUpdated;
        playerExternalActionsModel.ActionsCleared += OnPlayerExternalActionsCleared;

        progressModel.CashChanged += OnCashChanged;
        progressModel.GoldChanged += OnGoldChanged;
        progressModel.ExpChanged += OnExpChanged;
        progressModel.LevelChanged += OnLevelChanged;
        actionsDataModel.ActionDataAmountChanged += OnActionDataAmountChanged;
        actionsDataModel.ActionDataCooldownTimestampChanged += OnActionDataCooldownTimestampChanged;
        designModel.FloorChanged += OnFloorChanged;
        designModel.WallChanged += OnWallChanged;
        designModel.WindowChanged += OnWindowChanged;
        designModel.DoorChanged += OnDoorChanged;
        designModel.SizeXChanged += OnSizeChanged;
        designModel.SizeYChanged += OnSizeChanged;
        warehouseModel.SlotsAdded += OnWarehouseSlotsAdded;
        warehouseModel.VolumeAdded += OnWarehouseVolumeAdded;
        warehouseModel.SlotProductIsSet += OnWarehouseSlotProductSet;
        warehouseModel.SlotProductAmountChanged += OnWarehouseSlotProductAmountChanged;
        warehouseModel.SlotProductRemoved += OnWarehouseSlotProductRemoved;
        personalModel.PersonalWorkingTimeUpdated += OnPersonalWorkingTimeUpdated;
        playerDailyMissionsModel.MissionAdded += OnMissionAdded;
        playerDailyMissionsModel.MissionRemoved += OnMissionRemoved;
        foreach (var mission in playerDailyMissionsModel.MissionsList)
        {
            SubscribeOnMission(mission);
        }

        _gameStateModel.GameStateChanged += OnGameStateChanged;
        _gameStateModel.ActionStateChanged += OnActionStateChanged;
        _gameStateModel.PopupRemoved += OnPopupRemoved;
        _gameStateModel.ViewingUserModelChanged += OnViewingUserModelChanged;

        _dispatcher.RequestMarkToSaveField += OnRequestMarkToSaveField;
        _dispatcher.RequestTriggerSave += OnRequestTriggerSave;

        _updatesProvider.RealtimeSecondUpdate += OnRealtimeSecondUpdate;
    }

    private void OnRequestTriggerSave()
    {
        TriggerSave();
    }

    private void OnMissionAdded(DailyMissionModel mission)
    {
        SubscribeOnMission(mission);
        MarkToSaveField(SaveField.DailyMissions);
    }

    private void OnMissionRemoved(DailyMissionModel mission)
    {
        UnsubscribeFromMission(mission);
        MarkToSaveField(SaveField.DailyMissions);
    }

    private void SubscribeOnMission(DailyMissionModel mission)
    {
        UnsubscribeFromMission(mission);
        mission.RewardTaken += OnMissionStateChanged;
        mission.ValueChanged += OnMissionStateChanged;
    }

    private void UnsubscribeFromMission(DailyMissionModel mission)
    {
        mission.RewardTaken -= OnMissionStateChanged;
        mission.ValueChanged -= OnMissionStateChanged;
    }

    private void OnMissionStateChanged()
    {
        MarkToSaveField(SaveField.DailyMissions);
    }

    private void OnRequestMarkToSaveField(SaveField saveField)
    {
        MarkToSaveField(saveField);
    }

    private void OnBillboardStateChanged()
    {
        MarkToSaveField(SaveField.Billboard);
    }

    private void OnPlayerSettingsUpdated()
    {
        MarkToSaveField(SaveField.Settings);
        TriggerSave();
    }

    private void OnBonusStateUpdated()
    {
        MarkToSaveField(SaveField.Bonus);
        TriggerSave();
    }

    private void OnPlayerExternalActionsCleared()
    {
        _saveExternalDataQueue.Enqueue(_playerModel);
    }

    private void OnViewingUserModelChanged(UserModel userModel)
    {
        _unsubscribeFromFriendActionsDelegate?.Invoke();
        _unsubscribeFromFriendActionsDelegate = null;

        if (userModel.Uid != _playerModel.Uid)
        {
            userModel.ExternalActionsModel.ActionAdded += OnFriendExternalActionAdded;
            _unsubscribeFromFriendActionsDelegate = () => userModel.ExternalActionsModel.ActionAdded -= OnFriendExternalActionAdded;
        }
    }

    private void OnFriendExternalActionAdded(ExternalActionModelBase actionModel)
    {
        if (_saveExternalDataQueue.Count <= 0
            || _saveExternalDataQueue.Peek().Uid != _gameStateModel.ViewingUserModel.Uid)
        {
            _saveExternalDataQueue.Enqueue(_gameStateModel.ViewingUserModel);
        }
    }

    private void OnActionDataAmountChanged(string friendId, FriendShopActionData actionData)
    {
        MarkToSaveField(SaveField.FriendsActionsData);
    }

    private void OnActionDataCooldownTimestampChanged(string friendId, FriendShopActionData actionData)
    {
        MarkToSaveField(SaveField.FriendsActionsData);
        TriggerSave();
    }

    private void OnTutorialStepPassed(int stepIndex)
    {
        MarkToSaveField(SaveField.TutorialSteps);
        TriggerSave();
    }

    private void TriggerSave()
    {
        if (CheckStartSaveConditions())
        {
            _saveProcessIsTriggered = NeedToSave;
            _savePlayerDataPrewarmSeconds = DefaultSavePrewarmSeconds;
        }
    }

    private async void OnRealtimeSecondUpdate()
    {
        SetSaveInProgress(_saveProcessIsTriggered);
        if (_saveExternalDataCooldownSeconds > 0)
        {
            _saveExternalDataCooldownSeconds--;
            return;
        }

        if (_isSaveRequestSending == false && _saveProcessIsTriggered == true)
        {
            if (_savePlayerDataPrewarmSeconds > 0)
            {
                _savePlayerDataPrewarmSeconds--;
            }

            if (_savePlayerDataPrewarmSeconds <= 0 && CheckSaveConditions())
            {
                if (NeedToSavePlayerData)
                {
                    _saveExternalDataCooldownSeconds = DefaultSaveCooldownSeconds;
                    _isSaveRequestSending = true;
                    await SaveAsync();
                    _isSaveRequestSending = false;
                }

                if (NeedToSaveExternalData)
                {
                    _saveExternalDataCooldownSeconds = DefaultSaveCooldownSeconds;
                    _isSaveRequestSending = true;
                    await SaveExternalDataAsync();
                    _isSaveRequestSending = false;
                }

                _saveProcessIsTriggered = false;
            }
        }
    }

    private void SetSaveInProgress(bool isSaveInProgress)
    {
        if (_saveInProgress != isSaveInProgress)
        {
            _saveInProgress = isSaveInProgress;
            _dispatcher.SaveStateChanged(_saveInProgress);
        }
    }

    private void OnUnwashRemoved(Vector2Int coords)
    {
        MarkToSaveField(SaveField.Unwashes);
        TriggerSave();
    }

    private void OnUnwashAdded(Vector2Int coords)
    {
        MarkToSaveField(SaveField.Unwashes);
    }

    private void OnPopupRemoved(PopupViewModelBase popupViewModel)
    {
        TriggerSave();
    }

    private void OnActionStateChanged(ActionStateName previousState, ActionStateName currentState)
    {
        TriggerSave();
    }

    private void OnGameStateChanged(GameStateName previousState, GameStateName currentState)
    {
        TriggerSave();
        if (_gameStateModel.IsPlayingState && _gameStateModel.CheckIsPlayingState(previousState) == false)
        {
            new SaveLeaderboardDataCommand().Execute();
        }
    }

    private bool CheckStartSaveConditions()
    {
        return _gameStateModel.IsPlayingState
             && _gameStateModel.ShowingPopupModel == null
             && _gameStateModel.ActionState == ActionStateName.None;
    }

    private bool CheckSaveConditions()
    {
        return _gameStateModel.IsPlayingState
             && _gameStateModel.ActionState == ActionStateName.None;
    }

    private void UpdateSaveCooldownIfNeeded()
    {
        if (NeedToSavePlayerData)
        {
            _savePlayerDataPrewarmSeconds = DefaultSavePrewarmSeconds;
        }
    }

    private async Task SaveAsync()
    {
        var saveFieldsData = _saveFieldsData;
        _saveFieldsData = SaveField.None;
        var result = await new SaveDataCommand().ExecuteAsync(saveFieldsData);
        _dispatcher.SaveCompleted(result, saveFieldsData);
    }

    private async Task SaveExternalDataAsync()
    {
        var saveUserModel = _saveExternalDataQueue.Dequeue();
        var result = await new SaveExternalDataCommand().ExecuteAsync(saveUserModel);
        _dispatcher.SaveExternalDataCompleted(result);
    }

    private void OnSizeChanged(int previousValue, int currentValue)
    {
        MarkToSaveField(SaveField.Design);
    }

    private void OnPersonalWorkingTimeUpdated(PersonalConfig personalConfig)
    {
        MarkToSaveField(SaveField.Personal);
    }

    private void OnShopObjectPlaced(ShopObjectModelBase shopObjectModel)
    {
        if (shopObjectModel.Type == ShopObjectType.Shelf)
        {
            SubscribeOnShelfModel(shopObjectModel as ShelfModel);
        }
    }

    private void OnShopObjectRemoved(ShopObjectModelBase shopObjectModel)
    {
        if (shopObjectModel.Type == ShopObjectType.Shelf)
        {
            UnsubscribeFromShelfModel(shopObjectModel as ShelfModel);
        }
        else if (shopObjectModel.Type == ShopObjectType.CashDesk)
        {
            UnsubscribeFromCashDeskModel(shopObjectModel as CashDeskModel);
        }
    }

    private void SubscribeOnShelfModel(ShelfModel shelfModel)
    {
        shelfModel.ProductIsSetOnSlot += OnShelfProductIsSetOnSlot;
        shelfModel.ProductAmountChangedOnSlot += OnShelfProductAmountChangedOnSlot;
        shelfModel.ProductRemovedFromSlot += OnShelfProductRemovedFromSlot;
    }

    private void UnsubscribeFromShelfModel(ShelfModel shelfModel)
    {
        shelfModel.ProductIsSetOnSlot -= OnShelfProductIsSetOnSlot;
        shelfModel.ProductAmountChangedOnSlot -= OnShelfProductAmountChangedOnSlot;
        shelfModel.ProductRemovedFromSlot -= OnShelfProductRemovedFromSlot;
    }

    private void OnShelfProductRemovedFromSlot(ShelfModel shelfModel, int slotIndex, ProductModel productModel)
    {
        MarkToSaveField(SaveField.ShopObjects);
    }

    private void OnShelfProductAmountChangedOnSlot(ShelfModel shelfModel, int slotIndex, int deltaAmount)
    {
        MarkToSaveField(SaveField.ShopObjects);
    }

    private void OnShelfProductIsSetOnSlot(ShelfModel shelfModel, int slotIndex)
    {
        MarkToSaveField(SaveField.ShopObjects);
    }

    private void SubscribeOnCashDeskModel(CashDeskModel cashDeskModel)
    {
        cashDeskModel.DisplayItemChanged += OnCashDeskDisplayItemChanged;
    }

    private void UnsubscribeFromCashDeskModel(CashDeskModel cashDeskModel)
    {
        cashDeskModel.DisplayItemChanged -= OnCashDeskDisplayItemChanged;
    }

    private void OnCashDeskDisplayItemChanged()
    {
        MarkToSaveField(SaveField.ShopObjects);
    }

    private void OnWarehouseSlotProductSet(int slotIndex)
    {
        MarkToSaveField(SaveField.Warehouse);
    }

    private void OnWarehouseSlotProductAmountChanged(int slotIndex, int deltaAmount)
    {
        MarkToSaveField(SaveField.Warehouse);
    }

    private void OnWarehouseSlotProductRemoved(int slotIndex, ProductModel productModel)
    {
        MarkToSaveField(SaveField.Warehouse);
    }

    private void OnWarehouseSlotsAdded(int amount)
    {
        MarkToSaveField(SaveField.Warehouse);
    }

    private void OnWarehouseVolumeAdded(int amount)
    {
        MarkToSaveField(SaveField.Warehouse);
    }

    private void OnFloorChanged(Vector2Int coords, int id)
    {
        MarkToSaveField(SaveField.Design);
    }

    private void OnWallChanged(Vector2Int coords, int id)
    {
        MarkToSaveField(SaveField.Design);
    }

    private void OnWindowChanged(Vector2Int coords, int id)
    {
        MarkToSaveField(SaveField.Design);
    }

    private void OnDoorChanged(Vector2Int coords, int id)
    {
        MarkToSaveField(SaveField.Design);
    }

    private void OnLevelChanged(int delta)
    {
        MarkToSaveField(SaveField.Progress);
    }

    private void OnExpChanged(int delta)
    {
        MarkToSaveField(SaveField.Progress);
    }

    private void OnGoldChanged(int previousValue, int currentValue)
    {
        MarkToSaveField(SaveField.Progress);
        TriggerSave();
    }

    private void OnCashChanged(int previousValue, int currentValue)
    {
        MarkToSaveField(SaveField.Progress);
    }

    private void OnShopObjectsChanged()
    {
        MarkToSaveField(SaveField.ShopObjects);
    }
}

[Flags]
public enum SaveField
{
    None = 0,
    Progress = 1 << 0,
    Personal = 1 << 1,
    Warehouse = 1 << 2,
    Design = 1 << 3,
    ShopObjects = 1 << 4,
    Unwashes = 1 << 5,
    TutorialSteps = 1 << 6,
    FriendsActionsData = 1 << 7,
    Bonus = 1 << 8,
    Settings = 1 << 9,
    Billboard = 1 << 10,
    DailyMissions = 1 << 11,
    All = Progress | Personal | Warehouse | Design | ShopObjects | Unwashes | TutorialSteps | FriendsActionsData | Bonus | Settings | Billboard | DailyMissions,
}
