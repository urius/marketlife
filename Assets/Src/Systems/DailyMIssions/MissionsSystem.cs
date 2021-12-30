using System.Collections.Generic;
using System.Linq;

public class MissionsSystem
{
    private const int MissionsCount = 3;

    private readonly GameStateModel _gameStateModel;
    private readonly PlayerModelHolder _playerModelHolder;
    private readonly GameConfigManager _configManager;
    private readonly System.Random _random;
    private readonly Dictionary<DailyMissionModel, DailyMissionProcessorBase> _missionProcessors = new Dictionary<DailyMissionModel, DailyMissionProcessorBase>();
    private readonly Dictionary<string, DailyMissionFactoryBase> _missionFactories = new Dictionary<string, DailyMissionFactoryBase>()
    {
        { MissionKeys.AddFriends, new DailyMissionAddFriendsFactory() },
        { MissionKeys.AddShelfs, new DailyMissionAddShelfsFactory() },
    };

    public MissionsSystem()
    {
        _gameStateModel = GameStateModel.Instance;
        _playerModelHolder = PlayerModelHolder.Instance;
        _configManager = GameConfigManager.Instance;
        _random = new System.Random();
    }

    public void Start()
    {
#if UNITY_EDITOR
        DebucCheckAllMissionProcessorsExists();
#endif
        _gameStateModel.GameStateChanged += OnGameStateChanged;
    }

    private void DebucCheckAllMissionProcessorsExists()
    {
        //todo implement
    }

    private void OnGameStateChanged(GameStateName prevState, GameStateName currentState)
    {
        if (currentState == GameStateName.ReadyForStart)
        {
            var dailyMissionsModel = _playerModelHolder.UserModel.DailyMissionsModel;
            if (IsNewDay())
            {
                dailyMissionsModel.Clear();
            }
            else
            {
                CreateMissionProcessors();
            }
        }
        else if (prevState == GameStateName.ReadyForStart
            && currentState == GameStateName.PlayerShopSimulation)
        {
            if (IsNewDay())
            {
                CreateMissionModels();
                CreateMissionProcessors();
            }
        }
    }

    private void CreateMissionProcessors()
    {
        foreach (var kvp in _missionProcessors)
        {
            kvp.Value.Stop();
        }
        _missionProcessors.Clear();

        foreach (var mission in _playerModelHolder.UserModel.DailyMissionsModel.MissionsList)
        {
            var processor = _missionFactories[mission.Key].CreateProcessor(mission);
            _missionProcessors[mission] = processor;
            processor.Start();
        }
    }

    private void CreateMissionModels()
    {
        var playerModel = _playerModelHolder.UserModel;
        var availableMissionsConfigs = _configManager.DailyMissionsConfig.GetMissionsForLevel(playerModel.ProgressModel.Level)
            .Where(CanAddMission)
            .ToList();
        var newMissions = new List<DailyMissionModel>(MissionsCount);

        var safetyCounter = 1000;
        while (
            newMissions.Count < MissionsCount
            && availableMissionsConfigs.Count > 0
            && safetyCounter > 0)
        {
            safetyCounter--;
            var maxFrequency = availableMissionsConfigs.Max(m => m.Frequency);
            var currentFrequencyThreshold = _random.Next(1, maxFrequency + 1);
            var randomIndex = _random.Next(0, availableMissionsConfigs.Count);
            var chosenMissionConfig = availableMissionsConfigs[randomIndex];
            if (chosenMissionConfig.Frequency >= currentFrequencyThreshold)
            {
                var mission = CreateMission(chosenMissionConfig);
                if (mission != null)
                {
                    newMissions.Add(mission);
                    availableMissionsConfigs.RemoveAt(randomIndex);
                }
            }
        }

        newMissions.ForEach(playerModel.DailyMissionsModel.AddMission);
    }

    private DailyMissionModel CreateMission(MissionConfig missionConfig)
    {
        var complexityMultiplier = UnityEngine.Random.Range(0f, 1f);
        var factory = _missionFactories[missionConfig.Key];
        return factory.CreateModel(complexityMultiplier);
    }

    private bool CanAddMission(MissionConfig missionConfig)
    {
        if (_missionFactories.ContainsKey(missionConfig.Key) == false) return false;
        var factory = _missionFactories[missionConfig.Key];
        return factory.CanAdd();

        /*
        switch (missionConfig.Key)
        {
            case MissionKeys.BuyGold:
                return dailyMissionsModel.MissionsList.Any(m => m.Key == MissionKeys.BuyMoney) == false;
            case MissionKeys.BuyMoney:
                return dailyMissionsModel.MissionsList.Any(m => m.Key == MissionKeys.BuyGold) == false;
            case MissionKeys.ChangeBillboard:
                return playerModel.ShopModel.BillboardModel.IsAvailable;
            case MissionKeys.ChangeCashman:
                return true;
            case MissionKeys.GiftToFriend:
            case MissionKeys.VisitAllFriends:
                return _friendsDataHolder.Friends.Any(f => f.IsApp && f.IsActive());
            case MissionKeys.RepaintAllFloors:
            case MissionKeys.RepaintAllWalls:
            case MissionKeys.SellProduct:
            case MissionKeys.TakeProfit:
                return true;
            case MissionKeys.ExpandShop:
                return _configManager.UpgradesConfig.GetNextUpgradeForValue(UpgradeType.ExpandX, playerModel.ShopModel.ShopDesign.SizeX) != null
                    || _configManager.UpgradesConfig.GetNextUpgradeForValue(UpgradeType.ExpandY, playerModel.ShopModel.ShopDesign.SizeY) != null;
            case MissionKeys.UpgradeWarehouse:
                return _configManager.UpgradesConfig.GetNextUpgradeForValue(UpgradeType.WarehouseSlots, playerModel.ShopModel.WarehouseModel.Size) != null
                    || _configManager.UpgradesConfig.GetNextUpgradeForValue(UpgradeType.WarehouseVolume, playerModel.ShopModel.WarehouseModel.Volume) != null;
            default:
                return true;
        }*/
    }

    private bool IsNewDay()
    {
        return DateTimeHelper.IsSameDays(_playerModelHolder.UserModel.StatsData.LastVisitTimestamp, _gameStateModel.ServerTime) == false;
    }
}