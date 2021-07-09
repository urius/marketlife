using System;
using UnityEngine;

public class EventsHandleSystem
{
    private readonly Dispatcher _dispatcher;
    private readonly GameStateModel _gameStateModel;

    public EventsHandleSystem()
    {
        _dispatcher = Dispatcher.Instance;
        _gameStateModel = GameStateModel.Instance;
    }

    public void Start()
    {
        Activate();
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
        _dispatcher.UIBottomPanelWarehouseQuickDeliverClicked += OnUIBottomPanelWarehouseQuickDeliverClicked;
        _dispatcher.UIBottomPanelWarehouseRemoveProductClicked += OnUIBottomPanelWarehouseRemoveProductClicked;
        _dispatcher.UIBottomPanelAutoPlaceClicked += OnUIBottomPanelAutoPlaceClicked;
        _dispatcher.UIActionsRotateRightClicked += OnUIActionsRotateRightClicked;
        _dispatcher.UIActionsRotateLeftClicked += OnUIActionsRotateLeftClicked;
        _dispatcher.UIActionsMoveClicked += OnUIActionsMoveClicked;
        _dispatcher.UIActionsRemoveClicked += OnUIActionsRemoveClicked;
        _dispatcher.UIRemovePopupResult += OnUIRemovePopupResult;
        _dispatcher.UIRequestRemoveCurrentPopup += OnUIRequestRemoveCurrentPopup;
        _dispatcher.UIOrderProductClicked += OnUIOrderProductClicked;
        _dispatcher.UIShelfContentAddProductClicked += OnUIShelfContentAddProductClicked;
        _dispatcher.UIShelfContentRemoveProductClicked += OnUIShelfContentRemoveProductClicked;
        _dispatcher.UIWarehousePopupSlotClicked += OnUIWarehousePopupSlotClicked;
        _dispatcher.UIUpgradePopupBuyClicked += OnUIUpgradePopupBuyClicked;
        _dispatcher.UITopPanelLevelUpAnimationFinished += OnUITopPanelLevelUpAnimationFinished;
        _dispatcher.MouseCellCoordsUpdated += OnMouseCellCoordsUpdated;
        _dispatcher.BottomPanelInteriorClicked += BottomPanelInteriorClicked;
        _dispatcher.BottomPanelManageButtonClicked += BottomPanelManageClicked;
        _dispatcher.BottomPanelInteriorCloseClicked += BottomPanelInteriorCloseClicked;
        _dispatcher.BottomPanlelFinishPlacingClicked += BottomPanelFinishPlacingClicked;
        _dispatcher.BottomPanelRotateRightClicked += BottomPanelRotateRightClicked;
        _dispatcher.BottomPanelRotateLeftClicked += BottomPanelRotateLeftClicked;

        _gameStateModel.PlacingStateChanged += OnPlacingStateChanged;
        _gameStateModel.GameStateChanged += OnGameStateChanged;
    }

    private void OnUITopPanelLevelUpAnimationFinished()
    {
        new ShowLevelUpPopupCommand().Execute();
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

    private void OnUIRemovePopupResult(bool result)
    {
        new HandleRemovePopupResult().Execute(result);
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
        if (_gameStateModel.PlacingState != PlacingStateName.None)
        {
            new PlaceObjectCommand().Execute();
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
        _gameStateModel.ResetPlacingState();
    }

    private void BottomPanelInteriorClicked()
    {
        _gameStateModel.SetGameState(GameStateName.ShopInterior);
    }

    private void BottomPanelManageClicked()
    {
        new UIShowManagePopupCommand().Execute();
    }

    private void BottomPanelInteriorCloseClicked()
    {
        _gameStateModel.SetGameState(GameStateName.ShopSimulation);
    }

    private void OnMouseCellCoordsUpdated(Vector2Int newCoords)
    {
        new ProcessHighlightCommand().Execute(newCoords);

        if (_gameStateModel.PlacingShopObjectModel != null)
        {
            _gameStateModel.PlacingShopObjectModel.Coords = newCoords;
        }
    }

    private void OnPlacingStateChanged(PlacingStateName previous, PlacingStateName currentState)
    {
        if (currentState == PlacingStateName.None)
        {
            var mouseCoords = MouseCellCoordsProvider.Instance.MouseCellCoords;
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
}
