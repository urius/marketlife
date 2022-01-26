using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserModel
{
    public Action<int> TutorialStepPassed = delegate { };
    public Action BonusStateUpdated = delegate { };
    public Action SettingsUpdated = delegate { };

    public readonly string Uid;
    public readonly UserProgressModel ProgressModel;
    public readonly ShopModel ShopModel;
    public readonly UserStatsData StatsData;
    public readonly UserSessionDataModel SessionDataModel;
    public readonly AllFriendsShopsActionsModel FriendsActionsDataModels;
    public readonly List<int> TutorialSteps;
    public readonly UserSettingsModel UserSettingsModel;
    public readonly ExternalActionsModel ExternalActionsModel;
    public readonly DailyMissionsModel DailyMissionsModel;
    public readonly int RandomSeed;

    public UserModel(
        string uid,
        UserProgressModel progressModel,
        ShopModel shopModel,
        UserStatsData statsData,
        UserBonusState bonusState,
        int[] tutorialSteps,
        AllFriendsShopsActionsModel friendsActionsDataModel,
        UserSettingsModel userSettingsModel,
        ExternalActionsModel externalActionsModel,
        DailyMissionsModel dailyMissionsModel)
    {
        Uid = uid;
        ProgressModel = progressModel;
        ShopModel = shopModel;
        StatsData = statsData;
        BonusState = bonusState;
        FriendsActionsDataModels = friendsActionsDataModel;
        SessionDataModel = new UserSessionDataModel();
        //TutorialSteps = new List<int>(new int[] { 0,1,2,3,4,5,6,7,8,9 });
        TutorialSteps = new List<int>(tutorialSteps ?? Enumerable.Empty<int>());
        UserSettingsModel = userSettingsModel;
        ExternalActionsModel = externalActionsModel;
        DailyMissionsModel = dailyMissionsModel;

        if (int.TryParse(uid, out var uidInt))
        {
            RandomSeed = uidInt;
        }
        else
        {
            RandomSeed = UnityEngine.Random.Range(0, int.MaxValue);
        }

        SubscribeForSettingsModel();
    }

    public UserBonusState BonusState { get; private set; }

    public void UpdateDailyBonus(int takeTimestamp, int takenBonusRank)
    {
        var bonusState = BonusState;
        bonusState.LastBonusTakeTimestamp = takeTimestamp;
        bonusState.LastTakenBonusRank = takenBonusRank;
        UpdateBonusState(bonusState);
    }

    public void MarkOldGameBonusProcessed()
    {
        var bonusState = BonusState;
        bonusState.IsOldGameBonusProcessed = true;
        UpdateBonusState(bonusState);
    }

    public void ApplyExternalActions()
    {
        ApplyExternalAction(FriendShopActionId.AddUnwash);
        ApplyExternalAction(FriendShopActionId.AddProduct);
        ApplyExternalAction(FriendShopActionId.TakeProduct);
        ShopModel.TrimProducts();
    }

    private void ApplyExternalAction(FriendShopActionId actionId)
    {
        foreach (var action in ExternalActionsModel.Actions)
        {
            if (action.ActionId == actionId)
            {
                switch (action.ActionId)
                {
                    case FriendShopActionId.AddProduct:
                        ApplyAddProductAction(action as ExternalActionAddProduct);
                        break;
                    case FriendShopActionId.TakeProduct:
                        ApplyTakeAction(action as ExternalActionTakeProduct);
                        break;
                    case FriendShopActionId.AddUnwash:
                        ApplyAddUnwashAction(action as ExternalActionAddUnwash);
                        break;
                }
            }
        }
    }

    private void ApplyAddProductAction(ExternalActionAddProduct addProductAction)
    {
        var amountToAdd = addProductAction.Amount;
        if (amountToAdd > 0
            && ShopModel.ShopObjects.TryGetValue(addProductAction.Coords, out var shopObjectModel)
            && shopObjectModel.Type == ShopObjectType.Shelf)
        {
            var shelfModel = shopObjectModel as ShelfModel;
            var slot = shelfModel.Slots[Mathf.Min(addProductAction.ShelfSlotIndex, shelfModel.Slots.Length - 1)];
            if (slot.HasProduct
                && slot.Product.Config.Key == addProductAction.ProductConfig.Key)
            {
                slot.ChangeProductAmount(amountToAdd);
            }
            else if (slot.HasProduct == false)
            {
                slot.SetProduct(new ProductModel(addProductAction.ProductConfig, amountToAdd));
            }
        }
    }

    private void ApplyTakeAction(ExternalActionTakeProduct takeAction)
    {
        var takeAmountRest = takeAction.Amount;
        if (ShopModel.ShopObjects.TryGetValue(takeAction.Coords, out var shopObjectModel) && shopObjectModel.Type == ShopObjectType.Shelf)
        {
            var shelfModel = shopObjectModel as ShelfModel;
            foreach (var slot in shelfModel.Slots)
            {
                if (slot.HasProduct && slot.Product.Config.Key == takeAction.ProductConfig.Key)
                {
                    takeAmountRest += slot.ChangeProductAmount(-takeAmountRest);
                }
                if (takeAmountRest <= 0) break;
            }
        }
    }

    private void ApplyAddUnwashAction(ExternalActionAddUnwash addUnwashAction)
    {
        ShopModel.AddUnwash(addUnwashAction.Coords, 1);
    }

    private void SubscribeForSettingsModel()
    {
        UserSettingsModel.AudioMutedStateChanged += OnAudioMutedStateChanged;
        UserSettingsModel.MusicMutedStateChanged += OnMusicMutedStateChanged;
    }

    private void OnAudioMutedStateChanged()
    {
        SettingsUpdated();
    }

    private void OnMusicMutedStateChanged()
    {
        SettingsUpdated();
    }

    private void UpdateBonusState(UserBonusState newBonusState)
    {
        BonusState = newBonusState;
        BonusStateUpdated();
    }

    public void AddPassedTutorialStep(int stepIndex)
    {
        if (TutorialSteps.Contains(stepIndex) == false)
        {
            TutorialSteps.Add(stepIndex);
            TutorialStepPassed(stepIndex);
        }
    }

    public bool IsTutorialStepPassed(int stepIndex)
    {
        return TutorialSteps.Contains(stepIndex);
    }

    public bool CanSpendMoney(string price)
    {
        return CanSpendMoney(Price.FromString(price));
    }

    public bool CanSpendMoney(Price price)
    {
        return price.IsGold ? ProgressModel.CanSpendGold(price.Value) : ProgressModel.CanSpendCash(price.Value);
    }

    public bool TrySpendMoney(string price)
    {
        return TrySpendMoney(Price.FromString(price));
    }

    public bool TrySpendMoney(Price price)
    {
        return price.IsGold ? ProgressModel.TrySpendGold(price.Value) : ProgressModel.TrySpendCash(price.Value);
    }

    public void AddCash(int amount)
    {
        ProgressModel.AddCash(amount);
    }

    public void AddGold(int amount)
    {
        ProgressModel.AddGold(amount);
    }

    public void AddExp(int amount)
    {
        ProgressModel.AddExp(amount);
    }

    public OfflineCalculationResult CalculateOfflineToTime(int targetTime)
    {
        var userShopModel = ShopModel;
        var personalModel = userShopModel.PersonalModel;
        var warehouseModel = userShopModel.WarehouseModel;

        var (restProductsOnShelfs, totalShelfsVolume, restUsedShelfsVolume) = userShopModel.GetAllProductsInfo();
        var uniqueProductsOnShelfsCount = restProductsOnShelfs.Length;
        var startCalculationTime = StatsData.LastVisitTimestamp > 0 ? StatsData.LastVisitTimestamp : targetTime;
        var secondsSinceLastVisit = targetTime - startCalculationTime;
        var hoursSinceLastVisit = secondsSinceLastVisit / 3600f;
        var hoursCeilSinceLastVisit = (int)Math.Ceiling(hoursSinceLastVisit);
        var soldFromShelfsProducts = new Dictionary<ProductConfig, int>(uniqueProductsOnShelfsCount);
        var cleanedUnwashesAmount = 0;
        var soldFromWarehouseProducts = new Dictionary<ProductConfig, int>(uniqueProductsOnShelfsCount);
        var restWarehouseProductsForMerchandiser = new Dictionary<ProductConfig, int>(uniqueProductsOnShelfsCount);
        var random = new System.Random(targetTime);
        var grabbedProducts = GetGrabbedProductsForTime(restProductsOnShelfs, startCalculationTime, targetTime);
        CorrectRestProductsAmountsByGrabbedProducts(restProductsOnShelfs, grabbedProducts);

        foreach (var productModel in restProductsOnShelfs)
        {
            soldFromShelfsProducts[productModel.Config] = 0;
            soldFromWarehouseProducts[productModel.Config] = 0;
            restWarehouseProductsForMerchandiser[productModel.Config] = warehouseModel.GetDeliveredProductAmount(productModel.Config.NumericId, targetTime);
        }
        var unwashesCountAdded = 0;
        var totalShopSquare = userShopModel.ShopDesign.SizeX * userShopModel.ShopDesign.SizeY;
        for (var i = 0; i < hoursCeilSinceLastVisit; i++)
        {
            var unwashesCount = userShopModel.Unwashes.Count + unwashesCountAdded;
            var moodMultiplier = CalculationHelper.CalculateMood(restUsedShelfsVolume, totalShelfsVolume, unwashesCount, totalShopSquare);
            var hourMultiplier = Math.Min(1, hoursSinceLastVisit - i);
            var buyMultiplier = moodMultiplier * hourMultiplier;
            var iterationStartTime = startCalculationTime + i * 3600;
            var isMerchandiserActive = personalModel.GetMaxEndWorkTimeForPersonalType(PersonalType.Merchandiser) > iterationStartTime;
            var isCleanerActive = personalModel.GetMaxEndWorkTimeForPersonalType(PersonalType.Cleaner) > iterationStartTime;

            var haveProductsOnShefsFlag = false;
            foreach (var productModel in restProductsOnShelfs)
            {
                if (productModel.Amount > 0)
                {
                    haveProductsOnShefsFlag = true;

                    var productConfig = productModel.Config;
                    var demandedRestAmountToSell = CalculationHelper.GetIntegerDemand(productConfig.Demand * buyMultiplier);

                    if (isMerchandiserActive && restWarehouseProductsForMerchandiser[productConfig] > 0)
                    {
                        var sellFromWarehouseAmount = Math.Min(restWarehouseProductsForMerchandiser[productConfig], demandedRestAmountToSell);
                        restWarehouseProductsForMerchandiser[productConfig] -= sellFromWarehouseAmount;
                        soldFromWarehouseProducts[productConfig] += sellFromWarehouseAmount;
                        demandedRestAmountToSell -= sellFromWarehouseAmount;
                    }

                    var sellFromShelfsAmount = Math.Min(productModel.Amount, demandedRestAmountToSell);
                    if (sellFromShelfsAmount > 0)
                    {
                        soldFromShelfsProducts[productConfig] += sellFromShelfsAmount;
                        productModel.Amount -= sellFromShelfsAmount;
                        restUsedShelfsVolume -= productConfig.Volume * sellFromShelfsAmount;
                        demandedRestAmountToSell -= sellFromShelfsAmount;
                    }
                }
            }

            if (random.NextDouble() <= moodMultiplier)
            {
                if (isCleanerActive)
                {
                    cleanedUnwashesAmount++;
                }
                else
                {
                    unwashesCountAdded++;
                }
            }

            if (isCleanerActive)
            {
                var maxUnwashesToClean = Math.Max(1, (int)Math.Ceiling(hourMultiplier * 10));
                cleanedUnwashesAmount += CleanUnwashes(maxUnwashesToClean);
            }

            if (haveProductsOnShefsFlag == false)
            {
                break;
            }
        }

        return new OfflineCalculationResult(soldFromShelfsProducts, soldFromWarehouseProducts, unwashesCountAdded, cleanedUnwashesAmount);
    }

    private int CleanUnwashes(int maxUnwashesToClean)
    {
        var result = 0;
        var restUnwashes = Math.Min(maxUnwashesToClean, ShopModel.Unwashes.Count);
        while (restUnwashes > 0)
        {
            restUnwashes--;
            if (ShopModel.RemoveUnwash(ShopModel.Unwashes.First().Key))
            {
                result++;
            }
        }
        return result;
    }

    private void CorrectRestProductsAmountsByGrabbedProducts(ProductModel[] restProductsOnShelfs, Dictionary<ProductConfig, int> grabbedProductsAmounts)
    {
        foreach (var restProductItem in restProductsOnShelfs)
        {
            if (grabbedProductsAmounts.ContainsKey(restProductItem.Config))
            {
                restProductItem.Amount = Math.Max(0, restProductItem.Amount - grabbedProductsAmounts[restProductItem.Config]);
            }
        }
    }

    private Dictionary<ProductConfig, int> GetGrabbedProductsForTime(ProductModel[] restProducts, int startCalculationTime, int targetTime)
    {
        var grabbedProductsDict = new Dictionary<ProductConfig, int>();
        foreach (var action in ExternalActionsModel.Actions)
        {
            if (action.ActionId == FriendShopActionId.TakeProduct)
            {
                var takeAction = action as ExternalActionTakeProduct;
                if (grabbedProductsDict.ContainsKey(takeAction.ProductConfig) == false)
                {
                    grabbedProductsDict[takeAction.ProductConfig] = 0;
                }

                grabbedProductsDict[takeAction.ProductConfig] += takeAction.Amount;
            }
        }

        var restProductsDict = restProducts.ToDictionary(m => m.Config, m => m.Amount);
        var grabbedProductConfigs = grabbedProductsDict.Keys.ToArray();
        foreach (var productConfig in grabbedProductConfigs)
        {
            if (restProductsDict.ContainsKey(productConfig))
            {
                var restAmount = restProductsDict[productConfig];
                var amountTaken = Math.Min(restAmount, grabbedProductsDict[productConfig]);
                grabbedProductsDict[productConfig] = amountTaken;
            }
        }

        return grabbedProductsDict;
    }
}

