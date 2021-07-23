using System;
using System.Collections.Generic;
using UnityEngine;

public class HumansMediator : MonoBehaviour
{
    private GameStateModel _gameStateModel;
    private Dictionary<CustomerModel, CustomerMediator> _mediatorsByModel = new Dictionary<CustomerModel, CustomerMediator>(Constants.MaxCostumersCount);
    private UserSessionDataModel _currentSessionModel;

    private void Awake()
    {
        _gameStateModel = GameStateModel.Instance;
    }

    private void Start()
    {
        Activate();

        if (_gameStateModel.ViewingUserModel != null)
        {
            HandleNewUserModel(_gameStateModel.ViewingUserModel);
        }
    }

    private void Activate()
    {
        _gameStateModel.ViewingUserModelChanged += OnViewingUserModelChanged;
    }

    private void OnViewingUserModelChanged(UserModel userModel)
    {
        HandleNewUserModel(userModel);
    }

    private void HandleNewUserModel(UserModel viewingUserModel)
    {
        Clear();

        DeactivateCurrentSessionModel();
        _currentSessionModel = viewingUserModel.SessionDataModel;
        ActivateCurrentSessionModel();

        foreach (var customerModel in _currentSessionModel.Customers)
        {
            MediateNewCustomer(customerModel);
        }
    }

    private void ActivateCurrentSessionModel()
    {
        _currentSessionModel.CustomerAdded += OnCustomerAdded;
        _currentSessionModel.CustomerRemoved += OnCustomerRemoved;
    }

    private void DeactivateCurrentSessionModel()
    {
        if (_currentSessionModel != null)
        {
            _currentSessionModel.CustomerAdded -= OnCustomerAdded;
            _currentSessionModel.CustomerRemoved -= OnCustomerRemoved;
        }
    }

    private void OnCustomerAdded(CustomerModel customerModel)
    {
        MediateNewCustomer(customerModel);
    }

    private void OnCustomerRemoved(CustomerModel customerModel)
    {
        _mediatorsByModel[customerModel].Unmediate();
        _mediatorsByModel.Remove(customerModel);
    }

    private void MediateNewCustomer(CustomerModel customerModel)
    {
        var mediator = new CustomerMediator(transform, customerModel);
        _mediatorsByModel[customerModel] = mediator;
        mediator.Mediate();
    }

    private void Clear()
    {
        foreach (var kvp in _mediatorsByModel)
        {
            kvp.Value.Unmediate();
        }
        _mediatorsByModel.Clear();
    }
}
