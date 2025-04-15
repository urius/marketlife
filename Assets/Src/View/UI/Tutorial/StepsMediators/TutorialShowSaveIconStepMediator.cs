using Src.Common;
using Src.Managers;
using UnityEngine;

public class TutorialShowSaveIconStepMediator : TutorialStepMediatorBase
{
    private readonly LocalizationManager _loc;
    private readonly Dispatcher _dispatcher;

    public TutorialShowSaveIconStepMediator(RectTransform parentTransform)
        : base(parentTransform)
    {
        _loc = LocalizationManager.Instance;
        _dispatcher = Dispatcher.Instance;
    }

    public override void Mediate()
    {
        base.Mediate();
        View.SetButtonText(_loc.GetLocalization($"{LocalizationKeys.TutorialButtonPrefix}{ViewModel.StepIndex}"));
        View.SetQuadrant(1);

        _dispatcher.TutorialSaveStateChanged(true);
        HighlightUIElement(TutorialUIElement.TopSaveIcon, 1.2f);
    }

    public override void Unmediate()
    {
        _dispatcher.TutorialSaveStateChanged(false);
        base.Unmediate();
    }
}