public struct UserStatsData
{
    public int FirstVisitTimestamp;
    public int LastVisitTimestamp;
    public int TotalDaysPlayCount;

    public UserStatsData(
        int firstVisitTimestamp,
        int lastVisitTimestamp,
        int totalDaysPlayCount)
    {
        FirstVisitTimestamp = firstVisitTimestamp;
        LastVisitTimestamp = lastVisitTimestamp;
        TotalDaysPlayCount = totalDaysPlayCount;
    }
}

public struct UserBonusState
{
    public int LastBonusTakeTimestamp;
    public int LastTakenBonusRank;
    public bool IsOldGameBonusProcessed;
}

public class UserProgressModel
{
    public event Action<int, int> CashChanged = delegate { };
    public event Action<int, int> GoldChanged = delegate { };
    public event Action<int> ExpChanged = delegate { };
    public event Action<int> LevelChanged = delegate { };

    public int DelayedCash = 0;

    public int Cash => Decode(_cashEncoded);
    public int Gold => Decode(_goldEncoded);
    public int ExpAmount => Decode(_expEncoded);
    public int Level { get; private set; }
    public int PreviousLevel { get; private set; }
    public int CurrentLevelMinExp { get; private set; }
    public int NextLevelMinExp { get; private set; }
    public float LevelProgress => (ExpAmount - CurrentLevelMinExp) / (float)(NextLevelMinExp - CurrentLevelMinExp);

