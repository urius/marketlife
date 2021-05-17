using System.Collections.Generic;

public abstract class UIBottomPanelInteriorTabMediatorBase<T> : UIBottomPanelSubMediatorBase
    where T : PlacableItemConfigDto
{
    private readonly PlayerModel _playerModel;
    private readonly Dictionary<UIBottomPanelScrollItemView, int> _IdsByItem;

    public UIBottomPanelInteriorTabMediatorBase(BottomPanelView view)
        : base(view)
    {
        _playerModel = PlayerModel.Instance;

        _IdsByItem = new Dictionary<UIBottomPanelScrollItemView, int>();
    }

    public override void Mediate()
    {
        base.Mediate();

        ShowScrollBox();

        var configsToShow = GetConfigsForLevel(_playerModel.Level);
        foreach (var config in configsToShow)
        {
            var itemView = GetOrCreateScrollBoxItem();
            _IdsByItem.Add(itemView, config.id);
            SetupItem(itemView, config);
            ActivateItem(itemView);
        }
    }

    public override void Unmediate()
    {
        var itemViews = _IdsByItem.Keys;
        foreach (var itemView in itemViews)
        {
            DeactivateItem(itemView);
            ReturnOrDestroyScrollBoxItem(itemView);
        }

        HideScrollBox();

        base.Unmediate();
    }

    abstract protected IEnumerable<(int id, T config)> GetConfigsForLevel(int level);
    abstract protected void SetupItem(UIBottomPanelScrollItemView itemView, (int id, T config) configData);
    abstract protected void HandleClick(int id);

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
        var id = _IdsByItem[itemView];
        HandleClick(id);
    }
}
