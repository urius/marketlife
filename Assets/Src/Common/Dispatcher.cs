using System;
using UnityEngine;

public class Dispatcher
{
    public Action<ShopModel> PlayerShopLoaded = delegate { };
    public Action UIGameViewMouseDown = delegate { };
    public Action UIGameViewMouseUp = delegate { };
    public Action UIGameViewMouseClick = delegate { };    
    public Action<int> UIBottomPanelPlaceShelfClicked = delegate { };
    public Action<Vector2Int> MouseCellCoordsUpdated = delegate { };
    public Action BottomPanelInteriorClicked = delegate { };
    public Action BottomPanelInteriorCloseClicked = delegate { };
    public Action BottomPanlelFinishPlacingClicked = delegate { };
    public Action BottomPanelRotateRightClicked = delegate { };
    public Action BottomPanelRotateLeftClicked = delegate { };    

    private static Lazy<Dispatcher> _instance = new Lazy<Dispatcher>();
    public static Dispatcher Instance => _instance.Value;
}
