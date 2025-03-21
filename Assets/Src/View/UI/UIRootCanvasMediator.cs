using Src.Common;
using Src.Model;
using Src.View.UI.Bottom_Panel;
using Src.View.UI.Common;
using Src.View.UI.GameView_Panel;
using Src.View.UI.Left_Panel;
using Src.View.UI.Popups;
using Src.View.UI.Tutorial;
using UnityEngine;

namespace Src.View.UI
{
    public class UIRootCanvasMediator : MonoBehaviour
    {
        [SerializeField] private UIGameViewPanel _gameViewPanel;
        [SerializeField] private UISaveIconView _saveIconView;
        [SerializeField] private RectTransform _gameViewUiContainerRectTransform;
        [SerializeField] private BottomPanelView _bottomPanelView;
        [SerializeField] private UIGetDailyBonusButtonView _dailyBonusButtonView;
        [SerializeField] private UIDailyMissionsButtonView _dailyMissionsButtonView;
        [SerializeField] private Canvas _rootCanvas;
        [SerializeField] private Canvas _popupsCanvas;
        [SerializeField] private Canvas _tutorialCanvas;
        [SerializeField] private Canvas _poolCanvas;

        private const int ClickPositionSensitivity = 300;
        private const int ClickFramesSensitivity = 30;

        private Dispatcher _dispatcher;
        private GameStateModel _gameStateModel;
        private UISaveIconMediator _saveIconMediator;
        private BottomPanelMediator _bottomPanelMediator;
        private ShopObjectActionsPanelMediator _shopObjectActionsPanelMediator;
        private UIFlyingTextsMediator _flyingTextsMediator;
        private UIActionsCursorMediator _actionsCursorMediator;
        private PopupsMediator _popupsMediator;
        private TutorialMediator _tutorialMediator;
        private UIGetDailyBonusButtonMediator _dailyBonusButtonMediator;
        private UIDailyMissionsButtonMediator _dailyMissiosnButtonMediator;
        private PrefabsHolder _prefabsHolder;
        private Vector3 _mouseDownPosition;
        private int _mouseDownFramesCount;
        private GameObject _raycastsBlocker;

        private void Awake()
        {
            _dispatcher = Dispatcher.Instance;
            _gameStateModel = GameStateModel.Instance;
            _prefabsHolder = PrefabsHolder.Instance;

            _saveIconMediator = new UISaveIconMediator(_saveIconView);
            _bottomPanelMediator = new BottomPanelMediator(_bottomPanelView);
            _shopObjectActionsPanelMediator = new ShopObjectActionsPanelMediator(_gameViewUiContainerRectTransform);
            _flyingTextsMediator = new UIFlyingTextsMediator(_gameViewUiContainerRectTransform);
            var popupCanvasRectTransform = _popupsCanvas.transform as RectTransform;
            _actionsCursorMediator = new UIActionsCursorMediator(popupCanvasRectTransform);
            _popupsMediator = new PopupsMediator(popupCanvasRectTransform);
            _tutorialMediator = new TutorialMediator(_tutorialCanvas.transform as RectTransform);
            _dailyBonusButtonMediator = new UIGetDailyBonusButtonMediator(_dailyBonusButtonView);
            _dailyMissiosnButtonMediator = new UIDailyMissionsButtonMediator(_dailyMissionsButtonView);

            RootCanvasProvider.Instance.SetupRootCanvas(_rootCanvas);
            PoolCanvasProvider.Instance.SetupPoolCanvas(_poolCanvas);
        }

        private async void Start()
        {
            await _gameStateModel.GameDataLoadedTask;

            _gameViewPanel.PointerDown += OnPointerDown;
            _gameViewPanel.PointerUp += OnPointerUp;
            _gameViewPanel.PointerEnter += OnPointerEnter;
            _gameViewPanel.PointerExit += OnPointerExit;
            _dispatcher.UIRequestBlockRaycasts += OnUIRequestBlockRaycasts;
            _dispatcher.UIRequestUnblockRaycasts += OnUIRequestUnblockRaycasts;

            _saveIconMediator.Mediate();
            _bottomPanelMediator.Mediate();
            _shopObjectActionsPanelMediator.Mediate();
            _flyingTextsMediator.Mediate();
            _actionsCursorMediator.Mediate();
            _popupsMediator.Mediate();
            _tutorialMediator.Mediate();
            _dailyBonusButtonMediator.Mediate();
            _dailyMissiosnButtonMediator.Mediate();
        }

        private void OnUIRequestBlockRaycasts()
        {
            _raycastsBlocker = GameObject.Instantiate(_prefabsHolder.UIRaycastsBlocker, transform);
        }

        private void OnUIRequestUnblockRaycasts()
        {
            if (_raycastsBlocker != null)
            {
                GameObject.Destroy(_raycastsBlocker);
                _raycastsBlocker = null;
            }
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
}
