using System;
using UnityEngine;

public class TutorialMediator : IMediator
{
    private readonly RectTransform _parentTransform;
    private readonly GameStateModel _gameStateModel;

    private IMediator _currentTutorialStepMediator;

    public TutorialMediator(RectTransform parentTransform)
    {
        _parentTransform = parentTransform;

        _gameStateModel = GameStateModel.Instance;
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
        _gameStateModel.TutorialStepShown += OnTutorialStepShown;
        _gameStateModel.TutorialStepRemoved += OnTutorialStepRemoved;
    }

    private void Deactivate()
    {
        _gameStateModel.TutorialStepShown -= OnTutorialStepShown;
        _gameStateModel.TutorialStepRemoved -= OnTutorialStepRemoved;
    }

    private void OnTutorialStepShown()
    {
        var stepIndex = _gameStateModel.ShowingTutorialModel.StepIndex;
        _currentTutorialStepMediator = CreateTutorialStepMediator(stepIndex);
        _currentTutorialStepMediator.Mediate();
    }

    private void OnTutorialStepRemoved()
    {
        if (_currentTutorialStepMediator != null)
        {
            _currentTutorialStepMediator.Unmediate();
        }
    }

    private IMediator CreateTutorialStepMediator(int stepIndex)
    {
        return stepIndex switch
        {
            0 => new TutorialWelcomeStepMediator(_parentTransform),
            1 => new TutorialOpenWarehouseStepMediator(_parentTransform),
            2 => new TutorialOpenOrderPopupStepMediator(_parentTransform),
            3 => new TutorialOrderProductStepMediator(_parentTransform),
            _ => throw new ArgumentException($"No tutorial step with id {stepIndex}"),
        };
    }
}
