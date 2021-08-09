using System;
using UnityEngine;

public abstract class TutorialStepMediatorBase : IMediator
{
    private readonly RectTransform _parentTransform;
    private readonly PrefabsHolder _prefabsHodler;
    private readonly Dispatcher _dispatcher;
    private readonly LocalizationManager _loc;

    private TutorialOverlayView _tutorialOverlayView;
    private TutorialStepViewModel _viewModel;

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

        var tutorialOverlayGo = GameObject.Instantiate(_prefabsHodler.UITutorialOverlayPrefab, _parentTransform);
        _tutorialOverlayView = tutorialOverlayGo.GetComponent<TutorialOverlayView>();
        _tutorialOverlayView.Setup(Camera.main);

        _tutorialOverlayView.SetTitle(_loc.GetLocalization(LocalizationKeys.TutorialTitleDefault));
        View.SetMessageText(_loc.GetLocalization($"{LocalizationKeys.TutorialMessagePrefix}{_viewModel.StepIndex}"));

        Activate();

        _tutorialOverlayView.Appear(animateBgFlag: !_viewModel.IsImmediate, true);
    }

    public virtual void Unmediate()
    {
        Deactivate();
        GameObject.Destroy(_tutorialOverlayView.gameObject);
    }

    private void Activate()
    {
        _tutorialOverlayView.Clicked += OnViewButtonClicked;
    }

    private void Deactivate()
    {
        _tutorialOverlayView.Clicked -= OnViewButtonClicked;
    }

    private void OnViewButtonClicked()
    {
        _dispatcher.TutorialActionPerformed();
    }
}
