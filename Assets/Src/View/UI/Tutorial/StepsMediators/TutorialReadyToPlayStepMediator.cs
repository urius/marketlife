using UnityEngine;

public class TutorialReadyToPlayStepMediator : TutorialStepMediatorBase
{
    private readonly LocalizationManager _loc;

    private int _phase = 0;

    public TutorialReadyToPlayStepMediator(RectTransform parentTransform)
        : base(parentTransform)
    {
        _loc = LocalizationManager.Instance;
    }

    public override void Mediate()
    {
        base.Mediate();
        View.SetButtonText(_loc.GetLocalization($"{LocalizationKeys.TutorialButtonPrefix}{ViewModel.StepIndex}"));
    }

    protected override void SetupMessage()
    {
        View.SetMessageText(_loc.GetLocalization($"{LocalizationKeys.TutorialMessagePrefix}{ViewModel.StepIndex}"));
    }

    protected override void OnViewButtonClicked()
    {
        if (_phase == 0)
        {
            _phase++;
            var messageFormat = _loc.GetLocalization($"{LocalizationKeys.TutorialMessagePrefix}{ViewModel.StepIndex}_a");            ;
            View.SetMessageText(string.Format(messageFormat, _loc.GetLocalization(LocalizationKeys.BottomPanelInteriorButton)));
            View.SetButtonText(_loc.GetLocalization($"{LocalizationKeys.TutorialButtonPrefix}{ViewModel.StepIndex}_a"));
        }
        else
        {
            base.OnViewButtonClicked();
        }
    }
}
