using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HumansControlSystem
{
    private readonly GameStateModel _gameStateModel;
    private readonly UpdatesProvider _updatesProvider;
    private readonly Dispatcher _dispatcher;

    private UserModel _viewingUserModel;
    private ShopModel _viewingShopModel;
    private UserSessionDataModel _viewingSessionDataModel;

    public HumansControlSystem()
    {
        _gameStateModel = GameStateModel.Instance;
        _updatesProvider = UpdatesProvider.Instance;
        _dispatcher = Dispatcher.Instance;
    }

    public void Start()
    {
        Activate();
    }

    private void Activate()
    {
        _gameStateModel.GameStateChanged += OnGameStateChanged;
        _gameStateModel.ViewingUserModelChanged += OnViewingUserModelChanged;
        _updatesProvider.GameplaySecondUpdate += OnGameplaySecondUpdate;
    }

    private void OnViewingUserModelChanged(UserModel userModel)
    {
        _viewingUserModel = userModel;
        _viewingShopModel = userModel.ShopModel;
        _viewingSessionDataModel = userModel.SessionDataModel;
    }

    private void OnGameplaySecondUpdate()
    {
        if (_viewingUserModel != null)
        {
            ProcessSpawnLogic();
        }
    }

    private void ProcessSpawnLogic()
    {
        if (_gameStateModel.IsSimulationState && CanSpawnMoreCustomers())
        {
            if (_viewingSessionDataModel.SpawnCooldown <= 0)
            {
                if (TrySpawnCustomerAt(new Vector2Int(_viewingShopModel.ShopDesign.SizeX, -2)) == false)
                {
                    TrySpawnCustomerAt(new Vector2Int(-2, _viewingShopModel.ShopDesign.SizeY));
                }
            }
            else
            {
                _viewingSessionDataModel.SpawnCooldown--;
            }
        }
    }

    private bool CanSpawnMoreCustomers()
    {
        if (Math.Max(1, _viewingShopModel.MoodMultiplier * Constants.MaxCostumersCount) > _viewingSessionDataModel.Customers.Count)
        {
            return true;
        }

        return false;
    }

    private bool TrySpawnCustomerAt(Vector2Int coords)
    {
        if (_viewingSessionDataModel.HaveCustomerAt(coords) == false)
        {
            var hairId = Random.Range(1, 9);
            var topClothesId = Random.Range(1, 8);
            var bottomClothesId = Random.Range(1, 8);
            var glassesId = Random.Range(0, 5);
            var customer = new CustomerModel(hairId, topClothesId, bottomClothesId, glassesId, GetCostumerMood(), coords, 3);
            _viewingSessionDataModel.AddCustomer(customer);
            return true;
        }

        return false;
    }

    private Mood GetCostumerMood()
    {
        var moodList = new List<Mood>(10) { Mood.Default, Mood.o_O };
        var moodMultiplier = _viewingShopModel.MoodMultiplier;
        if (moodMultiplier > 0.5)
        {
            if (moodMultiplier > 0.6)
            {
                moodList.Add(Mood.SmileLite);
            }
            if (moodMultiplier > 0.7)
            {
                moodList.Add(Mood.SmileNorm);
            }
            if (moodMultiplier > 0.8)
            {
                moodList.Add(Mood.SmileGood);
            }
            if (moodMultiplier > 0.9)
            {
                moodList.Add(Mood.Dream1);
            }
        }
        else
        {
            moodList.Add(Mood.EvilNorm);
            if (moodMultiplier < 0.4)
            {
                moodList.Add(Mood.Evil1);
            }
            if (moodMultiplier < 0.3)
            {
                moodList.Add(Mood.EvilAngry);
            }
            if (moodMultiplier < 0.2)
            {
                moodList.Add(Mood.EvilPlan);
            }
        }

        return moodList[Random.Range(0, moodList.Count)];
    }

    private void OnGameStateChanged(GameStateName previousState, GameStateName currentState)
    {
        if (currentState != GameStateName.ShopSimulation)
        {
            Update();
        }
    }

    private void Update()
    {
        //TODO Remove custumers from wrong places
    }
}
