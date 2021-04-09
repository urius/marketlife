using System.Collections.Generic;
using UnityEngine;

public class ShopModel
{
    public readonly string Uid;
    public readonly ShoDesignModel ShopDesign;
    public readonly Dictionary<Vector2Int, ShopObjectBase> ShopObjects;

    public ShopModel(string uid, ShoDesignModel shopDesign, Dictionary<Vector2Int, ShopObjectBase> shopObjects)
    {
        Uid = uid;
        ShopDesign = shopDesign;
        ShopObjects = shopObjects;
    }
}

public class ShoDesignModel
{
    public int SizeX;
    public int SizeY;

    public readonly Dictionary<Vector2Int, int> Floors;
    public readonly Dictionary<Vector2Int, int> Walls;
    public readonly Dictionary<Vector2Int, int> Windows;
    public readonly Dictionary<Vector2Int, int> Doors;

    public ShoDesignModel(int sizeX, int sizeY,
        Dictionary<Vector2Int, int> floors,
        Dictionary<Vector2Int, int> walls,
        Dictionary<Vector2Int, int> doors,
        Dictionary<Vector2Int, int> windows)
    {
        SizeX = sizeX;
        SizeY = sizeY;

        Floors = floors;
        Walls = walls;
        Doors = doors;
        Windows = windows;
    }
}