using System;
using UnityEngine;

public class EventsHandleSystem
{
    private readonly Dispatcher _dispatcher;
    private readonly GameStateModel _gameStateModel;
    private UserModel _playerModel;

    public EventsHandleSystem()
    {
        _dispatcher = Dispatcher.Instance;
        _gameStateModel = GameStateModel.Instance;
    }

    public async void Start()
    {
        Activate();
        await _gameStateModel.GameDataLoadedTask;
        ActivateAfterLoad();
    }

    private void Activate()
    {
        _dispatcher.UIGameViewMouseClick += OnUIGameViewMouseClicked;
        _dispatcher.UIBottomPanelPointerEnter += OnUIBottomPanelPointerEnter;
        _dispatcher.UIBottomPanelPointerExit += OnUIBottomPanelPointerExit;
        _dispatcher.UIBottomPanelPlaceShelfClicked += OnUIBottomPanelPlaceShelfClicked;
        _dispatcher.UIBottomPanelPlaceFloorClicked += OnUIBottomPanelPlaceFloorClicked;
        _dispatcher.UIBottomPanelPlaceWallClicked += OnUIBottomPanelPlaceWallClicked;
        _dispatcher.UIBottomPanelPlaceWindowClicked += OnUIBottomPanelPlaceWindowClicked;
        _dispatcher.UIBottomPanelPlaceDoorClicked += OnUIBottomPanelPlaceDoorClicked;
        _dispatcher.UIBottomPanelWarehouseSlotClicked += OnUIBottomPanelWarehouseSlotClicked;
        _dispatcher.UIBottomPanelExpandWarehouseClicked += OnUIBottomPanelExpandWarehouseClicked;
        _dispatcher.UIBottomPanelWarehouseQuickDeliverClicked += OnUIBottomPanelWarehouseQuickDeliverClicked;
        _dispatcher.UIBottomPanelWarehouseRemoveProductClicked += OnUIBottomPanelWarehouseRemoveProductClicked;
        _dispatcher.UIBottomPanelAutoPlaceClicked += OnUIBottomPanelAutoPlaceClicked;
        _dispatcher.UIActionsRotateRightClicked += OnUIActionsRotateRightClicked;
        _dispatcher.UIActionsRotateLeftClicked += OnUIActionsRotateLeftClicked;
        _dispatcher.UIActionsMoveClicked += OnUIActionsMoveClicked;
        _dispatcher.UIActionsRemoveClicked += OnUIActionsRemoveClicked;
        _dispatcher.UIConfirmPopupResult += OnUIConfirmPopupResult;
        _dispatcher.UIRequestRemoveCurrentPopup += OnUIRequestRemoveCurrentPopup;
        _dispatcher.UIOrderProductClicked += OnUIOrderProductClicked;
        _dispatcher.UIShelfContentAddProductClicked += OnUIShelfContentAddProductClicked;
        _dispatcher.UIShelfContentRemoveProductClicked += OnUIShelfContentRemoveProductClicked;
        _dispatcher.UIWarehousePopupSlotClicked += OnUIWarehousePopupSlotClicked;
        _dispatcher.UIUpgradePopupBuyClicked += OnUIUpgradePopupBuyClicked;
        _dispatcher.MouseCellCoordsUpdated += OnMouseCellCoordsUpdated;
        _dispatcher.BottomPanelFriendsClicked += OnBottomPanelFriendsClicked;
        _dispatcher.BottomPanelWarehouseClicked += OnBottomPanelWarehouseClicked;
        _dispatcher.BottomPanelInteriorClicked += BottomPanelInteriorClicked;
        _dispatcher.BottomPanelManageButtonClicked += BottomPanelManageClicked;
        _dispatcher.BottomPanelBackClicked += OnBottomPanelBackClicked;
        _dispatcher.BottomPanelInteriorShelfsClicked += OnBottomPanelInteriorShelfsClicked;
        _dispatcher.BottomPanelInteriorFloorsClicked += OnBottomPanelInteriorFloorsClicked;
        _dispatcher.BottomPanelInteriorWallsClicked += OnBottomPanelInteriorWallsClicked;
        _dispatcher.BottomPanelInteriorWindowsClicked += OnBottomPanelInteriorWindowsClicked;
        _dispatcher.BottomPanelInteriorDoorsClicked += OnBottomPanelInteriorDoorsClicked;
        _dispatcher.BottomPanlelFinishPlacingClicked += BottomPanelFinishPlacingClicked;
        _dispatcher.BottomPanelRotateRightClicked += BottomPanelRotateRightClicked;
        _dispatcher.BottomPanelRotateLeftClicked += BottomPanelRotateLeftClicked;
        _dispatcher.UIBottomPanelFriendClicked += OnUIBottomPanelFriendClicked;
        _dispatcher.UIBottomPanelFriendShopActionClicked += OnUIBottomPanelFriendShopActionClicked;
        _dispatcher.UIBottomPanelBuyFriendShopActionClicked += OnUIBottomPanelBuyFriendShopActionClicked;
        _dispatcher.UITopPanelAddMoneyClicked += OnUITopPanelAddMoneyClicked;
        _dispatcher.UIGetBonusButtonClicked += OnUIGetBonusButtonClicked;
        _dispatcher.UICompensationPopupTakeClicked += OnUICompensationPopupTakeClicked;
        _dispatcher.UIDailyBonusTakeClicked += OnUIDailyBonusTakeClicked;
        _dispatcher.UIMuteAudioClicked += OnUIMuteAudioClicked;
        _dispatcher.UIMuteMusicClicked += OnUIMuteMusicClicked;
        _dispatcher.UIBankItemClicked += OnUIBankItemClicked;
        _dispatcher.UIDispatchBillboardClick += OnUIDispatchBillboardClick;
        _dispatcher.UIBillboardPopupApplyTextClicked += OnUIBillboardPopupApplyTextClicked;

        _gameStateModel.ActionStateChanged += OnActionStateChanged;
        _gameStateModel.GameStateChanged += OnGameStateChanged;
    }

