using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MissionsSystem
{
    private const int MissionsCount = 4;

    private readonly GameStateModel _gameStateModel;
    private readonly PlayerModelHolder _playerModelHolder;
    private readonly GameConfigManager _configManager;
    private readonly System.Random _random;
    private readonly Dispatcher _dispatcher;
    private readonly ScreenCalculator _screenCalculator;
    private readonly UpdatesProvider _updatesProvider;
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

    private int _endOfServerDayTimestamp;

    public MissionsSystem()
    {
        _gameStateModel = GameStateModel.Instance;
        _playerModelHolder = PlayerModelHolder.Instance;
        _configManager = GameConfigManager.Instance;
        _random = new System.Random();
        _dispatcher = Dispatcher.Instance;
        _screenCalculator = ScreenCalculator.Instance;
        _updatesProvider = UpdatesProvider.Instance;
    }

    public void Start()
    {
        WaitUserModelAndInitialize().Forget();
    }

    private async UniTaskVoid WaitUserModelAndInitialize()
    {
        await _playerModelHolder.SetUserModelTask;
        
        UpdateEndOfServerDayTimestamp();
        UpdateEndMissionTimeOffline();
            
        _gameStateModel.GameStateChanged += OnGameStateChanged;
        _dispatcher.UITakeDailyMissionRewardClicked += OnUITakeDailyMissionRewardClicked;
        _updatesProvider.RealtimeSecondUpdate += OnRealtimeSecondUpdate;
    }

    private void UpdateEndMissionTimeOffline()
    {
        var prevVisitDate = DateTimeHelper.GetDateTimeByUnixTimestamp(_playerModelHolder.UserModel.StatsData.LastVisitTimestamp);
        var currentVisitDate = DateTimeHelper.GetDateTimeByUnixTimestamp(_gameStateModel.StartGameServerTime);

        if (DateTimeHelper.IsSameDays(prevVisitDate, currentVisitDate) == false)
        {
            ClearMissions();
        }
    }

    private void UpdateEndOfServerDayTimestamp()
    {
        var serverTime = _gameStateModel.ServerTime;
        var serverUnixDate = DateTimeHelper.GetDateTimeByUnixTimestamp(serverTime);
        _endOfServerDayTimestamp = serverTime + DateTimeHelper.GetSecondsLeftForTheEndOfTheDay(serverUnixDate);
    }

    private void OnRealtimeSecondUpdate()
    {
        UpdateEndMissionTimeOnline();
    }

    private void UpdateEndMissionTimeOnline()
    {
        var secondToEndOfTheServerDay = _endOfServerDayTimestamp - _gameStateModel.ServerTime;
        var secondsLeft = Mathf.Max(0, secondToEndOfTheServerDay);
        if (secondsLeft <= 0)
        {
            UpdateEndOfServerDayTimestamp();
            ClearMissions();
        }
        _dispatcher.DailyMissionsSecondsLeftAmountUpdated(secondsLeft);
    }

    private void ClearMissions()
    {
        var activeMissions = _missionProcessors.Keys.ToArray();
        foreach (var mission in activeMissions)
        {
            RemoveProcessorForMission(mission);
        }

        var dailyMissionsModel = _playerModelHolder.UserModel.DailyMissionsModel;
        dailyMissionsModel.Clear();
    }

    private void OnUITakeDailyMissionRewardClicked(DailyMissionModel missionModel, Vector3 rewardIconWorldPosition)
    {
        if (missionModel.IsCompleted
            && missionModel.IsRewardTaken == false)
        {
            var screenPoint = _screenCalculator.WorldToScreenPoint(rewardIconWorldPosition);
            var missionReward = missionModel.Reward;
            switch (missionReward.Type)
            {
                case RewardType.Cash:
                    _dispatcher.UIRequestAddCashFlyAnimation(screenPoint, Mathf.Clamp((int)(missionReward.Amount * 0.002), 1, 10));
                    _playerModelHolder.UserModel.AddCash(missionReward.Amount);
                    break;
                case RewardType.Gold:
                    _dispatcher.UIRequestAddGoldFlyAnimation(screenPoint, Mathf.Clamp((int)(missionReward.Amount * 1), 1, 10));
                    _playerModelHolder.UserModel.AddGold(missionReward.Amount);
                    break;
                case RewardType.Exp:
                    _dispatcher.UIRequestAddExpFlyAnimation(screenPoint, Mathf.Clamp((int)(missionReward.Amount * 0.02), 1, 10));
                    _playerModelHolder.UserModel.AddExp(missionReward.Amount);
                    break;
            }

            missionModel.SetRewardTaken();
        }
    }

    private void OnGameStateChanged(GameStateName prevState, GameStateName currentState)
    {
        if (currentState == GameStateName.ReadyForStart)
        {
            var isNewDay = IsNewDay();
            var dailyMissionsModel = _playerModelHolder.UserModel.DailyMissionsModel;
            if (isNewDay || dailyMissionsModel.MissionsList.Count <= 0)
            {
                dailyMissionsModel.Clear();
                CreateMissionModels();
            }
            CreateMissionProcessors();
            if (isNewDay == false)
            {
                StartCreatedProcessors();
            }
        }
        else if (prevState == GameStateName.ReadyForStart
            && currentState == GameStateName.PlayerShopSimulation)
        {
            if (IsNewDay())
            {
                StartCreatedProcessors();
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
            if (mission.IsCompleted == false)
            {
                var processor = _missionFactories[mission.Key].CreateProcessor(mission);
                _missionProcessors[mission] = processor;
            }
        }
    }

    private void StartCreatedProcessors()
    {
        foreach (var kvp in _missionProcessors)
        {
            kvp.Value.Start();
        }
    }

    private void CreateMissionModels()
    {
        var playerModel = _playerModelHolder.UserModel;
        var missionsModel = playerModel.DailyMissionsModel;
        var availableMissionsConfigs = _configManager.DailyMissionsConfig.GetMissionsForLevel(playerModel.ProgressModel.Level)
            .ToList();

        var safetyCounter = 1000;
        while (
            missionsModel.MissionsList.Count < MissionsCount
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
                var needToRemoveMissionConfig = false;
                if (CanAddMission(chosenMissionConfig))
                {
                    var mission = CreateMission(chosenMissionConfig);
                    if (mission != null)
                    {
                        AddMission(mission);
                    }
                    else
                    {
                        needToRemoveMissionConfig = true;
                    }
                }
                else
                {
                    needToRemoveMissionConfig = true;
                }

                if (needToRemoveMissionConfig)
                {
                    availableMissionsConfigs.RemoveAt(missionConfigIndex);
                }
            }
        }
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
            .Select(kvp => kvp.Key)
            .ToArray();
        foreach (var completedMission in completedMissions)
        {
            RemoveProcessorForMission(completedMission);
        }
    }

    private void RemoveProcessorForMission(DailyMissionModel mission)
    {
        if (_missionProcessors.ContainsKey(mission))
        {
            _missionProcessors[mission].Stop();
            _missionProcessors.Remove(mission);
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