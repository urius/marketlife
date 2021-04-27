using System;

public class Dispatcher
{
    public Action<ShopModel> PlayerShopLoaded = delegate { };
    public Action UIGameViewMouseDown = delegate { };
    public Action UIGameViewMouseUp = delegate { };
    public Action<int> UIBottomPanelPlaceShelfClicked = delegate { };

    private static Lazy<Dispatcher> _instance = new Lazy<Dispatcher>();
    public static Dispatcher Instance => _instance.Value;
}
