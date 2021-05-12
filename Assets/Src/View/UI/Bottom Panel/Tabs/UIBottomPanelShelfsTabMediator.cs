using System.Collections.Generic;
using UnityEngine;

public class UIBottomPanelShelfsTabMediator : UIBottomPanelSubMediatorBase
{
    private readonly IShelfsConfig _shelfsConfig;
    private readonly PlayerModel _playerModel;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly SpritesProvider _spritesProvider;
    private readonly Dispatcher _dispatcher;
    private readonly Dictionary<UIBottomPanelScrollItemView, int> _shelfIdsByItem;

    public UIBottomPanelShelfsTabMediator(BottomPanelView view)
        : base(view)
    {
        _shelfsConfig = GameConfigManager.Instance.MainConfig;
        _playerModel = PlayerModel.Instance;
        _prefabsHolder = PrefabsHolder.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _dispatcher = Dispatcher.Instance;

        _shelfIdsByItem = new Dictionary<UIBottomPanelScrollItemView, int>();
    }

    public override void Mediate()
    {
        base.Mediate();

        var configsToShow = _shelfsConfig.GetShelfConfigsForLevel(_playerModel.Level);

        View.ScrollBoxView.gameObject.SetActive(true);
        foreach (var shelfConfig in configsToShow)
        {
            var itemGo = GameObject.Instantiate(_prefabsHolder.UIBottomPanelScrollItemPrefab, View.ScrollBoxView.Content);
            var itemView = itemGo.GetComponent<UIBottomPanelScrollItemView>();
            itemView.SetImage(_spritesProvider.GetShelfIcon(shelfConfig.id));
            itemView.SetPrice(Price.FromString(shelfConfig.config.price));

            _shelfIdsByItem.Add(itemView, shelfConfig.id);

            itemView.Clicked += OnItemClicked;
        }
    }

    public override void Unmediate()
    {
        var items = _shelfIdsByItem.Keys;
        foreach (var item in items)
        {
            item.Clicked -= OnItemClicked;
            GameObject.Destroy(item.gameObject);
        }
        View.ScrollBoxView.gameObject.SetActive(false);

        base.Unmediate();
    }

    private void OnItemClicked(UIBottomPanelScrollItemView itemView)
    {
        var shelfId = _shelfIdsByItem[itemView];
        _dispatcher.UIBottomPanelPlaceShelfClicked(shelfId);
    }
}
