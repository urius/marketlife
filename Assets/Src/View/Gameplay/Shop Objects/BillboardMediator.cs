using System;
using UnityEngine;

public class BillboardMediator : IMediator
{
    private readonly Transform _contentTransform;
    private readonly GameStateModel _gameStateModel;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly GridCalculator _gridCalculator;
    private readonly Vector2Int _billboardCoords;
    //
    private BillboardView _billboardView;
    private ShopModel _currentShopModel;

    public BillboardMediator(Transform contentTransform, Vector2Int coords)
    {
        _contentTransform = contentTransform;

        _gameStateModel = GameStateModel.Instance;
        _prefabsHolder = PrefabsHolder.Instance;
        _gridCalculator = GridCalculator.Instance;
        _billboardCoords = coords;
    }

    public void Mediate()
    {
        Activate();

        if (_gameStateModel.ViewingShopModel != null)
        {
            HandleNewShopModel(_gameStateModel.ViewingShopModel);
        }
    }

    public void Unmediate()
    {
        Deactivate();

        ForgetCurrentShopModel();
        if (_billboardView != null)
        {
            GameObject.Destroy(_billboardView.gameObject);
            _billboardView = null;
        }
    }

    private void Activate()
    {
        _gameStateModel.ViewingUserModelChanged += OnViewingUserModelChanged;
    }

    private void Deactivate()
    {
        _gameStateModel.ViewingUserModelChanged -= OnViewingUserModelChanged;
    }

    private void OnViewingUserModelChanged(UserModel userModel)
    {
        HandleNewShopModel(userModel.ShopModel);
    }

    private void HandleNewShopModel(ShopModel viewingShopModel)
    {
        ForgetCurrentShopModel();

        _currentShopModel = viewingShopModel;

        _currentShopModel.BillboardModel.AvailabilityChanged += OnBillboardAvailabilityChanged;
        _currentShopModel.BillboardModel.TextChanged += OnBillboardTextChanged;

        DisplayBillboard();
    }

    private void ForgetCurrentShopModel()
    {
        if (_currentShopModel != null)
        {
            _currentShopModel.BillboardModel.AvailabilityChanged -= OnBillboardAvailabilityChanged;
            _currentShopModel.BillboardModel.TextChanged -= OnBillboardTextChanged;
            _currentShopModel = null;
        }
    }

    private void OnBillboardTextChanged()
    {
        DisplayBillboard();
    }

    private void OnBillboardAvailabilityChanged()
    {
        DisplayBillboard();
    }

    private void DisplayBillboard()
    {
        if (_currentShopModel.BillboardModel.IsAvailable == true)
        {
            if (_billboardView == null)
            {
                var go = GameObject.Instantiate(_prefabsHolder.BillboardPrefab, _contentTransform);
                go.transform.position = _gridCalculator.CellToWorld(_billboardCoords);
                _billboardView = go.GetComponent<BillboardView>();
            }

            _billboardView.gameObject.SetActive(true);
            _billboardView.SetText(_currentShopModel.BillboardModel.Text);
        }
        else
        {
            if (_billboardView != null)
            {
                _billboardView.gameObject.SetActive(false);
            }
        }
    }
}
