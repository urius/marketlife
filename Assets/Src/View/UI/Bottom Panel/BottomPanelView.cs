using System;
using System.Linq;
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
    public event Action BackButtonClicked = delegate { };
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
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _interiorObjectsButton;
    [SerializeField] private Button _interiorFloorsButton;
    [SerializeField] private Button _interiorWallsButton;
    [SerializeField] private Button _interiorWindowsButton;
    [SerializeField] private Button _interiorDoorsButton;
    [Space(10)]
    [SerializeField] private Button _buttonFinishAction;
    [SerializeField] private Button _buttonRotateRight;
    [SerializeField] private Button _buttonRotateLeft;
    [SerializeField] private Button _buttonAutoPlace;
    [Space(10)]
    [SerializeField] private GameObject _blockPanel;
    [Space(10)]
    [SerializeField] private Image _bgImage;
    [SerializeField] private Sprite _bgSimulationModeSprite;
    [SerializeField] private Sprite _bgInteriorModeSprite;
    [SerializeField] private Sprite _bgFriendShopModeSprite;

    private const float DeltaPositionForAnimation = 40;

    private bool _isBlocked = false;
    private ButtonData[] _simulationModeButtons;
    private ButtonData[] _interiorModeButtons;
    private ButtonData[] _friendShopModeButtons;
    private ButtonData[] _allActionModeButtons;
    private ButtonData[] _placingShopObjectModeButtons;
    private ButtonData[] _movingShopObjectModeButtons;
    private ButtonData[] _placingDecorationModeButtons;
    private ButtonData[] _placingProductModeButtons;
    private ButtonData[] _performingFriendActionModeButtons;    
    private UIHintableView[] _allHintableViews;

    public Button FriendsButton => _friendsButton;
    public Button WarehouseButton => _warehouseButton;
    public Button InteriorButton => _interiorButton;
    public Button ManageButton => _manageButton;
    public Button InteriorObjectsButton => _interiorObjectsButton;
    public Button InteriorFloorsButton => _interiorFloorsButton;
    public Button InteriorWallsButton => _interiorWallsButton;
    public Button InteriorWindowsButton => _interiorWindowsButton;
    public Button InteriorDoorsButton => _interiorDoorsButton;

    public void Awake()
    {
        _simulationModeButtons = new[] { GetButtonData(_warehouseButton), GetButtonData(_friendsButton), GetButtonData(_interiorButton), GetButtonData(_manageButton) };
        _interiorModeButtons = new [] { GetButtonData(_backButton), GetButtonData(_interiorObjectsButton), GetButtonData(_interiorFloorsButton), GetButtonData(_interiorWallsButton), GetButtonData(_interiorWindowsButton), GetButtonData(_interiorDoorsButton) };
        _friendShopModeButtons = new [] { GetButtonData(_backButton) };
        _allActionModeButtons = new [] { GetButtonData(_buttonFinishAction), GetButtonData(_buttonRotateRight), GetButtonData(_buttonRotateLeft), GetButtonData(_buttonAutoPlace) };
        _placingShopObjectModeButtons = new [] { GetButtonData(_buttonFinishAction), GetButtonData(_buttonRotateRight), GetButtonData(_buttonRotateLeft) };
        _movingShopObjectModeButtons = new [] { GetButtonData(_buttonRotateRight), GetButtonData(_buttonRotateLeft) };
        _placingDecorationModeButtons = new [] { GetButtonData(_buttonFinishAction) };
        _placingProductModeButtons = new [] { GetButtonData(_buttonFinishAction), GetButtonData(_buttonAutoPlace) };
        _performingFriendActionModeButtons = new [] { GetButtonData(_buttonFinishAction) };

        _allHintableViews = GetComponentsInChildren<UIHintableView>(true);

        Activate();
    }

    public void DisableFriendsButton()
    {
        _simulationModeButtons.First(d => d.RectTransform == _friendsButton.transform).IsDisabled = true;
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

    public void ShowFriendShopModeBG()
    {
        _bgImage.sprite = _bgFriendShopModeSprite;
    }

    public UniTask ShowSimulationModeButtonsAsync()
    {
        return ShowButtonsInternalAsync(_simulationModeButtons, false);
    }

    public UniTask ShowInteriorModeButtonsAsync()
    {
        return ShowButtonsInternalAsync(_interiorModeButtons);
    }

    public UniTask ShowFriendShopModeButtonsAsync()
    {
        return ShowButtonsInternalAsync(_friendShopModeButtons);
    }

    public UniTask HideSimulationModeButtonsAsync()
    {
        return HideButtonsInternalAsync(_simulationModeButtons, withDelays: false);
    }

    public UniTask HideInteriorModeButtonsAsync()
    {
        return HideButtonsInternalAsync(_interiorModeButtons);
    }

    public UniTask HideFriendShopModeButtonsAsync()
    {
        return HideButtonsInternalAsync(_friendShopModeButtons, withDelays: false);
    }

    public UniTask ShowActionButtonsAsync(ActionModeType placingModeType)
    {
        return placingModeType switch
        {
            ActionModeType.PlacingNewShopObject => ShowButtonsInternalAsync(_placingShopObjectModeButtons, withDelays: false),
            ActionModeType.MovingShopObject => ShowButtonsInternalAsync(_movingShopObjectModeButtons, withDelays: false),
            ActionModeType.PlacingShopDecoration => ShowButtonsInternalAsync(_placingDecorationModeButtons, withDelays: false),
            ActionModeType.PlacingProduct => ShowButtonsInternalAsync(_placingProductModeButtons, withDelays: false),
            ActionModeType.FriendAction => ShowButtonsInternalAsync(_performingFriendActionModeButtons, withDelays: false),
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
        return HideButtonsInternalAsync(_allActionModeButtons);
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExit();
    }

    private void Activate()
    {
        _friendsButton.AddOnClickListener(OnFriendsButtonClicked);
        _warehouseButton.AddOnClickListener(OnWarehouseButtonClicked);
        _interiorButton.AddOnClickListener(OnInteriorButtonClicked);
        _manageButton.AddOnClickListener(OnManageButtonClicked);

        _backButton.AddOnClickListener(OnBackButtonClicked);
        _interiorObjectsButton.AddOnClickListener(OnInteriorObjectsButtonClicked);
        _interiorFloorsButton.AddOnClickListener(OnInteriorFloorsButtonClicked);
        _interiorWallsButton.AddOnClickListener(OnInteriorWallsButtonClicked);
        _interiorWindowsButton.AddOnClickListener(OnInteriorWindowsButtonClicked);
        _interiorDoorsButton.AddOnClickListener(OnInteriorDoorsButtonClicked);

        _buttonFinishAction.AddOnClickListener(OnFinishPlacingButtonClicked);
        _buttonRotateRight.AddOnClickListener(OnRotateRightButtonClicked);
        _buttonRotateLeft.AddOnClickListener(OnRotateLeftButtonClicked);
        _buttonAutoPlace.AddOnClickListener(OnAutoPlaceClicked);
    }

    private UniTask ShowButtonsInternalAsync(ButtonData[] buttonsData, bool withDelays = true)
    {
        var resultTsc = new UniTaskCompletionSource();
        foreach (var buttonData in buttonsData)
        {
            var canvasGroup = buttonData.CanvasGroup;
            var isButtonEnabled = !buttonData.IsDisabled;
            
            canvasGroup.alpha = 0;
            canvasGroup.gameObject.SetActive(isButtonEnabled);
            canvasGroup.interactable = isButtonEnabled;
        }

        var startPos = buttonsData[0].DefaultYPosition;
        LTDescr tweenDescription = null;
        for (var i = 0; i < buttonsData.Length; i++)
        {
            if (buttonsData[i].IsDisabled) continue;
            
            var delay = withDelays ? i * DefaultDelaySeconds : 0;

            var rectTransform = buttonsData[i].RectTransform;
            var v = rectTransform.anchoredPosition;
            rectTransform.anchoredPosition = new Vector2(v.x, startPos + DeltaPositionForAnimation);
            LeanTween.move(rectTransform, new Vector2(v.x, startPos), DefaultDurationSeconds).setDelay(delay).setEaseOutQuad();

            tweenDescription = LeanTween.alphaCanvas(buttonsData[i].CanvasGroup, 1, DefaultDurationSeconds).setDelay(delay);
        }
        if (tweenDescription != null)
        {
            tweenDescription.setOnComplete(() => resultTsc.TrySetResult());
        }
        else
        {
            resultTsc.TrySetResult();
        }

        return resultTsc.Task;
    }

    private UniTask HideButtonsInternalAsync(ButtonData[] buttonsData, bool withDelays = true)
    {
        var resultTsc = new UniTaskCompletionSource();
        void OnLastTweenComplete()
        {
            foreach (var buttonData in buttonsData)
            {
                var canvasGroup = buttonData.CanvasGroup;
                canvasGroup.alpha = 0;
                canvasGroup.gameObject.SetActive(false);
            }
            resultTsc.TrySetResult();
        }

        var targetPos = buttonsData[0].DefaultYPosition + DeltaPositionForAnimation;
        LTDescr tweenDescription = null;
        for (var i = 0; i < buttonsData.Length; i++)
        {
            if (buttonsData[i].RectTransform.gameObject.activeSelf == false) continue;

            var delay = withDelays ? i * DefaultDelaySeconds : 0;
            buttonsData[i].CanvasGroup.interactable = false;

            var rectTransform = buttonsData[i].RectTransform;
            var v = rectTransform.anchoredPosition;
            LeanTween.move(rectTransform, new Vector2(v.x, targetPos), DefaultDurationSeconds).setDelay(delay).setEaseInQuad();

            tweenDescription = LeanTween.alphaCanvas(buttonsData[i].CanvasGroup, 0, DefaultDurationSeconds).setDelay(delay);
        }
        if (tweenDescription != null)
        {
            tweenDescription.setOnComplete(OnLastTweenComplete);
        }
        else
        {
            resultTsc.TrySetResult();
        }

        return resultTsc.Task;
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

    private void OnBackButtonClicked()
    {
        InvokeActionIfNotBlocked(BackButtonClicked);
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

    private static ButtonData GetButtonData(Button button)
    {
        var canvasGroup = button.GetComponent<CanvasGroup>();
        var rectTransform = (RectTransform)button.transform;
        var yPos = rectTransform.anchoredPosition.y;
        
        return new ButtonData(rectTransform, canvasGroup, yPos);
    }
    
    private class ButtonData
    {
        public readonly RectTransform RectTransform;
        public readonly CanvasGroup CanvasGroup;
        public readonly float DefaultYPosition;
        
        public bool IsDisabled;

        public ButtonData(RectTransform rectTransform, CanvasGroup canvasGroup, float defaultYPosition)
        {
            RectTransform = rectTransform;
            CanvasGroup = canvasGroup;
            DefaultYPosition = defaultYPosition;
            IsDisabled = false;
        }
    }
}

public enum ActionModeType
{
    PlacingNewShopObject,
    MovingShopObject,
    PlacingShopDecoration,
    PlacingProduct,
    FriendAction,
}
