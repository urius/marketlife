using System;
using UnityEngine;

public class ShelfView : MonoBehaviour
{
    [SerializeField] private ProductFloor[] _productFloors;

    private void Awake()
    {

    }
}

[Serializable]
public class ProductFloor
{
    public Transform[] ProductPlaceholders;
}