    private readonly ILevelsConfig _levelsConfig;
    private string _cashEncoded;
    private string _goldEncoded;
    private string _expEncoded;

    public UserProgressModel(ILevelsConfig levelsConfig, int cash, int gold, int expAmount)
    {
        _levelsConfig = levelsConfig;
        _cashEncoded = Encode(cash);
        _goldEncoded = Encode(gold);
        _expEncoded = Encode(expAmount);
        UpdateLevel(_levelsConfig.GetLevelByExp(expAmount));
    }

    public void SetCash(int newValue)
    {
        var valueBefore = Cash;
        _cashEncoded = Encode(newValue);
        CashChanged(valueBefore, newValue);
    }

    public bool CanSpendCash(int spendAmount)
    {
        var currentValue = Cash;
        return currentValue >= spendAmount;
    }

    public void AddCash(int amount)
    {
        TrySpendCash(-amount);
    }

    public bool TrySpendMoney(Price price)
    {
        return price.IsGold ? TrySpendGold(price.Value) : TrySpendCash(price.Value);
    }

    public bool TrySpendCash(int spendAmount)
    {
        var currentValue = Cash;
        if (currentValue >= spendAmount)
        {
            currentValue -= spendAmount;
            SetCash(currentValue);
            return true;
        }
        return false;
    }

