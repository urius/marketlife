using Src.View.UI.Common;
using UnityEngine;

namespace Src.View.UI.Bottom_Panel.Tabs
{
    public class UIBottomPanelTabMediatorBase : UINotMonoMediatorBase
    {
        protected readonly BottomPanelView View;

        public UIBottomPanelTabMediatorBase(BottomPanelView view)
            : base(view.transform as RectTransform)
        {
            View = view;
        }
    }
}
