using System;
using UnityEngine;

public class Dispatcher
{
    public Action<ShopModel> PlayerShopLoaded = delegate { };

    public Action<Vector3> CameraMoved = delegate { };
    public Action MouseMoved = delegate { };    

    public Action UIGameViewMouseDown = delegate { };
    public Action UIGameViewMouseUp = delegate { };
    public Action UIGameViewMouseClick = delegate { };
    public Action UIGameViewMouseEnter = delegate { };
    public Action UIGameViewMouseExit = delegate { };
    public Action UIBottomPanelPointerEnter = delegate { };    
    public Action UIBottomPanelPointerExit = delegate { };

    public Action<int> UIBottomPanelPlaceShelfClicked = delegate { };
    public Action<int> UIBottomPanelPlaceFloorClicked = delegate { };
    public Action<int> UIBottomPanelPlaceWallClicked = delegate { };
    public Action<int> UIBottomPanelPlaceWindowClicked = delegate { };
    public Action<int> UIBottomPanelPlaceDoorClicked = delegate { };
    public Action<int> UIBottomPanelWarehouseSlotClicked = delegate { };
    public Action<int> UIBottomPanelWarehouseQuickDeliverClicked = delegate { };
    public Action<int> UIBottomPanelWarehouseRemoveProductClicked = delegate { };    

    public Action UIActionsRotateRightClicked = delegate { };
    public Action UIActionsRotateLeftClicked = delegate { };
    public Action UIActionsMoveClicked = delegate { };
    public Action UIActionsRemoveClicked = delegate { };
    public Action<bool> UIRemovePopupResult = delegate { };

    public Action<bool> UIRequestBlinkMoney = delegate { };
    public Action<Vector2, bool, int> UIRequestFlyingPrice = delegate { };
    public Action<Vector2, string> UIRequestFlyingText = delegate { };    
    public Action<Vector2, string, int> UIRequestFlyingProduct = delegate { };
    public Action UIRequestRemoveCurrentPopup = delegate { };
    public Action<RectTransform, Vector2, ProductConfig> UIOrderProductClicked = delegate { };
    public Action<RectTransform, Vector2, int, ProductModel> UIRequestOrderProductAnimation = delegate { };
    public Action<ShelfContentPopupViewModel, int> UIShelfContentAddProductClicked = delegate { };
    public Action<ShelfContentPopupViewModel, int> UIShelfContentRemoveProductClicked = delegate { };
    public Action<WarehousePopupViewModel, int> UIWarehousePopupSlotClicked = delegate { };
    public Action<UpgradesPopupItemViewModelBase> UIUpgradeWindownBuyClicked = delegate { };    

    public Action<Vector2Int> MouseCellCoordsUpdated = delegate { };
    public Action RequestForceMouseCellPositionUpdate = delegate { };
    
    public Action BottomPanelInteriorClicked = delegate { };
    public Action BottomPanelManageButtonClicked = delegate { };    
    public Action BottomPanelInteriorCloseClicked = delegate { };
    public Action BottomPanlelFinishPlacingClicked = delegate { };
    public Action BottomPanelRotateRightClicked = delegate { };
    public Action BottomPanelRotateLeftClicked = delegate { };
    public Action NotifyNotEnoughtMoney = delegate { };    

    private static Lazy<Dispatcher> _instance = new Lazy<Dispatcher>();
    public static Dispatcher Instance => _instance.Value;
}
