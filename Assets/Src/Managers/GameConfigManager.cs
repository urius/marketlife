using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfigManager", menuName = "Scriptable Objects/Managers/GameConfigManager")]
public class GameConfigManager : ScriptableObject
{
    public static GameConfigManager Instance { get; private set; }

    [SerializeField] private string _mainConfigUrl;

    public MainConfig MainConfig { get; private set; }

    public async UniTask<bool> LoadConfig()
    {
        var getConfigOperation = await new WebRequestsSender().GetAsync<MainConfig>(_mainConfigUrl);
        if (getConfigOperation.IsSuccess)
        {
            MainConfig = getConfigOperation.Result;
        }

        return getConfigOperation.IsSuccess;
    }

    private void OnEnable()
    {
        Instance = this;
    }
}

public class MainConfig
{
    public int GameplayAtlasVersion;
}
