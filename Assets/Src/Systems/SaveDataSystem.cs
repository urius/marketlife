using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SaveDataSystem
{
    private const int DefaultSavePrewarmSeconds = 2;
    private const int DefaultSaveCooldownSeconds = 6;

    private readonly GameStateModel _gameStateModel;
    private readonly Dispatcher _dispatcher;
    private readonly UpdatesProvider _updatesProvider;

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

    public SaveDataSystem()
    {
        _gameStateModel = GameStateModel.Instance;
        _dispatcher = Dispatcher.Instance;
        _updatesProvider = UpdatesProvider.Instance;
    }

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
        var actionsDataModel = _playerModel.ActionsDataModel;
        var designModel = _shopModel.ShopDesign;
        var warehouseModel = _shopModel.WarehouseModel;
        var personalModel = _shopModel.PersonalModel;
        var playerExternalActionsModel = _playerModel.ExternalActionsModel;

        foreach (var kvp in _shopModel.ShopObjects)
        {
            if (kvp.Value.Type == ShopObjectType.Shelf)
            {
                SubscribeOnShelfModel(kvp.Value as ShelfModel);
            }
        }
        _shopModel.ShopObjectsChanged += OnShopObjectsChanged;
        _shopModel.ShopObjectPlaced += OnShopObjectPlaced;
        _shopModel.ShopObjectRemoved += OnShopObjectRemoved;
        _shopModel.UnwashAdded += OnUnwashAdded;
        _shopModel.UnwashRemoved += OnUnwashRemoved;

        _playerModel.TutorialStepPassed += OnTutorialStepPassed;
        _playerModel.BonusStateUpdated += OnBonusStateUpdated;
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

        _gameStateModel.GameStateChanged += OnGameStateChanged;
        _gameStateModel.ActionStateChanged += OnActionStateChanged;
        _gameStateModel.PopupRemoved += OnPopupRemoved;
        _gameStateModel.ViewingUserModelChanged += OnViewingUserModelChanged;

        _updatesProvider.RealtimeSecondUpdate += OnRealtimeSecondUpdate;
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

    private void OnActionDataAmountChanged(AvailableFriendShopActionData actionData)
    {
        MarkToSaveField(SaveField.AvailableActionsData);
    }

    private void OnActionDataCooldownTimestampChanged(AvailableFriendShopActionData actionData)
    {
        MarkToSaveField(SaveField.AvailableActionsData);
        TriggerSave();
    }

    private void OnTutorialStepPassed(int stepIndex)
    {
        MarkToSaveField(SaveField.TutorialSteps);
        TriggerSave();
    }

    private void TriggerSave()
    {
        _saveProcessIsTriggered = NeedToSave;
        _savePlayerDataPrewarmSeconds = DefaultSavePrewarmSeconds;
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

    private void OnPopupRemoved()
    {
        if (CheckStartSaveConditions())
        {
            TriggerSave();
        }
    }

    private void OnActionStateChanged(ActionStateName previousState, ActionStateName currentState)
    {
        if (CheckStartSaveConditions())
        {
            TriggerSave();
        }
    }

    private void OnGameStateChanged(GameStateName previousState, GameStateName currentState)
    {
        if (CheckStartSaveConditions())
        {
            TriggerSave();
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
        await new SaveDataCommand().ExecuteAsync(saveFieldsData);
    }

    private async Task SaveExternalDataAsync()
    {
        var saveUserModel = _saveExternalDataQueue.Dequeue();
        await new SaveExternalDataCommand().ExecuteAsync(saveUserModel);
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
        if (CheckStartSaveConditions())
        {
            TriggerSave();
        }
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
    AvailableActionsData = 1 << 7,
    Bonus = 1 << 8,
    All = Progress | Personal | Warehouse | Design | ShopObjects | Unwashes | TutorialSteps | AvailableActionsData | Bonus,
}
