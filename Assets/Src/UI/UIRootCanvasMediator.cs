using UnityEngine;

public class UIRootCanvasMediator : MonoBehaviour
{
    [SerializeField] private UIGameViewPanel _gameViewPanel;

    private Dispatcher _dispatcher;

    private void Awake()
    {
        _dispatcher = Dispatcher.Instance;
    }

    private void Start()
    {
        _gameViewPanel.PointerDown += OnPointerDown;
        _gameViewPanel.PointerUp += OnPointerUp;
    }

    private void OnPointerDown()
    {
        _dispatcher.GameViewMouseDown();
    }

    private void OnPointerUp()
    {
        _dispatcher.GameViewMouseUp();
    }
}
