using Src.Managers;
using UnityEngine;

public class TutorialWelcomeStepMediator : TutorialStepMediatorBase
{
    private readonly LocalizationManager _loc;

    public TutorialWelcomeStepMediator(RectTransform parentTransform)
        : base(parentTransform)
    {
        _loc = LocalizationManager.Instance;
    }

    public override void Mediate()
    {
        base.Mediate();
        View.SetButtonText(_loc.GetLocalization($"{LocalizationKeys.TutorialButtonPrefix}{ViewModel.StepIndex}"));
    }
}