    private void OnUIBillboardPopupApplyTextClicked(string text)
    {
        new HandleBillboardPopupApplyTextCommand().Execute(text);
    }

    private void OnUIDispatchBillboardClick()
    {
        new HandleBillboardClickCommand().Execute();
    }

    private void OnUIBankItemClicked(BankConfigItem itemConfig)
    {
        AnalyticsManager.Instance.SendStoreItemClick(itemConfig.IsGold, itemConfig.Id);
    }

    private void OnUIMuteAudioClicked()
    {
        new ToggleMuteAudioCommand().Execute();
    }

    private void OnUIMuteMusicClicked()
    {
        new ToggleMuteMusicCommand().Execute();
    }

    private void OnUIDailyBonusTakeClicked(Vector3[] itemsWorldPositions)
    {
        new HandleTakeDailyBonusCommand().Execute(itemsWorldPositions);
    }

    private void OnUICompensationPopupTakeClicked(Vector3 takeButtonWorldCoords)
    {
        new HandleTakeOldGameCompensationCommand().Execute(takeButtonWorldCoords);
    }

    private void OnUIGetBonusButtonClicked()
    {
        new ProcessBonusClickCommand().Execute();
    }

    private void OnUIBottomPanelFriendClicked(FriendData friendData)
    {
        new HandleBottomPanelFriendClickedCommand().Execute(friendData);
    }

    private void OnUITopPanelAddMoneyClicked(bool isGold)
    {
        new HandleAddMoneyClickCommand().Execute(isGold);
    }

    private void OnUIBottomPanelFriendShopActionClicked(FriendShopActionId actionId)
    {
        new HandleFriendShopActionClickCommand().Execute(actionId, isBuyClicked: false);
    }

