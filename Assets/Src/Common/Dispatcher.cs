using System;

public class Dispatcher
{
    public Action<ShopModel> PlayerShopLoaded = delegate { };

    private static Dispatcher _instance;

    public static Dispatcher Instance => GetOrCreateInstance();

    private static Dispatcher GetOrCreateInstance()
    {
        if (_instance == null)
        {
            _instance = new Dispatcher();
        }

        return _instance;
    }
}
