using UnityEngine;

public class UIBottomPanelSubMediatorBase : UINotMonoMediatorBase
{
    protected readonly BottomPanelView View;

    public UIBottomPanelSubMediatorBase(BottomPanelView view)
        : base (view.transform as RectTransform)
    {
        View = view;
    }
}
