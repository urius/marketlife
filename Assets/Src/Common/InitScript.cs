using UnityEngine;

public class InitScript : MonoBehaviour
{
    [SerializeField] private string _debugUid;
    [Space(20)]
    [SerializeField] private AssetBundlesLoader _assetBundlesLoader;
    [SerializeField] private PrefabsHolder _prefabsHolder;
    [SerializeField] private GraphicsManager _graphicsManager;
    [SerializeField] private GameConfigManager _gameConfigManager;

    private PlayerModel _playerModel;

    private void Awake()
    {
        _playerModel = PlayerModel.Instance;
    }

    private async void Start()
    {
        _playerModel.SetUid(_debugUid);

        await new LoadConfigsCommand().ExecuteAsync();
        await new LoadGraphicsCommand().ExecuteAsync();

        await new LoadPlayerShopCommand().ExecuteAsync();

        _playerModel.SetViewingShopModel(_playerModel.ShopModel);
    }
}
