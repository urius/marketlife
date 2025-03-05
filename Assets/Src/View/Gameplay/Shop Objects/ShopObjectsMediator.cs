using System;
using System.Collections.Generic;
using Src.Common;
using Src.Common_Utils;
using Src.Model;
using Src.Model.ShopObjects;
using Src.View.Gameplay.Shop_Objects.Placing;
using UnityEngine;

namespace Src.View.Gameplay.Shop_Objects
{
    public class ShopObjectsMediator : MonoBehaviour
    {
        private const int MaxTreesAmount = 35;

        private GameStateModel _gameStateModel;
        private PrefabsHolder _prefabsHolder;
        private GridCalculator _gridCalculator;
        private SpritesProvider _spritesProvider;

        //
        private readonly Vector2Int _billboardCoords = new Vector2Int(2, -4);
        private readonly Dictionary<Vector2Int, ShopObjectMediatorBase> _shopObjectMediators = new Dictionary<Vector2Int, ShopObjectMediatorBase>();
        private readonly List<SpriteRenderer> _treesList = new List<SpriteRenderer>(MaxTreesAmount);
        private readonly List<GameObject> _debugSquaresList = new List<GameObject>();//debug

        private PlacingShopObjectMediator _currentPlacingShopObjectMediator;
        private BillboardMediator _billboardMediator;
        private ShopModel _currentShopModel;
        private System.Random _random;

        private void Awake()
        {
            _gameStateModel = GameStateModel.Instance;
            _prefabsHolder = PrefabsHolder.Instance;
            _gridCalculator = GridCalculator.Instance;
            _spritesProvider = SpritesProvider.Instance;

            _billboardMediator = new BillboardMediator(transform, _billboardCoords);
        }

        private void Start()
        {
            Activate();

            if (_gameStateModel.ViewingShopModel != null)
            {
                HandleNewShopModel(_gameStateModel.ViewingShopModel);
            }

            _billboardMediator.Mediate();
        }

        private void HandleNewShopModel(ShopModel viewingShopModel)
        {
            ForgetCurrentShopModel();

            _currentShopModel = viewingShopModel;
            _currentShopModel.ShopObjectPlaced += OnShopObjectPlaced;
            _currentShopModel.ShopObjectRemoved += OnShopObjectRemoved;
            _currentShopModel.ShopObjectsChanged += OnShopObjectsChanged;
            _currentShopModel.ShopDesign.SizeXChanged += OnSizeXChanged;
            _currentShopModel.ShopDesign.SizeYChanged += OnSizeYChanged;

            _random = new System.Random(_gameStateModel.ViewingUserModel.RandomSeed);

            DisplayShopObjects(viewingShopModel.ShopObjects);
            DisplayTrees();
        }

        private void OnSizeXChanged(int prevValue, int currentValue)
        {
            DisplayTrees();
        }

        private void OnSizeYChanged(int prevValue, int currentValu)
        {
            DisplayTrees();
        }

        private void DisplayTrees()
        {
            const int treesPadding = 15;
            const int treesGap = 2;

            var amount = _random.Next(0, MaxTreesAmount);
            var shopDesignModel = _currentShopModel.ShopDesign;
            var createdAmount = 0;
            for (var i = 0; i < amount; i++)
            {
                var coords = new Vector2Int(
                    _random.Next(-treesPadding, shopDesignModel.SizeX + treesPadding),
                    _random.Next(-treesPadding, shopDesignModel.SizeY + treesPadding));
                if (IsNearBillboard(coords) == false
                    && (coords.x < -treesGap
                        || coords.y < -treesGap
                        || coords.x > shopDesignModel.SizeX + treesGap
                        || coords.y > shopDesignModel.SizeY + treesGap))
                {
                    SpriteRenderer treeRenderer;
                    if (_treesList.Count <= createdAmount)
                    {
                        var go = GameObject.Instantiate(_prefabsHolder.SimpleObjectPrefab, transform);
                        treeRenderer = go.GetComponent<SpriteRenderer>();
                        _treesList.Add(treeRenderer);
                    }
                    else
                    {
                        treeRenderer = _treesList[createdAmount];
                    }

                    treeRenderer.sortingLayerName = SortingLayers.OrderableOutside;
                    treeRenderer.sprite = DateTimeHelper.IsWinter() ? _spritesProvider.GetWinterTreeSprite() : _spritesProvider.GetTreeSprite();
                    treeRenderer.transform.position = _gridCalculator.CellToWorld(coords);
                    createdAmount++;
                }
            }

            while (_treesList.Count > createdAmount)
            {
                var lastIndex = _treesList.Count - 1;
                var item = _treesList[lastIndex];
                GameObject.Destroy(item.gameObject);
                _treesList.RemoveAt(lastIndex);
            }
        }

