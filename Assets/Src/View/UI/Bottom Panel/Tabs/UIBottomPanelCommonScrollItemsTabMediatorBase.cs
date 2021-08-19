public abstract class UIBottomPanelCommonScrollItemsTabMediatorBase<TViewModel> : UIBottomPanelScrollItemsTabMediatorBase<UIBottomPanelScrollItemView, TViewModel>
{
    private readonly ViewsCache _viewsCache;

    protected UIBottomPanelCommonScrollItemsTabMediatorBase(BottomPanelView view) : base(view)
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
