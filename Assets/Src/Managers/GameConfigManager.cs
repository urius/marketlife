using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Random = System.Random;

public class GameConfigManager
{
    public static GameConfigManager Instance = new GameConfigManager();

    public MainConfig MainConfig { get; private set; }
    public IProductsConfig ProductsConfig => MainConfig;
    public IShelfsConfig ShelfsConfig => MainConfig;
    public IFloorsConfig FloorsConfig => MainConfig;
    public IWallsConfig WallsConfig => MainConfig;
    public IWindowsConfig WindowsConfig => MainConfig;
    public IDoorsConfig DoorsConfig => MainConfig;
    public IPersonalsConfig PersonalsConfig => MainConfig;
    public IUpgradesConfig UpgradesConfig => MainConfig;
    public ILevelsConfig LevelsConfig => MainConfig;
    public IDailyBonusConfig DailyBonusConfig => MainConfig;
    public IFriendActionsConfig FriendActionsConfig => MainConfig;
    public IMissionsConfig DailyMissionsConfig => MainConfig;
    public IAdvertConfig AdvertConfig => MainConfig;

    public async UniTask<bool> LoadMainConfigAsync(string mainConfigABPostfix)
    {
        var urlsHolder = URLsHolder.Instance;
        var url = string.Format(urlsHolder.MainConfigUrlFormat, mainConfigABPostfix);
#if UNITY_EDITOR
        if (DebugDataHolder.Instance.UseTestConfigFile == true) url = urlsHolder.DebugMainConfigUrl;
#endif
        var getConfigOperation = await new WebRequestsSender().GetAsync(URLHelper.AddAntiCachePostfix(url));
        if (getConfigOperation.IsSuccess)
        {
            var mainConfigDto = JsonConvert.DeserializeObject<MainConfigDto>(getConfigOperation.Result);
            MainConfig = ConvertToMainConfig(mainConfigDto);
        }

        return getConfigOperation.IsSuccess;
    }

    private MainConfig ConvertToMainConfig(MainConfigDto mainConfigDto)
    {
        var shelfsConfig = ConvertToConfigs(mainConfigDto.ShelfsConfig);
        var shopObjectsConfig = ConvertToConfigs(mainConfigDto.ShopObjectsConfig);
        var floorsConfig = ConvertToConfigs(mainConfigDto.FloorsConfig);
        var wallsConfig = ConvertToConfigs(mainConfigDto.WallsConfig);
        var windowsConfig = ConvertToConfigs(mainConfigDto.WindowsConfig);
        var doorsConfig = ConvertToConfigs(mainConfigDto.DoorsConfig);
        var productsConfig = ToProductsConfigs(mainConfigDto.ProductsConfig);
        var personalConfig = ToPersonalConfigs(mainConfigDto.PersonalConfig);
        var levelsConfig = ToLevelsConfig(mainConfigDto.LevelsConfig);
        var dailyBonusConfig = ToDailyBonusConfig(mainConfigDto.DailyBonusConfig);
        var friendActionsConfig = ToFriendActionsConfig(mainConfigDto.FriendActionsConfig);
        var missionsConfig = ToMissionsConfig(mainConfigDto.MissionsConfig);

        return new MainConfig(
            mainConfigDto,
            friendActionsConfig,
            productsConfig,
            shelfsConfig,
            shopObjectsConfig,
            floorsConfig,
            wallsConfig,
            windowsConfig,
            doorsConfig,
            personalConfig,
            ToUpgradeConfigs(UpgradeType.WarehouseVolume, mainConfigDto.WarehouseVolumeUpgradesConfig),
            ToUpgradeConfigs(UpgradeType.WarehouseSlots, mainConfigDto.WarehouseSlotsUpgradesConfig),
            ToUpgradeConfigs(UpgradeType.ExpandX, mainConfigDto.ExtendShopXUpgradesConfig),
            ToUpgradeConfigs(UpgradeType.ExpandY, mainConfigDto.ExtendShopYUpgradesConfig),
            levelsConfig,
            dailyBonusConfig,
            missionsConfig);
    }

    private MissionConfig[] ToMissionsConfig(List<MissionConfigDto> missionsConfigRaw)
    {
        var result = new MissionConfig[missionsConfigRaw.Count];
        var i = 0;
        foreach (var configDto in missionsConfigRaw)
        {
            var rewardConfigs = ToMissionRewardConfigs(configDto.reward);
            var maxComplexityFactor = configDto.max_complexity_factor <= 0 ? 1 : configDto.max_complexity_factor;
            var missionConfig = new MissionConfig(configDto.key, configDto.unlock_level, configDto.frequency, rewardConfigs, maxComplexityFactor);
            result[i] = missionConfig;
            i++;
        }

        return result;
    }

