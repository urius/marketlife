using UnityEngine;

public class BottomPanelMediator : UINotMonoMediatorBase
{
    private readonly BottomPanelView _view;

    private UINotMonoMediatorBase _currentTabMediator;

    public BottomPanelMediator(BottomPanelView view)
        : base(view.transform as RectTransform)
    {
        _view = view;
    }

    public override void Mediate()
    {
        base.Mediate();

        _currentTabMediator = new UIBottomPanelShelfsTabMediator(_view);
        _currentTabMediator.Mediate();
    }

    public override void Unmediate()
    {
        base.Unmediate();
    }
}
