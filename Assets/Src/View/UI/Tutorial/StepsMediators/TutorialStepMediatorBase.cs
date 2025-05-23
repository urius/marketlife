using System;
using Src.Common;
using Src.Managers;
using UnityEngine;

public abstract class TutorialStepMediatorBase : IMediator
{
    private readonly RectTransform _parentTransform;
    private readonly PrefabsHolder _prefabsHodler;
    private readonly Dispatcher _dispatcher;
    private readonly LocalizationManager _loc;
    private readonly UpdatesProvider _updatesProvider;
    private readonly TutorialUIElementsProvider _tutorialUIElementsProvider;

    //
    private TutorialOverlayView _tutorialOverlayView;
    private TutorialStepViewModel _viewModel;
    private Camera _camera;
    private RectTransform _allowedClickOnRectTransform;
    private Rect _allowedClickOnRectArea;

    public TutorialStepMediatorBase(RectTransform parentTransform)
    {
        _parentTransform = parentTransform;

        _prefabsHodler = PrefabsHolder.Instance;
        _dispatcher = Dispatcher.Instance;
        _loc = LocalizationManager.Instance;
        _updatesProvider = UpdatesProvider.Instance;
        _tutorialUIElementsProvider = TutorialUIElementsProvider.Instance;
    }

    protected TutorialOverlayView View => _tutorialOverlayView;
    protected TutorialStepViewModel ViewModel => _viewModel;
    protected Camera Camera => _camera;

    public virtual void Mediate()
    {
        _viewModel = GameStateModel.Instance.ShowingTutorialModel;
        _camera = Camera.main;

        var tutorialOverlayGo = GameObject.Instantiate(_prefabsHodler.UITutorialOverlayPrefab, _parentTransform);
        _tutorialOverlayView = tutorialOverlayGo.GetComponent<TutorialOverlayView>();
        _tutorialOverlayView.Setup(_camera);

        _tutorialOverlayView.SetTitle(_loc.GetLocalization(LocalizationKeys.TutorialTitleDefault));
        SetupMessage();

        Activate();

        if (_viewModel.StepIndex % 2 == 0)
        {
            _tutorialOverlayView.PlaceManLeft();
        }
        else
        {
            _tutorialOverlayView.PlaceManRight();
        }
        _tutorialOverlayView.Appear(animateBgFlag: !_viewModel.IsImmediate, animatePopupFlag: true);
    }

    public virtual void Unmediate()
    {
        Deactivate();
        GameObject.Destroy(_tutorialOverlayView.gameObject);
    }

    public void AllowClickOnRectTransform(RectTransform rectTransform)
    {
        _allowedClickOnRectTransform = rectTransform;
        UnsubscribeAllowClickHandlers();
        _updatesProvider.RealtimeUpdate += UpdateClickAvailabilityOnRectTransform;
    }

    public void DispatchTutorialActionPerformed()
    {
        _dispatcher.TutorialActionPerformed();
    }

    protected virtual void OnViewButtonClicked()
    {
        DispatchTutorialActionPerformed();
    }

    protected virtual void SetupMessage()
    {
        _tutorialOverlayView.SetMessageText(_loc.GetLocalization($"{LocalizationKeys.TutorialMessagePrefix}{_viewModel.StepIndex}"));
    }

    protected void HighlightUIElement(TutorialUIElement elementType, float sizeXMultiplier = 1)
    {
        var rectTransform = _tutorialUIElementsProvider.GetElementRectTransform(elementType);
        var sideSize = Math.Max(rectTransform.rect.size.x, rectTransform.rect.size.y);
        View.HighlightScreenRoundArea(rectTransform.position, new Vector2(sideSize * sizeXMultiplier, sideSize), animated: true);
    }

    private void Activate()
    {
        _tutorialOverlayView.Clicked += OnViewButtonClicked;
    }

    private void Deactivate()
    {
        _tutorialOverlayView.Clicked -= OnViewButtonClicked;
        UnsubscribeAllowClickHandlers();
    }

    private void UnsubscribeAllowClickHandlers()
    {
        _updatesProvider.RealtimeUpdate -= UpdateClickAvailabilityOnRectTransform;
        _updatesProvider.RealtimeUpdate -= UpdateClickAvailabilityOnRectArea;
    }

    private void UpdateClickAvailabilityOnRectTransform()
    {
        var isMouseOverRect = RectTransformUtility.RectangleContainsScreenPoint(_allowedClickOnRectTransform, Input.mousePosition, _camera);
        _tutorialOverlayView.SetClickBlockState(!isMouseOverRect);
    }

    private void UpdateClickAvailabilityOnRectArea()
    {
        var mousePosition = Input.mousePosition;
        var isMouseOverRect = _allowedClickOnRectArea.Contains(new Vector2(mousePosition.x, mousePosition.y));
        _tutorialOverlayView.SetClickBlockState(!isMouseOverRect);
    }

}
