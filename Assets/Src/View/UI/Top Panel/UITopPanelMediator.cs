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
    private AudioManager _audioManager;
    private UserProgressModel _playerProgressModel;
    private ShopModel _playerShopModel;
    private bool _isLevelUpInProgress;

    private void Awake()
    {
        _gameStateModel = GameStateModel.Instance;
        _dispatcher = Dispatcher.Instance;
        _playerModelHolder = PlayerModelHolder.Instance;
        _loc = LocalizationManager.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _audioManager = AudioManager.Instance;

    }

    public async void Start()
    {
        await _gameStateModel.GameDataLoadedTask;

        _playerProgressModel = _playerModelHolder.UserModel.ProgressModel;
        _playerShopModel = _playerModelHolder.UserModel.ShopModel;

        Initialize();
    }

    private void Initialize()
    {
        SetLevel(_playerProgressModel.Level);
        UpdateValues();

        Activate();
    }

    private void UpdateValues()
    {
        _crystalsBarView.Amount = _playerProgressModel.Gold;
        _cashBarView.Amount = _playerProgressModel.Cash;
        UpdateExp();
        UpdateMood();
    }

    private void UpdateMood(bool animated = false)
    {
        UpdateMoodHint();
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
            _moodBarView.SetProgressBarImageSprite(_spritesProvider.GetTopStribeGreen());
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

    private void UpdateMoodHint()
    {
        _moodBarView.HintableView.DisplayText = string.Format(_loc.GetLocalization(LocalizationKeys.HintTopPanelMoodFormat), _playerShopModel.SlotsFullnessPercent, _playerShopModel.ClarityPercent);
    }

    private void UpdateExp(bool animated = false)
    {
        UpdateExpHint();
        var expAmount = _playerProgressModel.ExpAmount;
        var levelProgress = _playerProgressModel.LevelProgress;
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

    private void UpdateExpHint()
    {
        var expAmount = _playerProgressModel.ExpAmount;
        var progresPercent = (int)(_playerProgressModel.LevelProgress * 100);
        var restExpForLevelup = _playerProgressModel.NextLevelMinExp - expAmount;
        _expBarView.HintableView.DisplayText = string.Format(_loc.GetLocalization(LocalizationKeys.HintTopPanelExpFomat), progresPercent, restExpForLevelup);
    }

    private void SetLevel(int level)
    {
        _levelText.text = level.ToString();
    }

    private void Activate()
    {
        _dispatcher.UIRequestBlinkMoney += OnUIRequestBlinkMoney;

        _gameStateModel.GameStateChanged += OnGameStateChanged;
        _playerProgressModel.CashChanged += OnCashChanged;
        _playerProgressModel.GoldChanged += OnGoldChanged;
        _playerProgressModel.ExpChanged += OnExpChanged;
        _playerProgressModel.LevelChanged += OnLevelChanged;
        _playerShopModel.MoodChanged += OnMoodChanged;
    }

    private void OnGameStateChanged(GameStateName previousState, GameStateName currentState)
    {
        if (previousState == GameStateName.ReadyForStart)
        {
            _cashBarView.SetAmountAnimatedAsync(_playerProgressModel.Cash);
            _crystalsBarView.SetAmountAnimatedAsync(_playerProgressModel.Gold);
            UpdateMood(animated: true);
            if (_isLevelUpInProgress == false)
            {
                UpdateExp(animated: true);
            }
        }
    }

    private async void OnLevelChanged(int delta)
    {
        _isLevelUpInProgress = true;
        _dispatcher.UIRequestBlockRaycasts();
        await AnimateFillExpToMaxOnLevelAsync();
        _expBarView.SetProgress(0);
        var updateExpTask = UpdateExpLevelUpModeAsync();
        await ShowNewLevelAnimationAsync(_playerProgressModel.Level);
        await updateExpTask;
        _dispatcher.UIRequestUnblockRaycasts();
        _dispatcher.UITopPanelLevelUpAnimationFinished();
        _isLevelUpInProgress = false;
    }

    public UniTask AnimateFillExpToMaxOnLevelAsync()
    {
        var setAmountTask = _expBarView.SetAmountAnimatedAsync(_playerProgressModel.CurrentLevelMinExp);
        var setProgressTask = _expBarView.SetProgressAnimatedAsync(1);
        return UniTask.WhenAll(setAmountTask, setProgressTask);
    }

    private UniTask UpdateExpLevelUpModeAsync()
    {
        UpdateExpHint();
        var setAmountTask = _expBarView.SetAmountAnimatedAsync(_playerProgressModel.ExpAmount, needToAnimateIcon: false);
        var setProgressTask = _expBarView.SetProgressAnimatedAsync(_playerProgressModel.LevelProgress);
        return UniTask.WhenAll(setAmountTask, setProgressTask);
    }

    private void OnGoldChanged(int previousValue, int currentValue)
    {
        if (_gameStateModel.IsRealtimeState)
        {
            _crystalsBarView.SetAmountAnimatedAsync(currentValue);
        }
    }

    private void OnCashChanged(int previousValue, int currentValue)
    {
        if (_gameStateModel.IsRealtimeState)
        {
            _cashBarView.SetAmountAnimatedAsync(currentValue);
        }
    }

    private void OnExpChanged(int delta)
    {
        if (_gameStateModel.IsRealtimeState && _isLevelUpInProgress == false)
        {
            UpdateExp(animated: true);
        }
    }

    private void OnMoodChanged(float delta)
    {
        if (_gameStateModel.IsRealtimeState)
        {
            UpdateMood(animated: true);
        }
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

        _audioManager.PlaySound(SoundNames.Negative3);
    }

    private UniTask ShowNewLevelAnimationAsync(int level)
    {
        var tcs = new UniTaskCompletionSource();
        var psPrefab = PrefabsHolder.Instance.GetRemotePrefab(PrefabsHolder.PSStarsName);
        var psGo = GameObject.Instantiate(psPrefab, _expBarView.transform);
        var ps = psGo.GetComponent<ParticleSystem>();
        _audioManager.PlaySound(SoundNames.StarsFall);
        LeanTween.delayedCall(ps.main.duration, async () =>
        {
            SetLevel(level);
            _audioManager.PlaySound(SoundNames.NewLevel);
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
