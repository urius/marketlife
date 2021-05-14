using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BottomPanelView : MonoBehaviour
{
    public event Action FriendsButtonClicked = delegate { };
    public event Action WarehouseButtonClicked = delegate { };
    public event Action InteriorButtonClicked = delegate { };
    public event Action ManageButtonClicked = delegate { };
    public event Action InteriorCloseButtonClicked = delegate { };
    public event Action InteriorObjectsButtonClicked = delegate { };
    public event Action InteriorWallsButtonClicked = delegate { };
    public event Action InteriorWindowsButtonClicked = delegate { };
    public event Action InteriorDoorsButtonClicked = delegate { };
    public event Action FinishPlacingClicked = delegate { };
    public event Action RotateRightClicked = delegate { };
    public event Action RotateLeftClicked = delegate { };

    private const float DefaultDurationSeconds = 0.15f;
    private const float DefaultDelaySeconds = 0.05f;

    [SerializeField] private ScrollBoxView _scrollBoxView;
    public ScrollBoxView ScrollBoxView => _scrollBoxView;
    [Space(10)]
    [SerializeField] private Button _friendsButton;
    [SerializeField] private Button _warehouseButton;
    [SerializeField] private Button _interiorButton;
    [SerializeField] private Button _manageButton;
    [Space(10)]
    [SerializeField] private Button _interiorCloseButton;
    [SerializeField] private Button _interiorObjectsButton;
    [SerializeField] private Button _interiorFloorsButton;
    [SerializeField] private Button _interiorWallsButton;
    [SerializeField] private Button _interiorWindowsButton;
    [SerializeField] private Button _interiorDoorsButton;
    [Space(10)]
    [SerializeField] private Button _buttonFinishPlacing;
    [SerializeField] private Button _buttonRotateRight;
    [SerializeField] private Button _buttonRotateLeft;
    [Space(10)]
    [SerializeField] private GameObject _blockPanel;

    private bool _isBlocked = false;
    private Dictionary<CanvasGroup[], float> _buttonYPositionsByCanvasGroup;
    private float DeltaPositionForAnimation = 40;
    private CanvasGroup[] _simulationModeButtons;
    private CanvasGroup[] _interiorModeButtons;
    private CanvasGroup[] _placingModeButtons;
    private UIHintableView[] _allHintableViews;

    public void Awake()
    {
        _simulationModeButtons = new CanvasGroup[] { GetCanvasGroup(_friendsButton), GetCanvasGroup(_warehouseButton), GetCanvasGroup(_interiorButton), GetCanvasGroup(_manageButton) };
        _interiorModeButtons = new CanvasGroup[] { GetCanvasGroup(_interiorCloseButton), GetCanvasGroup(_interiorObjectsButton), GetCanvasGroup(_interiorFloorsButton), GetCanvasGroup(_interiorWallsButton), GetCanvasGroup(_interiorWindowsButton), GetCanvasGroup(_interiorDoorsButton) };
        _placingModeButtons = new CanvasGroup[] { GetCanvasGroup(_buttonFinishPlacing), GetCanvasGroup(_buttonRotateRight), GetCanvasGroup(_buttonRotateLeft) };

        _buttonYPositionsByCanvasGroup = new Dictionary<CanvasGroup[], float>
        {
            [_simulationModeButtons] = (_simulationModeButtons[0].transform as RectTransform).anchoredPosition.y,
            [_interiorModeButtons] = (_interiorModeButtons[0].transform as RectTransform).anchoredPosition.y,
            [_placingModeButtons] = (_placingModeButtons[0].transform as RectTransform).anchoredPosition.y
        };

        _allHintableViews = GetComponentsInChildren<UIHintableView>(true);

        Activate();
    }

    public void SetIsBlocked(bool isBlocked)
    {
        _isBlocked = isBlocked;
        _blockPanel.SetActive(_isBlocked);

        Array.ForEach(_allHintableViews, v => v.SetEnabled(!_isBlocked));
    }

    public IDisposable SetBlockedDisposable()
    {
        SetIsBlocked(true);
        return new DisposableSource(() => SetIsBlocked(false));
    }

    public UniTask ShowSimulationModeButtonsAsync()
    {
        return ShowButtonsInternalAsync(_simulationModeButtons);
    }

    public UniTask ShowInteriorModeButtonsAsync()
    {
        return ShowButtonsInternalAsync(_interiorModeButtons);
    }

    public UniTask HideSimulationModeButtonsAsync()
    {
        return HideButtonsInternalAsync(_simulationModeButtons);
    }

    public UniTask HideInteriorModeButtonsAsync()
    {
        return HideButtonsInternalAsync(_interiorModeButtons);
    }

    public UniTask ShowPlacingButtonsAsync()
    {
        return ShowButtonsInternalAsync(_placingModeButtons);
    }

    public UniTask HidePlacingButtonsAsync()
    {
        return HideButtonsInternalAsync(_placingModeButtons);
    }

    public UniTask MinimizePanelAsync()
    {
        var tcs = new UniTaskCompletionSource();

        var rectTransform = transform as RectTransform;
        var targetPosition = rectTransform.anchoredPosition;
        targetPosition.y = -200;
        LeanTween.move(rectTransform, targetPosition, 0.5f)
            .setEaseOutCirc()
            .setOnComplete(() => tcs.TrySetResult());

        return tcs.Task;
    }

    public UniTask MaximizePanelAsync()
    {
        var tcs = new UniTaskCompletionSource();

        var rectTransform = transform as RectTransform;
        var targetPosition = Vector3.zero;
        LeanTween.move(rectTransform, targetPosition, 1f)
            .setEaseOutBounce()
            .setOnComplete(() => tcs.TrySetResult());

        return tcs.Task;
    }

    private UniTask ShowButtonsInternalAsync(CanvasGroup[] canvasGroups)
    {
        var resultTsc = new UniTaskCompletionSource();
        foreach (var canvasGroup in canvasGroups)
        {
            canvasGroup.alpha = 0;
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.interactable = true;
        }

        var startPos = _buttonYPositionsByCanvasGroup[canvasGroups];
        for (var i = 0; i < canvasGroups.Length; i++)
        {
            var delay = i * DefaultDelaySeconds;

            var rectTransform = canvasGroups[i].transform as RectTransform;
            var v = rectTransform.anchoredPosition;
            rectTransform.anchoredPosition = new Vector2(v.x, startPos + DeltaPositionForAnimation);
            LeanTween.move(rectTransform, new Vector2(v.x, startPos), DefaultDurationSeconds).setDelay(delay).setEaseOutQuad();

            var tweenDescr = LeanTween.alphaCanvas(canvasGroups[i], 1, DefaultDurationSeconds).setDelay(delay);
            if (i == canvasGroups.Length - 1)
            {
                tweenDescr.setOnComplete(() => resultTsc.TrySetResult());
            }
        }

        return resultTsc.Task;
    }

    private UniTask HideButtonsInternalAsync(CanvasGroup[] canvasGroups)
    {
        var resultTsc = new UniTaskCompletionSource();
        void OnLastTweenComplete()
        {
            foreach (var canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = 0;
                canvasGroup.gameObject.SetActive(false);
            }
            resultTsc.TrySetResult();
        }

        var targetPos = _buttonYPositionsByCanvasGroup[canvasGroups] + DeltaPositionForAnimation;
        for (var i = 0; i < canvasGroups.Length; i++)
        {
            var delay = i * DefaultDelaySeconds;
            canvasGroups[i].interactable = false;

            var rectTransform = canvasGroups[i].transform as RectTransform;
            var v = rectTransform.anchoredPosition;
            LeanTween.move(rectTransform, new Vector2(v.x, targetPos), DefaultDurationSeconds).setDelay(delay).setEaseInQuad();

            var tweenDescr = LeanTween.alphaCanvas(canvasGroups[i], 0, DefaultDurationSeconds).setDelay(delay);
            if (i == canvasGroups.Length - 1)
            {
                tweenDescr.setOnComplete(OnLastTweenComplete);
            }
        }

        return resultTsc.Task;
    }

    private void Activate()
    {
        _friendsButton.AddOnClickListener(OnFriendsButtonClicked);
        _warehouseButton.AddOnClickListener(OnWarehouseButtonClicked);
        _interiorButton.AddOnClickListener(OnInteriorButtonClicked);
        _manageButton.AddOnClickListener(OnManageButtonClicked);

        _interiorCloseButton.AddOnClickListener(OnInteriorCloseButtonClicked);
        _interiorObjectsButton.AddOnClickListener(OnInteriorObjectsButtonClicked);
        _interiorWallsButton.AddOnClickListener(OnInteriorWallsButtonClicked);
        _interiorWindowsButton.AddOnClickListener(OnInteriorWindowsButtonClicked);
        _interiorDoorsButton.AddOnClickListener(OnInteriorDoorsButtonClicked);

        _buttonFinishPlacing.AddOnClickListener(OnFinishPlacingButtonClicked);
        _buttonRotateRight.AddOnClickListener(OnRotateRightButtonClicked);
        _buttonRotateLeft.AddOnClickListener(OnRotateLeftButtonClicked);
    }

    private void OnRotateLeftButtonClicked()
    {
        InvokeActionIfNotBlocked(RotateLeftClicked);
    }

    private void OnRotateRightButtonClicked()
    {
        InvokeActionIfNotBlocked(RotateRightClicked);
    }

    private void OnFinishPlacingButtonClicked()
    {
        InvokeActionIfNotBlocked(FinishPlacingClicked);
    }

    private void OnFriendsButtonClicked()
    {
        InvokeActionIfNotBlocked(FriendsButtonClicked);
    }

    private void OnWarehouseButtonClicked()
    {
        InvokeActionIfNotBlocked(WarehouseButtonClicked);
    }

    private void OnInteriorButtonClicked()
    {
        InvokeActionIfNotBlocked(InteriorButtonClicked);
    }

    private void OnManageButtonClicked()
    {
        InvokeActionIfNotBlocked(ManageButtonClicked);
    }

    private void OnInteriorCloseButtonClicked()
    {
        InvokeActionIfNotBlocked(InteriorCloseButtonClicked);
    }

    private void OnInteriorObjectsButtonClicked()
    {
        InvokeActionIfNotBlocked(InteriorObjectsButtonClicked);
    }

    private void OnInteriorWallsButtonClicked()
    {
        InvokeActionIfNotBlocked(InteriorWallsButtonClicked);
    }

    private void OnInteriorWindowsButtonClicked()
    {
        InvokeActionIfNotBlocked(InteriorWindowsButtonClicked);
    }

    private void OnInteriorDoorsButtonClicked()
    {
        InvokeActionIfNotBlocked(InteriorDoorsButtonClicked);
    }

    private void InvokeActionIfNotBlocked(Action action)
    {
        if (_isBlocked == false)
        {
            action();
        }
    }

    private CanvasGroup GetCanvasGroup(Button button)
    {
        return button.GetComponent<CanvasGroup>();
    }
}
