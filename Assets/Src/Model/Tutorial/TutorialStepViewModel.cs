public class TutorialStepViewModel
{
    public readonly int StepIndex;
    public readonly bool IsImmediate;

    public TutorialStepViewModel(int stepIndex, bool isImmediate = false)
    {
        StepIndex = stepIndex;
        IsImmediate = isImmediate;
    }
}
