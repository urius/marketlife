using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITopPanelMediator : MonoBehaviour
{
    [SerializeField] private UITopPanelBarView _crystalsBarView;
    [SerializeField] private UITopPanelBarView _cashBarView;
    [SerializeField] private UITopPanelBarWithProgressView _expBarView;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private UITopPanelBarWithProgressView _moodBarView;
    [SerializeField] private RectTransform _animationsContainer;

    private GameStateModel _gameStateModel;
    private Dispatcher _dispatcher;
    private PlayerModelHolder _playerModelHolder;
    private LocalizationManager _loc;
    private SpritesProvider _spritesProvider;
    private AudioManager _audioManager;
    private TutorialUIElementsProvider _tutorialUIElementsProvider;
    private Camera _camera;
    private UserProgressModel _playerProgressModel;
    private ShopModel _playerShopModel;
    private bool _isLevelUpInProgress;
    private int _updateGoldAnimationDelayMs = 0;
    private int _updateCashAnimationDelayMs = 0;
    private int _updateExpAnimationDelayMs = 0;

    public void Awake()
    {
        _gameStateModel = GameStateModel.Instance;
        _dispatcher = Dispatcher.Instance;
        _playerModelHolder = PlayerModelHolder.Instance;
        _loc = LocalizationManager.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _audioManager = AudioManager.Instance;
        _tutorialUIElementsProvider = TutorialUIElementsProvider.Instance;
        _camera = Camera.main;
    }

    public async void Start()
    {
        await _gameStateModel.GameDataLoadedTask;

        _playerProgressModel = _playerModelHolder.UserModel.ProgressModel;
        _playerShopModel = _playerModelHolder.UserModel.ShopModel;
        _tutorialUIElementsProvider.SetElement(TutorialUIElement.TopPanelMoodBar, _moodBarView.AmountText.transform);

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

    private async void UpdateExp(bool animated = false)
    {
        UpdateExpHint();
        var expAmount = _playerProgressModel.ExpAmount;
        var levelProgress = _playerProgressModel.LevelProgress;
        if (animated)
        {
            if (_updateExpAnimationDelayMs > 0)
            {
                await UniTask.Delay(_updateExpAnimationDelayMs);
                _updateExpAnimationDelayMs = 0;
            }
            _expBarView.SetProgressAnimated(levelProgress);
            await _expBarView.SetAmountAnimatedAsync(expAmount);
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
        _crystalsBarView.ButtonClicked += OnAddGoldClicked;
        _cashBarView.ButtonClicked += OnAddCashClicked;
        _dispatcher.UIRequestAddGoldFlyAnimation += OnUIRequestAddGoldFlyAnimation;
        _dispatcher.UIRequestAddCashFlyAnimation += OnUIRequestAddCashFlyAnimation;
        _dispatcher.UIRequestAddExpFlyAnimation += OnUIRequestAddExpFlyAnimation;
    }

    private void OnUIRequestAddGoldFlyAnimation(Vector2 screenCoords, int amount)
    {
        _updateGoldAnimationDelayMs = 500;
        if (amount > 1)
        {
            for (var i = 0; i < amount; i++)
            {
                AnimateFlyingGold(Random.insideUnitCircle * 100 + screenCoords);
            }
        }
        else
        {
            AnimateFlyingGold(screenCoords);
        }
    }

    private void OnUIRequestAddCashFlyAnimation(Vector2 screenCoords, int amount)
    {
        _updateCashAnimationDelayMs = 500;
        for (var i = 0; i < amount; i++)
        {
            AnimateFlyingCash(Random.insideUnitCircle * 100 + screenCoords);
        }
    }

    private void OnUIRequestAddExpFlyAnimation(Vector2 screenCoords, int amount)
    {
        _updateExpAnimationDelayMs = 500;
        for (var i = 0; i < amount; i++)
        {
            AnimateFlyingExp(Random.insideUnitCircle * 100 + screenCoords);
        }
    }

    private void AnimateFlyingGold(Vector2 screenPos)
    {
        var image = CreateSpriteImage(_spritesProvider.GetGoldIcon(), screenPos);
        var targetLocalPos = _animationsContainer.InverseTransformPoint(_crystalsBarView.transform.position);//.parent.TransformPoint(_crystalsBarView.transform.localPosition));
        AnimateFlying(image, targetLocalPos);
    }

    private void AnimateFlyingCash(Vector2 screenPos)
    {
        var image = CreateSpriteImage(_spritesProvider.GetCashIcon(), screenPos);
        var targetLocalPos = _animationsContainer.InverseTransformPoint(_cashBarView.transform.position);
        AnimateFlying(image, targetLocalPos);
    }

    private void AnimateFlyingExp(Vector2 screenPos)
    {
        var image = CreateSpriteImage(_spritesProvider.GetStarIcon(isBig: false), screenPos);
        var targetLocalPos = _animationsContainer.InverseTransformPoint(_expBarView.transform.position);
        AnimateFlying(image, targetLocalPos);
    }

    private void AnimateFlying(Image image, Vector3 targetLocalPos)
    {
        image.color = Color.white.SetAlpha(0);
        var itemRect = image.rectTransform;
        var delay = Random.Range(0f, 0.2f);
        LeanTween.alpha(itemRect, 1, Random.Range(0.2f, 0.5f)).setDelay(delay);
        var descr = LeanTween.move(itemRect, targetLocalPos, Random.Range(0.6f, 0.9f)).setEaseInCubic().setDelay(delay);
        descr.destroyOnComplete = true;
    }

    private Image CreateSpriteImage(Sprite sprite, Vector2 screenPos)
    {
        var go = new GameObject("FlyingGold");
        go.transform.SetParent(_animationsContainer);
        go.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);// Vector3.one;
        go.transform.localEulerAngles = Vector3.zero;
        var image = go.AddComponent<Image>();
        image.preserveAspect = true;
        image.sprite = sprite;
        image.raycastTarget = false;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_animationsContainer, screenPos, _camera, out var worldCoords))
        {
            image.transform.position = worldCoords;
        }

        return image;
    }

    private void OnAddGoldClicked()
    {
        _dispatcher.UITopPanelAddMoneyClicked(true);
    }

    private void OnAddCashClicked()
    {
        _dispatcher.UITopPanelAddMoneyClicked(false);
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
        await AnimateFillExpToMaxOnLevelAsync();
        _expBarView.SetProgress(0);
        var updateExpTask = UpdateExpLevelUpModeAsync();
        await ShowNewLevelAnimationAsync(_playerProgressModel.Level);
        await updateExpTask;
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

    private async void OnGoldChanged(int previousValue, int currentValue)
    {
        if (_gameStateModel.IsPlayingState || _gameStateModel.GameState == GameStateName.ReadyForStart)
        {
            if (_updateGoldAnimationDelayMs > 0)
            {
                await UniTask.Delay(_updateGoldAnimationDelayMs);
            }
            _crystalsBarView.SetAmountAnimatedAsync(currentValue);
            if (previousValue > currentValue)
            {
                _audioManager.PlaySound(SoundNames.Cash1);
            }
            else if (currentValue > previousValue)
            {
                _audioManager.PlaySound(SoundNames.Cash2);
            }
        }
        _updateGoldAnimationDelayMs = 0;
    }

    private async void OnCashChanged(int previousValue, int currentValue)
    {
        if (_gameStateModel.IsPlayingState)
        {
            if (_updateCashAnimationDelayMs > 0)
            {
                await UniTask.Delay(_updateCashAnimationDelayMs);
            }
            _cashBarView.SetAmountAnimatedAsync(currentValue);
            if (previousValue > currentValue)
            {
                _audioManager.PlaySound(SoundNames.Cash1);
            }
            else if (currentValue > previousValue)
            {
                _audioManager.PlaySound(SoundNames.Cash2);
            }
        }
        _updateCashAnimationDelayMs = 0;
    }

    private void OnExpChanged(int delta)
    {
        if (_gameStateModel.IsPlayingState && _isLevelUpInProgress == false)
        {
            UpdateExp(animated: true);
        }
    }

    private void OnMoodChanged(float delta)
    {
        if (_gameStateModel.IsPlayingState)
        {
            UpdateMood(animated: true);
        }
    }

    private async void OnUIRequestBlinkMoney(bool isGold)
    {
        _audioManager.PlaySound(SoundNames.Negative3);

        if (isGold)
        {
            _crystalsBarView.Amount = _playerProgressModel.Gold;
            await _crystalsBarView.BlinkAmountAsync();
        }
        else
        {
            _cashBarView.Amount = _playerProgressModel.Cash;
            await _cashBarView.BlinkAmountAsync();
        }
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
