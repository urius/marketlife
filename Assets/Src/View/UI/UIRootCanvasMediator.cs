using System;
using UnityEngine;

public class UIRootCanvasMediator : MonoBehaviour
{
    [SerializeField] private UIGameViewPanel _gameViewPanel;
    [SerializeField] private RectTransform _gameViewUiContainerRectTransform;
    [SerializeField] private BottomPanelView _bottomPanelView;
    [SerializeField] private Canvas _popupsCanvas;
    [SerializeField] private Canvas _poolCanvas;

    private const int ClickPositionSensitivity = 30;
    private const int ClickFramesSensitivity = 30;

    private Dispatcher _dispatcher;
    private GameStateModel _gameStateModel;
    private BottomPanelMediator _bottomPanelMediator;
    private ShopObjectActionsPanelMediator _shopObjectActionsPanelMediator;
    private UIFlyingTextsMediator _flyingTextsMediator;
    private PopupsMediator _popupsMediator;
    private Vector3 _mouseDownPosition;
    private int _mouseDownFramesCount;

    private void Awake()
    {
        _dispatcher = Dispatcher.Instance;
        _gameStateModel = GameStateModel.Instance;

        _bottomPanelMediator = new BottomPanelMediator(_bottomPanelView);
        _shopObjectActionsPanelMediator = new ShopObjectActionsPanelMediator(_gameViewUiContainerRectTransform);
        _flyingTextsMediator = new UIFlyingTextsMediator(_gameViewUiContainerRectTransform);
        _popupsMediator = new PopupsMediator(_popupsCanvas.transform as RectTransform);

        PoolCanvasProvider.Instance.SetupPoolCanvas(_poolCanvas);
    }

    private async void Start()
    {
        await _gameStateModel.GameDataLoadedTask;

        _gameViewPanel.PointerDown += OnPointerDown;
        _gameViewPanel.PointerUp += OnPointerUp;
        _gameViewPanel.PointerEnter += OnPointerEnter;
        _gameViewPanel.PointerExit += OnPointerExit;

        _bottomPanelMediator.Mediate();
        _shopObjectActionsPanelMediator.Mediate();
        _flyingTextsMediator.Mediate();
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