    public void SetGold(int newValue)
    {
        var valueBefore = Gold;
        _goldEncoded = Encode(newValue);
        GoldChanged(valueBefore, newValue);
    }

    public bool CanSpendGold(int spendAmount)
    {
        var currentValue = Gold;
        return currentValue >= spendAmount;
    }

    public void AddGold(int amount)
    {
        TrySpendGold(-amount);
    }

    public bool TrySpendGold(int spendAmount)
    {
        var currentValue = Gold;
        if (currentValue >= spendAmount)
        {
            currentValue -= spendAmount;
            SetGold(currentValue);
            return true;
        }
        return false;
    }

    public void AddExp(int amount)
    {
        var valueBefore = ExpAmount;
        var newValue = valueBefore + amount;
        _expEncoded = Encode(newValue);
        var levelForNewExp = _levelsConfig.GetLevelByExp(newValue);
        if (levelForNewExp != Level)
        {
            UpdateLevel(levelForNewExp);
        }
        ExpChanged(newValue - valueBefore);
    }

    private void UpdateLevel(int level)
    {
        PreviousLevel = Level;
        Level = level;
        CurrentLevelMinExp = _levelsConfig.GetExpByLevel(level);
        NextLevelMinExp = _levelsConfig.GetExpByLevel(level + 1);
        if (PreviousLevel > 0 && PreviousLevel != Level)
        {
            LevelChanged(Level - PreviousLevel);
        }
    }

