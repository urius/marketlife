using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomerModel : PositionableObjectModelBase
{
    public event Action AnimationStateChanged = delegate { };

    private static int[][] _matrix = new int[][] { new int[] { 1 } };

    public CustomerLifetimeState LifetimeState;
    public int ShelfsVisited = 0;
    public Vector2Int TargetCell;

    private readonly List<ProductModel> _products = new List<ProductModel>(5);

    private int _hairId;
    private int _topClothesId;
    private int _bottomClothesId;
    private int _glassesId;
    private Mood _mood;
    private CustomerAnimationState _animationState;
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
    public CustomerAnimationState AnimationState => _animationState;
    public bool HaveUnwalkedPath => _path != null && _pathStepIndex < _path.Length - 1;
    public Vector2Int[] Path => _path;
    public IReadOnlyList<ProductModel> Products => _products;

    public void SetPath(Vector2Int[] path)
    {
        _path = path;
        _pathStepIndex = 0;
        Coords = _path[_pathStepIndex];
    }

    public bool TryGetNextStepCoords(out Vector2Int result)
    {
        result = Vector2Int.zero;
        if (HaveUnwalkedPath)
        {
            result = _path[_pathStepIndex + 1];
            return true;
        }

        return false;
    }

    public void AddProduct(ProductConfig config, int amount)
    {
        foreach (var product in _products)
        {
            if (product.Config.NumericId == config.NumericId)
            {
                product.Amount += amount;
                return;
            }
        }
        _products.Add(new ProductModel(config, amount));
    }

    public bool MakeStep()
    {
        if (HaveUnwalkedPath)
        {
            _pathStepIndex++;
            var newCoords = _path[_pathStepIndex];
            Side = SideHelper.VectorToSide(newCoords - Coords);
            Coords = newCoords;
            SetAnimationState(CustomerAnimationState.Moving);
            return true;
        }
        SetAnimationState(CustomerAnimationState.Idle);
        return false;
    }

    public void ToIdleState()
    {
        SetAnimationState(CustomerAnimationState.Idle);
    }

    public void ToThinkingState()
    {
        SetAnimationState(CustomerAnimationState.Thinking);
    }

    public void ToTakingProductState()
    {
        SetAnimationState(CustomerAnimationState.TakingProduct);
    }

    public void InsertNextStep(Vector2Int stepcoords)
    {
        var newpath = new Vector2Int[] { stepcoords }.Concat(_path.Take(_pathStepIndex)).ToArray();
        SetPath(newpath);
    }

    private void SetAnimationState(CustomerAnimationState newAnimationState)
    {
        if (_animationState != newAnimationState)
        {
            _animationState = newAnimationState;
            AnimationStateChanged();
        }
    }
}

public enum CustomerAnimationState
{
    Idle,
    Moving,
    Thinking,
    TakingProduct,
    Paying,
}

public enum CustomerLifetimeState
{
    Default,
    MovingToShelf,
    WatchingShelf,
    TakingProductFromShelf,
    MovingToCashDesk,
    PayingOnCashDesk,
    MovingToExit,
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
