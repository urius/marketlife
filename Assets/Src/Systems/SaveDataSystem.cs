using System;
using System.Threading.Tasks;
using UnityEngine;

public class SaveDataSystem
{
    private const int DefaultSaveCooldownSeconds = 5;

    private readonly GameStateModel _gameStateModel;
    private readonly Dispatcher _dispatcher;
    private readonly UpdatesProvider _updatesProvider;

    private SaveField _saveFieldsData = SaveField.None;
    private ShopModel _shopModel;
    private bool _saveInProgress = false;
    private UserModel _playerModel;
    private int _saveCooldownSeconds = 0;

    public SaveDataSystem()
    {
        _gameStateModel = GameStateModel.Instance;
        _dispatcher = Dispatcher.Instance;
        _updatesProvider = UpdatesProvider.Instance;
    }

    public bool NeedToSave => _saveFieldsData != SaveField.None;

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
        if (_saveCooldownSeconds > 0)
        {
            UpdateSaveCooldownIfNeeded();
        }
    }

    private void Activate()
    {
        var progressModel = _playerModel.ProgressModel;
        var designModel = _shopModel.ShopDesign;
        var warehouseModel = _shopModel.WarehouseModel;
        var personalModel = _shopModel.PersonalModel;

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

        progressModel.CashChanged += OnCashChanged;
        progressModel.GoldChanged += OnGoldChanged;
        progressModel.ExpChanged += OnExpChanged;
        progressModel.LevelChanged += OnLevelChanged;
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
        _gameStateModel.PlacingStateChanged += OnPlacingStateChanged;
        _gameStateModel.PopupRemoved += OnPopupRemoved;

        _updatesProvider.RealtimeSecondUpdate += OnRealtimeSecondUpdate;
    }

    private void OnTutorialStepPassed(int stepIndex)
    {
        MarkToSaveField(SaveField.TutorialSteps);
    }

    private async void OnRealtimeSecondUpdate()
    {
        if (NeedToSave && _saveCooldownSeconds > 0)
        {
            if (_saveInProgress == false)
            {
                _saveInProgress = true;
                _dispatcher.SaveStateChanged(_saveInProgress);
            }

            _saveCooldownSeconds--;
            if (_saveCooldownSeconds <= 0)
            {
                _saveCooldownSeconds = 0;
                await SaveAsync();
            }
        }
    }

    private void OnUnwashRemoved(Vector2Int coords)
    {
        MarkToSaveField(SaveField.Unwashes);
    }

    private void OnUnwashAdded(Vector2Int coords)
    {
        MarkToSaveField(SaveField.Unwashes);
    }

    private void OnPopupRemoved()
    {
        if (CheckSaveUserDataconditions())
        {
            UpdateSaveCooldownIfNeeded();
        }
    }

    private void OnPlacingStateChanged(PlacingStateName previousState, PlacingStateName currentState)
    {
        if (CheckSaveUserDataconditions())
        {
            UpdateSaveCooldownIfNeeded();
        }
    }

    private void OnGameStateChanged(GameStateName previousState, GameStateName currentState)
    {
        if (previousState != GameStateName.ReadyForStart && CheckSaveUserDataconditions())
        {
            UpdateSaveCooldownIfNeeded();
        }
    }

    private bool CheckSaveUserDataconditions()
    {
        return (_gameStateModel.GameState == GameStateName.ShopInterior || _gameStateModel.GameState == GameStateName.ShopSimulation)
             && _gameStateModel.ShowingPopupModel == null
             && _gameStateModel.PlacingState == PlacingStateName.None;
    }

    private void UpdateSaveCooldownIfNeeded()
    {
        if (NeedToSave)
        {
            _saveCooldownSeconds = DefaultSaveCooldownSeconds;
        }
    }

    private async Task SaveAsync()
    {
        var saveFieldsData = _saveFieldsData;
        _saveFieldsData = SaveField.None;
        await new SaveDataCommand().Execute(saveFieldsData);

        _saveInProgress = false;
        _dispatcher.SaveStateChanged(_saveInProgress);
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
    All = Progress | Personal | Warehouse | Design | ShopObjects | Unwashes | TutorialSteps,
}
