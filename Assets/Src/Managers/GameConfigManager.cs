using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfigManager", menuName = "Scriptable Objects/Managers/GameConfigManager")]
public class GameConfigManager : ScriptableObject
{
    public static GameConfigManager Instance { get; private set; }

    [SerializeField] private string _mainConfigUrl;

    public MainConfig MainConfig { get; private set; }
    public IProductsConfig ProductsConfig => MainConfig;
    public IShelfsConfig ShelfsConfig => MainConfig;
    public IFloorsConfig FloorsConfig => MainConfig;
    public IWallsConfig WallsConfig => MainConfig;
    public IWindowsConfig WindowsConfig => MainConfig;
    public IDoorsConfig DoorsConfig => MainConfig;
    public IPersonalConfig PersonalConfig => MainConfig;
    public IUpgradesConfig UpgradesConfig => MainConfig;
    public ILevelsConfig LevelsConfig => MainConfig;
    public IDailyBonusConfig DailyBonusConfig => MainConfig;

    public async UniTask<bool> LoadConfigAsync()
    {
        var getConfigOperation = await new WebRequestsSender().GetAsync(_mainConfigUrl);
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

        return new MainConfig(
            mainConfigDto,
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
            dailyBonusConfig);
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
        var result = new Dictionary<string, ProductConfig>();
        foreach (var kvp in configsDtosDictionary)
        {
            var numericId = int.Parse(kvp.Key.Split('p')[1]);
            result[kvp.Key] = new ProductConfig(numericId, kvp.Value);
        }

        return result;
    }

    private Dictionary<string, PersonalConfig> ToPersonalConfigs(Dictionary<string, PersonalConfigDto> configsDtosDictionary)
    {
        var result = new Dictionary<string, PersonalConfig>();
        foreach (var kvp in configsDtosDictionary)
        {
            result[kvp.Key] = new PersonalConfig(kvp.Key, kvp.Value);
        }

        return result;
    }

    private void OnEnable()
    {
        Instance = this;
    }
}