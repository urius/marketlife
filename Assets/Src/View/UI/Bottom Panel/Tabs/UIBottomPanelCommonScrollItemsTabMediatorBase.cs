namespace Src.View.UI.Bottom_Panel.Tabs
{
    public abstract class UIBottomPanelCommonScrollItemsTabMediatorBase<TViewModel> : UIBottomPanelScrollItemsTabMediatorBase<UIBottomPanelScrollItemView, TViewModel>
    {
        private readonly ViewsCache _viewsCache;

        protected UIBottomPanelCommonScrollItemsTabMediatorBase(BottomPanelView view)
            : base(view)
        {
            _viewsCache = ViewsCache.Instance;
        }

        protected override UIBottomPanelScrollItemView GetOrCreateItem()
        {
            var result = _viewsCache.GetOrCreateDefaultScrollBoxItem();
            result.Reset();
            return result;
        }

        protected override void ReturnOrDestroyScrollBoxItem(UIBottomPanelScrollItemView itemView)
        {
            _viewsCache.ReturnOrDestroyScrollBoxItem(itemView);
        }
    }

    public abstract class UIBottomPanelInteriorModeScrollItemsTabMediatorBase<TViewModel> : UIBottomPanelCommonScrollItemsTabMediatorBase<TViewModel>
    {
        protected UIBottomPanelInteriorModeScrollItemsTabMediatorBase(BottomPanelView view)
            : base(view)
        {
        }

        protected override UIBottomPanelScrollItemView GetOrCreateItem()
        {
            var result = base.GetOrCreateItem();
            result.SetSkinBlue();
            return result;
        }
    }
}