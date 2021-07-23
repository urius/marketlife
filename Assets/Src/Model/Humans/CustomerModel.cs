using System.Collections.Generic;
using UnityEngine;

public class CustomerModel : PositionableObjectModelBase
{
    private static int[][] _matrix = new int[][] { new int[] { 1 } };

    private readonly List<ProductModel> _products = new List<ProductModel>(5);

    private int _hairId;
    private int _topClothesId;
    private int _bottomClothesId;
    private int _glassesId;
    private Mood _mood;
    private CustumerState _state;
    private Vector2Int[] _path;
    private int _pathStepIndex;

    public CustomerModel(
        int hairId,
        int topClothesId,
        int bottomClothesId,
        int glassesId,
        Mood mood,
        Vector2Int coords,
        int side)
        : base(_matrix, false, coords, side)
    {
        _hairId = hairId;
        _topClothesId = topClothesId;
        _bottomClothesId = bottomClothesId;
        _glassesId = glassesId;
        _mood = mood;
    }

    public int HairId => _hairId;
    public int TopClothesId => _topClothesId;
    public int BottomClothesId => _bottomClothesId;
    public int GlassesId => _glassesId;
    public Mood Mood => _mood;
    public CustumerState State => _state;

    public void SetPath(Vector2Int[] path)
    {
        _path = path;
        _pathStepIndex = 0;
        Coords = _path[_pathStepIndex];
    }

    public bool MakeStep()
    {
        if (_path != null && _path.Length > 0 && _pathStepIndex < _path.Length - 1)
        {
            _pathStepIndex++;
            Coords = _path[_pathStepIndex];
            return true;
        }
        return false;
    }
}

public enum CustumerState
{
    Idle,
    Moving,
    TakingProduct,
    Paying,
}

public enum Mood
{
    Default = 0,
    SmileNorm = 1,
    SmileLite = 2,
    SmileGood = 3,
    o_O = 4,
    EvilPlan = 5,
    EvilNorm = 6,
    EvilAngry = 7,
    Evil1 = 8,
    Dream1 = 9,
}
