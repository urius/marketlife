namespace Src.Model.Tutorial
{
    public class TutorialStepViewModel
    {
        public readonly int StepIndex;
        public readonly bool IsImmediate;

        public TutorialStepViewModel(int stepIndex, bool isImmediate)
        {
            StepIndex = stepIndex;
            IsImmediate = isImmediate;
        }
    }
}