    private string Encode(int input)
    {
        return Base64Helper.Base64Encode(input.ToString());
    }

    private int Decode(string base64Input)
    {
        return int.Parse(Base64Helper.Base64Decode(base64Input));
    }
}

public class OfflineCalculationResult
{
    public readonly Dictionary<ProductConfig, int> SoldFromShelfs;
    public readonly Dictionary<ProductConfig, int> SoldFromWarehouse;

    public readonly int UnwashesAddedAmount;
    public readonly int UnwashesCleanedAmount;

    public OfflineCalculationResult(
        Dictionary<ProductConfig, int> soldFromShelfs,
        Dictionary<ProductConfig, int> soldFromWarehouse,
        int unwashesAddedAmount,
        int unwashesCleanedAmount)
    {
        SoldFromShelfs = soldFromShelfs;
        SoldFromWarehouse = soldFromWarehouse;
        UnwashesAddedAmount = unwashesAddedAmount;
        UnwashesCleanedAmount = unwashesCleanedAmount;
    }
}

public class AllFriendsShopsActionsModel
{
    public event Action<string, FriendShopActionData> ActionDataAmountChanged = delegate { };
    public event Action<string, FriendShopActionData> ActionDataCooldownTimestampChanged = delegate { };

    private readonly Dictionary<string, FriendShopActionsModel> _friendShopActionsModelById = new Dictionary<string, FriendShopActionsModel>();

    public AllFriendsShopsActionsModel()
    {
    }

    public AllFriendsShopsActionsModel(IEnumerable<FriendShopActionsModel> actionsModels)
        : this()
    {
        foreach (var actionsModel in actionsModels)
        {
            _friendShopActionsModelById[actionsModel.FriendId] = actionsModel;
            Subscribe(actionsModel);
        }
    }

    public IReadOnlyDictionary<string, FriendShopActionsModel> FriendShopActionsModelByUid => _friendShopActionsModelById;

    public FriendShopActionsModel GetFriendShopActionsModel(string friendId)
    {
        return _friendShopActionsModelById[friendId];
    }

    public void AddActionsModelForUid(string friendId)
    {
        var actionsModel = new FriendShopActionsModel(friendId);
        _friendShopActionsModelById[friendId] = actionsModel;
        Subscribe(actionsModel);
    }

    private void Subscribe(FriendShopActionsModel actionsModel)
    {
        actionsModel.ActionDataAmountChanged += OnActionDataAmountChanged;
        actionsModel.ActionDataCooldownTimestampChanged += OnActionDataCooldownTimestampChanged;
    }

