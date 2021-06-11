using System;
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
    private ShopModel _playerShopModel;

    private void Awake()
    {
        _gameStateModel = GameStateModel.Instance;
        _dispatcher = Dispatcher.Instance;
    }

    public void Start()
    {
        if (_gameStateModel.PlayerShopModel != null)
        {
            Initialize();
        }
        else
        {
            _gameStateModel.PlayerShopModelWasSet += OnPlayerShopMpdelSet;
        }
    }

    private void OnPlayerShopMpdelSet()
    {
        _gameStateModel.PlayerShopModelWasSet -= OnPlayerShopMpdelSet;
        Initialize();
    }

    private void Initialize()
    {
        _playerShopModel = _gameStateModel.PlayerShopModel;
        var progressModel = _playerShopModel.ProgressModel;

        _crystalsBarView.Amount = progressModel.Gold;
        _cashBarView.Amount = progressModel.Cash;
        _expBarView.Amount = progressModel.ExpAmount;
        //_moodBarView.Amount = progressModel; TODO: calculate mood

        Activate();
    }

    private void Activate()
    {
        _dispatcher.UIRequestBlinkMoney += OnUIRequestBlinkMoney;

        _playerShopModel.ProgressModel.CashChanged += OnCashChanged;
        _playerShopModel.ProgressModel.GoldChanged += OnGoldChanged;
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
