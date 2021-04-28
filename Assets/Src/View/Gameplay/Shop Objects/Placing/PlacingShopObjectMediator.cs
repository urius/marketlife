using System;
using UnityEngine;

public class PlacingShopObjectMediator : ShopObjectMediatorBase
{
    private readonly SpriteRenderer[] _buildMatrixSquares;
    private readonly GridCalculator _gridCalculator;

    public PlacingShopObjectMediator(Transform parentTransform, ShopObjectBase shelfModel)
        : base(parentTransform, shelfModel)
    {
        _gridCalculator = GridCalculator.Instance;

        var buildSquaresCount = Model.RotatedBuildMatrix[0].Length * Model.RotatedBuildMatrix.Length;
        _buildMatrixSquares = new SpriteRenderer[buildSquaresCount];
    }

    public override void Mediate()
    {
        base.Mediate();
        Activate();

        Model.RotatedBuildMatrix.ForEachElement(CreateBuildSquare);
        UpdateDisplayBuildMatrix();
    }

    public override void Unmediate()
    {
        Deactivate();
        base.Unmediate();
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
        Model.CoordsChanged += OnCoordsChanged;
        Model.SideChanged += OnSideChanged;
    }

    private void Deactivate()
    {
        Model.CoordsChanged -= OnCoordsChanged;
        Model.SideChanged -= OnSideChanged;      
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
        Model.RotatedBuildMatrix.ForEachElement(UpdateBuildSquare);
    }

    private void UpdateBuildSquare(Vector2Int localCoords, int flatIndex, int value)
    {
        var coords = Model.Coords + localCoords;

        _buildMatrixSquares[flatIndex].transform.position = _gridCalculator.CellToWord(coords);
        _buildMatrixSquares[flatIndex].color = _buildMatrixSquares[flatIndex].color.SetAlpha(Math.Abs(value) * 0.5f);
    }
}