        private bool IsNearBillboard(Vector2Int coords)
        {
            const int distance = 2;
            return coords.x >= _billboardCoords.x - distance
                   && coords.x <= _billboardCoords.x + distance
                   && coords.y >= _billboardCoords.y - distance
                   && coords.y <= _billboardCoords.y + distance;
        }

        private void ForgetCurrentShopModel()
        {
            if (_currentShopModel != null)
            {
                _currentShopModel.ShopObjectPlaced -= OnShopObjectPlaced;
                _currentShopModel.ShopObjectRemoved -= OnShopObjectRemoved;
                _currentShopModel.ShopObjectsChanged -= OnShopObjectsChanged;
                _currentShopModel.ShopDesign.SizeXChanged -= OnSizeXChanged;
                _currentShopModel.ShopDesign.SizeYChanged -= OnSizeYChanged;
            }
        }

        private void OnShopObjectRemoved(ShopObjectModelBase shopObjectModel)
        {
            _shopObjectMediators[shopObjectModel.Coords].Unmediate();
            _shopObjectMediators.Remove(shopObjectModel.Coords);
        }

        private void OnShopObjectPlaced(ShopObjectModelBase shopObject)
        {
            MediateNewShopObject(shopObject);
        }

        private void Activate()
        {
            _gameStateModel.ViewingUserModelChanged += OnViewingUserModelChanged;
            _gameStateModel.ActionStateChanged += OnActionStateChanged;
        }

        private void OnActionStateChanged(ActionStateName previousState, ActionStateName newState)
        {
            switch (newState)
            {
                case ActionStateName.None:
                    if (_currentPlacingShopObjectMediator != null)
                    {
                        _currentPlacingShopObjectMediator.Unmediate();
                        _currentPlacingShopObjectMediator = null;
                    }
                    break;
                case ActionStateName.PlacingNewShopObject:
                case ActionStateName.MovingShopObject:
                    _currentPlacingShopObjectMediator = new PlacingShopObjectMediator(transform, _gameStateModel.PlacingShopObjectModel);
                    _currentPlacingShopObjectMediator.Mediate();
                    break;
            }
        }

        private void OnViewingUserModelChanged(UserModel userModel)
        {
            HandleNewShopModel(userModel.ShopModel);
        }

        private void DisplayShopObjects(Dictionary<Vector2Int, ShopObjectModelBase> newShopObjectsData)
        {
            ClearCurrentObjects();

            foreach (var kvp in newShopObjectsData)
            {
                MediateNewShopObject(kvp.Value);
            }

            DebugDisplayBuildSquares();
        }

        private void MediateNewShopObject(ShopObjectModelBase shopObjectModel)
        {
            var coords = shopObjectModel.Coords;
            _shopObjectMediators[coords] = CreateShopObjectMediator(shopObjectModel);
            _shopObjectMediators[coords].Mediate();
        }

        private void OnShopObjectsChanged()
        {

#if UNITY_EDITOR
            DebugDisplayBuildSquares();
#endif
        }

        private void DebugDisplayBuildSquares()
        {
#if UNITY_EDITOR
            return;

            foreach (var squareGo in _debugSquaresList)
            {
                GameObject.Destroy(squareGo);
            }
            _debugSquaresList.Clear();

            var viewingShopModel = GameStateModel.Instance.ViewingShopModel;
            foreach (var kvp in viewingShopModel.Grid)
            {
                var squareGo = GameObject.Instantiate(PrefabsHolder.Instance.WhiteSquarePrefab, transform);
                _debugSquaresList.Add(squareGo);
                squareGo.transform.position = GridCalculator.Instance.CellToWorld(kvp.Key);
                var sr = squareGo.GetComponent<SpriteRenderer>();
                var buildState = kvp.Value.buildState;
                sr.color = sr.color.SetRGBA((buildState == 1) ? 1 : 0, (buildState == 0) ? 1 : 0, (buildState == -1) ? 1 : 0, 0.5f);
            }
#endif
        }

        private ShopObjectMediatorBase CreateShopObjectMediator(ShopObjectModelBase shopObjectModel)
        {
            ShopObjectMediatorBase result;
            switch (shopObjectModel.Type)
            {
                case ShopObjectType.Shelf:
                    result = new ShelfViewMediator(transform, (ShelfModel)shopObjectModel);
                    break;
                case ShopObjectType.CashDesk:
                    result = new CashDeskViewMediator(transform, (CashDeskModel)shopObjectModel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shopObjectModel.Type), $"CreateMediator: Unsupported ShopObjectType: {shopObjectModel.Type}");
            }

            return result;
        }

        private void ClearCurrentObjects()
        {
            foreach (var kvp in _shopObjectMediators)
            {
                kvp.Value.Unmediate();
            }
            _shopObjectMediators.Clear();
        }
    }
}
