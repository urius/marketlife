using Cysharp.Threading.Tasks;
using UnityEngine;

public class UITopPanelMediator : MonoBehaviour
{
    [SerializeField] private UITopPanelBarView _crystalsBarView;
    [SerializeField] private UITopPanelBarView _cashBarView;
    [SerializeField] private UITopPanelBarView _expBarView;
    [SerializeField] private UITopPanelBarView _moodBarView;

    private GameStateModel _gameStateModel;
    private Dispatcher _dispatcher;
    private PlayerModelHolder _playerModelHolder;
    private UserProgressModel _playerProgressModel;

    private void Awake()
    {
        _gameStateModel = GameStateModel.Instance;
        _dispatcher = Dispatcher.Instance;
        _playerModelHolder = PlayerModelHolder.Instance;
    }

    public async void Start()
    {
        await _gameStateModel.GameDataLoadedTask;

        _playerProgressModel = _playerModelHolder.UserModel.ProgressModel;

        Initialize();
    }

    private void OnPlayerShopModelSet()
    {
        _gameStateModel.PlayerShopModelWasSet -= OnPlayerShopModelSet;
        Initialize();
    }

    private void Initialize()
    {
        var progressModel = _playerProgressModel;

        _crystalsBarView.Amount = progressModel.Gold;
        _cashBarView.Amount = progressModel.Cash;
        _expBarView.Amount = progressModel.ExpAmount;
        //_moodBarView.Amount = progressModel; TODO: calculate mood

        Activate();
    }

    private void Activate()
    {
        _dispatcher.UIRequestBlinkMoney += OnUIRequestBlinkMoney;

        _playerProgressModel.CashChanged += OnCashChanged;
        _playerProgressModel.GoldChanged += OnGoldChanged;
    }

    private void OnCashChanged(int previousValue, int currentValue)
    {
        _cashBarView.SetAmountAnimatedAsync(currentValue);
    }

    private void OnGoldChanged(int previousValue, int currentValue)
    {
        _crystalsBarView.SetAmountAnimatedAsync(currentValue);
    }

    private async void OnUIRequestBlinkMoney(bool isGold)
    {
        if (isGold)
        {
            await _crystalsBarView.BlinkAmountAsync();
        }
        else
        {
            await _cashBarView.BlinkAmountAsync();
        }
    }

    private UniTask ShowNewLevelAnimationAsync()
    {
        var tcs = new UniTaskCompletionSource();
        var psPrefab = PrefabsHolder.Instance.GetRemotePrefab(PrefabsHolder.PSStarsName);
        var psGo = GameObject.Instantiate(psPrefab, _expBarView.transform);
        var ps = psGo.GetComponent<ParticleSystem>();
        LeanTween.delayedCall(ps.main.duration, async () =>
        {
            await _expBarView.JumpAndSaltoIconAsync();
            tcs.TrySetResult();
        });
        LeanTween.delayedCall(2 * ps.main.duration, () =>
        {
            GameObject.Destroy(psGo);
        });

        return tcs.Task;
    }
}
