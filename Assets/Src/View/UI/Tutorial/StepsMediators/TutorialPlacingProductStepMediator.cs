using System.Collections.Generic;
using UnityEngine;

public class TutorialPlacingProductStepMediator : TutorialStepMediatorBase
{
    private readonly TutorialUIElementsProvider _tutorialUiElementsProvider;
    private readonly GameStateModel _gameStateModel;
    private readonly ShopModel _playerShopModel;
    private readonly PrefabsHolder _prefabsHolder;
    private readonly GridCalculator _gridCalculator;
    private readonly Dictionary<Vector2Int, TutorialArrowPointerView> _pointerViewsByCoords = new Dictionary<Vector2Int, TutorialArrowPointerView>();

    private ShelfModel[] _targetShelfModels;

    public TutorialPlacingProductStepMediator(RectTransform parentTransform)
        : base(parentTransform)
    {
        _tutorialUiElementsProvider = TutorialUIElementsProvider.Instance;
        _gameStateModel = GameStateModel.Instance;
        _playerShopModel = PlayerModelHolder.Instance.ShopModel;
        _prefabsHolder = PrefabsHolder.Instance;
        _gridCalculator = GridCalculator.Instance;
    }

    public override void Mediate()
    {
        base.Mediate();

        View.DisablePopup();
        SetupHighlight();
        _targetShelfModels = GetEmptyShelfModels();
        DisplayArrowPointers();

        Activate();
    }

    public override void Unmediate()
    {
        Deactivate();
        RemoveArrowPointers();

        base.Unmediate();
    }

    private void Activate()
    {
        foreach (var shelfModel in _targetShelfModels)
        {
            shelfModel.ProductIsSetOnSlot += OnShelfProductIsSetOnSlot;
        }
        _gameStateModel.PlacingStateChanged += OnPlacingStateChanged;
    }

    private void Deactivate()
    {
        foreach (var shelfModel in _targetShelfModels)
        {
            shelfModel.ProductIsSetOnSlot -= OnShelfProductIsSetOnSlot;
        }
        _gameStateModel.PlacingStateChanged -= OnPlacingStateChanged;
    }

    private void SetupHighlight()
    {
        var corners = new Vector3[4];
        View.RootRect.GetWorldCorners(corners);
        var rootTransformRect = View.RootRect.rect;
        var center = (corners[0] + corners[2]) * 0.5f;
        var size = new Vector2(rootTransformRect.width * 0.75f, rootTransformRect.height * 0.7f);
        View.HighlightScreenSquareArea(center, size, animated: true);
        AllowClickOnRectTransform(View.HighlightRect);
    }

    private void OnShelfProductIsSetOnSlot(ShelfModel shelfModel, int slotIndex)
    {
        RemoveArrowPointerSafeOn(shelfModel.Coords);
        HandleEndOfStep();
    }

    private void HandleEndOfStep()
    {
        foreach (var shelf in _targetShelfModels)
        {
            if (shelf.IsEmpty()) return;
        }
        DispatchTutorialActionPerformed();
    }

    private void RemoveArrowPointerSafeOn(Vector2Int coords)
    {
        if (_pointerViewsByCoords.ContainsKey(coords))
        {
            GameObject.Destroy(_pointerViewsByCoords[coords].gameObject);
            _pointerViewsByCoords.Remove(coords);
        }
    }

    private void OnPlacingStateChanged(PlacingStateName prevState, PlacingStateName currentState)
    {
        if (currentState == PlacingStateName.None)
        {
            DispatchTutorialActionPerformed();
        }
    }

    private void RemoveArrowPointers()
    {
        foreach (var shelfModel in _targetShelfModels)
        {
            RemoveArrowPointerSafeOn(shelfModel.Coords);
        }
    }

    private void DisplayArrowPointers()
    {
        var floorTranform = _tutorialUiElementsProvider.GetElementTransform(TutorialUIElement.ShopFloorTransform);
        foreach (var emptyShelfModel in _targetShelfModels)
        {
            var pointerGo = GameObject.Instantiate(_prefabsHolder.TutorialCellPointerPrefab, floorTranform);
            var pointerView = pointerGo.GetComponent<TutorialArrowPointerView>();
            pointerView.transform.position = _gridCalculator.CellToWorld(emptyShelfModel.Coords);
            _pointerViewsByCoords.Add(emptyShelfModel.Coords, pointerView);
        }
    }

    private ShelfModel[] GetEmptyShelfModels()
    {
        var result = new List<ShelfModel>(_playerShopModel.ShopObjects.Count);
        foreach (var shopObject in _playerShopModel.ShopObjects)
        {
            if (shopObject.Value.Type == ShopObjectType.Shelf)
            {
                var shelfModel = shopObject.Value as ShelfModel;
                if (shelfModel.IsEmpty())
                {
                    result.Add(shelfModel);
                }
            }
        }
        return result.ToArray();
    }
}
