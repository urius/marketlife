using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BottomPanelView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event Action PointerEnter = delegate { };
    public event Action PointerExit = delegate { };
    public event Action FriendsButtonClicked = delegate { };
    public event Action WarehouseButtonClicked = delegate { };
    public event Action InteriorButtonClicked = delegate { };
    public event Action ManageButtonClicked = delegate { };
    public event Action InteriorCloseButtonClicked = delegate { };
    public event Action InteriorObjectsButtonClicked = delegate { };
    public event Action InteriorFloorsButtonClicked = delegate { };
    public event Action InteriorWallsButtonClicked = delegate { };
    public event Action InteriorWindowsButtonClicked = delegate { };
    public event Action InteriorDoorsButtonClicked = delegate { };
    public event Action FinishPlacingClicked = delegate { };
    public event Action RotateRightClicked = delegate { };
    public event Action RotateLeftClicked = delegate { };
    public event Action AutoPlaceClicked = delegate { };    

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
    [SerializeField] private Button _buttonAutoPlace;
    [SerializeField] private Text _buttonAutoPlacePriceText;
    [Space(10)]
    [SerializeField] private GameObject _blockPanel;
    [Space(10)]
    [SerializeField] private Image _bgImage;
    [SerializeField] private Sprite _bgSimulationModeSprite;
    [SerializeField] private Sprite _bgInteriorModeSprite;

    private const float DeltaPositionForAnimation = 40;

    private bool _isBlocked = false;
    private Dictionary<CanvasGroup[], float> _buttonYPositionsByCanvasGroup;
    private CanvasGroup[] _simulationModeButtons;
    private CanvasGroup[] _interiorModeButtons;
    private CanvasGroup[] _allPlacingModeButtons;
    private CanvasGroup[] _placingShopObjectModeButtons;
    private CanvasGroup[] _movingShopObjectModeButtons;
    private CanvasGroup[] _placingDecorationModeButtons;
    private CanvasGroup[] _placingProductModeButtons;
    private UIHintableView[] _allHintableViews;

    public Button FriendsButton => _friendsButton;
    public Button WarehouseButton => _warehouseButton;
    public Button InteriorObjectsButton => _interiorObjectsButton;
    public Button InteriorFloorsButton => _interiorFloorsButton;
    public Button InteriorWallsButton => _interiorWallsButton;
    public Button InteriorWindowsButton => _interiorWindowsButton;
    public Button InteriorDoorsButton => _interiorDoorsButton;

    public void Awake()
    {
        _simulationModeButtons = new CanvasGroup[] { GetCanvasGroup(_friendsButton), GetCanvasGroup(_warehouseButton), GetCanvasGroup(_interiorButton), GetCanvasGroup(_manageButton) };
        _interiorModeButtons = new CanvasGroup[] { GetCanvasGroup(_interiorCloseButton), GetCanvasGroup(_interiorObjectsButton), GetCanvasGroup(_interiorFloorsButton), GetCanvasGroup(_interiorWallsButton), GetCanvasGroup(_interiorWindowsButton), GetCanvasGroup(_interiorDoorsButton) };
        _allPlacingModeButtons = new CanvasGroup[] { GetCanvasGroup(_buttonFinishPlacing), GetCanvasGroup(_buttonRotateRight), GetCanvasGroup(_buttonRotateLeft), GetCanvasGroup(_buttonAutoPlace) };
        _placingShopObjectModeButtons = new CanvasGroup[] { GetCanvasGroup(_buttonFinishPlacing), GetCanvasGroup(_buttonRotateRight), GetCanvasGroup(_buttonRotateLeft) };
        _movingShopObjectModeButtons = new CanvasGroup[] { GetCanvasGroup(_buttonRotateRight), GetCanvasGroup(_buttonRotateLeft) };
        _placingDecorationModeButtons = new CanvasGroup[] { GetCanvasGroup(_buttonFinishPlacing) };
        _placingProductModeButtons = new CanvasGroup[] { GetCanvasGroup(_buttonFinishPlacing), GetCanvasGroup(_buttonAutoPlace) };

        _buttonYPositionsByCanvasGroup = new Dictionary<CanvasGroup[], float>
        {
            [_simulationModeButtons] = (_simulationModeButtons[0].transform as RectTransform).anchoredPosition.y,
            [_interiorModeButtons] = (_interiorModeButtons[0].transform as RectTransform).anchoredPosition.y,
            [_allPlacingModeButtons] = (_allPlacingModeButtons[0].transform as RectTransform).anchoredPosition.y,
            [_placingShopObjectModeButtons] = (_placingShopObjectModeButtons[0].transform as RectTransform).anchoredPosition.y,
            [_movingShopObjectModeButtons] = (_movingShopObjectModeButtons[0].transform as RectTransform).anchoredPosition.y,
            [_placingDecorationModeButtons] = (_placingDecorationModeButtons[0].transform as RectTransform).anchoredPosition.y,
            [_placingProductModeButtons] = (_placingProductModeButtons[0].transform as RectTransform).anchoredPosition.y,
        };

        _allHintableViews = GetComponentsInChildren<UIHintableView>(true);

        Activate();
    }

    public void SetAutoPlacePriceGold(int goldAmount)
    {
        _buttonAutoPlacePriceText.text = goldAmount.ToString();
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

    public void ShowSimulationModeBG()
    {
        _bgImage.sprite = _bgSimulationModeSprite;
    }

    public void ShowInteriorModeBG()
    {
        _bgImage.sprite = _bgInteriorModeSprite;
    }

    public UniTask ShowSimulationModeButtonsAsync()
    {
        return ShowButtonsInternalAsync(_simulationModeButtons, false);
    }

    public UniTask ShowInteriorModeButtonsAsync()
    {
        return ShowButtonsInternalAsync(_interiorModeButtons);
    }

    public UniTask HideSimulationModeButtonsAsync()
    {
        return HideButtonsInternalAsync(_simulationModeButtons, withDelays: false);
    }

    public UniTask HideInteriorModeButtonsAsync()
    {
        return HideButtonsInternalAsync(_interiorModeButtons);
    }

    public UniTask ShowPlacingButtonsAsync(PlacingModeType placingModeType)
    {
        return placingModeType switch
        {
            PlacingModeType.NewShopObject => ShowButtonsInternalAsync(_placingShopObjectModeButtons, withDelays: false),
            PlacingModeType.MovingShopObject => ShowButtonsInternalAsync(_movingShopObjectModeButtons, withDelays: false),
            PlacingModeType.ShopDecoration => ShowButtonsInternalAsync(_placingDecorationModeButtons, withDelays: false),
            PlacingModeType.Product => ShowButtonsInternalAsync(_placingProductModeButtons, withDelays: false),
            _ => UniTask.CompletedTask,
        };
    }

    public void SetButtonSelected(Button buttonToDisable)
    {
        buttonToDisable.interactable = false;
    }

    public void SetButtonUnselected(Button buttonToDisable)
    {
        buttonToDisable.interactable = true;
    }

    public UniTask HidePlacingButtonsAsync()
    {
        return HideButtonsInternalAsync(_allPlacingModeButtons);
    }

    public UniTask MinimizePanelAsync()
    {
        var tcs = new UniTaskCompletionSource();

        var rectTransform = transform as RectTransform;
        LeanTween.cancel(rectTransform);
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
        LeanTween.cancel(rectTransform);
        var targetPosition = Vector3.zero;
        LeanTween.move(rectTransform, targetPosition, 1f)
            .setEaseOutBounce()
            .setOnComplete(() => tcs.TrySetResult());

        return tcs.Task;
    }

    private UniTask ShowButtonsInternalAsync(CanvasGroup[] canvasGroups, bool withDelays = true)
    {
        var resultTsc = new UniTaskCompletionSource();
        foreach (var canvasGroup in canvasGroups)
        {
            canvasGroup.alpha = 0;
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.interactable = true;
        }

        var startPos = _buttonYPositionsByCanvasGroup[canvasGroups];
        LTDescr tweenDescription = null;
        for (var i = 0; i < canvasGroups.Length; i++)
        {
            var delay = withDelays ? i * DefaultDelaySeconds : 0;

            var rectTransform = canvasGroups[i].transform as RectTransform;
            var v = rectTransform.anchoredPosition;
            rectTransform.anchoredPosition = new Vector2(v.x, startPos + DeltaPositionForAnimation);
            LeanTween.move(rectTransform, new Vector2(v.x, startPos), DefaultDurationSeconds).setDelay(delay).setEaseOutQuad();

            tweenDescription = LeanTween.alphaCanvas(canvasGroups[i], 1, DefaultDurationSeconds).setDelay(delay);
        }
        if (tweenDescription != null)
        {
            tweenDescription?.setOnComplete(() => resultTsc.TrySetResult());
        }
        else
        {
            resultTsc.TrySetResult();
        }

        return resultTsc.Task;
    }

    private UniTask HideButtonsInternalAsync(CanvasGroup[] canvasGroups, bool withDelays = true)
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
        LTDescr tweenDescription = null;
        for (var i = 0; i < canvasGroups.Length; i++)
        {
            if (canvasGroups[i].gameObject.activeSelf == false) continue;

            var delay = withDelays ? i * DefaultDelaySeconds : 0;
            canvasGroups[i].interactable = false;

            var rectTransform = canvasGroups[i].transform as RectTransform;
            var v = rectTransform.anchoredPosition;
            LeanTween.move(rectTransform, new Vector2(v.x, targetPos), DefaultDurationSeconds).setDelay(delay).setEaseInQuad();

            tweenDescription = LeanTween.alphaCanvas(canvasGroups[i], 0, DefaultDurationSeconds).setDelay(delay);
        }
        if (tweenDescription != null)
        {
            tweenDescription?.setOnComplete(OnLastTweenComplete);
        }
        else
        {
            resultTsc.TrySetResult();
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
        _interiorFloorsButton.AddOnClickListener(OnInteriorFloorsButtonClicked);
        _interiorWallsButton.AddOnClickListener(OnInteriorWallsButtonClicked);
        _interiorWindowsButton.AddOnClickListener(OnInteriorWindowsButtonClicked);
        _interiorDoorsButton.AddOnClickListener(OnInteriorDoorsButtonClicked);

        _buttonFinishPlacing.AddOnClickListener(OnFinishPlacingButtonClicked);
        _buttonRotateRight.AddOnClickListener(OnRotateRightButtonClicked);
        _buttonRotateLeft.AddOnClickListener(OnRotateLeftButtonClicked);
        _buttonAutoPlace.AddOnClickListener(OnAutoPlaceClicked);
    }

    private void OnAutoPlaceClicked()
    {
        InvokeActionIfNotBlocked(AutoPlaceClicked);
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

    private void OnInteriorFloorsButtonClicked()
    {
        InvokeActionIfNotBlocked(InteriorFloorsButtonClicked);
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExit();
    }
}

public enum PlacingModeType
{
    NewShopObject,
    MovingShopObject,
    ShopDecoration,
    Product,
}
