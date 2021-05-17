using System;
using UnityEngine;

public class PlacingShopObjectMediator : ShopObjectMediatorBase
{
    private readonly SpriteRenderer[] _buildMatrixSquares;
    private readonly GridCalculator _gridCalculator;
    private readonly GameStateModel _gameStateModel;

    private ShopModel _viewingShopModel;
    private ShopModel _currentViewingShopModel;

    public PlacingShopObjectMediator(Transform parentTransform, ShopObjectModelBase shelfModel)
        : base(parentTransform, shelfModel)
    {
        _gridCalculator = GridCalculator.Instance;
        _gameStateModel = GameStateModel.Instance;

        var buildSquaresCount = Model.RotatedBuildMatrix[0].Length * Model.RotatedBuildMatrix.Length;
        _buildMatrixSquares = new SpriteRenderer[buildSquaresCount];
    }

    public override void Mediate()
    {
        base.Mediate();

        Model.RotatedBuildMatrix.ForEachElement(CreateBuildSquare);
        UpdateDisplayBuildMatrix();

        Activate();
    }

    public override void Unmediate()
    {
        Deactivate();
        DestroyBuildSquares();
        base.Unmediate();
    }

    private void DestroyBuildSquares()
    {
        foreach (var buildSquare in _buildMatrixSquares)
        {
            GameObject.Destroy(buildSquare.gameObject);
        }
    }

    private void CreateBuildSquare(Vector2Int localCoords, int flatIndex, int value)
    {
        var squareGo = GameObject.Instantiate(PrefabsHolder.Instance.WhiteSquarePrefab, ParentTransform);
        var spriteRenderer = squareGo.GetComponent<SpriteRenderer>();
        spriteRenderer.color = spriteRenderer.color.SetAlpha(0.5f);
        _buildMatrixSquares[flatIndex] = spriteRenderer;
    }

    private void Activate()
    {
        _currentViewingShopModel = _gameStateModel.ViewingShopModel;

        _currentViewingShopModel.ShopObjectPlaced += OnShopObjectPlaced;
        Model.CoordsChanged += OnCoordsChanged;
        Model.SideChanged += OnSideChanged;
    }

    private void Deactivate()
    {
        _currentViewingShopModel.ShopObjectPlaced -= OnShopObjectPlaced;
        Model.CoordsChanged -= OnCoordsChanged;
        Model.SideChanged -= OnSideChanged;

        _currentViewingShopModel = null;
    }

    private void OnShopObjectPlaced(ShopObjectModelBase shopObjectModel)
    {
        UpdateDisplayBuildMatrix();
    }

    private void OnCoordsChanged(Vector2Int oldCoords, Vector2Int newCoords)
    {
        UpdateDisplayBuildMatrix();
    }

    private void OnSideChanged(int oldSide, int newSide)
    {
        UpdateDisplayBuildMatrix();
    }

    private void UpdateDisplayBuildMatrix()
    {
        _viewingShopModel = _gameStateModel.ViewingShopModel;
        Model.RotatedBuildMatrix.ForEachElement(UpdateBuildSquare);
    }

    private void UpdateBuildSquare(Vector2Int localCoords, int flatIndex, int value)
    {
        var coords = Model.Coords + localCoords;
        var buildState = _viewingShopModel.GetCellBuildState(coords);

        _buildMatrixSquares[flatIndex].transform.position = _gridCalculator.CellToWord(coords);
        _buildMatrixSquares[flatIndex].color = GetBuildColor(value, buildState);
    }

    private Color GetBuildColor(int selfBuildState, int otherBuildState)
    {
        if (selfBuildState == 0)
        {
            return new Color(0, 0, 0, 0);
        }

        if (BuildHelper.CanBuild(selfBuildState, otherBuildState))
        {
            return new Color(0, 1, 0, 0.4f);
        }
        else
        {
            return new Color(1, 0, 0, 0.4f);
        }
    }
}
