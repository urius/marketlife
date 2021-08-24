using UnityEngine;

public class UIBottomPanelTabMediatorBase : UINotMonoMediatorBase
{
    protected readonly BottomPanelView View;

    public UIBottomPanelTabMediatorBase(BottomPanelView view)
        : base(view.transform as RectTransform)
    {
        View = view;
    }
}
