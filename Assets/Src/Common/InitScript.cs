using UnityEngine;

public class InitScript : MonoBehaviour
{
    [SerializeField] private string _debugUid;
    [Space(20)]
    [SerializeField] private AssetBundlesLoader _assetBundlesLoader;
    [SerializeField] private PrefabsHolder _prefabsHolder;
    [SerializeField] private GraphicsManager _graphicsManager;
    [SerializeField] private GameConfigManager _gameConfigManager;
    [SerializeField] private LocalizationManager _localizationManager;

    private PlayerModel _playerModel;

    private void Awake()
    {
        Application.targetFrameRate = 50;

        _playerModel = PlayerModel.Instance;
    }

    private async void Start()
    {
        _playerModel.SetUid(_debugUid);

        await new InitializeAndLoadCommand().ExecuteAsync();
    }
}
