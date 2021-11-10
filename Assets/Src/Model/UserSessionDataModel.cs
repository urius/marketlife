using System;
using System.Collections.Generic;
using UnityEngine;

public class UserSessionDataModel
{
    public event Action<CustomerModel> CustomerAdded = delegate { };
    public event Action<CustomerModel> CustomerRemoved = delegate { };

    public readonly Dictionary<ProductConfig, ProductInfoForBuy> ProductsData = new Dictionary<ProductConfig, ProductInfoForBuy>();

    public int SpawnCooldown = 0;

    private readonly List<CustomerModel> _customers = new List<CustomerModel>(Constants.MaxCustomersCount);
    private readonly Dictionary<Vector2Int, CustomerModel> _customersByCoords = new Dictionary<Vector2Int, CustomerModel>(Constants.MaxCustomersCount);

    public UserSessionDataModel()
    {
    }

    public IReadOnlyList<CustomerModel> Customers => _customers;

    public bool AddCustomer(CustomerModel model)
    {
        if (_customersByCoords.ContainsKey(model.Coords))
        {
            return false;
        }
        _customers.Add(model);
        _customersByCoords[model.Coords] = model;
        SubscribeForCustomer(model);
        CustomerAdded(model);

        return true;
    }

    public bool RemoveCustomer(CustomerModel model)
    {
        if (_customers.Remove(model))
        {
            _customersByCoords.Remove(model.Coords);
            UnsubscribeFromCustomer(model);
            CustomerRemoved(model);
            return true;
        }

        return false;
    }

    public bool HaveCustomerAt(Vector2Int coords)
    {
        return _customersByCoords.ContainsKey(coords);
    }

    public CustomerModel GetCustomerAt(Vector2Int coords)
    {
        _customersByCoords.TryGetValue(coords, out var result);
        return result;
    }

    private void SubscribeForCustomer(CustomerModel model)
    {
        model.CoordsChanged += OnCustomerCoordsChanged;
    }

    private void UnsubscribeFromCustomer(CustomerModel model)
    {
        model.CoordsChanged -= OnCustomerCoordsChanged;
    }

    private void OnCustomerCoordsChanged(PositionableObjectModelBase customerModel, Vector2Int previousCoords, Vector2Int currentCoords)
    {
        _customersByCoords.Remove(previousCoords);
        _customersByCoords[customerModel.Coords] = customerModel as CustomerModel;
    }
}
