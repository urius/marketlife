using System;
using UnityEngine;

public class TutorialMediator : IMediator
{
    private readonly RectTransform _parentTransform;
    private readonly Dispatcher _dispatcher;

    private IMediator _currentTutorialStepMediator;

    public TutorialMediator(RectTransform parentTransform)
    {
        _parentTransform = parentTransform;

        _dispatcher = Dispatcher.Instance;
    }

    public void Mediate()
    {
        Activate();
    }

    public void Unmediate()
    {
        Deactivate();
    }

    private void Activate()
    {
        _dispatcher.UIRequestShowTutorialStep += OnUIRequestShowTutorialStep;
    }

    private void Deactivate()
    {
        _dispatcher.UIRequestShowTutorialStep -= OnUIRequestShowTutorialStep;
    }

    private void OnUIRequestShowTutorialStep(int stepIndex)
    {
        if (_currentTutorialStepMediator != null)
        {
            _currentTutorialStepMediator.Unmediate();
        }

        _currentTutorialStepMediator = CreateTutorialStepMediator(stepIndex);
    }

    private IMediator CreateTutorialStepMediator(int stepIndex)
    {
        return stepIndex switch
        {
            0 => new TutorialWelcomeStepMediator(_parentTransform),
            1 => new TutorialOpenWarehouseStepMediator(_parentTransform),
            _ => throw new ArgumentException($"No tutorial step with id {stepIndex}"),
        };
    }
}
