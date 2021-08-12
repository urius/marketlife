using UnityEngine;

public abstract class TutorialStepMediatorBase : IMediator
{
    private readonly RectTransform _parentTransform;
    private readonly PrefabsHolder _prefabsHodler;
    private readonly Dispatcher _dispatcher;
    private readonly LocalizationManager _loc;

    private TutorialOverlayView _tutorialOverlayView;
    private TutorialStepViewModel _viewModel;
    private UpdatesProvider _updatesProvider;
    private Camera _camera;
    private RectTransform _allowedClickOnRectTransform;

    public TutorialStepMediatorBase(RectTransform parentTransform)
    {
        _parentTransform = parentTransform;

        _prefabsHodler = PrefabsHolder.Instance;
        _dispatcher = Dispatcher.Instance;
        _loc = LocalizationManager.Instance;
    }

    protected TutorialOverlayView View => _tutorialOverlayView;
    protected TutorialStepViewModel ViewModel => _viewModel;

    public virtual void Mediate()
    {
        _viewModel = GameStateModel.Instance.ShowingTutorialModel;
        _updatesProvider = UpdatesProvider.Instance;
        _camera = Camera.main;

        var tutorialOverlayGo = GameObject.Instantiate(_prefabsHodler.UITutorialOverlayPrefab, _parentTransform);
        _tutorialOverlayView = tutorialOverlayGo.GetComponent<TutorialOverlayView>();
        _tutorialOverlayView.Setup(_camera);

        _tutorialOverlayView.SetTitle(_loc.GetLocalization(LocalizationKeys.TutorialTitleDefault));
        _tutorialOverlayView.SetMessageText(_loc.GetLocalization($"{LocalizationKeys.TutorialMessagePrefix}{_viewModel.StepIndex}"));

        Activate();

        _tutorialOverlayView.Appear(animateBgFlag: !_viewModel.IsImmediate, true);
    }

    public virtual void Unmediate()
    {
        Deactivate();
        GameObject.Destroy(_tutorialOverlayView.gameObject);
    }

    public void AllowClickOnRectTransform(RectTransform rectTransform)
    {
        _allowedClickOnRectTransform = rectTransform;
        _updatesProvider.RealtimeUpdate += HandleClickOnRectTransform;
    }

    public void DispatchTutorialActionPerformed()
    {
        _dispatcher.TutorialActionPerformed();
    }

    private void Activate()
    {
        _tutorialOverlayView.Clicked += OnViewButtonClicked;
    }

    private void Deactivate()
    {
        _tutorialOverlayView.Clicked -= OnViewButtonClicked;
        _updatesProvider.RealtimeUpdate -= HandleClickOnRectTransform;
    }

    private void HandleClickOnRectTransform()
    {
        var isMouseOverRect = RectTransformUtility.RectangleContainsScreenPoint(_allowedClickOnRectTransform, Input.mousePosition, _camera);
        _tutorialOverlayView.SetClickBlockState(!isMouseOverRect);
    }

    private void OnViewButtonClicked()
    {
        DispatchTutorialActionPerformed();
    }
}
