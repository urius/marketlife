using System;
using UnityEngine;

public class HandleDebugKeyboardShortcutsSystem
{
    private UpdatesProvider _updatesProvider;
    private GameStateModel _gameStateModel;
    private PlayerModelHolder _playerModelHolder;

    public void Start()
    {
        _updatesProvider = UpdatesProvider.Instance;
        _gameStateModel = GameStateModel.Instance;
        _playerModelHolder = PlayerModelHolder.Instance;

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
    }

    private void HandleAddCash()
    {
        _playerModelHolder.UserModel.AddCash(500);
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
            }
        }
    }

    private void HandleSkipTime()
    {
        if (_gameStateModel.GameState == GameStateName.PlayerShopSimulation)
        {
            var serverTimeBuf = _gameStateModel.ServerTime;
            _gameStateModel.SetServerTime(_gameStateModel.ServerTime + 3600);
            _gameStateModel.SetGameState(GameStateName.ReadyForStart);
            _gameStateModel.SetServerTime(serverTimeBuf);
        }
    }
}
