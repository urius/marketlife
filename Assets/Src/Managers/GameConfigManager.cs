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

        return new MainConfig(
            mainConfigDto.GameplayAtlasVersion,
            mainConfigDto.InterfaceAtlasVersion,
            mainConfigDto.AutoPlacePriceGold,
            productsConfig,
            shelfsConfig,
            shopObjectsConfig,
            floorsConfig,
            wallsConfig,
            windowsConfig,
            doorsConfig);
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

    private void OnEnable()
    {
        Instance = this;
    }
}