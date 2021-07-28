using System;
using System.Collections.Generic;
using UnityEngine;

public class UserSessionDataModel
{
    public event Action<CustomerModel> CustomerAdded = delegate { };
    public event Action<CustomerModel> CustomerRemoved = delegate { };

    public int SpawnCooldown = 0;

    private readonly List<CustomerModel> _customers = new List<CustomerModel>(Constants.MaxCostumersCount);
    private readonly Dictionary<Vector2Int, CustomerModel> _customersByCoords = new Dictionary<Vector2Int, CustomerModel>(Constants.MaxCostumersCount);

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
