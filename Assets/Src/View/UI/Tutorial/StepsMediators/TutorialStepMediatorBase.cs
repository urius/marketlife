using System;
using UnityEngine;

public abstract class TutorialStepMediatorBase : IMediator
{
    private readonly RectTransform _parentTransform;
    private readonly PrefabsHolder _prefabsHodler;

    private TutorialOverlayView _tutorialOverlayView;

    public TutorialStepMediatorBase(RectTransform parentTransform)
    {
        _parentTransform = parentTransform;

        _prefabsHodler = PrefabsHolder.Instance;
    }

    protected TutorialOverlayView View => _tutorialOverlayView;

    public virtual void Mediate()
    {
        var tutorialOverlayGo = GameObject.Instantiate(_prefabsHodler.UITutorialOverlayPrefab, _parentTransform);
        _tutorialOverlayView = tutorialOverlayGo.GetComponent<TutorialOverlayView>();

        Activate();
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
        throw new NotImplementedException();
    }
}