    private void OnActionDataAmountChanged(string friendUid, FriendShopActionData actionData)
    {
        ActionDataAmountChanged(friendUid, actionData);
    }

    private void OnActionDataCooldownTimestampChanged(string friendUid, FriendShopActionData actionData)
    {
        ActionDataCooldownTimestampChanged(friendUid, actionData);
    }
}

public class FriendShopActionsModel
{
    public static readonly FriendShopActionId[] SupportedActions = new FriendShopActionId[] {
        FriendShopActionId.AddUnwash,
        FriendShopActionId.TakeProduct,
        FriendShopActionId.VisitBonus,
    };

    public event Action<string, FriendShopActionData> ActionDataAmountChanged = delegate { };
    public event Action<string, FriendShopActionData> ActionDataCooldownTimestampChanged = delegate { };

    public readonly string FriendId;

    private readonly Dictionary<FriendShopActionId, FriendShopActionData> _actionsById = new Dictionary<FriendShopActionId, FriendShopActionData>();

    public FriendShopActionsModel(string friendId)
    {
        FriendId = friendId;
    }

    public FriendShopActionsModel(string friendId, IEnumerable<FriendShopActionData> actionsData)
        : this(friendId)
    {
        foreach (var actionData in actionsData)
        {
            AddActionData(actionData);
        }
    }

    public bool AddActionData(FriendShopActionData friendShopActionData)
    {
        var friendShopActionId = friendShopActionData.ActionId;

        if (_actionsById.ContainsKey(friendShopActionId)
            || Array.IndexOf(SupportedActions, friendShopActionId) < 0) return false;

        _actionsById[friendShopActionId] = friendShopActionData;
        friendShopActionData.AmountChanged += OnActionDataAmountChanged;
        friendShopActionData.EndCooldownTimestampChanged += OnActionDataCooldownTimestampChanged;

        return true;
    }

    private void OnActionDataCooldownTimestampChanged(FriendShopActionData actionData)
    {
        ActionDataCooldownTimestampChanged(FriendId, actionData);
    }

    private void OnActionDataAmountChanged(FriendShopActionData actionData)
    {
        ActionDataAmountChanged(FriendId, actionData);
    }

    public IEnumerable<FriendShopActionData> ActionsData => _actionsById.Values;
    public IReadOnlyDictionary<FriendShopActionId, FriendShopActionData> ActionsById => _actionsById;
}

public class FriendShopActionData
{
    public event Action<FriendShopActionData> AmountChanged = delegate { };
    public event Action<FriendShopActionData> EndCooldownTimestampChanged = delegate { };

    public readonly FriendShopActionId ActionId;

    public FriendShopActionData(FriendShopActionId actionId, int restAmount, int endCooldownTimestamp = 0)
    {
        ActionId = actionId;
        RestAmount = restAmount;
        EndCooldownTimestamp = endCooldownTimestamp;
    }

    public int RestAmount { get; private set; }
    public int EndCooldownTimestamp { get; private set; }
    public int ActionIdInt => (int)ActionId;

    public void SetAmount(int amount)
    {
        RestAmount = amount;
        AmountChanged(this);
    }

    public void SetEndCooldownTime(int endCooldownTimestamp)
    {
        EndCooldownTimestamp = endCooldownTimestamp;
        EndCooldownTimestampChanged(this);
    }

    public bool IsAvailable(int targetTime)
    {
        return EndCooldownTimestamp <= targetTime && RestAmount > 0;
    }
}

public enum FriendShopActionId
{
    None = 0,
    TakeProduct = 1,
    AddUnwash = 2,
    VisitBonus = 3,
    AddProduct = 4,
}

public class ExternalActionsModel
{
    public event Action<ExternalActionModelBase> ActionAdded = delegate { };
    public event Action ActionsCleared = delegate { };

    private List<ExternalActionModelBase> _actions = new List<ExternalActionModelBase>();

    public ExternalActionsModel()
    {
    }

    public IReadOnlyList<ExternalActionModelBase> Actions => _actions;

