using Src.Common;
using Src.Managers;
using UnityEngine;

namespace Src.View.UI.Tutorial.StepsMediators
{
    public class TutorialShowMoodInteriorFriendsMediator : TutorialStepMediatorBase
    {
        private readonly LocalizationManager _loc;

        private int _phaseIndex = 0;

        public TutorialShowMoodInteriorFriendsMediator(RectTransform parentTransform)
            : base(parentTransform)
        {
            _loc = LocalizationManager.Instance;
        }

        public override void Mediate()
        {
            base.Mediate();

            View.SetButtonText(_loc.GetLocalization($"{LocalizationKeys.TutorialButtonPrefix}{ViewModel.StepIndex}"));
            View.SetQuadrant(1);
            HighlightMoodBar();
        }

        protected override void OnViewButtonClicked()
        {
            ProcessNextPhase();
        }

        private void ProcessNextPhase()
        {
            _phaseIndex++;
            switch (_phaseIndex)
            {
                case 1:
                    HandleInteriorPhase();
                    break;
                case 2:
                    if (DisabledLogicFlags.IsFriendsLogicDisabled)
                    {
                        ProcessNextPhase();
                    }
                    else
                    {
                        HandleFriendsPhase();
                    }

                    break;
                case 3:
                    HandleManagePhase();
                    break;
                default:
                    base.OnViewButtonClicked();
                    break;
            }
        }

        private void HandleInteriorPhase()
        {
            var (message, buttonText) = GetTextsForCurrentPhase();
            View.SetMessageText(string.Format(message, _loc.GetLocalization(LocalizationKeys.BottomPanelInteriorButton)));
            View.SetButtonText(buttonText);
            View.ToRight();
            HighlightUIElement(TutorialUIElement.BottomPanelInteriorButton);
        }

        private void HandleFriendsPhase()
        {
            var (message, buttonText) = GetTextsForCurrentPhase();
            View.SetMessageText(string.Format(message, _loc.GetLocalization(LocalizationKeys.BottomPanelFriendsButton)));
            View.SetButtonText(buttonText);
            View.ToLeft();
            HighlightUIElement(TutorialUIElement.BottomPanelFriendsButton);
        }

        private void HandleManagePhase()
        {
            var (message, buttonText) = GetTextsForCurrentPhase();
            View.SetMessageText(string.Format(message, _loc.GetLocalization(LocalizationKeys.BottomPanelManageButton)));
            View.SetButtonText(buttonText);
            View.ToRight();
            HighlightUIElement(TutorialUIElement.BottomPanelManageButton);
        }

        private (string message, string buttonText) GetTextsForCurrentPhase()
        {
            var message = _loc.GetLocalization($"{LocalizationKeys.TutorialMessagePrefix}{ViewModel.StepIndex}_phase_{_phaseIndex}");
            var buttonText = _loc.GetLocalization($"{LocalizationKeys.TutorialButtonPrefix}{ViewModel.StepIndex}_phase_{_phaseIndex}");
            return (message, buttonText);
        }

        private void HighlightMoodBar()
        {
            HighlightUIElement(TutorialUIElement.TopPanelMoodBar, 2.5f);
        }
    }
}
