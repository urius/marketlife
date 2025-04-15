using System.Collections.Generic;
using System.Linq;
using Src.Common;

public class DailyMissionSellProductProcessor : DailyMissionProcessorBase
{
    private readonly PlayerOfflineReportHolder _offlineReportHolder;
    private readonly GameStateModel _gameStateModel;
    private readonly Dispatcher _dispatcher;

    //
    private DailyMissionSellProductModel _missionModel;
    private bool _isOfflineRepoertProcessed = false;

    public DailyMissionSellProductProcessor()
    {
        _offlineReportHolder = PlayerOfflineReportHolder.Instance;
        _gameStateModel = GameStateModel.Instance;
        _dispatcher = Dispatcher.Instance;
    }

    public override void SetupMissionModel(DailyMissionModel missionModel)
    {
        base.SetupMissionModel(missionModel);
        _missionModel = missionModel as DailyMissionSellProductModel;
    }

    public override void Start()
    {
        _gameStateModel.GameStateChanged += OnGameStateChanged;
        _dispatcher.CustomerBuyProduct += OnCustomerBuyProduct;
    }

    public override void Stop()
    {
        _gameStateModel.GameStateChanged -= OnGameStateChanged;
        _dispatcher.CustomerBuyProduct -= OnCustomerBuyProduct;
    }

    private void OnGameStateChanged(GameStateName prevState, GameStateName currentState)
    {
        if (_isOfflineRepoertProcessed == false
            && prevState == GameStateName.ReadyForStart
            && _gameStateModel.IsPlayingState)
        {
            if (_offlineReportHolder.PlayerOfflineReport != null)
            {
                var soldCount = GetSoldProductsCount(_offlineReportHolder.PlayerOfflineReport.SoldFromShelfs) +
                                GetSoldProductsCount(_offlineReportHolder.PlayerOfflineReport.SoldFromWarehouse);
                _missionModel.AddValue(soldCount);
                _isOfflineRepoertProcessed = true;
            }
        }
    }

    private void OnCustomerBuyProduct(ProductModel productModel)
    {
        if (productModel.Config.NumericId == _missionModel.ProductConfig.NumericId)
        {
            _missionModel.AddValue(productModel.Amount);
        }
    }

    private int GetSoldProductsCount(Dictionary<ProductConfig, int> soldProducts)
    {
        return soldProducts
            .Where(kvp => kvp.Key.NumericId == _missionModel.ProductConfig.NumericId)
            .Sum(kvp => kvp.Value);
    }
}
