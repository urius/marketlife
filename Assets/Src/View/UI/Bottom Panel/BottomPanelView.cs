using System;
using System.Collections;
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

    private Dictionary<CanvasGroup[], float> _buttonYPositionsByCanvasGroup;
    private float DeltaPositionForAnimation = 40;
    private CanvasGroup[] _simulationModeButtons;
    private CanvasGroup[] _interiorModeButtons;

    public void Awake()
    {
        _simulationModeButtons = new CanvasGroup[] { GetCanvasGroup(_friendsButton), GetCanvasGroup(_warehouseButton), GetCanvasGroup(_interiorButton), GetCanvasGroup(_manageButton) };
        _interiorModeButtons = new CanvasGroup[] { GetCanvasGroup(_interiorCloseButton), GetCanvasGroup(_interiorObjectsButton), GetCanvasGroup(_interiorFloorsButton), GetCanvasGroup(_interiorWallsButton), GetCanvasGroup(_interiorWindowsButton), GetCanvasGroup(_interiorDoorsButton) };

        _buttonYPositionsByCanvasGroup = new Dictionary<CanvasGroup[], float>();
        _buttonYPositionsByCanvasGroup[_simulationModeButtons] = (_simulationModeButtons[0].transform as RectTransform).anchoredPosition.y;
        _buttonYPositionsByCanvasGroup[_interiorModeButtons] = (_interiorModeButtons[0].transform as RectTransform).anchoredPosition.y;

        Activate();
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
    }

    private void OnFriendsButtonClicked()
    {
        FriendsButtonClicked();
    }

    private void OnWarehouseButtonClicked()
    {
        WarehouseButtonClicked();
    }

    private void OnInteriorButtonClicked()
    {
        InteriorButtonClicked();
    }

    private void OnManageButtonClicked()
    {
        ManageButtonClicked();
    }

    private void OnInteriorCloseButtonClicked()
    {
        InteriorCloseButtonClicked();
    }

    private void OnInteriorObjectsButtonClicked()
    {
        InteriorObjectsButtonClicked();
    }

    private void OnInteriorWallsButtonClicked()
    {
        InteriorWallsButtonClicked();
    }

    private void OnInteriorWindowsButtonClicked()
    {
        InteriorWindowsButtonClicked();
    }

    private void OnInteriorDoorsButtonClicked()
    {
        InteriorDoorsButtonClicked();
    }

    private CanvasGroup GetCanvasGroup(Button button)
    {
        return button.GetComponent<CanvasGroup>();
    }
}
