using Src.Managers;
using UnityEngine;

namespace Src.View.UI.Tutorial.StepsMediators
{
    public class TutorialReadyToPlayStepMediator : TutorialStepMediatorBase
    {
        private readonly LocalizationManager _loc;

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
    }
}
