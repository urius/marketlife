using UnityEngine;

public class InitScript : MonoBehaviour
{
    [SerializeField] private string _debugUid;
    [Space(20)]
    [SerializeField] private AssetBundlesLoader _assetBundlesLoader;
    [SerializeField] private PrefabsHolder _prefabsHolder;
    [SerializeField] private URLsHolder _urlsHolder;
    [SerializeField] private GraphicsManager _graphicsManager;
    [SerializeField] private GameConfigManager _gameConfigManager;
    [SerializeField] private LocalizationManager _localizationManager;

    private PlayerModelHolder _playerModelHolder;

    private void Awake()
    {
        //Application.targetFrameRate = 50;

        _playerModelHolder = PlayerModelHolder.Instance;
    }

    private async void Start()
    {
        _playerModelHolder.SetUid(_debugUid);

        await new InitializeAndLoadCommand().ExecuteAsync();
    }
}
