using System.Collections.Generic;
using System.Linq;

public abstract class UIBottomPanelInteriorTabMediatorBase<TViewModel> : UIBottomPanelSubMediatorBase
{
    private readonly Dictionary<UIBottomPanelScrollItemView, TViewModel> _viewModelsByItem;
    private TViewModel[] _viewModels;

    public UIBottomPanelInteriorTabMediatorBase(BottomPanelView view)
        : base(view)
    {
        _viewModelsByItem = new Dictionary<UIBottomPanelScrollItemView, TViewModel>();
    }

    public override void Mediate()
    {
        base.Mediate();

        ShowScrollBox();

        _viewModels = GetViewModelsToShow().ToArray();
        UIBottomPanelScrollItemView itemView;

        for (var i = 0; i < _viewModels.Length; i++)
        {
            var config = _viewModels[i];
            itemView = GetOrCreateScrollBoxItem();
            _viewModelsByItem.Add(itemView, config);
            SetupItem(itemView, config);
            ActivateItem(itemView);
        }
    }

    public override void Unmediate()
    {
        var itemViews = _viewModelsByItem.Keys;
        foreach (var itemView in itemViews)
        {
            DeactivateItem(itemView);
            ReturnOrDestroyScrollBoxItem(itemView);
        }
        _viewModelsByItem.Clear();

        HideScrollBox();

        base.Unmediate();
    }

    abstract protected IEnumerable<TViewModel> GetViewModelsToShow();
    abstract protected void SetupItem(UIBottomPanelScrollItemView itemView, TViewModel viewModel);
    abstract protected void HandleClick(TViewModel viewModel);

    private void ActivateItem(UIBottomPanelScrollItemView itemView)
    {
        itemView.Clicked += OnItemClicked;
    }

    private void DeactivateItem(UIBottomPanelScrollItemView itemView)
    {
        itemView.Clicked -= OnItemClicked;
    }

    private void OnItemClicked(UIBottomPanelScrollItemView itemView)
    {
        var viewModel = _viewModelsByItem[itemView];
        HandleClick(viewModel);
    }
}
