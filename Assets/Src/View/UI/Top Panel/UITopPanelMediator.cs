using UnityEngine;

public class UITopPanelMediator : MonoBehaviour
{
    [SerializeField] private UITopPanelBarView _crystalsBarView;
    [SerializeField] private UITopPanelBarView _cashBarView;
    [SerializeField] private UITopPanelBarView _expBarView;
    [SerializeField] private UITopPanelBarView _moodBarView;

    private GameStateModel _gameStateModel;
    private ShopModel _playerShopModel;

    private void Awake()
    {
        _gameStateModel = GameStateModel.Instance;
    }

    public void Start()
    {
        if (_gameStateModel.PlayerShopModel != null)
        {
            Initialize();
        }
        else
        {
            _gameStateModel.PlayerShopModelWasSet += OnPlayerShopMpdelSet;
        }
    }

    private void OnPlayerShopMpdelSet()
    {
        _gameStateModel.PlayerShopModelWasSet -= OnPlayerShopMpdelSet;
        Initialize();
    }

    private void Initialize()
    {
        _playerShopModel = _gameStateModel.PlayerShopModel;
        var progressModel = _playerShopModel.ProgressModel;

        _crystalsBarView.Amount = progressModel.Gold;
        _cashBarView.Amount = progressModel.Cash;
        _expBarView.Amount = progressModel.ExpAmount;
        //_moodBarView.Amount = progressModel; TODO: calculate mood
    }
}
