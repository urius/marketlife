using UnityEngine;

public class UIRootCanvasMediator : MonoBehaviour
{
    [SerializeField] private UIGameViewPanel _gameViewPanel;
    [SerializeField] private BottomPanelView _bottomPanelView;

    private Dispatcher _dispatcher;
    private BottomPanelMediator _bottomPanelMediator;

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
        _dispatcher.UIGameViewMouseDown();
    }

    private void OnPointerUp()
    {
        _dispatcher.UIGameViewMouseUp();
    }
}