    public void AddAction(ExternalActionModelBase externalActionModel)
    {
        var isCombined = false;
        foreach (var action in _actions)
        {
            if (action.CanCombine(externalActionModel))
            {
                action.Combine(externalActionModel);
                isCombined = true;
                break;
            }
        }

        if (isCombined == false)
        {
            _actions.Add(externalActionModel);
        }

        ActionAdded(externalActionModel);
    }

    public void Clear()
    {
        _actions.Clear();
        ActionsCleared();
    }
}

public abstract class ExternalActionModelBase
{
    public readonly string PerformerId;

    public ExternalActionModelBase(string performerId)
    {
        PerformerId = performerId;
    }

    public abstract FriendShopActionId ActionId { get; }

    public virtual bool CanCombine(ExternalActionModelBase another)
    {
        return another.PerformerId == PerformerId && another.ActionId == ActionId;
    }

    public abstract void Combine(ExternalActionModelBase another);
}

public class ExternalActionTakeProduct : ExternalActionModelBase
{
    public readonly Vector2Int Coords;
    public readonly ProductConfig ProductConfig;

    public int Amount;

    public ExternalActionTakeProduct(string performerId, Vector2Int coords, ProductConfig productConfig, int amount)
        : base(performerId)
    {
        Coords = coords;
        ProductConfig = productConfig;
        Amount = amount;
    }

    public override FriendShopActionId ActionId => FriendShopActionId.TakeProduct;

    public override bool CanCombine(ExternalActionModelBase another)
    {
        var anotherAction = another as ExternalActionTakeProduct;
        return base.CanCombine(another)
            && anotherAction.Coords == Coords
            && anotherAction.ProductConfig.Key == ProductConfig.Key;
    }

    public override void Combine(ExternalActionModelBase another)
    {
        Amount += (another as ExternalActionTakeProduct).Amount;
    }
}

public class ExternalActionAddProduct : ExternalActionModelBase
{
    public readonly Vector2Int Coords;
    public readonly int ShelfSlotIndex;
    public readonly ProductConfig ProductConfig;

    public int Amount;

    public ExternalActionAddProduct(string performerId, Vector2Int coords, int shelfSlotIndex, ProductConfig productConfig, int amount)
        : base(performerId)
    {
        Coords = coords;
        ShelfSlotIndex = shelfSlotIndex;
        ProductConfig = productConfig;
        Amount = amount;
    }

    public override FriendShopActionId ActionId => FriendShopActionId.AddProduct;

    public override bool CanCombine(ExternalActionModelBase another)
    {
        var anotherAction = another as ExternalActionAddProduct;
        return base.CanCombine(another)
            && anotherAction.Coords == Coords
            && anotherAction.ProductConfig.Key == ProductConfig.Key
            && anotherAction.ShelfSlotIndex == ShelfSlotIndex;
    }

    public override void Combine(ExternalActionModelBase another)
    {
        Amount += (another as ExternalActionAddProduct).Amount;
    }
}

public class ExternalActionAddUnwash : ExternalActionModelBase
{
    public readonly Vector2Int Coords;

    public ExternalActionAddUnwash(string performerId, Vector2Int coords)
        : base(performerId)
    {
        Coords = coords;
    }

    public override FriendShopActionId ActionId => FriendShopActionId.AddUnwash;

    public override bool CanCombine(ExternalActionModelBase another)
    {
        return base.CanCombine(another) && (another as ExternalActionAddUnwash).Coords == Coords;
    }

    public override void Combine(ExternalActionModelBase another)
    {
        //combine = skip
    }
}

public class UserSettingsModel
{
    public event Action MusicMutedStateChanged = delegate { };
    public event Action AudioMutedStateChanged = delegate { };

    public UserSettingsModel(bool isMusicMusted, bool isAudioMuted)
    {
        IsMusicMuted = isMusicMusted;
        IsAudioMuted = isAudioMuted;
    }

    public bool IsMusicMuted { get; private set; }
    public bool IsAudioMuted { get; private set; }

    public void SetMusicMutedState(bool isMuted)
    {
        IsMusicMuted = isMuted;
        MusicMutedStateChanged();
    }

    public void SetAudioMutedState(bool isMuted)
    {
        IsAudioMuted = isMuted;
        AudioMutedStateChanged();
    }
}
