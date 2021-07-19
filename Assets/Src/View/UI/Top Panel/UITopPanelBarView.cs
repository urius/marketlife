using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITopPanelBarView : MonoBehaviour
{
    public event Action OnClick = delegate { };

    [SerializeField] private TMP_Text _text;
    [SerializeField] private Button _button;
    [SerializeField] private RectTransform _icon;
    [SerializeField] private UIHintableView _hintableView;
    public UIHintableView HintableView => _hintableView;
    [SerializeField]
    private string _format = "{0:n0}";

    public const float ChangeAmountDuration = 1.2f;

    private const float AnimationInDurationSec = 0.4f;
    private const float AnimationOutDurationSec = 0.6f;

    private RectTransform _buttonRectTransform;

    private Vector2 _iconDefaultAnchoredPosition;
    private Vector3 _iconDefaultRotation;
    private Vector2 _buttonDefaultAnchoredPosition;
    private Color _startTextColor;
    private AudioManager _audioManager;
    private bool _isAnimationInProgress;
    private int _amount;

    public int Amount
    {
        get => _amount;
        set
        {
            if (_amount != value)
            {
                _audioManager.PlaySound(SoundNames.ScoreTick);
                _amount = value;
                _text.text = string.Format(_format, value);
            }
        }
    }

    public UniTask SetAmountAnimatedAsync(float targetAmount, bool needToAnimateIcon = true)
    {
        if (targetAmount == _amount) return UniTask.CompletedTask;

        var animateIconTask = needToAnimateIcon
            ? ((targetAmount > _amount)
                ? JumpIconAsync()
                : RotateIconAsync())
            : UniTask.CompletedTask;

        _text.color = (targetAmount > _amount) ? Color.green : Color.yellow;
        var tsc = new UniTaskCompletionSource();
        LeanTween.cancel(_text.gameObject);
        var changeTextDuration = ChangeAmountDuration;
        LeanTween.value(_text.gameObject, f => Amount = (int)f, _amount, targetAmount, changeTextDuration)
            .setEaseInOutSine()
            .setOnComplete(() => tsc.TrySetResult());
        LeanTween.value(_text.gameObject, c => _text.color = c, _text.color, _startTextColor, changeTextDuration)
            .setEaseInOutSine();
        return UniTask.WhenAll(tsc.Task, animateIconTask);
    }

    private void Awake()
    {
        _iconDefaultAnchoredPosition = _icon.anchoredPosition;
        _iconDefaultRotation = _icon.eulerAngles;
        _startTextColor = _text.color;

        _audioManager = AudioManager.Instance;

        if (_button != null)
        {
            _buttonRectTransform = _button.transform as RectTransform;
            _buttonDefaultAnchoredPosition = _buttonRectTransform.anchoredPosition;
            _button.onClick.AddListener(OnButtonClick);
        }
    }

    public UniTask HideButtonAsync()
    {
        if (_button == null) return UniTask.CompletedTask;
        var tcs = new UniTaskCompletionSource();
        _button.interactable = false;
        var targetPos = _buttonRectTransform.anchoredPosition;
        targetPos.y = 0;
        LeanTween.move(_buttonRectTransform, targetPos, AnimationInDurationSec)
            .setOnComplete(() => tcs.TrySetResult());

        return tcs.Task;
    }

    public UniTask ShowButtonAsync()
    {
        if (_button == null) return UniTask.CompletedTask;
        var tcs = new UniTaskCompletionSource();
        _button.interactable = true;
        LeanTween.move(_buttonRectTransform, _buttonDefaultAnchoredPosition, AnimationOutDurationSec)
            .setEaseOutBounce()
            .setOnComplete(() => tcs.TrySetResult());

        return tcs.Task;
    }

    public async UniTask JumpIconAsync()
    {
        if (_isAnimationInProgress) return;
        _isAnimationInProgress = true;

        var targetPos = _icon.anchoredPosition;
        targetPos.y = 35;
        var (task, tweenDescription) = LeanTweenHelper.MoveBounceAsync(_icon, targetPos, AnimationInDurationSec);
        tweenDescription.setEaseOutQuad();
        await task;

        var (task2, tweenDescription2) = LeanTweenHelper.MoveBounceAsync(_icon, _iconDefaultAnchoredPosition, 2 * AnimationInDurationSec);
        tweenDescription2.setEaseOutBounce();
        await task2;

        _isAnimationInProgress = false;
    }

    public async UniTask RotateIconAsync()
    {
        if (_isAnimationInProgress) return;
        _isAnimationInProgress = true;

        var (task, tweenDescription) = RotateZAsync(_icon.gameObject, _iconDefaultRotation.z + 60, AnimationInDurationSec);
        tweenDescription.setEaseOutQuad();
        await task;

        var (task2, tweenDescription2) = RotateZAsync(_icon.gameObject, _iconDefaultRotation.z, 2 * AnimationInDurationSec);
        tweenDescription2.setEaseOutBounce();
        await task2;

        _isAnimationInProgress = false;
    }

    public async UniTask BlinkAmountAsync()
    {
        LeanTween.cancel(_text.gameObject);
        for (var i = 0; i < 3; i++)
        {
            await ChangeAmountColorAsync(Color.white, Color.red, 0.1f);
            await ChangeAmountColorAsync(Color.red, Color.white, 0.2f);
        }
    }

    public async UniTask JumpAndSaltoIconAsync()
    {
        if (_isAnimationInProgress) return;
        _isAnimationInProgress = true;

        var targetPos = _icon.anchoredPosition;
        targetPos.y = 35;
        var (jumpTask1, jumpDescription1) = LeanTweenHelper.MoveBounceAsync(_icon, targetPos, AnimationInDurationSec);
        jumpDescription1.setEaseOutQuad();

        var (rotateTask1, rotateDescription1) = RotateZAsync(_icon.gameObject, _iconDefaultRotation.z + 180, 0.5f * AnimationOutDurationSec);
        //rotateDescription1.setDelay(0.1f);        
        await rotateTask1;
        var (rotateTask2, rotateDescription2) = RotateZAsync(_icon.gameObject, _iconDefaultRotation.z + 360, 0.5f * AnimationOutDurationSec);
        await rotateTask2;

        await jumpTask1;

        var (jumpTask2, jumpDescription2) = LeanTweenHelper.MoveBounceAsync(_icon, _iconDefaultAnchoredPosition, 2 * AnimationInDurationSec);
        jumpDescription2.setDelay(0.2f);
        jumpDescription2.setEaseOutBounce();
        await jumpTask2;

        _icon.eulerAngles = _iconDefaultRotation;
        _isAnimationInProgress = false;
    }

    private UniTask ChangeAmountColorAsync(Color from, Color to, float duration)
    {
        var tsc = new UniTaskCompletionSource();
        LeanTween.value(_text.gameObject, UpdateTextColor, from, to, duration)
            .setOnComplete(() => tsc.TrySetResult());
        return tsc.Task;
    }

    private void UpdateTextColor(Color color)
    {
        _text.color = color;
    }

    private (UniTask task, LTDescr tweenDescription) RotateZAsync(GameObject gameObject, float to, float duration)
    {
        var tcs = new UniTaskCompletionSource();
        var tweenDescription = LeanTween.rotateZ(gameObject, to, duration)
            .setOnComplete(() => tcs.TrySetResult());

        return (tcs.Task, tweenDescription);
    }

    private void OnButtonClick()
    {
        OnClick();
    }
}
