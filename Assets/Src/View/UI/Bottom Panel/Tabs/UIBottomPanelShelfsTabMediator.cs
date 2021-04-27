using System.Collections.Generic;
using UnityEngine;

public class UIBottomPanelShelfsTabMediator : UINotMonoMediatorBase<BottomPanelView>
{
    private readonly IShelfsConfig _shelfsConfig;
    private readonly PlayerModel _playerModel;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly SpritesProvider _spritesProvider;
    private readonly Dispatcher _dispatcher;
    private readonly Dictionary<UIBottomPanelScrollItemView, int> _shelfIdsByItem;

    public UIBottomPanelShelfsTabMediator()
    {
        _shelfsConfig = GameConfigManager.Instance.MainConfig;
        _playerModel = PlayerModel.Instance;
        _prefabsHolder = PrefabsHolder.Instance;
        _spritesProvider = SpritesProvider.Instance;
        _dispatcher = Dispatcher.Instance;

        _shelfIdsByItem = new Dictionary<UIBottomPanelScrollItemView, int>();
    }

    protected override void OnStart()
    {
        var configsToShow = _shelfsConfig.GetShelfConfigsForLevel(_playerModel.Level);

        foreach (var shelfConfig in configsToShow)
        {
            var itemGo = GameObject.Instantiate(_prefabsHolder.UIBottomPanelScrollItem, View.ScrollBoxView.Content);
            var itemView = itemGo.GetComponent<UIBottomPanelScrollItemView>();
            itemView.SetImage(_spritesProvider.GetShelfIcon(shelfConfig.id));
            itemView.SetPrice(Price.FromString(shelfConfig.config.price));

            _shelfIdsByItem.Add(itemView, shelfConfig.id);

            itemView.Clicked += OnItemClicked;
        }
    }

    protected override void OnStop()
    {
        var items = _shelfIdsByItem.Keys;
        foreach (var item in items)
        {
            item.Clicked -= OnItemClicked;
            GameObject.Destroy(item.gameObject);
        }
    }

    private void OnItemClicked(UIBottomPanelScrollItemView itemView)
    {
        var shelfId = _shelfIdsByItem[itemView];
        _dispatcher.UIBottomPanelPlaceShelfClicked(shelfId);
    }
}
