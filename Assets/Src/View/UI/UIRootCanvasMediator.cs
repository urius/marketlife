using UnityEngine;

public class UIRootCanvasMediator : MonoBehaviour
{
    [SerializeField] private UIGameViewPanel _gameViewPanel;
    [SerializeField] private BottomPanelView _bottomPanelView;

    private const int ClickPositionSensitivity = 30;
    private const int ClickFramesSensitivity = 30;

    private Dispatcher _dispatcher;
    private BottomPanelMediator _bottomPanelMediator;

    private Vector3 _mouseDownPosition;
    private int _mouseDownFramesCount;

    private void Awake()
    {
        _dispatcher = Dispatcher.Instance;
        _bottomPanelMediator = new BottomPanelMediator(_bottomPanelView);
    }

    private async void Start()
    {
        await GameStateModel.Instance.GameDataLoadedTask;

        _gameViewPanel.PointerDown += OnPointerDown;
        _gameViewPanel.PointerUp += OnPointerUp;

        _bottomPanelMediator.Mediate();
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
