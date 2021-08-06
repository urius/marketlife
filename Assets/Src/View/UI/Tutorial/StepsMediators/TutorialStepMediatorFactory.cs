using System;
using UnityEngine;

public struct TutorialStepMediatorFactory
{
    public IMediator Create(int stepIndex, RectTransform contentTransform)
    {
        return stepIndex switch
        {
            0 => new TutorialWelcomeStepMediator(contentTransform),
            1 => new TutorialOpenWarehouseStepMediator(contentTransform),
            _ => throw new ArgumentException($"No tutorial step with id {stepIndex}"),
        };
    }
}
