using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UITopPanelMediator : MonoBehaviour
{
    [SerializeField] private UITopPanelBarView _crystalsBarView;
    [SerializeField] private UITopPanelBarView _cashBarView;
    [SerializeField] private UITopPanelBarWithProgressView _expBarView;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private UITopPanelBarWithProgressView _moodBarView;

    private GameStateModel _gameStateModel;
    private Dispatcher _dispatcher;
    private PlayerModelHolder _playerModelHolder;
    private LocalizationManager _loc;
    private SpritesProvider _spritesProvider;
    private UserProgressModel _playerProgressModel;
    private ShopModel _playerShopModel;

    private void Awake()
    {
        _gameStateModel = GameStateModel.Instance;
        _dispatcher = Dispatcher.Instance;
        _playerModelHolder = PlayerModelHolder.Instance;
        _loc = LocalizationManager.Instance;
        _spritesProvider = SpritesProvider.Instance;
    }

    public async void Start()
    {
        await _gameStateModel.GameDataLoadedTask;

        _playerProgressModel = _playerModelHolder.UserModel.ProgressModel;
        _playerShopModel = _playerModelHolder.UserModel.ShopModel;

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
        UpdateExp();
        SetLevel(progressModel.Level);
        UpdateMood();

        Activate();
    }

    private void UpdateMood(bool animated = false)
    {
        var moodPercent = (int)(_playerShopModel.MoodMultiplier * 100);
        if (animated)
        {
            _moodBarView.SetAmountAnimatedAsync(moodPercent);
            _moodBarView.SetProgressAnimated(_playerShopModel.MoodMultiplier);
        }
        else
        {
            _moodBarView.Amount = moodPercent;
            _moodBarView.SetProgress(_playerShopModel.MoodMultiplier);
        }

        if (moodPercent >= 70)
        {
            _moodBarView.SetProgressBarImageSprite(_spritesProvider.GetTopStribeRed());
        }
        else if (moodPercent <= 30)
        {
            _moodBarView.SetProgressBarImageSprite(_spritesProvider.GetTopStribeRed());
        }
        else
        {
            _moodBarView.SetProgressBarImageSprite(_spritesProvider.GetTopStribeYellow());
        }
    }

    private void UpdateExp(bool animated = false)
    {
        var expAmount = _playerProgressModel.ExpAmount;
        var levelProgress = _playerProgressModel.LevelProgress;
        var progresPercent = (int)(_playerProgressModel.LevelProgress * 100);
        var restExpForLevelup = _playerProgressModel.NextLevelMinExp - expAmount;
        _expBarView.HintableView.DisplayText = string.Format(_loc.GetLocalization(LocalizationKeys.HintTopPanelExpFomat), progresPercent, restExpForLevelup);
        if (animated)
        {
            _expBarView.SetAmountAnimatedAsync(expAmount);
            _expBarView.SetProgressAnimated(levelProgress);
        }
        else
        {
            _expBarView.Amount = expAmount;
            _expBarView.SetProgress(levelProgress);
        }
    }

    private void SetLevel(int level)
    {
        _levelText.text = level.ToString();
    }

    private void Activate()
    {
        _dispatcher.UIRequestBlinkMoney += OnUIRequestBlinkMoney;

        _playerProgressModel.CashChanged += OnCashChanged;
        _playerProgressModel.GoldChanged += OnGoldChanged;
        _playerProgressModel.ExpChanged += OnExpChanged;
        _playerProgressModel.LevelChanged += OnLevelChanged;
        _playerShopModel.MoodChanged += OnMoodChanged;
    }

    private void OnMoodChanged(float delta)
    {
        UpdateMood(animated: true);
    }

    private void OnLevelChanged(int prevValue, int currentValue)
    {
        SetLevel(currentValue);
    }

    private void OnExpChanged(int previousValue, int currentValue)
    {
        UpdateExp(animated: true);
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
