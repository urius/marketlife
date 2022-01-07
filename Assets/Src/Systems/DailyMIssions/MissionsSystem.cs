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
    private readonly Dictionary<string, IDailyMissionFactory> _missionFactories = new Dictionary<string, IDailyMissionFactory>()
    {
        { MissionKeys.AddFriends, new DailyMissionAddFriendsFactory() },
        { MissionKeys.AddShelfs, new DailyMissionAddShelfsFactory() },
        { MissionKeys.AddGold, new DailyMissionAddGoldFactory() },
        { MissionKeys.AddCash, new DailyMissionAddCashFactory() },
        { MissionKeys.ChangeBillboard, new DailyMissionChangeBillboardFactory() },
        { MissionKeys.ChangeCashman, new DailyMissionChangeCashmanFactory() },
        { MissionKeys.GiftToFriend, new DailyMissionGiftToFriendFactory() },
        { MissionKeys.RepaintFloors, new DailyMissionRepaintFloorsFactory() },
        { MissionKeys.RepaintWalls, new DailyMissionRepaintWallsFactory() },
        { MissionKeys.SellProduct, new DailyMissionSellProductFactory() },
        { MissionKeys.CleanUnwashes, new DailyMissionCleanUnwashesFactory() },
        { MissionKeys.VisitFriends, new DailyMissionVisitFriendsFactory() },
        { MissionKeys.ExpandShop, new DailyMissionExpandShopFactory() },
        { MissionKeys.UpgradeWarehouseVolume, new DailyMissionUpgradeWarehouseVolumeFactory() },
        { MissionKeys.AddWarehouseCells, new DailyMissionAddWarehouseCellsFactory() },
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
        _gameStateModel.GameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameStateName prevState, GameStateName currentState)
    {
        if (currentState == GameStateName.ReadyForStart)
        {
            var dailyMissionsModel = _playerModelHolder.UserModel.DailyMissionsModel;
            if (IsNewDay())
            {
                dailyMissionsModel.Clear();
                CreateMissionModels();
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
            var missionConfigIndex = _random.Next(0, availableMissionsConfigs.Count);
            var chosenMissionConfig = availableMissionsConfigs[missionConfigIndex];
            if (chosenMissionConfig.Frequency >= currentFrequencyThreshold)
            {
                var mission = CreateMission(chosenMissionConfig);
                if (mission != null)
                {
                    newMissions.Add(mission);
                    if (_missionFactories[chosenMissionConfig.Key].IsMultipleAllowed == false)
                    {
                        availableMissionsConfigs.RemoveAt(missionConfigIndex);
                    }
                }
            }
        }

        newMissions.ForEach(AddMission);
    }

    private void AddMission(DailyMissionModel mission)
    {
        _playerModelHolder.UserModel.DailyMissionsModel.AddMission(mission);
        ActivateMission(mission);
    }

    private void ActivateMission(DailyMissionModel mission)
    {
        mission.ValueChanged += OnMissionValueChanged;
    }

    private void OnMissionValueChanged()
    {
        CheckCompletedProcessors();
    }

    private void CheckCompletedProcessors()
    {
        var completedMissions = _missionProcessors
            .Where(kvp => kvp.Key.IsCompleted)
            .Select(kvp => kvp.Key);
        foreach (var completedMission in completedMissions)
        {
            if (_missionProcessors.ContainsKey(completedMission))
            {
                _missionProcessors[completedMission].Stop();
                _missionProcessors.Remove(completedMission);
            }
        }
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
    }

    private bool IsNewDay()
    {
        return DateTimeHelper.IsSameDays(_playerModelHolder.UserModel.StatsData.LastVisitTimestamp, _gameStateModel.ServerTime) == false;
    }
}