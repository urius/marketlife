using UnityEngine;

public class HandleDebugKeyboardShortcutsSystem
{
    private UpdatesProvider _updatesProvider;
    private GameStateModel _gameStateModel;
    private PlayerModelHolder _playerModelHolder;
    private Dispatcher _dispatcher;

    public void Start()
    {
        _updatesProvider = UpdatesProvider.Instance;
        _gameStateModel = GameStateModel.Instance;
        _playerModelHolder = PlayerModelHolder.Instance;
        _dispatcher = Dispatcher.Instance;

        Activate();
    }

    private void Activate()
    {
        _updatesProvider.RealtimeUpdate += OnRealtimeUpdate;
    }

    private void OnRealtimeUpdate()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            HandleSkipTime();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            HandleDeliverProducts();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            HandleAddGold();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            HandleAddCash();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            ResetMissions();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            SimulateAdvertWatchResult();
        }
    }

    private void SimulateAdvertWatchResult()
    {
        new ProcessShowAdsResultCommand().Execute("{\"data\":{\"is_success\":true}}");
    }

    private void ResetMissions()
    {
        _playerModelHolder.UserModel.DailyMissionsModel.Clear();
        _dispatcher.RequestTriggerSave();
    }

    private void HandleAddCash()
    {
        _playerModelHolder.UserModel.AddCash(Input.GetKey(KeyCode.LeftShift) ? 10000 : 1000);
    }

    private void HandleAddGold()
    {
        _playerModelHolder.UserModel.AddGold(10);
    }

    private void HandleDeliverProducts()
    {
        foreach (var whSlot in _playerModelHolder.ShopModel.WarehouseModel.Slots)
        {
            if (whSlot.HasProduct)
            {
                whSlot.Product.DeliverTime = 0;

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    var gameStateModel = GameStateModel.Instance;
                    gameStateModel.SetPlacingProductOnPlayersShop(whSlot.Index);
                    new AutoPlaceCommand().Execute();
                    gameStateModel.ResetActionState();
                }
            }
        }
    }

    private void HandleSkipTime()
    {
        if (_gameStateModel.GameState == GameStateName.PlayerShopSimulation)
        {
            var timeToSkip = Input.GetKey(KeyCode.LeftShift) ? 3600 : 24 * 3600;
            var serverTimeBuf = _gameStateModel.ServerTime;
            _gameStateModel.SetServerTime(_gameStateModel.ServerTime + timeToSkip);

            var advertModel = AdvertViewStateModel.Instance;
            advertModel.PrepareTarget(AdvertTargetType.OfflineProfitX2);
            advertModel.MarkCurrentAsWatched();
            advertModel.PrepareTarget(AdvertTargetType.OfflineExpX2);
            advertModel.MarkCurrentAsWatched();

            _gameStateModel.SetGameState(GameStateName.ReadyForStart);
            _gameStateModel.SetServerTime(serverTimeBuf);
        }
    }
}