    private MissionRewardConfig[] ToMissionRewardConfigs(string rewardConfigRaw)
    {
        var splittedRewardsRaw = rewardConfigRaw.Split(',');

        var result = new MissionRewardConfig[splittedRewardsRaw.Length];
        var i = 0;
        foreach (var rewardConfigStr in splittedRewardsRaw)
        {
            var splitted1 = rewardConfigStr.Split('*');
            var levelMultiplier = 0;
            if (splitted1.Length > 1)
            {
                if (splitted1[1].IndexOf("lvl") >= 0)
                {
                    levelMultiplier = 1;
                }
            }
            var splittedMinMax = splitted1[0].Split('-');
            Reward minReward;
            Reward maxReward;
            if (splittedMinMax.Length > 1)
            {
                maxReward = Reward.FromString(splittedMinMax[1]);
                minReward = new Reward()
                {
                    Type = maxReward.Type,
                    Amount = Reward.FromString(splittedMinMax[0]).Amount,
                };
            }
            else
            {
                minReward = maxReward = Reward.FromString(splittedMinMax[0]);
            }
            result[i] = new MissionRewardConfig(minReward, maxReward, levelMultiplier);
            i++;
        }
        return result;
    }

    private FriendActionConfig[] ToFriendActionsConfig(Dictionary<string, FriendActionConfigDto> friendActionsConfig)
    {
        var result = new FriendActionConfig[friendActionsConfig.Count];
        var i = 0;
        foreach (var kvp in friendActionsConfig)
        {
            result[i] = new FriendActionConfig(kvp.Key, kvp.Value);
            i++;
        }

        return result;
    }

    private DailyBonusConfig[] ToDailyBonusConfig(List<string> dailyBonusConfigDto)
    {
        var result = new DailyBonusConfig[dailyBonusConfigDto.Count];
        for (var i = 0; i < dailyBonusConfigDto.Count; i++)
        {
            var configStr = dailyBonusConfigDto[i];
            var dayNum = i + 1;
            var reward = Price.FromString(configStr);
            result[i] = new DailyBonusConfig
            {
                DayNum = dayNum,
                Reward = reward,
            };
        }

        return result;
    }

    private float[] ToLevelsConfig(Dictionary<string, float> levelsConfig)
    {
        var result = new float[levelsConfig.Count + 1];
        result[1] = 0;
        foreach (var kvp in levelsConfig)
        {
            if (kvp.Key == Constants.LevelsMultiplier)
            {
                result[0] = kvp.Value;
            }
            else
            {
                var index = int.Parse(kvp.Key.Split('_')[1]);
                result[index] = kvp.Value;
            }
        }
        return result;
    }

    private UpgradeConfig[] ToUpgradeConfigs(UpgradeType upgradeType, IList<UpgradeConfigDto> upgradesConfigsDto)
    {
        var result = new UpgradeConfig[upgradesConfigsDto.Count];
        for (var i = 0; i < upgradesConfigsDto.Count; i++)
        {
            result[i] = new UpgradeConfig(upgradeType, upgradesConfigsDto[i]);
        }
        return result;
    }

    private Dictionary<string, ItemConfig<T>> ConvertToConfigs<T>(Dictionary<string, T> configsDtosDictionary)
        where T : PlaceableItemConfigDto
    {
        var result = new Dictionary<string, ItemConfig<T>>();
        foreach (var kvp in configsDtosDictionary)
        {
            var numericId = int.Parse(kvp.Key.Split('_')[1]);
            result[kvp.Key] = new ItemConfig<T>(numericId, kvp.Value);
        }

        return result;
    }

    private Dictionary<string, ProductConfig> ToProductsConfigs(Dictionary<string, ProductConfigDto> configsDtosDictionary)
    {
        var dayOfYear = DateTime.Now.DayOfYear;
        var rnd = new Random(dayOfYear);
        
        var result = new Dictionary<string, ProductConfig>();
        var sortedConfigs = configsDtosDictionary
            .OrderBy(kvp => int.Parse(kvp.Key[1..]));
        
        foreach (var kvp in sortedConfigs)
        {
            var numericId = int.Parse(kvp.Key.Split('p')[1]);
            var demandMultiplier = (float)rnd.NextDouble();
            var deliverMultiplier = 0.2f + (float)rnd.NextDouble() * 3;
            result[kvp.Key] = new ProductConfig(numericId, kvp.Value, demandMultiplier, deliverMultiplier);
        }

        return result;
    }

    private Dictionary<string, PersonalConfig> ToPersonalConfigs(Dictionary<string, PersonalConfigDto> configsDtosDictionary)
    {
        var result = new Dictionary<string, PersonalConfig>();
        foreach (var kvp in configsDtosDictionary)
        {
            var personalType = PersonalConfig.GetPersonalTypeByIdStr(kvp.Key);
            switch (personalType)
            {
                case PersonalType.Merchandiser:
                    result[kvp.Key] = new MerchandiserPersonalConfig(kvp.Key, kvp.Value);
                    break;
                default:
                    result[kvp.Key] = new PersonalConfig(kvp.Key, kvp.Value);
                    break;
            }
        }

        return result;
    }
}