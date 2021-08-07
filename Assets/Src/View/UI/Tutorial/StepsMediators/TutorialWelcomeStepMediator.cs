using UnityEngine;

public class TutorialWelcomeStepMediator : TutorialStepMediatorBase
{
    private readonly LocalizationManager _loc;
    private readonly GameStateModel _gameStateModel;

    public TutorialWelcomeStepMediator(RectTransform parentTransform)
        : base(parentTransform)
    {
        _loc = LocalizationManager.Instance;
        _gameStateModel = GameStateModel.Instance;
    }

    public override void Mediate()
    {
        base.Mediate();

        var stepIndex = _gameStateModel.ShowingTutorialModel.StepIndex;
        View.SetTexts(_loc.GetLocalization(LocalizationKeys.TutorialTitleDefault),
            _loc.GetLocalization($"{LocalizationKeys.TutorialMessagePrefix}{stepIndex}"),
            _loc.GetLocalization($"{LocalizationKeys.TutorialButtonPrefix}{stepIndex}"));
    }

    public override void Unmediate()
    {
        base.Unmediate();
    }
}
