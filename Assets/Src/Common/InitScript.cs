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
        _playerModelHolder.UidIsSet += OnUidIsSet;
    }

#if UNITY_EDITOR
    private void Start()
    {
        new SetupExternalDebugDataCommand().Execute(_debugUid);
    }
#endif

    private async void OnUidIsSet()
    {
        _playerModelHolder.UidIsSet -= OnUidIsSet;

        await new InitializeAndLoadCommand().ExecuteAsync();
    }
}
