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
        return (TutorialStep)stepIndex switch
        {
            TutorialStep.Welcome => new TutorialWelcomeStepMediator(_parentTransform),
            TutorialStep.OpenWarehouse => new TutorialOpenWarehouseStepMediator(_parentTransform),
            TutorialStep.OpenOrderPopup => new TutorialOpenOrderPopupStepMediator(_parentTransform),
            TutorialStep.OrderProduct => new TutorialOrderProductStepMediator(_parentTransform),
            TutorialStep.Delivering => new TutorialDeliveringStepMediator(_parentTransform),
            TutorialStep.PlacingProduct => new TutorialPlacingProductStepMediator(_parentTransform),
            TutorialStep.FinishPlacingProduct => new TutorialFinishPlacingProductStepMediator(_parentTransform),
            TutorialStep.ShowMoodInteriorAndFriendsUI => new TutorialShowMoodInteriorFriendsMediator(_parentTransform),
            TutorialStep.ShowSaveIcon => new TutorialShowSaveIconStepMediator(_parentTransform),
            TutorialStep.ReadyToPlay => new TutorialReadyToPlayStepMediator(_parentTransform),
            TutorialStep.FriendUI => new TutorialFriendUIStepMediator(_parentTransform),
            TutorialStep.Billboard => new TutorialBillboardStepMediator(_parentTransform),
            _ => throw new ArgumentException($"No tutorial step with id {stepIndex}"),
        };
    }
}