    private void OnUIBottomPanelBuyFriendShopActionClicked(FriendShopActionId actionId)
    {
        new HandleFriendShopActionClickCommand().Execute(actionId, isBuyClicked: true);
    }

    private void ActivateAfterLoad()
    {
        _playerModel = PlayerModelHolder.Instance.UserModel;
        _playerModel.ProgressModel.LevelChanged += OnLevelChanged;
    }

    private void OnLevelChanged(int delta)
    {
        new ProcessLevelUpCommand().Execute();
    }

    private void OnUIBottomPanelAutoPlaceClicked()
    {
        new AutoPlaceCommand().Execute();
    }

    private void OnGameStateChanged(GameStateName previousState, GameStateName currentState)
    {
        if (currentState == GameStateName.ReadyForStart)
        {
            new CalculateForReportCommand().Execute();
        }
    }

    private void OnUIUpgradePopupBuyClicked(UpgradesPopupItemViewModelBase itemViewModel)
    {
        new UIUpgradePopupBuyClickCommand().Execute(itemViewModel);
    }

    private void OnUIWarehousePopupSlotClicked(WarehousePopupViewModel popupModel, int warehouseSlotIndex)
    {
        new UIWarehousePopupSlotClickedCommand().Execute(popupModel, warehouseSlotIndex);
    }

    private void OnUIShelfContentAddProductClicked(ShelfContentPopupViewModel popupModel, int shelfSlotIndex)
    {
        new UIShelfContentAddProductClicked().Execute(popupModel, shelfSlotIndex);
    }

    private void OnUIShelfContentRemoveProductClicked(ShelfContentPopupViewModel popupModel, int shelfSlotIndex)
    {
        new RemoveProductFromShelfCommand().Execute(popupModel, shelfSlotIndex);
    }

    private void OnUIBottomPanelWarehouseRemoveProductClicked(int slotIndex)
    {
        new RequestRemovePopupCommand().Execute(slotIndex);
    }

    private void OnUIBottomPanelWarehouseQuickDeliverClicked(int slotIndex)
    {
        new QuickDeliverCommand().Execute(slotIndex);
    }

    private void OnUIOrderProductClicked(RectTransform transform, Vector2 startAnimationScreenPoint, ProductConfig productConfig)
    {
        new OrderProductCommand().Execute(transform, startAnimationScreenPoint, productConfig);
    }

    private void OnUIRequestRemoveCurrentPopup()
    {
        new CloseCurrentPopupCommand().Execute();
    }

    private void OnUIBottomPanelPointerEnter()
    {
        _gameStateModel.ResetHighlightedState();
    }

    private void OnUIBottomPanelPointerExit()
    {
        _dispatcher.RequestForceMouseCellPositionUpdate();
    }

    private void OnUIConfirmPopupResult(bool result)
    {
        new HandleConfirmPopupResultCommand().Execute(result);
    }

    private void OnUIActionsRemoveClicked()
    {
        new RequestRemovePopupCommand().Execute();
    }

    private void OnUIActionsMoveClicked()
    {
        new StartMovingHighlightedObjectCommand().Execute();
    }

    private void OnUIActionsRotateRightClicked()
    {
        new RotateHighlightedObjectCommand().Execute(1);
    }

    private void OnUIActionsRotateLeftClicked()
    {
        new RotateHighlightedObjectCommand().Execute(-1);
    }

    private void OnUIGameViewMouseClicked()
    {
        if (_gameStateModel.ActionState != ActionStateName.None)
        {
            new PerformActionCommand().Execute();
        }
        else if (_gameStateModel.HighlightState.IsHighlighted == true)
        {
            new ProcessHighlightedObjectClickCommand().Execute();
        }
    }

    private void BottomPanelRotateLeftClicked()
    {
        new RotatePlacingObjectCommand().Execute(false);
    }

    private void BottomPanelRotateRightClicked()
    {
        new RotatePlacingObjectCommand().Execute(true);
    }

