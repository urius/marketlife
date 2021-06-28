using UnityEngine;

public class UIRootCanvasMediator : MonoBehaviour
{
    [SerializeField] private UIGameViewPanel _gameViewPanel;
    [SerializeField] private UISaveIconView _saveIconView;
    [SerializeField] private RectTransform _gameViewUiContainerRectTransform;
    [SerializeField] private BottomPanelView _bottomPanelView;
    [SerializeField] private Canvas _popupsCanvas;
    [SerializeField] private Canvas _poolCanvas;

    private const int ClickPositionSensitivity = 30;
    private const int ClickFramesSensitivity = 30;

    private Dispatcher _dispatcher;
    private GameStateModel _gameStateModel;
    private UISaveIconMediator _saveIconMediator;
    private BottomPanelMediator _bottomPanelMediator;
    private ShopObjectActionsPanelMediator _shopObjectActionsPanelMediator;
    private UIFlyingTextsMediator _flyingTextsMediator;
    private PlacingProductMediator _placingProductMediator;
    private PopupsMediator _popupsMediator;
    private Vector3 _mouseDownPosition;
    private int _mouseDownFramesCount;

    private void Awake()
    {
        _dispatcher = Dispatcher.Instance;
        _gameStateModel = GameStateModel.Instance;

        _saveIconMediator = new UISaveIconMediator(_saveIconView);
        _bottomPanelMediator = new BottomPanelMediator(_bottomPanelView);
        _shopObjectActionsPanelMediator = new ShopObjectActionsPanelMediator(_gameViewUiContainerRectTransform);
        _flyingTextsMediator = new UIFlyingTextsMediator(_gameViewUiContainerRectTransform);
        var popupCanvasRectTransform = _popupsCanvas.transform as RectTransform;
        _placingProductMediator = new PlacingProductMediator(popupCanvasRectTransform);
        _popupsMediator = new PopupsMediator(popupCanvasRectTransform);

        PoolCanvasProvider.Instance.SetupPoolCanvas(_poolCanvas);
    }

    private async void Start()
    {
        await _gameStateModel.GameDataLoadedTask;

        _gameViewPanel.PointerDown += OnPointerDown;
        _gameViewPanel.PointerUp += OnPointerUp;
        _gameViewPanel.PointerEnter += OnPointerEnter;
        _gameViewPanel.PointerExit += OnPointerExit;

        _saveIconMediator.Mediate();
        _bottomPanelMediator.Mediate();
        _shopObjectActionsPanelMediator.Mediate();
        _flyingTextsMediator.Mediate();
        _placingProductMediator.Mediate();
        _popupsMediator.Mediate();
    }

    private void OnPointerEnter()
    {
        _dispatcher.UIGameViewMouseEnter();
    }

    private void OnPointerExit()
    {
        _dispatcher.UIGameViewMouseExit();
    }

    private void OnPointerDown()
    {
        _mouseDownPosition = Input.mousePosition;
        _mouseDownFramesCount = Time.frameCount;

        _dispatcher.UIGameViewMouseDown();
    }

    private void OnPointerUp()
    {
        _dispatcher.UIGameViewMouseUp();

        if (Vector2.Distance(_mouseDownPosition, Input.mousePosition) <= ClickPositionSensitivity
            && Time.frameCount - _mouseDownFramesCount <= ClickFramesSensitivity)
        {
            _dispatcher.UIGameViewMouseClick();
        }
    }
}
