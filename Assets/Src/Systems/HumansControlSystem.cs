using System;
using System.Collections.Generic;
using System.Linq;
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
    private ShelfModel[] _allShelfs;
    private WalkableCellsProvider _cellsProvider;
    private WalkableCellsConsiderHumansProvider _cellsConsiderHumansProvider;
    private Dictionary<ProductConfig, ProductInfoForBuy> _productsData = new Dictionary<ProductConfig, ProductInfoForBuy>();

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
        _dispatcher.CustomerAnimationEnded += OnCustomerAnimationEnded;
    }

    private void OnViewingUserModelChanged(UserModel userModel)
    {
        _viewingUserModel = userModel;
        _viewingShopModel = userModel.ShopModel;
        _viewingSessionDataModel = userModel.SessionDataModel;
        _cellsProvider = new WalkableCellsProvider(_viewingShopModel, _viewingSessionDataModel);
        UpdateShelfsList();
    }

    private void UpdateShelfsList()
    {
        var shelfs = new List<ShelfModel>(_viewingShopModel.ShopObjects.Count);
        foreach (var kvp in _viewingShopModel.ShopObjects)
        {
            if (kvp.Value.Type == ShopObjectType.Shelf)
            {
                shelfs.Add(kvp.Value as ShelfModel);
            }
        }
        _allShelfs = shelfs.ToArray();
    }

    private void OnGameplaySecondUpdate()
    {
        if (_viewingUserModel != null)
        {
            if (_gameStateModel.IsSimulationState && _gameStateModel.IsGamePaused == false)
            {
                ProcessSpawnLogic();
                ProcessIdleUnits();
            }
        }
    }

    private void OnCustomerAnimationEnded(CustomerModel customer)
    {
        ProcessState(customer);
    }

    private void ProcessIdleUnits()
    {
        if (_allShelfs == null || _allShelfs.Length <= 0) return;

        foreach (var customer in _viewingSessionDataModel.Customers)
        {
            if (customer.AnimationState == CustomerAnimationState.Idle)
            {
                ProcessState(customer);
            }
        }
    }

    private void ProcessState(CustomerModel customer)
    {
        switch (customer.LifetimeState)
        {
            case CustomerLifetimeState.Default:
                MoveToRandomShelf(customer);
                break;
            case CustomerLifetimeState.MovingToShelf:
                ProcessMovingToShelf(customer);
                break;
            case CustomerLifetimeState.WatchingShelf:
                ProcessWatchingShelf(customer);
                break;
            case CustomerLifetimeState.MovingToCashDesk:
                ProcessMovingToCashDesk(customer);
                break;
        }
    }

    private void ProcessWatchingShelf(CustomerModel customer)
    {
        if (_viewingShopModel.ShopObjects.TryGetValue(customer.TargetCell, out var targetShelf)
            && targetShelf.Type == ShopObjectType.Shelf)
        {
            customer.ShelfsVisited++;
            var shelfModel = targetShelf as ShelfModel;
            foreach (var slot in shelfModel.Slots)
            {
                if (slot.HasProduct)
                {
                    var productBuyInfo = GetProductInfoForBuying(slot.Product);
                    if (productBuyInfo.TakenAmount < productBuyInfo.CanTakeMaxAmount)
                    {
                        var product = slot.Product;
                        var productConfig = product.Config;
                        var amountIn1000v = productConfig.GetAmountInVolume(1000);
                        var takeAmount = Math.Min(product.Amount, Math.Max(productBuyInfo.CanTakeMaxAmount - productBuyInfo.TakenAmount, amountIn1000v));
                        customer.AddProduct(productConfig, takeAmount);
                        _viewingUserModel.ProgressModel.DelayedCash += productConfig.GetSellPriceForAmount(takeAmount);
                        product.Amount -= takeAmount;

                        customer.LifetimeState = CustomerLifetimeState.TakingProductFromShelf;
                        customer.ToTakingProductState();
                        //todo animate taking
                        return;
                    }
                }
            }
            ProcessAfterShelfVisit(customer);
        }
        else
        {
            MoveToRandomShelf(customer);
        }
    }

    private void ProcessAfterShelfVisit(CustomerModel customer)
    {
        if (customer.ShelfsVisited > _viewingShopModel.MoodMultiplier * 0.1)
        {
            MoveToRandomCashDesk(customer);
        }
        else
        {
            MoveToRandomShelf(customer);
        }
    }

    private void MoveToRandomCashDesk(CustomerModel customer)
    {
        var cashDesks = new List<CashDeskModel>();
        foreach (var kvp in _viewingShopModel.ShopObjects)
        {
            if (kvp.Value.Type == ShopObjectType.CashDesk)
            {
                cashDesks.Add(kvp.Value as CashDeskModel);
            }
        }
        var targetCashDesk = cashDesks[Random.Range(0, cashDesks.Count)];
        var targetCellCoords = targetCashDesk.Coords + SideHelper.SideToVector(targetCashDesk.Side);
        customer.TargetCell = targetCellCoords;
        var path = Pathfinder.FindPath(_cellsProvider, customer.Coords, targetCellCoords);
        customer.SetPath(path);
        customer.LifetimeState = CustomerLifetimeState.MovingToCashDesk;
        ProcessState(customer);
    }

    private ProductInfoForBuy GetProductInfoForBuying(ProductModel product)
    {
        if (_productsData.ContainsKey(product.Config) == false)
        {
            _productsData[product.Config] = new ProductInfoForBuy();
        }
        var hoursPassed = Time.realtimeSinceStartup / 3600f;
        _productsData[product.Config].CanTakeMaxAmount = (int)(product.Config.Demand * hoursPassed * _viewingShopModel.MoodMultiplier);

        return _productsData[product.Config];
    }

    private void ProcessMovingToShelf(CustomerModel customer)
    {
        if (customer.TryGetNextStepCoords(out var coords))
        {
            if (_viewingSessionDataModel.HaveCustomerAt(coords))
            {
                var lastPathCoords = customer.Path[customer.Path.Length - 1];
                var path = Pathfinder.FindPath(_cellsConsiderHumansProvider, customer.Coords, lastPathCoords);
                if (path.Length > 0 && path[path.Length - 1] == lastPathCoords)
                {
                    customer.SetPath(path);
                    customer.MakeStep();
                }
                else
                {
                    var nearWalkableCells = _cellsProvider.GetWalkableNearCells(customer.Coords).ToArray();
                    if (nearWalkableCells.Length > 0)
                    {
                        var rndCell = nearWalkableCells[Random.Range(0, nearWalkableCells.Length)];
                        customer.InsertNextStep(rndCell);
                        customer.MakeStep();
                    }
                    else
                    {
                        customer.ToIdleState();
                    }
                }
            }
            else
            {
                customer.MakeStep();
            }
        }
        else
        {
            customer.Side = SideHelper.VectorToSide(customer.TargetCell - customer.Coords);
            customer.LifetimeState = CustomerLifetimeState.WatchingShelf;
            customer.ToThinkingState();
        }
    }

    private void ProcessMovingToCashDesk(CustomerModel customer)
    {
        //todo additional logic (idle in case of near customer with same target)
        if (customer.TryGetNextStepCoords(out var coords))
        {
            if (_viewingSessionDataModel.HaveCustomerAt(coords))
            {
                var lastPathCoords = customer.Path[customer.Path.Length - 1];
                var path = Pathfinder.FindPath(_cellsConsiderHumansProvider, customer.Coords, lastPathCoords);
                if (path.Length > 0 && path[path.Length - 1] == lastPathCoords)
                {
                    customer.SetPath(path);
                    customer.MakeStep();
                }
                else
                {
                    var nearWalkableCells = _cellsProvider.GetWalkableNearCells(customer.Coords).ToArray();
                    if (nearWalkableCells.Length > 0)
                    {
                        var rndCell = nearWalkableCells[Random.Range(0, nearWalkableCells.Length)];
                        customer.InsertNextStep(rndCell);
                        customer.MakeStep();
                    }
                    else
                    {
                        customer.ToIdleState();
                    }
                }
            }
            else
            {
                customer.MakeStep();
            }
        }
        else
        {
            customer.Side = SideHelper.VectorToSide(customer.TargetCell - customer.Coords);
            customer.LifetimeState = CustomerLifetimeState.PayingOnCashDesk;
            //customer.ToPayingState(); //todo
        }
    }

    private void MoveToRandomShelf(CustomerModel customer)
    {
        var targetShelf = GetRandomShelf();
        customer.TargetCell = targetShelf.Coords;
        var path = Pathfinder.FindPath(_cellsProvider, customer.Coords, targetShelf.EntryCoords);
        customer.SetPath(path);
        customer.LifetimeState = CustomerLifetimeState.MovingToShelf;
        ProcessState(customer);
    }

    private ShelfModel GetRandomShelf()
    {
        if (_allShelfs == null || _allShelfs.Length == 0)
        {
            return null;
        }

        return _allShelfs[Random.Range(0, _allShelfs.Length)];
    }

    private void ProcessSpawnLogic()
    {
        if (CanSpawnMoreCustomers())
        {
            if (_viewingSessionDataModel.SpawnCooldown <= 0)
            {
                var spawnPoints = new Vector2Int[] { new Vector2Int(_viewingShopModel.ShopDesign.SizeX, -2), new Vector2Int(-2, _viewingShopModel.ShopDesign.SizeY) };
                TrySpawnCustomerAt(spawnPoints[Random.Range(0, spawnPoints.Length)]);
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
            _viewingSessionDataModel.SpawnCooldown = 3;
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
        if (_gameStateModel.IsSimulationState)
        {
            UpdateShelfsList();
        }

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

public class ProductInfoForBuy
{
    public int TakenAmount;
    public int CanTakeMaxAmount;
}

public class WalkableCellsProvider : ICellsProvider<Vector2Int>
{
    protected readonly UserSessionDataModel SessionDataModel;

    private readonly ShopModel _shopModel;
    private readonly ShoDesignModel _shopDesign;

    public WalkableCellsProvider(ShopModel shopModel, UserSessionDataModel sessionDataModel)
    {
        _shopModel = shopModel;
        _shopDesign = shopModel.ShopDesign;
        SessionDataModel = sessionDataModel;
    }

    public int GetCellMoveCost(Vector2Int cell)
    {
        return 1;
    }

    public IEnumerable<Vector2Int> GetWalkableNearCells(Vector2Int cell)
    {
        for (var side = 1; side < 5; side++)
        {
            var cellToCheck = cell + SideHelper.SideToVector(side);
            if (IsWalkable(cellToCheck))
            {
                yield return cellToCheck;
            }
        }
    }

    protected virtual bool IsWalkable(Vector2Int cell)
    {
        if (cell.x < -3 || cell.y < -3 || cell.x > _shopDesign.SizeX + 2 || cell.y > _shopDesign.SizeY + 2)
        {
            return false;
        }
        if ((cell.x == -1 && cell.y >= 0) || (cell.y == -1 && cell.x >= 0))
        {
            return _shopModel.ShopDesign.Doors.ContainsKey(cell);
        }
        if ((cell.x >= _shopDesign.SizeX && cell.y >= 0) || (cell.y >= _shopDesign.SizeY && cell.x >= 0))
        {
            return false;
        }
        if (_shopModel.IsBuiltOnCell(cell))
        {
            return false;
        }

        return true;
    }

    public bool IsCellEquals(Vector2Int cellA, Vector2Int cellB)
    {
        return cellA == cellB;
    }
}

public class WalkableCellsConsiderHumansProvider : WalkableCellsProvider
{
    public WalkableCellsConsiderHumansProvider(ShopModel shopModel, UserSessionDataModel sessionDataModel)
        : base(shopModel, sessionDataModel)
    {
    }

    protected override bool IsWalkable(Vector2Int cell)
    {
        if (SessionDataModel.HaveCustomerAt(cell))
        {
            return false;
        }
        return base.IsWalkable(cell);
    }
}
