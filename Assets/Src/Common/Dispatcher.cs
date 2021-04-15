using System;

public class Dispatcher
{
    public Action<ShopModel> PlayerShopLoaded = delegate { };
    public Action GameViewMouseDown = delegate { };
    public Action GameViewMouseUp = delegate { };

    private static Lazy<Dispatcher> _instance = new Lazy<Dispatcher>();
    public static Dispatcher Instance => _instance.Value;
}
