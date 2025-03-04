using Cysharp.Threading.Tasks;
using Src.Common;
using UnityEngine;

public class InitScript : MonoBehaviour
{
    [SerializeField] private string _debugUid;
    [SerializeField] private bool _disableSave;
    [SerializeField] private bool _disableTutorial;
    [SerializeField] private bool _loadDebugConfigVersion;
    [Space(20)]
    [SerializeField] private PrefabsHolder _prefabsHolder;
    [SerializeField] private GraphicsManager _graphicsManager;
    [SerializeField] private LocalizationManager _localizationManager;
    [SerializeField] private ColorsHolder _colorsHolder;

    private PlayerModelHolder _playerModelHolder;

    private void Awake()
    {
        Application.targetFrameRate = 51;

        _playerModelHolder = PlayerModelHolder.Instance;
        new InitializeSystemsCommand().Execute();

        Debug.Log($"Application.absoluteURL: {Application.absoluteURL}");
    }

    private async void Start()
    {
        ExecuteAdditionalStartupLogic();

        await _playerModelHolder.SetUidTask;
        await new InitializeAndLoadCommand().ExecuteAsync();
    }

    private void ExecuteAdditionalStartupLogic()
    {
        if (MirraSdkWrapper.IsVk == false)
        {
            InitViaMirraSdk().Forget();
            return;
        }
        
#if UNITY_EDITOR || UNITY_ANDROID
        new SetupExternalDebugDataCommand().Execute(_debugUid);
        new SetupDebugSystemsCommand().Execute();
#endif
    }

    private async UniTaskVoid InitViaMirraSdk()
    {
        MirraSdkWrapper.Log("InitViaMirraSdk");

        var playerId = await MirraSdkWrapper.GetPlayerId();

        MirraSdkWrapper.Log("PlayerId: " + playerId);

        if (MirraSdkWrapper.IsYandexGames)
        {
            Urls.UpdateBasePathPostfix("/marketYG");
            _playerModelHolder.SetInitialData(playerId, SocialType.YG);
        }
    }

    private void OnValidate()
    {
        DebugDataHolder.Instance.IsSaveDisabled = _disableSave;
        DebugDataHolder.Instance.UseTestConfigFile = _loadDebugConfigVersion;
        DebugDataHolder.Instance.IsTutorialDisabled = _disableTutorial;
    }
}