    private void BottomPanelFinishPlacingClicked()
    {
        _gameStateModel.ResetActionState();
    }

    private void OnBottomPanelFriendsClicked()
    {
        _gameStateModel.BottomPanelViewModel.SetSimulationTab(BottomPanelSimulationModeTab.Friends);
    }

    private void OnBottomPanelWarehouseClicked()
    {
        _gameStateModel.BottomPanelViewModel.SetSimulationTab(BottomPanelSimulationModeTab.Warehouse);
    }

    private void BottomPanelInteriorClicked()
    {
        _gameStateModel.SetGameState(GameStateName.ShopInterior);
    }

    private void BottomPanelManageClicked()
    {
        new UIShowManagePopupCommand().Execute();
    }

    private void OnBottomPanelBackClicked()
    {
        new BottomPanelHandleBackClickCommand().Execute();
    }

    private void OnBottomPanelInteriorShelfsClicked()
    {
        _gameStateModel.BottomPanelViewModel.SetInteriorTab(BottomPanelInteriorModeTab.Shelfs);
    }

    private void OnBottomPanelInteriorFloorsClicked()
    {
        _gameStateModel.BottomPanelViewModel.SetInteriorTab(BottomPanelInteriorModeTab.Floors);
    }

    private void OnBottomPanelInteriorWallsClicked()
    {
        _gameStateModel.BottomPanelViewModel.SetInteriorTab(BottomPanelInteriorModeTab.Walls);
    }

    private void OnBottomPanelInteriorWindowsClicked()
    {
        _gameStateModel.BottomPanelViewModel.SetInteriorTab(BottomPanelInteriorModeTab.Windows);
    }

    private void OnBottomPanelInteriorDoorsClicked()
    {
        _gameStateModel.BottomPanelViewModel.SetInteriorTab(BottomPanelInteriorModeTab.Doors);
    }

    private void OnMouseCellCoordsUpdated(Vector2Int newCoords)
    {
        new ProcessHighlightCommand().Execute(newCoords);

        if (_gameStateModel.PlacingShopObjectModel != null)
        {
            _gameStateModel.PlacingShopObjectModel.Coords = newCoords;
        }
    }

    private void OnActionStateChanged(ActionStateName previous, ActionStateName currentState)
    {
        if (currentState == ActionStateName.None)
        {
            var mouseCoords = MouseDataProvider.Instance.MouseCellCoords;
            new ProcessHighlightCommand().Execute(mouseCoords);
        }
        else
        {
            _gameStateModel.ResetHighlightedState();
        }
    }

    private void OnUIBottomPanelPlaceShelfClicked(int shelfNumericId)
    {
        new UIRequestPlaceShelfCommand().Execute(shelfNumericId);
    }

    private void OnUIBottomPanelPlaceFloorClicked(int floorNumericId)
    {
        new UIRequestPlacingDecorationCommand().Execute(ShopDecorationObjectType.Floor, floorNumericId);
    }

    private void OnUIBottomPanelPlaceWallClicked(int wallNumericId)
    {
        new UIRequestPlacingDecorationCommand().Execute(ShopDecorationObjectType.Wall, wallNumericId);
    }

    private void OnUIBottomPanelPlaceWindowClicked(int windowNumericId)
    {
        new UIRequestPlacingDecorationCommand().Execute(ShopDecorationObjectType.Window, windowNumericId);
    }

    private void OnUIBottomPanelPlaceDoorClicked(int doorNumericId)
    {
        new UIRequestPlacingDecorationCommand().Execute(ShopDecorationObjectType.Door, doorNumericId);
    }

    private void OnUIBottomPanelWarehouseSlotClicked(int slotIndex)
    {
        new UIProcessWarehouseSlotClickCommand().Execute(slotIndex);
    }

    private void OnUIBottomPanelExpandWarehouseClicked()
    {
        new HandleExpandWarehouseClickCommand().Execute();
    }
}
